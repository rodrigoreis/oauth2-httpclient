using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Extensions;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace OAuth2.Net.Http.UnitTest
{
    public class OAuthNetHttpUnitTest
    {
        private readonly OAuth2HttpClient _httpClient;

        private const string TokenUrl = "http://my.identity.server/token";

        private const string FormUrlEncoded =
            "grant_type=password&client_id=client-id&client_secret=dGhpcy1pcy1hLWNsaWVudC1zZWNyZXQ%3d&scope=scope1+scope2&username=username&password=p%405%24w0Rd";

        public OAuthNetHttpUnitTest()
        {
            _httpClient = new OAuth2HttpClient
            {
                ClientId = "client-id",
                ClientSecret = "dGhpcy1pcy1hLWNsaWVudC1zZWNyZXQ=",
                Username = "username",
                Password = "p@5$w0Rd",
                GrantType = "password"
            };

            _httpClient.Scope.Add("scope1");
            _httpClient.Scope.Add("scope2");
        }

        [Fact]
        public void Must_Get_OAuth2_HttpClient_Handler_With_Success()
        {
            var handler = _httpClient.GetHandler();

            handler.Should().NotBeNull();
            handler.Client.Should().NotBeNull();
            handler.Client.Should().Be(_httpClient);
            handler.GetType().Should().Be(typeof(OAuth2HttpClientHandler));
        }

        [Fact]
        public void Must_Get_Build_Http_Token_Uri_With_Success()
        {
            _httpClient.TokenUrl = TokenUrl;

            var tokenUri = _httpClient.BuildTokenUri();

            tokenUri.ToString().Should().BeEquivalentTo(TokenUrl);
        }

        [Fact]
        public void Must_Get_Build_Null_Token_Uri_With_Success()
        {
            _httpClient.BaseAddress = new Uri("http://my.identity.server/");
            var tokenUri = _httpClient.BuildTokenUri();

            tokenUri.ToString().Should().BeEquivalentTo(TokenUrl);
        }

        [Fact]
        public void Must_Thow_When_Get_Build_Null_Token_Uri()
        {
            _httpClient.Invoking(m => m.BuildTokenUri()).Should().Throw<OperationCanceledException>();
        }

        [Fact]
        public void Must_Get_Build_Token_Uri_With_Success()
        {
            const string tokenUrl = "http://my.identity.server/my-token";
            _httpClient.BaseAddress = new Uri("http://my.identity.server/");
            _httpClient.TokenUrl = "/my-token";
            var tokenUri = _httpClient.BuildTokenUri();

            tokenUri.ToString().Should().BeEquivalentTo(tokenUrl);
        }

        [Fact]
        public async Task Must_Get_Build_FormUrlEncoded_Content_With_Success()
        {
            var content = _httpClient.BuildFormUrlEncodedContent();
            var formData = await content.ReadAsFormDataAsync();

            content.Should().NotBeNull();
            formData.ToString().Should().BeEquivalentTo(FormUrlEncoded);
        }

        [Fact]
        public async Task Must_Get_Build_Request_Token_HttpRequestMessage_With_Success()
        {
            _httpClient.BaseAddress = new Uri("http://my.identity.server/");
            var message = _httpClient.BuildRequestTokenHttpRequestMessage();
            var formData = await message.Content.ReadAsFormDataAsync();

            message.Should().NotBeNull();
            message.Method.ToString().Should().BeEquivalentTo("POST");
            message.RequestUri.ToString().Should().BeEquivalentTo(TokenUrl);
            formData.ToString().Should().BeEquivalentTo(FormUrlEncoded);
        }
    }
}
