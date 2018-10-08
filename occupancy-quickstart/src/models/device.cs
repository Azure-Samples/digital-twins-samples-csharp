// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    // TODO: Need to fixup ids in models to be Guids (were approriate)
    // Then we can change upstream apis like DescriptionExtensions
    public class Device
    {
        public string ConnectionString { get; set; }
        public string HardwareId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SpaceId { get; set; }
    }
}