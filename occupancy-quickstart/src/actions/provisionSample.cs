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
        public static async Task<IEnumerable<ProvisionResults.Space>> ProvisionSample(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<SpaceDescription> spaceCreateDescriptions;
            using (var r = new StreamReader("actions/provisionSample.yaml"))
            {
                spaceCreateDescriptions = await GetProvisionSampleTopology(r);
            }
            var results = await CreateSpaces(httpClient, logger, spaceCreateDescriptions, Guid.Empty);
            var createdSpaceIdsAsString = results
                .Select(x => x.Id.ToString())
                .Aggregate((acc, cur) => acc + ", " + cur);
            logger.LogInformation($"Created spaces: {createdSpaceIdsAsString}");
            return results;
        }

        public static async Task<IEnumerable<SpaceDescription>> GetProvisionSampleTopology(TextReader textReader)
            => new Deserializer().Deserialize<IEnumerable<SpaceDescription>>(await textReader.ReadToEndAsync());

        public static async Task<IEnumerable<ProvisionResults.Space>> CreateSpaces(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<SpaceDescription> descriptions,
            Guid parentId)
        {
            var spaceResults = new List<ProvisionResults.Space>();
            foreach (var description in descriptions)
            {
                var spaceId = await GetExistingSpaceOrCreate(httpClient, logger, parentId, description);

                if (spaceId != Guid.Empty)
                {
                    // This must happen before devices (or anyhting that could have devices like other spaces)
                    // or the device create will fail because a resource is required on an ancestor space
                    if (description.resources != null)
                        await CreateResources(httpClient, logger, description.resources, spaceId);

                    if (description.keystores != null)
                        await CreateKeyStoresWithKeys(httpClient, logger, description.keystores, spaceId);

                    // TODO: test getKeyStore from space

                    var sasTokens = description.devices != null
                        ? (await CreateDevices(httpClient, logger, description.devices, spaceId))
                            .Select(deviceId => CreateSasToken(httpClient, logger, deviceId))
                        : Array.Empty<string>();

                    if (description.matchers != null)
                        await CreateMatchers(httpClient, logger, description.matchers, spaceId);

                    if (description.userdefinedfunctions != null)
                        await CreateUserDefinedFunctions(httpClient, logger, description.userdefinedfunctions, spaceId);

                    if (description.roleassignments != null)
                        await CreateRoleAssignments(httpClient, logger, description.roleassignments, spaceId);

                    var childSpaceResults = description.spaces != null
                        ? await CreateSpaces(httpClient, logger, description.spaces, spaceId)
                        : Array.Empty<ProvisionResults.Space>();

                    spaceResults.Add(new ProvisionResults.Space()
                    {
                        Devices = sasTokens.Select(sasToken => new ProvisionResults.Device() { SasToken = sasToken } ),
                        Id = spaceId,
                        Spaces = childSpaceResults,
                    });
                }
            }

            return spaceResults;
        }

        private static async Task<IEnumerable<Guid>> CreateDevices(HttpClient httpClient, ILogger logger, IEnumerable<DeviceDescription> descriptions, Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("Devices must have a spaceId");

            var deviceIds = new List<Guid>();

            foreach (var description in descriptions)
            {
                var deviceId = await GetExistingDeviceOrCreate(httpClient, logger, spaceId, description);

                if (deviceId != Guid.Empty)
                {
                    deviceIds.Add(deviceId);

                    if (description.sensors != null)
                        await CreateSensors(httpClient, logger, description.sensors, deviceId);
                }
            }

            return deviceIds;
        }

        private static async Task<IEnumerable<Guid>> CreateKeyStoresWithKeys(HttpClient httpClient, ILogger logger, IEnumerable<KeyStoreDescription> descriptions, Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("KeyStores must have a spaceId");

            var keyStoreIds = new List<Guid>();

            foreach (var description in descriptions)
            {
                var keyStoreId = await Api.CreateKeyStore(httpClient, logger, description.ToKeyStoreCreate(spaceId));
                if (keyStoreId != Guid.Empty)
                {
                    keyStoreIds.Add(keyStoreId);
                }
            }

            return keyStoreIds;
        }

        private static async Task CreateMatchers(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<MatcherDescription> descriptions,
            Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("Matchers must have a spaceId");

            foreach (var description in descriptions)
            {
                await Api.CreateMatcher(httpClient, logger, description.ToMatcherCreate(spaceId));
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

        private static async Task CreateRoleAssignments(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<RoleAssignmentDescription> descriptions,
            Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("RoleAssignments must have a spaceId");

            var space = await Api.GetSpace(httpClient, logger, spaceId, includes: "fullpath");

            // A SpacePath is the list of spaces formatted like so: "space1/space2" - where space2 has space1 as a parent
            // When getting SpacePaths of a space itself there is always exactly one path - the path from the root to itself
            // This is not true when getting space paths of other topology items (ie non spaces)
            var path = space.SpacePaths.Single();

            foreach (var description in descriptions)
            {
                string objectId;
                switch (description.objectIdType)
                {
                    case "UserDefinedFunctionId":
                        objectId = (await Api.FindUserDefinedFunction(httpClient, logger, description.objectName, spaceId))?.Id;
                        break;
                    default:
                        objectId = null;
                        logger.LogError($"roleAssignment with objectName must have known objectIdType but instead has '{description.objectIdType}'");
                        break;
                }

                if (objectId != null)
                {
                    await Api.CreateRoleAssignment(httpClient, logger, description.ToRoleAssignmentCreate(objectId, path));
                }
            }
        }

        private static string CreateSasToken(HttpClient httpClient, ILogger logger, Guid deviceId)
        {
            return "";
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

        private static async Task CreateUserDefinedFunctions(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<UserDefinedFunctionDescription> descriptions,
            Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("UserDefinedFunctions must have a spaceId");

            foreach (var description in descriptions)
            {
                var matcher = await Api.FindMatcher(httpClient, logger, description.matcher, spaceId);

                using (var r = new StreamReader(description.script))
                {
                    var js = await r.ReadToEndAsync();
                    if (String.IsNullOrWhiteSpace(js))
                    {
                        logger.LogError($"Error creating user defined function: Couldn't read from {description.script}");
                    }
                    else
                    {
                        await Api.CreateUserDefinedFunction(
                            httpClient,
                            logger,
                            description.ToUserDefinedFunctionCreate(spaceId, new [] { matcher.Id }),
                            js);
                    }
                }
            }
        }

        private static void EnsureKeyStoreHasKey(HttpClient httpClient, ILogger logger, Guid keyStoreId)
        {
            // /api/v1.0/keystores/{id}/keys/last
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