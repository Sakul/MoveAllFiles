using System.IO.Abstractions;
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

            // Search for all directories and work with atomic directories
            // Move all files to the root directory then delete the empty dirctories
            // Move temp files from root directory to the temp folder
        }
    }
}
