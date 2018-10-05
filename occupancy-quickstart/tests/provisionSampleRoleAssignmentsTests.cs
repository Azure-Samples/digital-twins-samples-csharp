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
    public class ProvisionSampleRoleAssignmentsTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid roleAssignmentGuid1 = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid roleAssignmentGuid2 = new Guid("00000000-0000-0000-0000-000000000002");
        private static Guid roleIdGuid = new Guid("99999999-0000-0000-0000-000000000000");
        private static Models.Space space1 = new Models.Space()
        {
            Id = new Guid("80000000-0000-0000-0000-000000000001").ToString(),
            Name = "Space 1",
            SpacePaths = new []
            {
                "Some1\\Path1",
            },
        };
        private static Models.UserDefinedFunction function1 = new Models.UserDefinedFunction()
        {
            Id = new Guid("90000000-0000-0000-0000-000000000001").ToString(),
            Name = "Function 1",
        };
        private static Models.UserDefinedFunction function2 = new Models.UserDefinedFunction()
        {
            Id = new Guid("90000000-0000-0000-0000-000000000002").ToString(),
            Name = "Function 2",
        };
        private static HttpResponseMessage function1GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { function1 })),
        };
        private static HttpResponseMessage function2GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new [] { function2 })),
        };
        private static HttpResponseMessage space1GetResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(space1)),
        };

        [Fact]
        public async Task GetProvisionSampleCreatesDescriptions()
        {
            var yaml = @"
                - name: Test1
                  roleassignments:
                  - roleId: Id1
                    objectName: Name1
                    objectIdType: Type1
                  - roleId: Id2
                    objectName: Name2
                    objectIdType: Type2
                ";
            var expectedDescriptions = new [] { new SpaceDescription()
            {
                name = "Test1",
                roleassignments = new [] {
                    new RoleAssignmentDescription()
                    {
                        roleId = "Id1",
                        objectName = "Name1",
                        objectIdType = "Type1",
                    },
                    new RoleAssignmentDescription()
                    {
                        roleId = "Id2",
                        objectName = "Name2",
                        objectIdType = "Type2",
                    },
                },
            }};
            var actualDescriptions = await Actions.GetProvisionSampleTopology(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateRoleAssignmentWithUnknownTypeAndNameFails()
        {
            // How 'objectName' is used is based on objectIdType. In this test we choose an unknow type
            // and expect it then to fail the role assignment creation since it doesn't know how to use the name

            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: null,
                getResponses: new [] { space1GetResponse }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                roleassignments = new [] {
                    new RoleAssignmentDescription()
                    {
                        roleId = roleIdGuid.ToString(),
                        objectName = "SomeName",
                        objectIdType = "UnknownObjectIdType"
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.False(httpHandler.PostRequests.ContainsKey("roleassignments"));
        }

        [Fact]
        public async Task CreateTwoUdfRoleAssignments()
        {
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.CreateWithSpace(
                postResponseGuids: new [] { roleAssignmentGuid1, roleAssignmentGuid2 },
                getResponses: new [] { space1GetResponse, function1GetResponse, function2GetResponse }
            );

            var descriptions = new [] { new SpaceDescription()
            {
                name = FakeDigitalTwinsHttpClient.Space.Name,
                roleassignments = new [] {
                    new RoleAssignmentDescription()
                    {
                        roleId = roleIdGuid.ToString(),
                        objectName = "Function 1",
                        objectIdType = "UserDefinedFunctionId"
                    },
                    new RoleAssignmentDescription()
                    {
                        roleId = roleIdGuid.ToString(),
                        objectName = "Function 2",
                        objectIdType = "UserDefinedFunctionId"
                    }},
            }};

            await Actions.CreateSpaces(httpClient, Loggers.SilentLogger, descriptions, Guid.Empty);
            Assert.Equal(2, httpHandler.PostRequests["roleassignments"].Count);
            Assert.Equal(2, httpHandler.GetRequests["userdefinedfunctions"].Count);
            Assert.False(httpHandler.GetRequests.ContainsKey("roleassignments"));
        }
    }
}