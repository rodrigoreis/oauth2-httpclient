using System.Collections.Generic;
using System.Reflection;

namespace System.Net.Http.Extensions
{
    internal static class OAuth2HttpClientExtensions
    {
        public static OAuth2HttpClientHandler GetHandler(this OAuth2HttpClient httpClient)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var httpClientBaseType = typeof(HttpMessageInvoker);
            var field = httpClientBaseType.GetField("handler", flags) ?? httpClientBaseType.GetField("_handler", flags);
            return (OAuth2HttpClientHandler) field?.GetValue(httpClient);
        }

        private static Uri BuildTokenUri(this OAuth2HttpClient httpClient)
        {
            var tempTokenUrl = (httpClient.TokenUrl ?? "/token");

            var tokenUrl = tempTokenUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? tempTokenUrl
                : $"{httpClient.BaseAddress}{tempTokenUrl}";

            return new Uri(tempTokenUrl);
        }

        private static FormUrlEncodedContent BuildFormUrlEncodedContent(this OAuth2HttpClient httpClient)
        {
            var payload = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", httpClient.GrantType),
                new KeyValuePair<string, string>("client_id", httpClient.ClientId),
                new KeyValuePair<string, string>("client_secret", httpClient.ClientSecret),
                new KeyValuePair<string, string>("scope", string.Join(" ", httpClient.Scope)),
            };

            if (httpClient.HasCredentials)
            {
                payload.Add(new KeyValuePair<string, string>("username", httpClient.Username));
                payload.Add(new KeyValuePair<string, string>("password", httpClient.Password));
            }

            return new FormUrlEncodedContent(payload);
        }

        public static HttpRequestMessage BuildRequestTokenHttpRequestMessage(this OAuth2HttpClient httpClient)
        {
            return new HttpRequestMessage(HttpMethod.Post, httpClient.BuildTokenUri())
            {
                Content = httpClient.BuildFormUrlEncodedContent()
            };
        }
    }
}
