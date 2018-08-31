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
    public class ProvisionSampleTests
    {
        private static ILogger _silentLogger = new Mock<ILogger>().Object;
        private static Serializer _yamlSerializer = new Serializer();
        private static HttpResponseMessage _notFoundResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
        };
        private static Guid _guid1 = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid _guid2 = new Guid("00000000-0000-0000-0000-000000000002");
        private static Guid _guid3 = new Guid("00000000-0000-0000-0000-000000000003");
        private static Models.Space _space1 = new Models.Space()
        {
            Name = "Space1",
            Id = _guid1.ToString(),
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
            Assert.Equal(_yamlSerializer.Serialize(expectedDescriptions), _yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateSpacesWithNoDescriptionsReturnsEmptyAndMakesNoRequests()
        {
            (var httpClient, var httpHandler) = FakeHttpHandler.CreateHttpClient();

            var createdIds = await Actions.CreateSpaces(httpClient, _silentLogger, new SpaceDescription[0], Guid.Empty);

            Assert.Equal(0, createdIds.Count());
            Assert.Equal(0, httpHandler.PostRequests.Count);
            Assert.Equal(0, httpHandler.GetRequests.Count);
        }

        [Fact]
        public async Task CreateSpacesWithSingleSpaceMakesRequestsAndReturnsRootId()
        {
            (var httpClient, var httpHandler) = FakeHttpHandler.CreateHttpClient(
                postResponses: CreateGuidResponses(new [] { _guid1 }),
                getResponses: Enumerable.Repeat(_notFoundResponse, 1000));
            var descriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
            }};

            var createdIds = await Actions.CreateSpaces(httpClient, _silentLogger, descriptions, Guid.Empty);
            Assert.Equal(_guid1, createdIds.Single());
            Assert.Equal(1, httpHandler.PostRequests.Count);
            Assert.Equal(1, httpHandler.GetRequests.Count);
        }

        [Fact]
        public async Task CreateSpacesWithAlreadyCreatedSpaceUsesIt()
        {
            var getResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new [] { _space1 })),
            };
            (var httpClient, var httpHandler) = FakeHttpHandler.CreateHttpClient(
                postResponses: CreateGuidResponses(new [] { _guid1 }),
                getResponses: new [] { getResponse });
            var descriptions = new [] { new SpaceDescription()
            {
                name = _space1.Name,
            }};

            var createdIds = await Actions.CreateSpaces(httpClient, _silentLogger, descriptions, Guid.Empty);
            Assert.Equal(_guid1, createdIds.Single());
            Assert.Equal(0, httpHandler.PostRequests.Count);
            Assert.Equal(1, httpHandler.GetRequests.Count);
        }

        [Fact]
        public async Task CreateSpacesWithSingleRootAndChildrenMakesRequestsAndReturnsRootId()
        {
            (var httpClient, var httpHandler) = FakeHttpHandler.CreateHttpClient(
                postResponses: CreateGuidResponses(new [] { _guid1, _guid2, _guid3 }),
                getResponses: Enumerable.Repeat(_notFoundResponse, 1000));
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

            var createdIds = await Actions.CreateSpaces(httpClient, _silentLogger, descriptions, Guid.Empty);
            Assert.Equal(_guid1, createdIds.Single());
            Assert.Equal(3, httpHandler.PostRequests.Count);
            Assert.Equal(3, httpHandler.GetRequests.Count);
        }

        [Fact]
        public async Task CreateSpacesWithMultipleRootsMakesRequestsAndReturnsAllRootIds()
        {
            (var httpClient, var httpHandler) = FakeHttpHandler.CreateHttpClient(
                postResponses: CreateGuidResponses(new [] { _guid1, _guid2 }),
                getResponses: Enumerable.Repeat(_notFoundResponse, 1000));
            var descriptions = new [] {
                new SpaceDescription()
                {
                    name = "Test1",
                },
                new SpaceDescription()
                {
                    name = "Test2",
                }};

            var createdIds = await Actions.CreateSpaces(httpClient, _silentLogger, descriptions, Guid.Empty);
            Assert.Equal(new [] { _guid1, _guid2 }, createdIds);
            Assert.Equal(2, httpHandler.PostRequests.Count);
            Assert.Equal(2, httpHandler.GetRequests.Count);
        }

        private IEnumerable<HttpResponseMessage> CreateGuidResponses(IEnumerable<Guid> guids)
            => guids.Select(guid => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"\"{guid.ToString()}\""),
                });
    }
}