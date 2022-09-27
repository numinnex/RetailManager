using RMDataManger.Library.Models;
using System.Collections.Generic;

namespace RMDataManger.Library.DataAccess
{
    public interface IProductData
    {
        ProductModel GetProductById(int productId);
        List<ProductModel> GetProducts();
    }
}