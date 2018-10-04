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
    public class CreateEndpointsTests
    {
        private static Serializer yamlSerializer = new Serializer();

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - type: Type1
                  eventTypes:
                  - EventType1
                  - EventType2
                  connectionString: connectionString1
                  secondaryConnectionString: connectionString2
                  path: path1
                ";
            var expectedDescriptions = new [] { new EndpointDescription()
            {
                type = "Type1",
                eventTypes = new [] { "EventType1", "EventType2" },
                connectionString = "connectionString1",
                secondaryConnectionString = "connectionString2",
                path = "path1",
            }};
            var actualDescriptions = await Actions.GetCreateEndpointsDescriptions(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoEndpoints()
        {
            (var httpClient, var httpHandler) = FakeHttpHandler.CreateHttpClient(
                postResponses: new [] { Responses.OK, Responses.OK }
            );

            var descriptions = new [] { 
                new EndpointDescription()
                {
                    type = "Type1",
                    eventTypes = new [] { "EventType1", "EventType2" },
                    connectionString = "connectionString1",
                    secondaryConnectionString = "connectionString2",
                    path = "path1",
                },
                new EndpointDescription()
                {
                    type = "TypeA",
                    eventTypes = new [] { "EventTypeA", "EventTypeB" },
                    connectionString = "connectionStringA",
                    secondaryConnectionString = "connectionStringB",
                    path = "pathA",
                },
            };

            var result = await Actions.CreateEndpoints(httpClient, Loggers.SilentLogger, descriptions);
            Assert.Equal(2, httpHandler.PostRequests["endpoints"].Count);
            Assert.False(httpHandler.GetRequests.ContainsKey("endpoints"));
        }
    }
}