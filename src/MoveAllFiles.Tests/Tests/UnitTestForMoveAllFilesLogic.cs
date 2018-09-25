using FluentAssertions;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace MoveAllFiles.Tests
{
    public class UnitTestForMoveAllFilesLogic
    {
        private const string RootDirectoryPath = @"c:\";

        [Fact(DisplayName = "System can list all directory paths.")]
        public void SystemCanListAllDirectoryPathsCorrectly()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"c:\myfile.txt", new MockFileData("Some text."));
            mockFileSystem.AddFile(@"c:\a\aQuery.js", new MockFileData("Some jquery"));
            mockFileSystem.AddFile(@"c:\a\a1\aQuery.js", new MockFileData("Some jquery"));
            mockFileSystem.AddFile(@"c:\b\aQuery.js", new MockFileData("Some jquery"));
            mockFileSystem.AddFile(@"c:\b\b1\b12\aQuery.js", new MockFileData("Some jquery"));

            var testFileSystem = new FileSystemForTesting(mockFileSystem);
            var sut = new MoveAllFilesLogic(testFileSystem);
            var actual = sut.GetAllDirectoryPaths(RootDirectoryPath);
            var expectedDirectories = new List<string>
            {
                @"c:\a\",
                @"c:\b\",
                @"c:\a\a1\",
                @"c:\b\b1\",
                @"c:\b\b1\b12\"
            };
            actual.Should().BeEquivalentTo(expectedDirectories, "The structure must like the mock.");
        }

        [Fact(DisplayName = "When we move files from a directory, System should delete that directory after that.")]
        public void MoveAllFilesToRootDirectoryAndDeleteAllDirectories()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"c:\a\aQuery.js", new MockFileData("Some jquery"));
            mockFileSystem.AddFile(@"c:\a\aText.txt", new MockFileData("Some text."));

            var testFileSystem = new FileSystemForTesting(mockFileSystem);
            var sut = new MoveAllFilesLogic(testFileSystem);
            sut.RootDirectoryPath = RootDirectoryPath;
            sut.MoveAllFilesToRootDirectoryAndDeleteIt(@"c:\a\");

            sut.GetAllDirectoryPaths(RootDirectoryPath)
                .Should()
                .BeEmpty("All directories must be deleted after we move their files.");

            var expectedFilePaths = new[]
            {
                @"c:\aQuery.js",
                @"c:\aText.txt",
            };
            sut.GetAllFilePaths(RootDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedFilePaths, "All files must be here.");
        }

        [Fact(DisplayName = "When we move files from a directory with existing file name, System should rename it.")]
        public void MoveAllFilesToRootDirectoryWithExistringFileName()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"c:\aText.txt", new MockFileData("Some text."));
            mockFileSystem.AddFile(@"c:\a\aText.txt", new MockFileData("Some text."));
            mockFileSystem.AddFile(@"c:\b\aText.txt", new MockFileData("Some text."));
            mockFileSystem.AddFile(@"c:\b\b1\aText.txt", new MockFileData("Some text."));

            var testFileSystem = new FileSystemForTesting(mockFileSystem);
            var sut = new MoveAllFilesLogic(testFileSystem);
            sut.RootDirectoryPath = RootDirectoryPath;
            sut.MoveAllFilesToRootDirectoryAndDeleteIt(@"c:\a\");
            sut.MoveAllFilesToRootDirectoryAndDeleteIt(@"c:\b\b1");
            sut.MoveAllFilesToRootDirectoryAndDeleteIt(@"c:\b\");

            sut.GetAllDirectoryPaths(RootDirectoryPath)
                .Should()
                .BeEmpty("All directories must be deleted after we move their files.");

            var expectedFilePaths = new[]
            {
                @"c:\aText.txt",
                @"c:\aText01.txt",
                @"c:\aText02.txt",
                @"c:\aText03.txt",
            };
            sut.GetAllFilePaths(RootDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedFilePaths, "All files must be here.");
        }

        [Theory(DisplayName = "System should move all out of whitelist files into temp directory.")]
        [InlineData]
        [InlineData(".txt")]
        [InlineData(".mp4")]
        [InlineData(".mp4", ".wmv")]
        public void SystemMoveAllOutOfWhitelistFilesToTempDirectory(params string[] whitelistExtensions)
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"c:\firstText.txt", new MockFileData("Some text."));
            foreach (var extension in whitelistExtensions)
                mockFileSystem.AddFile($"c:\\somefile{extension}", new MockFileData(new byte[] { 0x12 }));

            var allFilePaths = mockFileSystem.Directory.GetFiles(RootDirectoryPath).ToList();

            var testFileSystem = new FileSystemForTesting(mockFileSystem);
            var sut = new MoveAllFilesLogic(testFileSystem);
            sut.RootDirectoryPath = RootDirectoryPath;
            sut.WhitelistExtensionNames = whitelistExtensions;
            sut.MoveOutOfWhitelistFilesToTempFolder(RootDirectoryPath);

            var expectedWhitelistFilePaths = allFilePaths
                .Where(it => whitelistExtensions.Any(e => it.Contains(e)));
            sut.GetAllFilePaths(RootDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedWhitelistFilePaths, "All whitelist files must be here.");

            var tempDirectoryPath = $"{RootDirectoryPath}temp\\";
            var expectedOutOfWhitelistFilePaths = allFilePaths
                .Where(it => !whitelistExtensions.Any(e => it.Contains(e)))
                .Select(it => it.Replace(RootDirectoryPath, tempDirectoryPath));
            sut.GetAllFilePaths(tempDirectoryPath)
                .Should()
                .BeEquivalentTo(expectedOutOfWhitelistFilePaths, "All out of whitelist files must be here.");
        }

        [Theory(DisplayName = "System can handle all kind of working directory paths.")]
        [InlineData(@"c:\", @"c:\")]
        [InlineData(@"c:\download\", @"c:\download\")]
        [InlineData(@"c:\download", @"c:\download\")]
        public void ValidateRootDirectory(string rootDirPath, string expectedPath)
        {
            var mockFileSystem = new MockFileSystem();
            var testFileSystem = new FileSystemForTesting(mockFileSystem);
            var sut = new MoveAllFilesLogic(testFileSystem);
            sut.GetRootDirectoryPath(rootDirPath)
                .Should()
                .BeEquivalentTo(expectedPath);
        }
    }
}
