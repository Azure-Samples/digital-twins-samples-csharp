using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static partial class Actions
    {
        // Prints out and returns spaces with the occupany property key set
        public static async Task GetOccupancy(HttpClient httpClient, ILogger logger)
        {
            var spaces = await Api.GetSpaces(
                httpClient, logger,
                maxNumberToGet: 100, propertyKey: "AvailableAndFresh", includes: "properties");

            Console.WriteLine($"Spaces with 'AvailableAndFresh' PropertyKey: {JsonConvert.SerializeObject(spaces, Formatting.Indented)}");
        }
    }
}