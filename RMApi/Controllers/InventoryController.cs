using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMDataManger.Library.DataAccess;
using RMDataManger.Library.Models;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private IConfiguration _config;
        public InventoryController(IConfiguration config)
        {
            _config = config;
           
        }
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData(_config);

            return data.GetInventory();
        }
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData(_config);

            data.SaveInventoryRecord(item);
        }



    }
}
