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
using System.IO;
using System.Collections;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public class ProvisionSampleResourcesTests
    {
        private static ILogger silentLogger = new Mock<ILogger>().Object;
        private static Serializer yamlSerializer = new Serializer();
        private static HttpResponseMessage notFoundResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
        };
        private static Guid guid1 = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid guid2 = new Guid("00000000-0000-0000-0000-000000000002");
        private static Guid guid3 = new Guid("00000000-0000-0000-0000-000000000003");
        private static Models.Space space1 = new Models.Space()
        {
            Name = "Space1",
            Id = guid1.ToString(),
            Type = "Space1Type",
        };

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  resources:
                  - type: Type1
                    region: Region1
                  - type: Type2
                    region: Region2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                resources = new [] {
                    new ResourceDescription()
                    {
                        region = "Region1",
                        type = "Type1",
                    },
                    new ResourceDescription()
                    {
                        region = "Region2",
                        type = "Type2",
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateSingleResource()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithRootSpace(new [] { guid1 });
               
            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.RootSpace.Name,
                resources = new [] { new ResourceDescription()
                {
                    type = "ResourceType",
                }},
            }};

            var createdIds = await Actions.CreateSpaces(httpClient, silentLogger, descriptions, Guid.Empty);
            Assert.Equal(1, httpHandler.PostRequests.Count); // post Resource
            Assert.Equal(1, httpHandler.GetRequests.Count); // get Space
        }
    }
}