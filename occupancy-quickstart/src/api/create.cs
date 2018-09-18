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
        public static async Task<Guid> CreateDevice(HttpClient httpClient, ILogger logger, Models.DeviceCreate deviceCreate)
        {
            logger.LogInformation($"Creating Device: {JsonConvert.SerializeObject(deviceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(deviceCreate);
            var response = await httpClient.PostAsync("devices", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateMatcher(HttpClient httpClient, ILogger logger, Models.MatcherCreate matcherCreate)
        {
            logger.LogInformation($"Creating Matcher: {JsonConvert.SerializeObject(matcherCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(matcherCreate);
            var response = await httpClient.PostAsync("matchers", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateResource(HttpClient httpClient, ILogger logger, Models.ResourceCreate resourceCreate)
        {
            logger.LogInformation($"Creating Resource: {JsonConvert.SerializeObject(resourceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(resourceCreate);
            var response = await httpClient.PostAsync("resources", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateSensor(HttpClient httpClient, ILogger logger, Models.SensorCreate sensorCreate)
        {
            logger.LogInformation($"Creating Sensor: {JsonConvert.SerializeObject(sensorCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(sensorCreate);
            var response = await httpClient.PostAsync("sensors", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateSpace(HttpClient httpClient, ILogger logger, Models.SpaceCreate spaceCreate)
        {
            logger.LogInformation($"Creating Space: {JsonConvert.SerializeObject(spaceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(spaceCreate);
            var response = await httpClient.PostAsync("spaces", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            Models.UserDefinedFunctionCreate userDefinedFunctionCreate,
            string js)
        {
            logger.LogInformation($"Creating UserDefinedFunction with Metadata: {JsonConvert.SerializeObject(userDefinedFunctionCreate, Formatting.Indented)}");
            var displayContent = js.Length > 100 ? js.Substring(0, 100) + "..." : js;
            logger.LogInformation($"Creating UserDefinedFunction with Content: {displayContent}");

            var metadataContent = new StringContent(JsonConvert.SerializeObject(userDefinedFunctionCreate), Encoding.UTF8, "application/json");
            metadataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            var multipartContent = new MultipartFormDataContent("userDefinedFunctionBoundary");
            multipartContent.Add(metadataContent, "metadata");
            multipartContent.Add(new StringContent(js), "contents");

            var response = await httpClient.PostAsync("userdefinedfunctions", multipartContent);
            return await GetIdFromResponse(response, logger);
        }

        private static async Task<Guid> GetIdFromResponse(HttpResponseMessage response, ILogger logger)
        {
            if (!response.IsSuccessStatusCode)
                return Guid.Empty;

            var content = await response.Content.ReadAsStringAsync();

            // strip out the double quotes that come in the response and parse into a guid
            if (!Guid.TryParse(content.Substring(1, content.Length - 2), out var createdId))
            {
                logger.LogError($"Returned value from POST did not parse into a guid: {content}");
                return Guid.Empty;
            }

            return createdId;
        }
    }
}