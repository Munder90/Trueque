using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class PresupuestoController : Controller
    {
        USUARIO user = new USUARIO();
        public ActionResult Index()
        {
            int pagina = 301; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                ViewBag.usrScts = user.SOCIEDADs.ToList();
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
            Session["sociedad"] = null;
            Session["cambio"] = null;
            Session["periodoconsu"] = null;
            Session["anioconsu"] = null;
            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();
            ViewBag.anio = "20" + carga.consultaAnio();
            DatosPresupuesto sociedades = new DatosPresupuesto();
            carga.consultSociedad(ref sociedades, "", User.Identity.Name);
            return View(sociedades);
        }
        [HttpPost]
        public ActionResult Index(string cpt, string excel, string[] select, string anioconsu, string periodoconsu, string cambio)
        {
            //try
            //{
            //    if (Session["sociedad"].ToString() != select)
            //    {
            //        cambio = null;
            //        periodoconsu = null;
            //        anioconsu = null;
            //    }
            //    if (periodoconsu == "")
            //    {
            //        periodoconsu = null;
            //    }
            //}
            //catch (Exception)
            //{
            //}
            
            Session["sociedad"] = select;
            Session["cambio"] = cambio;
            Session["periodoconsu"] = periodoconsu;
            Session["anioconsu"] = anioconsu;
            int pagina = 301; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery;;
                ViewBag.usrScts = user.SOCIEDADs.ToList();
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
            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            DatosPresupuesto presu = new DatosPresupuesto();
            ViewBag.ultMod = carga.consultarUCarga();
            ViewBag.anio = "20" + carga.consultaAnio();
            ViewBag.chkcpt = cpt;
            presu = carga.consultarDatos(select, anioconsu, periodoconsu, cambio, cpt, excel, Server.MapPath("~/pdfTemp/"), User.Identity.Name);
            if (excel != null)
            {
                return File(Server.MapPath("~/pdfTemp/Presupuesto.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Presupuesto.xlsx");
            }
            else
            {
                return View(presu);
            }
            ;
        }
        public ActionResult Carga()
        {
            Models.CargarModel carga = new Models.CargarModel();
            int pagina = 302; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                ViewBag.usrScts = user.SOCIEDADs.ToList();
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
            Session["Sociedadcpt"] = null;
            Session["Periodocpt"] = null;
            return View(carga.consultSociedad(User.Identity.Name));
        }
        [HttpPost]
        public ActionResult Carga(string enviar, string guardar, HttpPostedFileBase fileCPT, HttpPostedFileBase[] fileSAP, string[] sociedadsap, string[] periodocpt, string[] sociedadcpt, string[] periodosap, string[] aniocpt, string[] aniosap, string opciong)
        {
            int pagina = 302; //ID EN BASE DE DATOS
            DatosPresupuesto pRESUPUESTOP = new DatosPresupuesto();
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                //ViewBag.usrScts = user.SOCIEDADs.ToList();
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
            Models.CargarModel carga = new Models.CargarModel();
            pRESUPUESTOP = carga.consultSociedad(User.Identity.Name);
            string nombref = "", nombre = "";
            try
            {
                if (String.IsNullOrEmpty(enviar) == false || String.IsNullOrEmpty(guardar) == false)
                {
                    if (fileCPT != null || fileSAP != null)
                    {
                        string mensajeC = "", mensajeS = "";
                        try
                        {
                            if (fileCPT != null)
                            {
                                nombref += "CPT_BUDGET_" + DateTime.Today.Year.ToString().Substring(2, 2);
                                nombre = fileCPT.FileName.Remove(11, 4).Substring(0, 13);
                                if (nombre == nombref)
                                {
                                    pRESUPUESTOP.presupuestoCPT = carga.cargarPresupuestoCPT(fileCPT, sociedadcpt, periodocpt, aniocpt, ref mensajeC, user.SPRAS_ID, pagina);
                                    Session["Presupuesto"] = pRESUPUESTOP;
                                    Session["Sociedadcpt"] = sociedadcpt;
                                    Session["Aniocpt"] = aniocpt;
                                    Session["Periodocpt"] = periodocpt;
                                    ViewBag.sociedadcpt = 1;
                                }
                                else
                                {
                                    ViewBag.MensajeGE = carga.mensajes(16, user.SPRAS_ID, pagina);
                                }
                            }
                            if (fileSAP[0] != null)
                            {
                                nombref += "OUTBOUND_BUDG_" + DateTime.Today.Year.ToString() + string.Format("{0:000}", periodosap[0].PadLeft(3, '0'));
                                nombre = fileSAP[0].FileName.Remove(14, 5).Remove(16, 2).Substring(0, 18); nombre = fileSAP[0].FileName.Remove(14, 5).Substring(0, 21);
                                if (nombre == nombref)
                                {
                                    pRESUPUESTOP.presupuestoSAP = carga.cargarPresupuestoSAP(fileSAP, sociedadsap, periodosap, aniosap, ref mensajeS, user.SPRAS_ID, pagina);
                                    Session["Presupuesto"] = pRESUPUESTOP;
                                    Session["Sociedadsap"] = sociedadsap;
                                    Session["Aniosap"] = aniosap;
                                    Session["Periodosap"] = periodosap;
                                    ViewBag.sociedadsap = 1;
                                }
                                else
                                {
                                    ViewBag.MensajeGE = carga.mensajes(16, user.SPRAS_ID, pagina);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ViewBag.MensajeGE = carga.mensajes(1, user.SPRAS_ID, pagina);//"Error en la carga de archivo CPT y/o SAP";
                                                                                 //ViewBag.MensajeG = e.Message;
                        }
                        ViewBag.MensajeC = mensajeC;
                        ViewBag.MensajeS = mensajeS;
                        return View(pRESUPUESTOP);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(guardar) == false)
                        {
                            try
                            {
                                pRESUPUESTOP = Session["Presupuesto"] as DatosPresupuesto;
                                if (pRESUPUESTOP.presupuestoCPT.Count > 0 || pRESUPUESTOP.presupuestoSAP.Count > 0)
                                {
                                    ViewBag.MensajeC = carga.guardarPresupuesto(ref pRESUPUESTOP, Session["Sociedadcpt"] as string[], Session["Periodocpt"] as string[], Session["Sociedadsap"] as string[], Session["Periodosap"] as string[], User.Identity.Name, opciong, user.SPRAS_ID, pagina);
                                    if (pRESUPUESTOP.bannerscanal.Count > 0)
                                    {
                                        ViewBag.MensajeGI = carga.mensajes(3, user.SPRAS_ID, pagina);//"Se encontraron banners sin canal asignados";
                                    }
                                }
                                else
                                {
                                    ViewBag.MensajeGE = carga.mensajes(6, user.SPRAS_ID, pagina);//"Ocurrio algo intente de nuevo cargar el/los archivo/s";
                                }
                            }
                            catch (Exception e)
                            {
                                ViewBag.MensajeGE = carga.mensajes(6, user.SPRAS_ID, pagina);//"Ocurrio algo, intenté de nuevo cargar el/los archivo/s";
                                                                                     //ViewBag.MensajeG = e.InnerException.Message;
                            }
                        }
                        else
                        {
                            ViewBag.MensajeGE = carga.mensajes(2, user.SPRAS_ID, pagina);//"Cargue algún archivo";
                        }
                        Session["Presupuesto"] = null;
                        Session["Sociedadsap"] = null;
                        Session["Periodosap"] = null;
                        //Session["Sociedadcpt"] = null;
                        Session["Periodocpt"] = null;
                        Session["Aniocpt"] = null;
                        Session["Aniosap"] = null;
                        pRESUPUESTOP.presupuestoCPT = new List<PRESUPUESTOP>();
                        pRESUPUESTOP.presupuestoSAP = new List<PRESUPSAPP>();
                        return View(pRESUPUESTOP);
                    }
                }
                else
                {
                    Session["Presupuesto"] = null;
                    Session["Sociedadsap"] = null;
                    Session["Periodosap"] = null;
                    Session["Sociedadcpt"] = null;
                    Session["Periodocpt"] = null;
                    Session["Aniocpt"] = null;
                    Session["Aniosap"] = null;
                    pRESUPUESTOP.presupuestoCPT = new List<PRESUPUESTOP>();
                    pRESUPUESTOP.presupuestoSAP = new List<PRESUPSAPP>();
                    ViewBag.MensajeGI = carga.mensajes(4, user.SPRAS_ID, pagina);//"Carga cancelada";
                    return View(pRESUPUESTOP);
                }
            }
            catch (Exception)
            {
                ViewBag.MensajeGI = carga.mensajes(1, user.SPRAS_ID, pagina);
                return View(pRESUPUESTOP);
            }
            
        }
        [HttpPost]
        public FileResult Descargar(string excel)
        {
            Models.CargarModel carga = new Models.CargarModel();
            string[] defaul = Session["Sociedadcpt"] as string[];
            carga.bannres(Server.MapPath("~/pdfTemp/"), defaul, user.SPRAS_ID, 302);
            return File(Server.MapPath("~/pdfTemp/Banners sin canal.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Banners sin canal.xlsx");
        }
    }
}