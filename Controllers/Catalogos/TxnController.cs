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
    public class TxnController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Txn
        public ActionResult Index()
        {
            int pagina = 821; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
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
            return View(db.TX_TNOTA.ToList());
        }

        // GET: Txn/Details/5
        public ActionResult Details(int id)
        {
            int pagina = 822; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(821) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_TNOTA tX_TNOTA = db.TX_TNOTA.Find(id);
            if (tX_TNOTA == null)
            {
                return HttpNotFound();
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tX_TNOTA);
        }

        // GET: Txn/Create
        public ActionResult Create()
        {
            int pagina = 824; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(821) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            ViewBag.SPRAS = db.SPRAS.ToList();
            TX_TNOTA concepto = new TX_TNOTA();
            concepto.ACTIVO = true;
            return View(concepto);
        }

        // POST: Txn/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TX_TNOTA tX_TNOTA, string[] txval)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TX_TNOTA.Add(tX_TNOTA);
                    db.SaveChanges();
                    TX_TNOTA txn = db.TX_TNOTA.Where(x => x.DESCRIPCION == tX_TNOTA.DESCRIPCION).FirstOrDefault();
                    if (txn != null)
                    {
                        List<SPRA> ss = db.SPRAS.ToList();
                        var i = 0;
                        foreach (SPRA s in ss)
                        {
                            try
                            {
                                TX_NOTAT txt = new TX_NOTAT();
                                txt.SPRAS_ID = s.ID;
                                txt.TXT50 = txval[i];
                                txt.TNOTA_ID = txn.ID;
                                db.Entry(txt).State = EntityState.Added;
                                db.SaveChanges();
                                i++;
                            }
                            catch (Exception e)
                            {
                                var ex = e.ToString();
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }

            }
            catch (Exception e) { var a = e.ToString(); }
            return View(tX_TNOTA);
        }
        // GET: Txn/Create
        public ActionResult CreateA()
        {
            int pagina = 824; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(821) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            return View();
        }

        // POST: Txn/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CreateA([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TX_TNOTA tX_TNOTA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tX_TNOTA.ACTIVO = true;
                    db.TX_TNOTA.Add(tX_TNOTA);
                    db.SaveChanges();
                    ////TX_TNOTA txn = db.TX_TNOTA.Where(x => x.DESCRIPCION == tX_TNOTA.DESCRIPCION).FirstOrDefault();
                    ////if (txn != null)
                    ////{
                    List<SPRA> ss = db.SPRAS.ToList();
                    foreach (SPRA s in ss)
                    {
                        TX_NOTAT txt = new TX_NOTAT();
                        txt.SPRAS_ID = s.ID;
                        txt.TNOTA_ID = tX_TNOTA.ID;
                        txt.TXT50 = tX_TNOTA.DESCRIPCION;
                        db.TX_NOTAT.Add(txt);
                        db.SaveChanges();
                    }
                    ////}
                    TempData["Mensaje"] = "Tipo de contribuyente creado correctamente.";
                    return Json("Tipo de contribuyente creado correctamente.", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e) { var a = e.ToString(); }
            return View(tX_TNOTA);
        }
        // GET: Txn/Edit/5
        public ActionResult Edit(int id)
        {
            int pagina = 823; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(821) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_TNOTA tX_TNOTA = db.TX_TNOTA.Find(id);
            if (tX_TNOTA == null)
            {
                return HttpNotFound();
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tX_TNOTA);
        }

        // POST: Txn/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TX_TNOTA tX_TNOTA, string[] txval)
        {
            if (ModelState.IsValid)
            {
                //Recuperamos todas las descripciones en sus lenguajes
                List<SPRA> ss = db.SPRAS.ToList();
                foreach (SPRA s in ss)
                {
                    try
                    {
                        TX_NOTAT txnt = new TX_NOTAT();
                        txnt.SPRAS_ID = s.ID;
                        txnt.TNOTA_ID = tX_TNOTA.ID;
                        txnt.TXT50 = Request.Form[s.ID].ToString();
                        db.Entry(txnt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        var ex = e.ToString();
                    }
                }
                if (txval != null)
                {
                    //Posterior a lo ingresado
                    List<TX_NOTAT> lstc = db.TX_NOTAT.Where(i => i.TNOTA_ID == tX_TNOTA.ID).ToList();
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
                                TX_NOTAT trvt = new TX_NOTAT();
                                trvt.SPRAS_ID = "PT";
                                trvt.TNOTA_ID = tX_TNOTA.ID;
                                trvt.TXT50 = txval[0];
                                db.TX_NOTAT.Add(trvt);
                                db.SaveChanges();
                            }
                            if (lstc[1].SPRAS_ID == "PT")
                            {  //Lleno el primer objeto
                                TX_NOTAT trvt = new TX_NOTAT();
                                trvt.SPRAS_ID = "ES";
                                trvt.TNOTA_ID = tX_TNOTA.ID;
                                trvt.TXT50 = txval[0];
                                db.TX_NOTAT.Add(trvt);
                                db.SaveChanges();
                            }
                        }
                        if (lstc[0].SPRAS_ID == "ES")
                        {
                            if (lstc[1].SPRAS_ID == "PT")
                            {
                                //Lleno el primer objeto
                                TX_NOTAT trvt = new TX_NOTAT();
                                trvt.SPRAS_ID = "EN";
                                trvt.TNOTA_ID = tX_TNOTA.ID;
                                trvt.TXT50 = txval[0];
                                db.TX_NOTAT.Add(trvt);
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
                            TX_NOTAT trvt = new TX_NOTAT();
                            trvt.SPRAS_ID = "EN";
                            trvt.TNOTA_ID = tX_TNOTA.ID;
                            trvt.TXT50 = txval[0];
                            db.TX_NOTAT.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TX_NOTAT trvt2 = new TX_NOTAT();
                            trvt2.SPRAS_ID = "PT";
                            trvt2.TNOTA_ID = tX_TNOTA.ID;
                            trvt2.TXT50 = txval[1];
                            db.TX_NOTAT.Add(trvt2);
                            db.SaveChanges();
                        }
                        else if (lstc[0].SPRAS_ID == "EN")
                        {
                            //Lleno el primer objeto
                            TX_NOTAT trvt = new TX_NOTAT();
                            trvt.SPRAS_ID = "ES";
                            trvt.TNOTA_ID = tX_TNOTA.ID;
                            trvt.TXT50 = txval[0];
                            db.TX_NOTAT.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TX_NOTAT trvt2 = new TX_NOTAT();
                            trvt2.SPRAS_ID = "PT";
                            trvt2.TNOTA_ID = tX_TNOTA.ID;
                            trvt2.TXT50 = txval[1];
                            db.TX_NOTAT.Add(trvt2);
                            db.SaveChanges();
                        }
                        else if (lstc[0].SPRAS_ID == "PT")
                        {
                            //Lleno el primer objeto
                            TX_NOTAT trvt = new TX_NOTAT();
                            trvt.SPRAS_ID = "ES";
                            trvt.TNOTA_ID = tX_TNOTA.ID;
                            trvt.TXT50 = txval[0];
                            db.TX_NOTAT.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TX_NOTAT trvt2 = new TX_NOTAT();
                            trvt2.SPRAS_ID = "EN";
                            trvt2.TNOTA_ID = tX_TNOTA.ID;
                            trvt2.TXT50 = txval[1];
                            db.TX_NOTAT.Add(trvt2);
                            db.SaveChanges();
                        }
                    }
                }
                db.Entry(tX_TNOTA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tX_TNOTA);
        }

        // GET: Txn/Delete/5
        public ActionResult Delete(int id)
        {
            int pagina = 825; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(821) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_TNOTA tX_TNOTA = db.TX_TNOTA.Find(id);
            ViewBag.SPRAS = db.SPRAS.ToList();
            if (tX_TNOTA == null)
            {
                return HttpNotFound();
            }
            return View(tX_TNOTA);
        }

        // POST: Txn/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TX_TNOTA tX_TNOTA = db.TX_TNOTA.Where(x => x.ID == id).FirstOrDefault();
                //Lo Desactivamos
                tX_TNOTA.ACTIVO = false;
                db.Entry(tX_TNOTA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e) { var ex = e.ToString(); }
            int pagina = 825; //ID EN BASE DE DATOS
            USUARIO user = null;
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(821) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            return View();
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var TXN = db.TX_TNOTA.Where(a => a.ACTIVO == true).ToList();
            generarExcelHome(TXN, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTxn" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTxn" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TX_TNOTA> lst, string ruta)
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
                        BANNER = "DESCRIPCION"
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
                    worksheet.Cell("B" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].DESCRIPCION
                      },
                    };
                }
                var rt = ruta + @"\DocTxn" + DateTime.Now.ToShortDateString() + ".xlsx";
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
