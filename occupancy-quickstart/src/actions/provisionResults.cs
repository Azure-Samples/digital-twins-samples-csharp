// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DigitalTwins.Samples.ProvisionResults
{
    public struct Space
    {
        public Guid Id;
        public IEnumerable<Device> Devices;
        public IEnumerable<Sensor> Sensors;
        public IEnumerable<Space> Spaces;
    }

    public struct Device
    {
        public string ConnectionString;
        public string HardwareId;
    }

    public struct Sensor
    {
        public string DataType;
        public string HardwareId;
    }
}