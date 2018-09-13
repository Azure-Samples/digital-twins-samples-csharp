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
    // TODO: we should find something better than manually implementing this but I
    // haven't been able to yet:
    // See https://github.com/dotnet/corefx/issues/1624 for an example of non trivial discussion around this
    public class FakeHttpHandler : DelegatingHandler
    {
        static public (HttpClient, FakeHttpHandler) CreateHttpClient(
            IEnumerable<HttpResponseMessage> postResponses = null,
            IEnumerable<HttpResponseMessage> getResponses = null)
        {
            var httpHandler = new FakeHttpHandler()
            {
                PostResponses = postResponses,
                GetResponses = getResponses,
            };
            return (
                new HttpClient(httpHandler)
                {
                    BaseAddress = new Uri("http://bing.com"),
                },
                httpHandler);
        }

        public FakeHttpHandler()
            : base(new HttpClientHandler())
        {
            requests[HttpMethod.Post] = new List<HttpRequestMessage>();
            requests[HttpMethod.Get] = new List<HttpRequestMessage>();
        }

        public IReadOnlyList<HttpRequestMessage> PostRequests => requests[HttpMethod.Post];
        public IReadOnlyList<HttpRequestMessage> GetRequests => requests[HttpMethod.Get];
        private Dictionary<HttpMethod, List<HttpRequestMessage>> requests = new Dictionary<HttpMethod, List<HttpRequestMessage>>();

        public IEnumerable<HttpResponseMessage> PostResponses { get; set; }
        private IEnumerator<HttpResponseMessage> enumeratePostResponses;

        public IEnumerable<HttpResponseMessage> GetResponses { get; set; }
        private IEnumerator<HttpResponseMessage> enumerateGetResponses;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            requests[request.Method].Add(request);

            return Task.FromResult(GetNextResponse(ChooseResponseEnumerator(request)));
        }

        private static HttpResponseMessage GetNextResponse(IEnumerator<HttpResponseMessage> enumerator)
        {
            if (enumerator == null || !enumerator.MoveNext())
                throw new InvalidOperationException("FakeHttpHandler ran out of responses");
            return enumerator.Current;
        }

        private IEnumerator<HttpResponseMessage> ChooseResponseEnumerator(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get)
            {
                if (enumerateGetResponses == null)
                    enumerateGetResponses = GetResponses.GetEnumerator();
                return enumerateGetResponses;
            }
            else if (request.Method == HttpMethod.Post)
            {
                if (enumeratePostResponses == null)
                    enumeratePostResponses = PostResponses.GetEnumerator();
                return enumeratePostResponses;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}