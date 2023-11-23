using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;
namespace CozaStore.Areas.Dashboard.Controllers
{
    public class UsersController : Controller
    {
        private CozaStoreEntities db = new CozaStoreEntities();

        // GET: Dashboard/Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Dashboard/Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Dashboard/Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Userid,FullName,Gender,BirthDay,Gmail,Password,Phone,Address")] User user, HttpPostedFileBase fFileUpload)
        {
            if (ModelState.IsValid)
            {
                if(fFileUpload != null)
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
                    if(user.Gender == true)
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
                TempData["AlertMessage"] = "New Users " + user.FullName;
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Dashboard/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Dashboard/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Userid,FullName,Gender,BirthDay,Gmail,Password,Phone,Address")] User user, HttpPostedFileBase fFileUpload)
        {
            if (ModelState.IsValid)
            {
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
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Update Users " + user.FullName;
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int id)
        {
            User user = db.Users.Find(id);
            var order = from c in db.Orders
                        where c.Userid == user.Userid
                        select c;
     
                foreach (var item in order)
                {
                    var detailorder = from c in db.DetailsOrders
                                      where c.Orderid == item.Orderid
                                      select c;
 
                        db.DetailsOrders.RemoveRange(detailorder);
                        db.SaveChanges();
                    

                }
                db.Orders.RemoveRange(order);
            
            db.Users.Remove(user);
            db.SaveChanges();
            TempData["AlertMessage"] = "Delete Users " + user.FullName;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
