using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        MobileDbContext db = new MobileDbContext();


        //<---------------- Dashboard -------------->
        public ActionResult Dashboard()
        {
            var TotalTransaction = db.Transactions.ToList();
            decimal sum = 0;
            foreach(var i in TotalTransaction)
            {
                sum += i.TransactionAmount;
            }
            ViewBag.TotalSale = sum;
            ViewBag.brandCount = db.Brands.Count();
            ViewBag.productCount = db.Products.Count();
            ViewBag.userCount = db.Users.Count();
            ViewBag.SelectedList = "panelList1";
            decimal[] arr = {0,0,0,0,0,0,0,0,0,0,0,0 };
            foreach(var i in TotalTransaction)
            {
                int month = i.CreatedDate.Month;
                arr[month - 1] += i.TransactionAmount;
            }
            ViewBag.Chart = arr;
            return View();
        }


        //<----------------- Product Grid -------------->
        public ActionResult ProductGrid()
        {
            var products = db.Products.Include(b => b.ProductImages).ToList();
            ViewBag.SelectedList = "panelList2";
            return View(products);
        }



        //<---------------- Add Product -------------->
        public ActionResult AddProduct()
        {
            ViewBag.SelectedList = "panelList2";
            var brands = db.Brands.ToList();
            var categories = db.Categories.ToList();
            Session["Brands"] = new SelectList(brands, "BrandId", "Name");
            Session["Categories"] = new SelectList(categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product P)
        {
            if (ModelState.IsValid == true)
            {
                if (P.ProductImages != null)
                {
                    foreach (var img in P.ProductImages)
                    {
                        if (img.ImageFile != null && img.ImageFile.ContentLength > 0)
                        {
                            byte[] imageData;
                            var name = Path.GetFileName(img.ImageFile.FileName);
                            using (var binaryReader = new BinaryReader(img.ImageFile.InputStream))
                            {
                                imageData = binaryReader.ReadBytes((int)img.ImageFile.InputStream.Length);
                            }
                            img.Data = imageData;
                            img.ProductId = P.ProductId;
                            img.FileName = name;
                            db.ProductImages.Add(img);
                        }
                    }
                }
                Inventory inventory = new Inventory();
                inventory.Stock = 5;
                inventory.InventoryId = P.ProductId;
                P.Inventory = inventory;
                db.Inventories.Add(inventory);
                db.Products.Add(P);


                var a = db.SaveChanges();

                if (a > 0)
                {
                    TempData["AddProduct"] = "<script>alert('Product added successfully')</script>";
                    return RedirectToAction("ProductGrid", "Admin");
                }
                else
                {
                    ViewBag.product = "<script>alert('Product not added')</script>";
                }

            }
            return View();
        }


        //<---------------- Edit Product -------------->
        public ActionResult EditProduct(int? id)
        {
            ViewBag.SelectedList = "panelList2";
            var brands = db.Brands.ToList();
            var categories = db.Categories.ToList();
            Session["Brands"] = new SelectList(brands, "BrandId", "Name");
            Session["Categories"] = new SelectList(categories, "CategoryId", "Name");
            var product = db.Products.Where(m => m.ProductId == id).FirstOrDefault();
            if(product==null)
            {
                return View("error");
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product p)
        {
            var brands = db.Brands.ToList();
            var categories = db.Categories.ToList();
            Session["Brands"] = new SelectList(brands, "BrandId", "Name");
            Session["Categories"] = new SelectList(categories, "CategoryId", "Name");

            if(ModelState.IsValid)
            {
                //db.Entry(p).State = EntityState.Modified;
                var product = db.Products.Where(model=>model.ProductId == p.ProductId).FirstOrDefault();
                product.Title = p.Title;
                product.Description = p.Description;
                product.Price = p.Price;
                product.CategoryId = p.CategoryId;
                product.BrandId =   p.BrandId;
                var a = db.SaveChanges();
                if(a>0)
                {
                    TempData["EditProduct"] = "<script>alert('Edited Successfully')</script>";
                    return RedirectToAction("ProductGrid", "Admin");
                }
                else
                {
                    ViewBag.product = "<script>alert('Edited not Successfully')</script>";
                    return View();
                }
            }
            return View();
        }



        //<---------------- Delete Product -------------->
        public ActionResult DeleteProduct(int? id)
        {
            var product = db.Products.Where(m => m.ProductId == id).FirstOrDefault();

            if(product!=null)
            {
                var inventory = db.Inventories.Where(m=>m.InventoryId == product.ProductId).FirstOrDefault();
                var images = db.ProductImages.Where(m => m.ProductId == id).ToList();
                db.ProductImages.RemoveRange(images);
                db.Inventories.Remove(inventory);
                db.Products.Remove(product);
                
                var a = db.SaveChanges();
                if (a > 0)
                {
                    TempData["DeleteProduct"] = "<script>alert('Deleted Successfully')</script>";
                }
                else
                {
                    TempData["DeleteProduct"] = "<script>alert('Deleted not Successfully')</script>";
                }
            }
            else
            {

                return View("error");
            }

            return RedirectToAction("ProductGrid","Admin");
        }

        //<---------------- Inventory -------------->
        public ActionResult Inventory()
        {
            ViewBag.SelectedList = "panelList5";
            var products = db.Products.Include("ProductImages").Include("Inventory").ToList();

            return View(products);
        }

        //<---------------- AddStock -------------->
        [HttpPost]
        public ActionResult AddStock(int qty, int id)
        {
            if(qty < 0)
            {
                qty = 0;
            }
            var inventory = db.Inventories.Where(m=> m.InventoryId == id).FirstOrDefault();
            if(inventory != null)
            {
                inventory.Stock += qty;
                db.SaveChanges();
            }
            return RedirectToAction("Inventory", "Admin");
        }

        //<---------------- Transaction -------------->
        public ActionResult Transaction()
        {
            ViewBag.SelectedList = "panelList6";
            var transaction = db.Transactions.ToList();

            return View(transaction);
        }

        //<---------------- Customers -------------->
        public ActionResult Customers()
        {
            ViewBag.SelectedList = "panelList3";
            var users = db.Users.ToList();

            return View(users);
        }

        //<---------------- Delete Customer -------------->
        public ActionResult DeleteCustomer(int id)
        {
            var cutomer = db.Users.Where(m=> m.UserId ==  id).FirstOrDefault();
            if(cutomer != null)
            {
                var address = db.Addresses.Where(m => m.UserId == id).ToList();
                var order = db.Orders.Where(m => m.UserId == id).ToList();
                if (address != null)
                {
                    db.Addresses.RemoveRange(address);
                }
                if (order != null)
                {
                    db.Orders.RemoveRange(order);
                }
                db.Users.Remove(cutomer);
                db.SaveChanges();
            }

            return RedirectToAction("Customers", "Admin");
        }
        //<---------------- Order -------------->
        public ActionResult Order()
        {
            ViewBag.SelectedList = "panelList4";
            var order = db.Orders.Include("User").Include("Product").ToList();

            return View(order);
        }

        //<---------------- Feedback -------------->
        public ActionResult Feedback()
        {
            ViewBag.SelectedList = "panelList7";
            var feedbacks = db.Feedbacks.ToList();

            return View(feedbacks);
        }
    }
}