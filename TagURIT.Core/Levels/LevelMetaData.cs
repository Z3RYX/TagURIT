using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TagURIT.Core.Levels
{
    public record LevelMetaData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public Uri? ThumbnailPath { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastUpdated { get; set; }
        public int Version { get; set; }

        public static bool ValidateMetaData(LevelMetaData data, out string? error)
        {
            var validImageFormats = new string[] { "png", "jpg", "jpeg", "bmp", "gif", "webp" };
            int maxImageFileSize = 1024 * 1024 * 10; // 10 Megabytes

            if (data == null)
            {
                error = "Data object cannot be null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.Name))
            {
                error = "Level name cannot be null, empty, or only consisting of whitespace characters.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.Description) && data.Description != null)
            {
                error = "Level description cannot be empty or only consisting of whitespace characters. Use null instead.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.Author))
            {
                error = "Author name cannot be null, empty, or only consisting of whitespace characters.";
                return false;
            }

            if (data.ThumbnailPath != null)
            {
                if (!File.Exists(data.ThumbnailPath.ToString()))
                {
                    error = $"Thumbnail does not exist at the specified path: {data.ThumbnailPath}";
                    return false;
                }

                if (!data.ThumbnailPath.IsFile)
                {
                    error = $"The specified path for the thumbnail does not point to a file: {data.ThumbnailPath}";
                    return false;
                }

                if (!validImageFormats.Contains(data.ThumbnailPath.AbsolutePath.Split('.').Last()))
                {
                    error = $"Thumbnail is not one of the allowed image file formats: " + string.Join(", ", validImageFormats);
                    return false;
                }

                if (new FileInfo(data.ThumbnailPath.ToString()).Length > maxImageFileSize)
                {
                    error = $"Thumbnail cannot be bigger than {maxImageFileSize} bytes. Current size: {new FileInfo(data.ThumbnailPath.ToString()).Length}";
                    return false;
                }

                //TODO: Verify image data and image aspect ratio (should be between 1:1 and 2:1)
            }

            if (data.CreationTime > DateTime.UtcNow)
            {
                error = "Creation time cannot be set in the future.";
                return false;
            }

            if (data.LastUpdated > DateTime.UtcNow)
            {
                error = "Time of last update cannot be set in the future.";
                return false;
            }

            if (data.LastUpdated < data.CreationTime)
            {
                error = "Time of last update cannot be before creation time.";
                return false;
            }

            if (data.Version < 1)
            {
                error = "Level version cannot be lower than 1";
                return false;
            }

            error = null;
            return true;
        }
    }
}
