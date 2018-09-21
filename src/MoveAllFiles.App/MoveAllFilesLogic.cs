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
        private int _runningNo;
        private readonly IFileSystem fileSystem;

        public string RootDirectoryPath { get; internal set; }
        public IEnumerable<string> WhitelistExtensionNames { get; set; } = Enumerable.Empty<string>();

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
            fileSystem.Directory.GetFiles(workingDirectoryPath)
                .ToList()
                .ForEach(it => moveFile(it, RootDirectoryPath));
            fileSystem.Directory.Delete(workingDirectoryPath);
        }

        internal void MoveOutOfWhitelistFilesToTempFolder(string directoryPath)
        {
            var allFilePathQry = fileSystem.Directory.GetFiles(directoryPath)
                .Select(it => new { fileSystem.FileInfo.FromFileName(it).Extension, Path = it });
            var outOfListFilePathQry = allFilePathQry.Where(it => !string.IsNullOrEmpty(it.Extension))
                .Where(it => !WhitelistExtensionNames.Contains(it.Extension));
            var unknowExtensionFilePathQry = allFilePathQry.Where(it => string.IsNullOrEmpty(it.Extension));

            const string TempDirectoryName = "temp";
            var tempDirectoryPath = $"{RootDirectoryPath}{TempDirectoryName}\\";
            fileSystem.Directory.CreateDirectory(tempDirectoryPath);

            outOfListFilePathQry.Union(unknowExtensionFilePathQry)
                .ToList()
                .ForEach(it => moveFile(it.Path, tempDirectoryPath));
        }

        private void moveFile(string srcPath, string descPath)
        {
            var fileInfo = fileSystem.FileInfo.FromFileName(srcPath);
            var destinationPath = $"{descPath}{fileInfo.Name}";
            var isFileExisting = fileSystem.File.Exists(destinationPath);
            if (isFileExisting) destinationPath = $"{descPath}{createNewFileName(fileInfo.Name, fileInfo.Extension)}";
            fileSystem.File.Move(srcPath, destinationPath);
        }

        private string createNewFileName(string fullName, string extensionName)
            => $"{getFileName(fullName)}{string.Format("{0:00}", ++_runningNo)}{extensionName}";

        private string getFileName(string fullName)
            => fullName.Split(".", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }
}
