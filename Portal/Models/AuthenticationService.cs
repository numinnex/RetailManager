
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Portal.Models
{
    public class AuthenticationService : IAuthenticationService
    {
        private HttpClient _client;
        private AuthenticationStateProvider _authStateProvider;
        private ILocalStorageService _localStorage;
        private IConfiguration _config;
        private string authToken;

        public AuthenticationService(HttpClient client, AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage, IConfiguration config)
        {
            _client = client;
            _config = config;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            authToken = _config["authTokenStorageKey"];

        }

        public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel user)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type" , "password"),
                new KeyValuePair<string,string>("username" , user.Email),
                new KeyValuePair<string,string>("password" , user.Password)

            });

            string api = _config["api"] + _config["tokenEndPoint"];

            var authResult = await _client.PostAsync(api, data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (!authResult.IsSuccessStatusCode)
                return null;

            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(authContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await _localStorage.SetItemAsync(authToken, result.Access_Token);

            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Access_Token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);

            return result;
        }

        public async Task Logout()
        {

            await _localStorage.RemoveItemAsync(authToken);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogOut();
            _client.DefaultRequestHeaders.Authorization = null;
        }

    }
}
