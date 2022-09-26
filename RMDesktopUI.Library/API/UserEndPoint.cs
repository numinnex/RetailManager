using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.API
{
    public class UserEndPoint : IUserEndPoint
    {
        private IAPIHelper _apiHelper;
        public UserEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }


        public async Task<List<ApplicationUserModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllUsers"))
            {
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsAsync<List<ApplicationUserModel>>();
                else
                    throw new Exception(response.ReasonPhrase);
            }

        }
        public async Task<Dictionary<string,string>> GetAllRoles()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllRoles"))
            {
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsAsync<Dictionary<string,string>>();
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task AddUserToRole(string userId, string roleName)
        {
            var data = new {userId = userId , Role = roleName};

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/AddRole", data))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);
            }
        }
        public async Task RemoveUserFromRole(string userId, string roleName)
        {
            var data = new {userId = userId , Role = roleName};

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/RemoveFromRole", data))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);
            }
        }


    }
}
