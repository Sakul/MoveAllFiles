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
            var fileSyste = new MockFileSystem();
            fileSyste.AddFile(@"c:\myfile.txt", new MockFileData("Some text."));
            fileSyste.AddFile(@"c:\a\aQuery.js", new MockFileData("Some jquery"));
            fileSyste.AddFile(@"c:\a\a1\aQuery.js", new MockFileData("Some jquery"));
            fileSyste.AddFile(@"c:\b\aQuery.js", new MockFileData("Some jquery"));
            fileSyste.AddFile(@"c:\b\b1\b12\aQuery.js", new MockFileData("Some jquery"));

            var sut = new MoveAllFilesLogic(fileSyste);
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
            var fileSyste = new MockFileSystem();
            fileSyste.AddFile(@"c:\a\aQuery.js", new MockFileData("Some jquery"));
            fileSyste.AddFile(@"c:\a\aText.txt", new MockFileData("Some text."));

            var sut = new MoveAllFilesLogic(fileSyste);
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
            var fileSyste = new MockFileSystem();
            fileSyste.AddFile(@"c:\aText.txt", new MockFileData("Some text."));
            fileSyste.AddFile(@"c:\a\aText.txt", new MockFileData("Some text."));
            fileSyste.AddFile(@"c:\b\aText.txt", new MockFileData("Some text."));
            fileSyste.AddFile(@"c:\b\b1\aText.txt", new MockFileData("Some text."));

            var sut = new MoveAllFilesLogic(fileSyste);
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
            var fileSyste = new MockFileSystem();
            fileSyste.AddFile(@"c:\firstText.txt", new MockFileData("Some text."));
            foreach (var extension in whitelistExtensions)
                fileSyste.AddFile($"c:\\somefile{extension}", new MockFileData(new byte[] { 0x12 }));

            var allFilePaths = fileSyste.Directory.GetFiles(RootDirectoryPath).ToList();

            var sut = new MoveAllFilesLogic(fileSyste);
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
            var fileSyste = new MockFileSystem();
            var sut = new MoveAllFilesLogic(fileSyste);
            sut.GetRootDirectoryPath(rootDirPath)
                .Should()
                .BeEquivalentTo(expectedPath);
        }
    }
}
