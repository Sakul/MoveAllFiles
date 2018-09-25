using FluentAssertions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace MoveAllFiles.Tests
{
    public class TestMoveAllFilesLogic
    {
        [Fact(DisplayName = "Begin move all file in root directory, then system can move all files and handle temp files correctly.")]
        public void BeginMoveAllFilesInRootDirectory()
        {
            var workingDirectoryPath = @"c:\";
            var allFilePathsInSystem = new[]
            {
                @"c:\firstText.txt",
                @"c:\somefile01.jpg",
                @"c:\somefile02.mp4",
                @"c:\somefile03.avi",
            };
            var whitelistExtensions = new[] { ".mp4", ".avi" };
            var expectedWhitelistFilePaths = new[]
            {
                @"c:\somefile02.mp4",
                @"c:\somefile03.avi",
            };
            var expectedOutOfWhitelistFilePaths = new[]
            {
                @"c:\temp\firstText.txt",
                @"c:\temp\somefile01.jpg",
            };
            validateMoveFiles(workingDirectoryPath, allFilePathsInSystem, whitelistExtensions, expectedWhitelistFilePaths, expectedOutOfWhitelistFilePaths);
        }

        [Fact(DisplayName = "Begin move all file in other directory, then system can move all files and handle temp files correctly.")]
        public void BeginMoveAllFilesInOtherDirectory()
        {
            var workingDirectoryPath = @"c:\download\";
            var allFilePathsInSystem = new[]
            {
                @"c:\download\firstText.txt",
                @"c:\download\somefile01.jpg",
                @"c:\download\somefile02.mp4",
                @"c:\download\somefile03.avi",
            };
            var whitelistExtensions = new[] { ".mp4", ".avi" };
            var expectedWhitelistFilePaths = new[]
            {
                @"c:\download\somefile02.mp4",
                @"c:\download\somefile03.avi",
            };
            var expectedOutOfWhitelistFilePaths = new[]
            {
                @"c:\download\temp\firstText.txt",
                @"c:\download\temp\somefile01.jpg",
            };
            validateMoveFiles(workingDirectoryPath, allFilePathsInSystem, whitelistExtensions, expectedWhitelistFilePaths, expectedOutOfWhitelistFilePaths);
        }

        private void validateMoveFiles(string workingDirectoryPath, string[] allFilePathsInSystem, string[] whitelistExtensions, string[] expectedWhitelistFilePaths, string[] expectedOutOfWhitelistFilePaths)
        {
            var fileSyste = new MockFileSystem();
            foreach (var item in allFilePathsInSystem)
                fileSyste.AddFile(item, new MockFileData(new byte[] { 0x12 }));

            var sut = new MoveAllFilesLogic(fileSyste);
            sut.Begin(workingDirectoryPath, whitelistExtensions);

            sut.GetAllFilePaths(workingDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedWhitelistFilePaths, "All whitelist files must be here.");

            var tempDirectoryPath = $"{workingDirectoryPath}temp\\";

            sut.GetAllFilePaths(tempDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedOutOfWhitelistFilePaths, "All out of whitelist files must be here.");
        }
    }
}
