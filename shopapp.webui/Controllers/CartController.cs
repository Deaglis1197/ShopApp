using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [Authorize]
    public class CartController:Controller
    {
        private UserManager<User> _userManager; 
        private ICartService _cartService;
        private IOrderService _orderService;

        public CartController(IOrderService orderService,ICartService cartService,UserManager<User> userManager){
            _cartService = cartService;
            _userManager = userManager;
            _orderService = orderService;
        }
        [Authorize(Roles="Customer")]        
        public IActionResult Index(){
            var cart=_cartService.GetCartByUserId(_userManager.GetUserId(User));
            return View(new CartModel(){
                CartId=cart.Id,
                CartItems=cart.CartItems.Select(i=>new CartItemModel(){
                CartItemId=i.Id,
                Name=i.Product.Name,
                Price=(double)i.Product.Price,
                ImageUrl=i.Product.ImageUrl,
                Quantity=i.Quantity,
                productId=i.ProductId
                }).ToList()
            });
        }
        [Authorize(Roles="Customer")]   
        [HttpPost]
        public IActionResult AddToCart(int productId,int quantity){
            var userId=_userManager.GetUserId(User);
            _cartService.AddToCart(userId,productId,quantity);
            return RedirectToAction("Index");
        }
        [Authorize(Roles="Customer")]   
        [HttpPost]
        public IActionResult DeleteFromCart(int productId){
            _cartService.DeleteFromCart(_userManager.GetUserId(User),productId);
            return RedirectToAction("Index");
        }
        public IActionResult CheckOut(){
            var cart=_cartService.GetCartByUserId(_userManager.GetUserId(User));
            var orderModel=new OrderModel();
            orderModel.CartModel=new CartModel(){
                CartId=cart.Id,
                CartItems=cart.CartItems.Select(i=>new CartItemModel(){
                CartItemId=i.Id,
                Name=i.Product.Name,
                Price=(double)i.Product.Price,
                ImageUrl=i.Product.ImageUrl,
                Quantity=i.Quantity,
                productId=i.ProductId
                }).ToList()
            };
            return View(orderModel); 
        }
        [HttpPost]
        public IActionResult Checkout(string cartId,OrderModel Model){
            //You can check Payment here
            if(ModelState.IsValid){
                var paymentstate = "success";
                var userId=_userManager.GetUserId(User);
                var cart=_cartService.GetCartByUserId(userId);
                Model.CartModel=new CartModel(){
                    CartId=cart.Id,
                    CartItems=cart.CartItems.Select(i=>new CartItemModel(){
                        CartItemId=i.Id,
                        Name=i.Product.Name,
                        Price=(double)i.Product.Price,
                        ImageUrl=i.Product.ImageUrl,
                        Quantity=i.Quantity,
                        productId=i.ProductId
                    }).ToList()
            };
            var payment = PaymentProcess(Model);
            if(paymentstate=="success"){
                SaveOrder(Model,payment,userId);
                ClearCart(Model.CartModel.CartId);
            return View("Success");
            }else{
                var msg=new AlertMessage(){
                    Message="payment error", 
                    AlertType="danger"
                };
                TempData["message"]=JsonConvert.SerializeObject(msg);
            }
            }
            
            
            return View(Model);
            
        }
        public IActionResult GetOrders(){
            var userId=_userManager.GetUserId(User);
            var orders=_orderService.GetOrders(userId);
            var orderListModel=new List<OrderListModel>();
            OrderListModel orderModel;
            foreach(var order in orders){
                orderModel=new OrderListModel();
                orderModel.OrderId=order.Id;
                orderModel.OrderNumber=order.OrderNumber;
                orderModel.OrderDate=order.OrderDate;
                orderModel.Phone=order.Phone;
                orderModel.Email=order.Email;
                orderModel.FirstName=order.FirstName;
                orderModel.LastName=order.LastName;
                orderModel.City=order.City;
                orderModel.OrderState=order.OrderState;
                orderModel.PaymentType=order.PaymentType;
                orderModel.OrderItems=order.OrderItems.Select(i=>new OrderItemModel(){
                    OrderItemId=i.Id,
                    Name=i.Product.Name,
                    Price=(double)i.Price,
                    Quantity=i.Quantity,
                    ImageUrl=i.Product.ImageUrl

                }).ToList();
                orderListModel.Add(orderModel);
                
            }
            return View("Orders",orderListModel);

        }
        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, object payment, string userId)
        {
            var order=new Order();
            order.OrderNumber=new Random().Next(11111,99999).ToString();
            order.OrderState=EnumOrderState.completed;
            order.PaymentType=EnumPaymentTypes.creditcart;
            order.PaymentId="No Payment Method add yet!";
            order.ConverstationId="No Payment Method add yet!";
            order.OrderDate=new DateTime();
            order.FirstName=model.FirstName;
            order.LastName=model.LastName;
            order.UserId=userId;
            order.Address=model.Address;
            order.Phone=model.Phone;
            order.Email=model.Email;
            order.City=model.City;
            order.Note=model.Note;
            order.OrderItems=new List<entity.OrderItem>();
            foreach(var item in model.CartModel.CartItems){
                var orderItem=new shopapp.entity.OrderItem(){
                    Price=item.Price,
                    Quantity=item.Quantity,
                    ProductId=item.productId
                };
                order.OrderItems=new List<entity.OrderItem>();
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }

        private object PaymentProcess(object model)
        {
            return model;
        }
    }
}