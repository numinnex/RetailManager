using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMDataManger.Library.DataAccess;
using RMDataManger.Library.Models;
using System.Security.Claims;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private ISaleData _data;
        public SaleController( ISaleData data)
        {
            _data = data;

        }
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            _data.SaveSale(sale, userId);

        }
        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            return _data.GetSaleReport();
        }

    }
}
