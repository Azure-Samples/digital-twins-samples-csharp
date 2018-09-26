using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    internal static class Authentication
    {
        internal static async Task<string> GetToken(
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