using Microsoft.Extensions.Configuration;
using RMDataManger.Library.Internal.DataAccess;
using RMDataManger.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManger.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private IConfiguration _config;
        private ISQLDataAccess _sql;
        public InventoryData(IConfiguration config , ISQLDataAccess sql)
        {
            _config = config;
            _sql = sql;
        }
        public List<InventoryModel> GetInventory()
        {

            var output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {

            _sql.SaveData("dbo.SpInventory_Insert", item, "RMData");
        }
    }
}
