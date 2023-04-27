using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagURIT.Core.Levels;

namespace TagURIT.Core
{
    public class LevelCompiler
    {
        private const string FILE_EXT = "tab";
        private readonly string _TagPath;
        public string TagPath { get { return _TagPath; } }

        public LevelCompiler(string tagPath)
        {
            _TagPath = tagPath;
        }

        public void CompileLevel(LevelMetaData levelData)
        {
            if (!LevelMetaData.ValidateMetaData(levelData, out string? errorMsg)) throw new ArgumentException(errorMsg);

            //TODO: Save compiled level in temp folder and return that as a path or stream
            using var file = new FileStream($"{levelData.Name!.Replace(" ", "_")}.{FILE_EXT}", FileMode.Open);
            using var archive = new ZipArchive(file, ZipArchiveMode.Update);

            var metaFile = archive.CreateEntry("meta.ini");
            using (var writer = new StreamWriter(metaFile.Open()))
            {
                writer.WriteLine("levelname=" + levelData.Name.Trim());
                writer.WriteLine("description=" + (levelData.Description == null ? "" : levelData.Description.Trim()));
                writer.WriteLine("author=" + levelData.Author!.Trim());
                writer.WriteLine("created=" + levelData.CreationTime);
                writer.WriteLine("lastupdated=" + levelData.LastUpdated);
                writer.WriteLine("version=" + levelData.Version);
            }

            if (levelData.ThumbnailPath != null)
            {
                archive.CreateEntryFromFile(
                    levelData.ThumbnailPath.ToString(), // Source file
                    "thumbnail" + new FileInfo(levelData.ThumbnailPath.ToString()).Extension // Destination
                );
            }

            /* TODO:
             * Add level file to the archive
             * Check level file for all assets it depends on
             * Create list of assets that don't ship with the game
             * Add those assets to the archive
             */
        }
    }
}
