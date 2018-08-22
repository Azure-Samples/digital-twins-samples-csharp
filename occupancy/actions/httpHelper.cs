using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static class HttpHelper
    {
        public static HttpRequestMessage MakeRequest(HttpMethod method, string query, string content = null)
        {
            var request = new HttpRequestMessage(method, query);
            if (content != null)
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            return request;
        }
    }
}