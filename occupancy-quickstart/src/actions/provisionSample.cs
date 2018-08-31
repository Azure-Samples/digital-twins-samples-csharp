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
        public static async Task<IEnumerable<Guid>> ProvisionSample(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<SpaceDescription> spaceCreateDescriptions;
            using (var r = new StreamReader("actions/provisionSample.yaml"))
            {
                spaceCreateDescriptions = await GetProvisionSampleTopology(r);
            }
            var createdSpaceIds = await CreateSpaces(httpClient, logger, spaceCreateDescriptions, Guid.Empty);
            var createdSpaceIdsAsString = createdSpaceIds
                .Select(x => x.ToString())
                .Aggregate((acc, cur) => acc + ", " + cur);
            logger.LogInformation($"Created spaces: {createdSpaceIdsAsString}");
            return createdSpaceIds;
        }

        public static async Task<IEnumerable<SpaceDescription>> GetProvisionSampleTopology(TextReader textReader)
            => new Deserializer().Deserialize<IEnumerable<SpaceDescription>>(await textReader.ReadToEndAsync());

        public static async Task<IEnumerable<Guid>> CreateSpaces(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<SpaceDescription> descriptions,
            Guid parentId)
        {
            var spaceIds = new List<Guid>();
            foreach (var description in descriptions)
            {
                var spaceId = await GetExistingSpaceOrCreate(httpClient, logger, parentId, description);

                if (spaceId != Guid.Empty)
                {
                    spaceIds.Add(spaceId);

                    if (description.spaces != null)
                        await CreateSpaces(httpClient, logger, description.spaces, spaceId);
                }
            }

            return spaceIds;
        }

        // Returns a space with same name and parentId if there is exactly one
        // that maches that criteria. Otherwise returns null.
        private static async Task<Models.Space> GetExistingSpace(
            HttpClient httpClient,
            ILogger logger,
            string name,
            Guid parentId)
        {
            var filterName = $"Name eq '{name}'";
            var filterParentSpaceId = parentId != Guid.Empty
                ? $"ParentSpaceId eq guid'{parentId}'"
                : $"ParentSpaceId eq null";
            var odataFilter = $"$filter={filterName} and {filterParentSpaceId}";

            var response = await httpClient.GetAsync($"spaces?{odataFilter}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonConvert.DeserializeObject<IReadOnlyCollection<Models.Space>>(content);
                var matchingSpace = spaces.SingleOrDefault();
                if (matchingSpace != null)
                {
                    logger.LogInformation($"Retrieved Unique Space using 'name' and 'parentSpaceId': {JsonConvert.SerializeObject(matchingSpace, Formatting.Indented)}");
                    return matchingSpace;
                }
            }
            return null;
        }

        private static async Task<Guid> GetExistingSpaceOrCreate(HttpClient httpClient, ILogger logger, Guid parentId, SpaceDescription description)
        {
            var existingSpace = await GetExistingSpace(httpClient, logger, description.name, parentId);
            return existingSpace?.Id != null
                ? Guid.Parse(existingSpace.Id)
                : await CreateSpace(httpClient, logger, description.ToSpaceCreate(parentId));
        }

        private static async Task<Guid> CreateSpace(HttpClient httpClient, ILogger logger, Models.SpaceCreate spaceCreate)
        {
            logger.LogInformation($"Creating Space: {JsonConvert.SerializeObject(spaceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(spaceCreate);
            var response = await httpClient.PostAsync("spaces", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        private static async Task<Guid> GetIdFromResponse(HttpResponseMessage response, ILogger logger)
        {
            if (!response.IsSuccessStatusCode)
                return Guid.Empty;

            var content = await response.Content.ReadAsStringAsync();

            // strip out the double quotes that come in the response and parse into a guid
            if (!Guid.TryParse(content.Substring(1, content.Length - 2), out var createdId))
            {
                logger.LogError($"ERROR: Returned value from POST did not parse into a guid: {content}");
                return Guid.Empty;
            }

            return createdId;
        }
    }
}