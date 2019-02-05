using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TAT001.Entities;
using TAT001.Models;
using ClosedXML.Excel;
using TAT001.Services;
using System.Web.Script.Serialization;
using static TAT001.Models.ReportesModel;
using TAT001.Filters;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class HomeController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Solicitudes
        //[Authorize]
        public ActionResult Index(string id)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                int pagina = 101; //ID EN BASE DE DATOS
                string u = User.Identity.Name;
                ////if (pais != null)
                ////    Session["pais"] = pais;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user;
                ViewBag.returnUrl = Request.Url.PathAndQuery;
                ViewBag.nivelUsuario = user.PUESTO_ID;
                ViewBag.sociedadesUsuario = user.SOCIEDADs;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                List<Documento> listaDocsc = new List<Documento>();
                if (user.PUESTO_ID == 14)
                {
                    pagina = 1101;
                    //}
                    foreach (var cocode in user.SOCIEDADs)
                    {
                        List<CSP_DOCUMENTOSXCOCODE_Result> dOCUMENTOesc = db.CSP_DOCUMENTOSXCOCODE(cocode.BUKRS, user.SPRAS_ID).ToList();
                        ViewBag.imgnoticia = db.NOTICIAs.Where(x => x.FECHAI <= DateTime.Now && x.FECHAF >= DateTime.Now && x.ACTIVO == true).Select(x => x.PATH).FirstOrDefault();
                        List<ESTATU> eec = db.ESTATUS.Where(x => x.ACTIVO == true).ToList();

                        
                        foreach (CSP_DOCUMENTOSXCOCODE_Result item in dOCUMENTOesc)
                        {
                            Documento ld = new Documento();
                            ld.BUTTON = item.BUTTON;

                            ld.NUM_DOC = item.NUM_DOC;
                            ld.NUM_DOC_TEXT = item.NUM_DOC_TEXT;
                            ld.SOCIEDAD_ID = item.SOCIEDAD_ID;
                            ld.PAIS_ID = item.PAIS_ID;
                            ld.FECHADD = item.FECHAD.Value.Day + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Year;
                            ld.FECHAD = item.FECHAD.Value.Year + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Day;
                            ld.HORAC = item.HORAC.Value.ToString().Split('.')[0];
                            ld.PERIODO = item.PERIODO + "";

                            if (item.ESTATUS == "R")
                            {
                                FLUJO flujo = db.FLUJOes.Include("USUARIO").Where(x => x.NUM_DOC == item.NUM_DOC & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault();
                                item.ESTATUSS = item.ESTATUSS.Substring(0, 6) +
                                                (flujo.USUARIO != null ? flujo.USUARIO.PUESTO_ID.ToString() : "") +
                                                item.ESTATUSS.Substring(6, 2);
                            }
                            else
                            {
                                item.ESTATUSS = item.ESTATUSS.Substring(0, 6) + " " + item.ESTATUSS.Substring(6, 2); ;
                            }
                            Estatus e = new Estatus();
                            ld.ESTATUS = e.getText(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID, eec);
                            ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID, eec);
                            //ld.ESTATUS = e.getText(item.ESTATUSS);
                            //ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS);

                            ld.PAYER_ID = item.PAYER_ID;

                            ld.CLIENTE = item.NAME1;
                            ld.CANAL = item.CANAL;
                            ld.TSOL = item.TXT020;
                            ld.TALL = item.TXT50;
                            //foreach (CUENTA cuenta in db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(item.SOCIEDAD_ID)).ToList())
                            //{
                            //    if (item.TALL != null)
                            //    {
                            //        if (cuenta.TALL_ID == item.TALL.ID)
                            //        {
                            //            ld.CUENTAS = cuenta.CARGO.ToString();
                            //            break;
                            //        }
                            //    }
                            //}
                            try
                            {
                                ld.CUENTAS = item.CARGO + "";
                                ld.CUENTAP = item.CUENTAP;
                                ld.CUENTAPL = item.CUENTAPL;
                                ld.CUENTACL = item.CUENTACL;
                            }
                            catch { }
                            ld.CONCEPTO = item.CONCEPTO;
                            ld.MONTO_DOC_ML = item.MONTO_DOC_MD != null ? Convert.ToDecimal(item.MONTO_DOC_MD).ToString("C") : "";

                            ld.FACTURA = item.FACTURA;
                            ld.FACTURAK = item.FACTURAK;

                            ld.USUARIOC_ID = item.USUARIOD_ID;
                            ld.USUARIOM_ID = item.USUARIOD_ID;

                            if (item.DOCUMENTO_SAP != null)
                            {
                                if (item.PADRE)
                                {
                                    ld.NUM_PRO = item.DOCUMENTO_SAP;
                                    ld.NUM_AP = "";
                                    ld.NUM_NC = "";
                                    ld.NUM_REV = "";
                                }
                                else if (item.REVERSO)
                                {
                                    ld.NUM_REV = item.DOCUMENTO_SAP;
                                    ld.NUM_AP = "";
                                    ld.NUM_NC = "";
                                    ld.NUM_PRO = "";
                                }
                                else
                                {
                                    ld.NUM_NC = item.DOCUMENTO_SAP;
                                    ld.NUM_AP = "";
                                    ld.NUM_PRO = "";
                                    ld.NUM_REV = "";
                                }
                                //<!--NUM_SAP-->
                                ld.BLART = item.BLART;
                                ld.NUM_PAYER = item.KUNNR;
                                ld.NUM_CLIENTE = item.DESCR;
                                ld.NUM_IMPORTE = item.IMPORTE != null ? Convert.ToDecimal(item.IMPORTE).ToString("C") : "";
                                ld.NUM_CUENTA = item.CUENTA_C;
                            }
                            else
                            {
                                //<td></td>
                                //<td></td>
                                //<td></td>
                                //<td></td>
                                //<td></td>
                                //<td></td>
                                //<td></td>
                                //<td></td>
                                //<td></td>
                            }

                            listaDocsc.Add(ld);
                        }
                    }

                }
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                    PAI pa = db.PAIS.Find(p);
                    if (pa != null)
                    {
                        ViewBag.miles = pa.MILES;//LEJGG 090718
                        ViewBag.dec = pa.DECIMAL;//LEJGG 090718
                    }
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    ////return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                try//Mensaje de documento creado
                {
                    string p = Session["NUM_DOC"].ToString();
                    ViewBag.NUM_DOC = p;
                    Session["NUM_DOC"] = null;
                }
                catch
                {
                    ViewBag.NUM_DOC = "";
                }

                try//Mensaje de documento creado
                {
                    string error_files = Session["ERROR_FILES"].ToString();
                    ViewBag.ERROR_FILES = error_files;
                    Session["ERROR_FILES"] = null;
                }
                catch
                {
                    ViewBag.ERROR_FILES = "";
                }


                string us = "";
                DateTime fecha = DateTime.Now.Date;
                List<TAT001.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).ToList();

                if (del.Count > 0)
                {
                    List<USUARIO> users = new List<USUARIO>();
                    foreach (DELEGAR de in del)
                    {
                        users.Add(de.USUARIO);
                    }
                    users.Add(ViewBag.usuario);
                    ViewBag.delegados = users.ToList();

                    if (id != null)
                        us = id;
                    else
                        us = User.Identity.Name;
                    ViewBag.usuariod = us;
                }
                else
                    us = User.Identity.Name;

                List<CSP_DOCUMENTOSXUSER2_Result> dOCUMENTOes = db.CSP_DOCUMENTOSXUSER2(us, user.SPRAS_ID).ToList();

                //dOCUMENTOes = dOCUMENTOes.Distinct(new DocumentoComparer()).ToList();
                //dOCUMENTOes = dOCUMENTOes.OrderByDescending(a => a.FECHAC).OrderByDescending(a => a.NUM_DOC).ToList();
                //ViewBag.Clientes = db.CLIENTEs.ToList();
                //ViewBag.Cuentas = db.CUENTAs.ToList();
                //ViewBag.DOCF = db.DOCUMENTOFs.ToList();
                //jemo inicio 4/07/2018
                ViewBag.imgnoticia = db.NOTICIAs.Where(x => x.FECHAI <= DateTime.Now && x.FECHAF >= DateTime.Now && x.ACTIVO == true).Select(x => x.PATH).FirstOrDefault();
                List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO == true).ToList();

                List<Documento> listaDocs = new List<Documento>();
                foreach (CSP_DOCUMENTOSXUSER2_Result item in dOCUMENTOes)
                {
                    Documento ld = new Documento();
                    ld.BUTTON = item.BUTTON;
                    if (ld.BUTTON == "add")
                    {
                        if (item.nRelacionadas == 0)
                            ld.BUTTON = "disabled";
                    }
                    else if (ld.BUTTON == "expand_more")
                    {
                        if (item.nRecurrentes == 0)
                            ld.BUTTON += " disabled";
                    }
                    if (item.nRecurrentes > 0)
                        ld.BUTTON = "expand_more";
                    ld.NUM_DOC = item.NUM_DOC;
                    ld.NUM_DOC_TEXT = item.NUM_DOC_TEXT;
                    ld.SOCIEDAD_ID = item.SOCIEDAD_ID;
                    ld.DOCUMENTO_REF = item.DOCUMENTO_REF;
                    ld.PAIS_ID = item.PAIS_ID;
                    ld.FECHADD = item.FECHAD.Value.Day + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Year;
                    ld.FECHAD = item.FECHAD.Value.Year + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Day;
                    ld.HORAC = item.HORAC.Value.ToString().Split('.')[0];
                    ld.PERIODO = item.PERIODO + "";

                    if (item.ESTATUS == "R")
                    {
                        FLUJO flujo = db.FLUJOes.Include("USUARIO").Where(x => x.NUM_DOC == item.NUM_DOC & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault();
                        item.ESTATUSS = item.ESTATUSS.Substring(0, 6) +
                                        (flujo.USUARIO != null ? flujo.USUARIO.PUESTO_ID.ToString() : "") +
                                        item.ESTATUSS.Substring(6, 2);
                    }
                    else
                    {
                        item.ESTATUSS = item.ESTATUSS.Substring(0, 6) + " " + item.ESTATUSS.Substring(6, 2); ;
                    }
                    Estatus e = new Estatus();
                    ld.ESTATUS = e.getText(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID, ee);
                    ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID, ee);
                    //ld.ESTATUS_WF = item.ESTATUS!="R" && item.ESTATUS!="S"?item.ESTATUS:"";
                    ld.ESTATUS_WF = item.ESTATUS_WF_USER != "R" ? item.ESTATUS_WF_USER  : "";
                    if (ld.ESTATUS_WF == "P" && (item.USUARIOA_ID != us))
                        ld.ESTATUS_WF = "";
                    //ld.ESTATUS = e.getText(item.ESTATUSS);
                    //ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS);

                    ld.PAYER_ID = item.PAYER_ID;

                    ld.CLIENTE = item.NAME1;
                    ld.CANAL = item.CANAL;
                    ld.TSOL = item.TXT020;
                    ld.TALL = item.TXT50;
                    //foreach (CUENTA cuenta in db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(item.SOCIEDAD_ID)).ToList())
                    //{
                    //    if (item.TALL != null)
                    //    {
                    //        if (cuenta.TALL_ID == item.TALL.ID)
                    //        {
                    //            ld.CUENTAS = cuenta.CARGO.ToString();
                    //            break;
                    //        }
                    //    }
                    //}
                    try
                    {
                        ld.CUENTAS = item.CARGO + "";
                        ld.CUENTAP = item.CUENTAP;
                        ld.CUENTAPL = item.CUENTAPL;
                        ld.CUENTACL = item.CUENTACL;
                    }
                    catch { }
                    ld.CONCEPTO = item.CONCEPTO;
                    ld.MONTO_DOC_ML = item.MONTO_DOC_MD != null ? Convert.ToDecimal(item.MONTO_DOC_MD).ToString("C") : "";

                    ld.FACTURA = item.FACTURA;
                    ld.FACTURAK = item.FACTURAK;

                    ld.USUARIOC_ID = item.USUARIOD_ID;
                    ld.USUARIOM_ID = item.USUARIOD_ID;

                    if (item.DOCUMENTO_SAP != null)
                    {
                        if (item.PADRE)
                        {
                            ld.NUM_PRO = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_NC = "";
                            ld.NUM_REV = "";
                        }
                        else if (item.REVERSO)
                        {
                            ld.NUM_REV = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_NC = "";
                            ld.NUM_PRO = "";
                        }
                        else
                        {
                            ld.NUM_NC = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_PRO = "";
                            ld.NUM_REV = "";
                        }
                        //<!--NUM_SAP-->
                        ld.BLART = item.BLART;
                        ld.NUM_PAYER = item.KUNNR;
                        ld.NUM_CLIENTE = item.DESCR;
                        ld.NUM_IMPORTE = item.IMPORTE != null ? Convert.ToDecimal(item.IMPORTE).ToString("C") : "";
                        ld.NUM_CUENTA = item.CUENTA_C;
                    }
                    else
                    {
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                    }

                    listaDocs.Add(ld);
                }
                ////List<DOCUMENTO> dss = db.DOCUMENTOes.Where(x => x.DOCUMENTO_SAP != null & x.DOCUMENTOSAP == null).ToList();
                ////foreach(DOCUMENTO ds in dss)
                ////{
                ////    DOCUMENTOSAP dos = new DOCUMENTOSAP();
                ////    dos.NUM_DOC = ds.NUM_DOC;
                ////    dos.BLART = "SA";
                ////    dos.BUKRS = ds.SOCIEDAD_ID;
                ////    dos.CUENTA_A = "0000000001";
                ////    dos.EJERCICIO = 2018;
                ////    dos.FECHAC = DateTime.Now;
                ////    dos.IMPORTE = 10000;
                ////    db.DOCUMENTOSAPs.Add(dos);
                ////}
                ////db.SaveChanges();
                

                if (user.PUESTO_ID == 14) {
                    return  View(listaDocsc);
                }
                else
                {
                    return View(listaDocs);
                }
                    

            }
        }
        [HttpGet]
        public ActionResult SelPais(string pais, string returnUrl, string sociedad_id)
        {
            Session["pais"] = pais.ToUpper();
            Session["sociedad_id"] = sociedad_id;
            if (!string.IsNullOrEmpty(returnUrl))
            {

                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult Pais(string returnUrl)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                int pagina = 102; //ID EN BASE DE DATOS
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                //ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.flag = true;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();


                var p = (from P in db.PAIS.ToList()
                         join C in db.CLIENTEs.Where(x => x.ACTIVO == true).ToList()
                         on P.LAND equals C.LAND
                         join U in db.USUARIOFs.Where(x => x.USUARIO_ID == u & x.ACTIVO == true)
                         on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                         where P.ACTIVO == true
                         select P).DistinctBy(x => x.LAND);

                List<Delegados> delegados = new List<Delegados>();
                DateTime fecha = DateTime.Now.Date;
                List<TAT001.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).ToList();
                //List<TAT001.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(u)).ToList();
                foreach (DELEGAR de in del)
                {

                    var pd = (from P in db.PAIS.ToList()
                              join C in db.CLIENTEs.Where(x => x.ACTIVO == true).ToList()
                              on P.LAND equals C.LAND
                              join U in db.USUARIOFs.Where(x => x.USUARIO_ID == de.USUARIO_ID & x.ACTIVO == true)
                              on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                              where P.ACTIVO == true
                              select P).DistinctBy(x => x.LAND).ToList();

                    Delegados delegado = new Delegados();
                    delegado.usuario = de.USUARIO_ID;
                    delegado.nombre = de.USUARIO.NOMBRE + " " + de.USUARIO.APELLIDO_P + " " + de.USUARIO.APELLIDO_M;
                    delegado.LISTA = pd;
                    if (delegado.LISTA.Count > 0)
                        delegados.Add(delegado);
                }
                if (delegados.Count > 0)
                    ViewBag.delegados = delegados;

                ViewBag.returnUrl = returnUrl;


                return View(p.ToList());
            }
            //return View();

        }

        [HttpPost]
        public FileResult Descargar()
        {
            //var dOCUMENTOes = db.DOCUMENTOes.Where(a => a.USUARIOC_ID.Equals(User.Identity.Name)).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.CLIENTE).Include(d => d.PAI).Include(d => d.SOCIEDAD).ToList();
            //var dOCUMENTOVs = db.DOCUMENTOVs.Where(a => a.USUARIOA_ID.Equals(User.Identity.Name)).ToList();
            //var tsol = db.TSOLs.ToList();
            //var tall = db.TALLs.ToList();
            //foreach (DOCUMENTOV v in dOCUMENTOVs)
            //{
            //    DOCUMENTO d = new DOCUMENTO();
            //    var ppd = d.GetType().GetProperties();
            //    var ppv = v.GetType().GetProperties();
            //    foreach (var pv in ppv)
            //    {
            //        foreach (var pd in ppd)
            //        {
            //            if (pd.Name == pv.Name)
            //            {
            //                pd.SetValue(d, pv.GetValue(v));
            //                break;
            //            }
            //        }
            //    }
            //    d.TSOL = tsol.Where(a => a.ID.Equals(d.TSOL_ID)).FirstOrDefault();
            //    d.TALL = tall.Where(a => a.ID.Equals(d.TALL_ID)).FirstOrDefault();
            //    dOCUMENTOes.Add(d);
            //}
            //dOCUMENTOes = dOCUMENTOes.Distinct(new DocumentoComparer()).ToList();
            //dOCUMENTOes = dOCUMENTOes.OrderByDescending(a => a.FECHAC).OrderByDescending(a => a.NUM_DOC).ToList();

            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            string us = "";
            us = User.Identity.Name;
            var sociedades = user.SOCIEDADs;
            List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO == true).ToList();
            List<CSP_DOCUMENTOSXUSER2_Result> dOCUMENTOes = db.CSP_DOCUMENTOSXUSER2(us, user.SPRAS_ID).ToList();

            List<Documento> listaDocs = new List<Documento>();
            if (user.PUESTO_ID == 14) {
                foreach (var cocode in sociedades) {
                    List<CSP_DOCUMENTOSXCOCODE_Result> dOCUMENTOesc = db.CSP_DOCUMENTOSXCOCODE(cocode.BUKRS, user.SPRAS_ID).ToList();
                    foreach (CSP_DOCUMENTOSXCOCODE_Result item in dOCUMENTOesc)
                {
                    Documento ld = new Documento();
                    ld.BUTTON = item.BUTTON;

                    ld.NUM_DOC = item.NUM_DOC;
                    ld.NUM_DOC_TEXT = item.NUM_DOC_TEXT;
                    ld.SOCIEDAD_ID = item.SOCIEDAD_ID;
                    ld.PAIS_ID = item.PAIS_ID;
                    ld.FECHADD = item.FECHAD.Value.Day + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Year;
                    ld.FECHAD = item.FECHAD.Value.Year + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Day;
                    ld.HORAC = item.HORAC.Value.ToString().Split('.')[0];
                    ld.PERIODO = item.PERIODO + "";

                    if (item.ESTATUS == "R")
                    {
                        FLUJO flujo = db.FLUJOes.Include("USUARIO").Where(x => x.NUM_DOC == item.NUM_DOC & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault();
                        item.ESTATUSS = item.ESTATUSS.Substring(0, 6) +
                                        (flujo.USUARIO != null ? flujo.USUARIO.PUESTO_ID.ToString() : "") +
                                        item.ESTATUSS.Substring(6, 2);
                    }
                    else
                    {
                        item.ESTATUSS = item.ESTATUSS.Substring(0, 6) + " " + item.ESTATUSS.Substring(6, 2); ;
                    }
                    Estatus e = new Estatus();
                    ld.ESTATUS = e.getText(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID);
                    ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS, ld.NUM_DOC);
                    //ld.ESTATUS = e.getText(item.ESTATUSS);
                    //ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS);

                    ld.PAYER_ID = item.PAYER_ID;

                    ld.CLIENTE = item.NAME1;
                    ld.CANAL = item.CANAL;
                    ld.TSOL = item.TXT020;
                    ld.TALL = item.TXT50;
                    //foreach (CUENTA cuenta in db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(item.SOCIEDAD_ID)).ToList())
                    //{
                    //    if (item.TALL != null)
                    //    {
                    //        if (cuenta.TALL_ID == item.TALL.ID)
                    //        {
                    //            ld.CUENTAS = cuenta.CARGO.ToString();
                    //            break;
                    //        }
                    //    }
                    //}
                    try
                    {
                        ld.CUENTAS = item.CARGO + "";
                        ld.CUENTAP = item.CUENTAP;
                        ld.CUENTAPL = item.CUENTAPL;
                        ld.CUENTACL = item.CUENTACL;
                    }
                    catch { }
                    ld.CONCEPTO = item.CONCEPTO;
                    ld.MONTO_DOC_ML = item.MONTO_DOC_MD != null ? Convert.ToDecimal(item.MONTO_DOC_MD).ToString("C") : "";

                    ld.FACTURA = item.FACTURA;
                    ld.FACTURAK = item.FACTURAK;

                    ld.USUARIOC_ID = item.USUARIOD_ID;
                    ld.USUARIOM_ID = item.USUARIOD_ID;

                    if (item.DOCUMENTO_SAP != null)
                    {
                        if (item.PADRE)
                        {
                            ld.NUM_PRO = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_NC = "";
                            ld.NUM_REV = "";
                        }
                        else if (item.REVERSO)
                        {
                            ld.NUM_REV = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_NC = "";
                            ld.NUM_PRO = "";
                        }
                        else
                        {
                            ld.NUM_NC = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_PRO = "";
                            ld.NUM_REV = "";
                        }
                        //<!--NUM_SAP-->
                        ld.BLART = item.BLART;
                        ld.NUM_PAYER = item.KUNNR;
                        ld.NUM_CLIENTE = item.DESCR;
                        ld.NUM_IMPORTE = item.IMPORTE != null ? Convert.ToDecimal(item.IMPORTE).ToString("C") : "";
                        ld.NUM_CUENTA = item.CUENTA_C;
                    }
                    else
                    {
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                    }

                    listaDocs.Add(ld);
                }
                }
            }
            else { 
            foreach (CSP_DOCUMENTOSXUSER2_Result item in dOCUMENTOes)
            {
                Documento ld = new Documento();
                ld.BUTTON = item.BUTTON;
                    if (ld.BUTTON == "expand_more")
                    {
                        ld.TIPO_RECURRENTE = item.TIPO_RECURRENTE;
                    }                        
                ld.NUM_DOC = item.NUM_DOC;
                ld.NUM_DOC_TEXT = item.NUM_DOC_TEXT;
                ld.SOCIEDAD_ID = item.SOCIEDAD_ID;
                    ld.DOCUMENTO_REF = item.DOCUMENTO_REF;
                ld.PAIS_ID = item.PAIS_ID;
                ld.FECHADD = item.FECHAD.Value.Day + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Year;
                ld.FECHAD = item.FECHAD.Value.Year + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Day;
                ld.HORAC = item.HORAC.Value.ToString().Split('.')[0];
                ld.PERIODO = item.PERIODO + "";
                //ld.TIPO_RECURRENTE = item.TIPO_RECURRENTE;
                if (item.ESTATUS == "R")
                {
                    FLUJO flujo = db.FLUJOes.Include("USUARIO").Where(x => x.NUM_DOC == item.NUM_DOC & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault();
                    item.ESTATUSS = item.ESTATUSS.Substring(0, 6) +
                                    (flujo.USUARIO != null ? flujo.USUARIO.PUESTO_ID.ToString() : "") +
                                    item.ESTATUSS.Substring(6, 2);
                }
                else
                {
                    item.ESTATUSS = item.ESTATUSS.Substring(0, 6) + " " + item.ESTATUSS.Substring(6, 2); ;
                }
                Estatus e = new Estatus();
                ld.ESTATUS = e.getText(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID,ee);
                ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS, ld.NUM_DOC,user.SPRAS_ID,ee);
                    ld.ESTATUS_WF = item.ESTATUS_WF_USER != "R" ? item.ESTATUS_WF_USER : "";
                    if (ld.ESTATUS_WF == "P" && (item.USUARIOA_ID != us))
                        ld.ESTATUS_WF = "";
                    //ld.ESTATUS = e.getText(item.ESTATUSS);
                    //ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS);

                    ld.PAYER_ID = item.PAYER_ID;

                ld.CLIENTE = item.NAME1;
                ld.CANAL = item.CANAL;
                ld.TSOL = item.TXT020;
                ld.TALL = item.TXT50;
                //foreach (CUENTA cuenta in db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(item.SOCIEDAD_ID)).ToList())
                //{
                //    if (item.TALL != null)
                //    {
                //        if (cuenta.TALL_ID == item.TALL.ID)
                //        {
                //            ld.CUENTAS = cuenta.CARGO.ToString();
                //            break;
                //        }
                //    }
                //}
                try
                {
                    ld.CUENTAS = item.CARGO + "";
                    ld.CUENTAP = item.CUENTAP;
                    ld.CUENTAPL = item.CUENTAPL;
                    ld.CUENTACL = item.CUENTACL;
                }
                catch { }
                ld.CONCEPTO = item.CONCEPTO;
                ld.MONTO_DOC_ML = item.MONTO_DOC_MD != null ? Convert.ToDecimal(item.MONTO_DOC_MD).ToString("C") : "";

                ld.FACTURA = item.FACTURA;
                ld.FACTURAK = item.FACTURAK;

                ld.USUARIOC_ID = item.USUARIOD_ID;
                ld.USUARIOM_ID = item.USUARIOD_ID;

                if (item.DOCUMENTO_SAP != null)
                {
                    if (item.PADRE)
                    {
                        ld.NUM_PRO = item.DOCUMENTO_SAP;
                        ld.NUM_AP = "";
                        ld.NUM_NC = "";
                        ld.NUM_REV = "";
                    }
                    else if (item.REVERSO)
                    {
                        ld.NUM_REV = item.DOCUMENTO_SAP;
                        ld.NUM_AP = "";
                        ld.NUM_NC = "";
                        ld.NUM_PRO = "";
                    }
                    else
                    {
                        ld.NUM_NC = item.DOCUMENTO_SAP;
                        ld.NUM_AP = "";
                        ld.NUM_PRO = "";
                        ld.NUM_REV = "";
                    }
                    //<!--NUM_SAP-->
                    ld.BLART = item.BLART;
                    ld.NUM_PAYER = item.KUNNR;
                    ld.NUM_CLIENTE = item.DESCR;
                    ld.NUM_IMPORTE = item.IMPORTE != null ? Convert.ToDecimal(item.IMPORTE).ToString("C") : "";
                    ld.NUM_CUENTA = item.CUENTA_C;
                }
                else
                {
                    //<td></td>
                    //<td></td>
                    //<td></td>
                    //<td></td>
                    //<td></td>
                    //<td></td>
                    //<td></td>
                    //<td></td>
                    //<td></td>
                }

                listaDocs.Add(ld);
                }
            }
            generarExcelHome(listaDocs.OrderByDescending(t => t.ESTATUS_WF).ThenByDescending(t => t.FECHAD).ThenByDescending(t => t.HORAC).ThenByDescending(t => t.NUM_DOC).ToList(), Server.MapPath("~/PdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/Doc" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Doc" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<Documento> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");

            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
{
                  new {
                      BANNER = "Recurrencia"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
{
                  new {
                      BANNER = "Número Solicitud"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
             {
                  new {
                      BANNER = "Número Provisión"
                      },
                    };
                worksheet.Cell("D1").Value = new[]
            {
                  new {
                      BANNER = "Company Code"
                      },
                    };
                worksheet.Cell("E1").Value = new[]
            {
                  new {
                      BANNER = "País"
                      },
                    };
                worksheet.Cell("F1").Value = new[]
            {
                  new {
                      BANNER = "Fecha Solicitud"
                      },
                    };
                worksheet.Cell("G1").Value = new[]
            {
                  new {
                      BANNER = "Hora de Solicitud"
                      },
                    };
                worksheet.Cell("H1").Value = new[]
            {
                  new {
                      BANNER = "Período Contable"
                      },
                    };
                worksheet.Cell("I1").Value = new[]
              {
                  new {
                      BANNER = "Estatus"
                      },
                    };
                worksheet.Cell("J1").Value = new[]
            {
                  new {
                      BANNER = "Cliente ID"
                      },
                    };
                worksheet.Cell("K1").Value = new[]
            {
                  new {
                      BANNER = "Cliente"
                      },
                    };
                worksheet.Cell("L1").Value = new[]
                {
                    new {
                        BANNER = "Canal"
                    },
                };
                worksheet.Cell("M1").Value = new[]
            {
                  new {
                      BANNER = "Tipo Solicitud"
                      },
                    };
                worksheet.Cell("N1").Value = new[]
            {
                  new {
                      BANNER = "Clasificación Allowances"
                      },
                    };
                worksheet.Cell("O1").Value = new[]
      {
                  new {
                      BANNER = "Cuenta Contable Gasto"
                      },
                    };
                worksheet.Cell("P1").Value = new[]
{
                  new {
                      BANNER = "Cuenta Contable Pasivo"
                      },
                    };
                worksheet.Cell("Q1").Value = new[]
                {
                  new {
                      BANNER = "Cuenta Contable Clearing"
                      },
                    };
                worksheet.Cell("R1").Value = new[]
{
                  new {
                      BANNER = "Descripción Solicitud"
                      },
                    };
                worksheet.Cell("S1").Value = new[]
      {
                  new {
                      BANNER = "$ Importe Solicitud"
                      },
                    };
                worksheet.Cell("T1").Value = new[]
      {
                  new {
                      BANNER = "Número Factura Proveedor"
                      },
                    };
                worksheet.Cell("U1").Value = new[]
      {
                  new {
                      BANNER = "Número Factura Kellogg"
                      },
                    };
                worksheet.Cell("V1").Value = new[]
                {
                  new {
                      BANNER = "Creado por"
                      },
                    };
                worksheet.Cell("W1").Value = new[]
     {
                  new {
                      BANNER = "Modificado por"
                      },
                    };
                worksheet.Cell("X1").Value = new[]
      {
                  new {
                      BANNER = "Núm. Registro Provisión"
                      },
                    };
                worksheet.Cell("Y1").Value = new[]
     {
                  new {
                      BANNER = "Núm. Registro NC/OP"
                      },
                    };
                worksheet.Cell("Z1").Value = new[]
  {
                  new {
                      BANNER = "Núm. Registro AP"
                      },
                    };
                worksheet.Cell("AA1").Value = new[]
                {
                  new {
                      BANNER = "Núm. Registro Reverso"
                      },
                    };
                worksheet.Cell("AB1").Value = new[]
                {
                  new {
                      BANNER = "Tipo Registro SAP"
                      },
                    };
                worksheet.Cell("AC1").Value = new[]
                {
                  new {
                      BANNER = "Cliente ID"
                      },
                    };
                worksheet.Cell("AD1").Value = new[]
               {
                  new {
                      BANNER = "Cliente"
                      },
                    };
                worksheet.Cell("AF1").Value = new[]
              {
                  new {
                      BANNER = "$ Importe Moneda Local"
                      },
                    };
                worksheet.Cell("AG1").Value = new[]
              {
                  new {
                      BANNER = "Cuenta Contable Gasto"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
{
                  new {
                      BANNER       = !String.IsNullOrEmpty(lst[i-2].TIPO_RECURRENTE)?"X":""
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
{
                  new {
                      BANNER       = lst[i-2].NUM_DOC
                      },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                 {
                  new {
                      BANNER       = lst[i-2].DOCUMENTO_REF
                      },
                    };
                    worksheet.Cell("D" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].SOCIEDAD_ID
                      },
                    };
                    worksheet.Cell("E" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].PAIS_ID
                        },
                      };
                    worksheet.Cell("F" + i).Value = new[]
                  {
                   new {
                       BANNER       = lst[i-2].FECHAD
                       },
                     };
                    worksheet.Cell("G" + i).Value = new[]
               {
                  new {
                      BANNER       = lst[i-2].HORAC.ToString().Split('.')[0]
                      },
                    };
                    var fx = lst[i - 2].PERIODO;
                    worksheet.Cell("H" + i).Value = new[]
                 {
                  new {
                      BANNER       = fx
                      },
                    };
                    //Verdes
                    if (lst[i - 2].ESTATUS_CLASS.Contains("green") || lst[i - 2].ESTATUS_CLASS == "new badge green darken-1 white-text" || lst[i - 2].ESTATUS_CLASS == "lbl_txt new badge green darken-1 white-text" || lst[i - 2].ESTATUS_CLASS == "lbl_rev new badge green darken-1 white-text")
                    {
                        worksheet.Cell("I" + i).Value = new[]
                    {
                        new
                        {
                            BANNER = lst[i - 2].ESTATUS
                    },
                };
                        worksheet.Range("I" + i + ":I" + i).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#43A047")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    }//grises
                    else if (lst[i - 2].ESTATUS_CLASS.Contains("grey") || lst[i - 2].ESTATUS_CLASS == "new badge grey darken-2 white-text")
                    {
                        worksheet.Cell("I" + i).Value = new[]
              {
                        new
                        {
                            BANNER = lst[i - 2].ESTATUS
                        },
                };
                        worksheet.Range("I" + i + ":I" + i).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#616161")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    }//amarillos
                    else if (lst[i - 2].ESTATUS_CLASS.Contains("yellow") || lst[i - 2].ESTATUS_CLASS=="new badge yellow darken-1 white-text" || lst[i - 2].ESTATUS_CLASS == "lbl_ts yellow darken-2 white-text new badge" || lst[i - 2].ESTATUS_CLASS == "lbl_soporte new badge yellow darken-2 white-text")
                    {
                        worksheet.Cell("I" + i).Value = new[]
              {
                        new
                        {
                            BANNER = lst[i - 2].ESTATUS
                        },
                };
                        worksheet.Range("I" + i + ":I" + i).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#fbc02d")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    }//naranjas
                    else if (lst[i - 2].ESTATUS_CLASS.Contains("orange") || lst[i - 2].ESTATUS_CLASS == "new badge orange darken-1 white-text" || lst[i - 2].ESTATUS_CLASS == "lbl_borr new badge orange darken-2 white-text")
                    {
                        worksheet.Cell("I" + i).Value = new[]
              {
                        new
                        {
                            BANNER = lst[i - 2].ESTATUS
                        },
                };
                        worksheet.Range("I" + i + ":I" + i).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#f57c00")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    }//Rojos
                    else if (lst[i - 2].ESTATUS_CLASS.Contains("red") || lst[i - 2].ESTATUS_CLASS == "new badge red darken-1 white-text" || lst[i - 2].ESTATUS_CLASS == "lbl_txt new badge red darken-1 white-text" || lst[i - 2].ESTATUS_CLASS == "lbl_cancelled new badge red darken-1 white-text")
                    {
                        worksheet.Cell("I" + i).Value = new[]
                    {
                        new
                        {
                            BANNER = lst[i - 2].ESTATUS
                        },
                };
                        worksheet.Range("I" + i + ":I" + i).Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#E53935")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    }
                    worksheet.Cell("J" + i).Value = new[]
                {
                    new {
                        BANNER       = lst[i-2].PAYER_ID
                        },
                      };
                    //Para sacar el Nombre "I" y "J"
                    worksheet.Cell("K" + i).Value = new[]
                    {
                                //List Clientes y for para sacar su name 
                                new {
                                    BANNER = lst[i-2].CLIENTE
                                },
                            };

                    worksheet.Cell("L" + i).Value = new[]
                    {
                                //   List Clientes y for para sacar su name
                                new
                                {
                                    BANNER = lst[i-2].CANAL
                                },
                            };
                    worksheet.Cell("M" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].TSOL
                        },
                    };
                    worksheet.Cell("N" + i).Value = new[]
                   {
                        new {
                            BANNER       = lst[i-2].TALL
                        },
                    };

                    worksheet.Cell("O" + i).Value = new[]
                    {
                                new {
                                    BANNER       = lst[i-2].CUENTAP
                                },
                            };
                    worksheet.Cell("P" + i).Value = new[]
                            {
                                new {
                                    BANNER       = lst[i-2].CUENTAPL
                                },
                            };
                    worksheet.Cell("Q" + i).Value = new[]
                            {
                                new {
                                    BANNER       = lst[i-2].CUENTACL
                                },
                            };

                    worksheet.Cell("R" + i).Value = new[]
                   {
                        new {
                            BANNER       = lst[i-2].CONCEPTO
                        },
                    };
                    worksheet.Cell("S" + i).Value = new[]
                   {
                        new {
                            BANNER       = lst[i-2].MONTO_DOC_ML
                        },
                    };
                    worksheet.Cell("T" + i).Value = new[]
                    {
                                new {
                                    BANNER       =lst[i-2].FACTURA
                                },
                            };

                    worksheet.Cell("U" + i).Value = new[]
                    {
                                new {
                                    BANNER       = lst[i-2].FACTURAK
                                },
                            };

                    worksheet.Cell("V" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].USUARIOC_ID
                        },
                    };
                    worksheet.Cell("W" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].USUARIOC_ID
                        },
                    };
                    worksheet.Cell("X" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_PRO
                        },
                    };
                    worksheet.Cell("Y" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_NC
                        },
                    };
                    worksheet.Cell("Z" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_AP
                        },
                    };
                    worksheet.Cell("AA" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_REV
                        },
                    };
                    worksheet.Cell("AB" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].BLART
                        },
                    };
                    worksheet.Cell("AC" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_PAYER
                        },
                    };
                    worksheet.Cell("AD" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_CLIENTE
                        },
                    };
                    worksheet.Cell("AF" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_IMPORTE
                        },
                    };
                    worksheet.Cell("AG" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_CUENTA
                        },
                    };
                }
                var rt = ruta + @"\Doc" + DateTime.Now.ToShortDateString() + ".xlsx";
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }

        }

        [HttpGet]
        public JsonResult Clientes(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";

            TruequeEntities db = new TruequeEntities();

            var c = (from N in db.CLIENTEs
                     where N.KUNNR.Contains(Prefix)
                     select new { N.KUNNR, N.NAME1 });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpPost]
        public FileResult DescargarEx()
        {

            generarExcelCons((List<Concentrado>)Session["excelUsuariosConsulta"], Server.MapPath("~/PdfTemp/"));
            return File(Server.MapPath("~/PdfTemp/Solicitudes_Consulta/" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Solicitudes_Consulta" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelCons(List<Concentrado> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Solicitudes");
            try
            {
                worksheet.Cell("A1").Value = new[] { new { BANNER = "Co. Code" }, };
                worksheet.Cell("B1").Value = new[] { new { BANNER = "País" }, };
                worksheet.Cell("C1").Value = new[] { new { BANNER = "Subregión" }, };
                worksheet.Cell("D1").Value = new[] { new { BANNER = "Estado" }, };
                worksheet.Cell("E1").Value = new[] { new { BANNER = "Ciudad" }, };
                worksheet.Cell("F1").Value = new[] { new { BANNER = "Número solicitud" }, };
                worksheet.Cell("G1").Value = new[] { new { BANNER = "Fecha Solicitud" }, };
                worksheet.Cell("H1").Value = new[] { new { BANNER = "Hora solicitud" }, };
                worksheet.Cell("I1").Value = new[] { new { BANNER = "Semana del periodo" }, };
                worksheet.Cell("J1").Value = new[] { new { BANNER = "Periódo contable" }, };
                worksheet.Cell("K1").Value = new[] { new { BANNER = "Año" }, };
                worksheet.Cell("L1").Value = new[] { new { BANNER = "Tipo Solicitud" }, };
                worksheet.Cell("M1").Value = new[] { new { BANNER = "Tipo Provisión" }, };
                worksheet.Cell("N1").Value = new[] { new { BANNER = "Tipo NC" }, };
                worksheet.Cell("O1").Value = new[] { new { BANNER = "Tipo OP" }, };
                worksheet.Cell("P1").Value = new[] { new { BANNER = "Status" }, };
                worksheet.Cell("Q1").Value = new[] { new { BANNER = "Concepto" }, };
                worksheet.Cell("R1").Value = new[] { new { BANNER = "Mecánica" }, };
                worksheet.Cell("S1").Value = new[] { new { BANNER = "De" }, };
                worksheet.Cell("T1").Value = new[] { new { BANNER = "A" }, };
                worksheet.Cell("U1").Value = new[] { new { BANNER = "De" }, };
                worksheet.Cell("V1").Value = new[] { new { BANNER = "A" }, };
                worksheet.Cell("W1").Value = new[] { new { BANNER = "Clasificación" }, };
                worksheet.Cell("X1").Value = new[] { new { BANNER = "Cuenta Contable Gasto" }, };
                worksheet.Cell("Y1").Value = new[] { new { BANNER = "Nombre de la cuenta" }, };
                worksheet.Cell("Z1").Value = new[] { new { BANNER = "Cliente" }, };
                worksheet.Cell("AA1").Value = new[] { new { BANNER = "Nombre" }, };
                worksheet.Cell("AB1").Value = new[] { new { BANNER = "Tipo de Cliente" }, };
                worksheet.Cell("AC1").Value = new[] { new { BANNER = "Organización de Ventas" }, };
                worksheet.Cell("AD1").Value = new[] { new { BANNER = "Tax ID" }, };
                worksheet.Cell("AE1").Value = new[] { new { BANNER = "Canal" }, };
                worksheet.Cell("AF1").Value = new[] { new { BANNER = "Descripción" }, };
                worksheet.Cell("AG1").Value = new[] { new { BANNER = "Nombre de Contacto" }, };
                worksheet.Cell("AH1").Value = new[] { new { BANNER = "Email de Contacto" }, };
                worksheet.Cell("AI1").Value = new[] { new { BANNER = "$ Monto Original de Provisión" }, };
                worksheet.Cell("AJ1").Value = new[] { new { BANNER = "$ Monto NC/OP" }, };
                worksheet.Cell("AK1").Value = new[] { new { BANNER = "$Monto Aplicado" }, };
                worksheet.Cell("AL1").Value = new[] { new { BANNER = "Saldo Remanente Provisión" }, };
                worksheet.Cell("AM1").Value = new[] { new { BANNER = "$ Monto Reverso Provisión" }, };
                worksheet.Cell("AN1").Value = new[] { new { BANNER = "% Reverso" }, };
                worksheet.Cell("AO1").Value = new[] { new { BANNER = "$ Exceso/(Insuficiencia) Provisión" }, };
                worksheet.Cell("AP1").Value = new[] { new { BANNER = "% Exceso/(Insuficiencia) Provisión" }, };
                worksheet.Cell("AQ1").Value = new[] { new { BANNER = "Impactos / Beneficios en el gasto del Periodo" }, };
                worksheet.Cell("AR1").Value = new[] { new { BANNER = "Núm. Registro Provisión" }, };
                worksheet.Cell("AS1").Value = new[] { new { BANNER = "Núm. Registro NC/OP" }, };
                worksheet.Cell("AT1").Value = new[] { new { BANNER = "Núm. Registro Reverso" }, };
                worksheet.Cell("AU1").Value = new[] { new { BANNER = "Fecha Reverso Provisión" }, };
                worksheet.Cell("AV1").Value = new[] { new { BANNER = "Periodo Contable Reverso" }, };
                worksheet.Cell("AW1").Value = new[] { new { BANNER = "Razón Reverso" }, };
                worksheet.Cell("AX1").Value = new[] { new { BANNER = "Comentario Reverso" }, };
                worksheet.Cell("AY1").Value = new[] { new { BANNER = "Usuario" }, };
                worksheet.Cell("AZ1").Value = new[] { new { BANNER = "Back Up" }, };
                worksheet.Cell("BA1").Value = new[] { new { BANNER = "Creado Por" }, };
                worksheet.Cell("BB1").Value = new[] { new { BANNER = "Creado Por ID" }, };
                worksheet.Cell("BC1").Value = new[] { new { BANNER = "Solicitado Por" }, };
                worksheet.Cell("BD1").Value = new[] { new { BANNER = "Solicitado Por ID" }, };
                worksheet.Cell("BE1").Value = new[] { new { BANNER = "Modificado Por" }, };
                worksheet.Cell("BF1").Value = new[] { new { BANNER = "Modificado Por ID" }, };
                //worksheet.Cell("BG1").Value = new[] { new { BANNER = "Payer" }, };
                worksheet.Cell("BG1").Value = new[] { new { BANNER = "Aprobador Nivel 1" }, };
                worksheet.Cell("BH1").Value = new[] { new { BANNER = "Aprobador Nivel 2" }, };
                worksheet.Cell("B1").Value = new[] { new { BANNER = "Aprobador Nivel 3" }, };
                worksheet.Cell("BJ1").Value = new[] { new { BANNER = "Aprobador Nivel 4" }, };
                worksheet.Cell("BK1").Value = new[] { new { BANNER = "Aprobador Nivel 5" }, };
                worksheet.Cell("BL1").Value = new[] { new { BANNER = "Proveedor" }, };
                worksheet.Cell("BM1").Value = new[] { new { BANNER = "Nombre Proveedor" }, };
                worksheet.Cell("BN1").Value = new[] { new { BANNER = "Número Factura Proveedor" }, };
                worksheet.Cell("BO1").Value = new[] { new { BANNER = "Número Factura Kellogg´s" }, };
                worksheet.Cell("BP1").Value = new[] { new { BANNER = "Número NC" }, };
                worksheet.Cell("BQ1").Value = new[] { new { BANNER = "Numero Orden de Pago" }, };
                worksheet.Cell("BR1").Value = new[] { new { BANNER = "Expense Recognition en periodo" }, };
                worksheet.Cell("BS1").Value = new[] { new { BANNER = "Expense Recognition Ejercicio" }, };
                worksheet.Cell("BT1").Value = new[] { new { BANNER = "Soporte Incorrecto" }, };
                worksheet.Cell("BU1").Value = new[] { new { BANNER = "Soporte validado con base variable pendiente" }, };
                worksheet.Cell("BV1").Value = new[] { new { BANNER = "Carta" }, };
                worksheet.Cell("BW1").Value = new[] { new { BANNER = "Contrato" }, };
                worksheet.Cell("BX1").Value = new[] { new { BANNER = "JBP" }, };
                worksheet.Cell("BY1").Value = new[] { new { BANNER = "Factura" }, };
                worksheet.Cell("BZ1").Value = new[] { new { BANNER = "Otros" }, };
                worksheet.Cell("CA1").Value = new[] { new { BANNER = "Negociación por monto" }, };
                worksheet.Cell("CB1").Value = new[] { new { BANNER = "Negociación por %" }, };
                worksheet.Cell("CC1").Value = new[] { new { BANNER = "Distribución por material" }, };
                worksheet.Cell("CD1").Value = new[] { new { BANNER = "Distribución categoría" }, };
                worksheet.Cell("CE1").Value = new[] { new { BANNER = "Monto Base" }, };
                worksheet.Cell("CF1").Value = new[] { new { BANNER = "Monto %" }, };
                worksheet.Cell("CG1").Value = new[] { new { BANNER = "Recurrente" }, };
                worksheet.Cell("CH1").Value = new[] { new { BANNER = "Recurrente por porcentaje" }, };
                worksheet.Cell("CI1").Value = new[] { new { BANNER = "Recurrente por Monto" }, };
                worksheet.Cell("CJ1").Value = new[] { new { BANNER = "Recurrente Cancelada" }, };
                worksheet.Cell("CK1").Value = new[] { new { BANNER = "Objetivo Inicio" }, };
                worksheet.Cell("CL1").Value = new[] { new { BANNER = "Objetivo Límite" }, };
                worksheet.Cell("CM1").Value = new[] { new { BANNER = "Estatus" }, };
                worksheet.Cell("CN1").Value = new[] { new { BANNER = "Venta del Periodo" }, };
                worksheet.Cell("CO1").Value = new[] { new { BANNER = "Monto Provisión" }, };
                worksheet.Cell("CP1").Value = new[] { new { BANNER = "Back Order" }, };
                worksheet.Cell("CQ1").Value = new[] { new { BANNER = "Venta Real + BO" }, };
                worksheet.Cell("CR1").Value = new[] { new { BANNER = "NC / OP o Reverso" }, };
                worksheet.Cell("CS1").Value = new[] { new { BANNER = "Monto" }, };



                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    DateTime fechac = (DateTime)lst[i - 2].documento.FECHAC;
                    DateTime horac = (DateTime)lst[i - 2].documento.FECHAC;
                    string monotdocmd = String.Format("{0:C}", lst[i - 2].documento.MONTO_DOC_MD);
                    string monotodocmd2 = String.Format("({0:C})", lst[i - 2].documento.MONTO_DOC_MD);
                    string prepupuestoptc = "";
                    string presupestoconsu = "";
                    try
                    {
                        prepupuestoptc = String.Format("({0:C})", lst[i - 2].PRESUPUESTO.PC_T);
                        presupestoconsu = String.Format("{0:C}", lst[i - 2].PRESUPUESTO.CONSU);
                    }
                    catch { }
                    worksheet.Cell("A" + i).Value = new[] { new { BANNER = lst[i - 2].documento.SOCIEDAD_ID }, };
                    worksheet.Cell("B" + i).Value = new[] { new { BANNER = lst[i - 2].documento.PAIS_ID }, };
                    worksheet.Cell("C" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CLIENTE.REGION }, };
                    worksheet.Cell("D" + i).Value = new[] { new { BANNER = lst[i - 2].documento.ESTADO }, };
                    worksheet.Cell("E" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CIUDAD }, };
                    worksheet.Cell("F" + i).Value = new[] { new { BANNER = lst[i - 2].documento.NUM_DOC }, };
                    worksheet.Cell("G" + i).Value = new[] { new { BANNER = fechac.ToShortDateString() }, };
                    worksheet.Cell("H" + i).Value = new[] { new { BANNER = horac.ToString("HH:mm:ss") }, };
                    worksheet.Cell("I" + i).Value = new[] { new { BANNER = lst[i - 2].SEMANA }, };
                    worksheet.Cell("J" + i).Value = new[] { new { BANNER = lst[i - 2].documento.PERIODO }, };
                    worksheet.Cell("K" + i).Value = new[] { new { BANNER = lst[i - 2].documento.EJERCICIO }, };
                    try
                    {
                        worksheet.Cell("L" + i).Value = new[] { new { BANNER = lst[i - 2].documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(lst[i - 2].documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT020 }, };
                    }
                    catch { }

                    if (lst[i - 2].documento.TSOL_ID.StartsWith("PR"))
                    {
                        try
                        {
                            worksheet.Cell("M" + i).Value = new[] { new { BANNER = lst[i - 2].documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(lst[i - 2].documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50 }, };
                        }
                        catch { }
                    }

                    try
                    {
                        if (lst[i - 2].documento.TSOL_ID.StartsWith("NC"))
                        {
                            worksheet.Cell("N" + i).Value = new[] { new { BANNER = lst[i - 2].documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(lst[i - 2].documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50 }, };
                        }
                    }
                    catch { }

                    if (lst[i - 2].documento.TSOL_ID.StartsWith("OP"))
                    {
                        worksheet.Cell("O" + i).Value = new[] { new { BANNER = lst[i - 2].documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(lst[i - 2].documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50 }, };
                    }


                    worksheet.Cell("P" + i).Value = new[] { new { BANNER = lst[i - 2].ESTATUS_STRING }, };
                    worksheet.Cell("Q" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CONCEPTO }, };
                    worksheet.Cell("R" + i).Value = new[] { new { BANNER = lst[i - 2].documento.NOTAS }, };
                    try
                    {
                        worksheet.Cell("S" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FECHAI_VIG.Value.ToShortDateString() }, };
                        worksheet.Cell("T" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FECHAF_VIG.Value.ToShortDateString() }, };
                    }
                    catch { }
                    worksheet.Cell("U" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("V" + i).Value = new[] { new { BANNER = "" }, };
                    try
                    {
                        worksheet.Cell("W" + i).Value = new[] { new { BANNER = lst[i - 2].documento.GALL.GALLTs.FirstOrDefault().TXT50 }, };
                    }
                    catch { }

                    worksheet.Cell("X" + i).Value = new[] { new { BANNER = lst[i - 2].CUENTA_CARGO }, };
                    worksheet.Cell("Y" + i).Value = new[] { new { BANNER = lst[i - 2].CUENTA_CARGO_NOMBRE }, };
                    worksheet.Cell("Z" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CLIENTE.KUNNR }, };
                    worksheet.Cell("AA" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CLIENTE.NAME1 }, };
                    //worksheet.Cell("AB" + i).Value = new[] { new { BANNER = lst[i - 2].CANAL }, };

                    if (lst[i - 2].documento.SOLD_TO_ID != null)
                    {
                        worksheet.Cell("AB" + i).Value = new[] { new { BANNER = "Sold To" }, };
                    }
                    else
                    {
                        worksheet.Cell("AB" + i).Value = new[] { new { BANNER = "Payer" }, };
                    }
                    worksheet.Cell("AC" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CLIENTE.VKORG }, };
                    worksheet.Cell("AD" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CLIENTE.STCD1 }, };
                    worksheet.Cell("AE" + i).Value = new[] { new { BANNER = lst[i - 2].documento.CLIENTE.CANAL }, };


                    if (lst[i - 2].CANAL == null)
                    {
                        worksheet.Cell("AF" + i).Value = new[] { new { BANNER = "" }, };
                    }
                    else
                    {
                        worksheet.Cell("AF" + i).Value = new[] { new { BANNER = lst[i - 2].CANAL.CDESCRIPCION }, };
                    }
                    worksheet.Cell("AG" + i).Value = new[] { new { BANNER = lst[i - 2].documento.PAYER_NOMBRE }, };
                    worksheet.Cell("AH" + i).Value = new[] { new { BANNER = lst[i - 2].documento.PAYER_EMAIL }, };

                    if (lst[i - 2].documento.TSOL_ID.StartsWith("PR"))
                    {
                        worksheet.Cell("AI" + i).Value = new[] { new { BANNER = monotdocmd }, };
                    }
                    if (lst[i - 2].documento.TSOL_ID.StartsWith("NC") || lst[i - 2].documento.TSOL_ID.StartsWith("OP"))
                    {
                        worksheet.Cell("AJ" + i).Value = new[] { new { BANNER = monotodocmd2 }, };
                    }

                    try
                    {
                        if (lst[i - 2].PRESUPUESTO.PC_T == 0)
                        {
                            worksheet.Cell("AK" + i).Value = new[] { new { BANNER = string.Empty }, };

                        }
                        else
                        {
                            worksheet.Cell("AK" + i).Value = new[] { new { BANNER = prepupuestoptc }, };
                        }
                    }
                    catch { }
                    try
                    {
                        if (lst[i - 2].PRESUPUESTO.CONSU == 0)
                        {
                            worksheet.Cell("AL" + i).Value = new[] { new { BANNER = string.Empty }, };

                        }
                        else
                        {
                            worksheet.Cell("AL" + i).Value = new[] { new { BANNER = presupestoconsu }, };
                        }
                    }
                    catch { }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AM" + i).Value = new[] { new { BANNER = String.Format("{0:C}", lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null).GetType().GetProperty("MONTO_DOC_MD").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null), null)) }, };
                    }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AN" + i).Value = new[] { new { BANNER = (Convert.ToDecimal(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null).GetType().GetProperty("MONTO_DOC_MD").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null), null)) * 100) / lst[i - 2].documento.MONTO_DOC_MD }, };
                    }
                    worksheet.Cell("AO" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("AP" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("AQ" + i).Value = new[] { new { BANNER = "" }, };
                    if (lst[i - 2].documento.DOCUMENTOSAP != null)
                    {
                        worksheet.Cell("AR" + i).Value = new[] { new { BANNER = lst[i - 2].documento.DOCUMENTOSAP.NUM_DOC.ToString() }, };
                    }
                    else
                    {
                        worksheet.Cell("AR" + i).Value = new[] { new { BANNER = "" }, };
                    }
                    if (lst[i - 2].documento.TSOL_ID.StartsWith("NC") || lst[i - 2].documento.TSOL_ID.StartsWith("OP"))
                    {
                        worksheet.Cell("AS" + i).Value = new[] { new { BANNER = lst[i - 2].documento.NUM_DOC }, };
                    }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AT" + i).Value = new[] { new { BANNER = lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("NUM_DOC").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null) }, };
                    }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AU" + i).Value = new[] { new { BANNER = lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("FECHAC").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null) }, };
                    }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AV" + i).Value = new[] { new { BANNER = lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("d").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("PERIODO").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("d").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null) }, };
                    }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AW" + i).Value = new[] { new { BANNER = lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("tr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("TXT100").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("tr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null) }, };
                    }
                    if (lst[i - 2].DOCSREFREVERSOS != null)
                    {
                        worksheet.Cell("AX" + i).Value = new[] { new { BANNER = lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null).GetType().GetProperty("COMENTARIO").GetValue(lst[i - 2].DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(lst[i - 2].DOCSREFREVERSOS, null), null) }, };
                    }
                    worksheet.Cell("AY" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIO.NOMBRE + " " + lst[i - 2].documento.USUARIO.APELLIDO_P }, };
                    worksheet.Cell("AZ" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIO.BACKUP_ID }, };
                    worksheet.Cell("BA" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIO.NOMBRE + " " + lst[i - 2].documento.USUARIO.APELLIDO_P }, };
                    worksheet.Cell("BB" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIO.ID }, };
                    worksheet.Cell("BC" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIO.NOMBRE + " " + lst[i - 2].documento.USUARIO.APELLIDO_P }, };
                    worksheet.Cell("BD" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIO.ID }, };
                    worksheet.Cell("BE" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIOD_ID }, };
                    worksheet.Cell("BF" + i).Value = new[] { new { BANNER = lst[i - 2].documento.USUARIOD_ID }, };
                    try
                    {
                        worksheet.Cell("BG" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FLUJOes.Where(u => u.WF_POS == 1).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault() }, };
                    }
                    catch { }

                    try
                    {
                        worksheet.Cell("BH" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FLUJOes.Where(u => u.WF_POS == 2).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault() }, };
                    }
                    catch { }
                    try
                    {
                        worksheet.Cell("BI" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FLUJOes.Where(u => u.WF_POS == 3).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault() }, };

                    }
                    catch { }
                    try
                    {
                        worksheet.Cell("BJ" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FLUJOes.Where(u => u.WF_POS == 4).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault() }, };
                    }
                    catch { }
                    try
                    {
                        worksheet.Cell("BK" + i).Value = new[] { new { BANNER = lst[i - 2].documento.FLUJOes.Where(u => u.WF_POS == 5).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault() }, };
                    }
                    catch { }
                    worksheet.Cell("BL" + i).Value = new[] { new { BANNER = lst[i - 2].documento.DOCUMENTOFs.Select(df => df.PROVEEDOR).FirstOrDefault() }, };
                    worksheet.Cell("BM" + i).Value = new[] { new { BANNER = lst[i - 2].PROVEEDOR_NOMBRE }, };
                    worksheet.Cell("BN" + i).Value = new[] { new { BANNER = lst[i - 2].documento.DOCUMENTOFs.Select(df => df.BILL_DOC).FirstOrDefault() }, };
                    worksheet.Cell("BO" + i).Value = new[] { new { BANNER = lst[i - 2].documento.DOCUMENTOFs.Select(df => df.FACTURAK).FirstOrDefault() }, };
                    worksheet.Cell("BP" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("BQ" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("BR" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 1 && (bool)x.CHECKS).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BS" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 2 && (bool)x.CHECKS).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BT" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 3 && (bool)x.CHECKS).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BU" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 4 && (bool)x.CHECKS).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BV" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("CAR")).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BW" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("CON")).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BX" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("JBP")).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BY" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("FAC")).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("BZ" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("OTR")).Count() > 0) ? "X" : "" }, };
                    worksheet.Cell("CA" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CB" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CC" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CD" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CE" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CF" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CG" + i).Value = new[] { new { BANNER = (lst[i - 2].documento.TIPO_RECURRENTE != null) ? "Si" : "No" }, };
                    worksheet.Cell("CH" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CI" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CJ" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CK" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CL" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CM" + i).Value = new[] { new { BANNER = lst[i - 2].ESTATUS_STRING }, };
                    worksheet.Cell("CN" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CO" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CP" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CQ" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CR" + i).Value = new[] { new { BANNER = "" }, };
                    worksheet.Cell("CS" + i).Value = new[] { new { BANNER = "" }, };

                }
                var rt = ruta + @"\Solicitudes_Consulta/" + DateTime.Now.ToShortDateString() + ".xlsx";
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }
        }
    }
}
