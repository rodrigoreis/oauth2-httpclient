using System.Net.Http.Extensions;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class OAuth2HttpClientHandler : HttpClientHandler
    {
        public OAuth2HttpClient Client { get; set; }

        public virtual async Task RequestTokenAsync(CancellationToken cancellationToken)
        {
            if (Client.IsValid)
                return;

            var response = await base.SendAsync(Client.BuildRequestTokenHttpRequestMessage(), cancellationToken);
            var result = await response.Content.ReadAsAsync<dynamic>(cancellationToken);

            Client.AccessToken = new AccessToken(
                result.access_token,
                result.token_type,
                DateTime.UtcNow,
                TimeSpan.FromSeconds(result.expires_in)
            );
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken)
        {
            await RequestTokenAsync(cancellationToken);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(Client.AuthenticationType, Client.AccessToken.Value);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
