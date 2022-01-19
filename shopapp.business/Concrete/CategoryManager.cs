using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _unitofWork;
        public CategoryManager(IUnitOfWork unitofWork){
            _unitofWork=unitofWork;
        }

        public string ErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Create(Category entity)
        {
            _unitofWork.Categories.Create(entity);
            _unitofWork.Save();
        }

        public void Delete(Category entity)
        {
            _unitofWork.Categories.Delete(entity);
            _unitofWork.Save();
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            _unitofWork.Categories.DeleteFromCategory(productId,categoryId);
            _unitofWork.Save();
        }

        public List<Category> GetAll()
        {
            return _unitofWork.Categories.GetAll();
        }

        public Category GetById(int id)
        {
            return _unitofWork.Categories.GetById(id);
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            return _unitofWork.Categories.GetByWithProducts(categoryId);
        }

        public void Update(Category entity)
        {
            _unitofWork.Categories.Update(entity);
            _unitofWork.Save();
        }

        public bool Validation(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}