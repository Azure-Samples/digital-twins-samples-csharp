using System.Collections.Generic;
using Newtonsoft.Json;

// Descriptions are a representation of a space topology that are specific to this app
// They are an in memory version of the yaml file in this directory

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class DeviceDescription
    {
        public string hardwareId { get; set; }
        public string name { get; set; }

        [JsonIgnore]
        public IEnumerable<SensorDescription> sensors { get; set; }
    }

    public class ResourceDescription
    {
        public string type { get; set; }
    }

    public class SensorDescription
    {
        public string hardwareId { get; set; }
    }

    public class SpaceDescription
    {
        public string name { get; set; }
        public string type { get; set; }
        public string subType { get; set; }

        [JsonIgnore]
        public IEnumerable<DeviceDescription> devices { get; set; }

        [JsonIgnore]
        public IEnumerable<ResourceDescription> resources { get; set; }

        [JsonIgnore]
        public IEnumerable<SpaceDescription> spaces { get; set; }
    }
}