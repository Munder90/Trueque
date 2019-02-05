using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Models;

namespace TAT001.Controllers.Catalogos
{
    public class ImpuestoController : Controller
    {
        // GET: Impuesto
        public ActionResult Index(string vko, string vtw, string kun, string spa, string mws)
        {
            int pagina = 641; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
                ViewBag.rol = user.MIEMBROS.FirstOrDefault().ROL.NOMBRE;
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
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                //return View(db.CLIENTEIs.Where(x => x.ACTIVO == true).ToList());
                var con = db.CLIENTEIs
                            .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ACTIVO == true)
                            .ToList();

                ViewBag.clienteBD = con;

                Impuesto imp = new Impuesto();
                imp.vkorg = vko;
                imp.vtweg = vtw;
                imp.kunnr = kun;
                imp.spart = spa;
                imp.listaBD = con;

                return View(imp);
            }
        }

        [HttpPost]
        public ActionResult Index(string vko, string vtw, string kun, string spa)
        {
            int pagina = 641; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;
                //ViewBag.rol = user.MIEMBROS.FirstOrDefault().ROL.NOMBRE;
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
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                var con = db.CLIENTEIs
                            .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ACTIVO == true)
                            .ToList();

                ViewBag.clienteBD = con;

                Impuesto imp = new Impuesto();
                imp.vkorg = vko;
                imp.vtweg = vtw;
                imp.kunnr = kun;
                imp.spart = spa;
                imp.listaBD = con;

                return View(imp);
            }
        }

        // GET: Impuesto/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Impuesto/Create
        public ActionResult Create(string vko, string vtw, string kun, string spa, string mws)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

                var con = db.CLIENTEIs
                            .Add(new CLIENTEI { VKORG = vko, VTWEG = vtw, KUNNR = kun, SPART = spa, MWSKZ = mws, ACTIVO = true });
                
                db.SaveChanges();

                return RedirectToAction("Index", new { vko = vko, vtw = vtw, kun = kun, spa = spa, mws = mws });
            }
        }

        // POST: Impuesto/Create
        [HttpPost]
        public ActionResult Create(string vko, string vtw, string kun, string spa)
        {
            int pagina = 641; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
                ViewBag.rol = user.MIEMBROS.FirstOrDefault().ROL.NOMBRE;
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
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                var conCli = db.CLIENTEIs
                            .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa)
                            .Select(x => x.MWSKZ)
                            .ToList();

                List<string> lista = new List<string>();
                lista = conCli;

                var shi = db.IMPUESTOes.Where(x => !lista.Contains(x.MWSKZ)).ToList();

                Impuesto imp = new Impuesto();
                imp.vkorg = vko;
                imp.vtweg = vtw;
                imp.kunnr = kun;
                imp.spart = spa;
                imp.listaBD2 = shi;
                return View(imp);
            }
        }

        // GET: Impuesto/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Impuesto/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Impuesto/Delete/5
        public ActionResult Delete(string vko, string vtw, string kun, string spa, string mws)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

                var con = db.CLIENTEIs.Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.MWSKZ == mws).First();
                con.ACTIVO = false;
                db.SaveChanges();

                return RedirectToAction("Index", new { vko = vko, vtw = vtw, kun = kun, spa = spa, mws = mws });
            }
        }

        // POST: Impuesto/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
