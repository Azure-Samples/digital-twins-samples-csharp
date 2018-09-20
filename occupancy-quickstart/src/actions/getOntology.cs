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
        public static async Task<IEnumerable<Models.Ontology>> GetOntology(HttpClient httpClient, ILogger logger)
        {
            var response = await httpClient.GetAsync($"ontologies");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ontologies = JsonConvert.DeserializeObject<IEnumerable<Models.Ontology>>(content);
                logger.LogInformation($"Retrieved Ontologies: {JsonConvert.SerializeObject(ontologies, Formatting.Indented)}");
                return ontologies;
            }
            else
            {
                return Array.Empty<Models.Ontology>();
            }
        }
    }
}