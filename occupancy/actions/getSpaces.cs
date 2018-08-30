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
        public static async Task<IEnumerable<Models.Space>> GetSpaces(HttpClient httpClient, ILogger logger)
        {
            var response = await httpClient.GetAsync($"spaces?includes=types,values&$top=10");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonConvert.DeserializeObject<IEnumerable<Models.Space>>(content);
                logger.LogInformation($"Retrieved Spaces: {JsonConvert.SerializeObject(spaces, Formatting.Indented)}");
                return spaces;
            }
            else
            {
                return Array.Empty<Models.Space>();
            }
        }
    }
}