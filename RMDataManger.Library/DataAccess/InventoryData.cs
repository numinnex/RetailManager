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
    public class InventoryData
    {
        private IConfiguration _config;
        public InventoryData(IConfiguration config)
        {
             _config = config;
        }
        public List<InventoryModel> GetInventory()
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            sql.SaveData("dbo.SpInventory_Insert", item, "RMData");
        }
    }
}
