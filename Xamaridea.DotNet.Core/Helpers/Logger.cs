using System;

namespace Xamaridea.Core.Helpers
{
    public interface ILogger
    {
        void AppendLog(string format, params object[] args);
    }
    
    public class Logger : ILogger
    {
        public void AppendLog (string format, params object[] args)
        {
            Console.WriteLine (" > " + format, args);
        }
    }
}