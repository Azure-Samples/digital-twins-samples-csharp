// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var appSettings = AppSettings.Load();

                var actionName = ParseArgs(args);
                if (actionName == null)
                    return;

                switch (actionName)
                {
                    case ActionName.CreateEndpoints:
                        await Actions.CreateEndpoints(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.CreateRoleAssignments:
                        await Actions.CreateRoleAssignments(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.GetAvailableAndFreshSpaces:
                        await Actions.GetAvailableAndFreshSpaces(await SetupHttpClient(Loggers.SilentLogger, appSettings));
                        break;
                    case ActionName.GetOntologies:
                        await Api.GetOntologies(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.GetSpaces:
                        await Actions.GetSpaces(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.ProvisionSample:
                        await Actions.ProvisionSample(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        private static ActionName? ParseArgs(string[] args)
        {
            if (args.Length >= 1 && Enum.TryParse(args[0], out ActionName actionName))
            {
                return actionName;
            }
            else
            {
                // Generate the list of available action names from the enum
                // and output them in the usage string
                var actionNames = Enum.GetNames(typeof(ActionName))
                    .Aggregate((string acc, string s) => acc + " | " + s);
                Console.WriteLine($"Usage: dotnet run [{actionNames}]");

                return null;
            }
        }

        private static async Task<HttpClient> SetupHttpClient(ILogger logger, AppSettings appSettings)
        {
            var httpClient = new HttpClient(new LoggingHttpHandler(logger))
            {
                BaseAddress = new Uri(appSettings.BaseUrl),
            };
            
            // MSAL.NET configuration. Review the product documentation for more information about MSAL.NET authentication options.
            // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/
            IPublicClientApplication app = PublicClientApplicationBuilder
                .Create(appSettings.ClientId)
                .WithRedirectUri(appSettings.AadRedirectUri)
                .WithAuthority(appSettings.Authority)
                .Build();

            AuthenticationResult result = await app
                .AcquireTokenInteractive(appSettings.Scopes)
                .ExecuteAsync();

            Console.WriteLine("MSAL Authentication Token Acquired: {0}", result.AccessToken);
            Console.WriteLine("");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);
            return httpClient;
        }
    }
}