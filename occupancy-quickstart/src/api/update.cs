// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public partial class Api
    {
        public static async Task UpdateUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            Models.UserDefinedFunctionUpdate userDefinedFunction,
            string js)
        {
            logger.LogInformation($"Updating UserDefinedFunction with Metadata: {JsonConvert.SerializeObject(userDefinedFunction, Formatting.Indented)}");
            var displayContent = js.Length > 100 ? js.Substring(0, 100) + "..." : js;
            logger.LogInformation($"Updating UserDefinedFunction with Content: {displayContent}");

            var metadataContent = new StringContent(JsonConvert.SerializeObject(userDefinedFunction), Encoding.UTF8, "application/json");
            metadataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            var multipartContent = new MultipartFormDataContent("userDefinedFunctionBoundary");
            multipartContent.Add(metadataContent, "metadata");
            multipartContent.Add(new StringContent(js), "contents");

            await httpClient.PatchAsync($"userdefinedfunctions/{userDefinedFunction.Id}", multipartContent);
        }
    }
}