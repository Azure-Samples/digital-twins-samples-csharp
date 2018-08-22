using System;
using System.Linq;
using Xunit;
using Microsoft.Azure.DigitalTwins.Samples;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public class GetSpacesTests
    {
        private static HttpResponseMessage _notFoundResponse = new HttpResponseMessage()
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

        [Fact]
        public async Task GetSpacesWithFailedResponseReturnsEmptySpaceList()
        {
            var httpClient = FakeHttpHandler.CreateHttpClient(_notFoundResponse);
            Assert.Equal(0, (await Actions.GetSpaces(httpClient)).Count());
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
            var httpClient = FakeHttpHandler.CreateHttpClient(response);

            Assert.Equal(
                expectedSpaces.Select(x => x.Name),
                (await Actions.GetSpaces(httpClient)).Select(x => x.Name));
        }
    }
}