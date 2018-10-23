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
        public static async Task<IEnumerable<Guid>> CreateRoleAssignments(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<RoleAssignmentDescription> roleAssignmentsDescriptions;
            using (var r = new StreamReader("actions/createRoleAssignments.yaml"))
            {
                roleAssignmentsDescriptions = await GetCreateRoleAssignmentsDescriptions(r);
            }

            var createdIds = (await CreateRoleAssignments(httpClient, logger, roleAssignmentsDescriptions)).ToList();

            Console.WriteLine($"CreateRoleAssignments completed: {GetCreationSummary("role assignment", "role assignments", createdIds)}");

            return createdIds;
        }

        public static async Task<IEnumerable<RoleAssignmentDescription>> GetCreateRoleAssignmentsDescriptions(TextReader textReader)
            => new Deserializer().Deserialize<IEnumerable<RoleAssignmentDescription>>(await textReader.ReadToEndAsync());

        public static async Task<IEnumerable<Guid>> CreateRoleAssignments(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<RoleAssignmentDescription> descriptions)
        {
            var roleAssignmentIds = new List<Guid>();
            foreach (var description in descriptions)
            {
                roleAssignmentIds.Add(await Api.CreateRoleAssignment(httpClient, logger, description.ToRoleAssignmentCreate()));
            }

            return roleAssignmentIds.Where(id => id != Guid.Empty);
        }
    }
}