using System;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class ConsoleLogger : Logger
    {
        public void Write(string s)
        {
            Console.Write(s);
        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }
    }
}