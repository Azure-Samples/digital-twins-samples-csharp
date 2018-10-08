// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static partial class Actions
    {
        public static async Task CreateRoleAssignment(HttpClient httpClient, ILogger logger,
            Guid objectId, string objectIdType, Guid tenantId)
        {
            var roleAssignmentId = await Api.CreateRoleAssignment(
                httpClient, logger, new Models.RoleAssignmentCreate()
                {
                    ObjectId = objectId.ToString(),
                    ObjectIdType = objectIdType,
                    Path = "/",
                    RoleId = "98e44ad7-28d4-4007-853b-b9968ad132d1", // System Role: SpaceAdministrator
                    TenantId = tenantId.ToString(),
                });

            Console.WriteLine($"CreateRoleAssignment: {roleAssignmentId.ToString()}");
        }
    }
}