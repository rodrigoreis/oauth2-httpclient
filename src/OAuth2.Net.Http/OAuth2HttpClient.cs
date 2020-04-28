using System.Collections.Generic;
using System.Net.Http.Extensions;

namespace System.Net.Http
{
    public class OAuth2HttpClient : HttpClient
    {
        private readonly OAuth2HttpClientHandler _handler;

        public string ClientId { get; set; }
        
        public string ClientSecret { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public IList<string> Scope { get; set; }
        
        public string GrantType { get; set; }
        
        public string TokenUrl { get; set; }
        
        public string AuthenticationType { get; set; } = "Bearer";
        
        public AccessToken AccessToken { get; set; }
        
        public bool IsValid => AccessToken != null && !AccessToken.HasExpired;

        public bool HasCredentials => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        public OAuth2HttpClient() : base(new OAuth2HttpClientHandler())
        {
            _handler = this.GetHandler();
            _handler.Client = this;
            Scope = new List<string>();
        }
    }
}
