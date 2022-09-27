using RMDataManger.Library.Models;
using System.Collections.Generic;

namespace RMDataManger.Library.DataAccess
{
    public interface ISaleData
    {
        List<SaleReportModel> GetSaleReport();
        void SaveSale(SaleModel saleInfo, string cashierId);
    }
}