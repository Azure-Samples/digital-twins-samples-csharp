// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Net.Http;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    internal static class Authentication
    {
        // Gets an access token
        // First tries (by making a request) using a cached token and if that
        // fails we generated a new one using device login and cache it.
        internal static async Task<string> GetToken(AppSettings appSettings)
        {
            var accessTokenFilename = ".accesstoken";
            var accessToken = ReadAccessTokenFromFile(accessTokenFilename);
            if (accessToken == null || !(await TryRequestWithAccessToken(new Uri(appSettings.BaseUrl), accessToken)))
            {
                accessToken = await GetResultsUsingMsal(appSettings);
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
            return await GetResultsUsingMsal(appSettings);
        }

        // MSAL.NET configuration. Review the product documentation for more information about MSAL.NET authentication options.
        // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/
        private static async Task<string> GetResultsUsingMsal(AppSettings appSettings)
        {
            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(appSettings.ClientId)
                .WithRedirectUri(appSettings.AadRedirectUri)
                .WithAuthority(appSettings.Authority)
                .Build();

            AuthenticationResult result = await app
                .AcquireTokenInteractive(appSettings.Scopes)
                .ExecuteAsync();

            Console.WriteLine("");
            Console.WriteLine("MSAL Authentication Token Acquired: {0}", result.AccessToken);
            Console.WriteLine("");
            return result.AccessToken;
        }
    }
}
