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
    public class ProvisionSampleKeyStoresTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid keyStoreGuid1 = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid keyStoreGuid2 = new Guid("00000000-0000-0000-0000-000000000002");

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  keystores:
                  - name: KeyStore1
                  - name: KeyStore2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                keystores = new [] {
                    new KeyStoreDescription()
                    {
                        name = "KeyStore1"
                    },
                    new KeyStoreDescription()
                    {
                        name = "KeyStore2"
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoKeyStores()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { keyStoreGuid1, keyStoreGuid2 },
                getResponses: null
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                keystores = new [] {
                    new KeyStoreDescription()
                    {
                        name = "KeyStore1",
                    },
                    new KeyStoreDescription()
                    {
                        name = "KeyStore2",
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(2, httpHandler.PostRequests["keystores"].Count);
            Assert.False(httpHandler.GetRequests.ContainsKey("keystores"));
        }
    }
}