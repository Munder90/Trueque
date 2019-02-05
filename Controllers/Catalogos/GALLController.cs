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
using TAT001.Filters;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class GALLController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: GALL
        public ActionResult Index()
        {
            int pagina = 721; //ID EN BASE DE DATOS
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
            var qr = db.GALLs.Include(x => x.GALLTs).ToList();
            return View(qr);
        }

        // GET: GALL/Details/5
        public ActionResult Details(string id)
        {
            int pagina = 722; //ID EN BASE DE DATOS
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
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GALL gALL = db.GALLs.Find(id);
            if (gALL == null)
            {
                return HttpNotFound();
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(gALL);
        }

        // GET: GALL/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: GALL/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DESCRIPCION,ACTIVO,GRUPO_ALL")] GALL gALL)
        {
            if (ModelState.IsValid)
            {
                db.GALLs.Add(gALL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gALL);
        }

        // GET: GALL/Edit/5
        public ActionResult Edit(string id)
        {
            int pagina = 723; //ID EN BASE DE DATOS
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GALL gALL = db.GALLs.Find(id);
            if (gALL == null)
            {
                return HttpNotFound();
            }
            GALL tSOPORTE = db.GALLs.Find(id);
            if (tSOPORTE.GALLTs.Count > 0)
                foreach (var e in tSOPORTE.GALLTs)
                {
                    if (e.SPRAS_ID == "EN")
                        ViewBag.EN = e.TXT50;
                    if (e.SPRAS_ID == "ES")
                        ViewBag.ES = e.TXT50;
                    if (e.SPRAS_ID == "PT")
                        ViewBag.PT = e.TXT50;
                }
            else
            {
                ViewBag.EN = tSOPORTE.DESCRIPCION;
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(gALL);
        }

        // POST: GALL/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DESCRIPCION,ACTIVO,GRUPO_ALL")] GALL gALL, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var GALLID = from a in db.GALLs where a.ID == gALL.ID select a.GRUPO_ALL;
                gALL.GRUPO_ALL = GALLID.FirstOrDefault();
                gALL.ACTIVO =gALL.ACTIVO == null?false :gALL.ACTIVO;
                //gALL.ACTIVO = gALL.ACTIVO;
                db.Entry(gALL).State = EntityState.Modified;
                //db.SaveChanges();

                GALL mATERIAL1 = db.GALLs.Find(gALL.ID);
                var materialtextos = db.GALLTs.Where(t => t.GALL_ID == gALL.ID).ToList();
                db.GALLTs.RemoveRange(materialtextos);
                List<GALLT> ListmATERIALTs = new List<GALLT>();
                if (collection.AllKeys.Contains("EN"))
                {
                    if (!String.IsNullOrEmpty(collection["EN"]))
                    {
                        GALLT m = new GALLT { SPRAS_ID = "EN", GALL_ID = gALL.ID, TXT50 = collection["EN"]};
                        ListmATERIALTs.Add(m);
                    }
                }
                if (collection.AllKeys.Contains("ES") && !String.IsNullOrEmpty(collection["ES"]))
                {
                    GALLT m = new GALLT { SPRAS_ID = "ES", GALL_ID = gALL.ID, TXT50 = collection["ES"]};
                    ListmATERIALTs.Add(m);
                }
                if (collection.AllKeys.Contains("PT") && !String.IsNullOrEmpty(collection["PT"]))
                {
                    GALLT m = new GALLT { SPRAS_ID = "PT", GALL_ID = gALL.ID, TXT50 = collection["PT"]};
                    ListmATERIALTs.Add(m);
                }
                db.GALLTs.AddRange(ListmATERIALTs);
                //db.Entry(mATERIAL).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //if (ModelState.IsValid)
            //{
            //    gALL.ACTIVO = gALL.ACTIVO;
            //    db.Entry(gALL).State = EntityState.Modified;

            //    foreach (SPRA spr in db.SPRAS.ToList())
            //    {
            //        string val = Request.Form["A" + spr.ID];
            //        GALLT tt = db.GALLTs.Where(x => x.SPRAS_ID == spr.ID & x.GALL_ID == gALL.ID).FirstOrDefault();
            //        tt.TXT50 = val;
            //        db.Entry(tt).State = EntityState.Modified;
            //        db.SaveChanges();

            //    }
            //    GALL t = db.GALLs.Where(x => x.ID == gALL.ID).FirstOrDefault();
            //    t.ID = gALL.ID;
            //    try { t.DESCRIPCION = Request.Form["AEN"]; } catch { }
            //    db.Entry(t).State = EntityState.Modified;
            //    db.SaveChanges();

            //    if (txval != null)
            //    {
            //        //Posterior a lo ingresado
            //        List<GALLT> lstc = db.GALLTs.Where(i => i.GALL_ID == gALL.ID).ToList();
            //        //si el arreglo solo incluye 1 dato, significa que ya hay 2 lenguajes
            //        if (txval.Length == 1)
            //        {
            //            var x1 = lstc[0].SPRAS_ID;
            //            var x2 = lstc[1].SPRAS_ID;
            //            if (lstc[0].SPRAS_ID == "EN")
            //            {
            //                if (lstc[1].SPRAS_ID == "ES")
            //                {
            //                    // Lleno el primer objeto
            //                    GALLT trvt = new GALLT();
            //                    trvt.SPRAS_ID = "PT";
            //                    trvt.GALL_ID = gALL.ID;
            //                    trvt.TXT50 = txval[0];
            //                    db.GALLTs.Add(trvt);
            //                    db.SaveChanges();
            //                }
            //                if (lstc[1].SPRAS_ID == "PT")
            //                {  //Lleno el primer objeto
            //                    GALLT trvt = new GALLT();
            //                    trvt.SPRAS_ID = "ES";
            //                    trvt.GALL_ID = gALL.ID;
            //                    trvt.TXT50 = txval[0];
            //                    db.GALLTs.Add(trvt);
            //                    db.SaveChanges();
            //                }
            //            }
            //            if (lstc[0].SPRAS_ID == "ES")
            //            {
            //                if (lstc[1].SPRAS_ID == "PT")
            //                {
            //                    //Lleno el primer objeto
            //                    GALLT trvt = new GALLT();
            //                    trvt.SPRAS_ID = "EN";
            //                    trvt.GALL_ID = gALL.ID;
            //                    trvt.TXT50 = txval[0];
            //                    db.GALLTs.Add(trvt);
            //                    db.SaveChanges();
            //                }
            //            }
            //        }
            //        //si el arreglo  incluye 2 datos, significa que ya hay 1 lenguaje
            //        else if (txval.Length == 2)
            //        {
            //            if (lstc[0].SPRAS_ID == "ES")
            //            {
            //                //Lleno el primer objeto
            //                GALLT trvt = new GALLT();
            //                trvt.SPRAS_ID = "EN";
            //                trvt.GALL_ID = gALL.ID;
            //                trvt.TXT50 = txval[0];
            //                db.GALLTs.Add(trvt);
            //                db.SaveChanges();
            //                //Lleno el segundo objeto
            //                GALLT trvt2 = new GALLT();
            //                trvt2.SPRAS_ID = "PT";
            //                trvt2.GALL_ID = gALL.ID;
            //                trvt2.TXT50 = txval[1];
            //                db.GALLTs.Add(trvt2);
            //                db.SaveChanges();
            //            }
            //            else if (lstc[0].SPRAS_ID == "EN")
            //            {
            //                //Lleno el primer objeto
            //                GALLT trvt = new GALLT();
            //                trvt.SPRAS_ID = "ES";
            //                trvt.GALL_ID = gALL.ID;
            //                trvt.TXT50 = txval[0];
            //                db.GALLTs.Add(trvt);
            //                db.SaveChanges();
            //                //Lleno el segundo objeto
            //                GALLT trvt2 = new GALLT();
            //                trvt2.SPRAS_ID = "PT";
            //                trvt2.GALL_ID = gALL.ID;
            //                trvt2.TXT50 = txval[1];
            //                db.GALLTs.Add(trvt2);
            //                db.SaveChanges();
            //            }
            //            else if (lstc[0].SPRAS_ID == "PT")
            //            {
            //                //Lleno el primer objeto
            //                GALLT trvt = new GALLT();
            //                trvt.SPRAS_ID = "ES";
            //                trvt.GALL_ID = gALL.ID;
            //                trvt.TXT50 = txval[0];
            //                db.GALLTs.Add(trvt);
            //                db.SaveChanges();
            //                //Lleno el segundo objeto
            //                GALLT trvt2 = new GALLT();
            //                trvt2.SPRAS_ID = "EN";
            //                trvt2.GALL_ID = gALL.ID;
            //                trvt2.TXT50 = txval[1];
            //                db.GALLTs.Add(trvt2);
            //                db.SaveChanges();
            //            }
            //        }
            //    }
            //    return RedirectToAction("Index");
            //}
            int pagina = 723; //ID EN BASE DE DATOS
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
            return View(gALL);
        }

        // GET: GALL/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GALL gALL = db.GALLs.Find(id);
            if (gALL == null)
            {
                return HttpNotFound();
            }
            return View(gALL);
        }

        // POST: GALL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GALL gALL = db.GALLs.Find(id);
            db.GALLs.Remove(gALL);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var pr = db.GALLs.Include(x => x.GALLTs).ToList();
            generarExcelHome(pr, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocGall" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocGall" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<GALL> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]{
                  new {
                      BANNER = "ID GRUPO ALLOWANCE"
                      },
                    };
                worksheet.Cell("B1").Value = new[]{
                  new {
                      BANNER = "DESCRIPCION"
                      },
                    };
                worksheet.Cell("C1").Value = new[]{
                  new {
                      BANNER = "GRUPO"
                      },
                    };
                worksheet.Cell("D1").Value = new[]{
                  new {
                      BANNER = "TEXTO"
                      },
                    };
                worksheet.Cell("E1").Value = new[]{
                  new {
                      BANNER = "ACTIVO"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    string activo = lst[i - 2].ACTIVO==true ? "SI" : "NO";
                    worksheet.Cell("A" + i).Value = new[]
                    {
                  new {
                      BANNER       = lst[i-2].ID
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                    {
                  new {
                      BANNER       = lst[i-2].DESCRIPCION
                      },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                    {
                  new {
                      BANNER       = lst[i-2].GRUPO_ALL
                      },
                    };
                    List<GALLT> tst = lst[i - 2].GALLTs.ToList();
                    string u = User.Identity.Name;
                    var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                    var tslt = tst.Where(x => x.SPRAS_ID == user.SPRAS_ID).FirstOrDefault();
                    worksheet.Cell("D" + i).Value = new[]
                    {
                        new {
                            BANNER       = tslt.TXT50
                        },
                    };
                    worksheet.Cell("E" + i).Value = new[]
                   {
                  new {
                      BANNER       = activo
                      },
                    };
                }
                var rt = ruta + @"\DocGall" + DateTime.Now.ToShortDateString() + ".xlsx";
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
