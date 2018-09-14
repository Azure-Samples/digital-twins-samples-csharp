// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    [DataContract(Name="TelemetryMessage")]
    public class TelemetryMessage
    {
        [DataMember(Name="SensorValue")]
        public string SensorValue { get; set; }
    }
}