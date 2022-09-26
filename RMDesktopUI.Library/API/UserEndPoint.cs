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

    }
}
