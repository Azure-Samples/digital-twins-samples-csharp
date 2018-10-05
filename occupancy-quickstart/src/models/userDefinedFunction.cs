// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class UserDefinedFunction
    {
        public string Id { get; set; }
        public IEnumerable<Models.Matcher> Matchers { get; set; }
        public string Name { get; set; }
        public string SpaceId { get; set; }
    }
}