using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class NegsController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Negs
        public ActionResult Index()
        {
            int pagina = 901; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //ViewBag.pais = "mx.svg";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
                ViewBag.lan = user.SPRAS_ID;
            return View(db.NEGOCIACION2.ToList());
        }

        // GET: Negs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NEGOCIACION nEGOCIACION = db.NEGOCIACIONs.Find(id);
            if (nEGOCIACION == null)
            {
                return HttpNotFound();
            }
            return View(nEGOCIACION);
        }

        // GET: Negs/Create
        public ActionResult Create()
        {
            int pagina = 903; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller,901);
            ViewBag.mensajes = JsonConvert.SerializeObject(db.MENSAJES.Where(a => (a.PAGINA_ID.Equals(901) || a.PAGINA_ID.Equals(0)) && a.SPRAS.Equals(user.SPRAS_ID)).ToList(), Formatting.Indented);
            try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                }
            ViewBag.FRECUENCIA = new SelectList(FnCommon.ObtenerCmbFrecuencia(user.SPRAS_ID), "Value", "Text");
            ViewBag.ORDINAL_DSEMANA = new SelectList(FnCommon.ObtenerCmbDias(user.SPRAS_ID), "Value", "Text");
            ViewBag.ORDINAL_MES = new SelectList(FnCommon.ObtenerCmbOrdinales(user.SPRAS_ID), "Value", "Text");
            Session["spras"] = user.SPRAS_ID;
                ViewBag.lan = user.SPRAS_ID;
            return View();
        }

        // POST: Negs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TITULO,FINICIO,FRECUENCIA,FRECUENCIA_N,DIA_SEMANA,DIA_MES,ORDINAL_MES,ORDINAL_DSEMANA")] NEGOCIACION2 nEGOCIACION, FormCollection col)
        {
            int pagina = 903; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller, 901);
            ViewBag.mensajes = JsonConvert.SerializeObject(db.MENSAJES.Where(a => (a.PAGINA_ID.Equals(901) || a.PAGINA_ID.Equals(0)) && a.SPRAS.Equals(user.SPRAS_ID)).ToList(), Formatting.Indented);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
            }
            ViewBag.lan = user.SPRAS_ID;
            if (ModelState.IsValid)
            {
                NEGOCIACION2 nego = new NEGOCIACION2();
                var proximaFecha = FnCommon.obtenerProximaFecha(nEGOCIACION, col["group1"]);
                nego.FRECUENCIA_N = nEGOCIACION.FRECUENCIA_N;
                nego.TITULO = nEGOCIACION.TITULO;
                nego.FRECUENCIA = nEGOCIACION.FRECUENCIA;
                nego.FINICIO = nEGOCIACION.FINICIO;
                if(nego.FRECUENCIA=="S")
                {
                    nego.DIA_SEMANA = nEGOCIACION.DIA_SEMANA;
                }
                else if(nego.FRECUENCIA=="M")
                {
                    if (col["group1"] == "1")
                    {
                        nego.DIA_MES = nEGOCIACION.DIA_MES;
                    }
                    else
                    {
                        nego.ORDINAL_DSEMANA = nEGOCIACION.ORDINAL_DSEMANA;
                        nego.ORDINAL_MES = nEGOCIACION.ORDINAL_MES;
                    }
                }
                db.NEGOCIACION2.Add(nego);
                db.SaveChanges();
                try
                {
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    return View("Error");
                }
            }
            else
            {
                ViewBag.FRECUENCIA = new SelectList(FnCommon.ObtenerCmbFrecuencia(user.SPRAS_ID), "Value", "Text");
                ViewBag.ORDINAL_DSEMANA = new SelectList(FnCommon.ObtenerCmbDias(user.SPRAS_ID), "Value", "Text");
                ViewBag.ORDINAL_MES = new SelectList(FnCommon.ObtenerCmbOrdinales(user.SPRAS_ID), "Value", "Text");
                return View(nEGOCIACION);
            }          
        }

        // GET: Negs/Edit/5
        public ActionResult Edit(int id)
        {
            int pagina = 902; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller, 901);
            ViewBag.mensajes = JsonConvert.SerializeObject(db.MENSAJES.Where(a => (a.PAGINA_ID.Equals(901) || a.PAGINA_ID.Equals(0)) && a.SPRAS.Equals(user.SPRAS_ID)).ToList(), Formatting.Indented);
            try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                }
                Session["spras"] = user.SPRAS_ID;
                ViewBag.lan = user.SPRAS_ID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NEGOCIACION2 nEGOCIACION = db.NEGOCIACION2.Where(x => x.ID == id).FirstOrDefault();
            if (nEGOCIACION == null)
            {
                return HttpNotFound();
            }
            ViewBag.FRECUENCIA = new SelectList(FnCommon.ObtenerCmbFrecuencia(user.SPRAS_ID), "Value", "Text",nEGOCIACION.FRECUENCIA);
            ViewBag.ORDINAL_DSEMANA = new SelectList(FnCommon.ObtenerCmbDias(user.SPRAS_ID), "Value", "Text", nEGOCIACION.ORDINAL_DSEMANA);
            ViewBag.ORDINAL_MES = new SelectList(FnCommon.ObtenerCmbOrdinales(user.SPRAS_ID), "Value", "Text", nEGOCIACION.ORDINAL_MES);
            return View(nEGOCIACION);
        }

        // POST: Negs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TITULO,FINICIO,FRECUENCIA,FRECUENCIA_N,DIA_SEMANA,DIA_MES,ORDINAL_MES,ORDINAL_DSEMANA")] NEGOCIACION2 nEGOCIACION, FormCollection col)
        {
            int pagina = 902; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller, 901);
            ViewBag.mensajes = JsonConvert.SerializeObject(db.MENSAJES.Where(a => (a.PAGINA_ID.Equals(901) || a.PAGINA_ID.Equals(0)) && a.SPRAS.Equals(user.SPRAS_ID)).ToList(), Formatting.Indented);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
            }
            ViewBag.lan = user.SPRAS_ID;
            if (ModelState.IsValid)
            {
                NEGOCIACION2 nego = db.NEGOCIACION2.Find(nEGOCIACION.ID);
                var proximaFecha = FnCommon.obtenerProximaFecha(nEGOCIACION, col["group1"]);

                NEGOCIACION nn = db.NEGOCIACIONs.Find(1);
                nn.FECHAI = new DateTime();
                nn.FECHAF = proximaFecha;
                nn.FECHAN = proximaFecha;
                nn.ACTIVO = true;
                db.Entry(nn).State = EntityState.Modified;

                nego.FRECUENCIA_N = nEGOCIACION.FRECUENCIA_N;
                nego.TITULO = nEGOCIACION.TITULO;
                nego.FRECUENCIA = nEGOCIACION.FRECUENCIA;
                nego.FINICIO = nEGOCIACION.FINICIO;
                if (nego.FRECUENCIA == "S")
                {
                    nego.DIA_SEMANA = nEGOCIACION.DIA_SEMANA;
                    nego.DIA_MES = null;
                    nego.ORDINAL_DSEMANA = null;
                    nego.ORDINAL_MES = null;
                }
                else if (nego.FRECUENCIA == "M")
                {
                    nego.DIA_SEMANA = null;
                    if (col["group1"] == "1")
                    {
                        nego.DIA_MES = nEGOCIACION.DIA_MES;
                        nego.ORDINAL_DSEMANA = null;
                        nego.ORDINAL_MES = null;
                    }
                    else
                    {
                        nego.DIA_MES = null;
                        nego.ORDINAL_DSEMANA = nEGOCIACION.ORDINAL_DSEMANA;
                        nego.ORDINAL_MES = nEGOCIACION.ORDINAL_MES;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.FRECUENCIA = new SelectList(FnCommon.ObtenerCmbFrecuencia(user.SPRAS_ID), "Value", "Text");
                ViewBag.ORDINAL_DSEMANA = new SelectList(FnCommon.ObtenerCmbDias(user.SPRAS_ID), "Value", "Text");
                ViewBag.ORDINAL_MES = new SelectList(FnCommon.ObtenerCmbOrdinales(user.SPRAS_ID), "Value", "Text");
                return View(nEGOCIACION);
            }
        }

        // GET: Negs/Delete/5
        public ActionResult Delete(long? id)
        {
            int pagina = 904; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(901) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.svg";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
                ViewBag.lan = user.SPRAS_ID;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NEGOCIACION nEGOCIACION = db.NEGOCIACIONs.Where(x => x.ID == id).FirstOrDefault();
            if (nEGOCIACION == null)
            {
                return HttpNotFound();
            }
            return View(nEGOCIACION);
        }

        // POST: Negs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            NEGOCIACION ng = db.NEGOCIACIONs.Where(x => x.ID == id).FirstOrDefault();
            ng.ACTIVO = false;
            db.Entry(ng).State = EntityState.Modified;
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
