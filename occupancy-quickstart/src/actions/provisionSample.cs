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

                    // This must happen before devices or the device create will fail because
                    if (description.resources != null)
                        await CreateResources(httpClient, logger, description.resources, spaceId);
                }
            }

            return spaceIds;
        }

        private static async Task CreateResources(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<ResourceDescription> descriptions,
            Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("Resources must have a spaceId");

            foreach (var description in descriptions)
            {
                var createdId = await Api.CreateResource(httpClient, logger, description.ToResourceCreate(spaceId));
                if (createdId != Guid.Empty)
                {
                    // After creation resources might take time to be ready to use so we need
                    // to poll until it is done since downstream operations (like device creation)
                    // may depend on it
                    logger.LogInformation("Polling until resource is no longer in 'Provisioning' state...");
                    while (await Api.IsResourceProvisioning(httpClient, logger, createdId))
                    {
                        await Task.Delay(5000);
                    }
                }
            }
        }

        private static async Task<Guid> GetExistingSpaceOrCreate(HttpClient httpClient, ILogger logger, Guid parentId, SpaceDescription description)
        {
            var existingSpace = await Api.FindSpace(httpClient, logger, description.name, parentId);
            return existingSpace?.Id != null
                ? Guid.Parse(existingSpace.Id)
                : await Api.CreateSpace(httpClient, logger, description.ToSpaceCreate(parentId));
        }
    }
}