using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.webui.EmailServices;
using shopapp.webui.Extensions;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class AccountController:Controller
    {
        
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IEmailSender emailSender,ICartService cartService){
            _cartService=cartService;
            _userManager=userManager;
            _signInManager=signInManager;
            _emailSender=emailSender;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string ReturnUrl=null){
            if(User.Identity.IsAuthenticated){
                return Redirect("~/");
            }
            return View(new LoginModel(){
                 ReturnUrl=ReturnUrl
            });
        }
        [AllowAnonymous]
        [HttpPost]
         public async Task<IActionResult> Login(LoginModel model){
             if(!ModelState.IsValid){
                 return View(model);
             }
             //var user=await _userManger.FindByNameAsync(model.UserName);
             var user=await _userManager.FindByEmailAsync(model.Email); 
             if(user==null){
                 ModelState.AddModelError("","Password or Username is incorrect");
                 return View(model);
             }
             if(!await _userManager.IsEmailConfirmedAsync(user)){
                 ModelState.AddModelError("","Email not confirmed yet!");
                 return View(model);
             }
             var result=await _signInManager.PasswordSignInAsync(user,model.Password,true,false);
             if(result.Succeeded){
                 return Redirect(model.ReturnUrl??"~/");
             }
             ModelState.AddModelError("","Password or Username is incorrect");
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(){
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken] //for securty ,csrf attack!
         public async Task<IActionResult> Register(RegisterModel model){
             if(!ModelState.IsValid){
                 return View(model);
             }
             var user=new User(){
                 FirstName=model.FirstName,
                 LastName=model.LastName,
                 UserName=model.UserName,
                 Email=model.Email

             };
             var result=await _userManager.CreateAsync(user,model.Password);
             if(result.Succeeded){
                 await _userManager.AddToRoleAsync(user,"Customer");
                 //generate token
                
                var code=await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url=Url.Action("ConfirmEmail","Account",new{
                    userId=user.Id,
                    token=code
                });
                
                 //email
                 await _emailSender.SendEmailAsync(model.Email,"Confirm Your Account.",$"Press link for confirm your account: <a href='https://localhost:5001{url}'> Link </a>");
                 Console.WriteLine(url);
                 return RedirectToAction("Login","Account");
             }
            ModelState.AddModelError("","Error!");
            return View(model);
        }
        public async Task<IActionResult> Logout(){
            await _signInManager.SignOutAsync();
            TempData.Put("message",new AlertMessage(){
                    Title="Logout",
                    Message="Account safety exit.",
                    AlertType="success"
                });
            return Redirect("~/");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId,string token){
            if(userId==null || token==null){
              //  TempData["message"]="Geçersiz token.";
                TempData.Put("message",new AlertMessage(){
                    Title="Valid Token!",
                    Message="Valid Token!",
                    AlertType="danger"
                });
    
                return View();
            }
            var user=await _userManager.FindByIdAsync(userId);
            
            if(user!=null){
                var result=await _userManager.ConfirmEmailAsync(user,token);
               if(result.Succeeded){
                  _cartService.InitializeCart(user.Id); 
                  TempData.Put("message",new AlertMessage(){
                    Title="Account Confirmed!",
                    Message="Account Confirmed!",
                    AlertType="success"
                });
                   return View();
               } 
            }
             TempData.Put("message",new AlertMessage(){
                    Title="Account not Confirmed!",
                    Message="Account not Confirmed!",
                    AlertType="danger"
                });
            //TempData["message"]="Hesabınız onaylanmadı.";
                return View();
       

        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string userId,string token){
            if(userId==null||token==null){
                return RedirectToAction("Home","Index");
            }
            var model=new ResetPasswordModel{Token=token};

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model){
            if(!ModelState.IsValid){
                return View(model);

            }
            var user=await _userManager.FindByEmailAsync(model.Email);
            if(user==null){
                return RedirectToAction("Home","Index");
            }
            var result=await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if(result.Succeeded){
                return RedirectToAction("Login","Account");
            }
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword(){
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email){
            if(string.IsNullOrEmpty(Email)){
                return View();
            }
            var user=await _userManager.FindByEmailAsync(Email);
            if(user==null){
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                var url=Url.Action("ResetPassword","Account",new{
                    userId=user.Id,
                    token=code
                });
                
                 //email
                 await _emailSender.SendEmailAsync(user.Email,"Reset Password.",$"Press link for reset password your account: <a href='https://localhost:5001{url}'> Link </a>");
            return View();
        }
        
        public IActionResult AccessDenied(){
            return View();
        }
    }
}