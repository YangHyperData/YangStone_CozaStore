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
    public class SizesController : Controller
    {
        private CozaStoreEntities db = new CozaStoreEntities();

        // GET: Dashboard/Sizes
        public ActionResult Index()
        {
            return View(db.Sizes.ToList());
        }

        // GET: Dashboard/Sizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }

        // GET: Dashboard/Sizes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Sizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sizeid,NameSize")] Size size)
        {
            if (ModelState.IsValid)
            {
                db.Sizes.Add(size);
                db.SaveChanges();
                @TempData["MessageAlert"] = "Create Size " + size.NameSize;
                return RedirectToAction("Index");
            }

            return View(size);
        }

        // GET: Dashboard/Sizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = db.Sizes.Find(id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }

        // POST: Dashboard/Sizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sizeid,NameSize")] Size size)
        {
            if (ModelState.IsValid)
            {
                db.Entry(size).State = EntityState.Modified;
                db.SaveChanges();
                @TempData["MessageAlert"] = "Update Size " + size.NameSize;
                return RedirectToAction("Index");
            }
            return View(size);
        }

        public ActionResult Delete(int id)
        {
            Size size = db.Sizes.Find(id);
            db.Sizes.Remove(size);
            db.SaveChanges();
            @TempData["MessageAlert"] = "Delete Size " + size.NameSize;
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
