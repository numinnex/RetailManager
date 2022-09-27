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
    public class ProductData
    {
        private IConfiguration _config;
        public ProductData(IConfiguration config)
        {
            _config = config;
        }
        public List<ProductModel> GetProducts()
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            var output = sql.LoadData<ProductModel , dynamic>("dbo.spProduct_GetAll", new { }, "RMData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            var output = sql.LoadData<ProductModel , dynamic>("dbo.spProduct_GetById", new { Id = productId }, "RMData").FirstOrDefault();

            return output;

        }

    }
}
