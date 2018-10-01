using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class UserDefinedFunction
    {
        public string Id { get; set; }
        public IEnumerable<string> Matchers { get; set; }
        public string Name { get; set; }
        public string SpaceId { get; set; }
    }
}