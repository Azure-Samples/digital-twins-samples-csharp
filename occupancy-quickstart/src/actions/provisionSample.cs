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

                    // This must happen before devices (or anyhting that could have devices like other spaces)
                    // or the device create will fail because a resource is required on an ancestor space
                    if (description.resources != null)
                        await CreateResources(httpClient, logger, description.resources, spaceId);

                    if (description.devices != null)
                        await CreateDevices(httpClient, logger, description.devices, spaceId);

                    if (description.spaces != null)
                        await CreateSpaces(httpClient, logger, description.spaces, spaceId);
                }
            }

            return spaceIds;
        }

        private static async Task CreateDevices(HttpClient httpClient, ILogger logger, IEnumerable<DeviceDescription> descriptions, Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("Devices must have a spaceId");

            foreach (var description in descriptions)
            {
                var deviceId = await GetExistingDeviceOrCreate(httpClient, logger, spaceId, description);

                if (deviceId != Guid.Empty)
                {
                    if (description.sensors != null)
                        await CreateSensors(httpClient, logger, description.sensors, deviceId);
                }
            }
        }

        private static async Task CreateSensors(HttpClient httpClient, ILogger logger, IEnumerable<SensorDescription> descriptions, Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new ArgumentException("Sensors must have a deviceId");

            foreach (var description in descriptions)
            {
                await Api.CreateSensor(httpClient, logger, description.ToSensorCreate(deviceId));
            }
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

        private static async Task<Guid> GetExistingDeviceOrCreate(HttpClient httpClient, ILogger logger, Guid spaceId, DeviceDescription description)
        {
            var existingDevice = await Api.FindDevice(httpClient, logger, description.hardwareId, spaceId);
            return existingDevice?.Id != null
                ? Guid.Parse(existingDevice.Id)
                : await Api.CreateDevice(httpClient, logger, description.ToDeviceCreate(spaceId));
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