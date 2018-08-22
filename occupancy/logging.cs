using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static class Logging
    {
        enum Level {
            Normal,
            Verbose
        }

        private static Level _level = Level.Normal;

        public static void LogRequest(HttpRequestMessage request)
        {
            if (_level == Level.Verbose)
            {
                Console.WriteLine($"Request: {Serialize(request)}");
            }
            else
            {
                Console.WriteLine($"Request: {request.Method} {request.RequestUri}");
            }
        }

        public static async Task LogResponse(HttpResponseMessage response)
        {
            if (_level == Level.Verbose)
            {
                Console.WriteLine($"Response: {Serialize(response)}");
                var content = await response.Content?.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {content}");
            }
            else
            {
                const int maxContentLength = 200;
                var content = await response.Content?.ReadAsStringAsync();
                var contentMaxLength = content == null || content.Length < maxContentLength
                    ? content
                    : content.Substring(0, maxContentLength - 3) + "...";
                var contentDisplay = contentMaxLength == null ? "" : $", {contentMaxLength}";
                Console.WriteLine($"Response Status: {(int)response.StatusCode}, {response.StatusCode}{contentDisplay}");
            }
        }

        private static string Serialize(object o)
            => JsonConvert.SerializeObject(o, Formatting.Indented);
    }
}