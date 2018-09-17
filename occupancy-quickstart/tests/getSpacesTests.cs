using System;
using System.Linq;
using Xunit;
using Microsoft.Azure.DigitalTwins.Samples;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Moq;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public class GetSpacesTests
    {
        private static Models.Space space1 = new Models.Space()
        {
            Name = "Space1",
            Type = "Space1Type",
        };

        private static Models.Space space2 = new Models.Space()
        {
            Name = "Space2",
            Type = "Space2Type",
        };

        [Fact]
        public async Task GetSpacesWithFailedResponseReturnsEmptySpaceList()
        {
            (var httpClient, var _) = FakeHttpHandler.CreateHttpClient(
                postResponses: Enumerable.Repeat(Responses.NotFound, 1000),
                getResponses: Enumerable.Repeat(Responses.NotFound, 1000));
            Assert.Equal(0, (await Actions.GetSpaces(httpClient, Loggers.SilentLogger)).Count());
        }

        [Fact]
        public async Task GetSpacesWithResponseReturnsSpaces()
        {
            var expectedSpaces = new [] { space1, space2 };
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectedSpaces)),
            };
            (var httpClient, var _) = FakeHttpHandler.CreateHttpClient(
                getResponses: new [] { response });

            Assert.Equal(
                expectedSpaces.Select(x => x.Name),
                (await Actions.GetSpaces(httpClient, Loggers.SilentLogger)).Select(x => x.Name));
        }
    }
}