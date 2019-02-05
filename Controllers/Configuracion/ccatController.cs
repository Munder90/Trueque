using ClosedXML.Excel;
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

namespace TAT001.Controllers
{
    [Authorize]
    public class ccatController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: ccat
        public ActionResult Index(string id)
        {
            int pagina_id = 871; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            ////using (TruequeEntities db = new TruequeEntities())
            ////{
            ////    string u = User.Identity.Name;
            ////    //string u = "admin";
            ////    var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ////    ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ////    ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ////    ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
            ////    ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ////    ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ////    ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ////    ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

            ////    try
            ////    {
            ////        string p = Session["pais"].ToString();
            ////        ViewBag.pais = p + ".svg";
            ////    }
            ////    catch
            ////    {
            ////        //ViewBag.pais = "mx.svg";
            ////        //return RedirectToAction("Pais", "Home");
            ////    }
            ////    Session["spras"] = user.SPRAS_ID;
            ////}

            List<CONFDIST_CAT> cONFDIST_CAT ;
            if (id==null)
             cONFDIST_CAT = db.CONFDIST_CAT.Include(c => c.CAMPOZKE24).Include(c => c.SOCIEDAD).ToList();
            else
                cONFDIST_CAT = db.CONFDIST_CAT.Where(x=>x.SOCIEDAD_ID == id).Include(c => c.CAMPOZKE24).Include(c => c.SOCIEDAD).ToList();

            return View(cONFDIST_CAT.Where(a => a.ACTIVO == true).ToList());
        }

