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
    public class ProvisionSampleSensorsTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid sensor1Guid = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid sensor2Guid = new Guid("00000000-0000-0000-0000-000000000002");

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  devices:
                  - name: Device1
                    hardwareId: DeviceHardwareId1
                    sensors:
                    - dataType: SensorType1
                      hardwareId: SensorHardwareId1
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                devices = new [] {
                    new DeviceDescription()
                    {
                        name = "Device1",
                        hardwareId = "DeviceHardwareId1",
                        sensors = new [] {
                            new SensorDescription()
                            {
                                dataType = "SensorType1",
                                hardwareId = "SensorHardwareId1",
                            }
                        },
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoSensors()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithDevice(
                postResponseGuids: new [] { sensor1Guid, sensor2Guid },
                getResponses: Enumerable.Repeat(Responses.NotFound, 2)
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                devices = new [] {
                    new DeviceDescription()
                    {
                        name = FakeDigitalTwinsHttpClient.Device.Name,
                        hardwareId = FakeDigitalTwinsHttpClient.Device.HardwareId,
                        sensors = new [] {
                            new SensorDescription()
                            {
                                dataType = "SensorType1",
                                hardwareId = "SensorHardwareId1",
                            },
                            new SensorDescription()
                            {
                                dataType = "SensorType2",
                                hardwareId = "SensorHardwareId2",
                            }
                        }
                    }
                },
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(2, httpHandler.PostRequests["sensors"].Count);
        }
    }
}