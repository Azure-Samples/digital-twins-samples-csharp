using System;
using System.Linq;
using Xunit;
using Microsoft.Azure.DigitalTwins.Samples;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Moq;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public class GetSpacesTests
    {
        private static HttpResponseMessage _notFoundResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
        };

        private static Models.Space _space1 = new Models.Space()
        {
            Name = "Space1",
            Type = "Space1Type",
        };

        private static Models.Space _space2 = new Models.Space()
        {
            Name = "Space2",
            Type = "Space2Type",
        };

        private static ILogger _silentLogger = new Mock<ILogger>().Object;

        [Fact]
        public async Task GetSpacesWithFailedResponseReturnsEmptySpaceList()
        {
            var httpClient = FakeHttpHandler.CreateHttpClient(_notFoundResponse);
            Assert.Equal(0, (await Actions.GetSpaces(httpClient, _silentLogger)).Count());
        }

        [Fact]
        public async Task GetSpacesWithResponseReturnsSpaces()
        {
            var expectedSpaces = new [] { _space1, _space2 };
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectedSpaces)),
            };
            var httpClient = FakeHttpHandler.CreateHttpClient(response);

            Assert.Equal(
                expectedSpaces.Select(x => x.Name),
                (await Actions.GetSpaces(httpClient, _silentLogger)).Select(x => x.Name));
        }
    }
}