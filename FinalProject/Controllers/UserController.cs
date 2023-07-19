using FinalProject.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;

namespace FinalProject.Controllers
{
    [Authorize(Roles = "Customer")]
    public class UserController : Controller
    {
        MobileDbContext db = new MobileDbContext();

        //<---------------- Sign Up -------------->
        [AllowAnonymous]
        public ActionResult Signup()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Signup(User u)
        {
            if(ModelState.IsValid == true)
            {
                var isUser = db.Users.Where(user=> user.Email == u.Email).FirstOrDefault();
                if (isUser == null)
                {
                    u.Role = "Customer";
                    db.Users.Add(u);
                    var x = db.SaveChanges();
                    if (x > 0)
                    {
                        TempData["alertSignin"] = "<script>alert('Registration Successful!')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Signin", "User");
                    }
                    else
                    {
                        ViewBag.alert = "<script>alert('Registration Failed!')</script>";
                    }
                }
                else
                {
                    ViewBag.alert = "<script>alert('User already exists with this email')</script>";
                }

            }
            return View();
        }


        //<---------------- Sign In -------------->
        [AllowAnonymous]
        public ActionResult Signin()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Signin(User u, string ReturnUrl)
        {
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Name");
            ModelState.Remove("UserId");
            ModelState.Remove("Mobile");
            if (ModelState.IsValid == true)
            {
                var user = db.Users.Where(model => model.Email == u.Email && model.Password == u.Password).FirstOrDefault();
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(u.Email,false);
                    Session["login"] = user.Name;
                    TempData["alertSignin"] = "<script>alert('Login Successful!')</script>";
                    ModelState.Clear();

                    if(user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }

                    if(ReturnUrl!=null)
                    {
                        return RedirectToAction(ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.abc = "<script>alert('Incorrect Email or Password')</script>";
                }
            }
            return View();
        }


        //<---------------- Sign Out -------------->
        [AllowAnonymous]
        public ActionResult Signout() 
        {
            FormsAuthentication.SignOut();
            Session["Count"] = 0;
            Session["login"] = null;
            return RedirectToAction("Index", "Home");
        }


        //<---------------- Cart -------------->
        public ActionResult Cart()
        {
            Session["Count"] = 0;
            var user = db.Users.Where(m=> m.Email == User.Identity.Name).FirstOrDefault();
            var cart = db.Carts.Include("Product.ProductImages").Include("Product.Inventory").Where(m => m.UserId == user.UserId).ToList();
            
            if (user != null)
            {
                int qty = 0;
                foreach (var cartItem in cart)
                {
                    qty += cartItem.Quantity;
                }
                Session["Count"] = qty;
            }
            return View(cart);
        }


        //<---------------- Add To Cart -------------->
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var product = db.Products.Include("Inventory").Where(m => m.ProductId == id).FirstOrDefault();

            if (user != null && product !=null)
            {
                var cart = db.Carts.Where(m=> m.ProductId == id && m.UserId == user.UserId).FirstOrDefault();
                if(cart != null)
                {
                    if(cart.Quantity + quantity <= product.Inventory.Stock)
                    {
                        cart.Quantity += quantity;
                        if(cart.Quantity == 0)
                        {
                            db.Carts.Remove(cart);
                        }
                        db.SaveChanges();
                        
                    }
                }
                else
                {
                    Cart cart1 = new Cart();
                    cart1.UserId = user.UserId;
                    cart1.Quantity = quantity;
                    cart1.ProductId = id;
                    cart1.Product = product;
                    db.Carts.Add(cart1);
                    db.SaveChanges();
                }

            }
            if (user != null)
            {
                var cart = db.Carts.Where(m => m.UserId == user.UserId).ToList();
                int qty = 0;
                foreach (var cartItem in cart)
                {
                    qty += cartItem.Quantity;
                }
                Session["Count"] = qty;
            }
            return Json(Session["Count"]);
        }

        //<---------------- Add To Cart with Cart view -------------->
        [HttpPost]
        public ActionResult AddCartItem(int id, int quantity)
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var product = db.Products.Include("Inventory").Where(m => m.ProductId == id).FirstOrDefault();

