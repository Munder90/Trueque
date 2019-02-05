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
    public class RegionController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Region
        public ActionResult Index()
        {
            int pagina_id = 950;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            string spras_id = ViewBag.spras_id;
            return View(db.REGIONs.ToList());
        }

        // GET: Region/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REGION rEGION = db.REGIONs.Find(id);
            if (rEGION == null)
            {
                return HttpNotFound();
            }
            return View(rEGION);
        }

        // GET: Region/Create
        public ActionResult Create()
        {
            int pagina_id = 951;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller,950);
            string spras_id = ViewBag.spras_id;
            ViewBag.SOCIEDAD = new SelectList(db.SOCIEDADs.Where(t => t.ACTIVO).ToList(), "BUKRS", "BUKRS");
            return View();
        }

        // POST: Region/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "REGION1,SOCIEDAD")] REGION rEGION)
        {
            int pagina_id = 951;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 950);
            string spras_id = ViewBag.spras_id;
            ViewBag.SOCIEDAD = new SelectList(db.SOCIEDADs.Where(t => t.ACTIVO).ToList(), "BUKRS", "BUKRS");
            if (ModelState.IsValid)
            {
                if (!existeRegion(rEGION.REGION1))
                {
                    rEGION.REGION1= rEGION.REGION1.ToUpper();
                    db.REGIONs.Add(rEGION);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    TempData["MensajeExiste"] = "Ya existe una región con el mismo ID";
            }

            return View(rEGION);
        }
        public bool existeRegion(string id)
        {
            var region = db.REGIONs.Where(t => t.REGION1 == id).SingleOrDefault();
            if (region == null)
                return false;
            else
                return true;
        }
        // GET: Region/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REGION rEGION = db.REGIONs.Find(id);
            if (rEGION == null)
            {
                return HttpNotFound();
            }
            int pagina_id = 952;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller,950);
            string spras_id = ViewBag.spras_id;
            ViewBag.SOCIEDAD = new SelectList(db.SOCIEDADs.Where(t => t.ACTIVO).ToList(), "BUKRS", "BUKRS", rEGION.SOCIEDAD);
            return View(rEGION);
        }

        // POST: Region/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "REGION1,SOCIEDAD")] REGION rEGION)
        {
            int pagina_id = 951;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 950);
            string spras_id = ViewBag.spras_id;
            ViewBag.SOCIEDAD = new SelectList(db.SOCIEDADs.Where(t => t.ACTIVO).ToList(), "BUKRS", "BUKRS",rEGION.SOCIEDAD);
            if (ModelState.IsValid)
            {
                db.Entry(rEGION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rEGION);
        }

        // GET: Region/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REGION rEGION = db.REGIONs.Find(id);
            if (rEGION == null)
            {
                return HttpNotFound();
            }
            return View(rEGION);
        }

        // POST: Region/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            REGION rEGION = db.REGIONs.Find(id);
            db.REGIONs.Remove(rEGION);
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
