// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public partial class Api
    {
        public static async Task<bool> IsResourceProvisioning(
            HttpClient httpClient,
            ILogger logger,
            Guid id)
        {
            var resource = await GetResource(httpClient, logger, id);
            if (resource == null)
            {
                logger.LogError($"Failed to find expected resource, {id.ToString()}");
                return false;
            }

            return resource.Status.ToLower() == "provisioning";
        }
    }
}