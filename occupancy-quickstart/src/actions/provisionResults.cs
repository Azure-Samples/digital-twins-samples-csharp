
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DigitalTwins.Samples.ProvisionResults
{
    public struct Space
    {
        public Guid Id;
        public IEnumerable<Device> Devices;
        public IEnumerable<Space> Spaces;
    }

    public struct Device
    {
        public string SasToken;
    }
}