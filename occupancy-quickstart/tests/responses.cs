using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public static class Responses
    {
        public static HttpResponseMessage NotFound = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
        };

        public static HttpResponseMessage OK = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
        };
    }
}