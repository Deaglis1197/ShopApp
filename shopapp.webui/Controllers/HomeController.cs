using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    public class HomeController:Controller
    {
        private IProductService _productService;
        public HomeController(IProductService productService){
            this._productService=productService;
        }
        public IActionResult Index(){
            var productViewModel= new ProductListViewModel(){
                Products = _productService.GetHomePageProduct()
            };
            return View(productViewModel);
        }
        public IActionResult About(){

            return View();
        }
    }
}