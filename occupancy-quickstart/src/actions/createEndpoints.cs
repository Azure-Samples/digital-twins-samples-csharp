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
        public static async Task<IEnumerable<Guid>> CreateEndpoints(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<EndpointDescription> endpointDescriptions;
            using (var r = new StreamReader("actions/createEndpoints.yaml"))
            {
                endpointDescriptions = await GetCreateEndpointsDescriptions(r);
            }

            var createdIds = (await CreateEndpoints(httpClient, logger, endpointDescriptions)).ToList();
            var createdIdsAsString = createdIds
                .Select(id => id.ToString())
                .Aggregate((acc, cur) => acc + ", " + cur);
            var createdIdsSummary =
                createdIds.Count == 0
                    ? "Created 0 endpoints."
                    : createdIds.Count == 1
                        ? $"Created 1 endpoint: {createdIdsAsString}"
                        : $"Created {createdIds.Count} endpoints: {createdIdsAsString}";

            Console.WriteLine($"CreateEndpoints completed. {createdIdsSummary}");

            return createdIds;
        }

        public static async Task<IEnumerable<EndpointDescription>> GetCreateEndpointsDescriptions(TextReader textReader)
            => new Deserializer().Deserialize<IEnumerable<EndpointDescription>>(await textReader.ReadToEndAsync());

        public static async Task<IEnumerable<Guid>> CreateEndpoints(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<EndpointDescription> descriptions)
        {
            var endpointIds = new List<Guid>();
            foreach (var description in descriptions)
            {
                endpointIds.Add(await Api.CreateEndpoints(httpClient, logger, description.ToEndpointCreate()));
            }
            return endpointIds.Where(id => id != Guid.Empty);
        }
    }
}