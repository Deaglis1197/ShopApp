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
 
    public class AdminController:Controller
    {
        private ICategoryService _categoryService;
        private IProductService _productService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        public AdminController(IProductService productService,
                               ICategoryService categoryService,
                               RoleManager<IdentityRole> roleManager,
                               UserManager<User> userManager){
            _productService=productService;
            _categoryService = categoryService;
            _roleManager=roleManager;
            _userManager=userManager;
        }
        public async Task<IActionResult> UserEdit(string id){
            var user=await _userManager.FindByIdAsync(id);
            if(user!=null){
                var selectedRoles= await _userManager.GetRolesAsync(user);
                var roles=_roleManager.Roles.Select(i=>i.Name);
                ViewBag.Roles=roles;
                return View(new UserDetailsModel(){
                    UserId=user.Id,
                    UserName=user.UserName,
                    FirstName=user.FirstName,
                    LastName=user.LastName,
                    Email=user.Email,
                    EmailConfirmed=user.EmailConfirmed,
                    SelectedRoles=selectedRoles
                });
            }
            return Redirect("~/admin/user/list");
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model,string[] selectedRoles){
            if(ModelState.IsValid){
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null){
                    user.FirstName=model.FirstName;
                    user.LastName=model.LastName;
                    user.UserName=model.UserName;
                    user.Email=model.Email;
                    user.EmailConfirmed=model.EmailConfirmed;
                    var result=await _userManager.UpdateAsync(user);
                    if(result.Succeeded){
                        var userRoles=await _userManager.GetRolesAsync(user);
                        selectedRoles=selectedRoles??new string[]{};
                        await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());
                        return Redirect("/admin/user/list");
                    }
                }
                return Redirect("/admin/user/list");
            }
            return View(model);    
        }
        public IActionResult UserList(){
            return View(_userManager.Users);
        }
        public async Task<IActionResult> RoleEdit(string id){
            var role= await _roleManager.FindByIdAsync(id);
            var members=new List<User>();
            var nonmembers=new List<User>();
            foreach(var user in _userManager.Users){
                var list=await _userManager.IsInRoleAsync(user,role.Name)?members:nonmembers;
                list.Add(user);
            }
            var model=new RoleDetails(){
                Role=role,
                Members=members,
                NonMembers=nonmembers
            };  
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model){
            if(ModelState.IsValid){
                foreach(var userId in model.IdsToAdd ?? new string[]{}){
                    var user=await _userManager.FindByIdAsync(userId);
                    if(user!=null){
                        var result=await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded){
                            foreach(var error in result.Errors){
                                ModelState.AddModelError("",error.Description);
                            }
                        }
                    }
                }
                foreach(var userId in model.IdsToDelete ?? new string[]{}){
                    var user=await _userManager.FindByIdAsync(userId);
                    if(user!=null){
                        var result=await _userManager.RemoveFromRoleAsync(user,model.RoleName);
                        if(!result.Succeeded){
                            foreach(var error in result.Errors){
                                ModelState.AddModelError("",error.Description);
                            }
                        }
                    } 
            }
            }
            return Redirect("/admin/role/"+model.RoleId);
            }
        public IActionResult RoleList(){
            return View(_roleManager.Roles);
        }
        public IActionResult RoleCreate(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model){
            if(ModelState.IsValid){
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if(result.Succeeded){
                    return RedirectToAction("RoleList");
                }else{
                    foreach(var error in result.Errors){
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            return View(model);
        }
        public IActionResult CategoryList(){
            return View(new CategoryListViewModel(){
                Categories=_categoryService.GetAll()
            });     
        }
        [HttpGet]
        public IActionResult ProductList(){
            return View(new ProductListViewModel(){
                Products= _productService.GetAll()
            });     
        }
         [HttpGet]
        public IActionResult CategoryCreate(){
             return View();
         }
        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model){
           if(ModelState.IsValid){
                var entity=new Category(){
                Name=model.Name,
                Url=model.Url,
                Desc=model.Desc,

            };
            _categoryService.Create(entity);
            var msg = new AlertMessage(){
                 Message= $"{entity.Name} category has been added",
                 AlertType= "success"
             };
             TempData["message"]=JsonConvert.SerializeObject(msg);
             return RedirectToAction("CategoryList");
           }
           return View(model);
         }
         [HttpGet]
         public IActionResult CategoryEdit(int? id){
             if(id==null){
                 return NotFound();
            }
            var entity=_categoryService.GetByIdWithProducts((int)id);
            if(entity==null){
                 return NotFound();
            }
            var model=new CategoryModel(){
                CategoryId=entity.CategoryId,
                Name=entity.Name,
                Url=entity.Url,
                Desc=entity.Desc,
                Products=entity.ProductCategories.Select(p=>p.Product).ToList()
            };
             return View(model);
         }
         [HttpPost]
         public IActionResult CategoryEdit(CategoryModel Model){
             if(ModelState.IsValid){
              var entity=_categoryService.GetById(Model.CategoryId);
             if(entity==null){
                 return NotFound();
             }
             entity.Name=Model.Name;
             entity.Url=Model.Url;
             entity.Desc=Model.Desc;
             _categoryService.Update(entity);
             var msg= new AlertMessage(){
                 Message= $"{entity.Name} category has been updated.",
                 AlertType= "warning"
             };
             TempData["message"]=JsonConvert.SerializeObject(msg);
             return RedirectToAction("CategoryList");   
             }
             return View(Model);
             
         }
            public IActionResult deletecategory(int CategoryId){
             var entity=_categoryService.GetById(CategoryId);
             if(entity!=null){
                 _categoryService.Delete(entity);
             }
             var msg= new AlertMessage(){
                 Message= $"{entity.Name} category has been deleted.",
                 AlertType= "danger"
             };
            TempData["message"]=JsonConvert.SerializeObject(msg);
             return RedirectToAction("CategoryList");
         }
         
        [HttpGet]
        public IActionResult ProductCreate(){
             return View();
         }
        [HttpPost]
        public IActionResult ProductCreate(ProductModel model){
            
            if(ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name = model.Name,
                    Url = model.Url,
                    Price = (double)model.Price,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl

                };
                if (_productService.Create(entity))
                {
                    TempData.Put("message",new AlertMessage(){
                    Title="Add Product",
                    Message="Add Product",
                    AlertType="success"
                });
                    return RedirectToAction("ProductList");
                }
                TempData.Put("message",new AlertMessage(){
                    Title=_productService.ErrorMessage,
                    Message=_productService.ErrorMessage,
                    AlertType="danger"
                    });
                
                return View(model);
            }
            return View(model);
         }

        [HttpGet]
         public IActionResult ProductEdit(int? id){
            
             if(id==null){
                 return NotFound();
            }
            var entity=_productService.GetByIdWithCategories((int)id);
            if(entity==null){
                 return NotFound();
            }
            var model=new ProductModel(){
                ProductID=entity.ProductID,
                Name=entity.Name,
                Url=entity.Url,
                Price=entity.Price,
                ImageUrl=entity.ImageUrl,
                Description=entity.Description,
                SelectedCategories=entity.ProductCategories.Select(i=>i.Category).ToList(),
                IsHome=entity.IsHome
            };
            ViewBag.Categories= _categoryService.GetAll();
             return View(model);
         }
         [HttpPost]
         public async Task<IActionResult> ProductEdit(ProductModel Model,int[] categoryIds,IFormFile file){
             if(ModelState.IsValid){
                  var entity=_productService.GetById(Model.ProductID);
             if(entity==null){
                 return NotFound();
             }
             entity.Name=Model.Name;
             entity.Price=(double)Model.Price;
             entity.Url=Model.Url;
             entity.Description=Model.Description;
             entity.IsHome=Model.IsHome;
             if(file!=null){
                 var extention=Path.GetExtension(file.FileName);
                 var randomName=string.Format($"{Guid.NewGuid()}{extention}"); //Guid.NewGuid()DateTime.Now.Ticks
                 entity.ImageUrl=randomName; 
                 var path=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",randomName);
                 using(var stream=new FileStream(path,FileMode.Create)){
                     await file.CopyToAsync(stream);
                 }
             }
             if (_productService.Update(entity,categoryIds))
                {
                    TempData.Put("message",new AlertMessage(){
                    Title="Product Updated",
                    Message="Product Updated.",
                    AlertType="success"
                });
                    return RedirectToAction("ProductList");
                }
                TempData.Put("message",new AlertMessage(){
                    Title=_productService.ErrorMessage,
                    Message=_productService.ErrorMessage,
                    AlertType="danger"
                });
             }
             ViewBag.Categories= _categoryService.GetAll();
             return View(Model);
            
         }
         public async Task<IActionResult> deleteUser(string Id){
             Console.WriteLine(Id);
             var msg=new AlertMessage();
             var user=await _userManager.FindByIdAsync(Id);
             if(user!=null){
                 var rolesForUser = await _userManager.GetRolesAsync(user);
                 await _userManager.DeleteAsync(user);
             if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                    // item should be the name of the role
                    var result = await _userManager.RemoveFromRoleAsync(user, item);
                    }
                }    
             msg= new AlertMessage(){
                 Message= "Name : " + user.UserName +" user has been deleted.",
                 AlertType= "danger"
             };
             }
             else{
             msg= new AlertMessage(){
                 Message=" user not found.",
                 AlertType= "danger"
             };
             }
             
            TempData["message"]=JsonConvert.SerializeObject(msg);
             return RedirectToAction("UserList");
         }
         public IActionResult deleteproduct(int productId){
             var entity=_productService.GetById(productId);
             if(entity!=null){
                 _productService.Delete(entity);
             }
             var msg= new AlertMessage(){
                 Message= $"{entity.Name} product has been deleted.",
                 AlertType= "danger"
             };
            TempData["message"]=JsonConvert.SerializeObject(msg);
             return RedirectToAction("ProductList");
         }
         [HttpPost]
         public IActionResult DeleteFromCategory(int productId,int categoryId){
             _categoryService.DeleteFromCategory(productId,categoryId);
             return Redirect("/admin/categories/"+categoryId);
         }
         [HttpPost]
         public IActionResult Login(LoginModel model){
             return View();
         }
        
    }
}