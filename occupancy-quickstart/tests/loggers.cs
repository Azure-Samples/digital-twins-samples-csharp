using Microsoft.Extensions.Logging;
using Moq;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public static class Loggers
    {
        public static ILogger SilentLogger = new Mock<ILogger>().Object;
        public static ILogger ConsoleLogger =
            new Microsoft.Extensions.Logging.LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger("DigitalTwinsQuickstartTests");
    }
}