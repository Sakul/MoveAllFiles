using System;
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
        public IEnumerable<string> WhitelistExtensionNames { get; set; }

        public MoveAllFilesLogic(IFileSystem fileSystem) => this.fileSystem = fileSystem;

        public void Begin(string rootDirectoryPath, IEnumerable<string> whitelistExtensionNames = null)
        {
            RootDirectoryPath = rootDirectoryPath;
            WhitelistExtensionNames = whitelistExtensionNames ?? Enumerable.Empty<string>();

            // Search for all directorie paths
            // Move all files to the root directory then delete the dirctories
            // Move out of whitelist files from root directory to the temp folder
        }

        internal IEnumerable<string> GetAllDirectoryPaths(string directoryPath)
        {
            var paths = Enumerable.Empty<string>();
            var directoryPaths = fileSystem.Directory.GetDirectories(directoryPath);
            paths = paths.Union(directoryPaths);
            foreach (var path in directoryPaths)
            {
                var subDirectoriesPath = GetAllDirectoryPaths(path);
                paths = paths.Union(subDirectoriesPath);
            }
            return paths;
        }

        internal IEnumerable<string> GetAllFilePaths(string directoryPath) 
            => fileSystem.Directory.GetFiles(directoryPath);

        internal void MoveAllFilesToRootDirectoryAndDeleteIt(string workingDirectoryPath)
        {
            var filePaths = fileSystem.Directory.GetFiles(workingDirectoryPath);
            foreach (var path in filePaths)
            {
                var fileInfo = fileSystem.FileInfo.FromFileName(path);
                var destinationPath = $"{RootDirectoryPath}{fileInfo.Name}";
                //var isFileExisting = fileSystem.File.Exists(destinationPath);
                //if (isFileExisting) destinationPath = $"{RootDirectoryPath}{fileInfo.Name.Split(".", StringSplitOptions.RemoveEmptyEntries)[0]}{Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0]}{fileInfo.Extension}";
                fileSystem.File.Move(path, destinationPath);
            }
            
fileSystem.Directory.Delete(workingDirectoryPath);
        }

    }
}
