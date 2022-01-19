using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public static class SeedDatabase
    {
        enum categoies{Phones,Computer,Electronic,HomeElectronic}
       /* public static void Seed(){
            var context =new ShopContext();
            if(context.Database.GetPendingMigrations().Count()==0){
                if(context.Categories.Count()==0){
                    context.Categories.AddRange(Categories);
                }
            
            if(context.Products.Count()==0){
                    context.Products.AddRange(Products);
                    context.AddRange(ProductCategories);
                }
                
            }
            
            
            context.SaveChanges();
        }*/
       
    }
}