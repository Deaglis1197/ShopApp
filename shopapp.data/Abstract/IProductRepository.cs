using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.data.Abstract
{
    public interface IProductRepository:IRepository<Product>
    {
        Product GetProductDetails(string url);
          Product GetProductDetails(int id);
          List<Product> GetProductByCategory(string name,int page,int pageSize);

          List<Product> GetSearchResult(string searchString);
          List<Product> GetPopularProduct();
          List<Product> GetHomePageProduct();
          int GetCountByCategory(string category);
          Product GetByIdWithCategories(int id);
         void Update(Product entity, int[] categoryIds);
    
    }
}