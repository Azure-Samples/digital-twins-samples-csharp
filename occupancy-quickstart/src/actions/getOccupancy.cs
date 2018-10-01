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
            var spaces = await GetManagementItemsAsync<Models.Space>(httpClient, "spaces", "name=Focus Room A1&includes=values");
            var serializedObjects = JsonConvert.SerializeObject(
                    spaces,
                    Formatting.Indented,
                    new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine($"Get Space 'Focus Room A1': {serializedObjects}");
        }

        private static async Task<IEnumerable<T>> GetManagementItemsAsync<T>(
            HttpClient httpClient,
            string queryItem,
            string queryParams)
        {
            var response = await httpClient.GetAsync($"{queryItem}?{queryParams}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var objects = JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                return objects;
            }

            return null;
        }
    }

    public class AnyItem
    {
        public string Id { get; set; }
    }
}