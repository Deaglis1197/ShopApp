using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.data;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface IProductService:IValidator<Product>
    {
        List<Product> GetProductByCategory(string name,int page,int pageSize);
        Product GetProductDetails(string url);
        Product GetById(int id);
        List<Product> GetAll();

        bool Create(Product entity);
        void Update(Product entity);

        bool Update(Product entity, int[] categoryIds);
        void Delete(Product entity);
        List<Product> GetHomePageProduct();

        List<Product> GetSearchResult(string searchString);
        int GetCountByCategory(string category);

        Product GetByIdWithCategories(int id);
    }
}