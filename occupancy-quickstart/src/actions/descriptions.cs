// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Newtonsoft.Json;

// Descriptions are a representation of a space topology that are specific to this app
// They are an in memory version of the yaml file in this directory

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class DeviceDescription
    {
        public string hardwareId { get; set; }
        public string name { get; set; }

        [JsonIgnore]
        public IEnumerable<SensorDescription> sensors { get; set; }
    }

    public class EndpointDescription
    {
        public string type { get; set; }
        public string[] eventTypes { get; set; }
        public string connectionString { get; set; }
        public string secondaryConnectionString { get; set; }
        public string path { get; set; }
    }

    public class KeyStoreDescription
    {
        public string name { get; set; }
    }

    public class MatcherDescription
    {
        public string name { get; set; }
        public string dataTypeValue { get; set; }
    }

    public class ResourceDescription
    {
        public string type { get; set; }
    }

    public class RoleAssignmentDescription
    {
        public string objectId { get; set; }
        public string objectIdType { get; set; }
        public string objectName { get; set; }
        public string path { get; set; }
        public string roleId { get; set; }
        public string tenantId { get; set; }
    }

    public class SensorDescription
    {
        public string dataType { get; set; }
        public string hardwareId { get; set; }
    }

    public class UserDefinedFunctionDescription
    {
        public string name { get; set; }
        public IEnumerable<string> matcherNames { get; set; }
        public string script { get; set; }
    }

    public class SpaceDescription
    {
        public string name { get; set; }
        public string type { get; set; }
        public string subType { get; set; }

        [JsonIgnore]
        public IEnumerable<DeviceDescription> devices { get; set; }

        [JsonIgnore]
        public IEnumerable<KeyStoreDescription> keystores { get; set; }

        [JsonIgnore]
        public IEnumerable<MatcherDescription> matchers { get; set; }

        [JsonIgnore]
        public IEnumerable<RoleAssignmentDescription> roleassignments { get; set; }

        [JsonIgnore]
        public IEnumerable<ResourceDescription> resources { get; set; }

        [JsonIgnore]
        public IEnumerable<SpaceDescription> spaces { get; set; }

        [JsonIgnore]
        public IEnumerable<UserDefinedFunctionDescription> userdefinedfunctions { get; set; }
    }
}