using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.webui.Models;
namespace shopapp.webui.Data
{
    public class CategoryRepository
    {
         private static List<Category> _categories=null;
        static CategoryRepository(){
            _categories=new List<Category>{
                new Category{CategoryId=1,Name="Phones",Desc="Phone Status"},
                new Category{CategoryId=2,Name="Computer",Desc="Computer Status"},
                new Category{CategoryId=3,Name="Electronic",Desc="Electronic Status"}
            };
        }
        public static List<Category> Categories{
            get{return _categories;}
        }
        public static void AddCategory(Category category){
            _categories.Add(category);
        }
        public static Category GetProductById(int Id){
            return _categories.FirstOrDefault(c=>c.CategoryId==Id);
        }
    }
}