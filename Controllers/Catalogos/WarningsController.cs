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
    public class WarningsController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: Warnings
        public ActionResult Index(string id)
        {
            int pagina = 510; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            TempData["id"] = id;
            try
            {
                int idi = Int32.Parse(id);
                var wARNINGs = db.WARNINGs.Where(a => a.PAGINA_ID.Equals(idi)).Include(w => w.CAMPOS);
                return View(wARNINGs.ToList());
            }
            catch
            {
                var wARNINGs = db.WARNINGs.Include(w => w.CAMPOS);
                return View(wARNINGs.ToList());
            }
        }

        // GET: Warnings/Details/5
        public ActionResult Details(string spras_id, string campo_id, int pagina_id)
        {
            int pagina = 513; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 510);
            try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
           
            WARNING wARNING = db.WARNINGs.Find(pagina_id, campo_id, spras_id);
            if (wARNING == null)
            {
                return HttpNotFound();
            }
            return View(wARNING);
        }

        // GET: Warnings/Create

        public ActionResult Create(string id)
        {
            int pagina = 511; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 510);
            try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            int ids = Int32.Parse(id);
            ViewBag.CAMPO_ID = new SelectList(db.CAMPOS.Where(a => a.PAGINA_ID.Equals(ids)), "ID", "ID");
            ViewBag.SPRAS_ID = new SelectList(db.SPRAS.Where(x=>x.ID!="PT"), "ID", "ID");
            ViewBag.POSICION = new SelectList(db.POSICIONs, "ID", "ID");


            WARNING w = new WARNING();
            w.PAGINA_ID = ids;
            return View(w);
        }

        // POST: Warnings/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PAGINA_ID,CAMPO_ID,SPRAS_ID,WARNING1,POSICION,ACTIVO")] WARNING wARNING)
        {
            if (ModelState.IsValid)
            {
                var obj = db.WARNINGs.Where(a => a.PAGINA_ID.Equals(wARNING.PAGINA_ID) && a.CAMPO_ID.Equals(wARNING.CAMPO_ID) && a.SPRAS_ID.Equals(wARNING.SPRAS_ID)).FirstOrDefault();
                if (obj == null)
                {
                    wARNING.WARNING1 = wARNING.WARNING1.Replace("\r\n", "<br>");
                    db.WARNINGs.Add(wARNING);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = wARNING.PAGINA_ID });
                }
            }
            int pagina = 511; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 510);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            ViewBag.CAMPO_ID = new SelectList(db.CAMPOS.Where(a => a.PAGINA_ID.Equals(wARNING.PAGINA_ID)), "ID", "ID");
            ViewBag.SPRAS_ID = new SelectList(db.SPRAS.Where(x => x.ID != "PT"), "ID", "ID");
            ViewBag.POSICION = new SelectList(db.POSICIONs, "ID", "ID");
            ViewBag.Error = "Registro duplicado";
            return View(wARNING);
        }

        // GET: Warnings/Edit/5
        public ActionResult Edit(string spras_id, string campo_id, int pagina_id)
        {
            int pagina = 512; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 510);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
           
            WARNING wARNING = db.WARNINGs.Find(pagina_id, campo_id, spras_id);
            if (wARNING == null)
            {
                return HttpNotFound();
            }
            wARNING.WARNING1 = wARNING.WARNING1.Replace("<br>", "\r\n");
            ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION", wARNING.PAGINA_ID);
            ViewBag.CAMPO_ID = new SelectList(db.CAMPOS.Where(a => a.PAGINA_ID.Equals(wARNING.PAGINA_ID)), "ID", "ID");
            ViewBag.POSICION = new SelectList(db.POSICIONs, "ID", "ID");
            return View(wARNING);
        }

        // POST: Warnings/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PAGINA_ID,CAMPO_ID,SPRAS_ID,WARNING1,POSICION,ACTIVO")] WARNING wARNING)
        {
            if (ModelState.IsValid)
            {
                wARNING.WARNING1 = wARNING.WARNING1.Replace("\r\n", "<br>");
                db.Entry(wARNING).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = wARNING.PAGINA_ID });
            }
            int pagina = 512; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 510);
            try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION", wARNING.PAGINA_ID);
            ViewBag.POSICION = new SelectList(db.POSICIONs, "ID", "ID");
            return View(wARNING);
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
