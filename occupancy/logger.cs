using System;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public interface Logger
    {
        void Write(string s);
        void WriteLine(string s);
    }
}