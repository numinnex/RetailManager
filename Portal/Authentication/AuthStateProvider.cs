using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Security;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Portal.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private HttpClient _httpClient;
        private ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;
        private IConfiguration _config;
        private string authToken;
        
        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration config)
        {
            _config = config;
            _httpClient = httpClient;
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            authToken = _config["authTokenStorageKey"];


        }



        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(authToken);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal
                (new ClaimsIdentity
                (JWTParser.ParseClaimsFromJWT(token), "jwtAuthType")));
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = (new ClaimsPrincipal
                (new ClaimsIdentity
                (JWTParser.ParseClaimsFromJWT(token), "jwtAuthType")));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogOut()
        {
            var authState = Task.FromResult(_anonymous);

            NotifyAuthenticationStateChanged(authState);
        }
    }
}
