// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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