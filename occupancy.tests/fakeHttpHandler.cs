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
    public class FakeHttpHandler : DelegatingHandler
    {
        static public HttpClient CreateHttpClient(IEnumerable<HttpResponseMessage> responses)
        {
            var httpHandler = new FakeHttpHandler()
            {
                Responses = responses,
            };
            return new HttpClient(httpHandler)
            {
                BaseAddress = new Uri("http://bing.com"),
            };
        }

        static public HttpClient CreateHttpClient(
            HttpResponseMessage response1,
            HttpResponseMessage response2 = null,
            HttpResponseMessage response3 = null)
        {
            var responses = new List<HttpResponseMessage>();
            if (response1 != null)
                responses.Add(response1);
            if (response2 != null)
                responses.Add(response2);
            if (response3 != null)
                responses.Add(response3);
            return CreateHttpClient(responses);
        }


        public FakeHttpHandler()
            : base(new HttpClientHandler())
        {
        }

        public IEnumerable<HttpResponseMessage> Responses { get; set; }
        private IEnumerator<HttpResponseMessage> _enumerateResponses;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_enumerateResponses == null)
                _enumerateResponses = Responses.GetEnumerator();

            if (_enumerateResponses == null || !_enumerateResponses.MoveNext())
                throw new InvalidOperationException("FakeHttpHandler ran out of responses");

            return Task.FromResult(_enumerateResponses.Current);
        }
    }
}