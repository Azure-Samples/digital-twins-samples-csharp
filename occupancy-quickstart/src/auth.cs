// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    internal static class Authentication
    {
        // Gets an access token
        // First tries (by making a request) using a cached token and if that
        // fails we generated a new one using device login and cache it.
        internal static async Task<string> GetToken(ILogger logger, AppSettings appSettings)
        {
            var accessTokenFilename = ".accesstoken";
            var accessToken = ReadAccessTokenFromFile(accessTokenFilename);
            if (accessToken == null || !(await TryRequestWithAccessToken(new Uri(appSettings.BaseUrl), accessToken)))
            {
                accessToken = await Authentication.GetNewToken(logger, appSettings);
                System.IO.File.WriteAllText(accessTokenFilename, accessToken);
            }

            return accessToken;
        }

        private static async Task<bool> TryRequestWithAccessToken(Uri baseAddress, string accessToken)
        {
            // We create a new httpClient so we can force console logging for this operation
            var httpClient = new HttpClient(new LoggingHttpHandler(Loggers.ConsoleLogger))
            {
                BaseAddress = baseAddress,
            };
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            Loggers.ConsoleLogger.LogInformation("Checking if previous access token is valid...");

            return (await httpClient.GetAsync("ontologies")).IsSuccessStatusCode;
        }

        private static string ReadAccessTokenFromFile(string filename)
            => System.IO.File.Exists(filename) ? System.IO.File.ReadAllText(filename) : null;

        private static async Task<string> GetNewToken(
            ILogger logger,
            AppSettings appSettings)
        {
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(appSettings.Authority);
            return (await GetResultsUsingDeviceCode(authContext, appSettings)).AccessToken;
        }

        // This prompts the user to open a browser and input a unique key to authenticate their app
        // This allows dotnet core apps to authorize an application through user credentials without displaying UI.
        private static async Task<AuthenticationResult> GetResultsUsingDeviceCode(AuthenticationContext authContext, AppSettings appSettings)
        {
            var codeResult = await authContext.AcquireDeviceCodeAsync(appSettings.Resource, appSettings.ClientId);
            Console.WriteLine(codeResult.Message);
            return await authContext.AcquireTokenByDeviceCodeAsync(codeResult);
        }
    }
}