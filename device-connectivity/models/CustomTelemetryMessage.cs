// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    [DataContract(Name="CustomTelemetryMessage")]
    public class CustomTelemetryMessage
    {
        [DataMember(Name="SensorValue")]
        public string SensorValue { get; set; }
    }
}