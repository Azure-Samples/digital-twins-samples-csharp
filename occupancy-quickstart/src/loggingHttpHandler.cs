// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        private ILogger logger;

        public LoggingHttpHandler(ILogger logger)
            : base(new HttpClientHandler())
        {
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LogRequest(request);

            var response = await base.SendAsync(request, cancellationToken);

            await LogResponse(response);

            return response;
        }

        private void LogRequest(HttpRequestMessage request)
        {
            logger.LogTrace($"Request: {request.Method} {request.RequestUri}");

            // logger.LogDebug($"More Info: {Serialize(request)}");
        }

        private async Task LogResponse(HttpResponseMessage response)
        {
            const int maxContentLength = 200;
            var content = await response.Content?.ReadAsStringAsync();

            var statusCode = (int)response.StatusCode;

            // Truncate responses if they are successful.
            if (statusCode < 400)
            {
                var contentMaxLength = content == null || content.Length < maxContentLength
                    ? content
                    : content.Substring(0, maxContentLength - 3) + "...";

                content = contentMaxLength == null ? "" : $", {contentMaxLength}";
            }

            logger.LogTrace($"Response Status: {(int)response.StatusCode}, {response.StatusCode} {content}");

            // Enable to get more details:
            // logger.LogTrace($"Full Response: {Serialize(response)}");
            // logger.LogTrace($"Full Response Content: {content}");
        }

        private static string Serialize(object o)
            => JsonConvert.SerializeObject(o, Formatting.Indented);
    }
}