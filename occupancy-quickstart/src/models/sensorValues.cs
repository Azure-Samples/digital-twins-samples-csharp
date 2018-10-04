using System.Collections.Generic;

namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class SensorValue
    {
        public string Type;
        public string Value;
        public string Timestamp;
        public IEnumerable<HistoricalValues> HistoricalValues;
    }
}