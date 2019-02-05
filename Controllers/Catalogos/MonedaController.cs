using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class MonedaController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Moneda
        public ActionResult Index()
        {
            int pagina_id = 930;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            return View(db.MONEDAs.ToList());
        }

        // GET: Moneda/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONEDA mONEDA = db.MONEDAs.Find(id);
            if (mONEDA == null)
            {
                return HttpNotFound();
            }
            return View(mONEDA);
        }

        // GET: Moneda/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Moneda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WAERS,ISOCD,ALTWR,LTEXT,KTEXT,ACTIVO")] MONEDA mONEDA)
        {
            if (ModelState.IsValid)
            {
                db.MONEDAs.Add(mONEDA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mONEDA);
        }

        // GET: Moneda/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONEDA mONEDA = db.MONEDAs.Find(id);
            if (mONEDA == null)
            {
                return HttpNotFound();
            }
            return View(mONEDA);
        }

        // POST: Moneda/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WAERS,ISOCD,ALTWR,LTEXT,KTEXT,ACTIVO")] MONEDA mONEDA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mONEDA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mONEDA);
        }

        // GET: Moneda/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONEDA mONEDA = db.MONEDAs.Find(id);
            if (mONEDA == null)
            {
                return HttpNotFound();
            }
            return View(mONEDA);
        }

        // POST: Moneda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MONEDA mONEDA = db.MONEDAs.Find(id);
            db.MONEDAs.Remove(mONEDA);
            db.SaveChanges();
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
