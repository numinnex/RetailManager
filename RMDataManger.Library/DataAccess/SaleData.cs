using RMDataManger.Library.Internal.DataAccess;
using RMDataManger.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RMDataManger.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate()/100;

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

            SQLDataAccess sql = new SQLDataAccess();

            sql.SaveData("dbo.spSale_Insert", sale, "RMData");

            sale.Id = sql.LoadData<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "RMData").FirstOrDefault();


            foreach (var item in details)
            {
                item.SaleId = sale.Id;
                sql.SaveData("dbo.spSaleDetail_Insert", item, "RMData");
            }


        }

    }
}
