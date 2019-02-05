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
    public class TCambioController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: TCambio
        public ActionResult Index()
        {
            int pagina = 831; //ID EN BASE DE DATOS
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
                ViewBag.lan = user.SPRAS_ID;
            }
            return View(db.TCAMBIOs.ToList());
        }
        // GET: TCambio/Details/5
        public ActionResult Details(string fcur, string tcur, string gd)
        {
            int pagina = 832; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(831) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
                //Para las version de las fechas
                var arrF = gd.Split('/');
                var dtgd = arrF[1] + '/' + arrF[0] + '/' + arrF[2];
                DateTime dt = DateTime.Parse(dtgd);
                var con = db.TCAMBIOs
                         .Where(x => x.FCURR == fcur && x.TCURR == tcur && x.GDATU == dt).FirstOrDefault();

                TCAMBIO co = new TCAMBIO();
                co.KURST = con.KURST;
                co.FCURR = con.FCURR;
                co.TCURR = con.TCURR;
                co.GDATU = con.GDATU;
                co.UKURS = con.UKURS;
                return View(co);
            }
        }
        // GET: TCambio/Create
        //public ActionResult Create()
        //{
        //    int pagina = 834; //ID EN BASE DE DATOS
        //    using (TruequeEntities db = new TruequeEntities())
        //    {
        //        string u = User.Identity.Name;
        //        //string u = "admin";
        //        var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
        //        ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
        //        ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
        //        ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
        //        ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //        ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //        ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //        ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(831) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

        //        try
        //        {
        //            string p = Session["pais"].ToString();
        //            ViewBag.pais = p + ".svg";
        //        }
        //        catch
        //        {
        //            //ViewBag.pais = "mx.svg";
        //            //return RedirectToAction("Pais", "Home");
        //        }
        //        Session["spras"] = user.SPRAS_ID;
        //    }
        //    ViewBag.TCURR = new SelectList(db.MONEDAs, "WAERS", "WAERS");
        //    ViewBag.FCURR = new SelectList(db.MONEDAs, "WAERS", "WAERS");
        //    return View();
        //}

        // POST: TCambio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "KURST,FCURR,TCURR,GDATU,UKURS")] TCAMBIO tCAMBIO)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (tCAMBIO.UKURS != 0 && tCAMBIO.UKURS != null)
        //            {
        //                tCAMBIO.KURST = "C";
        //                db.TCAMBIOs.Add(tCAMBIO);
        //                db.SaveChanges();
        //                return RedirectToAction("Index");
        //            }
        //        }

        //        ViewBag.TCURR = new SelectList(db.MONEDAs, "WAERS", "WAERS", tCAMBIO.TCURR);
        //        ViewBag.FCURR = new SelectList(db.MONEDAs, "WAERS", "WAERS", tCAMBIO.FCURR);
        //        //Como se genera la lista se borra y se ocupa regenerar
        //        int pagina = 834; //ID EN BASE DE DATOS
        //        using (TruequeEntities db = new TruequeEntities())
        //        {
        //            string u = User.Identity.Name;
        //            //string u = "admin";
        //            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
        //            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
        //            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
        //            ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
        //            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(831) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //            ViewBag.error = "Campo Nulo o Valor Incorrecto";
        //            try
        //            {
        //                string p = Session["pais"].ToString();
        //                ViewBag.pais = p + ".svg";
        //            }
        //            catch
        //            {
        //                //ViewBag.pais = "mx.svg";
        //                //return RedirectToAction("Pais", "Home");
        //            }
        //            Session["spras"] = user.SPRAS_ID;
        //        }

        //        return View(tCAMBIO);
        //    }
        //    catch (Exception e)
        //    {
        //        ViewBag.TCURR = new SelectList(db.MONEDAs, "WAERS", "WAERS", tCAMBIO.TCURR);
        //        ViewBag.FCURR = new SelectList(db.MONEDAs, "WAERS", "WAERS", tCAMBIO.FCURR);
        //        //Como se genera la lista se borra y se ocupa regenerar
        //        int pagina = 834; //ID EN BASE DE DATOS
        //        using (TruequeEntities db = new TruequeEntities())
        //        {
        //            string u = User.Identity.Name;
        //            //string u = "admin";
        //            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
        //            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
        //            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
        //            ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
        //            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(831) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //            ViewBag.error = e.Message.ToString();
        //            try
        //            {
        //                string p = Session["pais"].ToString();
        //                ViewBag.pais = p + ".svg";
        //            }
        //            catch
        //            {
        //                //ViewBag.pais = "mx.svg";
        //                //return RedirectToAction("Pais", "Home");
        //            }
        //            Session["spras"] = user.SPRAS_ID;
        //        }
        //        return View(tCAMBIO);
        //    }

        //}

        // GET: TCambio/Edit/5
        public ActionResult Edit(string fcur, string tcur, string gd)
        {
            int pagina = 833; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(831) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;    
                //Para las version de las fechas
                var arrF = gd.Split('/');
                var dtgd = arrF[1] + '/' + arrF[0] + '/' + arrF[2];
                DateTime dt = DateTime.Parse(dtgd);
                var con = db.TCAMBIOs
                          .Where(x => x.FCURR == fcur && x.TCURR == tcur && x.GDATU == dt).FirstOrDefault();
                TCAMBIO co = new TCAMBIO();
                co.KURST = con.KURST;
                co.FCURR = con.FCURR;
                co.TCURR = con.TCURR;
                co.GDATU = con.GDATU;
                co.UKURS = con.UKURS;
                return View(co);
            }
        }

        // POST: TCambio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KURST,FCURR,TCURR,GDATU,UKURS")] TCAMBIO tCAMBIO)
        {
            //Codigo incrustado
            //tCAMBIO.KURST = "C";
            if (ModelState.IsValid)
            {

                db.Entry(tCAMBIO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tCAMBIO);
        }

        // GET: TCambio/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TCAMBIO tCAMBIO = db.TCAMBIOs.Find(id);
            if (tCAMBIO == null)
            {
                return HttpNotFound();
            }
            return View(tCAMBIO);
        }

        // POST: TCambio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TCAMBIO tCAMBIO = db.TCAMBIOs.Find(id);
            db.TCAMBIOs.Remove(tCAMBIO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var tc = db.TCAMBIOs.ToList();
            generarExcelHome(tc, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTC" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTC" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TCAMBIO> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                worksheet.Cell("A1").Value = new[]
             {
                  new {
                      BANNER = "FCURR"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
            {
                  new {
                      BANNER = "TCURR"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
            {
                  new {
                      BANNER = "GDATU"
                      },
                    };
                worksheet.Cell("D1").Value = new[]
           {
                  new {
                      BANNER = "UKURSS"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
               {
                  new {
                      BANNER       = lst[i-2].FCURR
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].TCURR
                      },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].GDATU
                        },
                      };
                    worksheet.Cell("D" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].UKURS
                        },
                    };
                }
                var rt = ruta + @"\DocTC" + DateTime.Now.ToShortDateString() + ".xlsx";
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
