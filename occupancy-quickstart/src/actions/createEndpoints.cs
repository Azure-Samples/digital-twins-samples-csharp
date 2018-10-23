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
        public static async Task<IEnumerable<Guid>> CreateEndpoints(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<EndpointDescription> endpointDescriptions;
            using (var r = new StreamReader("actions/createEndpoints.yaml"))
            {
                endpointDescriptions = await GetCreateEndpointsDescriptions(r);
            }

            var createdIds = (await CreateEndpoints(httpClient, logger, endpointDescriptions)).ToList();

            Console.WriteLine($"CreateEndpoints completed. {GetCreationSummary("endpoint", "endpoints", createdIds)}");

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