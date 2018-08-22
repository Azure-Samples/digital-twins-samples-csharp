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
        public static async Task<IEnumerable<Models.Space>> GetSpaces(
            HttpClient httpClient)
        {
            var request = HttpHelper.MakeRequest(HttpMethod.Get, "spaces?includes=types&$top=10");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonConvert.DeserializeObject<IEnumerable<Models.Space>>(content);
                Console.WriteLine($"Retrieved Spaces: {JsonConvert.SerializeObject(spaces, Formatting.Indented)}");
                return spaces;
            }
            else
            {
                return new Models.Space[0];
            }
        }
    }
}