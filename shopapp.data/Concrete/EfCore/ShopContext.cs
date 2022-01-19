using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Configurations;

namespace shopapp.data.Concrete.EfCore
{
    public class ShopContext:DbContext
    {
        public ShopContext(DbContextOptions options): base(options){
            
        }
        public DbSet<Product> Products{get;set;}
        public DbSet<Category> Categories{get;set;}
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems{get;set;}
        public DbSet<Order> Orders{get;set;}
        public DbSet<OrderItem> OrderItems{get;set;}

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
            optionsBuilder.UseSqlServer(";Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=appshopDB;Data Source=DESKTOP-NLE4M7D\\SQLEXPRESS;");
        }*/
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
            modelBuilder.Seed();

        }
    }
}