        // GET: ccat/Details/5
        public ActionResult Details(string id)
        {
            int pagina_id = 872; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 871);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ////int per = Convert.ToInt32(pr);
            var cONFDIST_CAT = db.CONFDIST_CAT.Where(i => i.SOCIEDAD_ID == id).FirstOrDefault();
            if (cONFDIST_CAT == null)
            {
                return RedirectToAction("Create", new { id = id});
                ////return HttpNotFound();
            }
            ViewBag.porcad = (cONFDIST_CAT.PORC_AD == true) ? "True" : "False";
            return View(cONFDIST_CAT);
        }

        // GET: ccat/Create
        public ActionResult Create(string id)
        {
            int pagina_id = 874; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 871);
            
            List<bool> lstb = new List<bool>();
            lstb.Add(true);
            lstb.Add(false);
            ViewBag.CAMPO = new SelectList(db.CAMPOZKE24, "CAMPO", "CAMPO");
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", id);
            ViewBag.bools = new SelectList(lstb);
            return View();
        }

        // POST: ccat/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SOCIEDAD_ID,CAMPO,PORC_AD,PERIODOS,ACTIVO")] CONFDIST_CAT cONFDIST_CAT, string bools)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == cONFDIST_CAT.SOCIEDAD_ID))
                    {
                        if (db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == cONFDIST_CAT.SOCIEDAD_ID && x.CAMPO == cONFDIST_CAT.CAMPO && x.ACTIVO == true))
                        {
                            return RedirectToAction("Details", new { id = cONFDIST_CAT.SOCIEDAD_ID });
                        }
                        else
                        {
                            ////CONFDIST_CAT cd = db.CONFDIST_CAT.First(x => x.SOCIEDAD_ID == cONFDIST_CAT.SOCIEDAD_ID);

                            ////cd.CAMPO = cONFDIST_CAT.CAMPO;
                            ////cd.ACTIVO = true;
                            ////cd.PORC_AD = false;
                            ////cd.PERIODOS = cONFDIST_CAT.PERIODOS;
                            
                            ////db.Entry(cd).State = EntityState.Modified;

                        }
                    }
                    else
                    {
                        var cf = cONFDIST_CAT;
                        if (cf.PERIODOS == null)
                        {
                            cf.PERIODOS = 0;
                        }
                        cf.ACTIVO = true;
                        cf.PORC_AD = (bools == "True") ? true : false;
                        db.CONFDIST_CAT.Add(cf);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = cONFDIST_CAT.SOCIEDAD_ID });
                }
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }
            int pagina = 874; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(871) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            List<bool> lstb = new List<bool>();
            lstb.Add(true);
            lstb.Add(false);
            ViewBag.CAMPO = new SelectList(db.CAMPOZKE24, "CAMPO", "CAMPO", cONFDIST_CAT.CAMPO);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", cONFDIST_CAT.SOCIEDAD_ID);
            ViewBag.bools = new SelectList(lstb);
            return View(cONFDIST_CAT);
        }

        // GET: ccat/Edit/5
        public ActionResult Edit(string id)
        {
            int pagina_id = 873; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 871);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ////var per = Convert.ToInt32(pr);
            CONFDIST_CAT cONFDIST_CAT = db.CONFDIST_CAT.Where(i => i.SOCIEDAD_ID == id).FirstOrDefault();
            if (cONFDIST_CAT == null)
            {
                return HttpNotFound();
            }
            List<bool> lstb = new List<bool>();
            lstb.Add(true);
            lstb.Add(false);
            ViewBag.CAMPO = new SelectList(db.CAMPOZKE24, "CAMPO", "CAMPO", cONFDIST_CAT.CAMPO);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", cONFDIST_CAT.SOCIEDAD_ID);
            ViewBag.bools = new SelectList(lstb, cONFDIST_CAT.PORC_AD);
            return View(cONFDIST_CAT);
        }

        // POST: ccat/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOCIEDAD_ID,CAMPO,PORC_AD,PERIODOS,ACTIVO")] CONFDIST_CAT cONFDIST_CAT, string bools)
        {
            try
            {
                ////CONFDIST_CAT cf = db.CONFDIST_CAT.Where(i => i.SOCIEDAD_ID == cONFDIST_CAT.SOCIEDAD_ID && i.CAMPO == cONFDIST_CAT.CAMPO && i.PERIODOS == cONFDIST_CAT.PERIODOS).FirstOrDefault();
                CONFDIST_CAT cf = db.CONFDIST_CAT.Where(i => i.SOCIEDAD_ID == cONFDIST_CAT.SOCIEDAD_ID).FirstOrDefault();
                cf.PORC_AD = false;
                cf.ACTIVO = true;
                cf.CAMPO = cONFDIST_CAT.CAMPO;
                cf.PERIODOS = cONFDIST_CAT.PERIODOS;
                if (cf.PERIODOS == null)
                    cf.PERIODOS = 1;
                if (ModelState.IsValid)
                {
                    db.Entry(cf).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = cf.SOCIEDAD_ID });
                }
            }
            catch (Exception e)
            {
                ViewBag.error = e.ToString();
            }
            int pagina = 873; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(871) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
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
            List<bool> lstb = new List<bool>();
            lstb.Add(true);
            lstb.Add(false);
            ViewBag.CAMPO = new SelectList(db.CAMPOZKE24, "CAMPO", "CAMPO", cONFDIST_CAT.CAMPO);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUKRS", cONFDIST_CAT.SOCIEDAD_ID);
            ViewBag.bools = new SelectList(lstb);
            return View(cONFDIST_CAT);
        }

        // GET: ccat/Delete/5
        public ActionResult Delete(string id, string cp, string pr)
        {
            int pagina = 875; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(871) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            int per = Convert.ToInt32(pr);
            var cONFDIST_CAT = db.CONFDIST_CAT.Where(i => i.SOCIEDAD_ID == id && i.CAMPO == cp && i.PERIODOS == per).FirstOrDefault();
            if (cONFDIST_CAT == null)
            {
                return HttpNotFound();
            }
            ViewBag.porcad = (cONFDIST_CAT.PORC_AD == true) ? "True" : "False";
            return View(cONFDIST_CAT);
        }

        // POST: ccat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string cp, string pr)
        {
            int per = Convert.ToInt32(pr);
            CONFDIST_CAT cONFDIST_CAT = db.CONFDIST_CAT.Where(i => i.SOCIEDAD_ID == id && i.CAMPO == cp && i.PERIODOS == per).FirstOrDefault();
            cONFDIST_CAT.ACTIVO = false;
            db.Entry(cONFDIST_CAT).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var cONFDIST_CAT = db.CONFDIST_CAT.Include(c => c.CAMPOZKE24).Include(c => c.SOCIEDAD);
            generarExcelHome(cONFDIST_CAT.Where(x => x.ACTIVO == true).ToList(), Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocCC" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocCC" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<CONFDIST_CAT> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
             {
                  new {
                      BANNER = "Periodos"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
            {
                  new {
                      BANNER = "Descripción"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
            {
                  new {
                      BANNER = "Sociedad"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
               {
                  new {
                      BANNER       = lst[i-2].PERIODOS
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].CAMPOZKE24.DESCRIPCION
                      },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].SOCIEDAD_ID
                        },
                      };
                }
                var rt = ruta + @"\DocCC" + DateTime.Now.ToShortDateString() + ".xlsx";
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
