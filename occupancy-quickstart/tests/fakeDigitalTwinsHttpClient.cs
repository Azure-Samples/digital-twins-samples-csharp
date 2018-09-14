using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public static class FakeDigitalTwinsHttpClient
    {
        public static Models.Space RootSpace = new Models.Space()
        {
            Name = "Space1",
            Id = new Guid("90000000-0000-0000-0000-000000000001").ToString(),
            Type = "Space1Type",
        };

        public static (HttpClient, FakeHttpHandler) Create(
            IEnumerable<Guid> postResponseGuids = null,
            IEnumerable<HttpResponseMessage> getResponses = null)
        {
            return FakeHttpHandler.CreateHttpClient(
                postResponses: postResponseGuids != null ? CreateGuidResponses(postResponseGuids) : null,
                getResponses: getResponses);
        }

        public static (HttpClient, FakeHttpHandler) CreateWithRootSpace(
            IEnumerable<Guid> postResponseGuids,
            IEnumerable<HttpResponseMessage> getResponses = null,
            Models.Space rootSpace = null)
        {
            getResponses = getResponses ?? Array.Empty<HttpResponseMessage>();
            rootSpace = rootSpace ?? RootSpace;

            var getRootSpaceResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new [] { rootSpace })),
            };
            return FakeHttpHandler.CreateHttpClient(
                postResponses: CreateGuidResponses(postResponseGuids),
                getResponses: new [] { getRootSpaceResponse }.Concat(getResponses) );
        }

        private static IEnumerable<HttpResponseMessage> CreateGuidResponses(IEnumerable<Guid> guids)
            => guids.Select(guid => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"\"{guid.ToString()}\""),
                });
    }
}