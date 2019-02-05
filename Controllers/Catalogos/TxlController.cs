using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Controllers.Catalogos
{
    public class TxlController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Txl
        public ActionResult Index()
        {
            int pagina = 811; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
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
            var tAX_LAND = db.TAX_LAND.Include(t => t.PAI);
            return View(tAX_LAND.Where(a => a.ACTIVO == true).ToList());
        }

        // GET: Txl/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TAX_LAND tAX_LAND = db.TAX_LAND.Find(id);
            if (tAX_LAND == null)
            {
                return HttpNotFound();
            }
            return View(tAX_LAND);
        }

        // GET: Txl/Create
        public ActionResult Create()
        {
            int pagina = 812; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(811) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            //Solo añadire a una tercer lista las sociedades disponibles
            List<TAX_LAND> lstT = db.TAX_LAND.ToList();
            List<SOCIEDAD> lstS = db.SOCIEDADs.ToList();
            List<SOCIEDAD> lstSoc = new List<SOCIEDAD>();
            for (int y = 0; y < lstS.Count; y++)
            {
                //SOCIEDAD sc = lstT.Where(x => x.SOCIEDAD_ID == lstS[y].BUKRS).FirstOrDefault();
                TAX_LAND sc = lstT.Where(x => x.SOCIEDAD_ID == lstS[y].BUKRS).FirstOrDefault();
                if (sc == null)
                {
                    lstSoc.Add(lstS[y]);
                }
            }
            //ViewBag.Sociedad = new SelectList(lstSoc, "BUKRS", "BUKRS");
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT");
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "LANDX");
            return View();
        }

        // POST: Txl/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SOCIEDAD_ID,PAIS_ID,ACTIVO")] TAX_LAND tAX_LAND)
        {
            if (ModelState.IsValid)
            {
                tAX_LAND.SOCIEDAD_ID =tAX_LAND.SOCIEDAD_ID;
                tAX_LAND.PAIS_ID = tAX_LAND.PAIS_ID;
                tAX_LAND.ACTIVO = true;
                db.TAX_LAND.Add(tAX_LAND);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", tAX_LAND.SOCIEDAD_ID);
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "LAND", tAX_LAND.PAIS_ID);
            return View(tAX_LAND);
        }

        // GET: Txl/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TAX_LAND tAX_LAND = db.TAX_LAND.Find(id);
            if (tAX_LAND == null)
            {
                return HttpNotFound();
            }
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", tAX_LAND.PAIS_ID);
            return View(tAX_LAND);
        }

        // POST: Txl/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOCIEDAD_ID,PAIS_ID,ACTIVO")] TAX_LAND tAX_LAND)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tAX_LAND).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", tAX_LAND.PAIS_ID);
            return View(tAX_LAND);
        }

        // GET: Txl/Delete/5
        public ActionResult Delete(string sid, string pid)
        {
            int pagina = 813; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(811) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            if (sid == null && pid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TAX_LAND tAX_LAND = db.TAX_LAND.Where(x => x.SOCIEDAD_ID == sid && x.PAIS_ID == pid).FirstOrDefault();
            if (tAX_LAND == null)
            {
                return HttpNotFound();
            }
            return View(tAX_LAND);
        }

        // POST: Txl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(TAX_LAND id)
        {
            TAX_LAND tAX_LAND = db.TAX_LAND.Where(t => t.SOCIEDAD_ID == id.SOCIEDAD_ID).FirstOrDefault();
            tAX_LAND.PAIS_ID = tAX_LAND.PAI.LAND;
            tAX_LAND.ACTIVO = false;
            db.Entry(tAX_LAND).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var tAX_LAND = db.TAX_LAND.Include(t => t.PAI);
            generarExcelHome(tAX_LAND.Where(a => a.ACTIVO == true).ToList(), Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTxl" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTxl" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TAX_LAND> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                worksheet.Cell("A1").Value = new[]
                {
                  new {
                      BANNER = "SOCIEDAD ID"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
                {
                    new {
                        BANNER = "PAIS"
                    },
                };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
               {
                        new {
                      BANNER       = lst[i-2].SOCIEDAD_ID
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].PAI.LAND
                      },
                    };
                }
                var rt = ruta + @"\DocTxl" + DateTime.Now.ToShortDateString() + ".xlsx";
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }
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
