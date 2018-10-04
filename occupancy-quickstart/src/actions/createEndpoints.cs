using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static partial class Actions
    {
        public static async Task<HttpResponseMessage> CreateEndpoints(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<EndpointDescription> endpointDescriptions;
            using (var r = new StreamReader("actions/createEndpoints.yaml"))
            {
                endpointDescriptions = await GetCreateEndpointsDescriptions(r);
            }

            var response = await CreateEndpoints(httpClient, logger, endpointDescriptions);

            Console.WriteLine($"CreateEndpoints completed with: {JsonConvert.SerializeObject(response, Formatting.Indented)}");

            return response;
        }

        public static async Task<IEnumerable<EndpointDescription>> GetCreateEndpointsDescriptions(TextReader textReader)
            => new Deserializer().Deserialize<IEnumerable<EndpointDescription>>(await textReader.ReadToEndAsync());

        public static async Task<HttpResponseMessage> CreateEndpoints(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<EndpointDescription> descriptions)
        {
            foreach (var description in descriptions)
            {
                await Api.CreateEndpoints(httpClient, logger, description.ToEndpointCreate());
            }
            return null;
        }
    }
}