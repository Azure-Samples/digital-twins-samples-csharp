using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static partial class Actions
    {
        public static async Task<IEnumerable<Models.Space>> GetSpaces(HttpClient httpClient, Logger logger)
        {
            var response = await httpClient.GetAsync($"spaces?includes=types,values&$top=10");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonConvert.DeserializeObject<IEnumerable<Models.Space>>(content);
                logger.WriteLine($"Retrieved Spaces: {JsonConvert.SerializeObject(spaces, Formatting.Indented)}");
                return spaces;
            }
            else
            {
                return new Models.Space[0];
            }
        }
    }
}