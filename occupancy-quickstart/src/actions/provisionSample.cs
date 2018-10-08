// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

            Console.WriteLine($"Completed Provisioning: {JsonConvert.SerializeObject(results, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore } )}");

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

                    var devices = description.devices != null
                        ? await CreateDevices(httpClient, logger, description.devices, spaceId)
                        : Array.Empty<Models.Device>();

                    if (description.matchers != null)
                        await CreateMatchers(httpClient, logger, description.matchers, spaceId);

                    if (description.userdefinedfunctions != null)
                        await CreateUserDefinedFunctions(httpClient, logger, description.userdefinedfunctions, spaceId);

                    if (description.roleassignments != null)
                        await CreateRoleAssignments(httpClient, logger, description.roleassignments, spaceId);

                    var childSpacesResults = description.spaces != null
                        ? await CreateSpaces(httpClient, logger, description.spaces, spaceId)
                        : Array.Empty<ProvisionResults.Space>();

                    var sensors = await Api.GetSensorsOfSpace(httpClient, logger, spaceId);

                    spaceResults.Add(new ProvisionResults.Space()
                    {
                        Id = spaceId,
                        Devices = devices.Select(device => new ProvisionResults.Device()
                            {
                                ConnectionString = device.ConnectionString,
                                HardwareId = device.HardwareId,
                            }),
                        Sensors = sensors.Select(sensor => new ProvisionResults.Sensor()
                            {
                                DataType = sensor.DataType,
                                HardwareId = sensor.HardwareId,
                            }),
                        Spaces = childSpacesResults,
                    });
                }
            }

            return spaceResults;
        }

        private static async Task<IEnumerable<Models.Device>> CreateDevices(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<DeviceDescription> descriptions,
            Guid spaceId)
        {
            if (spaceId == Guid.Empty)
                throw new ArgumentException("Devices must have a spaceId");

            var devices = new List<Models.Device>();

            foreach (var description in descriptions)
            {
                var device = await GetExistingDeviceOrCreate(httpClient, logger, spaceId, description);

                if (device != null)
                {
                    devices.Add(device);

                    if (description.sensors != null)
                        await CreateSensors(httpClient, logger, description.sensors, Guid.Parse(device.Id));
                }
            }

            return devices;
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
                var matchers = await Api.FindMatchers(httpClient, logger, description.matcherNames, spaceId);

                using (var r = new StreamReader(description.script))
                {
                    var js = await r.ReadToEndAsync();
                    if (String.IsNullOrWhiteSpace(js))
                    {
                        logger.LogError($"Error creating user defined function: Couldn't read from {description.script}");
                    }
                    else
                    {
                        await CreateOrPatchUserDefinedFunction(
                            httpClient,
                            logger,
                            description,
                            js,
                            spaceId,
                            matchers);
                    }
                }
            }
        }

        private static async Task<Models.Device> GetExistingDeviceOrCreate(HttpClient httpClient, ILogger logger, Guid spaceId, DeviceDescription description)
        {
            // NOTE: The API doesn't support getting connection strings on bulk get devices calls so we
            // even in the case where we are reusing a preexisting device we need to make the GetDevice
            // call below to get the connection string
            var existingDeviceId = (await Api.FindDevice(httpClient, logger, description.hardwareId, spaceId))?.Id;
            var deviceId = existingDeviceId != null
                ? Guid.Parse(existingDeviceId)
                : await Api.CreateDevice(httpClient, logger, description.ToDeviceCreate(spaceId));
            return await Api.GetDevice(httpClient, logger, deviceId, includes: "ConnectionString");
        }

        private static async Task<Guid> GetExistingSpaceOrCreate(HttpClient httpClient, ILogger logger, Guid parentId, SpaceDescription description)
        {
            var existingSpace = await Api.FindSpace(httpClient, logger, description.name, parentId);
            return existingSpace?.Id != null
                ? Guid.Parse(existingSpace.Id)
                : await Api.CreateSpace(httpClient, logger, description.ToSpaceCreate(parentId));
        }

        private static async Task CreateOrPatchUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            UserDefinedFunctionDescription description,
            string js,
            Guid spaceId,
            IEnumerable<Models.Matcher> matchers)
        {
            var userDefinedFunction = await Api.FindUserDefinedFunction(httpClient, logger, description.name, spaceId);

            if (userDefinedFunction != null)
            {
                await Api.UpdateUserDefinedFunction(
                    httpClient,
                    logger,
                    description.ToUserDefinedFunctionUpdate(userDefinedFunction.Id, spaceId, matchers.Select(m => m.Id)),
                    js);
            }
            else
            {
                await Api.CreateUserDefinedFunction(
                    httpClient,
                    logger,
                    description.ToUserDefinedFunctionCreate(spaceId, matchers.Select(m => m.Id)),
                    js);
            }
        }
    }
}