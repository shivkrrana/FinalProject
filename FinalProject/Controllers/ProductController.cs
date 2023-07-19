using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace FinalProject.Controllers
{
    public class ProductController : Controller
    {
        MobileDbContext db = new MobileDbContext();

        //<---------------- Product List-------------->
        public ActionResult ProductList()
        {
            ViewBag.Brands = db.Brands.ToList();
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.search = "";

            if (Session["SortProduct"]==null)
            {
                Session["SortProduct"] = db.Products.Include("Brand").Include("ProductImages").Include("Category").ToList();
            }

            if (Session["CategorySelected"] != null && Session["BrandSelected"] != null)
            {
                int i = (int)Session["CategorySelected"];
                int j = (int)Session["BrandSelected"];
                var productList = (List<Product>)Session["SortProduct"];
                Session["Product"] = productList.Where(m => m.CategoryId == i && m.BrandId == j).ToList();
            }
            else if(Session["CategorySelected"] != null && Session["BrandSelected"] == null)
            {
                int i = (int)Session["CategorySelected"];
                var productList = (List<Product>)Session["SortProduct"];
                Session["Product"] = productList.Where(m => m.CategoryId == i).ToList();
            }
            else if (Session["CategorySelected"] == null && Session["BrandSelected"] != null)
            {
                int i = (int)Session["BrandSelected"];
                var productList = (List<Product>)Session["SortProduct"];
                Session["Product"] = productList.Where(m => m.BrandId == i).ToList();
            }
            else
            {
                Session["Product"] = Session["SortProduct"];
            }
            
            return View(Session["Product"]);
        }

        [HttpPost]
        public ActionResult ProductList(string search)
        {
            ViewBag.search = Request.Form["search"];
            ViewBag.Brands = db.Brands.ToList();
            ViewBag.Categories = db.Categories.ToList();
            if (search == "" || search == null)
            {
                var product = Session["Product"];
                return View(product);
            }
            var productList = (List<Product>)Session["Product"];
            var products = productList.Where(m=> m.Title.ToLower().StartsWith(search.ToLower())).ToList();
            return View(products);
        }

        //<---------------- Category Filter-------------->
        public ActionResult Category(int id)
        {
            var Category = db.Categories.Where(m => m.CategoryId == id).FirstOrDefault();
            if (Category != null)
            {
                Session["CategorySelected"] = Category.CategoryId;
            }
            else
            {
                Session["CategorySelected"] = null;
            }

            return RedirectToAction("ProductList","Product");
        }

        //<---------------- Brand Filter-------------->
        public ActionResult Brand(int id)
        {
            var Brand = db.Brands.Where(m=> m.BrandId == id).FirstOrDefault();
            if (Brand != null)
            {
                Session["BrandSelected"] = Brand.BrandId;
            }

            return RedirectToAction("ProductList", "Product");
        }

        //<---------------- Sorting -------------->
        public ActionResult Sort(string str)
        {
            Session["SortSelected"] = str;

            if(str == "az")
            {
                Session["SortProduct"] = db.Products.Include("Brand").Include("ProductImages").Include("Category").OrderBy(m => m.Title).ToList();
            }
            else if (str == "za")
            {
                Session["SortProduct"] = db.Products.Include("Brand").Include("ProductImages").Include("Category").OrderByDescending(m => m.Title).ToList();
            }
            else if (str == "lh")
            {
                Session["SortProduct"] = db.Products.Include("Brand").Include("ProductImages").Include("Category").OrderBy(m => m.Price).ToList();
            }
            else if (str == "hl")
            {
                Session["SortProduct"] = db.Products.Include("Brand").Include("ProductImages").Include("Category").OrderByDescending(m => m.Price).ToList();
            }
            else
            {
                Session["SortProduct"] = db.Products.Include("Brand").Include("ProductImages").Include("Category").ToList();
            }

            return RedirectToAction("ProductList", "Product");
        }

        //<---------------- Clear Filter-------------->
        public ActionResult ClearFilter()
        {
            Session["CategorySelected"] = null;
            Session["BrandSelected"] = null;
            Session["SortProduct"] = null;
            Session["SortSelected"] = null;

            return RedirectToAction("ProductList", "Product");
        }


        //<---------------- Product Detail-------------->
        public ActionResult ProductDetail(int? id)
        {

            var product = db.Products.Include("ProductImages").Include("Brand").Where(m=>m.ProductId == id).FirstOrDefault();
            var inventory = db.Inventories.Where(m => m.InventoryId == id).FirstOrDefault();
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();

            if (product != null)
            {
                if (inventory != null)
                {
                    ViewBag.Stock = inventory.Stock;
                }
                return View(product);
            }
            if(User != null)
            {
                var cart = db.Carts.Where(m => m.UserId == user.UserId).ToList();
                int qty = 0;
                foreach (var cartItem in cart)
                {
                    qty += cartItem.Quantity;
                }
                Session["Count"] = qty;
            }
            
            return RedirectToAction("Index","Home");
        }

    }
}