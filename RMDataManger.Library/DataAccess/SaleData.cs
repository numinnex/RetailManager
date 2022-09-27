using Microsoft.Extensions.Configuration;
using RMDataManger.Library.Internal.DataAccess;
using RMDataManger.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RMDataManger.Library.DataAccess
{
    public class SaleData
    {
        private IConfiguration _config;
        public SaleData(IConfiguration config)
        {
            _config = config;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData(_config);
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = (new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
                var productInfo = products.GetProductById(detail.ProductId);

                if (productInfo == null)
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in database.");

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                    detail.Tax = (detail.PurchasePrice * taxRate);

                details.Add(detail);
            }
            SaleDBModel sale = new SaleDBModel();

            sale.SubTotal = details.Sum(x => x.PurchasePrice);
            sale.Tax = details.Sum(x => x.Tax);

            sale.Total = sale.SubTotal + sale.Tax;

            sale.CashierId = cashierId;

            using (SQLDataAccess sql = new SQLDataAccess(_config))
            {
                try
                {
                    sql.StartTransaction("RMData");

                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollBackTransaction();
                    throw;
                }
            }
        }
        public List<SaleReportModel> GetSaleReport()
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RMData");

            return output;
        }








    }


}
