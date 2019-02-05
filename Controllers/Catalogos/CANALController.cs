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
    public class CANALController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: CANAL
        public ActionResult Index()
        {
            int pagina_id = 940;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            
            return View(db.CANALs.ToList());
        }

        // GET: CANAL/Create
        public ActionResult Create()
        {
            int pagina_id = 941;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            
            return View();
        }

        // POST: CANAL/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CANAL1,CDESCRIPCION")] CANAL cANAL)
        {
            if (ModelState.IsValid)
            {
                var canalexiste = db.CANALs.Where(t => t.CANAL1 == cANAL.CANAL1).SingleOrDefault();
                if (canalexiste == null)
                {
                    db.CANALs.Add(cANAL);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Error", "El canal ya existe");
                    int pagina_id = 941;//ID EN BASE DE DATOS
                    FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                    
                    return View(cANAL);
                }
            }

            return View(cANAL);
        }

        // GET: CANAL/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CANAL cANAL = db.CANALs.Find(id);
            if (cANAL == null)
            {
                return HttpNotFound();
            }
            int pagina_id = 942;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            
            return View(cANAL);
        }

        // POST: CANAL/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CANAL1,CDESCRIPCION")] CANAL cANAL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cANAL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cANAL);
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
