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

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class CuentaController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: Cuenta
        public ActionResult Index()
        {
            int pagina = 691; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            var cUENTAs = db.CUENTAs.Include(c => c.TALL).Include(c => c.SOCIEDAD).Include(c => c.PAI).ToList();
            return View(cUENTAs.ToList());
        }

        // GET: Cuenta/Details/5
        public ActionResult Details(string soc, string pai, string tal, int? eje)
        {
            int pagina = 694; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller,693);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }
            if (soc == null || pai == null || tal == null || eje == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CUENTA cUENTA = db.CUENTAs.Find(soc, pai, tal, eje);
            if (cUENTA == null)
            {
                return HttpNotFound();
            }
            return View(cUENTA);
        }

        // GET: Cuenta/Create
        public ActionResult Create()
        {
            int pagina = 693; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }
            return View();
        }

        // POST: Cuenta/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SOCIEDAD_ID,PAIS_ID,TALL_ID,EJERCICIO,ABONO,CARGO,CLEARING,LIMITE,IMPUESTO")] CUENTA cUENTA)
        {
            int pagina = 693; //ID EN BASE DE DATOS
            try
            {
                if (ModelState.IsValid)
                {
                    db.CUENTAs.Add(cUENTA);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(cUENTA);
            }catch(Exception e)
            {
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);

                if (e.InnerException.InnerException.Message.Contains("PK_CUENTA"))
                {
                    ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina, "lbl_mnjCuentaExistente", User.Identity.Name);
                }
                return View(cUENTA);
            }
        }

        // GET: Cuenta/Edit/5
        public ActionResult Edit(string soc, string pai, string tal, int? eje)
        {
            int pagina = 692; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            if (soc == null || pai == null || tal == null || eje == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUENTA cUENTA = db.CUENTAs.Find(soc, pai, tal, eje);
            if (cUENTA == null)
            {
                return HttpNotFound();
            }
           return View(cUENTA);
        }

        // POST: Cuenta/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOCIEDAD_ID,PAIS_ID,TALL_ID,EJERCICIO,ABONO,CARGO,CLEARING,LIMITE,IMPUESTO")] CUENTA cUENTA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cUENTA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cUENTA);
        }

        // GET: Cuenta/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CUENTA cUENTA = db.CUENTAs.Find(id);
            if (cUENTA == null)
            {
                return HttpNotFound();
            }
            return View(cUENTA);
        }

        // POST: Cuenta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string soc, string pai, string tal, int? eje)
        {
            CUENTA cUENTA = db.CUENTAs.Find(soc, pai, tal, eje);
            db.CUENTAs.Remove(cUENTA);
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
