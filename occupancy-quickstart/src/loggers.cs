// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static class Loggers
    {
        public static ILogger SilentLogger =
            new Microsoft.Extensions.Logging.LoggerFactory()
                .CreateLogger("DigitalTwinsQuickstart");
        public static ILogger ConsoleLogger =
            new Microsoft.Extensions.Logging.LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger("DigitalTwinsQuickstart");
    }
}