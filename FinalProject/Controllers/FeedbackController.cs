using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace FinalProject.Controllers
{
    public class FeedbackController : Controller
    {
        MobileDbContext db = new MobileDbContext();

        //<---------------- Feedback -------------->

        [HttpPost]
        public ActionResult Index(string message)
        {
            Feedback feedback = new Feedback();
            feedback.Message = message;
            feedback.CreatedDate = DateTime.Now;
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Where(m=> m.Email == User.Identity.Name).FirstOrDefault();
                if(user != null)
                {
                    feedback.UserName = user.Name;
                }
            }
            else
            {
                feedback.UserName = "Anonymous";
            }
            db.Feedbacks.Add(feedback);
            var a = db.SaveChanges();
            if (a > 0)
            {
                TempData["feedback"] = "<script>alert('Feedback Submited')</script>";
            }
            return RedirectToAction("Index","Home");
        }
    }
}