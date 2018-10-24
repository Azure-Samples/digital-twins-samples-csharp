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
    public class CreateRoleAssignmentsTests
    {
        private static Serializer yamlSerializer = new Serializer();
        private static Guid roleAssignment1Guid = new Guid("00000000-0000-0000-0000-000000000001");
        private static Guid roleAssignment2Guid = new Guid("00000000-0000-0000-0000-000000000002");

        [Fact]
        public async Task GetCreateRoleAssignmentsCreatesDescriptions()
        {
            var yaml = @"
                - objectId: Id1
                  objectIdType: Type1
                  path: Path1
                  roleId: RoleId1
                  tenantId: TenantId1
                ";
            var expectedDescriptions = new [] { new RoleAssignmentDescription()
            {
                objectId = "Id1",
                objectIdType = "Type1",
                path = "Path1",
                roleId = "RoleId1",
                tenantId = "TenantId1",
            }};
            var actualDescriptions = await Actions.GetCreateRoleAssignmentsDescriptions(new StringReader(yaml));
            Assert.Equal(yamlSerializer.Serialize(expectedDescriptions), yamlSerializer.Serialize(actualDescriptions));
        }

        [Fact]
        public async Task CreateTwoRoleAssignments()
        {
            var roleAssignmentGuids = new[] { roleAssignment1Guid, roleAssignment2Guid };
            (var httpClient, var httpHandler) = FakeDigitalTwinsHttpClient.Create(
                postResponseGuids: roleAssignmentGuids
            );

            var descriptions = new [] {
                new RoleAssignmentDescription()
                {
                    objectId = "10000000-0000-0000-0000-000000000001",
                    objectIdType = "UserId",
                    path = "/",
                    roleId = "10000000-0000-0000-0000-000000000002",
                    tenantId = "10000000-0000-0000-0000-000000000003",
                },
                new RoleAssignmentDescription()
                {
                    objectId = "20000000-0000-0000-0000-000000000001",
                    objectIdType = "UserId",
                    path = "/",
                    roleId = "20000000-0000-0000-0000-000000000002",
                    tenantId = "20000000-0000-0000-0000-000000000003",
                },
            };

            Assert.Equal(roleAssignmentGuids,
                await Actions.CreateRoleAssignments(httpClient, Loggers.SilentLogger, descriptions));
            Assert.Equal(2, httpHandler.PostRequests["roleassignments"].Count);
            Assert.False(httpHandler.GetRequests.ContainsKey("roleassignments"));
        }
    }
}