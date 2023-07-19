using FinalProject.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;

namespace FinalProject.Controllers
{
    [Authorize(Roles = "Customer")]
    public class PaymentController : Controller
    {
        MobileDbContext db = new MobileDbContext();

        //<---------------- Save Order Method-------------->
        private void SaveOrder(int address, string payment, string TransactionId)
        {
            var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
            var cart = db.Carts.Include("Product.Inventory").Where(m => m.UserId == user.UserId).ToList();
            var add = db.Addresses.Where(m => m.AddressId == address && m.UserId == user.UserId).FirstOrDefault();
            Random random = new Random();
            int num = random.Next(10000, 99999);
            DateTime date = DateTime.Now;
            int sum = 0;
            foreach (var cartItem in cart)
            {
                sum += cartItem.Quantity * cartItem.Product.Price;
                Models.Order obj = new Models.Order();
                obj.OrderId = num;
                obj.ProductId = cartItem.ProductId;
                obj.Quantity = cartItem.Quantity;
                obj.UserId = user.UserId;
                obj.CreatedDate = date;
                obj.Product = cartItem.Product;
                obj.Address = add;
                obj.PaymentMode = payment;
                cartItem.Product.Inventory.Stock -= cartItem.Quantity;
                db.Orders.Add(obj);
            }
            sum += (2 * sum) / 100;
            Transaction transaction = new Transaction();
            transaction.OrderId = num;
            transaction.PaymentMode = payment;
            transaction.UserName = user.Name;
            transaction.TransactionAmount = sum;
            transaction.CreatedDate = date;
            if (TransactionId == "NA")
            {
                transaction.TransactionId = "NA";
                transaction.TransactionStatus = "PENDING";
            }
            else
            {
                transaction.TransactionId = TransactionId;
                transaction.TransactionStatus = "PAID";
            }
            db.Transactions.Add(transaction);
            db.Carts.RemoveRange(cart);
            Session["Count"] = 0;
            db.SaveChanges();
        }

        //<---------------- Place Order -------------->
        [HttpPost]
        public ActionResult PlaceOrder(int address, string payment)
        {
            if (payment == "COD")
            {
                SaveOrder(address, payment,"NA");
                return View("PlaceOrder");
            }
            else if (payment == "Online")
            {
                var user = db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefault();
                var cart = db.Carts.Include("Product").Where(m => m.UserId == user.UserId).ToList();
                int sum = 0;
                foreach (var i in cart)
                {
                    sum += i.Product.Price * i.Quantity;
                }
                sum += (2 * sum) / 100; 
                Random random = new Random();
                string TransactionId = random.Next(0, 100000).ToString();

                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", sum * 100);
                input.Add("currency", "INR");
                input.Add("receipt", TransactionId);

                string key = "rzp_test_MjbnXcG85zU2fi";
                string secret = "QVKNGuqC4HfqOWLjYNWxaqmp";

                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Order order = client.Order.Create(input);
                ViewBag.orderId = order["id"].ToString();
                ViewBag.Payment = payment;
                ViewBag.Address = address;

                return View("Pay", user);
            }
            return RedirectToAction("CheckOut", "User");
        }

        //<---------------- On Successful Payment -------------->
        public ActionResult Payment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature,int address, string payment)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

            Utils.verifyPaymentSignature(attributes);

            SaveOrder(address, payment, razorpay_payment_id);

            return View("PlaceOrder");
        }
    }
}