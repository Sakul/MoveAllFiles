using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveAllFiles.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileSystem = new FileSystem();
            var logic = new MoveAllFilesLogic(fileSystem, new ConsoleLog());
            logic.Begin(Environment.CurrentDirectory, new[]
            {
                ".exe",
                ".3g2", ".3gp", ".aaf", ".asf",
                ".avchd", ".avi", ".drc", ".flv",
                ".m2v", ".m4p", ".m4v", ".mkv",
                ".mng", ".mov", ".mp2", ".mp4",
                ".mpe", ".mpeg", ".mpg", ".mpv",
                ".mxf", ".nsv", ".ogg", ".ogv",
                ".qt", ".rm", ".rmvb", ".roq",
                ".svi", ".vob", ".webm", ".wmv",
                ".yuv"
            });
        }
    }
}
