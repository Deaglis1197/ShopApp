using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class ProductManager : IProductService
    {
        private IUnitOfWork _unitofWork;

        

        public ProductManager(IUnitOfWork unitofWork){
            _unitofWork=unitofWork;
        }

        public bool Create(Product entity)
        {
            if(Validation(entity)){
                _unitofWork.Products.Create(entity);
                 _unitofWork.Save();
                return true;
            }
            return false;
            
        }

        public void Delete(Product entity)
        {
             _unitofWork.Products.Delete(entity);
              _unitofWork.Save();

        }
        public List<Product> GetAll()
        {
            return  _unitofWork.Products.GetAll();
        }

        public Product GetById(int id)
        {
           return  _unitofWork.Products.GetById(id);
        }

        public List<Product> GetProductByCategory(string name,int page,int pageSize)
        {
            return  _unitofWork.Products.GetProductByCategory(name,page,pageSize);
        }

        public int GetCountByCategory(string category)
        {
            return  _unitofWork.Products.GetCountByCategory(category);
        }

        public Product GetProductDetails(string url)
        {
            return  _unitofWork.Products.GetProductDetails(url);
        }

        public void Update(Product entity)
        {
            
             _unitofWork.Products.Update(entity);
              _unitofWork.Save();
        }

        public List<Product> GetHomePageProduct()
        {
            return  _unitofWork.Products.GetHomePageProduct();
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return  _unitofWork.Products.GetSearchResult(searchString);
        }

        public Product GetByIdWithCategories(int id)
        {
            return  _unitofWork.Products.GetByIdWithCategories(id);
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if(Validation(entity)){
                if(categoryIds.Length==0){
                    ErrorMessage+="Dont Select any Category!";
                    return false;
                }
                 _unitofWork.Products.Update(entity,categoryIds);
                  _unitofWork.Save();
                return true;
            }
            return false;
        }
        public string ErrorMessage { get; set; }
        public bool Validation(Product entity)
        {
            var isValid=true;
            if(string.IsNullOrEmpty(entity.Name)){
                ErrorMessage+="Name is null.\n";
                isValid=false;
            }
            if(entity.Price<0){
                ErrorMessage+="Price cant be negative!\n";
                isValid=false;
            }
            return isValid;
        }
    }
}