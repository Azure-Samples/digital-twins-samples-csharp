using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class ConditionCreate
    {
        public string Target { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
        public string Comparison { get; set; }
    }

    public class MatcherCreate
    {
        public string Name { get; set; }
        public string SpaceId { get; set; }
        public IEnumerable<ConditionCreate> Conditions { get; set; }
    }
}