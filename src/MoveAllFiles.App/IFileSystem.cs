using System.IO;

namespace MoveAllFiles
{
    public interface IFileSystem
    {
        string[] GetDirectories(string path);
        string[] GetFiles(string path);
        void DeleteDirectory(string path);
        void CreateDirectory(string path);
        bool IsFileExists(string path);
        void MoveFile(string sourceFilePath, string destinationFilePath);
        FileInfo GetFileInfo(string fileName);
    }
}
