using Microsoft.Extensions.Configuration;
using RMDataManger.Library.Internal.DataAccess;
using RMDataManger.Library.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RMDataManger.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private ISQLDataAccess _sql;
        private IProductData _productData;
        private IConfiguration _config;
        public SaleData(IProductData productData, ISQLDataAccess sql, IConfiguration config)
        {
            _config = config;
            _sql = sql;
            _productData = productData;
        }
        public decimal GetTaxRate()
        {

            string rateText = _config.GetValue<string>("taxRate");

            bool isValidTaxRate = Decimal.TryParse(rateText, out decimal output);

            if (!isValidTaxRate)
                throw new ConfigurationErrorsException("The tax rate is not setup properly");

            output /= 100;

            return output;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            var taxRate = GetTaxRate(); 

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = (new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
                var productInfo = _productData.GetProductById(detail.ProductId);

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

            try
            {
                _sql.StartTransaction("RMData");

                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);
                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                foreach (var item in details)
                {
                    item.SaleId = sale.Id;
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollBackTransaction();
                throw;
            }
        }
        public List<SaleReportModel> GetSaleReport()
        {

            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RMData");

            return output;
        }








    }


}
