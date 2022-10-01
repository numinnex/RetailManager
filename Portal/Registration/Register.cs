using RMDesktopUI.Library.Models;
using System.Net.Http.Headers;

namespace Portal.Registration
{
    public class Register : IRegister
    {
        private HttpClient _httpClient;
        private IConfiguration _config;

        public Register(IConfiguration config)
        {
            _config = config;

            string api = _config["api"];

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(api);
            //_httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task RegisterUser(CreateUserModel user)
        {
            var data = new { FirstName =  user.FirstName, LastName =  user.LastName, EmailAdress =  user.EmailAdress,  Password = user.Password };

            using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/User/Test", new { user.FirstName, user.LastName , user.EmailAdress, user.Password })) 
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception("doesnt work");
            }

        }
    }
}
