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
    public class ProvisionSampleDevicesTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid device1Guid = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid device2Guid = new Guid("00000000-0000-0000-0000-000000000002");
        private static Models.Device device1 = new Models.Device()
        {
            HardwareId = "HardwareId1",
            Id = device1Guid.ToString(),
            Name = "Device1",
        };
        private static Models.Device device2 = new Models.Device()
        {
            HardwareId = "HardwareId2",
            Id = device2Guid.ToString(),
            Name = "Device2",
        };
        private static HttpResponseMessage device1GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { device1 })),
        };
        private static HttpResponseMessage device2GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { device2 })),
        };

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  devices:
                  - name: Device1
                    hardwareId: HardwareId1
                  - name: Device2
                    hardwareId: HardwareId2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                devices = new [] {
                    new DeviceDescription()
                    {
                        name = "Device1",
                        hardwareId = "HardwareId1",
                    },
                    new DeviceDescription()
                    {
                        name = "Device2",
                        hardwareId = "HardwareId2",
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoDevices()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithRootSpace(
                postResponseGuids: new [] { device1Guid, device2Guid },
                getResponses: Enumerable.Repeat(Responses.NotFound, 2)
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.RootSpace.Name,
                devices = new [] {
                    new DeviceDescription()
                    {
                        name = "Device1",
                        hardwareId = "HardwareId1",
                    },
                    new DeviceDescription()
                    {
                        name = "Device2",
                        hardwareId = "HardwareId2",
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.ConsoleLogger, descriptions, Guid.Empty);
            Assert.Equal(2, httpHandler.PostRequests["devices"].Count);
            Assert.Equal(2, httpHandler.GetRequests["devices"].Count);
        }

        [Fact]
        public async Task CreateDeviceReusesMatchingPreexistingDevice()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithRootSpace(
                postResponseGuids: new [] { device1Guid, device2Guid },
                getResponses: new [] { device1GetResponse }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.RootSpace.Name,
                devices = new [] {
                    new DeviceDescription()
                    {
                        name = device1.Name,
                        hardwareId = device1.HardwareId,
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.ConsoleLogger, descriptions, Guid.Empty);
            Assert.False(httpHandler.PostRequests.ContainsKey("devices"));
            Assert.Equal(1, httpHandler.GetRequests["devices"].Count);
        }
    }
}