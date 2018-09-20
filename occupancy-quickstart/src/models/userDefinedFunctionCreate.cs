using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class UserDefinedFunctionCreate
    {
        public string Name { get; set; }
        public string SpaceId { get; set; }
        public IEnumerable<string> Matchers { get; set; }
    }
}