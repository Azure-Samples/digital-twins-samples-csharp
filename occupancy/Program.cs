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
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            try
            {
                var appSettings = AppSettings.Load();
                var logger = new ConsoleLogger();

                var actionName = ParseArgs(args);
                if (actionName == null)
                    return;

                var httpClient = await SetupHttpClient(appSettings, logger);

                switch (actionName)
                {
                    case ActionName.GetSpaces:
                        await Actions.GetSpaces(httpClient, logger);
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
                var actionNames = Enum.GetNames(typeof(ActionName))
                    .Aggregate((string acc, string s) => acc + " | " + s);
                Console.WriteLine($"Usage: dotnet run [{actionNames}]");

                return null;
            }
        }

        private static async Task<HttpClient> SetupHttpClient(AppSettings appSettings, Logger logger)
        {
            var httpClient = new HttpClient(new LoggingHttpHandler(logger))
            {
                BaseAddress = new Uri(appSettings.BaseUrl),
            };
            var accessToken = (await Authenticate(appSettings)).AccessToken;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return httpClient;
        }

        private static Task<AuthenticationResult> Authenticate(AppSettings appSettings) =>
            new AuthenticationContext(appSettings.Authority)
                .AcquireTokenAsync(
                    resource: appSettings.Resource,
                    clientCredential: new ClientCredential(appSettings.ClientId, appSettings.ClientSecret));
    }
}