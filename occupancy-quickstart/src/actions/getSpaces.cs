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
    public static partial class Actions
    {
        public static async Task GetSpaces(HttpClient httpClient, ILogger logger)
        {
            var spaces = await Api.GetSpaces(
                httpClient, logger,
                maxNumberToGet: 100, includes: "types,values,properties");

            Console.WriteLine($"GetSpaces: {JsonConvert.SerializeObject(spaces, Formatting.Indented)}");
        }
    }
}