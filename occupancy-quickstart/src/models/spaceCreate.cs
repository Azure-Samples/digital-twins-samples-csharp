// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class SpaceCreate
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ParentSpaceId { get; set; }
        public string Subtype { get; set; }
    }
}