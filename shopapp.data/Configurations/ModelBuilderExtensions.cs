using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.data.Configurations
{
    public static class ModelBuilderExtensions
    {
        enum categoies{Phones=1,Computer,Electronic,HomeElectronic}
        public static void Seed(this ModelBuilder modelBuilder){
            modelBuilder.Entity<Product>().HasData(
                new Product(){ProductID=1,Name="Iphone 9",Price=2000,ImageUrl="1.jpg",Description="Good", Url="Iphone-9"},
                new Product(){ProductID=2,Name="Iphone X",Price=5000,ImageUrl="2.jpg",Description="Fine", Url="Iphone-X"},
                new Product(){ProductID=3,Name="Dell",Price=4000,ImageUrl="dell.jpg",Description="Like New", Url="Dell"}
            );
            modelBuilder.Entity<Category>().HasData(
                new Category(){CategoryId=1,Name="Phone",Url="phones"},
                new Category(){CategoryId=2,Name="Computer",Url="computer"},
                new Category(){CategoryId=3,Name="Electronic",Url="electronic"},
                new Category(){CategoryId=4,Name="Home Electronic",Url="home-electronic"}
            );
            
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory(){ProductID=1,CategoryId=((int)categoies.Phones)},
                new ProductCategory(){ProductID=2,CategoryId=((int)categoies.Phones)},
                new ProductCategory(){ProductID=3,CategoryId=((int)categoies.Computer)}
          );

        }
    }
}