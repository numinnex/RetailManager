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
        private IProductData _data;
        public ProductController(IProductData data)
        {
            _data = data;

        }
        [HttpGet]
        public List<ProductModel> Get()
        {
            return _data.GetProducts();
        }
    }
}
