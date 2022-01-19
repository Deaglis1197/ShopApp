using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class ProductModel
    {
        public int ProductID {get;set;}
        public string Name { get; set; }
        [Required(ErrorMessage ="Url is required!")]
        public string Url{get;set;}
    
        public double? Price {get;set;}
        [Required(ErrorMessage ="Image is required!")]
        public string ImageUrl {get;set;}
        public string Description{get;set;} 
        public bool IsHome{get;set;}
        public List<Category> SelectedCategories{get;set;} 
    }
}