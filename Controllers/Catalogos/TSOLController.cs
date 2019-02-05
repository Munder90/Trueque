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
using TAT001.Filters;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class TSOLController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: TSOL
        public ActionResult Index()
        {
            int pagina = 790; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            string u = User.Identity.Name;
            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();           
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

            var tSOLs = db.TSOLs.Include(t => t.RANGO).Include(t => t.TSOL2).Include(x => x.TSOLTs).ToList();
            return View(tSOLs);
        }

        // GET: TSOL/Details/5
        public ActionResult Details(string id)
        {
            int pagina = 793; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(791) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TSOL tSOL = db.TSOLs.Find(id);
            if (tSOL == null)
            {
                return HttpNotFound();
            }
            var rangos = db.RANGOes.Select(x => new { x.ID, DESCRIPCION = x.INICIO + "-" + x.FIN });
            ViewBag.RANGO_ID = new SelectList(rangos, "ID", "DESCRIPCION", tSOL.RANGO_ID);
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tSOL);
        }

        // GET: TSOL/Create
        public ActionResult Create()
        {   
            TSOL tsol = new TSOL();
            var rangos = db.RANGOes.Select(x => new { x.ID, DESCRIPCION = x.INICIO + "-" + x.FIN });
            ViewBag.RANGO_ID = new SelectList(rangos, "ID", "DESCRIPCION");
            int pagina_id = 791;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tsol);
        }

        // POST: TSOL/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DESCRIPCION,RANGO_ID,ESTATUS,FACTURA,PADRE,REVERSO,NEGO,CARTA,ADICIONA")] TSOL tSOL, FormCollection collection, string[] txval)
        {
            if (ModelState.IsValid)
            {
                if (!existeTSOL(tSOL.ID))
                {
                    if (Convert.ToBoolean(tSOL.ESTATUS))
                        tSOL.ESTATUS = "M";
                    else
                        tSOL.ESTATUS = "X";
                    var radio = collection["group1"];
                    if (radio == "FACTURA")
                        tSOL.FACTURA = true;
                    if (radio == "PADRE")
                        tSOL.PADRE = true;
                    if (radio == "REVERSO")
                        tSOL.REVERSO = true;
                    db.TSOLs.Add(tSOL);
                    db.SaveChanges();
                    List<SPRA> ss = db.SPRAS.ToList();
                    List<TSOLT> lstc = db.TSOLTs.Where(i => i.TSOL_ID == tSOL.ID).ToList();
                    if (lstc.Count == 0)
                    {
                        var j = 0;
                        foreach (SPRA s in ss)
                        {

                            try
                            {
                                TSOLT p = new TSOLT();
                                p.TSOL_ID = tSOL.ID;
                                p.SPRAS_ID = s.ID;
                                //p.TXT020 = txval[j].ToString();
                                p.TXT50 = txval[j].ToString();
                                db.Entry(p).State = EntityState.Added;
                                db.SaveChanges();
                                j++;
                            }
                            catch (Exception e)
                            {
                                var x = e.ToString();
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    int pagina_id = 791;//ID EN BASE DE DATOS
                    FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                    ViewBag.RANGO_ID = new SelectList(db.RANGOes, "ID", "ID", tSOL.RANGO_ID);
                    return View(tSOL);
                }
            }

            ViewBag.RANGO_ID = new SelectList(db.RANGOes, "ID", "ID", tSOL.RANGO_ID);
            return View(tSOL);
        }
        bool existeTSOL(string ID)
        {
            var reg=db.TSOLs.Where(t => t.ID == ID).SingleOrDefault();
            if (reg == null)
                return false;
            else
                return true;
        }
        // GET: TSOL/Edit/5
        public ActionResult Edit(string id)
        {
            int pagina = 792; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(791) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TSOL tSOL = db.TSOLs.Find(id);
            if (tSOL == null)
            {
                return HttpNotFound();
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            var rangos = db.RANGOes.Select(x => new { x.ID, DESCRIPCION = x.INICIO + "-" + x.FIN });
            ViewBag.RANGO_ID = new SelectList(rangos, "ID", "DESCRIPCION",tSOL.RANGO_ID);
            //ViewBag.RANGO_ID = new SelectList(db.RANGOes, "ID", "ID", tSOL.RANGO_ID);
            //ViewBag.TSOLR = new SelectList(db.TSOLs, "ID", "DESCRIPCION", tSOL.TSOLR);
            return View(tSOL);
        }

        // POST: TSOL/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ACTIVO,CARTA,NEGO,ADICIONA")] TSOL tSOL, FormCollection collection, string[] txval)
        {
            if (ModelState.IsValid)
            {
                List<SPRA> ss = db.SPRAS.ToList();
                List<TSOLT> lstc = db.TSOLTs.Where(i => i.TSOL_ID == tSOL.ID).ToList();
                var TSOL = db.TSOLs.Where(t => t.ID == tSOL.ID).SingleOrDefault();
                if (lstc.Count == 0)
                {
                    var j = 0;
                    foreach (SPRA s in ss)
                    {

                        try
                        {
                            TSOLT p = new TSOLT();
                            p.TSOL_ID = tSOL.ID;
                            p.SPRAS_ID = s.ID;
                            //p.TXT020 = txval[j].ToString();
                            p.TXT50 = txval[j].ToString();
                            db.Entry(p).State = EntityState.Added;
                            db.SaveChanges();
                            j++;
                        }
                        catch (Exception e)
                        {
                            var x = e.ToString();
                        }
                    }
                }
                else
                {
                    var j = 0;
                    foreach (var texto in lstc)
                    {
                        try
                        {
                            //texto.TXT020 = txval[j].ToString();
                            texto.TXT50 = txval[j].ToString();
                            db.Entry(texto).State = EntityState.Modified;
                            db.SaveChanges();
                            j++;
                        }
                        catch (Exception e)
                        {
                            var x = e.ToString();
                        }
                    }
                }
                
                TSOL.ACTIVO = tSOL.ACTIVO;
                TSOL.CARTA = tSOL.CARTA;
                TSOL.NEGO = tSOL.NEGO;
                TSOL.ADICIONA = tSOL.ADICIONA;
                db.SaveChanges();
                //if (txval != null)
                //{
                //    //Posterior a lo ingresado
                //    List<TSOLT> lstc = db.TSOLTs.Where(i => i.TSOL_ID == tSOL.ID).ToList();
                //    //si el arreglo solo incluye 1 dato, significa que ya hay 2 lenguajes
                //    if (txval.Length == 1)
                //    {
                //        var x1 = lstc[0].SPRAS_ID;
                //        var x2 = lstc[1].SPRAS_ID;
                //        if (lstc[0].SPRAS_ID == "EN")
                //        {
                //            if (lstc[1].SPRAS_ID == "ES")
                //            {
                //                // Lleno el primer objeto
                //                TSOLT trvt = new TSOLT();
                //                trvt.SPRAS_ID = "PT";
                //                trvt.TSOL_ID = tSOL.ID;
                //                trvt.TXT020 = txval[0];
                //                trvt.TXT50 = txval[0];
                //                db.TSOLTs.Add(trvt);
                //                db.SaveChanges();
                //            }
                //            if (lstc[1].SPRAS_ID == "PT")
                //            {  //Lleno el primer objeto
                //                TSOLT trvt = new TSOLT();
                //                trvt.SPRAS_ID = "ES";
                //                trvt.TSOL_ID = tSOL.ID;
                //                trvt.TXT020 = txval[0];
                //                trvt.TXT50 = txval[0];
                //                db.TSOLTs.Add(trvt);
                //                db.SaveChanges();
                //            }
                //        }
                //        if (lstc[0].SPRAS_ID == "ES")
                //        {
                //            if (lstc[1].SPRAS_ID == "PT")
                //            {
                //                // Lleno el primer objeto
                //                TSOLT trvt = new TSOLT();
                //                trvt.SPRAS_ID = "EN";
                //                trvt.TSOL_ID = tSOL.ID;
                //                trvt.TXT020 = txval[0];
                //                trvt.TXT50 = txval[0];
                //                db.TSOLTs.Add(trvt);
                //                db.SaveChanges();
                //            }
                //        }
                //    }
                //    //si el arreglo  incluye 2 datos, significa que ya hay 1 lenguaje
                //    else if (txval.Length == 2)
                //    {
                //        if (lstc[0].SPRAS_ID == "ES")
                //        {
                //            // Lleno el primer objeto
                //            TSOLT trvt = new TSOLT();
                //            trvt.SPRAS_ID = "EN";
                //            trvt.TSOL_ID = tSOL.ID;
                //            trvt.TXT020 = txval[0];
                //            trvt.TXT50 = txval[0];
                //            db.TSOLTs.Add(trvt);
                //            db.SaveChanges();
                //            //Lleno el segundo objeto
                //            TSOLT trvt2 = new TSOLT();
                //            trvt2.SPRAS_ID = "PT";
                //            trvt2.TSOL_ID = tSOL.ID;
                //            trvt2.TXT020 = txval[1];
                //            trvt2.TXT50 = txval[1];
                //            db.TSOLTs.Add(trvt2);
                //            db.SaveChanges();
                //        }
                //        else if (lstc[0].SPRAS_ID == "EN")
                //        {
                //            // Lleno el primer objeto
                //            TSOLT trvt = new TSOLT();
                //            trvt.SPRAS_ID = "ES";
                //            trvt.TSOL_ID = tSOL.ID;
                //            trvt.TXT020 = txval[0];
                //            trvt.TXT50 = txval[0];
                //            db.TSOLTs.Add(trvt);
                //            db.SaveChanges();
                //            //Lleno el segundo objeto
                //            TSOLT trvt2 = new TSOLT();
                //            trvt2.SPRAS_ID = "PT";
                //            trvt2.TSOL_ID = tSOL.ID;
                //            trvt2.TXT020 = txval[1];
                //            trvt2.TXT50 = txval[1];
                //            db.TSOLTs.Add(trvt2);
                //            db.SaveChanges();
                //        }
                //        else if (lstc[0].SPRAS_ID == "PT")
                //        {
                //            // Lleno el primer objeto
                //            TSOLT trvt = new TSOLT();
                //            trvt.SPRAS_ID = "ES";
                //            trvt.TSOL_ID = tSOL.ID;
                //            trvt.TXT020 = txval[0];
                //            trvt.TXT50 = txval[0];
                //            db.TSOLTs.Add(trvt);
                //            db.SaveChanges();
                //            //Lleno el segundo objeto
                //            TSOLT trvt2 = new TSOLT();
                //            trvt2.SPRAS_ID = "EN";
                //            trvt2.TSOL_ID = tSOL.ID;
                //            trvt2.TXT020 = txval[1];
                //            trvt2.TXT50 = txval[1];
                //            db.TSOLTs.Add(trvt2);
                //            db.SaveChanges();
                //        }
                //    }
                //    else if (txval.Length == 3)
                //    {
                //        if (lstc[0].SPRAS_ID == "ES")
                //        {
                //            // Lleno el primer objeto
                //            TSOLT trvt = new TSOLT();
                //            trvt.SPRAS_ID = "EN";
                //            trvt.TSOL_ID = tSOL.ID;
                //            trvt.TXT020 = txval[0];
                //            trvt.TXT50 = txval[0];
                //            db.TSOLTs.Add(trvt);
                //            db.SaveChanges();
                //            //Lleno el segundo objeto
                //            TSOLT trvt2 = new TSOLT();
                //            trvt2.SPRAS_ID = "PT";
                //            trvt2.TSOL_ID = tSOL.ID;
                //            trvt2.TXT020 = txval[1];
                //            trvt2.TXT50 = txval[1];
                //            db.TSOLTs.Add(trvt2);
                //            db.SaveChanges();
                //        }
                //        else if (lstc[0].SPRAS_ID == "EN")
                //        {
                //            // Lleno el primer objeto
                //            //TSOLT trvt = new TSOLT();
                //            //trvt.SPRAS_ID = "ES";
                //            //trvt.TSOL_ID = tSOL.ID;
                //            lstc[0].TXT020 = txval[0];
                //            lstc[0].TXT50 = txval[0];
                //            db.Entry(lstc[0]).State = EntityState.Modified;
                //            db.SaveChanges();
                //            //Lleno el segundo objeto
                //            TSOLT trvt2 = new TSOLT();
                //            trvt2.SPRAS_ID = "PT";
                //            trvt2.TSOL_ID = tSOL.ID;
                //            trvt2.TXT020 = txval[1];
                //            trvt2.TXT50 = txval[1];
                //            db.TSOLTs.Add(trvt2);
                //            db.SaveChanges();
                //        }
                //        else if (lstc[0].SPRAS_ID == "PT")
                //        {
                //            // Lleno el primer objeto
                //            TSOLT trvt = new TSOLT();
                //            trvt.SPRAS_ID = "ES";
                //            trvt.TSOL_ID = tSOL.ID;
                //            trvt.TXT020 = txval[0];
                //            trvt.TXT50 = txval[0];
                //            db.TSOLTs.Add(trvt);
                //            db.SaveChanges();
                //            //Lleno el segundo objeto
                //            TSOLT trvt2 = new TSOLT();
                //            trvt2.SPRAS_ID = "EN";
                //            trvt2.TSOL_ID = tSOL.ID;
                //            trvt2.TXT020 = txval[1];
                //            trvt2.TXT50 = txval[1];
                //            db.TSOLTs.Add(trvt2);
                //            db.SaveChanges();
                //        }
                //    }
                //}
                return RedirectToAction("Index");
            }
            int pagina = 793; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(790) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //ViewBag.pais = "mx.svg";
                    return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }
            ViewBag.RANGO_ID = new SelectList(db.RANGOes, "ID", "ID", tSOL.RANGO_ID);
            ViewBag.TSOLR = new SelectList(db.TSOLs, "ID", "DESCRIPCION", tSOL.TSOLR);
            return View(tSOL);
        }

        // GET: TSOL/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TSOL tSOL = db.TSOLs.Find(id);
            if (tSOL == null)
            {
                return HttpNotFound();
            }
            return View(tSOL);
        }

        // POST: TSOL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TSOL tSOL = db.TSOLs.Find(id);
            db.TSOLs.Remove(tSOL);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var tSOLs = db.TSOLs.Include(t => t.RANGO).Include(t => t.TSOL2).Include(x => x.TSOLTs).ToList();
            generarExcelHome(tSOLs, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTS" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTS" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TSOL> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                worksheet.Cell("A1").Value = new[]
             {
                  new {
                      BANNER = "ID"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
            {
                  new {
                      BANNER = "DESCRIPCIÓN"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
{
                  new {
                      BANNER = "INTERVALO"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
               {
                  new {
                      BANNER       = lst[i-2].ID
                      },
                    };
                    var tslt = "";
                    try
                    {
                        string u = User.Identity.Name;
                        USUARIO user = null;
                        user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                        tslt = lst[i - 2].TSOLTs.Where(x => x.SPRAS_ID == user.SPRAS_ID).FirstOrDefault().TXT50;
                    }
                    catch (Exception e)
                    {
                        tslt = "";
                    }
                    worksheet.Cell("B" + i).Value = new[]
                    {
                        new {
                            BANNER       = tslt
                        },
                    };
                    worksheet.Cell("C" + i).Value = new[]
{
                  new {
                      BANNER       = lst[i-2].RANGO.ID
                      },
                    };
                }
                var rt = ruta + @"\DocTS" + DateTime.Now.ToShortDateString() + ".xlsx";
                worksheet.Columns().AdjustToContents();
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
