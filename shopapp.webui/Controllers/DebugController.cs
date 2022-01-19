using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Extensions;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [Authorize(Roles="Admin")]
     
    public class DebugController:Controller
    {
        private ICategoryService _categoryService;
        private IProductService _productService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        public DebugController(IProductService productService,
                               ICategoryService categoryService,
                               RoleManager<IdentityRole> roleManager,
                               UserManager<User> userManager){
            _productService=productService;
            _categoryService = categoryService;
            _roleManager=roleManager;
            _userManager=userManager;
        }
        public IActionResult stockDebugPage(){
            return View();
        } 
    }
}