using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RMDesktopUI.Library.API;
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
        private readonly IAPIHelper _apiHelper;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration config, IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
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
            bool isAuthenticated = await NotifyUserAuthentication(token);

            if(!isAuthenticated)
                return _anonymous;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal
                (new ClaimsIdentity
                (JWTParser.ParseClaimsFromJWT(token), "jwtAuthType")));
        }

        public async Task<bool> NotifyUserAuthentication(string token)
        {
            bool result;
            Task<AuthenticationState>? authState;
            try
            {
                await _apiHelper.GetLoggedInUserInfo(token);

                var authenticatedUser = (new ClaimsPrincipal
                    (new ClaimsIdentity
                    (JWTParser.ParseClaimsFromJWT(token), "jwtAuthType")));

                authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                await NotifyUserLogOut();
                result = false;
            }

            return result;

        }

        public async Task NotifyUserLogOut()
        {

            await _localStorage.RemoveItemAsync(authToken);
            var authState = Task.FromResult(_anonymous);
            _apiHelper.LogOffUser();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            NotifyAuthenticationStateChanged(authState);
        }
    }
}
