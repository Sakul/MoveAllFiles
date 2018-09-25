using System;
using System.IO;

namespace MoveAllFiles
{
    public class FileSystemForTesting : IFileSystem
    {
        private readonly System.IO.Abstractions.IFileSystem fileSystem;

        public FileSystemForTesting(System.IO.Abstractions.IFileSystem fileSystem)
            => this.fileSystem = fileSystem;

        public void CreateDirectory(string newDirectoryPath)
            => fileSystem.Directory.CreateDirectory(newDirectoryPath);

        public void DeleteDirectory(string directoryPath)
            => fileSystem.Directory.Delete(directoryPath);

        public string[] GetDirectories(string directoryPath)
            => fileSystem.Directory.GetDirectories(directoryPath);

        public FileInfo GetFileInfo(string filePath)
            => new FileInfo(filePath);

        public string[] GetFiles(string directoryPath)
            => fileSystem.Directory.GetFiles(directoryPath);

        public bool IsFileExists(string filePath)
            => fileSystem.File.Exists(filePath);

        public void MoveFile(string fromPath, string toPath)
            => fileSystem.File.Move(fromPath, toPath);
    }
}
