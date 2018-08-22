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
        public LoggingHttpHandler()
            : base(new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Logging.LogRequest(request);

            var response = await base.SendAsync(request, cancellationToken);

            await Logging.LogResponse(response);

            return response;
        }
    }
}