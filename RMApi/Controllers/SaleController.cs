using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
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
        private IConfiguration _config;
        public SaleController(IConfiguration config)
        {
            _config = config;

        }
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  //RequestContext.Principal.Identity.GetUserId();

            SaleData data = new SaleData(_config);
            data.SaveSale(sale, userId);

        }
        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            SaleData data = new SaleData(_config);

            return data.GetSaleReport();
        }

    }
}
