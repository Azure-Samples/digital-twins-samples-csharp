// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        public static (HttpClient, FakeHttpHandler) CreateHttpClient(
            IEnumerable<HttpResponseMessage> postResponses = null,
            IEnumerable<HttpResponseMessage> getResponses = null,
            IEnumerable<HttpResponseMessage> patchResponses = null)
        {
            var httpHandler = new FakeHttpHandler()
            {
                PostResponses = postResponses,
                GetResponses = getResponses,
                PatchResponses = patchResponses,
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
            requests[HttpMethod.Post] = new Dictionary<string, List<HttpRequestMessage>>();
            requests[HttpMethod.Get] = new Dictionary<string, List<HttpRequestMessage>>();
            requests[HttpMethod.Patch] = new Dictionary<string, List<HttpRequestMessage>>();
        }

        public IReadOnlyDictionary<string, List<HttpRequestMessage>> PatchRequests => requests[HttpMethod.Patch];
        public IReadOnlyDictionary<string, List<HttpRequestMessage>> GetRequests => requests[HttpMethod.Get];
        public IReadOnlyDictionary<string, List<HttpRequestMessage>> PostRequests => requests[HttpMethod.Post];
        private Dictionary<HttpMethod, Dictionary<string, List<HttpRequestMessage>>> requests = new Dictionary<HttpMethod, Dictionary<string, List<HttpRequestMessage>>>();

        public IEnumerable<HttpResponseMessage> PostResponses { get; set; }
        private IEnumerator<HttpResponseMessage> enumeratePostResponses;

        public IEnumerable<HttpResponseMessage> GetResponses { get; set; }
        private IEnumerator<HttpResponseMessage> enumerateGetResponses;

        public IEnumerable<HttpResponseMessage> PatchResponses { get; set; }
        private IEnumerator<HttpResponseMessage> enumeratePatchResponses;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestName = request.RequestUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
            if (!requests[request.Method].ContainsKey(requestName))
                requests[request.Method].Add(requestName, new List<HttpRequestMessage>());
            requests[request.Method][requestName].Add(request);

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
            else if (request.Method == HttpMethod.Patch)
            {
                if (enumeratePatchResponses == null)
                    enumeratePatchResponses = PatchResponses.GetEnumerator();
                return enumeratePatchResponses;
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