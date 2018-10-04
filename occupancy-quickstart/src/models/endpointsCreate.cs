namespace Microsoft.Azure.DigitalTwins.Samples.Models
{
    public class EndpointsCreate
    {
        public string ConnectionString { get; set; }
        public string[] EventTypes { get; set; }
        public string Path { get; set; }
        public string SecondaryConnectionString { get; set; }
        public string Type { get; set; }
   }
}