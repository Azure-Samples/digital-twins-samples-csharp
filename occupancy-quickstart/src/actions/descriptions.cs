using System.Collections.Generic;
using Newtonsoft.Json;

// Descriptions are a representation of a space topology that are specific to this app
// They are an in memory version of the yaml file in this directory

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class SpaceDescription
    {
        public string name { get; set; }
        public string type { get; set; }
        public string subType { get; set; }

        [JsonIgnore]
        public IEnumerable<SpaceDescription> spaces { get; set; }
    }
}