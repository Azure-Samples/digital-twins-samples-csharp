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
    public class ProvisionSampleResourcesTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid resource1Guid = new Guid("00000000-0000-0000-0000-000000000001");
        private static Models.Resource resource1 = new Models.Resource()
        {
            Id = resource1Guid.ToString(),
            Status = "Something",
        };
        private static HttpResponseMessage resource1GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(resource1)),
        };

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  resources:
                  - type: Type1
                  - type: Type2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                resources = new [] {
                    new ResourceDescription()
                    {
                        type = "Type1",
                    },
                    new ResourceDescription()
                    {
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
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { resource1Guid },
                getResponses: new [] { resource1GetResponse });

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                resources = new [] { new ResourceDescription()
                {
                    type = "ResourceType",
                }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(1, httpHandler.PostRequests["resources"].Count);
            Assert.Equal(1, httpHandler.GetRequests["resources"].Count);
        }
    }
}