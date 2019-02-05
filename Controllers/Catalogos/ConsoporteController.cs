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
    public class ConsoporteController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: Consoporte
        public ActionResult Index()
        {
            int pagina_id = 841; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
        
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            var cONSOPORTEs = db.CONSOPORTEs.Include(c => c.TSOL.TSOLTs).Include(c => c.TSOPORTE.TSOPORTETs);
            return View(cONSOPORTEs.ToList());
        }

        // GET: Consoporte/Details/5
        public ActionResult Details(string tsol, string tsopo)
        {
            int pagina_id = 843; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            if (tsol == null || tsopo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONSOPORTE cONSOPORTE = db.CONSOPORTEs.Include(c => c.TSOL.TSOLTs).Include(c => c.TSOPORTE.TSOPORTETs).Where(x => x.TSOL_ID == tsol && x.TSOPORTE_ID == tsopo).First();
            if (cONSOPORTE == null)
            {
                return HttpNotFound();
            }
            return View(cONSOPORTE);
        }

        // GET: Consoporte/Create
        public ActionResult Create()
        {
            int pagina_id = 843; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }
            CONSOPORTE consoporte = new CONSOPORTE
            {
                ACTIVO = true,
                OBLIGATORIO = true
            };
            return View(consoporte);
        }

        // POST: Consoporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TSOL_ID,TSOPORTE_ID,OBLIGATORIO,ACTIVO")] CONSOPORTE cONSOPORTE)
        {
            if (ModelState.IsValid)
            {
                if (db.CONSOPORTEs.Any(x => x.TSOPORTE_ID == cONSOPORTE.TSOPORTE_ID && x.TSOL_ID == cONSOPORTE.TSOL_ID))
                {
                    CONSOPORTE cONSOPORTEAux = db.CONSOPORTEs.First(x => x.TSOPORTE_ID == cONSOPORTE.TSOPORTE_ID && x.TSOL_ID == cONSOPORTE.TSOL_ID);
                    cONSOPORTEAux.ACTIVO = cONSOPORTE.ACTIVO;
                    cONSOPORTEAux.OBLIGATORIO = cONSOPORTE.OBLIGATORIO;
                    db.Entry(cONSOPORTEAux).State = EntityState.Modified;
                }
                else {
                    db.CONSOPORTEs.Add(cONSOPORTE);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TSOL_ID = new SelectList(db.TSOLs, "ID", "DESCRIPCION", cONSOPORTE.TSOL_ID);
            ViewBag.TSOPORTE_ID = new SelectList(db.TSOPORTEs, "ID", "DESCRIPCION", cONSOPORTE.TSOPORTE_ID);
            return View(cONSOPORTE);
        }

        // GET: Consoporte/Edit/5
        public ActionResult Edit(string tsol, string tsopo)
        {
            int pagina_id = 843; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            ViewBag.IdTsol = tsol;
            ViewBag.activo = true;

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            if (tsol == null || tsopo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONSOPORTE cONSOPORTE = db.CONSOPORTEs.Include(c => c.TSOL.TSOLTs).Include(c => c.TSOPORTE.TSOPORTETs).Where(x => x.TSOL_ID == tsol && x.TSOPORTE_ID == tsopo).First();
            if (cONSOPORTE == null)
            {
                return HttpNotFound();
            }
            return View(cONSOPORTE);
        }

        // POST: Consoporte/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TSOL_ID,TSOPORTE_ID,OBLIGATORIO,ACTIVO")] CONSOPORTE cONSOPORTE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cONSOPORTE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            cONSOPORTE = db.CONSOPORTEs.Include(c => c.TSOL.TSOLTs).Include(c => c.TSOPORTE.TSOPORTETs).Where(x => x.TSOL_ID == cONSOPORTE.TSOL_ID && x.TSOPORTE_ID == cONSOPORTE.TSOPORTE_ID).First();

            return View(cONSOPORTE);
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
