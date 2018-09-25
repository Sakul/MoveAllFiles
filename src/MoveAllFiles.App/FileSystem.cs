using System.IO;

namespace MoveAllFiles
{
    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string path)
            => Directory.CreateDirectory(path);

        public void DeleteDirectory(string path)
            => Directory.Delete(path);

        public string[] GetDirectories(string path)
            => Directory.GetDirectories(path);

        public FileInfo GetFileInfo(string fileName)
            => new FileInfo(fileName);

        public string[] GetFiles(string path)
            => Directory.GetFiles(path);

        public bool IsFileExists(string path)
            => File.Exists(path);

        public void MoveFile(string sourceFilePath, string destinationFilePath)
            => File.Move(sourceFilePath, destinationFilePath);
    }
}
