using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using shopapp.webui.Models;

namespace shopapp.webui.Data
{
    public class ProductRepository
    {
        private static List<Product> _product=null;
        static ProductRepository(){
            _product=new List<Product>{
                new Product {ProductID=1,Name="Iphone 9",Price=5000,Description="good",ImageUrl="1.jpg",CategoryId=1},
                new Product {ProductID=2,Name="Iphone X",Price=7000,Description="fine",ImageUrl="2.jpg",CategoryId=1},
                new Product {ProductID=3,Name="Lenovo",Price=3000,Description="good",ImageUrl="lenovo.jpg",CategoryId=2},
                new Product {ProductID=4,Name="Dell",Price=5000,Description="fine",ImageUrl="dell.jpg",CategoryId=2}
            };
        }
        public static List<Product> Products{
            get{return _product;}
        }
        public static void EditProduct(Product product){
            foreach(var p in _product){
                if(p.ProductID==product.ProductID){
                    p.Name=product.Name;
                    p.Price=product.Price;
                    p.ImageUrl=product.ImageUrl;
                    p.Description=product.Description;
                    p.CategoryId=product.CategoryId;

                }
            }
        }
        public static void AddProduct(Product product){
            _product.Add(product);
        }
        public static Product GetProductById(int id){
            return _product.FirstOrDefault(p=>p.ProductID==id);
        }
        public static void DeleteProduct(int id){
            var product = GetProductById(id);
            if(product!=null){
                _product.Remove(product);
            }
        }
    }
}