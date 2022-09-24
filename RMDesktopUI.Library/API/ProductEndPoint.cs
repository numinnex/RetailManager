using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.API
{
    public class ProductEndPoint : IProductEndPoint
    {
        private IAPIHelper _apiHelper;
        public ProductEndPoint(IAPIHelper apIHelper)
        {
            _apiHelper = apIHelper;
        }
        public async Task<List<ProductModel>> GetAll()
        {

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Product"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<ProductModel>>();

                    return result;
                }
                else
                    throw new Exception(response.ReasonPhrase);

            }
        }
    }
}
