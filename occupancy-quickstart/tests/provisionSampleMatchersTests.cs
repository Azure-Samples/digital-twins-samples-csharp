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
    public class ProvisionSampleMatchersTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid matcher1Guid = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid matcher2Guid = new Guid("00000000-0000-0000-0000-000000000002");

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  matchers:
                  - name: Matcher1
                    dataTypeValue: dataType1
                  - name: Matcher2
                    dataTypeValue: dataType2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                matchers = new [] {
                    new MatcherDescription()
                    {
                        name = "Matcher1",
                        dataTypeValue = "dataType1",
                    },
                    new MatcherDescription()
                    {
                        name = "Matcher2",
                        dataTypeValue = "dataType2",
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoMatchers()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { matcher1Guid, matcher2Guid },
                getResponses: null
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                matchers = new [] {
                    new MatcherDescription()
                    {
                        name = "Matcher1",
                        dataTypeValue = "DataType1",
                    },
                    new MatcherDescription()
                    {
                        name = "Matcher2",
                        dataTypeValue = "DataType2",
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(2, httpHandler.PostRequests["matchers"].Count);
            Assert.False(httpHandler.GetRequests.ContainsKey("matchers"));
        }
    }
}