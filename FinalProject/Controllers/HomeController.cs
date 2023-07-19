using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        MobileDbContext db = new MobileDbContext();

        public ActionResult Index()
        {
            Session["Count"] = 0;
            var products = db.Products.Include("ProductImages").Include("Inventory").Where(m=> m.Inventory.Stock > 0).OrderByDescending(m=> m.ProductId).Take(4).ToList();
            var user = db.Users.Where(m=> m.Email == User.Identity.Name).FirstOrDefault();
            if(user != null)
            {
                var cart =  db.Carts.Where(m => m.UserId == user.UserId).ToList();
                int qty = 0;
                foreach(var cartItem in cart)
                {
                    qty += cartItem.Quantity;
                }
                Session["Count"] = qty;
            }
            return View(products);
        }

        public ActionResult About()
        {

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}