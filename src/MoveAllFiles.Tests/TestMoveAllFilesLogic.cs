using FluentAssertions;
using MoveAllFiles.App;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace MoveAllFiles.Tests
{
    public class TestMoveAllFilesLogic
    {
        private const string RootDirectoryPath = @"c:\";

        [Fact(DisplayName = "Begin move all file, then system can move all files and handle temp files correctly.")]
        public void BeginMoveAllFiles()
        {
            var fileSyste = new MockFileSystem();
            fileSyste.AddFile(@"c:\firstText.txt", new MockFileData("Some text."));
            fileSyste.AddFile(@"c:\somefile01.jpg", new MockFileData(new byte[] { 0x12 }));
            fileSyste.AddFile(@"c:\somefile02.mp4", new MockFileData(new byte[] { 0x12 }));
            fileSyste.AddFile(@"c:\somefile03.avi", new MockFileData(new byte[] { 0x12 }));

            var allFilePaths = fileSyste.Directory.GetFiles(RootDirectoryPath).ToList();

            var sut = new MoveAllFilesLogic(fileSyste);
            var whitelistExtensions = new[] { ".mp4", ".avi" };
            sut.Begin(RootDirectoryPath, whitelistExtensions);

            var expectedWhitelistFilePaths = new[]
            {
                @"c:\somefile02.mp4",
                @"c:\somefile03.avi",
            };
            sut.GetAllFilePaths(RootDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedWhitelistFilePaths, "All whitelist files must be here.");

            var tempDirectoryPath = $"{RootDirectoryPath}temp\\";
            var expectedOutOfWhitelistFilePaths = new[]
            {
                @"c:\temp\firstText.txt",
                @"c:\temp\somefile01.jpg",
            };
            sut.GetAllFilePaths(tempDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedOutOfWhitelistFilePaths, "All out of whitelist files must be here.");
        }
    }
}
