using RMDataManger.Library.Models;
using System.Collections.Generic;

namespace RMDataManger.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryModel> GetInventory();
        void SaveInventoryRecord(InventoryModel item);
    }
}