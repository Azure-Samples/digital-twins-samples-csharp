// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public partial class Api
    {
        // Returns a device with same hardwareId and spaceId if there is exactly one.
        // Otherwise returns null.
        public static async Task<Models.Device> FindDevice(
            HttpClient httpClient,
            ILogger logger,
            string hardwareId,
            Guid? spaceId,
            string includes = null)
        {
            var filterHardwareIds = $"hardwareIds={hardwareId}";
            var filterSpaceId = spaceId != null ? $"&spaceIds={spaceId.ToString()}" : "";
            var includesParam = includes != null ? $"&includes={includes}" : "";
            var filter = $"{filterHardwareIds}{filterSpaceId}{includesParam}";

            var response = await httpClient.GetAsync($"devices?{filter}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var devices = JsonConvert.DeserializeObject<IReadOnlyCollection<Models.Device>>(content);
                var matchingDevice = devices.SingleOrDefault();
                if (matchingDevice != null)
                {
                    logger.LogInformation($"Retrieved Unique Device using 'hardwareId' and 'spaceId': {JsonConvert.SerializeObject(matchingDevice, Formatting.Indented)}");
                    return matchingDevice;
                }
            }
            return null;
        }

        // Returns a matcher with same name and spaceId if there is exactly one.
        // Otherwise returns null.
        public static async Task<IEnumerable<Models.Matcher>> FindMatchers(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<string> names,
            Guid spaceId)
        {
            var commaDelimitedNames = names.Aggregate((string acc, string s) => acc + "," + s);
            var filterNames = $"names={commaDelimitedNames}";
            var filterSpaceId = $"&spaceIds={spaceId.ToString()}";
            var filter = $"{filterNames}{filterSpaceId}";

            var response = await httpClient.GetAsync($"matchers?{filter}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var matchers = JsonConvert.DeserializeObject<IReadOnlyCollection<Models.Matcher>>(content);
                if (matchers != null)
                {
                    logger.LogInformation($"Retrieved Unique Matchers using 'names' and 'spaceId': {JsonConvert.SerializeObject(matchers, Formatting.Indented)}");
                    return matchers;
                }
            }
            return null;
        }

        // Returns a space with same name and parentId if there is exactly one
        // that maches that criteria. Otherwise returns null.
        public static async Task<Models.Space> FindSpace(
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

        // Returns a user defined fucntion with same name and spaceId if there is exactly one.
        // Otherwise returns null.
        public static async Task<Models.UserDefinedFunction> FindUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            string name,
            Guid spaceId)
        {
            var filterNames = $"names={name}";
            var filterSpaceId = $"&spaceIds={spaceId.ToString()}";
            var filter = $"{filterNames}{filterSpaceId}";

            var response = await httpClient.GetAsync($"userdefinedfunctions?{filter}&includes=matchers");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userDefinedFunctions = JsonConvert.DeserializeObject<IReadOnlyCollection<Models.UserDefinedFunction>>(content);
                var userDefinedFunction = userDefinedFunctions.SingleOrDefault();
                if (userDefinedFunction != null)
                {
                    logger.LogInformation($"Retrieved Unique UserDefinedFunction using 'name' and 'spaceId': {JsonConvert.SerializeObject(userDefinedFunction, Formatting.Indented)}");
                    return userDefinedFunction;
                }
            }
            return null;
        }
    }
}
