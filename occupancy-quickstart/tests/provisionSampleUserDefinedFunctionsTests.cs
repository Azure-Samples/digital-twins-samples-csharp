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
    public class ProvisionSampleUserDefinedFunctionsTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid udfGuid1 = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid udfGuid2 = new Guid("00000000-0000-0000-0000-000000000002");
        private static Models.Matcher matcher1 = new Models.Matcher()
        {
            Id = new Guid("90000000-0000-0000-0000-000000000001").ToString(),
            Name = "Matcher1",
            SpaceId = null,
        };
        private static HttpResponseMessage matcher1GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { matcher1 })),
        };
        private static Models.UserDefinedFunction udf1 = new Models.UserDefinedFunction()
        {
            Id = udfGuid1.ToString(),
            Name = "User Defined Function 1",
        };
        private static HttpResponseMessage udf1GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { udf1 })),
        };
        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  userdefinedfunctions:
                  - name: Function 1
                    script: some1/path1
                  - name: Function 2
                    script: some2/path2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                userdefinedfunctions = new [] {
                    new UserDefinedFunctionDescription()
                    {
                        name = "Function 1",
                        script = "some1/path1",
                    },
                    new UserDefinedFunctionDescription()
                    {
                        name = "Function 2",
                        script = "some2/path2",
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoUserDefinedFunctions()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { udfGuid1, udfGuid2 },
                getResponses: new [] { matcher1GetResponse, Responses.NotFound, matcher1GetResponse, Responses.NotFound }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                userdefinedfunctions = new [] {
                    new UserDefinedFunctionDescription()
                    {
                        name = "Function 1",
                        matcher = "Matcher1",
                        script = "userDefinedFunctions/function1.js",
                    },
                    new UserDefinedFunctionDescription()
                    {
                        name = "Function 2",
                        matcher = "Matcher1",
                        script = "userDefinedFunctions/function2.js",
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(2, httpHandler.PostRequests["userdefinedfunctions"].Count);
            Assert.Equal(2, httpHandler.GetRequests["matchers"].Count);
            Assert.Equal(2, httpHandler.GetRequests["userdefinedfunctions"].Count);
        }

        [Fact]
        public async Task UpdateUserDefinedFunction()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { udfGuid1, udfGuid2 },
                getResponses: new [] { matcher1GetResponse, udf1GetResponse },
                patchResponses: new [] { Responses.OK }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                userdefinedfunctions = new [] {
                    new UserDefinedFunctionDescription()
                    {
                        name = "Function 1",
                        matcher = "Matcher1",
                        script = "userDefinedFunctions/function1.js",
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.False(httpHandler.PostRequests.ContainsKey("userdefinedfunctions"));
            Assert.Equal(1, httpHandler.PatchRequests["userdefinedfunctions"].Count);
            Assert.Equal(1, httpHandler.GetRequests["matchers"].Count);
            Assert.Equal(1, httpHandler.GetRequests["userdefinedfunctions"].Count);
        }
    }
}