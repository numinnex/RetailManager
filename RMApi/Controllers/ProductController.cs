using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMDataManger.Library.DataAccess;
using RMDataManger.Library.Models;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        private IConfiguration _configuration;
        public ProductController(IConfiguration config)
        {
            _configuration = config;

        }
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData(_configuration);

            return data.GetProducts();
        }
    }
}
