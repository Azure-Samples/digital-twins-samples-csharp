using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public partial class Api
    {
        public static async Task<IEnumerable<Models.Ontology>> GetOntologies(
            HttpClient httpClient, 
            ILogger logger)
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

        public static async Task<Models.Resource> GetResource(
            HttpClient httpClient,
            ILogger logger,
            Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetResource requires a non empty guid as id");

            var response = await httpClient.GetAsync($"resources/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resource = JsonConvert.DeserializeObject<Models.Resource>(content);
                logger.LogInformation($"Retrieved Resource: {JsonConvert.SerializeObject(resource, Formatting.Indented)}");
                return resource;
            }

            return null;
        }

        public static async Task<Models.Space> GetSpace(
            HttpClient httpClient,
            ILogger logger,
            Guid id,
            string includes = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetSpace requires a non empty guid as id");

            var response = await httpClient.GetAsync($"spaces/{id}/" + (includes != null ? $"?includes={includes}" : ""));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var space = JsonConvert.DeserializeObject<Models.Space>(content);
                logger.LogInformation($"Retrieved Space: {JsonConvert.SerializeObject(space, Formatting.Indented)}");
                return space;
            }

            return null;
        }
    }
}