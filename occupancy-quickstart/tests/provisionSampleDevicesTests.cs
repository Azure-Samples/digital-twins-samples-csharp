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
            ConnectionString = "ConnectionString1",
        };
        private static Models.Device device2 = new Models.Device()
        {
            HardwareId = "HardwareId2",
            Id = device2Guid.ToString(),
            Name = "Device2",
            ConnectionString = "ConnectionString2",
        };
        private static HttpResponseMessage getDevicesResponse_device1 = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { device1 })),
        };
        private static HttpResponseMessage getDeviceResponse_device1 = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(device1)),
        };
        private static HttpResponseMessage getDevicesResponse_device2 = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { device2 })),
        };
        private static HttpResponseMessage getDeviceResponse_device2 = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(device2)),
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
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { device1Guid, device2Guid },
                getResponses: new [] { Responses.NotFound, getDeviceResponse_device1, Responses.NotFound, getDeviceResponse_device2 }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
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

            var spaceResult = (await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty))
                            .Single();
            Assert.Equal(2, httpHandler.PostRequests["devices"].Count);
            Assert.Equal(4, httpHandler.GetRequests["devices"].Count);
            Assert.Equal(new [] {
                new ProvisionResults.Device () {
                    ConnectionString = "ConnectionString1",
                    HardwareId = "HardwareId1",
                },
                new ProvisionResults.Device () {
                    ConnectionString = "ConnectionString2",
                    HardwareId = "HardwareId2",
                }},
                spaceResult.Devices);
        }

        [Fact]
        public async Task CreateDeviceReusesMatchingPreexistingDevice()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: null,
                getResponses: new [] { getDevicesResponse_device1, getDeviceResponse_device1 }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                devices = new [] {
                    new DeviceDescription()
                    {
                        name = device1.Name,
                        hardwareId = device1.HardwareId,
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.False(httpHandler.PostRequests.ContainsKey("devices"));
            Assert.Equal(2, httpHandler.GetRequests["devices"].Count);
        }
    }
}