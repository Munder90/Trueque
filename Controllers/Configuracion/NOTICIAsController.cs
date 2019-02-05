using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers.Configuracion
{
    [Authorize]
    [LoginActive]
    public class NOTICIAsController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: NOTICIAs
        public ActionResult Index()
        {
            int pagina = 911; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
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
            var nOTICIAs = db.NOTICIAs.Where(n => n.ACTIVO == true);
            ViewBag.imgnoticia = db.NOTICIAs.Where(x => x.FECHAI <= DateTime.Now && x.FECHAF >= DateTime.Now && x.ACTIVO == true).Select(x => x.PATH).FirstOrDefault();
            return View(nOTICIAs.ToList());
        }

        // GET: NOTICIAs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NOTICIA nOTICIA = db.NOTICIAs.Find(id);
            if (nOTICIA == null)
            {
                return HttpNotFound();
            }
            return View(nOTICIA);
        }

        // GET: NOTICIAs/Create
        public ActionResult Create()
        {
            int pagina = 912; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
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
            ViewBag.imgnoticia = db.NOTICIAs.Where(x => x.FECHAI <= DateTime.Now && x.FECHAF >= DateTime.Now && x.ACTIVO == true).Select(x => x.PATH).FirstOrDefault();
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS");
            return View();
        }

        // POST: NOTICIAs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FECHAI,FECHAF,PATH,USUARIOC_ID,FECHAC,ACTIVO")] NOTICIA nOTICIA)
        {
            if (ModelState.IsValid)
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                var _p = db.NOTICIAs.Where(x => x.PATH == "/images/" + nOTICIA.PATH).FirstOrDefault();
                //si p es null, significa que podemos hacer incersiones
                if (_p == null)
                {
                    nOTICIA.FECHAC = DateTime.Now;
                    nOTICIA.PATH = "/images/" + nOTICIA.PATH;
                    nOTICIA.USUARIOC_ID = user.ID;
                    nOTICIA.ACTIVO = true;
                    db.NOTICIAs.Add(nOTICIA);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.error = "Ya existe un registro con esos datos";
            int pagina = 912; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
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
            ViewBag.imgnoticia = db.NOTICIAs.Where(x => x.FECHAI <= DateTime.Now && x.FECHAF >= DateTime.Now && x.ACTIVO == true).Select(x => x.PATH).FirstOrDefault();
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS", nOTICIA.USUARIOC_ID);
            return View(nOTICIA);
        }

        // GET: NOTICIAs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NOTICIA nOTICIA = db.NOTICIAs.Find(id);
            if (nOTICIA == null)
            {
                return HttpNotFound();
            }
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS", nOTICIA.USUARIOC_ID);
            return View(nOTICIA);
        }

        // POST: NOTICIAs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FECHAI,FECHAF,PATH,USUARIOC_ID,FECHAC,ACTIVO")] NOTICIA nOTICIA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nOTICIA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS", nOTICIA.USUARIOC_ID);
            return View(nOTICIA);
        }

        // GET: NOTICIAs/Delete/5
        public ActionResult Delete(long? id)
        {
            int pagina = 913; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
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
            ViewBag.imgnoticia = db.NOTICIAs.Where(x => x.FECHAI <= DateTime.Now && x.FECHAF >= DateTime.Now && x.ACTIVO == true).Select(x => x.PATH).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NOTICIA nOTICIA = db.NOTICIAs.Find(id);
            if (nOTICIA == null)
            {
                return HttpNotFound();
            }
            return View(nOTICIA);
        }

        // POST: NOTICIAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            NOTICIA nOTICIA = db.NOTICIAs.Find(id);
            db.NOTICIAs.Remove(nOTICIA);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public string guardarImagen()
        {
            var _xx = "";
            if (Request.Files.Count > 0)
            {
                var f = Request.Files[0];
                var fileName = Path.GetFileName(f.FileName);
                var path = Path.Combine(Server.MapPath("~/images/"), fileName);
                //Para comprobar si el archivo ya existe en ese directorio
                if (System.IO.File.Exists(path))
                {
                    _xx = "0";
                }
                else
                {
                    f.SaveAs(path);
                    _xx = fileName;
                }
            }
            return _xx;
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
