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

namespace TAT001.Controllers.Configuracion
{
    [Authorize]
    [LoginActive]
    public class TaxeopController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: Taxeop
        public ActionResult Index(string sc, string ld, string kun, string vk, string vtw, string sp, string con)
        {
            int pagina_id = 861; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
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

            //Para Regresar al Index
            ViewBag.sc = sc;
            ViewBag.ld = ld;
            ViewBag.kun = kun;
            ViewBag.vk = vk;
            ViewBag.vtw = vtw;
            ViewBag.sp = sp;
            ViewBag.con = con;
            var tAXEOPs = db.TAXEOPs.Include(t => t.RETENCION).Include(t => t.TAXEOH).Include(t => t.TRETENCION);
            CLIENTE cli = db.CLIENTEs.Find(vk, vtw, sp, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(tAXEOPs.Where(s => s.SOCIEDAD_ID == sc).ToList());
        }

        // GET: Taxeop/Details/5
        public ActionResult Details(string sc, string ld, string kun, string vk, string vtw, string sp, string con, string pos, string rid)
        {
            int pagina = 862; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(865)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(861) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            if (sc == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int c = Convert.ToInt32(con);
            int ps = Convert.ToInt32(pos);
            int r = Convert.ToInt32(rid);
            TAXEOP txp;
            if (rid != null)
                txp = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == sc && t.PAIS_ID == ld && t.KUNNR == kun && t.VKORG == vk && t.VTWEG == vtw && t.SPART == sp && t.CONCEPTO_ID == c && t.POS == ps && t.RETENCION_ID == r).FirstOrDefault();
            else
                txp = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == sc && t.PAIS_ID == ld && t.KUNNR == kun && t.VKORG == vk && t.VTWEG == vtw && t.SPART == sp && t.CONCEPTO_ID == c && t.POS == ps && t.RETENCION_ID == null).FirstOrDefault();
            ////http://localhost:64497/Taxeop/Details?sc=KLCO&ld=CO&kun=0000413500&vk=0155&vtw=50&sp=54&con=19&pos=1
            if (txp == null)
            {
                return HttpNotFound();
            }
            //Para Regresar al Index
            ViewBag.sc = sc;
            ViewBag.ld = ld;
            ViewBag.kun = kun;
            ViewBag.vk = vk;
            ViewBag.vtw = vtw;
            ViewBag.sp = sp;
            ViewBag.con = con;
            //
            var retenciones = db.RETENCIONs.Where(t => t.ACTIVO).Select(t => new { t.ID, DESCRIPCION = t.DESCRIPCIÓN + "-" + t.PORC + "%" });
            ViewBag.RETENCION_ID = new SelectList(retenciones, "ID", "DESCRIPCION", txp.RETENCION_ID);
            var tretenciones = db.TRETENCIONs.Where(t => t.ACTIVO == true).Select(t => new { t.ID, DESCRIPCION = t.ID + "-" + t.DESCRIPCION });
            ViewBag.TRETENCION_ID = new SelectList(tretenciones, "ID", "DESCRIPCION", txp.TRETENCION_ID);
            CLIENTE cli = db.CLIENTEs.Find(vk, vtw, sp, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(txp);
        }
        [HttpPost]
        public ActionResult CreateR([Bind(Include = "ID,DESCRIPCIÓN,PORC,ACTIVO")] RETENCION tX_CONCEPTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var retenciones = db.RETENCIONs.OrderByDescending(t => t.ID).First();
                    tX_CONCEPTO.ID = retenciones.ID + 1;
                    tX_CONCEPTO.ACTIVO = true;
                    db.RETENCIONs.Add(tX_CONCEPTO);
                    db.SaveChanges();
                    //Posterior a la insercion del registro, insertar en treversat
                    RETENCION trvi = db.RETENCIONs.Where(x => x.ID == tX_CONCEPTO.ID).FirstOrDefault();
                    //si trae registros entra
                    if (trvi != null)
                    {
                        List<SPRA> ss = db.SPRAS.ToList();
                        foreach (SPRA s in ss)
                        {
                            RETENCIONT trvt = new RETENCIONT();
                            trvt.SPRAS_ID = s.ID;
                            trvt.RETENCION_ID = trvi.ID;
                            trvt.TXT50 = tX_CONCEPTO.DESCRIPCIÓN;
                            db.RETENCIONTs.Add(trvt);
                            db.SaveChanges();
                        }
                    }
                    TempData["Mensaje"] = "Retención creado correctamente.";
                    return Json("Retención creada correctamente.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Modelo no valido", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                var x = e.ToString();
                return Json(e, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CreateTR([Bind(Include = "ID,DESCRIPCION,ACTIVO")] TRETENCION tX_CONCEPTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tX_CONCEPTO.ACTIVO = true;
                    db.TRETENCIONs.Add(tX_CONCEPTO);
                    db.SaveChanges();
                    //Posterior a la insercion del registro, insertar en treversat
                    TRETENCION trvi = db.TRETENCIONs.Where(x => x.ID == tX_CONCEPTO.ID).FirstOrDefault();
                    //si trae registros entra
                    if (trvi != null)
                    {
                        List<SPRA> ss = db.SPRAS.ToList();
                        foreach (SPRA s in ss)
                        {
                            TRETENCIONT trvt = new TRETENCIONT();
                            trvt.SPRAS_ID = s.ID;
                            trvt.TRETENCION_ID = trvi.ID;
                            trvt.TXT50 = tX_CONCEPTO.DESCRIPCION;
                            db.TRETENCIONTs.Add(trvt);
                            db.SaveChanges();
                        }
                    }
                    TempData["Mensaje"] = "Retención creada correctamente.";
                    return Json("Tipo Retención creada correctamente.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Modelo no valido", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                var x = e.ToString();
                return Json(e, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Taxeop/Create
        public ActionResult Create(string sc, string ld, string kun, string vk, string vtw, string sp, string con)
        {
            int pagina = 863; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(861) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            //Para Regresar al Index
            ViewBag.sc = sc;
            ViewBag.ld = ld;
            ViewBag.kun = kun;
            ViewBag.vk = vk;
            ViewBag.vtw = vtw;
            ViewBag.sp = sp;
            ViewBag.con = con;

            int c = Convert.ToInt32(con);
            var txp = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == sc && t.PAIS_ID == ld && t.KUNNR == kun && t.VKORG == vk && t.VTWEG == vtw && t.SPART == sp && t.CONCEPTO_ID == c).FirstOrDefault();
            if (txp == null)
            {
                txp = new TAXEOP();
                txp.SOCIEDAD_ID = sc;
                txp.PAIS_ID = ld;
                txp.KUNNR = kun;
                txp.VKORG = vk;
                txp.VTWEG = vtw;
                txp.SPART = sp;
                txp.CONCEPTO_ID = Convert.ToInt32(con);
            }
            txp.PORC = null;
            var retenciones = db.RETENCIONs.Where(t => t.ACTIVO).Select(t => new { t.ID, DESCRIPCION = t.DESCRIPCIÓN + "-" + t.PORC + "%" });
            ViewBag.RETENCION_ID = new SelectList(retenciones, "ID", "DESCRIPCION");
            var tretenciones = db.TRETENCIONs.Where(t => t.ACTIVO == true).Select(t => new { t.ID, DESCRIPCION = t.ID + "-" + t.DESCRIPCION });
            ViewBag.TRETENCION_ID = new SelectList(tretenciones, "ID", "DESCRIPCION");
            CLIENTE cli = db.CLIENTEs.Find(vk, vtw, sp, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(txp);
        }

        // POST: Taxeop/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SOCIEDAD_ID,PAIS_ID,VKORG,VTWEG,SPART,KUNNR,CONCEPTO_ID,POS,RETENCION_ID,PORC,ACTIVO,TRETENCION_ID")] TAXEOP tAXEOP, string sc, string ld, string kun, string vk, string vtw, string sp, string con)
        {
            if (ModelState.IsValid)
            {
                var pa = db.SOCIEDADs.Where(p => p.BUKRS == tAXEOP.SOCIEDAD_ID).Select(o => o.LAND).FirstOrDefault();
                var ps = db.TAXEOPs.ToList();
                var tx = tAXEOP;
                tx.PAIS_ID = pa;
                tx.POS = ps[ps.Count - 1].POS + 1;
                tx.ACTIVO = true;
                db.TAXEOPs.Add(tx);
                db.SaveChanges();
                return RedirectToAction("Index", new { sc = tAXEOP.SOCIEDAD_ID, ld = tAXEOP.PAIS_ID, kun = tAXEOP.KUNNR, vk = tAXEOP.VKORG, vtw = tAXEOP.VTWEG, sp = tAXEOP.SPART, con = tAXEOP.CONCEPTO_ID });
            }

            int pagina = 863; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(861) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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

            //Para Regresar al Index
            ViewBag.sc = sc;
            ViewBag.ld = ld;
            ViewBag.kun = kun;
            ViewBag.vk = vk;
            ViewBag.vtw = vtw;
            ViewBag.sp = sp;
            ViewBag.con = con;
            ViewBag.RETENCION_ID = new SelectList(db.RETENCIONs, "ID", "ID");
            ViewBag.TRETENCION_ID = new SelectList(db.TRETENCIONs, "ID", "ID");
            return View(tAXEOP);
        }

        // GET: Taxeop/Edit/5
        public ActionResult Edit(string sc, string ld, string kun, string vk, string vtw, string sp, string con, string pos, string rid)
        {
            int pagina = 862; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(861) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            if (sc == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int c = Convert.ToInt32(con);
            int ps = Convert.ToInt32(pos);
            int r = Convert.ToInt32(rid);

            TAXEOP txp;
            if (rid != null)
                txp = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == sc && t.PAIS_ID == ld && t.KUNNR == kun && t.VKORG == vk && t.VTWEG == vtw && t.SPART == sp && t.CONCEPTO_ID == c && t.POS == ps && t.RETENCION_ID == r).FirstOrDefault();
            else
                txp = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == sc && t.PAIS_ID == ld && t.KUNNR == kun && t.VKORG == vk && t.VTWEG == vtw && t.SPART == sp && t.CONCEPTO_ID == c && t.POS == ps && t.RETENCION_ID == null).FirstOrDefault();

            if (txp == null)
            {
                return HttpNotFound();
            }
            //Para Regresar al Index
            ViewBag.sc = sc;
            ViewBag.ld = ld;
            ViewBag.kun = kun;
            ViewBag.vk = vk;
            ViewBag.vtw = vtw;
            ViewBag.sp = sp;
            ViewBag.con = con;
            //
            var retenciones = db.RETENCIONs.Where(t => t.ACTIVO).Select(t => new { t.ID, DESCRIPCION = t.DESCRIPCIÓN + "-" + t.PORC + "%" });
            ViewBag.RETENCION_ID = new SelectList(retenciones, "ID", "DESCRIPCION", txp.RETENCION_ID);
            var tretenciones = db.TRETENCIONs.Where(t => t.ACTIVO == true).Select(t => new { t.ID, DESCRIPCION = t.ID + "-" + t.DESCRIPCION });
            ViewBag.TRETENCION_ID = new SelectList(tretenciones, "ID", "DESCRIPCION", txp.TRETENCION_ID);
            CLIENTE cli = db.CLIENTEs.Find(vk, vtw, sp, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(txp);
        }

        // POST: Taxeop/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOCIEDAD_ID,PAIS_ID,VKORG,VTWEG,SPART,KUNNR,CONCEPTO_ID,POS,RETENCION_ID,PORC,ACTIVO,TRETENCION_ID")] TAXEOP tAXEOP)
        {
            TAXEOP tx = db.TAXEOPs.Where(t => t.POS == tAXEOP.POS).FirstOrDefault();
            tx.RETENCION_ID = tAXEOP.RETENCION_ID;
            tx.TRETENCION_ID = tAXEOP.TRETENCION_ID;
            tx.PORC = tAXEOP.PORC;
            if (ModelState.IsValid)
            {
                db.Entry(tx).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { sc = tx.SOCIEDAD_ID, ld = tx.PAIS_ID, kun = tx.KUNNR, vk = tx.VKORG, vtw = tx.VTWEG, sp = tx.SPART, con = tx.CONCEPTO_ID });
            }
            ViewBag.RETENCION_ID = new SelectList(db.RETENCIONs, "ID", "DESCRIPCIÓN", tAXEOP.RETENCION_ID);
            ViewBag.SOCIEDAD_ID = new SelectList(db.TAXEOHs, "SOCIEDAD_ID", "USUARIOC_ID", tAXEOP.SOCIEDAD_ID);
            ViewBag.TRETENCION_ID = new SelectList(db.TRETENCIONs, "ID", "DESCRIPCION", tAXEOP.TRETENCION_ID);
            return View(tAXEOP);
        }

        // GET: Taxeop/Delete/5
        public ActionResult Delete(string sc, string ld, string kun, string vk, string vtw, string sp, string con, string pos, string rid)
        {
            int pagina = 864; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(861) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

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
            if (sc == null && ld == null && kun == null && vk == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int c = Convert.ToInt32(con);
            int ps = Convert.ToInt32(pos);
            int r = Convert.ToInt32(rid);
            var txp = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == sc && t.PAIS_ID == ld && t.KUNNR == kun && t.VKORG == vk && t.VTWEG == vtw && t.SPART == sp && t.CONCEPTO_ID == c && t.POS == ps && t.RETENCION_ID == r).FirstOrDefault();
            if (txp == null)
            {
                return HttpNotFound();
            }
            //Para Regresar al Index
            ViewBag.sc = sc;
            ViewBag.ld = ld;
            ViewBag.kun = kun;
            ViewBag.vk = vk;
            ViewBag.vtw = vtw;
            ViewBag.sp = sp;
            ViewBag.con = con;
            return View(txp);
        }

        // POST: Taxeop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(TAXEOP tp)
        {
            TAXEOP tx = db.TAXEOPs.Where(t => t.SOCIEDAD_ID == tp.SOCIEDAD_ID && t.PAIS_ID == tp.PAIS_ID && t.KUNNR == tp.KUNNR && t.VKORG == tp.VKORG && t.VTWEG == tp.VTWEG && t.SPART == tp.SPART && t.CONCEPTO_ID == tp.CONCEPTO_ID && t.POS == tp.POS && t.RETENCION_ID == tp.RETENCION_ID).FirstOrDefault();
            tx.ACTIVO = false;
            db.Entry(tx).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { sc = tx.SOCIEDAD_ID, ld = tx.PAIS_ID, kun = tx.KUNNR, vk = tx.VKORG, vtw = tx.VTWEG, sp = tx.SPART, con = tx.CONCEPTO_ID });
        }

        [HttpPost]
        public FileResult Descargar(string sc, string ld, string kun, string vk, string vtw, string sp, string con)
        {
            var tAXEOPs = db.TAXEOPs.Include(t => t.RETENCION).Include(t => t.TAXEOH).Include(t => t.TRETENCION);
            var pr = tAXEOPs.Where(s => s.SOCIEDAD_ID == sc).ToList();
            generarExcelHome(pr, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTxp" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTxp" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TAXEOP> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
             {
                  new {
                      BANNER = "CO. CODE"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
            {
                  new {
                      BANNER = "PAIS"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
            {
                  new {
                      BANNER = "USUARIO CREADOR"
                      },
                    };
                worksheet.Cell("D1").Value = new[]
           {
                  new {
                      BANNER = "VKORG"
                      },
                    };
                worksheet.Cell("E1").Value = new[]
            {
                  new {
                      BANNER = "VTWEG"
                      },
                    };
                worksheet.Cell("F1").Value = new[]
            {
                  new {
                      BANNER = "SPART"
                      },
                    };
                worksheet.Cell("G1").Value = new[]
            {
                  new {
                      BANNER = "KUNNR"
                      },
                    };
                worksheet.Cell("H1").Value = new[]
            {
                  new {
                      BANNER = "ID CONCEPTO"
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
                      BANNER       = lst[i-2].PAIS_ID
                      },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].TAXEOH.USUARIOC_ID
                        },
                      };
                    worksheet.Cell("D" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].VKORG
                        },
                      };
                    worksheet.Cell("E" + i).Value = new[]
             {
                  new {
                      BANNER       = lst[i-2].VTWEG
                      },
                    };
                    worksheet.Cell("F" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].SPART
                        },
                      };
                    worksheet.Cell("G" + i).Value = new[]
                    {
                    new {
                        BANNER       = lst[i-2].KUNNR
                        },
                      };
                    worksheet.Cell("h" + i).Value = new[]
                  {
                    new {
                        BANNER       = lst[i-2].CONCEPTO_ID
                        },
                      };
                }
                var rt = ruta + @"\DocTxp" + DateTime.Now.ToShortDateString() + ".xlsx";
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
