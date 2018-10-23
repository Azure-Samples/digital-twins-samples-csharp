// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

// These extensions translate the descriptions (which are in memory representations
// of the sample yaml file) to Digital Twins models.

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static class DescriptionExtensions
    {
        public static Models.DeviceCreate ToDeviceCreate(this DeviceDescription description, Guid spaceId)
            => new Models.DeviceCreate()
            {
                HardwareId = description.hardwareId,
                Name = description.name,
                SpaceId = spaceId.ToString(),
            };

        public static Models.EndpointsCreate ToEndpointCreate(this EndpointDescription description)
            => new Models.EndpointsCreate()
            {
                ConnectionString = description.connectionString,
                EventTypes = description.eventTypes,
                Path = description.path,
                SecondaryConnectionString = description.secondaryConnectionString,
                Type = description.type,
            };

        public static Models.MatcherCreate ToMatcherCreate(this MatcherDescription description, Guid spaceId)
            => new Models.MatcherCreate()
            {
                Name = description.name,
                SpaceId = spaceId.ToString(),
                Conditions = new [] {
                    new Models.ConditionCreate()
                    {
                        Target = "Sensor",
                        Path = "$.dataType",
                        Value = $"\"{description.dataTypeValue}\"",
                        Comparison = "Equals",
                    }
                }
            };

        public static Models.KeyStoreCreate ToKeyStoreCreate(this KeyStoreDescription description, Guid spaceId)
            => new Models.KeyStoreCreate()
            {
                Name = description.name,
                SpaceId = spaceId.ToString(),
            };

        public static Models.RoleAssignmentCreate ToRoleAssignmentCreate(this RoleAssignmentDescription description, string objectId = null, string path = null)
            => new Models.RoleAssignmentCreate()
            {
                ObjectId = objectId ?? description.objectId,
                ObjectIdType = description.objectIdType,
                Path = path ?? description.path,
                RoleId = description.roleId,
                TenantId = description.tenantId,
            };

        public static Models.ResourceCreate ToResourceCreate(this ResourceDescription description, Guid spaceId)
            => new Models.ResourceCreate()
            {
                SpaceId = spaceId.ToString(),
                Type = description.type,
            };

        public static Models.SensorCreate ToSensorCreate(this SensorDescription description, Guid deviceId)
            => new Models.SensorCreate()
            {
                DataType = description.dataType,
                DeviceId = deviceId.ToString(),
                HardwareId = description.hardwareId,
            };

        public static Models.SpaceCreate ToSpaceCreate(this SpaceDescription description, Guid parentId)
            => new Models.SpaceCreate()
            {
                Name = description.name,
                ParentSpaceId = parentId != Guid.Empty ? parentId.ToString() : "",
            };

        public static Models.UserDefinedFunction ToUserDefinedFunction(this UserDefinedFunctionDescription description, string Id, Guid spaceId, IEnumerable<Models.Matcher> matchers)
            => new Models.UserDefinedFunction()
            {
                Id = Id,
                Name = description.name,
                SpaceId = spaceId.ToString(),
                Matchers = matchers,
            };

        public static Models.UserDefinedFunctionCreate ToUserDefinedFunctionCreate(this UserDefinedFunctionDescription description, Guid spaceId, IEnumerable<string> matcherIds)
            => new Models.UserDefinedFunctionCreate()
            {
                Name = description.name,
                SpaceId = spaceId.ToString(),
                Matchers = matcherIds,
            };

        public static Models.UserDefinedFunctionUpdate ToUserDefinedFunctionUpdate(this UserDefinedFunctionDescription description, string id, Guid spaceId, IEnumerable<string> matcherIds)
            => new Models.UserDefinedFunctionUpdate()
            {
                Id = id,
                Name = description.name,
                SpaceId = spaceId.ToString(),
                Matchers = matcherIds,
            };
    }
}