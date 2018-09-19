using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MoveAllFiles.Tests")]

namespace MoveAllFiles.App
{
    public class MoveAllFilesLogic
    {
        private readonly IFileSystem fileSystem;

        public string RootDirectoryPath { get; internal set; }

        public MoveAllFilesLogic(IFileSystem fileSystem) => this.fileSystem = fileSystem;

        public void Begin(string rootDirectoryPath)
        {
            RootDirectoryPath = rootDirectoryPath;

            // Search for all directorie paths
            // Move all files to the root directory then delete the dirctories
            // Move out of whitelist files from root directory to the temp folder
        }

        internal IEnumerable<string> GetAllDirectoryPaths(string rootDirectoryPath)
        {
            var paths = Enumerable.Empty<string>();
            var directoryPaths = fileSystem.Directory.GetDirectories(rootDirectoryPath);
            paths = paths.Union(directoryPaths);
            foreach (var path in directoryPaths)
            {
                var subDirectoriesPath = GetAllDirectoryPaths(path);
                paths = paths.Union(subDirectoriesPath);
            }
            return paths;
        }
    }
}
