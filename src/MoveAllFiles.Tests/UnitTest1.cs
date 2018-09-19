using MoveAllFiles.App;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace MoveAllFiles.Tests
{
    public class UnitTest1
    {
        [Fact(DisplayName ="System can list all directory paths")]
        public void SystemCanListAllDirectoryPathsCorrectly()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Some text.") },
                { @"c:\a\aQuery.js", new MockFileData("Some jquery") },
                { @"c:\a\a1\aQuery.js", new MockFileData("Some jquery") },
                { @"c:\b\aQuery.js", new MockFileData("Some jquery") },
                { @"c:\b\b1\b12\aQuery.js", new MockFileData("Some jquery") },
            });

            var sut = new MoveAllFilesLogic(fileSystem);
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
    }
}
