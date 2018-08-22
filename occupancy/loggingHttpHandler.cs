using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        private Logger _logger;

        public LoggingHttpHandler(Logger logger)
            : base(new HttpClientHandler())
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LogRequest(request);

            var response = await base.SendAsync(request, cancellationToken);

            await LogResponse(response);

            return response;
        }

        enum LogLevel {
            Normal,
            Verbose
        }

        private static LogLevel _level = LogLevel.Normal;

        private void LogRequest(HttpRequestMessage request)
        {
            if (_level == LogLevel.Verbose)
            {
                _logger.WriteLine($"Request: {Serialize(request)}");
            }
            else
            {
                _logger.WriteLine($"Request: {request.Method} {request.RequestUri}");
            }
        }

        private async Task LogResponse(HttpResponseMessage response)
        {
            if (_level == LogLevel.Verbose)
            {
                _logger.WriteLine($"Response: {Serialize(response)}");
                var content = await response.Content?.ReadAsStringAsync();
                _logger.WriteLine($"Response Content: {content}");
            }
            else
            {
                const int maxContentLength = 200;
                var content = await response.Content?.ReadAsStringAsync();
                var contentMaxLength = content == null || content.Length < maxContentLength
                    ? content
                    : content.Substring(0, maxContentLength - 3) + "...";
                var contentDisplay = contentMaxLength == null ? "" : $", {contentMaxLength}";
                _logger.WriteLine($"Response Status: {(int)response.StatusCode}, {response.StatusCode}{contentDisplay}");
            }
        }

        private static string Serialize(object o)
            => JsonConvert.SerializeObject(o, Formatting.Indented);
    }
}