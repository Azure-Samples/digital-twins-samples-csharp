using System.IO;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class AppSettings {
        public string AADInstance { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string Tenant { get; set; }
        public string BaseUrl { get; set; }
        public string Authority => AADInstance + Tenant;

        public static AppSettings Load() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("usersettings.json", optional: true)
                .Build()
                .Get<AppSettings>();
    }
}