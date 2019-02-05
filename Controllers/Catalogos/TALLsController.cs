using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class TALLsController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: TALLs
        public ActionResult Index()
        {
            int pagina = 721; //ID EN BASE DE DATOS
            int pagina2 = 881;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina2)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina2) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }
            var tALLs = db.TALLs.Include(t => t.GALL);
            return View(tALLs.ToList());
        }

        // GET: TALLs/Details/5
        public ActionResult Details(string id)
        {
            int pagina = 882; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TALL tALL = db.TALLs.Find(id);
            if (tALL == null)
            {
                return HttpNotFound();
            }
            return View(tALL);
        }

        // GET: TALLs/Create
        public ActionResult Create()
        {
            int pagina = 884; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }
            ViewBag.GALL_ID = new SelectList(db.GALLs, "ID", "DESCRIPCION");
            return View();
        }

        // POST: TALLs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DESCRIPCION,FECHAI,FECHAF,GALL_ID,ACTIVO")] TALL tALL)
        {       
            if (ModelState.IsValid)
            {
                //TSOPORTE TS = new TSOPORTE();
               
                tALL.FECHAI = DateTime.Today;
                tALL.FECHAF = DateTime.MaxValue;
                tALL.ACTIVO = true;
                db.TALLs.Add(tALL);
                db.SaveChanges();
                List<SPRA> ss = db.SPRAS.ToList();

                foreach (SPRA s in ss)
                {
                    TALLT pt = new TALLT();
                    pt.TALL_ID = tALL.ID;
                    pt.SPRAS_ID = s.ID;
                    pt.TXT50 = tALL.DESCRIPCION;
                    db.TALLTs.Add(pt);                  
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        
          
            int pagina = 723; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(721) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }
            ViewBag.GALL_ID = new SelectList(db.GALLs, "ID", "DESCRIPCION");
            return View(tALL);
        }

        // GET: TALLs/Edit/5
        public ActionResult Edit(string id)
        {
            int pagina = 883; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TALL tALL = db.TALLs.Find(id);
            if (tALL == null)
            {
                return HttpNotFound();
            }
            ViewBag.GALL_ID = new SelectList(db.GALLs, "ID", "DESCRIPCION", tALL.GALL_ID);
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tALL);
        }

        // POST: TALLs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GALL_ID,DESCRIPCION,ACTIVO")] TALL tALL, string txtN, string sp, string[] txval)
        {
            if (ModelState.IsValid)
            {
                tALL.ACTIVO = tALL.ACTIVO == null ? false : tALL.ACTIVO;
                //tALL.ACTIVO = tALL.ACTIVO;
                var fecha = from a in db.TALLs where a.ID == tALL.ID select a.FECHAI;
                tALL.FECHAI = fecha.FirstOrDefault();
                tALL.FECHAF = DateTime.MaxValue;
                db.Entry(tALL).State = EntityState.Modified;

                foreach (SPRA spr in db.SPRAS.ToList())
                {
                    string val = Request.Form["A" + spr.ID];
                    TALLT tt = db.TALLTs.Where(x => x.SPRAS_ID == spr.ID & x.TALL_ID == tALL.ID).FirstOrDefault();
                    tt.TXT50 = val;
                    db.Entry(tt).State = EntityState.Modified;
                    db.SaveChanges();

                }
                db.Entry(tALL).State = EntityState.Modified;
                db.SaveChanges();
                
                if (txval != null)
                {
                    //Posterior a lo ingresado
                    List<TALLT> lstc = db.TALLTs.Where(i => i.TALL_ID == tALL.ID).ToList();
                    //si el arreglo solo incluye 1 dato, significa que ya hay 2 lenguajes
                    if (txval.Length == 1)
                    {
                        var x1 = lstc[0].SPRAS_ID;
                        var x2 = lstc[1].SPRAS_ID;
                        if (lstc[0].SPRAS_ID == "EN")
                        {
                            if (lstc[1].SPRAS_ID == "ES")
                            {
                                // Lleno el primer objeto
                                TALLT trvt = new TALLT();
                                trvt.SPRAS_ID = "PT";
                                trvt.TALL_ID = tALL.ID;
                                trvt.TXT50 = txval[0];
                                db.TALLTs.Add(trvt);
                                db.SaveChanges();
                            }
                            if (lstc[1].SPRAS_ID == "PT")
                            {  //Lleno el primer objeto
                                TALLT trvt = new TALLT();
                                trvt.SPRAS_ID = "ES";
                                trvt.TALL_ID = tALL.ID;
                                trvt.TXT50 = txval[0];
                                db.TALLTs.Add(trvt);
                                db.SaveChanges();
                            }
                        }
                        if (lstc[0].SPRAS_ID == "ES")
                        {
                            if (lstc[1].SPRAS_ID == "PT")
                            {
                                //Lleno el primer objeto
                                TALLT trvt = new TALLT();
                                trvt.SPRAS_ID = "EN";
                                trvt.TALL_ID = tALL.ID;
                                trvt.TXT50 = txval[0];
                                db.TALLTs.Add(trvt);
                                db.SaveChanges();
                            }
                        }
                    }
                    //si el arreglo  incluye 2 datos, significa que ya hay 1 lenguaje
                    else if (txval.Length == 2)
                    {
                        if (lstc[0].SPRAS_ID == "ES")
                        {
                            //Lleno el primer objeto
                            TALLT trvt = new TALLT();
                            trvt.SPRAS_ID = "EN";
                            trvt.TALL_ID = tALL.ID;
                            trvt.TXT50 = txval[0];
                            db.TALLTs.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TALLT trvt2 = new TALLT();
                            trvt2.SPRAS_ID = "PT";
                            trvt2.TALL_ID = tALL.ID;
                            trvt2.TXT50 = txval[1];
                            db.TALLTs.Add(trvt2);
                            db.SaveChanges();
                        }
                        else if (lstc[0].SPRAS_ID == "EN")
                        {
                            //Lleno el primer objeto
                            TALLT trvt = new TALLT();
                            trvt.SPRAS_ID = "ES";
                            trvt.TALL_ID = tALL.ID;
                            trvt.TXT50 = txval[0];
                            db.TALLTs.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TALLT trvt2 = new TALLT();
                            trvt2.SPRAS_ID = "PT";
                            trvt2.TALL_ID = tALL.ID;
                            trvt2.TXT50 = txval[1];
                            db.TALLTs.Add(trvt2);
                            db.SaveChanges();
                        }
                        else if (lstc[0].SPRAS_ID == "PT")
                        {
                            //Lleno el primer objeto
                            TALLT trvt = new TALLT();
                            trvt.SPRAS_ID = "ES";
                            trvt.TALL_ID = tALL.ID;
                            trvt.TXT50 = txval[0];
                            db.TALLTs.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TALLT trvt2 = new TALLT();
                            trvt2.SPRAS_ID = "EN";
                            trvt2.TALL_ID = tALL.ID;
                            trvt2.TXT50 = txval[1];
                            db.TALLTs.Add(trvt2);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            int pagina = 723; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(721) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }

            ViewBag.GALL_ID = new SelectList(db.GALLs, "ID", "DESCRIPCION", tALL.GALL_ID);
            return View(tALL);
        }

        // GET: TALLs/Delete/5
        public ActionResult Delete(string id)
        {
            int pagina = 723; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(721) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TALL tALL = db.TALLs.Find(id);
            ViewBag.SPRAS = db.SPRAS.ToList();
            if (tALL == null)
            {
                return HttpNotFound();
            }
            return View(tALL);
        }

        // POST: TALLs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                TALL tALLs = db.TALLs.Find(id);
                //db.TALLs.Remove(tALL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                int pagina = 723; //ID EN BASE DE DATOS
                using (TruequeEntities db = new TruequeEntities())
                {
                    string u = User.Identity.Name;
                    //string u = "admin";
                    var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                    ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                    ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                    ViewBag.usuario = user;
                    ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                    ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                    ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                    ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(721) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
                }
            }
            return View();
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