            if (user != null && product != null )
            {
                var cart = db.Carts.Where(m => m.ProductId == id && m.UserId == user.UserId).FirstOrDefault();
                if (cart != null)
                {
                    if (cart.Quantity + quantity <= product.Inventory.Stock)
                    {
                        cart.Quantity += quantity;
                        if (cart.Quantity == 0)
                        {
                            db.Carts.Remove(cart);
                        }
                        db.SaveChanges();

                    }
                }
                else
                {
                    Cart cart1 = new Cart();
                    cart1.UserId = user.UserId;
                    cart1.Quantity = quantity;
                    cart1.ProductId = id;
                    cart1.Product = product;
                    db.Carts.Add(cart1);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("Cart","User");
        }

        //<---------------- Delete Cart Item -------------->
        public ActionResult DeleteCartItem(int id)
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var Cart = db.Carts.Where(m => m.UserId == user.UserId && m.CartId == id).FirstOrDefault();
            db.Carts.Remove(Cart);
            db.SaveChanges();

            return RedirectToAction("Cart", "User");
        }

        //<---------------- Clear Cart -------------->
        public ActionResult ClearCart()
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var Cart = db.Carts.Where(m => m.UserId == user.UserId).ToList();
            db.Carts.RemoveRange(Cart);
            db.SaveChanges();

            return RedirectToAction("Cart", "User");
        }

        //<---------------- Add Address -------------->
        public ActionResult AddAddress()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAddress(Address A)
        {
            var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
            if(user != null)
            {
                if(ModelState.IsValid)
                {
                    A.UserId = user.UserId;
                    db.Addresses.Add(A);
                    db.SaveChanges();
                    return RedirectToAction("CheckOut", "User");
                }
            }
            return View();
        }

        //<---------------- Edit Address -------------->
        public ActionResult EditAddress(int? id)
        {
            var user = db.Users.Where(m=> m.Email == User.Identity.Name).FirstOrDefault();
            var address = db.Addresses.Where(m=> m.AddressId  == id && m.UserId == user.UserId).FirstOrDefault();
            if(address!=null)
            {
                return View(address);
            }

            return RedirectToAction("CheckOut", "User");
        }

        [HttpPost]
        public ActionResult EditAddress(Address A)
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var address = db.Addresses.Where(m => m.AddressId == A.AddressId && m.UserId == user.UserId).FirstOrDefault();
            if(address!=null)
            {
                address.Name = A.Name;
                address.Phone = A.Phone;
                address.City = A.City;
                address.Pincode = A.Pincode;
                address.Region1 = A.Region1;
                address.Region2 = A.Region2;
                address.Landmark = A.Landmark;
                address.State = A.State;
                db.SaveChanges();
                return RedirectToAction("CheckOut", "User");
            }
            return View(address);
        }


            //<---------------- Delete Address -------------->
            public ActionResult DeleteAddress(int? id)
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var address = db.Addresses.Where(m => m.AddressId == id && m.UserId == user.UserId).FirstOrDefault();
            if (address != null)
            {
                db.Addresses.Remove(address);
                db.SaveChanges();
            }
            return RedirectToAction("CheckOut","User");
        }

        //<---------------- Check Out -------------->
        public ActionResult CheckOut()
        {
            var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
            var address = db.Addresses.Where(m => m.UserId == user.UserId).ToList();
            var cart = db.Carts.Include("Product").Where(m => m.UserId == user.UserId).ToList();
            int sum = 0;
            foreach (var i in cart)
            {
                sum += i.Product.Price * i.Quantity;
            }
            ViewBag.Sum = sum;
            return View(address);
        }

        [HttpPost]
        public ActionResult CheckOut(string address, string payment)
        {
            var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {

            }
            return View();
        }

        //<---------------- Orders -------------->
        public ActionResult Orders()
        {

            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var orders = db.Orders.Include("Product.ProductImages").Include("Address").Where(m => m.UserId == user.UserId).OrderBy(m=> m.CreatedDate).ToList();

            if (user != null)
            {
                var cart = db.Carts.Where(m => m.UserId == user.UserId).ToList();
                int qty = 0;
                foreach (var cartItem in cart)
                {
                    qty += cartItem.Quantity;
                }
                Session["Count"] = qty;
            }
            return View(orders);
        }

    }
}