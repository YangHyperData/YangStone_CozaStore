using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;
namespace CozaStore.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        CozaStoreEntities db = new CozaStoreEntities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string Email = f["Email"].ToString();
            string Password = f["Password"].ToString();
            User user = db.Users.SingleOrDefault(n => n.Gmail == Email && n.Password == Password);
            if(user == null)
            {
                TempData["err1"] = "Email or Password is incorrect !!!";
                TempData["Email"] = Email;
                TempData["Password"] = Password;
                
                return Redirect("Login");
            }
            Session["User"] = user;
            return RedirectToAction("Index", "CozaHome");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            string FullName = f["FullName"]; 
            string Email = f["Email"]; 
            string Password = f["Password"]; 
            string RePassword = f["RePassword"];
            DateTime BirthDay = DateTime.Parse(f["BirthDay"].ToString());
            bool Gender = bool.Parse(f["Gender"]);
            string PhoneNumber = f["PhoneNumber"];
            string Address = f["Address"];

            if(Password != RePassword)
            {
                TempData["err1"] = "Password no Same!!!";
                return RedirectToAction("Register");
            }
            User usertmp = db.Users.SingleOrDefault(n => n.Gmail == Email);
            if(usertmp != null)
            {
                TempData["err1"] = "Email already exists!!!";
                return RedirectToAction("Register");
            }
            User user = new User();
            user.FullName = FullName;
            user.Gmail = Email;
            user.Password = Password;
            user.BirthDay = BirthDay;
            user.Gender = Gender;
            user.Phone = PhoneNumber;
            user.Address = Address;
            if (fFileUpload != null)
            {
                var FileName = Path.GetFileName(fFileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Asset/images"), FileName);
                if (!System.IO.File.Exists(path))
                {
                    fFileUpload.SaveAs(path);
                }
                user.image = FileName;
            }
            else
            {
                if (user.Gender == true)
                {
                    user.image = "gallery-03.jpg";
                }
                else
                {
                    user.image = "gallery-02.jpg";
                }
            }
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Login");
        }
    }
}