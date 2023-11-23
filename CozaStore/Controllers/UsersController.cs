using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;
namespace CozaStore.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        CozaStoreEntities db = new CozaStoreEntities();
        public ActionResult Information(int id)
        {
            var user = db.Users.SingleOrDefault(n => n.Userid == id);
            return View(user);
        }
        [HttpPost]
        public ActionResult Information(FormCollection f)
        {
            string FullName = f["FullName"];
            string Email = f["Email"];
            string Password = f["Password"];
            DateTime BirthDay = DateTime.Parse(f["BirthDay"].ToString());
            bool Gender = bool.Parse(f["Gender"]);
            string PhoneNumber = f["PhoneNumber"];
            string Address = f["Address"];
            User user = db.Users.SingleOrDefault(n => n.Gmail == Email);
            user.FullName = FullName;
            user.Gmail = Email;
            user.Password = Password;
            user.BirthDay = BirthDay;
            user.Gender = Gender;
            user.Phone = PhoneNumber;
            user.Address = Address;
            Session["User"] = user;
            db.SaveChanges();
            return RedirectToAction("Information",new {id = user.Userid });

        }
        public ActionResult MyOrder(int id)
        {
            var order = db.Orders.Where(n => n.Userid == id);
            return View(order.ToList());
        }
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "CozaHome");
        }
    }
}