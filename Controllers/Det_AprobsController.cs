using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class Det_AprobsController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Det_Aprobs
        public ActionResult Index()
        {
            int pagina = 103; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
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
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                //var dET_APROB = db.DET_APROB.Include(d => d.PUESTO).Include(d => d.PUESTO1).Include(d => d.SOCIEDAD).Include(d => d.PUESTO.PUESTOTs).Include(d => d.PUESTO1.PUESTOTs);
                var dET_APROBV = db.DET_APROBV.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                return View(dET_APROBV);
            }
        }

        // GET: Det_Aprobs/Details/5
        public ActionResult Details(string bukrs, int puestoc)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //DET_APROB dET_APROB = db.DET_APROB.Find(id);
            //if (dET_APROB == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(dET_APROB);

            int pagina = 103; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
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
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                var dET_APROB = db.DET_APROB.Where(a => a.BUKRS.Equals(bukrs) & a.PUESTOC_ID.Equals(puestoc)).Include(d => d.PUESTO).Include(d => d.PUESTO1).Include(d => d.SOCIEDAD).Include(d => d.PUESTO.PUESTOTs).Include(d => d.PUESTO1.PUESTOTs);
                return View(dET_APROB.ToList());
            }
        }

        // GET: Det_Aprobs/Create
        public ActionResult Create()
        {
            int pagina = 103; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
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
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }
            ViewBag.PUESTOC_ID = new SelectList(db.PUESTOes, "ID", "ID");
            ViewBag.PUESTOA_ID = new SelectList(db.PUESTOes, "ID", "ID");
            ViewBag.BUKRS = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT");
            return View();
        }

        // POST: Det_Aprobs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PUESTOC_ID,POS,PUESTOA_ID,BUKRS,MONTO,PRESUPUESTO,ACTIVO")] DET_APROB dET_APROB)
        {
            if (ModelState.IsValid)
            {
                db.DET_APROB.Add(dET_APROB);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PUESTOC_ID = new SelectList(db.PUESTOes, "ID", "ID", dET_APROB.PUESTOC_ID);
            ViewBag.PUESTOA_ID = new SelectList(db.PUESTOes, "ID", "ID", dET_APROB.PUESTOA_ID);
            ViewBag.BUKRS = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", dET_APROB.BUKRS);
            return View(dET_APROB);
        }

        // GET: Det_Aprobs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DET_APROB dET_APROB = db.DET_APROB.Find(id);
            if (dET_APROB == null)
            {
                return HttpNotFound();
            }
            ViewBag.PUESTOC_ID = new SelectList(db.PUESTOes, "ID", "ID", dET_APROB.PUESTOC_ID);
            ViewBag.PUESTOA_ID = new SelectList(db.PUESTOes, "ID", "ID", dET_APROB.PUESTOA_ID);
            ViewBag.BUKRS = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", dET_APROB.BUKRS);
            return View(dET_APROB);
        }

        // POST: Det_Aprobs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PUESTOC_ID,POS,PUESTOA_ID,BUKRS,MONTO,PRESUPUESTO,ACTIVO")] DET_APROB dET_APROB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dET_APROB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PUESTOC_ID = new SelectList(db.PUESTOes, "ID", "ID", dET_APROB.PUESTOC_ID);
            ViewBag.PUESTOA_ID = new SelectList(db.PUESTOes, "ID", "ID", dET_APROB.PUESTOA_ID);
            ViewBag.BUKRS = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", dET_APROB.BUKRS);
            return View(dET_APROB);
        }

        // GET: Det_Aprobs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DET_APROB dET_APROB = db.DET_APROB.Find(id);
            if (dET_APROB == null)
            {
                return HttpNotFound();
            }
            return View(dET_APROB);
        }

        // POST: Det_Aprobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DET_APROB dET_APROB = db.DET_APROB.Find(id);
            db.DET_APROB.Remove(dET_APROB);
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
