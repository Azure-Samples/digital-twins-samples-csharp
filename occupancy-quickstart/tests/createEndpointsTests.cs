// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        private static Guid endpoint1Guid = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid endpoint2Guid = new Guid("00000000-0000-0000-0000-000000000002");

        [Fact]
        public async Task GetCreateEndpointsCreatesDescriptions()
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
            var endpointGuids = new[] { endpoint1Guid, endpoint2Guid };
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.Create(
                postResponseGuids: endpointGuids
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

            Assert.Equal(endpointGuids,
                await Actions.CreateEndpoints(httpClient, Loggers.SilentLogger, descriptions));
            Assert.Equal(2, httpHandler.PostRequests["endpoints"].Count);
            Assert.False(httpHandler.GetRequests.ContainsKey("endpoints"));
        }
    }
}