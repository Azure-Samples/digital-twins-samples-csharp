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
    public class ProvisionSampleSpacesTests
    {
        private static Serializer yamlSerializer = new Serializer();
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
                  type: TestType1
                  spaces:
                  - name: Child1
                  - name: Child2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                type = "TestType1",
                spaces = new [] {
                    new SpaceDescription()
                    {
                        name = "Child1",
                    },
                    new SpaceDescription()
                    {
                        name = "Child2",
                    }},
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateSpacesWithNoDescriptionsReturnsEmptyAndMakesNoRequests()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.Create();

            var results = await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, Array.Empty<SpaceDescription>(), Guid.Empty);

            Assert.Equal(0, results.Count());
            Assert.False(httpHandler.PostRequests.ContainsKey("spaces"));
            Assert.False(httpHandler.GetRequests.ContainsKey("spaces"));
        }

        [Fact]
        public async Task CreateSpacesWithSingleSpaceMakesRequestsAndReturnsRootId()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.Create(
                postResponseGuids: new [] { guid1 },
                getResponses: Enumerable.Repeat(Responses.NotFound, 1000));

            var descriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
            }};

            var results = await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(guid1, results.Single().Id);
            Assert.Equal(1, httpHandler.PostRequests["spaces"].Count);
            Assert.Equal(1, httpHandler.GetRequests["spaces"].Count);
        }

        [Fact]
        public async Task CreateSpacesWithAlreadyCreatedSpaceUsesIt()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { guid1 },
                space: space1);

            var descriptions = new [] { new SpaceDescription()
            {
                name = space1.Name,
            }};

            var results = await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(guid1, results.Single().Id);
            Assert.False(httpHandler.PostRequests.ContainsKey("spaces"));
            Assert.Equal(1, httpHandler.GetRequests["spaces"].Count);
        }

        [Fact]
        public async Task CreateSpacesWithSingleRootAndChildrenMakesRequestsAndReturnsRootId()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.Create(
                postResponseGuids: new [] { guid1, guid2, guid3 },
                getResponses: Enumerable.Repeat(Responses.NotFound, 1000));

            var descriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                spaces = new [] {
                    new SpaceDescription()
                    {
                        name = "Child1",
                    },
                    new SpaceDescription()
                    {
                        name = "Child2",
                    }},
            }};

            var results = await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(guid1, results.Single().Id);
            Assert.Equal(3, httpHandler.PostRequests["spaces"].Count);
            Assert.Equal(3, httpHandler.GetRequests["spaces"].Count);
        }

        [Fact]
        public async Task CreateSpacesWithMultipleRootsMakesRequestsAndReturnsAllRootIds()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.Create(
                postResponseGuids: new [] { guid1, guid2 },
                getResponses: Enumerable.Repeat(Responses.NotFound, 1000));

            var descriptions = new []
            {
                new SpaceDescription()
                {
                    name = "Test1",
                },
                new SpaceDescription()
                {
                    name = "Test2",
                }
            };

            var results = await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(new [] { guid1, guid2 }, results.Select(r => r.Id));
            Assert.Equal(2, httpHandler.PostRequests["spaces"].Count);
            Assert.Equal(2, httpHandler.GetRequests["spaces"].Count);
        }
    }
}