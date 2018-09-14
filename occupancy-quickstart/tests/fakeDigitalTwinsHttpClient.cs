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
        static public Models.Space RootSpace = new Models.Space()
        {
            Name = "Space1",
            Id = new Guid("90000000-0000-0000-0000-000000000001").ToString(),
            Type = "Space1Type",
        };

        static public (HttpClient, FakeHttpHandler) Create(
            IEnumerable<Guid> postResponseGuids = null,
            IEnumerable<HttpResponseMessage> getResponses = null)
        {
            return FakeHttpHandler.CreateHttpClient(
                postResponses: postResponseGuids != null ? CreateGuidResponses(postResponseGuids) : null,
                getResponses: getResponses);
        }

        static public (HttpClient, FakeHttpHandler) CreateWithRootSpace(
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

        static private IEnumerable<HttpResponseMessage> CreateGuidResponses(IEnumerable<Guid> guids)
            => guids.Select(guid => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"\"{guid.ToString()}\""),
                });
    }
}