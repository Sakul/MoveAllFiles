using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MoveAllFiles.Tests")]

namespace MoveAllFiles
{
    public class MoveAllFilesLogic
    {
        #region Fields

        private int _runningNo;
        private readonly IFileSystem _fileSystem;
        private readonly ILog _log;

        #endregion Fields

        #region Properties

        public string RootDirectoryPath { get; internal set; }
        public IEnumerable<string> WhitelistExtensionNames { get; set; } = Enumerable.Empty<string>();

        #endregion Properties

        #region Constructors

        public MoveAllFilesLogic(IFileSystem fileSystem, ILog log = null)
        {
            _fileSystem = fileSystem;
            _log = log;
        }

        #endregion Constructors

        #region Methods

        public void Begin(string rootDirectoryPath, IEnumerable<string> whitelistExtensionNames = null)
        {
            RootDirectoryPath = GetRootDirectoryPath(rootDirectoryPath);
            WhitelistExtensionNames = whitelistExtensionNames;
            GetAllDirectoryPaths(RootDirectoryPath)
                .Reverse()
                .ToList()
                .ForEach(path => MoveAllFilesToRootDirectoryAndDeleteIt(path));
            MoveOutOfWhitelistFilesToTempFolder(RootDirectoryPath);
        }

        internal string GetRootDirectoryPath(string rootDirectoryPath)
            => rootDirectoryPath.EndsWith(@"\") ? rootDirectoryPath : $"{rootDirectoryPath}\\";

        internal IEnumerable<string> GetAllDirectoryPaths(string directoryPath)
        {
            var paths = Enumerable.Empty<string>();
            var directoryPaths = _fileSystem.Directory.GetDirectories(directoryPath);
            paths = paths.Union(directoryPaths);
            foreach (var path in directoryPaths)
            {
                var subDirectoriesPath = GetAllDirectoryPaths(path);
                paths = paths.Union(subDirectoriesPath);
            }
            return paths;
        }

        internal IEnumerable<string> GetAllFilePaths(string directoryPath)
            => _fileSystem.Directory.GetFiles(directoryPath);

        internal void MoveAllFilesToRootDirectoryAndDeleteIt(string workingDirectoryPath)
        {
            writeLog($"Moving files from '{workingDirectoryPath}'");
            _fileSystem.Directory.GetFiles(workingDirectoryPath)
                .ToList()
                .ForEach(it => moveFile(it, RootDirectoryPath));
            _fileSystem.Directory.Delete(workingDirectoryPath);
            writeLog($" -> DONE");
        }

        internal void MoveOutOfWhitelistFilesToTempFolder(string directoryPath)
        {
            writeLog("Clear files in the root directory");
            var allFilePathQry = _fileSystem.Directory.GetFiles(directoryPath)
                .Select(it => new { _fileSystem.FileInfo.FromFileName(it).Extension, Path = it });
            var outOfListFilePathQry = allFilePathQry.Where(it => !string.IsNullOrEmpty(it.Extension))
                .Where(it => !WhitelistExtensionNames.Contains(it.Extension));
            var unknowExtensionFilePathQry = allFilePathQry.Where(it => string.IsNullOrEmpty(it.Extension));

            const string TempDirectoryName = "temp";
            var tempDirectoryPath = $"{RootDirectoryPath}{TempDirectoryName}\\";
            _fileSystem.Directory.CreateDirectory(tempDirectoryPath);

            outOfListFilePathQry.Union(unknowExtensionFilePathQry)
                .ToList()
                .ForEach(it => moveFile(it.Path, tempDirectoryPath));
            writeLog($" -> DONE");
            writeLog($"------------------------------");
        }

        private void moveFile(string srcPath, string descPath)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(srcPath);
            writeLog($" - moving file '{fileInfo.Name}'");
            var destinationPath = $"{descPath}{fileInfo.Name}";
            var isFileExisting = _fileSystem.File.Exists(destinationPath);
            if (isFileExisting) destinationPath = $"{descPath}{createNewFileName(fileInfo.Name, fileInfo.Extension)}";
            _fileSystem.File.Move(srcPath, destinationPath);
        }

        private string createNewFileName(string fullName, string extensionName)
            => $"{Path.GetFileNameWithoutExtension(fullName)}{string.Format("{0:00}", ++_runningNo)}{extensionName}";

        private void writeLog(string message)
            => _log?.WriteLog(message);

        #endregion Methods
    }
}
