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
    public class TxcController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Txc
        public ActionResult Index()
        {
            int pagina = 801; //ID EN BASE DE DATOS
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
            return View(db.TX_CONCEPTO.ToList());
        }

        // GET: Txc/Details/5
        public ActionResult Details(int? id)
        {
            int pagina = 802; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_CONCEPTO tX_CONCEPTO = db.TX_CONCEPTO.Find(id);
            if (tX_CONCEPTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tX_CONCEPTO);
        }

        // GET: Txc/Create
        public ActionResult Create()
        {
            int pagina = 804; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_CONCEPTO concepto = new TX_CONCEPTO();
            concepto.ACTIVO = true;
            return View(concepto);
        }

        // POST: Txc/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TX_CONCEPTO tX_CONCEPTO, string[] txval)
        {
            try
            {
                if (tX_CONCEPTO.DESCRIPCION != null)
                {
                    if (ModelState.IsValid)
                    {
                        db.TX_CONCEPTO.Add(tX_CONCEPTO);
                        db.SaveChanges();
                        //Recuperamos todas las descripciones en sus lenguajes
                        List<SPRA> ss = db.SPRAS.ToList();
                        var i = 0;
                        foreach (SPRA s in ss)
                        {
                            try
                            {
                                TX_CONCEPTOT txt = new TX_CONCEPTOT();
                                txt.SPRAS_ID = s.ID;
                                txt.TXT50 = txval[i];
                                txt.CONCEPTO_ID = tX_CONCEPTO.ID;
                                db.Entry(txt).State = EntityState.Added;
                                db.SaveChanges();
                                i++;
                            }
                            catch (Exception e)
                            {
                                var ex = e.ToString();
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.error = "Sin Texto";
                }

            }
            catch (Exception e)
            {
                var x = e.ToString();
            }
            int pagina = 804; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            return View(tX_CONCEPTO);

        }

        public ActionResult CreateA()
        {
            int pagina = 804; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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

        // POST: Txc/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CreateA([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TX_CONCEPTO tX_CONCEPTO)
        {
            try
            {
                if (tX_CONCEPTO.DESCRIPCION != null)
                {
                    if (ModelState.IsValid)
                    {
                        tX_CONCEPTO.ACTIVO = true;
                        db.TX_CONCEPTO.Add(tX_CONCEPTO);
                        db.SaveChanges();
                        //Posterior a la insercion del registro, insertar en treversat


                        List<SPRA> ss = db.SPRAS.ToList();
                        foreach (SPRA s in ss)
                        {
                            TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                            trvt.SPRAS_ID = s.ID;
                            trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt.TXT50 = tX_CONCEPTO.DESCRIPCION;
                            db.TX_CONCEPTOT.Add(trvt);
                            db.SaveChanges();
                        }

                        TempData["Mensaje"] = "Concepto creado correctamente.";
                        return Json("Concepto creado correctamente.", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ViewBag.error = "Sin Texto";
                }

            }
            catch (Exception e)
            {
                var x = e.ToString();
            }
            int pagina = 804; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            return Json("");

        }

        // GET: Txc/Edit/5
        public ActionResult Edit(int id)
        {
            int pagina = 803; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_CONCEPTO tX_CONCEPTO = db.TX_CONCEPTO.Find(id);
            if (tX_CONCEPTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.SPRAS = db.SPRAS.ToList();
            return View(tX_CONCEPTO);
        }

        // POST: Txc/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TX_CONCEPTO tX_CONCEPTO, string[] txval)
        {
            if (ModelState.IsValid)
            {
                //Recuperamos todas las descripciones en sus lenguajes
                List<SPRA> ss = db.SPRAS.ToList();
                foreach (SPRA s in ss)
                {
                    try
                    {
                        TX_CONCEPTOT txt = new TX_CONCEPTOT();
                        txt.SPRAS_ID = s.ID;
                        txt.TXT50 = Request.Form[s.ID].ToString();
                        txt.CONCEPTO_ID = tX_CONCEPTO.ID;
                        db.Entry(txt).State = EntityState.Modified;
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
                    List<TX_CONCEPTOT> lstc = db.TX_CONCEPTOT.Where(i => i.CONCEPTO_ID == tX_CONCEPTO.ID).ToList();
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
                                TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                                trvt.SPRAS_ID = "PT";
                                trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                                trvt.TXT50 = txval[0];
                                db.TX_CONCEPTOT.Add(trvt);
                                db.SaveChanges();
                            }
                            if (lstc[1].SPRAS_ID == "PT")
                            {  //Lleno el primer objeto
                                TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                                trvt.SPRAS_ID = "ES";
                                trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                                trvt.TXT50 = txval[0];
                                db.TX_CONCEPTOT.Add(trvt);
                                db.SaveChanges();
                            }
                        }
                        if (lstc[0].SPRAS_ID == "ES")
                        {
                            if (lstc[1].SPRAS_ID == "PT")
                            {
                                //Lleno el primer objeto
                                TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                                trvt.SPRAS_ID = "EN";
                                trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                                trvt.TXT50 = txval[0];
                                db.TX_CONCEPTOT.Add(trvt);
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
                            TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                            trvt.SPRAS_ID = "EN";
                            trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt.TXT50 = txval[0];
                            db.TX_CONCEPTOT.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TX_CONCEPTOT trvt2 = new TX_CONCEPTOT();
                            trvt2.SPRAS_ID = "PT";
                            trvt2.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt2.TXT50 = txval[1];
                            db.TX_CONCEPTOT.Add(trvt2);
                            db.SaveChanges();
                        }
                        else if (lstc[0].SPRAS_ID == "EN")
                        {
                            //Lleno el primer objeto
                            TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                            trvt.SPRAS_ID = "ES";
                            trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt.TXT50 = txval[0];
                            db.TX_CONCEPTOT.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TX_CONCEPTOT trvt2 = new TX_CONCEPTOT();
                            trvt2.SPRAS_ID = "PT";
                            trvt2.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt2.TXT50 = txval[1];
                            db.TX_CONCEPTOT.Add(trvt2);
                            db.SaveChanges();
                        }
                        else if (lstc[0].SPRAS_ID == "PT")
                        {
                            //Lleno el primer objeto
                            TX_CONCEPTOT trvt = new TX_CONCEPTOT();
                            trvt.SPRAS_ID = "ES";
                            trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt.TXT50 = txval[0];
                            db.TX_CONCEPTOT.Add(trvt);
                            db.SaveChanges();
                            //Lleno el segundo objeto
                            TX_CONCEPTOT trvt2 = new TX_CONCEPTOT();
                            trvt.SPRAS_ID = "EN";
                            trvt.CONCEPTO_ID = tX_CONCEPTO.ID;
                            trvt.TXT50 = txval[1];
                            db.TX_CONCEPTOT.Add(trvt);
                            db.SaveChanges();
                        }
                    }
                }
                db.Entry(tX_CONCEPTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tX_CONCEPTO);
        }


        // GET: Txc/Delete/5
        public ActionResult Delete(int id)
        {
            int pagina = 805; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(801) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            TX_CONCEPTO tX_CONCEPTO = db.TX_CONCEPTO.Find(id);
            ViewBag.SPRAS = db.SPRAS.ToList();
            if (tX_CONCEPTO == null)
            {
                return HttpNotFound();
            }
            return View(tX_CONCEPTO);
        }

        // POST: Txc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TX_CONCEPTO txc = db.TX_CONCEPTO.Where(x => x.ID == id).FirstOrDefault();
                txc.ACTIVO = false;
                db.Entry(txc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e) { var x = e.ToString(); }
            return View();
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var txc = db.TX_CONCEPTO.Where(a => a.ACTIVO == true).ToList();
            generarExcelHome(txc, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTxc" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTxc" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TX_CONCEPTO> lst, string ruta)
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
                var rt = ruta + @"\DocTxc" + DateTime.Now.ToShortDateString() + ".xlsx";
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
