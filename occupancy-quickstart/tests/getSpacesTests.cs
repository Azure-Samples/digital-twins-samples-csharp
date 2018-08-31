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
        private static HttpResponseMessage notFoundResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
        };

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

        private static ILogger silentLogger = new Mock<ILogger>().Object;

        [Fact]
        public async Task GetSpacesWithFailedResponseReturnsEmptySpaceList()
        {
            (var httpClient, var _) = FakeHttpHandler.CreateHttpClient(
                postResponses: Enumerable.Repeat(notFoundResponse, 1000),
                getResponses: Enumerable.Repeat(notFoundResponse, 1000));
            Assert.Equal(0, (await Actions.GetSpaces(httpClient, silentLogger)).Count());
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
                (await Actions.GetSpaces(httpClient, silentLogger)).Select(x => x.Name));
        }
    }
}