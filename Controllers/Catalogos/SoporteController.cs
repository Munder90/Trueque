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
    public class SoporteController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: Soporte
        public ActionResult Index()
        {
            int pagina_id = 1314; //ID EN BASE DE DATOS
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

            var soportesES = db.TSOPORTETs.Where(x=>x.SPRAS_ID=="ES").ToList();

            ViewBag.soportesEN = db.TSOPORTETs.Where(x => x.SPRAS_ID == "EN").ToList();

            return View(soportesES);
        }

        // GET: Soporte/Details/5
        public ActionResult Details(string tsoporte_id)
        {
            int pagina_id = 1315; //ID EN BASE DE DATOS
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

            if (tsoporte_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sopor = db.TSOPORTEs.Where(x => x.ID == tsoporte_id).FirstOrDefault();
            if (tsoporte_id == null)
            {
                return HttpNotFound();
            }
            return View(sopor);
        }

        // GET: Soporte/Create
        public ActionResult Create()
        {
            int pagina_id = 1316; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

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
            TSOPORTE soporte = new TSOPORTE();
            soporte.ACTIVO = true;
            soporte.TSOPORTETs = new List<TSOPORTET>();
            soporte.TSOPORTETs.Add(new TSOPORTET { SPRAS_ID="ES"});
            soporte.TSOPORTETs.Add(new TSOPORTET { SPRAS_ID = "EN" });

            return View(soporte);
        }

        // POST: Soporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TSOPORTE tSOPORTE, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                db.TSOPORTEs.Add(tSOPORTE);
                db.SaveChanges();
                List<TSOPORTET> listTextos = new List<TSOPORTET>();
                if (collection.AllKeys.Contains("EN") && !String.IsNullOrEmpty(collection["EN"]))
                {

                    TSOPORTET m = new TSOPORTET { SPRAS_ID = "EN", TSOPORTE_ID = tSOPORTE.ID, TXT50 = collection["EN"] };
                    listTextos.Add(m);

                }
                if (collection.AllKeys.Contains("ES") && !String.IsNullOrEmpty(collection["ES"]))
                {
                    TSOPORTET m = new TSOPORTET { SPRAS_ID = "ES", TSOPORTE_ID = tSOPORTE.ID, TXT50 = collection["ES"] };
                    listTextos.Add(m);
                }
                db.TSOPORTETs.AddRange(listTextos);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
           

            int pagina_id = 1316; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

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

            
            return View(tSOPORTE);
        }

        // GET: Soporte/Edit/5
        public ActionResult Edit(string id)
        {
            int pagina_id = 1317; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var des = db.TSOPORTEs.Where(x => x.ID == id).FirstOrDefault();
            if (des == null)
            {
                return HttpNotFound();
            }
            TSOPORTE tSOPORTE = db.TSOPORTEs.Find(id);
            if (tSOPORTE.TSOPORTETs.Count > 0)
                foreach (var e in tSOPORTE.TSOPORTETs)
                {
                    if (e.SPRAS_ID == "EN")
                        ViewBag.EN = e.TXT50;
                    if (e.SPRAS_ID == "ES")
                        ViewBag.ES = e.TXT50;
                }
            else
            {
                ViewBag.EN = tSOPORTE.DESCRIPCION;
            }
            
            return View(tSOPORTE);
        }

        // POST: Soporte/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TSOPORTE tSOPORTE, FormCollection collection)
        {

            if (ModelState.IsValid)
            {
                db.Entry(tSOPORTE).State = EntityState.Modified;
                db.SaveChanges();
                
                var textos = db.TSOPORTETs.Where(t => t.TSOPORTE_ID == tSOPORTE.ID).ToList();
                db.TSOPORTETs.RemoveRange(textos);
                List<TSOPORTET> listTextos = new List<TSOPORTET>();
                if (collection.AllKeys.Contains("EN") && !String.IsNullOrEmpty(collection["EN"])) { 
                    
                        TSOPORTET m = new TSOPORTET { SPRAS_ID = "EN", TSOPORTE_ID = tSOPORTE.ID, TXT50 = collection["EN"].ToUpper() };
                    listTextos.Add(m);
                    
                }
                if (collection.AllKeys.Contains("ES") && !String.IsNullOrEmpty(collection["ES"]))
                {
                    TSOPORTET m = new TSOPORTET { SPRAS_ID = "ES", TSOPORTE_ID = tSOPORTE.ID, TXT50 = collection["ES"].ToUpper() };
                    listTextos.Add(m);
                }
                db.TSOPORTETs.AddRange(listTextos);

                db.SaveChanges();
                return RedirectToAction("Index");
            }



            int pagina_id = 1317; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

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
        
            
            return View(tSOPORTE);
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
