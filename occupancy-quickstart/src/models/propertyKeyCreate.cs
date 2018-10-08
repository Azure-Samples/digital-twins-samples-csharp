// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class PropertyKeyCreate
    {
        public string Name { get; set; }
        public string Scope { get; set; }
        public Guid SpaceId { get; set; }
    }
}