// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class Space
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string ParentSpaceId { get; set; }
        public string Subtype { get; set; }
        public int SubtypeId { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public IEnumerable<Space> Children { get; set; }
        public IEnumerable<string> SpacePaths { get; set; }
        public IEnumerable<SpaceValue> Values { get; set; }
    }
}