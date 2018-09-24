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

namespace Microsoft.Azure.DigitalTwins.Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var appSettings = AppSettings.Load();

                var actionName = ParseArgs(args);
                if (actionName == null)
                    return;

                var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory()
                    .AddConsole(LogLevel.Trace);
                var logger = loggerFactory.CreateLogger("DigitalTwinsQuickstart");
                var httpClient = await SetupHttpClient(appSettings, logger);

                switch (actionName)
                {
                    case ActionName.GetOccupancy:
                        await Actions.GetOccupancy(httpClient, logger);
                        break;
                    case ActionName.GetOntologies:
                        await Api.GetOntologies(httpClient, logger);
                        break;
                    case ActionName.GetSpaces:
                        await Actions.GetSpaces(httpClient, logger);
                        break;
                    case ActionName.ProvisionSample:
                        await Actions.ProvisionSample(httpClient, logger);
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
            if (args.Length == 1 && Enum.TryParse(args[0], out ActionName actionName))
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

        private static async Task<HttpClient> SetupHttpClient(AppSettings appSettings, ILogger logger)
        {
            var httpClient = new HttpClient(new LoggingHttpHandler(logger))
            {
                BaseAddress = new Uri(appSettings.BaseUrl),
            };
            var accessToken = (await Authenticate(appSettings)).AccessToken;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return httpClient;
        }

        private static Task<Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult> Authenticate(AppSettings appSettings) =>
            new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(appSettings.Authority)
                .AcquireTokenAsync(
                    resource: appSettings.Resource,
                    clientCredential: new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(appSettings.ClientId, appSettings.ClientSecret));
    }
}