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
    public class ColorsController : Controller
    {
        private CozaStoreEntities db = new CozaStoreEntities();

        // GET: Dashboard/Colors
        public ActionResult Index()
        {
            return View(db.Colors.ToList());
        }

        // GET: Dashboard/Colors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = db.Colors.Find(id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }

        // GET: Dashboard/Colors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Colors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Colorid,ColorName")] Color color)
        {
            if (ModelState.IsValid)
            {
                db.Colors.Add(color);
                db.SaveChanges();
                @TempData["MessageAlert"] = "Create Color " + color.ColorName;
                return RedirectToAction("Index");
            }

            return View(color);
        }

        // GET: Dashboard/Colors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = db.Colors.Find(id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }

        // POST: Dashboard/Colors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Colorid,ColorName")] Color color)
        {
            if (ModelState.IsValid)
            {
                db.Entry(color).State = EntityState.Modified;
                db.SaveChanges();
                @TempData["MessageAlert"] = "Edits Color " + color.ColorName;
                return RedirectToAction("Index");
            }
            return View(color);
        }
        // POST: Dashboard/Colors/Delete/5
       
        public ActionResult Delete(int id)
        {
            Color color = db.Colors.Find(id);
            db.Colors.Remove(color);
            db.SaveChanges();
            @TempData["MessageAlert"] = "Delete Color " + color.ColorName;
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
