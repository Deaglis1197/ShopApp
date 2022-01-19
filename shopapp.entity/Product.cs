using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace shopapp.entity
{
    public class Product
    {
        public int ProductID {get;set;}
        public string Name { get; set; }
        public string Url{get; set;}
        public double Price {get;set;}
        public string ImageUrl {get;set;}
        public string Description{get;set;}
        public List<ProductCategory> ProductCategories {get;set;}
        public DateTime DateAdded { get; set; }
        public bool IsHome{get;set;}

    }
}