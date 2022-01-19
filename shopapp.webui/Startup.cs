using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.FileProviders;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using shopapp.webui.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using shopapp.webui.EmailServices;
using Microsoft.Extensions.Configuration;

namespace shopapp.webui
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration){
            this._configuration=configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)

        {
            services.AddDbContext<ApplicationContext>(options=>options.UseSqlServer(_configuration.GetConnectionString("SqlServer")));
            services.AddDbContext<ShopContext>(options=>options.UseSqlServer(_configuration.GetConnectionString("SqlServer")).EnableSensitiveDataLogging());
            services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options=>{
                options.Password.RequireDigit=true;
                options.Password.RequireLowercase=true;
                options.Password.RequireUppercase=true;
                options.Password.RequiredLength=6;
                options.Password.RequireNonAlphanumeric=true;
                //lockout"
                options.Lockout.MaxFailedAccessAttempts=5;
                options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers=true;
                //
                options.User.RequireUniqueEmail=true;
                options.SignIn.RequireConfirmedEmail=true;
                options.SignIn.RequireConfirmedPhoneNumber=false;

            });
            services.ConfigureApplicationCookie(options=>{
                options.LoginPath="/account/login";
                options.LogoutPath="/account/login";
                options.AccessDeniedPath="/account/accessdenied";
                options.SlidingExpiration=true;
                options.ExpireTimeSpan=TimeSpan.FromMinutes(60);
                options.Cookie=new CookieBuilder{
                    HttpOnly=true,
                    Name=".Shopapp.Security.Cookie",
                    SameSite=SameSiteMode.Strict
                };
            });
            
            /*services.AddScoped<ICartRepository,EfCoreCartRepository>();
            services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>();
            services.AddScoped<IOrderRepository,EfCoreOrderRepository>();
            services.AddScoped<IProductRepository,EfCoreProductRepository>();*/
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            
            services.AddScoped<ICartService,CartManager>();
            services.AddScoped<IOrderService,OrderManager>();
            services.AddScoped<ICategoryService,CategoryManager>();
            services.AddScoped<IProductService,ProductManager>();
            
            services.AddScoped<IEmailSender,SmtpEmailSender>(i=>new SmtpEmailSender(
                _configuration["EmailSender:Host"],
                _configuration.GetValue<int>("EmailSender:Port"),
                _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                _configuration["EmailSender:UserName"],
                _configuration["EmailSender:Password"]
                )
                );
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IConfiguration configuration,UserManager<User> userManager,RoleManager<IdentityRole> roleManager,ICartService cartService)
        
        
            
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {FileProvider=new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"node_modules")),RequestPath="/modules"
            });
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            SeedIdentity.Seed(userManager,roleManager,cartService,configuration).Wait();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name:"orders",
                    pattern:"orders",
                    defaults:new {controller="Cart",Action="GetOrders"}
                );
                endpoints.MapControllerRoute(
                    name:"StockDebugPage",
                    pattern:"debug",
                    defaults:new {controller="Debug",Action="stockDebugPage"}
                );
                endpoints.MapControllerRoute(
                    name:"CheckOut",
                    pattern:"checkout",
                    defaults:new {controller="Cart",Action="checkout"}
                );
                endpoints.MapControllerRoute(
                    name:"cart",
                    pattern:"cart",
                    defaults:new {controller="Cart",Action="Index"}
                );
                endpoints.MapControllerRoute(
                    name:"adminusers",
                    pattern:"admin/user/delete",
                    defaults:new {controller="Admin",Action="deleteUser"}
                );
                endpoints.MapControllerRoute(
                    name:"adminusers",
                    pattern:"admin/user/list",
                    defaults:new {controller="Admin",Action="UserList"}
                );
                endpoints.MapControllerRoute(
                    name:"adminuseredit",
                    pattern:"admin/user/{id?}",
                    defaults:new {controller="Admin",Action="UserEdit"}
                );
                
                
                endpoints.MapControllerRoute(
                    name:"adminproduct",
                    pattern:"admin/products",
                    defaults:new {controller="Admin",Action="ProductList"}
                );
                endpoints.MapControllerRoute(
                    name:"adminrolelist",
                    pattern:"admin/role/list",
                    defaults:new {controller="Admin",Action="RoleList"}
                );
                endpoints.MapControllerRoute(
                    name:"adminrolecreate",
                    pattern:"admin/role/create",
                    defaults:new {controller="Admin",Action="RoleCreate"}
                );
                 endpoints.MapControllerRoute(
                    name:"adminroleedit",
                    pattern:"admin/role/{id?}",
                    defaults:new {controller="Admin",Action="RoleEdit"}
                );
                endpoints.MapControllerRoute(
                    name:"adminproduct",
                    pattern:"admin/products/create",
                    defaults:new {controller="Admin",Action="ProductCreate"}
                );
                endpoints.MapControllerRoute(
                    name:"adminproductedit",
                    pattern:"admin/products/{id?}",
                    defaults:new {controller="Admin",Action="ProductEdit"}
                );
                endpoints.MapControllerRoute(
                    name:"admincategories",
                    pattern:"admin/categories",
                    defaults:new {controller="Admin",Action="CategoryList"}
                );
                endpoints.MapControllerRoute(
                    name:"admincategorycreate",
                    pattern:"admin/categories/create",
                    defaults:new {controller="Admin",Action="CategoryCreate"}
                );
                endpoints.MapControllerRoute(
                    name:"admincategoryedit",
                    pattern:"admin/categories/{id?}",
                    defaults:new {controller="Admin",Action="CategoryEdit"}
                );
                
                endpoints.MapControllerRoute(
                    name:"search",
                    pattern:"search",
                    defaults:new {controller="Shop",action="search"}
                );
                endpoints.MapControllerRoute(
                    name:"products",
                    pattern:"products",
                    defaults:new {controller="Shop",action="list"}
                );
                endpoints.MapControllerRoute(
                    name:"productsdetails",
                    pattern:"{url}",
                    defaults:new {controller="Shop",action="details"}
                );
                endpoints.MapControllerRoute(
                    name:"products",
                    pattern:"products/{category?}",
                    defaults:new {controller="Shop",action="list"}
                );
                
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern:"{controller=Home}/{action=index}/{id?}"
                    );
                endpoints.MapControllerRoute(
                    name:"about",
                    pattern:"home",
                    defaults:new {controller="Home",Action="About"}
                );
            });
        }
    }
}
