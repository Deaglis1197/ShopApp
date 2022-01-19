using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class CategoryModel
    {
    public int CategoryId{get;set;}

       [Required(ErrorMessage ="Url is Required")]
       public string Url{get;set;}
       [Required(ErrorMessage ="Name is Required")]
       public string Name {get;set;}
       public string Desc{get;set;}

       public List<Product> Products{get;set;}
    }
}