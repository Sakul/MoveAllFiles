using FluentAssertions;
using FluentAssertions.Collections;
using MoveAllFiles.App;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace MoveAllFiles.Tests
{
    public class UnitTest1
    {
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
            var actual = sut.GetAllDirectoryPaths(@"c:\");
            var expectedDirectories = new List<string>
            {
                @"c:\a\",
                @"c:\b\",
                @"c:\a\a1\",
                @"c:\b\b1\",
                @"c:\b\b1\b12\"
            };
            Assert.Equal(expectedDirectories, actual);
        }

        [Fact(DisplayName = "When we move files from a directory, System should delete that directory after that.")]
        public void MoveAllFilesToRootDirectoryAndDeleteAllDirectories()
        {
            var fileSyste = new MockFileSystem();
            fileSyste.AddFile(@"c:\a\aQuery.js", new MockFileData("Some jquery"));
            fileSyste.AddFile(@"c:\a\aText.txt", new MockFileData("Some text."));

            const string RootDirectoryPath = @"c:\";
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
    }
}
