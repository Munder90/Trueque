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
    public class TextosController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Textos
        public ActionResult Index(string id)
        {
            int pagina = 520; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                TempData["id"] = id;
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

            }
            try
            {
                int idi = Int32.Parse(id);
                var tEXTOes = db.TEXTOes.Where(a => a.PAGINA_ID.Equals(idi) & a.SPRAS_ID == "ES").Include(t => t.CAMPOS).Include(t => t.SPRA);
                return View(tEXTOes.ToList());
            }
            catch
            {
                var tEXTOes = db.TEXTOes.Include(t => t.CAMPOS).Include(t => t.SPRA);
                return View(tEXTOes.ToList());
            }
        }

        // GET: Textos/Details/5
        public ActionResult Details(string spras_id, string campo_id, int pagina_id)
        {
            int pagina = 523; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            }
            if (pagina_id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TEXTO tEXTO = db.TEXTOes.Find(pagina_id, campo_id, spras_id);
            List<TEXTO> tEXTO = db.TEXTOes.Where(x => x.PAGINA_ID == pagina_id & x.CAMPO_ID == campo_id).ToList(); ;
            if (tEXTO == null)
            {
                return HttpNotFound();
            }
            //ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION", tEXTO.First().PAGINA_ID);
            //ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "DESCRIPCION", tEXTO.First().SPRAS_ID);
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tEXTO);
        }

        // GET: Textos/Create
        public ActionResult Create(string id)
        {
            int pagina = 521; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    ViewBag.pais = "mx.png";
                    ////return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }
            int ids = Int32.Parse(id);
            //ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION");
            ViewBag.CAMPO_ID = new SelectList(db.CAMPOS.Where(a => a.PAGINA_ID.Equals(ids) && (a.TIPO.Equals("label") | a.TIPO.Equals("btn")) & a.TEXTOes.Count == 0), "ID", "ID");
            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "ID");

            TEXTO w = new TEXTO();
            w.PAGINA_ID = ids;
            return View(w);
        }

        // POST: Textos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PAGINA_ID,CAMPO_ID,SPRAS_ID,TEXTOS,ACTIVO")] TEXTO tEXTO)
        {
            if (ModelState.IsValid)
            {
                List<SPRA> lan = db.SPRAS.ToList();
                foreach (SPRA l in lan)
                {
                    try
                    {
                        var obj = db.TEXTOes.Where(a => a.PAGINA_ID.Equals(tEXTO.PAGINA_ID) && a.CAMPO_ID.Equals(tEXTO.CAMPO_ID) && a.SPRAS_ID.Equals(l.ID)).FirstOrDefault();
                        if (obj == null)
                        {
                            TEXTO t = new TEXTO();
                            t.ACTIVO = true;
                            t.CAMPO_ID = tEXTO.CAMPO_ID;
                            t.PAGINA_ID = tEXTO.PAGINA_ID;
                            t.TEXTOS = tEXTO.TEXTOS;
                            t.SPRAS_ID = l.ID;

                            db.TEXTOes.Add(t);
                            db.SaveChanges();
                        }
                    }
                    catch { }
                }
                return RedirectToAction("Index", new { id = tEXTO.PAGINA_ID });
            }
            int pagina = 511; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            }
            //ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION");
            ViewBag.CAMPO_ID = new SelectList(db.CAMPOS.Where(a => a.PAGINA_ID.Equals(tEXTO.PAGINA_ID)), "ID", "ID");
            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "ID");
            ViewBag.Error = "Registro duplicado";
            return View(tEXTO);
        }

        // GET: Textos/Edit/5
        public ActionResult Edit(string spras_id, string campo_id, int pagina_id)
        {
            int pagina = 522; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            }
            if (pagina_id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TEXTO tEXTO = db.TEXTOes.Find(pagina_id, campo_id, spras_id);
            List<TEXTO> tEXTO = db.TEXTOes.Where(x => x.PAGINA_ID == pagina_id & x.CAMPO_ID == campo_id).ToList(); ;
            if (tEXTO == null)
            {
                return HttpNotFound();
            }
            //ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION", tEXTO.First().PAGINA_ID);
            //ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "DESCRIPCION", tEXTO.First().SPRAS_ID);
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tEXTO);
        }

        // POST: Textos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PAGINA_ID,CAMPO_ID,SPRAS_ID,TEXTOS,ACTIVO")] TEXTO tEXTO, string[] txval)
        {
            if (ModelState.IsValid)
            {
                tEXTO.ACTIVO = true;

                foreach (SPRA spr in db.SPRAS.ToList())
                {
                    string val = Request.Form["A" + spr.ID];
                    TEXTO tt = db.TEXTOes.Where(x => x.SPRAS_ID == spr.ID & x.PAGINA_ID == tEXTO.PAGINA_ID & x.CAMPO_ID == tEXTO.CAMPO_ID).FirstOrDefault();
                    if (tt != null)
                    {
                        tt.TEXTOS = val;
                        db.Entry(tt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        TEXTO t = new TEXTO();
                        t.ACTIVO = true;
                        t.CAMPO_ID = tEXTO.CAMPO_ID;
                        t.PAGINA_ID = tEXTO.PAGINA_ID;
                        t.SPRAS_ID = spr.ID;
                        t.TEXTOS = val;
                        db.TEXTOes.Add(t);
                        db.SaveChanges();
                    }

                }

                return RedirectToAction("Index", new { id = tEXTO.PAGINA_ID });
            }


            int pagina = 522; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            }
            ViewBag.PAGINA_ID = new SelectList(db.CAMPOS, "PAGINA_ID", "DESCRIPCION", tEXTO.PAGINA_ID);
            ViewBag.SPRAS_ID = new SelectList(db.SPRAS, "ID", "DESCRIPCION", tEXTO.SPRAS_ID);
            return View(tEXTO);
        }

        // GET: Textos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEXTO tEXTO = db.TEXTOes.Find(id);
            if (tEXTO == null)
            {
                return HttpNotFound();
            }
            return View(tEXTO);
        }

        // POST: Textos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TEXTO tEXTO = db.TEXTOes.Find(id);
            db.TEXTOes.Remove(tEXTO);
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
