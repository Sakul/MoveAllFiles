using System;
using System.Collections.Generic;
using System.Text;

namespace MoveAllFiles
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string message) => Console.WriteLine(message);
    }
}
