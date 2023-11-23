using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;

namespace CozaStore.Areas.Dashboard.Controllers
{
    public class CategoriesController : Controller
    {
        private CozaStoreEntities db = new CozaStoreEntities();

        // GET: Dashboard/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Dashboard/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Dashboard/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Categoryid,Name,Style")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                @TempData["MessageAlert"] = "Create Category " + category.Name;
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Dashboard/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Dashboard/Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Categoryid,Name,Style")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                @TempData["MessageAlert"] = "Update Category " + category.Name;
                return RedirectToAction("Index");
            }
            return View(category);
        }

      
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            var product = from c in db.Products
                          where c.Categoryid == category.Categoryid
                          select c;
            foreach(var item in product)
            {
                var orderdetals = from c in db.DetailsOrders
                                  where c.Productid == item.Productid
                                  select c;
                db.DetailsOrders.RemoveRange(orderdetals);
            }
            db.SaveChanges();
            db.Products.RemoveRange(product);
            db.SaveChanges();
            db.Categories.Remove(category);
            db.SaveChanges();
            @TempData["MessageAlert"] = "Delete Category " + category.Name;
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
