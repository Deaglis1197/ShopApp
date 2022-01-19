using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace shopapp.webui.Models
{
    public class Product
    {
        public int ProductID {get;set;}
        [Required]
        [StringLength(60,MinimumLength =10,ErrorMessage ="Max 60 chracter, Min 10 Chracter !")]
        public string Name { get; set; }
        
        [Required]
        [Range(1,10000)]
        public double Price {get;set;}
        
        [Required]
        public string ImageUrl {get;set;}
        public string Description{get;set;}
    
        [Required]
        public int? CategoryId{get;set;}

    }
}