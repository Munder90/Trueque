using EntityFramework.BulkInsert.Extensions;
using ExcelDataReader;
using MoreLinq;
using Newtonsoft.Json;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Models.Dao;
using TAT001.Services;

namespace TAT001.ControllersE
{
    [Authorize]
    [LoginActive]
    public class SolicitudesController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        //------------------DAO------------------------------
        readonly TallsDao tallsDao = new TallsDao();
        readonly TiposSolicitudesDao tiposSolicitudesDao = new TiposSolicitudesDao();
        readonly MaterialesgptDao materialesgptDao = new MaterialesgptDao();
        readonly MaterialesDao materialesDao = new MaterialesDao();
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Solicitudes/Details/5
        public ActionResult Details(decimal id, string ejercicio, string pais)
        {
            string uu = "";
            ////var _dp1 = db.DOCUMENTOes.Find(new object[] { id, ejercicio });
            int pagina = 203; //ID EN BASE DE DATOS
                              ////using (TruequeEntities db = new TruequeEntities())
                              ////{
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 202);

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".png";
            }
            catch
            {
                if (!string.IsNullOrEmpty(pais))
                {
                    ViewBag.pais = pais + ".png";
                    Session["pais"] = pais;
                }
                else
                {
                    ////ViewBag.pais = "mx.png";
                    ////return RedirectToAction("Pais", "Home");
                }
            }
            uu = user.SPRAS_ID;
            Session["spras"] = user.SPRAS_ID;
            ////}

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                    && a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                    && a.SPART.Equals(dOCUMENTO.SPART)
                                                    && a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
            dOCUMENTO.DOCUMENTOF = db.DOCUMENTOFs.Where(a => a.NUM_DOC.Equals(dOCUMENTO.NUM_DOC)).ToList();

            ////ViewBag.workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            var vbFl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            FLUJO fvbfl;

            //recuperamos si existe algun valor en fljunegoc
            var flng = db.FLUJNEGOes.Where(a => a.NUM_DOC.Equals(id)).ToList();
            if (flng.Count > 0)
            {
                for (int i = 0; i < flng.Count; i++)
                {
                    var kn = flng[i].KUNNR;
                    var clName = db.CLIENTEs.Where(c => c.KUNNR == kn).Select(s => s.NAME1).FirstOrDefault();
                    fvbfl = new FLUJO();
                    fvbfl.NUM_DOC = flng[i].NUM_DOC;
                    fvbfl.FECHAC = flng[i].FECHAC;
                    fvbfl.FECHAM = flng[i].FECHAM;
                    fvbfl.USUARIOA_ID = clName;//// + "(Cliente)";
                    fvbfl.COMENTARIO = flng[i].COMENTARIO;
                    vbFl.Add(fvbfl);
                }
            }
            ViewBag.workflow = vbFl;

            string usuariodel = "";
            DateTime fecha = DateTime.Now.Date;
            List<TAT001.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).ToList();


            FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) && a.ESTATUS.Equals("P")).FirstOrDefault();
            ViewBag.acciones = f;
            List<DOCUMENTOA> archivos = db.DOCUMENTOAs.Where(x => x.NUM_DOC.Equals(id) && x.ACTIVO).ToList();//RSG 15.05.2018
            ViewBag.files = archivos;
            if (f != null)
                if (f.USUARIOA_ID != null)
                {
                    if (del.Count > 0)
                    {
                        DELEGAR dell = del.FirstOrDefault(a => a.USUARIO_ID.Equals(f.USUARIOA_ID));
                        if (dell != null)
                            usuariodel = dell.USUARIO_ID;
                        else
                            usuariodel = User.Identity.Name;
                    }
                    else
                        usuariodel = User.Identity.Name;

                    if (f.USUARIOA_ID.ToUpper().Replace(" ", String.Empty).Equals(usuariodel.ToUpper()))
                        ViewBag.accion = db.WORKFPs.Where(a => a.ID.Equals(f.WORKF_ID) && a.POS.Equals(f.WF_POS) && a.VERSION.Equals(f.WF_VERSION)).FirstOrDefault().ACCION.TIPO;
                }
                else
                {
                    ViewBag.accion = db.WORKFPs.Where(a => a.ID.Equals(f.WORKF_ID) && a.POS.Equals(f.WF_POS) && a.VERSION.Equals(f.WF_VERSION)).FirstOrDefault().ACCION.TIPO;
                }
            DocumentoFlujo DF = new DocumentoFlujo();
            DF.D = dOCUMENTO;
            ViewBag.pais = dOCUMENTO.PAIS_ID + ".png"; //RSG 29.09.2018
            DF.F = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();
            ////DF.F.ESTATUS = "";
            ViewBag.ts = db.TS_FORM.Where(a => a.BUKRS_ID.Equals(DF.D.SOCIEDAD_ID) && a.LAND_ID.Equals(DF.D.PAIS_ID) && a.TS_CAMPO.ACTIVO).ToList();
            ViewBag.tts = db.DOCUMENTOTS.Where(a => a.NUM_DOC.Equals(DF.D.NUM_DOC)).ToList();

            if (DF.D.DOCUMENTO_REF != null)
                ViewBag.Title += " " + DF.D.DOCUMENTO_REF + "-";
            ViewBag.Title += " " + id;

            //LEJ 10.07.2018----------------------------------------------
            ViewBag.cartap = db.CARTAPs.Where(i => i.NUM_DOC == id).ToList();

            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();

            ViewBag.TSOL_RELA = tiposSolicitudesDao.TreeTiposSolicitudes(DF.D.SOCIEDAD_ID, uu, "SR");
            //RECUPERO EL PAIS para hacer una busqueda de su formato monetario
            ////var paisMon = Session["pais"].ToString();//------------------------LEJGG090718
            ViewBag.miles = DF.D.PAI.MILES;//LEJGG 090718
            ViewBag.dec = DF.D.PAI.DECIMAL;//LEJGG 090718

            /////////DRS 24.09.18/////////          
            var nombrec = db.CUENTAGLs.Where(x => x.ID == DF.D.CUENTAP).Select(x => x.NOMBRE);
            ViewBag.nombreC = nombrec.ToList();

            var nombrec1 = db.CUENTAGLs.Where(x => x.ID == DF.D.CUENTAPL).Select(x => x.NOMBRE);
            ViewBag.nombreC2 = nombrec1.ToList();

            ///////////////////////////////CAMBIOS LGPP INICIO//////////////////////////*@
            if (DF.D.TIPO_TECNICO == "M")
            {
                DF.D.TIPO_TECNICO = db.TEXTOes.Where(x => x.PAGINA_ID == 201 && x.CAMPO_ID == "lbl_monto" && x.SPRAS_ID == uu).FirstOrDefault().TEXTOS;
            }
            else if (DF.D.TIPO_TECNICO == "P")
            {
                DF.D.TIPO_TECNICO = db.TEXTOes.Where(x => x.PAGINA_ID == 201 && x.CAMPO_ID == "lbl_porcentaje" && x.SPRAS_ID == uu).FirstOrDefault().TEXTOS;
            }
            if (DF.D.DOCUMENTOPs.Count > 0)
                if (DF.D.DOCUMENTOPs.FirstOrDefault().MATNR != "")
                {
                    ViewBag.mat = db.TEXTOes.Where(x => x.PAGINA_ID == 201 && x.CAMPO_ID == "lbl_material" && x.SPRAS_ID == uu).FirstOrDefault().TEXTOS;
                }
                else
                {
                    ViewBag.mat = db.TEXTOes.Where(x => x.PAGINA_ID == 201 && x.CAMPO_ID == "lbl_categoria" && x.SPRAS_ID == uu).FirstOrDefault().TEXTOS;
                }
            ///////////////////////////////CAMBIOS LGPP FIN//////////////////////////*@
            if (DF.D.DOCUMENTORECs.Count == 0)
            {
                DOCUMENTOREC dr = db.DOCUMENTORECs.Where(x => x.DOC_REF == DF.D.NUM_DOC).FirstOrDefault();
                if (dr == null)
                    dr = db.DOCUMENTORECs.Where(x => x.NUM_DOC_Q == DF.D.NUM_DOC).FirstOrDefault();
                if (dr != null)
                {
                    DF.D.OBJETIVOQ = dr.DOCUMENTO.OBJETIVOQ;
                    DF.D.OBJQ_PORC = dr.DOCUMENTO.OBJQ_PORC;
                    DF.D.TSOL_LIG = dr.DOCUMENTO.TSOL_LIG;
                    DF.D.DOCUMENTORECs = db.DOCUMENTORECs.Where(x => x.NUM_DOC == dr.NUM_DOC).ToList();
                }
            }
            List<DOCUMENTO> recs = (from D in DF.D.DOCUMENTORECs
                                    join N in db.DOCUMENTOes.Where(x => x.USUARIOC_ID == DF.D.USUARIOC_ID).ToList()
                                    on D.DOC_REF equals N.NUM_DOC
                                    select N
                                    ).ToList();
            List<DOCUMENTO> recls = (from D in recs
                                     join N in db.DOCUMENTOes.Where(x => x.USUARIOC_ID == DF.D.USUARIOC_ID).ToList()
                                     on D.NUM_DOC equals N.DOCUMENTO_REF
                                     select N
                                    ).ToList();
            ViewBag.recs = recs;
            ViewBag.recls = recls;
            //Tab_Fin Análisis Solicitud
            ObtenerAnalisisSolicitud(DF.D);

            return View(DF);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(IEnumerable<HttpPostedFileBase> files_soporte, string[] labels_soporte, int pos)
        {
            decimal num_doc = decimal.Parse(Request.Form["D.NUM_DOC"].ToString());
            var res = "";
            string errorMessage = "";
            int numFiles = 0;
            //Checar si hay archivos para subir
            foreach (HttpPostedFileBase file in files_soporte.Where(x => x.ContentLength > 0))
            {
                if (file != null && file.ContentLength > 0)
                {
                    numFiles++;
                }
            }
            if (numFiles > 0)
            {
                string url = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") && x.ACTIVO).FirstOrDefault().VALUE;
                //Crear el directorio
                DOCUMENTO du = db.DOCUMENTOes.Find(num_doc);//RSG 01.08.2018
                ////var dir = createDir(url, num_doc.ToString());
                var dir = new Files().createDir(url, num_doc.ToString(), du.EJERCICIO.ToString());//RSG 01.08.2018

                //Evaluar que se creo el directorio
                if (string.IsNullOrEmpty(dir))
                {

                    int i = 0;
                    int indexlabel = 0;
                    int max = 0;
                    if (db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(num_doc)).Count() > 0)
                        max = db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(num_doc)).Max(a => a.POS);//RSG 15.05.2018
                    foreach (HttpPostedFileBase file in files_soporte)
                    {
                        string errorfiles = "";
                        if (file != null)
                        {
                            var clasefile = "";
                            try
                            {
                                clasefile = labels_soporte[indexlabel];
                            }
                            catch
                            {
                                clasefile = "";
                            }
                            if (file.ContentLength > 0)
                            {
                                string path = "";
                                string filename = file.FileName;

                                ////res = SaveFile(file, url, num_doc.ToString(), out errorfiles, out path);
                                res = new Files().SaveFile(file, url, num_doc.ToString(), out errorfiles, out path, du.EJERCICIO.ToString());//RSG 01.08.2018

                                if (errorfiles == "")
                                {
                                    DOCUMENTOA doc = new DOCUMENTOA();
                                    var ext = System.IO.Path.GetExtension(filename);
                                    i++;
                                    doc.NUM_DOC = num_doc;
                                    doc.POS = i;
                                    doc.TIPO = ext.Replace(".", "");
                                    try
                                    {
                                        var clasefileM = clasefile.ToUpper();
                                        doc.CLASE = clasefileM.Substring(0, 3);
                                    }
                                    catch (Exception e)
                                    {
                                        doc.CLASE = "";
                                    }
                                    if (max > 0)
                                    {
                                        doc.POS = max + 1;
                                        DOCUMENTOA da = db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(num_doc) && a.CLASE.Equals(doc.CLASE)).FirstOrDefault();
                                        if (da != null)
                                        {
                                            da.ACTIVO = false;
                                            db.Entry(da).State = EntityState.Modified;
                                        }
                                    }
                                    doc.STEP_WF = 1;
                                    doc.USUARIO_ID = User.Identity.Name;
                                    doc.PATH = path;
                                    doc.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(doc);
                                        db.SaveChanges();
                                    }
                                    catch
                                    {
                                        errorfiles = "" + filename;
                                    }
                                }
                            }
                        }
                        indexlabel++;
                        if (errorfiles != "")
                        {
                            errorMessage = new Cadena().concatena(errorMessage, "Error con el archivo " + errorfiles);
                        }



                    }
                    //LEJ 13.07.2018------------------------------------------------------
                    if (pos > 0)
                    {
                        string errorString = "";
                        var _cartap = db.CARTAPs.Where(p => p.NUM_DOC == num_doc && p.POS_ID == pos).ToList();
                        var _dp = db.DOCUMENTOPs.Where(d => d.NUM_DOC == num_doc).ToList();
                        List<DOCUMENTOREC> _dRE = db.DOCUMENTORECs.Where(d => d.NUM_DOC == num_doc).ToList();
                        DOCUMENTO _dc = db.DOCUMENTOes.Where(d => d.NUM_DOC == num_doc).FirstOrDefault();
                        SOCIEDAD id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == _dc.SOCIEDAD_ID).FirstOrDefault();
                        decimal MONTO_DOC_MD = 0;
                        for (int j = 0; j < _cartap.Count; j++)
                        {
                            ////var _xdp = _dp.Where(p => p.POS == _cartap[j].POS).FirstOrDefault();
                            //Comprobamos que sean la misma cantidad de registros
                            if (_cartap.Count == _dp.Count)//LEJ 17.07.2018-------------------
                            {
                                _dp[j].NUM_DOC = num_doc;
                                _dp[j].POS = _cartap[j].POS;
                                _dp[j].MATNR = _cartap[j].MATNR;
                                _dp[j].CANTIDAD = _cartap[j].CANTIDAD;
                                _dp[j].MONTO = _cartap[j].MONTO;
                                _dp[j].MONTO_APOYO = _cartap[j].MONTO_APOYO;
                                _dp[j].PRECIO_SUG = _cartap[j].PRECIO_SUG;
                                _dp[j].VOLUMEN_EST = _cartap[j].VOLUMEN_EST;
                                _dp[j].VOLUMEN_REAL = _cartap[j].VOLUMEN_REAL;
                                _dp[j].APOYO_REAL = _cartap[j].APOYO_REAL;
                                _dp[j].VIGENCIA_DE = _cartap[j].VIGENCIA_DE;
                                _dp[j].VIGENCIA_AL = _cartap[j].VIGENCIA_AL;
                                _dp[j].APOYO_EST = _cartap[j].APOYO_EST;
                                db.Entry(_dp[j]).State = EntityState.Modified;
                                db.SaveChanges();
                                ///////////////////Montos
                                //MONTO_DOC_MD

                                if (_dc.TSOL.FACTURA)
                                {
                                    MONTO_DOC_MD += decimal.Parse(_dp[j].APOYO_REAL.ToString());
                                }
                                else if (!_dc.TSOL.FACTURA)
                                {
                                    MONTO_DOC_MD += decimal.Parse(_dp[j].APOYO_EST.ToString());
                                }

                                if (_dc.TIPO_RECURRENTE == "1")
                                {
                                    foreach (DOCUMENTOREC dr in _dRE)
                                    {
                                        dr.MONTO_BASE = MONTO_DOC_MD;
                                        db.Entry(dr).State = EntityState.Modified;
                                    }
                                    db.SaveChanges();
                                }
                            }
                        }
                        //LEJ 17.07.2018-------------------
                        _dc.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);
                        try
                        {
                            //_dc.PORC_APOYO = decimal.Parse(bmonto_apoyo);
                        }
                        catch
                        {
                            //
                        }
                        //Obtener el monto de la sociedad
                        var MONTO_DOC_ML = MONTO_DOC_MD / _dc.TIPO_CAMBIOL;
                        _dc.MONTO_DOC_ML = MONTO_DOC_ML;
                        if (!errorString.Equals("")) //LEJ 13.07.2018
                        {
                            throw new Exception();
                        }

                        var _xx = _dc.TIPO_CAMBIOL2;
                        //MONTO_DOC_ML2 
                        var MONTO_DOC_ML2 = MONTO_DOC_MD / _xx;
                        _dc.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

                        //MONEDAL_ID moneda de la sociedad
                        _dc.MONEDAL_ID = id_bukrs.WAERS;

                        //MONEDAL2_ID moneda en USD
                        _dc.MONEDAL2_ID = "USD";

                        TCambio tcambio = new TCambio();//RSG 01.08.2018
                        //Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
                        _dc.TIPO_CAMBIOL = tcambio.getUkurs(id_bukrs.WAERS, _dc.MONEDA_ID, out errorString);

                        //Tipo cambio dolares TIPO_CAMBIOL2
                        _dc.TIPO_CAMBIOL2 = tcambio.getUkursUSD(_dc.MONEDA_ID, "USD", out errorString);
                        db.Entry(_dc).State = EntityState.Modified;
                        db.SaveChanges();
                        if (!errorString.Equals(""))
                        {
                            throw new Exception();
                        }

                        CARTA cartaSeleccionada = db.CARTAs.Where(c => c.POS == pos && c.NUM_DOC == num_doc).FirstOrDefault();
                        cartaSeleccionada.STATUS = true;
                        db.Entry(cartaSeleccionada).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    errorMessage = dir;
                }
                //LEJ 17.07.2018-------------------T
                ////}
                ////else
                ////    {
                ////        errorMessage = dir;
                ////    }

                ////errorString = errorMessage;
                //Guardar número de documento creado
                Session["ERROR_FILES"] = errorMessage;
            }
            if (errorMessage == "")
            {

                FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(num_doc)).OrderByDescending(a => a.POS).FirstOrDefault();
                FLUJO flujo = actual;
                flujo.ESTATUS = "A";
                flujo.FECHAM = DateTime.Now;
                flujo.COMENTARIO = "";
                flujo.USUARIOA_ID = User.Identity.Name;
                ProcesaFlujo pf = new ProcesaFlujo();
                string c = pf.procesa(flujo, "");
                if (res.Equals("1") | res.Equals("2") | res.Equals("3"))//CORREO
                {
                    //return RedirectToAction("Enviar", "Mails", new { id = flujo.NUM_DOC, index = false, tipo = "A" });
                    Email em = new Email();
                    DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == num_doc).First();
                    string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                    string image = Server.MapPath("~/images/logo_kellogg.png");
                    string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                    if (res.Equals("1") | res.Equals("2"))//CORREO
                    {
                        em.enviaMailC(flujo.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image, imageFlag);
                    }
                    else
                    {
                        em.enviaMailC(flujo.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Details", image, imageFlag);
                    }
                }

                using (TruequeEntities db1 = new TruequeEntities())
                {
                    FLUJO ff = db1.FLUJOes.Where(x => x.NUM_DOC == flujo.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    Estatus es = new Estatus();//RSG 18.09.2018
                    DOCUMENTO ddoc = db1.DOCUMENTOes.Find(flujo.NUM_DOC);
                    ff.STATUS = es.getEstatus(ddoc);
                    db1.Entry(ff).State = EntityState.Modified;
                    db1.SaveChanges();
                }

                if (c.Equals("0"))//Aprobado
                {
                    return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                }
                else
                {
                    TempData["error"] = c;
                    return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                }
            }
            //RECUPERO EL PAIS para hacer una busqueda de su formato monetario
            //var paisMon = Session["pais"].ToString();//------------------------LEJGG090718
            //ViewBag.miles = ",";
            //ViewBag.dec = ".";
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Taxeo()
        {
            decimal num_doc = decimal.Parse(Request.Form["D.NUM_DOC"].ToString());
            string conc = Request.Form["txt_concepto"].ToString();
            DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);
            d.CONCEPTO_ID = int.Parse(conc);
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();

            FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(num_doc)).OrderByDescending(a => a.POS).FirstOrDefault();
            ProcesaFlujo p = new ProcesaFlujo();
            f.ESTATUS = "A";
            p.procesa(f, "");


            return RedirectToAction("Details", new { id = num_doc });
        }

        // GET: Solicitudes
        public ActionResult Reversa(decimal id, decimal resto)
        {
            int pagina = 201; //ID EN BASE DE DATOS
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
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    ////return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }
            ViewBag.resto = Math.Round(resto, 2);
            DOCUMENTO d = db.DOCUMENTOes.Find(id);

            ViewBag.miles = d.PAI.MILES;//LEJGG 090718
            ViewBag.dec = d.PAI.DECIMAL;//LEJGG 090718

            return View(d);
        }

        public ActionResult Reversar(decimal id, string tsol)
        {
            Services.Reversa r = new Services.Reversa();
            decimal n_doc = 0;
            string a = r.creaReversa(id.ToString(), "RP", ref n_doc);
            ProcesaFlujo pf = new ProcesaFlujo();

            FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == n_doc).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
            while (a == "1")
            {
                Email em = new Email();
                DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == n_doc).First();
                string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                string[] ss = UrlDirectory.Split('/');
                UrlDirectory = "";
                for (int i = 0; i < ss.Length - 1; i++)
                {
                    UrlDirectory += ss[i] + "/";
                }
                UrlDirectory += n_doc;
                string image = Server.MapPath("~/images/logo_kellogg.png");
                string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                ////em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                em.enviaMailC(n_doc, true, Session["spras"].ToString(), UrlDirectory, "Index", image, imageFlag, 1);

                if (conta.WORKFP.ACCION.TIPO == "B")
                {
                    WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID && x.VERSION == conta.WF_VERSION && x.POS == conta.WF_POS).FirstOrDefault();
                    conta.ESTATUS = "A";
                    conta.FECHAM = DateTime.Now;
                    a = pf.procesa(conta, "");
                    conta = db.FLUJOes.Where(x => x.NUM_DOC == n_doc).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();

                }
                else
                {
                    a = "";
                }
            }

            return RedirectToAction("Index", "Home");
        }
        // GET: Solicitudes/Create
        [HttpGet]
        public ActionResult Create(string id_d, string tsol, string dp = null)
        {

            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();

            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            var relacionada_neg = "";
            var relacionada_dis = "";
            List<TSOLT_MODBD> tsols_valbd = new List<TSOLT_MODBD>();//RSG 13.06.2018

            DOCUMENTO d = new DOCUMENTO();
            string errorString = "";
            int pagina = 202; //ID EN BASE DE DATOS
            String res = "";//B20180611
            string unafactura = "false"; //MGC B20180625 MGC 
            string borrador = "false"; //MGC B20180625 MGC 
            string moneda_dis = "";//MGC B20180625 MGC 
            string id_waersf = "";//MGC B20180625 MGC 
            decimal bmonto_apoyo = 0;//MGC B20180625 MGC
            string notas_soporte = "";//MGC B20180625 MGC
            string tipo_cambio = "";//MGC B20180625 MGC
            string addrowst = "X"; //Add MGC B20180705 2018.07.05

            string usuariotextos = "";//B20180801 MGC Textos

            ////using (TruequeEntities db = new TruequeEntities())
            ////{
                string pais_id = "";
                string sociedad_id = "";
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);


                usuariotextos = user.SPRAS_ID;//B20180801 MGC Textos

                List<TREVERSAT> ldocr = new List<TREVERSAT>();
                decimal rel = 0;
                try
                {
                    if (id_d == null || id_d.Equals(""))
                    {
                        throw new Exception();
                    }
                    rel = Convert.ToDecimal(id_d);
                    ViewBag.relacionada = "prelacionada";
                    ViewBag.relacionadan = rel + "";
                    if (dp != "X")//ADD 31.10.2018
                    {
                        rel = Convert.ToDecimal(id_d);
                        ViewBag.relacionada = "prelacionada";
                        ViewBag.relacionadan = rel + "";
                    }
                    else//ADD 31.10.2018----------------------------i
                    {
                        ViewBag.relacionada = "";
                        ViewBag.relacionadan = "";
                    }//ADD 31.10.2018-------------------------------f

                }
                catch
                {
                    rel = 0;
                    ViewBag.relacionada = "";
                    ViewBag.relacionadan = "";
                }

                //Obtener los valores de tsols
                List<TSOL> tsols_val = new List<TSOL>();
                //List<TSOLT_MODBD> tsols_valbd = new List<TSOLT_MODBD>();//RSG 13.06.2018
                try
                {
                    tsols_val = db.TSOLs.ToList();
                    tsols_valbd = tsols_val.Select(tsv => new TSOLT_MODBD
                    {
                        ID = tsv.ID,
                        FACTURA = tsv.FACTURA
                    }).ToList();
                }
                catch (Exception e)
                {

                }
                //var tsols_valbdjs = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);//RSG 13.06.2018
                //ViewBag.TSOL_VALUES = tsols_valbdjs;

                //Add MGC B20180705 2018.07.05 conocer si se puede agregar renglones a la relacionada
                bool addon = true;
                try
                {
                    addon = tsols_val.Where(tsb => tsb.ID == tsol).FirstOrDefault().ADICIONA;
                }
                catch (Exception)
                {

                }
                if (addon == true)
                {
                    addrowst = "X";
                }
                else
                {
                    addrowst = "";
                }


                //Validar si es una reversa
                string isrn = "";
                string isr = "";
                var freversa = (dynamic)null;
                try
                {
                    if (tsol == null || tsol.Equals(""))
                    {
                        throw new Exception();
                    }
                    TSOL ts = tsols_val.Where(tsb => tsb.TSOLR == tsol).FirstOrDefault();
                    if (ts != null)
                    {
                        isrn = "X";
                        isr = "preversa";
                        freversa = theTime.ToString("dd/MM/yyyy");
                        //Obtener los tipos de reversas
                        try
                        {
                            ldocr = db.TREVERSATs.Where(a => a.TREVERSA.ACTIVO == true && a.SPRAS_ID == user.SPRAS_ID).ToList();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    isrn = "";
                    isr = "";
                    freversa = "";
                }

                ViewBag.reversa = isr;
                ViewBag.reversan = isrn;
                ViewBag.FECHAD_REV = freversa;
                ViewBag.TREVERSA = new SelectList(ldocr, "TREVERSA_ID", "TXT100");

                DOCUMENTBORR tmp = db.DOCUMENTBORRs.FirstOrDefault(x => x.USUARIOC_ID == user.ID && x.SOCIEDAD_ID == sociedad_id);//RSG 01.08.2018
                if (tmp != null)
                    Session["pais"] = tmp.PAIS_ID;

                if (dp == "X")//ADD 31.10.2018-------------------
                {
                    rel = decimal.Parse(id_d);
                    ViewBag.duplicate = rel;
                }//ADD 31.10.2018----------------------------
                if (rel == 0)
                {
                    try//RSG 15.05.2018
                    {
                        pais_id = Session["pais"].ToString();
                        sociedad_id = Session["sociedad_id"].ToString();
                        ViewBag.pais = pais_id + ".png";
                    }
                    catch
                    {
                        return RedirectToAction("Pais", "Home", new { returnUrl = Request.Url.AbsolutePath });//
                    }
                }
                Session["spras"] = user.SPRAS_ID;

                List<TSOLT_MOD> list_sol = new List<TSOLT_MOD>();
                //tipo de solicitud
                string tipS;
                if (id_d == null || id_d == "")
                {
                    tipS = "SD";
                    //directa SD
                }
                else if (dp == "X")//ADD RSG 07.11.2018
                {
                    DOCUMENTO duo = db.DOCUMENTOes.Find(decimal.Parse(id_d));
                    if (duo.DOCUMENTO_REF == null)
                        tipS = "SD";
                    else
                        tipS = "SR";
                }
                else
                {
                    tipS = "SR";
                    //relacionada SR
                }
                if (rel > 0)
                {
                    d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).FirstOrDefault();
                    sociedad_id = d.SOCIEDAD_ID;
                    pais_id = d.PAIS_ID;
                }
                if (dp == "X")//ADD 31.10.2018-------------------
                {
                    pais_id = db.DOCUMENTOes.Find(rel).PAIS_ID;
                    rel = 0;
                }//ADD 31.10.2018----------------------------
                if (ViewBag.reversa == "preversa")
                {
                    list_sol = tiposSolicitudesDao.ComboTiposSolicitudes(user.SPRAS_ID, null, true)
                        .Select(x => new TSOLT_MOD
                        {
                            SPRAS_ID = user.SPRAS_ID,
                            TSOL_ID = x.Value,
                            TEXT = x.Text
                        }).ToList();
                    ViewBag.listtreetsol = tiposSolicitudesDao.TreeTiposSolicitudes(sociedad_id, user.SPRAS_ID, null, true);
                }
                else
                {
                    list_sol = tiposSolicitudesDao.ComboTiposSolicitudes(user.SPRAS_ID, null)
                        .Select(x => new TSOLT_MOD
                        {
                            SPRAS_ID = user.SPRAS_ID,
                            TSOL_ID = x.Value,
                            TEXT = x.Text
                        }).ToList();
                    ViewBag.listtreetsol = tiposSolicitudesDao.TreeTiposSolicitudes(sociedad_id, user.SPRAS_ID, tipS);
                }

                //Obtener los documentos relacionados
                List<DOCUMENTO> docsrel = new List<DOCUMENTO>();

                SOCIEDAD id_bukrs = new SOCIEDAD();
                var id_pais = new PAI();

                List<TAT001.Models.GALL_MOD> list_grupo = new List<GALL_MOD>();
                //Grupos de solicitud
                list_grupo = db.GALLs.Where(g => g.ACTIVO == true)
                                .Join(
                                db.GALLTs.Where(gt => gt.SPRAS_ID == user.SPRAS_ID),
                                g => g.ID,
                                gt => gt.GALL_ID,
                                (g, gt) => new GALL_MOD
                                {
                                    SPRAS_ID = gt.SPRAS_ID,
                                    GALL_ID = gt.GALL_ID,
                                    TEXT = g.ID + " " + gt.TXT50
                                }).ToList();
                //clasificación
                //MGC B20180611
                List<TALLT_MOD> id_clas = tallsDao.ListaTallsConCuenta(TATConstantes.ACCION_LISTA_TALLTCONCUENTA, null, user.SPRAS_ID, pais_id, DateTime.Now.Year, sociedad_id)
                    .Select(x => new TALLT_MOD
                    {
                        SPRAS_ID = x.SPRAS_ID,
                        TALL_ID = x.TALL_ID,
                        TXT50 = x.TXT50
                    })
                            .ToList();
                id_clas = id_clas.OrderBy(x => x.TXT50).ToList();

                List<DOCUMENTOA> archivos = new List<DOCUMENTOA>();
                TAT001.Entities.TCAMBIO tcambio = new TAT001.Entities.TCAMBIO(); //MGC B20180625 MGC
                if (rel > 0)
                {
                    d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).FirstOrDefault();
                    docsrel = db.DOCUMENTOes.Where(docr => docr.DOCUMENTO_REF == rel && docr.ESTATUS_C == null).ToList();
                    id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == d.SOCIEDAD_ID && soc.ACTIVO == true).FirstOrDefault();
                    id_waersf = id_bukrs.WAERS;//MGC B20180625 MGC 
                    id_pais = db.PAIS.Where(pais => pais.LAND.Equals(d.PAIS_ID)).FirstOrDefault();//RSG 15.05.2018
                    d.DOCUMENTO_REF = rel;
                    relacionada_neg = d.TIPO_TECNICO;
                    if (d.DOCUMENTOLs.Count > 0 && (d.DOCUMENTOPs.First().MATNR != null && d.DOCUMENTOPs.First().MATNR != ""))
                        relacionada_neg = "M";
                    ViewBag.TSOL_ANT = d.TSOL_ID;
                    ViewBag.LIGADA = d.LIGADA;//RSG 09.07.2018
                    try//RSG 09.07.2018
                    {
                        bmonto_apoyo = (decimal)d.PORC_APOYO;
                    }
                    catch
                    { }

                    if (d != null)
                    {

                        d.TSOL_ID = tsol;
                        ViewBag.TSOL_ID = new SelectList(list_sol, "TSOL_ID", "TEXT", selectedValue: d.TSOL_ID);
                        ViewBag.GALL_ID = new SelectList(list_grupo, "GALL_ID", "TEXT", selectedValue: d.GALL_ID);
                        ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50", selectedValue: d.TALL_ID); //B20180618 v1 MGC 2018.06.18
                        TSOLT tsmod = new TSOLT();//RSG 30.07.2018
                        try
                        {
                            tsmod = db.TSOLTs.Where(x => x.TSOL_ID.Equals(d.TSOL_ID) && x.SPRAS_ID == user.SPRAS_ID).FirstOrDefault();//RSG 30.07.2018
                        }
                        catch
                        {
                            tsmod.TXT50 = "";
                        }
                        ViewBag.TSOL_IDI = tsmod.TXT50.ToString();
                        ViewBag.TALL_IDI = id_clas.Where(c => c.TALL_ID == d.TALL_ID).FirstOrDefault().TXT50; //B20180618 v1 MGC 2018.06.18
                        archivos = db.DOCUMENTOAs.Where(x => x.NUM_DOC.Equals(d.NUM_DOC)).Include(x => x.TSOPORTE).ToList();

                        List<DOCUMENTOP> docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == d.NUM_DOC).ToList();//Documentos que se obtienen de la provisión

                        List<DOCUMENTOM> docml = new List<DOCUMENTOM>();//MGC B20180611----------------------------
                        decimal totalcatrel = 0;//MGC B20180611
                        if (docpl.Count > 0)
                        {
                            docml = db.DOCUMENTOMs.Where(docm => docm.NUM_DOC == d.NUM_DOC).ToList();
                        }

                        List<DOCUMENTOP> docsrelp = new List<DOCUMENTOP>();
                        List<DOCUMENTOM> docsrelm = new List<DOCUMENTOM>();//MGC B20180611----------------------------
                        //Obtener los documentos de la relacionada
                        if (docsrel.Count > 0)
                        {
                            docsrelp = docsrel
                                //.Where(x => x.ESTATUS_C != "C")//ADD RSG 15.11.2018
                                .Where(x => x.ESTATUS_C != "C" && x.ESTATUS_WF != "B")//ADD RSG 15.11.2018
                                .Join(
                                db.DOCUMENTOPs,
                                docsl => docsl.NUM_DOC,
                                docspl => docspl.NUM_DOC,
                                (docsl, docspl) => new DOCUMENTOP
                                {
                                    NUM_DOC = docspl.NUM_DOC,
                                    POS = docspl.POS,
                                    MATNR = docspl.MATNR,
                                    MATKL = docspl.MATKL,
                                    CANTIDAD = docspl.CANTIDAD,
                                    MONTO = docspl.MONTO,
                                    PORC_APOYO = docspl.PORC_APOYO,
                                    MONTO_APOYO = docspl.MONTO_APOYO,
                                    PRECIO_SUG = docspl.PRECIO_SUG,
                                    VOLUMEN_EST = docspl.VOLUMEN_EST,
                                    VOLUMEN_REAL = docspl.VOLUMEN_REAL,
                                    APOYO_REAL = docspl.APOYO_REAL,
                                    APOYO_EST = docspl.APOYO_EST,
                                    VIGENCIA_DE = docspl.VIGENCIA_DE,
                                    VIGENCIA_AL = docspl.VIGENCIA_AL
                                }).ToList();

                            //MGC B20180611 Obtener los documentos en caso de distribución por categoría
                            if (docsrelp.Count > 0)
                            {
                                docsrelm = docsrelp
                                    .Join(
                                    db.DOCUMENTOMs,
                                    docspl => docspl.NUM_DOC,
                                    docsml => docsml.NUM_DOC,
                                    (docspl, docsml) => new DOCUMENTOM
                                    {
                                        NUM_DOC = docsml.NUM_DOC,
                                        POS_ID = docsml.POS_ID,
                                        POS = docsml.POS,
                                        MATNR = docsml.MATNR,
                                        PORC_APOYO = docsml.PORC_APOYO,
                                        APOYO_EST = docsml.APOYO_EST,
                                        APOYO_REAL = docsml.APOYO_REAL,
                                        VIGENCIA_DE = docsml.VIGENCIA_DE,
                                        VIGENCIA_A = docsml.VIGENCIA_A
                                    }).ToList();
                            }

                        }
                        //d.NUM_DOC = 0;
                        List<TAT001.Models.DOCUMENTOP_MOD> docsp = new List<DOCUMENTOP_MOD>();
                        decimal resta = 0;//ADD RSG 15.11.2018
                        var dis = "";
                        for (int j = 0; j < docpl.Count; j++)
                        {
                            try
                            {
                                //Documentos de la provisión
                                DOCUMENTOP_MOD docP = new DOCUMENTOP_MOD();
                                docP.NUM_DOC = d.NUM_DOC;
                                docP.POS = docpl[j].POS;
                                docP.MATNR = docpl[j].MATNR;
                                if (j == 0 && docP.MATNR == "")
                                {
                                    relacionada_dis = "C";
                                }
                                docP.MATKL = docpl[j].MATKL;
                                docP.MATKL_ID = docpl[j].MATKL;
                                docP.CANTIDAD = 1;
                                docP.MONTO = docpl[j].MONTO;
                                docP.PORC_APOYO = docpl[j].PORC_APOYO;
                                docP.MONTO_APOYO = docpl[j].MONTO_APOYO;
                                docP.PRECIO_SUG = docpl[j].PRECIO_SUG;
                                docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                                docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                                docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                                docP.APOYO_EST = docpl[j].APOYO_EST;
                                docP.APOYO_REAL = docpl[j].APOYO_REAL;

                                //Verificar si hay materiales en las relacionadas
                                if (docsrelp.Count > 0)
                                {
                                    List<DOCUMENTOP> docrel = new List<DOCUMENTOP>();

                                    if (docP.MATNR != null && docP.MATNR != "")
                                    {
                                        docrel = docsrelp.Where(docrell => docrell.MATNR == docP.MATNR).ToList();
                                    }
                                    else
                                    {
                                        docrel = docsrelp.Where(docrell => docrell.MATKL == docP.MATKL_ID).ToList();
                                        dis = "C";
                                    }

                                    for (int k = 0; k < docrel.Count; k++)
                                    {
                                        //Relacionada se obtiene el 
                                        decimal docr_vr = Convert.ToDecimal(docrel[k].VOLUMEN_REAL);
                                        decimal docr_ar = Convert.ToDecimal(docrel[k].APOYO_REAL);

                                        docP.VOLUMEN_EST -= docr_vr;
                                        docP.APOYO_EST -= docr_ar;

                                        if (dis == "C")
                                        {
                                            totalcatrel += Convert.ToDecimal(docrel[k].APOYO_REAL); //MGC B20180611
                                            //decimal docr_vr = Convert.ToDecimal(docrel[k].);
                                            //decimal docr_ar = Convert.ToDecimal(docrel[k].APOYO_REAL);
                                        }
                                        else if (tsmod.TSOL.REVERSO)
                                            totalcatrel += Convert.ToDecimal(docrel[k].APOYO_REAL); //MGC B20180611


                                    }
                                }

                                //Siempre tiene que ser igual a 0
                                if (docP.VOLUMEN_EST < 0)
                                {
                                    docP.VOLUMEN_EST = 0;
                                }
                                if (docP.APOYO_EST < 0)
                                {
                                    resta += (decimal)docP.APOYO_EST;//ADD RSG 15.11.2018
                                    docP.APOYO_EST = 0;
                                }

                                docP.MATNR = docpl[j].MATNR.TrimStart('0');//RSG 07.06.2018
                                docsp.Add(docP);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        //ADD RSG 15.11.2018------------------------------------I
                        for (int j = 0; j < docsrelp.Count; j++)
                        {
                            List<DOCUMENTOP> docrel = new List<DOCUMENTOP>();
                            docrel = docpl.Where(docrell => docrell.MATNR == docsrelp[j].MATNR).ToList();
                            if (docrel.Count == 0)
                            {
                                resta -= Convert.ToDecimal(docsrelp[j].APOYO_REAL);
                            }
                        }
                        //MGC B20180611 Obtener las categorias con el detalle de cada material
                        if (docml.Count > 0)
                        {
                            res = grupoMaterialesRel(docpl, docml);
                        }

                        //Restar el valor del documento menos los relacionados
                        d.MONTO_DOC_MD = d.MONTO_DOC_MD - totalcatrel;//MGC B20180611
                                                                      //if ((docsp.Count - mayores) > 0)
                                                                      //    resta = resta / (docsp.Count - mayores);
                                                                      ////while (resta != 0)

                        if (d.DOCUMENTOLs.Count > 0)
                        {
                            if (d.DOCUMENTOLs.First().BACKORDER == null)
                                d.DOCUMENTOLs.First().BACKORDER = 0;
                            d.MONTO_DOC_MD = (d.DOCUMENTOLs.First().MONTO_VENTA + d.DOCUMENTOLs.First().BACKORDER) * d.PORC_APOYO / 100;
                            ViewBag.caso2 = "X";
                            if (d.DOCUMENTOLs.First().BACKORDER > 0)
                            {
                                ViewBag.backorder = "X";
                            }
                        }
                        if (d.TIPO_RECURRENTE == "6")
                        {
                            ViewBag.caso1 = "X";
                        }

                        decimal suma = 0;
                        foreach (DOCUMENTOP_MOD ddp in docsp)
                        {
                            suma += (decimal)ddp.APOYO_EST;
                        }
                        if (suma > 0)
                            while (resta != 0)
                            {
                                foreach (DOCUMENTOP_MOD ddp in docsp)
                                {
                                    if (ddp.APOYO_EST > 0)
                                    {
                                        ddp.APOYO_EST += resta;
                                        if (ddp.APOYO_EST <= 0)
                                        {
                                            resta = (decimal)ddp.APOYO_EST;
                                            ddp.APOYO_EST = 0;
                                        }
                                        else
                                            resta = 0;
                                    }
                                }
                            }
                        //ADD RSG 15.11.2018------------------------------------F
                        d.DOCUMENTOP = docsp;
                    }
                }
                else
                {
                    //B20180625 MGC 2018.06.26 Verificar si hay algún borrador
                    DOCUMENTBORR docb = new DOCUMENTBORR();
                    try
                    {
                        docb = db.DOCUMENTBORRs.FirstOrDefault(x => x.USUARIOC_ID == user.ID && x.SOCIEDAD_ID == sociedad_id);
                        if (docb != null)
                        {
                            ViewBag.LIGADA = docb.LIGADA;//RSG 09.07.2018
                            pais_id = docb.PAIS_ID;//RSG 01.08.2018
                        }
                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Solicitudes", "Create");
                    }

                    id_pais = db.PAIS.Where(pais => pais.LAND.Equals(pais_id)).FirstOrDefault();//RSG 15.05.2018 //MGC B20180625 MGC 
                    id_bukrs = db.SOCIEDADs.FirstOrDefault(soc => soc.BUKRS.Equals(sociedad_id) && soc.ACTIVO);//RSG 15.05.2018 //MGC B20180625 MGC 
                    DET_APROBH dah = id_bukrs.DET_APROBH.FirstOrDefault(x => x.PUESTOC_ID == user.PUESTO_ID && x.ACTIVO);
                    if (dah == null) { Session["error"] = "Verifique el flujo de la sociedad: " + id_pais.SOCIEDAD_ID; Session["pais"] = null; return RedirectToAction("Index", "Home"); }
                    //if (docb != null)
                    if (docb != null || dp == "X")//ADD 31.10.2018
                    {

                        if (dp == "X")
                        {
                            rel = Convert.ToDecimal(id_d);
                            Duplicado dup = new Duplicado();
                            docb = dup.llenaDuplicado(db, rel, user.ID);
                            ViewBag.duplicado = true;
                            ViewBag.ligada = docb.LIGADA;
                        }
                        //Hay borrador 
                        borrador = "true";
                        d.TSOL_ID = docb.TSOL_ID;
                        ViewBag.TSOL_ID = new SelectList(list_sol, "TSOL_ID", "TEXT", selectedValue: docb.TSOL_ID);
                        //ViewBag.GALL_ID = new SelectList(list_grupo, "GALL_ID", "TEXT", selectedValue: docb.GALL_ID);
                        ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50", selectedValue: docb.TALL_ID); //B20180618 v1 MGC 2018.06.18
                        d.CIUDAD = docb.CIUDAD;
                        d.ESTADO = docb.ESTADO;
                        d.CONCEPTO = docb.CONCEPTO;
                        d.NOTAS = docb.NOTAS;
                        //d.PAYER_ID = docb.PAYER_ID;//RSG 20.09.2018 delete
                        if (docb.PAYER_ID != null)
                            d.PAYER_ID = docb.PAYER_ID.TrimStart('0');
                        d.FECHAI_VIG = docb.FECHAI_VIG;
                        d.FECHAF_VIG = docb.FECHAF_VIG;
                        d.PAYER_EMAIL = docb.PAYER_EMAIL;
                        d.PAYER_NOMBRE = docb.PAYER_NOMBRE;
                        d.MONTO_DOC_MD = docb.MONTO_DOC_MD;

                        relacionada_dis = docb.TIPO_TECNICO2;
                        relacionada_neg = docb.TIPO_TECNICO;
                        moneda_dis = docb.MONEDA_DIS;

                        id_waersf = docb.MONEDA_ID;//MGC B20180625 MGC
                        bmonto_apoyo = Convert.ToDecimal(docb.PORC_APOYO);//MGC B20180625 MGC
                        //Obtener las facturas del borrador
                        //List<DOCUMENTOBORRF> lbf = new List<DOCUMENTOBORRF>();
                        //lbf = db.DOCUMENTOBORRFs.Where(lb => lb.USUARIOC_ID == user.ID).ToList();
                        List<DOCUMENTOF> docfl = new List<DOCUMENTOF>();
                        for (int j = 0; j < docb.DOCUMENTOBORRFs.Count; j++)
                        {
                            //Definir si son varias facturas o una
                            if (j > 0)
                            {
                                unafactura = docb.DOCUMENTOBORRFs.ElementAt(j).ACTIVO + "";
                                unafactura = unafactura.ToLower();
                            }
                            DOCUMENTOF docf = new DOCUMENTOF();

                            docf.POS = j + 1;
                            docf.FACTURA = docb.DOCUMENTOBORRFs.ElementAt(j).FACTURA;
                            docf.SOCIEDAD = docb.DOCUMENTOBORRFs.ElementAt(j).SOCIEDAD;//jemo 06-08-2018
                            docf.FECHA = docb.DOCUMENTOBORRFs.ElementAt(j).FECHA;
                            docf.PROVEEDOR = docb.DOCUMENTOBORRFs.ElementAt(j).PROVEEDOR;
                            docf.CONTROL = docb.DOCUMENTOBORRFs.ElementAt(j).CONTROL;
                            docf.AUTORIZACION = docb.DOCUMENTOBORRFs.ElementAt(j).AUTORIZACION;
                            docf.VENCIMIENTO = docb.DOCUMENTOBORRFs.ElementAt(j).VENCIMIENTO;
                            docf.FACTURAK = docb.DOCUMENTOBORRFs.ElementAt(j).FACTURAK;
                            docf.EJERCICIOK = docb.DOCUMENTOBORRFs.ElementAt(j).EJERCICIOK;
                            docf.BILL_DOC = docb.DOCUMENTOBORRFs.ElementAt(j).BILL_DOC;
                            docf.BELNR = docb.DOCUMENTOBORRFs.ElementAt(j).BELNR;
                            docf.IMPORTE_FAC = docb.DOCUMENTOBORRFs.ElementAt(j).IMPORTE_FAC; //jemo 18-07-2018
                            docf.PAYER = docb.DOCUMENTOBORRFs.ElementAt(j).PAYER;//jemo 18-07-2018
                            docf.DESCRIPCION = docb.DOCUMENTOBORRFs.ElementAt(j).NAME1;//jemo 18-07-2018
                            docfl.Add(docf);
                        }
                        d.DOCUMENTOF = docfl;

                        //RSG add 20.09.2018------------------------------------------
                        d.DOCUMENTOREC = new List<DOCUMENTOREC>();
                        foreach (DOCUMENTOBORRREC drec in docb.DOCUMENTOBORRRECs)
                        {
                            DOCUMENTOREC dbp = new DOCUMENTOREC();

                            //dbp.USUARIOC_ID = doc.USUARIOC_ID;
                            dbp.POS = drec.POS;

                            dbp.DOC_REF = drec.DOC_REF;
                            dbp.EJERCICIO = drec.EJERCICIO;
                            dbp.ESTATUS = drec.ESTATUS;
                            dbp.FECHAF = drec.FECHAF;
                            dbp.FECHAV = drec.FECHAV;
                            dbp.MONTO_BASE = drec.MONTO_BASE;
                            dbp.MONTO_FIJO = drec.MONTO_FIJO;
                            dbp.MONTO_GRS = drec.MONTO_GRS;
                            dbp.MONTO_NET = drec.MONTO_NET;
                            dbp.PERIODO = drec.PERIODO;
                            dbp.PORC = drec.PORC;

                            d.DOCUMENTOREC.Add(dbp);
                        }
                        //RSG add 20.09.2018------------------------------------------

                        //Obtener las posiciones del borrador
                        List<DOCUMENTOP_MOD> docpl = new List<DOCUMENTOP_MOD>();
                        for (int j = 0; j < docb.DOCUMENTOBORRPs.Count; j++)
                        {
                            DOCUMENTOP_MOD docp = new DOCUMENTOP_MOD();

                            docp.POS = j + 1;
                            docp.MATNR = docb.DOCUMENTOBORRPs.ElementAt(j).MATNR;
                            docp.MATKL = docb.DOCUMENTOBORRPs.ElementAt(j).MATKL;
                            docp.MATKL_ID = docb.DOCUMENTOBORRPs.ElementAt(j).MATKL;
                            docp.DESC = "";
                            docp.CANTIDAD = 1;
                            docp.MONTO = Convert.ToDecimal(docb.DOCUMENTOBORRPs.ElementAt(j).MONTO);
                            docp.PORC_APOYO = Convert.ToDecimal(docb.DOCUMENTOBORRPs.ElementAt(j).PORC_APOYO);
                            docp.MONTO_APOYO = Convert.ToDecimal(docb.DOCUMENTOBORRPs.ElementAt(j).MONTO_APOYO);
                            //docp.MONTOC_APOYO = docb.DOCUMENTOBORRPs.ElementAt(j).;
                            //docp.PORC_APOYOEST = docb.DOCUMENTOBORRPs.ElementAt(j).BILL_DOC;
                            docp.PRECIO_SUG = Convert.ToDecimal(docb.DOCUMENTOBORRPs.ElementAt(j).PRECIO_SUG);
                            docp.VOLUMEN_EST = Convert.ToDecimal(docb.DOCUMENTOBORRPs.ElementAt(j).VOLUMEN_EST);
                            //docp.VOLUMEN_REAL = docb.DOCUMENTOBORRPs.ElementAt(j).BELNR;
                            docp.VIGENCIA_DE = docb.DOCUMENTOBORRPs.ElementAt(j).VIGENCIA_DE;
                            docp.VIGENCIA_AL = docb.DOCUMENTOBORRPs.ElementAt(j).VIGENCIA_AL;
                            docp.APOYO_EST = docb.DOCUMENTOBORRPs.ElementAt(j).APOYO_EST;
                            docp.APOYO_REAL = docb.DOCUMENTOBORRPs.ElementAt(j).APOYO_REAL;

                            docpl.Add(docp);
                        }

                        d.DOCUMENTOP = docpl;

                        //MGC B20180625 MGC
                        //Obtener las notas de soporte
                        try
                        {
                            notas_soporte = db.DOCUMENTOBORRNs.Where(dn => dn.USUARIOC_ID == user.ID).FirstOrDefault().TEXTO.ToString();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        id_waersf = id_bukrs.WAERS;//MGC B20180625 MGC 
                        ViewBag.TSOL_ID = new SelectList(list_sol, "TSOL_ID", "TEXT");
                        ViewBag.GALL_ID = new SelectList(list_grupo, "GALL_ID", "TEXT");
                        ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50"); //B20180618 v1 MGC 2018.06.18
                                                                                       //id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p) && soc.ACTIVO == true).FirstOrDefault();//RSG 15.05.2018                      
                    }

                    ViewBag.TSOL_ANT = "";
                    ViewBag.TSOL_IDI = "";
                    ViewBag.GALL_IDI = "";
                    ViewBag.TALL_IDI = ""; //B20180618 v1 MGC 2018.06.18
                }
                //ADD RSG 09.11.2018---------------------------------------------I
                if (dp != null)
                {
                    List<DOCUMENTOP> docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == d.NUM_DOC).ToList();//Documentos que se obtienen de la provisión     
                    List<DOCUMENTOM> docml = new List<DOCUMENTOM>();//MGC B20180611----------------------------                   
                    if (docpl.Count > 0)
                    {
                        docml = db.DOCUMENTOMs.Where(docm => docm.NUM_DOC == d.NUM_DOC).ToList();
                    }
                    //MGC B20180611 Obtener las categorias con el detalle de cada material
                    if (docml.Count > 0)
                    {
                        res = grupoMaterialesRel(docpl, docml);
                    }
                }
                //ADD RSG 09.11.2018---------------------------------------------F

                ViewBag.files = archivos;

                var id_waers = db.MONEDAs.Where(m => m.ACTIVO == true && (m.WAERS.Equals(id_bukrs.WAERS) | m.WAERS.Equals("USD"))).ToList();//RSG 01.08.2018
                var id_states = (from st in db.STATES
                                 join co in db.COUNTRIES
                                 on st.COUNTRY_ID equals co.ID
                                 where co.SORTNAME.Equals(id_pais.LAND)
                                 select new
                                 {
                                     st.ID,
                                     st.NAME,
                                     st.COUNTRY_ID
                                 }).ToList();



                List<TAT001.Entities.CITy> id_city = new List<TAT001.Entities.CITy>();

                ViewBag.SOCIEDAD_ID = id_bukrs;
                ViewBag.PAIS_ID = id_pais;
                ViewBag.STATE_ID = "";// new SelectList(id_states, "ID", dataTextField: "NAME");
                ViewBag.CITY_ID = "";// new SelectList(id_city, "ID", dataTextField: "NAME");
                ViewBag.MONEDA = new SelectList(id_waers, "WAERS", dataTextField: "WAERS", selectedValue: id_waersf); //Duda si cambia en la relacionada //B20180625 MGC 2018.06.28

                //Información del cliente
                var id_clientes = db.CLIENTEs.Where(c => c.LAND.Equals(pais_id) && c.ACTIVO == true).ToList();

                ViewBag.PAYER_ID = new SelectList(id_clientes, "KUNNR", dataTextField: "NAME1");

                //Información de categorías
                var id_cat = db.CATEGORIAs.Where(c => c.ACTIVO == true)
                                .Join(
                                db.CATEGORIATs.Where(ct => ct.SPRAS_ID == user.SPRAS_ID),
                                c => c.ID,
                                ct => ct.CATEGORIA_ID,
                                (c, ct) => new
                                {
                                    ct.CATEGORIA_ID,
                                    TEXT = ct.TXT50
                                }).ToList();

                id_cat.RemoveRange(0, id_cat.Count);//RSG 28.05.2018
                ViewBag.CATEGORIA_ID = new SelectList(id_cat, "CATEGORIA_ID", "TEXT");
                List<TAT001.Entities.CITy> id_cityy = new List<TAT001.Entities.CITy>();
                ViewBag.BASE_ID = new SelectList(id_cityy, "CATEGORIA_ID", "TEXT");

                d.SOCIEDAD_ID = id_bukrs.BUKRS;
                d.PAIS_ID = id_pais.LAND;//RSG 18.05.2018
                d.MONEDA_ID = id_waersf; //B20180625 MGC 2018.07.02
                var date = DateTime.Now.Date;
                try
                {
                    tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(id_waersf) && t.TCURR.Equals("USD") && t.GDATU.Equals(date)).FirstOrDefault();//B20180625 MGC 2018.07.02
                    if (tcambio == null)
                    {
                        var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(id_waersf) && t.TCURR.Equals("USD")).Max(a => a.GDATU);//B20180625 MGC 2018.07.02
                        tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(id_waersf) && t.TCURR.Equals("USD") && t.GDATU.Equals(max)).FirstOrDefault();//B20180625 MGC 2018.07.02
                    }
                    decimal con = Convert.ToDecimal(tcambio.UKURS);
                    var cons = con.ToString("0.#####"); //OCG 23/10/18

                    //ViewBag.tcambio = cons;
                    tipo_cambio = cons; //MGC B20180625 MGC
                }
                catch (Exception e)
                {
                    errorString = e.Message + "detail: conversion " + id_bukrs.WAERS + " to " + "USD" + " in date " + DateTime.Now.Date;
                    //ViewBag.tcambio = "";
                    tipo_cambio = ""; //MGC B20180625 MGC
                }


            ////}//RSG 13.06.2018
            Calendario445 cal = new Calendario445();
            d.PERIODO = cal.getPeriodo(DateTime.Now);
            d.EJERCICIO = cal.getEjercicio(DateTime.Now) + "";

            d.FECHAD = theTime;
            ViewBag.FECHAD = theTime.ToString("dd/MM/yyyy");
            ViewBag.PERIODO = d.PERIODO;
            ViewBag.EJERCICIO = d.EJERCICIO;
            ViewBag.STCD1 = "";
            ViewBag.PARVW = "";
            ViewBag.UNAFACTURA = unafactura; //MGC B20180625
            ViewBag.MONTO_DOC_ML2 = "";
            ViewBag.error = errorString;
            ViewBag.NAME1 = "";
            ViewBag.notas_soporte = notas_soporte; //MGC B20180625 MGC
            ViewBag.tcambio = tipo_cambio;//MGC B20180625 MGC
            d.TIPO_CAMBIO = decimal.Parse(tipo_cambio);//MGC B20180625 MGC



            ViewBag.SEL_NEG = relacionada_neg;
            ViewBag.SEL_DIS = relacionada_dis;
            ViewBag.BMONTO_APOYO = bmonto_apoyo; //MGC B20180625 MGC 
            ViewBag.CATMAT = res; //B20180618 v1 MGC 2018.06.18
            ViewBag.MONTO_DIS = "";
            ViewBag.borrador = borrador; //MGC B20180625 MGC 
            ViewBag.borradore = borrador; //B20180625 MGC2 2018.07.04
            ViewBag.moneda_dis = moneda_dis;//MGC B20180625 MGC 

            ViewBag.addrowt = addrowst; //Add MGC B20180705 2018.07.05 conocer si se puede agregar renglones a la relacionada

            //----------------------------RSG 18.05.2018
            string spras = Session["spras"].ToString();
            ViewBag.PERIODOS = new SelectList(db.PERIODOTs.Where(a => a.SPRAS_ID == spras).ToList(), "PERIODO_ID", "TXT50", DateTime.Now.Month);
            List<string> anios = new List<string>();
            int mas = 10;
            for (int i = 0; i < mas; i++)
            {
                //anios.Add((DateTime.Now.Year + i).ToString());
                if (dp == "X")
                    anios.Add((d.FECHAI_VIG.Value.Year + i).ToString());
                else
                    anios.Add((DateTime.Now.Year + i).ToString());
            }
            string selYear1 = "";//ADD RSG 04.11.2018-------------------------------
            string selYear2 = "";
            if (dp != "X")
            {
                selYear1 = DateTime.Now.Year.ToString();//ADD RSG 04.11.2018-------------------------------
                selYear2 = DateTime.Now.Year.ToString();
            }
            else
            {
                selYear1 = d.FECHAI_VIG.Value.Year.ToString();//ADD RSG 04.11.2018-------------------------------
                selYear2 = d.FECHAF_VIG.Value.Year.ToString();
            }

            if (dp == "X")
            {
                if (cal.anioMas(d.FECHAI_VIG.Value)) selYear1 = (d.FECHAI_VIG.Value.Year + 1).ToString();
                if (cal.anioMas(d.FECHAF_VIG.Value)) selYear2 = (d.FECHAF_VIG.Value.Year + 1).ToString();//ADD RSG 04.11.2018-------------------------------
            }
            //ViewBag.ANIOS = new SelectList(anios, DateTime.Now.Year.ToString());
            ViewBag.ANIOS = new SelectList(anios, selYear1);//ADD RSG 31.10.2018
            ViewBag.ANIOSF = new SelectList(anios, selYear2);//ADD RSG 31.10.2018
            d.SOCIEDAD = db.SOCIEDADs.Find(d.SOCIEDAD_ID);
            //----------------------------RSG 18.05.2018
            //----------------------------RSG 12.06.2018
            if (id_d != null)
            {
                decimal numPadre = decimal.Parse(id_d);
                DOCUMENTO padre = db.DOCUMENTOes.Find(numPadre);
                if (padre != null)
                {
                    ViewBag.original = padre.MONTO_DOC_MD;
                    List<DOCUMENTO> dd = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == padre.NUM_DOC && a.ESTATUS_C == null).ToList();
                    ViewBag.sumaRel = decimal.Parse("0.00000"); ;
                    foreach (DOCUMENTO dos in dd)
                    {
                        ViewBag.sumaRel += (decimal)dos.MONTO_DOC_MD;
                    }
                }
            }
            //----------------------------RSG 12.06.2018


            //RSG 13.06.2018--------------------------------------------------------
            DateTime fecha = DateTime.Now.Date;
            List<DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO == true).ToList();
            if (del.Count > 0)
            {

                List<Delegados> users = new List<Delegados>();

                List<Delegados> delegados = new List<Delegados>();
                foreach (DELEGAR de in del)
                {

                    Delegados delegado = new Delegados();
                    delegado.usuario = de.USUARIO_ID;
                    delegado.nombre = de.USUARIO_ID + " - " + de.USUARIO.NOMBRE + " " + de.USUARIO.APELLIDO_P + " " + de.USUARIO.APELLIDO_M;
                    delegado.LISTA = new List<PAI>();
                    delegados.Add(delegado);
                }
                Delegados del1 = new Delegados();
                del1.usuario = User.Identity.Name;
                USUARIO uu = db.USUARIOs.Find(User.Identity.Name);
                del1.nombre = User.Identity.Name + " - " + uu.NOMBRE + " " + uu.APELLIDO_P + " " + uu.APELLIDO_M;
                del1.LISTA = new List<PAI>();
                users.Add(del1);
                foreach (Delegados dele in delegados)
                {
                    PAI pqq = dele.LISTA.Where(a => a.LAND == d.PAIS_ID).FirstOrDefault();
                    users.Add(dele);
                }

                ViewBag.USUARIOD_ID = new SelectList(users, "usuario", "nombre", users[0].usuario);
            }
            List<DELEGAR> backup = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(User.Identity.Name) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO == true).ToList();
            if (backup.Count > 0)
            {
                ViewBag.USUARIO_BACKUPID = backup.First().USUARIOD_ID;
            }

            var tsols_valbdjs = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);
            ViewBag.TSOL_VALUES = tsols_valbdjs;
            List<FACTURASCONF> ffc = db.FACTURASCONFs.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) && a.PAIS_ID.Equals(d.PAIS_ID)).ToList();
            foreach (var item in tsols_valbd)
            {
                FACTURASCONF fc = ffc.FirstOrDefault(a => a.TSOL.Equals(item.ID));
                if (fc == null)
                    item.FACTURA = false;
            }
            ViewBag.TSOL_VALUES2 = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);
            //RSG 13.06.2018--------------------------------------------------------
            //}//RSG 13.06.2018--------------------------------------------------------
            //RECUPERO EL PAIS para hacer una busqueda de su formato monetario


            //B20180801 MGC Textos....................................................................................................
            string txt_volumenr = "";//B20180801 MGC Textos
            string txt_apoyor = "";//B20180801 MGC Textos
            string txt_volumene = "";//B20180801 MGC Textos
            string txt_apoyoe = "";//B20180801 MGC Textos

            try
            {
                txt_volumenr = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(usuariotextos) && a.CAMPO_ID.Equals("thead_disvolumenReal")).FirstOrDefault().TEXTOS;
            }
            catch (Exception)
            {

            }
            try
            {
                txt_volumene = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(usuariotextos) && a.CAMPO_ID.Equals("thead_disvolumenEst")).FirstOrDefault().TEXTOS;
            }
            catch (Exception)
            {

            }
            try
            {
                txt_apoyor = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(usuariotextos) && a.CAMPO_ID.Equals("thead_disapoyoReal")).FirstOrDefault().TEXTOS;
            }
            catch (Exception)
            {

            }
            try
            {
                txt_apoyoe = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(usuariotextos) && a.CAMPO_ID.Equals("thead_disapoyoEst")).FirstOrDefault().TEXTOS;
            }
            catch (Exception)
            {

            }

            ViewBag.apoyor = txt_apoyor;
            ViewBag.volumenr = txt_volumenr;
            ViewBag.apoyoe = txt_apoyoe;
            ViewBag.volumene = txt_volumene;
            //B20180801 MGC Textos....................................................................................................
            d.PAI = db.PAIS.Where(a => a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
            if (d.PAI != null)
            {
                ViewBag.miles = d.PAI.MILES;//LEJGG 090718
                ViewBag.dec = d.PAI.DECIMAL;//LEJGG 090718
            }
            //-----------------------------------------------------------------LEJ 09.07.18
            ViewBag.horaServer = DateTime.Now.Date.ToString().Split(new[] { ' ' }, 2)[1];//RSG 01.08.2018


            Warning w = new Warning();
            ViewBag.listaValid = w.listaW(d.SOCIEDAD_ID, usuariotextos);

            var aa = (from n in db.TSOLTs.Where(x => x.SPRAS_ID == usuariotextos)
                      join t in db.TSOL_TREE
                      on n.TSOL_ID equals t.TSOL_ID
                      where n.TSOL.FACTURA && !n.TSOL_ID.StartsWith("O")
                      select new { n.TSOL_ID, n.TXT020 }).DistinctBy(x => x.TSOL_ID).DistinctBy(x => x.TXT020).ToList();
            ViewBag.TSOL_LIG = new SelectList(aa, "TSOL_ID", "TXT020", d.TSOL_LIG);

            return View(d);
        }

        // POST: Solicitudes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NUM_DOC,TSOL_ID,TALL_ID,SOCIEDAD_ID,PAIS_ID,ESTADO,CIUDAD,PERIODO," +
            "EJERCICIO,TIPO_TECNICO,TIPO_RECURRENTE,CANTIDAD_EV,USUARIOC_ID,FECHAD,FECHAC,ESTATUS,ESTATUS_C,ESTATUS_SAP," +
            "ESTATUS_WF,DOCUMENTO_REF,NOTAS,MONTO_DOC_MD,MONTO_FIJO_MD,MONTO_BASE_GS_PCT_MD,MONTO_BASE_NS_PCT_MD,MONTO_DOC_ML," +
            "MONTO_FIJO_ML,MONTO_BASE_GS_PCT_ML,MONTO_BASE_NS_PCT_ML,MONTO_DOC_ML2,MONTO_FIJO_ML2,MONTO_BASE_GS_PCT_ML2," +
            "MONTO_BASE_NS_PCT_ML2,IMPUESTO,FECHAI_VIG,FECHAF_VIG,ESTATUS_EXT,SOLD_TO_ID,PAYER_ID,GRUPO_CTE_ID,CANAL_ID," +
            "MONEDA_ID,TIPO_CAMBIO,NO_FACTURA,FECHAD_SOPORTE,METODO_PAGO,NO_PROVEEDOR,PASO_ACTUAL,AGENTE_ACTUAL,FECHA_PASO_ACTUAL," +
            "VKORG,VTWEG,SPART,HORAC,FECHAC_PLAN,FECHAC_USER,HORAC_USER,CONCEPTO,PORC_ADICIONAL,PAYER_NOMBRE,PAYER_EMAIL," +
            "MONEDAL_ID,MONEDAL2_ID,TIPO_CAMBIOL,TIPO_CAMBIOL2,DOCUMENTOP, DOCUMENTOF, DOCUMENTOREC, GALL_ID, USUARIOD_ID, OBJQ_PORC, DOCUMENTORAN,TSOL_LIG")] DOCUMENTO dOCUMENTO,
                IEnumerable<HttpPostedFileBase> files_soporte, string notas_soporte, string[] labels_soporte, string unafact,
                string FECHAD_REV, string TREVERSA, string select_neg, string select_dis, string select_negi, string select_disi,
                string bmonto_apoyo, string catmat, string borrador_param, string monedadis, string chk_ligada, string sel_nn, string check_objetivoq,
                string lbl_volr, string lbl_apr, string lbl_vole, string lbl_ape, string txtPres, string txt_flujo) //B20180801 MGC Textos
        {
            string errorString = "";
            SOCIEDAD id_bukrs = new SOCIEDAD();
            Calendario445 cal = new Calendario445();
            bool test = true;
            string p = "";
            string rele = ""; //Add MGC B20180705 2018.07.05 relacionadaed editar el material en los nuevos renglones
            decimal monto_ret = Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD);
            string usuariotextos = "";
            if (test)
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        p = dOCUMENTO.PAIS_ID;
                        ViewBag.pais = p + ".png";

                        //B20180618 v1 MGC 2018.06.18
                        //Obtener el GALL_ID
                        if (dOCUMENTO.TALL_ID != null && db.TALLs.Any(t => t.ID == dOCUMENTO.TALL_ID))
                        {
                            dOCUMENTO.GALL_ID = db.TALLs.Where(t => t.ID == dOCUMENTO.TALL_ID).FirstOrDefault().GALL_ID;
                        }

                        //OCG 21/10/18
                        CUENTA cu = db.CUENTAs.Where(x => x.TALL_ID == dOCUMENTO.TALL_ID && x.SOCIEDAD_ID == dOCUMENTO.SOCIEDAD_ID).FirstOrDefault();
                        if (cu == null)
                        {
                            errorString = "Sin Cuentas relacionadas";
                        }
                        dOCUMENTO.CUENTAP = cu.ABONO;
                        dOCUMENTO.CUENTAPL = cu.CARGO;
                        dOCUMENTO.CUENTACL = cu.CLEARING;
                        //OCG 21/10/18

                        DOCUMENTO d = new DOCUMENTO();
                        if (dOCUMENTO.DOCUMENTO_REF > 0)
                        {
                            d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).FirstOrDefault();
                            id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == d.SOCIEDAD_ID).FirstOrDefault();
                            dOCUMENTO.ESTADO = d.ESTADO;
                            dOCUMENTO.CIUDAD = d.CIUDAD;
                            dOCUMENTO.PAYER_ID = d.PAYER_ID;
                            dOCUMENTO.CONCEPTO = d.CONCEPTO;
                            //dOCUMENTO.NOTAS = d.NOTAS;
                            dOCUMENTO.FECHAI_VIG = d.FECHAI_VIG;
                            dOCUMENTO.FECHAF_VIG = d.FECHAF_VIG;
                            dOCUMENTO.PAYER_NOMBRE = d.PAYER_NOMBRE;
                            dOCUMENTO.PAYER_EMAIL = d.PAYER_EMAIL;
                            dOCUMENTO.TIPO_CAMBIO = d.TIPO_CAMBIO;
                            dOCUMENTO.GALL_ID = d.GALL_ID;
                            dOCUMENTO.TALL_ID = d.TALL_ID;//RSG 12.06.2018
                                                          //Obtener el país
                            dOCUMENTO.PAIS_ID = d.PAIS_ID;//RSG 15.05.2018
                            dOCUMENTO.TIPO_TECNICO = d.TIPO_TECNICO; //B20180618 v1 MGC 2018.06.18

                            rele = "X";//Add MGC B20180705 2018.07.05 relacionadaed editar el material en los nuevos renglones
                        }
                        else
                        {
                            id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p)).FirstOrDefault();
                            //Obtener el país
                            dOCUMENTO.PAIS_ID = p.ToUpper();//RSG 15.05.2018
                        }
                        //Tipo técnico
                        if (select_neg != null)//RSG 03.11.2018
                            dOCUMENTO.TIPO_TECNICO = select_neg;
                        else if (select_negi != null)//RSG 03.11.2018
                            dOCUMENTO.TIPO_TECNICO = select_negi;
                        if (chk_ligada == "on")
                            dOCUMENTO.TIPO_TECNICO = "P";

                        dOCUMENTO.OBJETIVOQ = false;//RSG 01.08.2018----------------add start
                        if (check_objetivoq == "on")
                            dOCUMENTO.OBJETIVOQ = true;
                        if ((bool)dOCUMENTO.OBJETIVOQ)
                            dOCUMENTO.OBJQ_PORC = dOCUMENTO.OBJQ_PORC;
                        if (sel_nn != null)
                            dOCUMENTO.FRECUENCIA_LIQ = int.Parse(sel_nn);//RSG 01.08.2018----------------add end

                        dOCUMENTO.EXCEDE_PRES = txtPres;//RSG 18.09.2018

                        ////Obtener el número de documento
                        //decimal N_DOC = getSolID(dOCUMENTO.TSOL_ID);
                        //dOCUMENTO.NUM_DOC = N_DOC;

                        //Obtener SOCIEDAD_ID                     
                        dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;

                        ////Obtener el país
                        //dOCUMENTO.PAIS_ID = p.ToUpper();

                        //B20180625 MGC 2018.06.25
                        //CANTIDAD_EV > 1 si son recurrentes
                        dOCUMENTO.CANTIDAD_EV = 1;

                        //B20180625 MGC 2018.06.25
                        //Obtener usuarioc
                        USUARIO u = db.USUARIOs.Find(User.Identity.Name);//RSG 02/05/2018
                        dOCUMENTO.PUESTO_ID = u.PUESTO_ID;//RSG 02/05/2018
                        dOCUMENTO.USUARIOC_ID = User.Identity.Name;
                        if (dOCUMENTO.USUARIOD_ID == null)
                            dOCUMENTO.USUARIOD_ID = User.Identity.Name;

                        //Fechac
                        dOCUMENTO.FECHAC = DateTime.Now;

                        //Horac
                        dOCUMENTO.HORAC = DateTime.Now.TimeOfDay;

                        //FECHAC_PLAN
                        dOCUMENTO.FECHAC_PLAN = DateTime.Now.Date;

                        //FECHAC_USER
                        dOCUMENTO.FECHAC_USER = DateTime.Now.Date;

                        //HORAC_USER
                        dOCUMENTO.HORAC_USER = DateTime.Now.TimeOfDay;

                        //Estatus
                        dOCUMENTO.ESTATUS = "N";

                        //Estatus wf
                        //dOCUMENTO.ESTATUS_WF = "P";//ADD RSG 30.10.2018
                        dOCUMENTO.ESTATUS_WF = txt_flujo;

                        ///////////////////Montos
                        //MONTO_DOC_MD
                        var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
                        dOCUMENTO.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);
                        try
                        {
                            dOCUMENTO.PORC_APOYO = decimal.Parse(bmonto_apoyo);
                        }
                        catch
                        {

                        }
                        TCambio tcambio = new TCambio();//RSG 01.08.2018
                                                        //Obtener el monto de la sociedad
                        dOCUMENTO.MONTO_DOC_ML = tcambio.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);
                        //if (!errorString.Equals("") && !borrador_param.Equals("borrador")) //B20180625 MGC 2018.07.03
                        if (!errorString.Equals("")) //B20180625 MGC 2018.07.03
                        {
                            throw new Exception();
                        }

                        //MONTO_DOC_ML2 
                        var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_ML2;
                        dOCUMENTO.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

                        //MONEDAL_ID moneda de la sociedad
                        dOCUMENTO.MONEDAL_ID = id_bukrs.WAERS;

                        //MONEDAL2_ID moneda en USD
                        dOCUMENTO.MONEDAL2_ID = "USD";

                        //Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
                        dOCUMENTO.TIPO_CAMBIOL = tcambio.getUkurs(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, out errorString);

                        //Tipo cambio dolares TIPO_CAMBIOL2
                        dOCUMENTO.TIPO_CAMBIOL2 = tcambio.getUkursUSD(dOCUMENTO.MONEDA_ID, "USD", out errorString);
                        if (!errorString.Equals(""))
                        {
                            throw new Exception();
                        }
                        //Obtener datos del payer
                        Cadena cad = new Cadena();
                        dOCUMENTO.PAYER_ID = cad.completaCliente(dOCUMENTO.PAYER_ID);
                        CLIENTE payer = getCliente(dOCUMENTO.PAYER_ID);
                        //B20180621 MGC2 2018.06.21s
                        if (borrador_param == null)
                        {
                            borrador_param = "";
                        }
                        //jemo 11-07-2018 inicio
                        if (unafact == "true")
                        {
                            string masi = db.TSOLs.Where(x => x.ID == dOCUMENTO.TSOL_ID).Select(x => x.TSOLM).SingleOrDefault();
                            if (masi != null)
                                dOCUMENTO.TSOL_ID = masi;
                        }
                        //jemo 11-07-2018 fin
                        if (borrador_param.Equals("borrador"))
                        {
                            //Eliminar borrador anterior 
                            string borre = "";
                            borre = eliminarBorrador(dOCUMENTO);
                            //Guardar el borrador documento
                            if (borre != "")
                            {
                                try
                                {
                                    dOCUMENTO.VKORG = payer.VKORG;
                                    dOCUMENTO.VTWEG = payer.VTWEG;
                                    dOCUMENTO.SPART = payer.SPART;
                                }
                                catch (Exception)
                                {

                                }
                                DOCUMENTBORR docb = new DOCUMENTBORR();
                                //docb = guardarBorrador(dOCUMENTO, id_bukrs, select_dis, monedadis, bmonto_apoyo);//RSG 09.07.2018
                                docb = guardarBorrador(dOCUMENTO, id_bukrs, select_dis, monedadis, bmonto_apoyo, chk_ligada);
                                db.DOCUMENTBORRs.Add(docb);
                                db.SaveChanges();
                                //B20180625 MGC 2018.06.27 Almacenar facturas
                                guardarBorradorf(dOCUMENTO, unafact);
                                guardarBorradorp(dOCUMENTO);
                                guardarBorradorn(dOCUMENTO.USUARIOC_ID, notas_soporte);
                                Session["BORRADOR"] = "Borrador almacenado";
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            Rangos rangos = new Rangos();//RSG 01.08.2018
                                                         //Obtener el número de documento
                            decimal N_DOC = rangos.getSolID(dOCUMENTO.TSOL_ID);
                            dOCUMENTO.NUM_DOC = N_DOC;
                            //Actualizar el rango
                            rangos.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);


                            dOCUMENTO.VKORG = payer.VKORG;
                            dOCUMENTO.VTWEG = payer.VTWEG;
                            dOCUMENTO.SPART = payer.SPART;

                            if (chk_ligada == "on")
                                dOCUMENTO.LIGADA = true;
                            else
                                dOCUMENTO.LIGADA = false;

                            //Guardar el documento
                            db.DOCUMENTOes.Add(dOCUMENTO);
                            db.SaveChanges();

                            //B20180625 MGC2 2018.07.04
                            //Eliminar borrador anterior 
                            string borre = "";
                            borre = eliminarBorrador(dOCUMENTO);

                            //Se eliminó
                            if (borre != "")
                            {
                                borrador_param = "false";
                            }
                        }


                        //Redireccionar al inicio
                        //Guardar número de documento creado
                        Session["NUM_DOC"] = dOCUMENTO.NUM_DOC;

                        //Validar si es una reversa
                        var revn = "";
                        try
                        {
                            if (dOCUMENTO.TSOL_ID == null || dOCUMENTO.TSOL_ID.Equals(""))
                            {
                                throw new Exception();
                            }

                            TSOL ts = db.TSOLs.Where(tsb => tsb.ID == dOCUMENTO.TSOL_ID).FirstOrDefault();
                            if (ts != null)
                            {
                                revn = "";
                                //DateTime theTime = (dynamic)null;
                                DateTime dates = DateTime.Now;
                                try
                                {
                                    //dates = DateTime.Now;
                                    //theTime  = DateTime.ParseExact(FECHAD_REV, //"06/04/2018 12:00:00 a.m."
                                    //            "yyyy-MM-dd",
                                    //            System.Globalization.CultureInfo.InvariantCulture,
                                    //            System.Globalization.DateTimeStyles.None);
                                }
                                catch (Exception e)
                                {
                                }
                                //Si es una reversa
                                try
                                {
                                    if (TREVERSA != null)
                                    {
                                        DOCUMENTOR docr = new DOCUMENTOR();
                                        docr.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        docr.TREVERSA_ID = Convert.ToInt32(TREVERSA);
                                        docr.USUARIOC_ID = User.Identity.Name;
                                        docr.FECHAC = dates;
                                        docr.COMENTARIO = notas_soporte.ToString();

                                        db.DOCUMENTORs.Add(docr);
                                        db.SaveChanges();

                                        revn = "X";
                                    }

                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                        catch (Exception e)
                        {
                        }

                        //B20180618 v1 MGC 2018.06.18
                        if (select_neg == "" || select_neg == null)
                        {
                            select_neg = select_negi;
                        }
                        if (select_dis == "" || select_dis == null)
                        {
                            select_dis = select_disi;
                        }
                        //Se cambio de pocisión //B20180618 v1 MGC 2018.06.18--------------------------------------
                        //Si la distribución es categoría se obtienen las categorías
                        List<string> listcat = new List<string>();
                        decimal totalcats = 0;
                        List<CategoriaMaterial> listcatm = new List<CategoriaMaterial>();
                        if (select_dis == "C")
                        {
                            for (int j = 0; j < dOCUMENTO.DOCUMENTOP.Count; j++)
                            {
                                string cat = dOCUMENTO.DOCUMENTOP.ElementAt(j).MATKL_ID.ToString();
                                listcat.Add(cat);
                            }


                            listcatm = grupoMaterialesController(listcat, dOCUMENTO.VKORG, dOCUMENTO.SPART, dOCUMENTO.PAYER_ID, dOCUMENTO.SOCIEDAD_ID, out totalcats);
                        }
                        //Se cambio de pocisión //B20180618 v1 MGC 2018.06.18--------------------------------------
                        //Guardar los documentos p para el documento guardado
                        try
                        {
                            //Agregar materiales existentes para evitar que en la vista se hayan agregado o quitado
                            List<DOCUMENTOP> docpl = new List<DOCUMENTOP>();
                            if (dOCUMENTO.DOCUMENTO_REF > 0)
                            {
                                docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).ToList();

                                //Add MGC B20180705 2018.07.05 Agregar materiales agregados en la vista
                                if (rele == "X" && select_dis == "M" && select_neg == "M")
                                {
                                    List<DOCUMENTOP_MOD> listvista = new List<DOCUMENTOP_MOD>();

                                    for (int h = 0; h < dOCUMENTO.DOCUMENTOP.Count; h++)
                                    {
                                        DOCUMENTOP docmode = new DOCUMENTOP();

                                        string mmatnr = dOCUMENTO.DOCUMENTOP[h].MATNR.TrimStart('0');
                                        docmode = docpl.Where(dcp => dcp.MATNR.TrimStart('0') == mmatnr).FirstOrDefault();

                                        //Agregarlo a la lista
                                        if (docmode == null)
                                        {
                                            DOCUMENTOP docadd = new DOCUMENTOP();

                                            docadd.MATNR = dOCUMENTO.DOCUMENTOP[h].MATNR;
                                            if (dOCUMENTO.DOCUMENTOP[h].MATKL_ID == null)
                                            {
                                                dOCUMENTO.DOCUMENTOP[h].MATKL_ID = "";
                                            }
                                            docadd.MATKL = dOCUMENTO.DOCUMENTOP[h].MATKL_ID;
                                            docadd.CANTIDAD = 1;
                                            docadd.MONTO = dOCUMENTO.DOCUMENTOP[h].MONTO;
                                            docadd.PORC_APOYO = dOCUMENTO.DOCUMENTOP[h].PORC_APOYO;
                                            docadd.MONTO_APOYO = dOCUMENTO.DOCUMENTOP[h].MONTO_APOYO;
                                            docadd.PRECIO_SUG = dOCUMENTO.DOCUMENTOP[h].PRECIO_SUG;
                                            docadd.VOLUMEN_EST = dOCUMENTO.DOCUMENTOP[h].VOLUMEN_EST;
                                            docadd.VOLUMEN_REAL = dOCUMENTO.DOCUMENTOP[h].VOLUMEN_REAL;
                                            docadd.VIGENCIA_DE = dOCUMENTO.DOCUMENTOP[h].VIGENCIA_DE;
                                            docadd.VIGENCIA_AL = dOCUMENTO.DOCUMENTOP[h].VIGENCIA_AL;
                                            docadd.APOYO_EST = dOCUMENTO.DOCUMENTOP[h].APOYO_EST;
                                            docadd.APOYO_REAL = dOCUMENTO.DOCUMENTOP[h].APOYO_REAL;

                                            docpl.Add(docadd);
                                        }
                                    }
                                }

                                for (int j = 0; j < docpl.Count; j++)
                                {
                                    try
                                    {
                                        DOCUMENTOP_MOD docmod = new DOCUMENTOP_MOD();
                                        var cat = "";

                                        if (docpl[j].MATNR != null && docpl[j].MATNR != "")
                                        {
                                            string mmatnr = docpl[j].MATNR.TrimStart('0');//RSG 07.06.2018
                                            docmod = dOCUMENTO.DOCUMENTOP.Where(docp => docp.MATNR == mmatnr).FirstOrDefault();
                                        }
                                        else
                                        {
                                            docmod = dOCUMENTO.DOCUMENTOP.Where(docp => docp.MATKL_ID == docpl[j].MATKL).FirstOrDefault();
                                            cat = "C";
                                        }
                                        DOCUMENTOP docP = new DOCUMENTOP();
                                        //Si lo encuentra meter valores de la base de datos y vista
                                        if (docmod != null)
                                        {
                                            docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                                            docP.POS = docmod.POS;
                                            if (docmod.MATNR == null)
                                            {
                                                docmod.MATNR = "";
                                            }
                                            docP.MATNR = docmod.MATNR;
                                            docP.MATNR = new Cadena().completaMaterial(docP.MATNR);//RSG 07.06.2018
                                            if (docmod.MATKL == null)
                                            {
                                                docmod.MATKL = "";
                                            }
                                            try { docP.MATKL = docmod.MATKL_ID; } catch { }
                                            docP.CANTIDAD = 1;
                                            docP.MONTO = docmod.MONTO;
                                            docP.PORC_APOYO = docmod.PORC_APOYO;
                                            //docP.MONTO_APOYO = docmod.MONTO_APOYO;
                                            docP.MONTO_APOYO = docP.MONTO * (docP.PORC_APOYO / 100);

                                            docP.MONTO_APOYO = Math.Round(docP.MONTO_APOYO, 2);//RSG 16.05.2018
                                            docP.PRECIO_SUG = docmod.PRECIO_SUG;
                                            docP.VOLUMEN_EST = docmod.VOLUMEN_EST;
                                            docP.VOLUMEN_REAL = docmod.VOLUMEN_REAL;
                                            docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                                            docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                                            docP.APOYO_EST = docmod.APOYO_EST;
                                            docP.APOYO_REAL = docmod.APOYO_REAL;

                                        }
                                        else
                                        {
                                            docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                                            docP.POS = docpl[j].POS;
                                            docP.MATNR = docpl[j].MATNR;
                                            docP.MATKL = docpl[j].MATKL;
                                            docP.CANTIDAD = 1;
                                            docP.MONTO = docpl[j].MONTO;
                                            //docP.PORC_APOYO = docpl[j].PORC_APOYO;
                                            docP.MONTO_APOYO = docP.MONTO * (docpl[j].PORC_APOYO / 100);
                                            docP.MONTO_APOYO = docpl[j].MONTO_APOYO;
                                            docP.PRECIO_SUG = docpl[j].PRECIO_SUG;
                                            docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                                            docP.VOLUMEN_REAL = docpl[j].VOLUMEN_REAL;
                                            docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                                            docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                                            docP.APOYO_EST = docpl[j].APOYO_EST;
                                            docP.APOYO_REAL = docpl[j].APOYO_REAL;
                                        }

                                        //Agregarlo a la bd
                                        db.DOCUMENTOPs.Add(docP);
                                        db.SaveChanges();//RSG

                                        //Se agrego para las relacionadas //B20180618 v1 MGC 2018.06.18--------------------------------------
                                        //If matnr es "" agregar los materiales de la categoría
                                        List<DOCUMENTOM> docml = new List<DOCUMENTOM>();
                                        if (docP.MATNR == "")
                                        {
                                            string col = "";
                                            if (Convert.ToDecimal(docP.APOYO_EST) > 0)
                                            {
                                                col = "E";
                                            }
                                            else if (Convert.ToDecimal(docP.APOYO_REAL) > 0)
                                            {
                                                col = "R";
                                            }
                                            docml = addCatItems(listcatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                                Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, select_neg, select_dis, totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);
                                        }

                                        //Obtener apoyo estimado
                                        decimal apoyo_esti = 0;
                                        decimal apoyo_real = 0;
                                        //Categoría por monto
                                        if (select_neg == "M")
                                        {
                                            //Obtener el apoyo real o estimado para cada material
                                            var cantmat = 1;
                                            try
                                            {
                                                apoyo_esti = Convert.ToDecimal(docP.APOYO_EST) / cantmat;

                                            }
                                            catch (Exception)
                                            {
                                                apoyo_esti = 0;
                                            }

                                            try
                                            {
                                                apoyo_real = Convert.ToDecimal(docP.APOYO_REAL) / cantmat;

                                            }
                                            catch (Exception)
                                            {
                                                apoyo_real = 0;
                                            }

                                            for (int k = 0; k < docml.Count; k++)
                                            {
                                                try
                                                {
                                                    DOCUMENTOM docM = new DOCUMENTOM();
                                                    docM = docml[k];
                                                    docM.POS = k + 1;
                                                    CategoriaMaterial cm = listcatm.Where(x => x.ID == docP.MATKL).FirstOrDefault();
                                                    decimal porc = 0;
                                                    if (cm != null)
                                                    {
                                                        if (cm.TOTALCAT > 0) { porc = cm.MATERIALES.Where(x => x.MATNR == docM.MATNR).FirstOrDefault().VAL / cm.TOTALCAT * 100; }
                                                        docM.PORC_APOYO = porc;
                                                        //apoyo_real = apoyo_real * porc;
                                                        //apoyo_esti = apoyo_esti * porc;
                                                    }
                                                    else if (docP.MATKL == "000")
                                                    {
                                                        decimal suma = 0;
                                                        foreach (CategoriaMaterial cam in listcatm)
                                                            suma += cam.TOTALCAT;
                                                        foreach (CategoriaMaterial cam in listcatm)
                                                            foreach (DOCUMENTOM_MOD dm in cam.MATERIALES)
                                                                if (dm.MATNR == docM.MATNR)
                                                                    if (suma > 0) { porc = dm.VAL / suma * 100; }
                                                        docM.PORC_APOYO = porc;
                                                    }
                                                    docM.APOYO_REAL = apoyo_real * porc / 100;
                                                    docM.APOYO_EST = apoyo_esti * porc / 100;

                                                    db.DOCUMENTOMs.Add(docM);
                                                    db.SaveChanges();//RSG
                                                }
                                                catch (Exception e)
                                                {

                                                }
                                            }
                                        }
                                        else if (select_neg == "P")
                                        {
                                            if (d.LIGADA == true)
                                            {
                                                docP.APOYO_REAL = 0;
                                                docP.APOYO_EST = 0;
                                                docP.PORC_APOYO = 0;
                                            }
                                            foreach (DOCUMENTOP dep in d.DOCUMENTOPs.Where(x => x.POS == docpl[j].POS))
                                            {
                                                foreach (DOCUMENTOM dM in dep.DOCUMENTOMs)
                                                {
                                                    DOCUMENTOM docM = new DOCUMENTOM();
                                                    docM.POS = dM.POS;

                                                    DOCUMENTOM dm = dep.DOCUMENTOMs.Where(x => x.POS == docM.POS).FirstOrDefault();
                                                    //docM.APOYO_EST = ;
                                                    docM.APOYO_REAL = dm.APOYO_EST;
                                                    docM.APOYO_REAL = dOCUMENTO.MONTO_DOC_MD * dm.PORC_APOYO / 100;
                                                    docM.MATNR = dm.MATNR;
                                                    //dmm.NUM_DOC = dm.NUM_DOC;
                                                    docM.PORC_APOYO = dm.PORC_APOYO;
                                                    docM.POS_ID = dm.POS_ID;
                                                    docM.VALORH = dm.VALORH;
                                                    docM.VIGENCIA_A = dm.VIGENCIA_A;
                                                    docM.VIGENCIA_DE = dm.VIGENCIA_DE;

                                                    docP.DOCUMENTOMs.Add(docM);

                                                    docP.APOYO_REAL += docM.APOYO_REAL;
                                                    docP.PORC_APOYO += (decimal)docM.PORC_APOYO;
                                                }
                                            }

                                            db.SaveChanges();//RSG
                                                             //////Categoría por porcentaje
                                                             ////for (int k = 0; k < docml.Count; k++)
                                                             ////{
                                                             ////    try
                                                             ////    {
                                                             ////        DOCUMENTOM docM = new DOCUMENTOM();
                                                             ////        docM = docml[k];
                                                             ////        docM.POS = k + 1;

                                            ////        if (d.LIGADA == true && d != null)
                                            ////        {

                                            ////            foreach (DOCUMENTOP dep in d.DOCUMENTOPs.Where(x => x.POS == docM.POS_ID))
                                            ////            {
                                            ////                DOCUMENTOM dm = dep.DOCUMENTOMs.Where(x => x.POS == docM.POS).FirstOrDefault();
                                            ////                //docM.APOYO_EST = ;
                                            ////                docM.APOYO_REAL = dm.APOYO_EST;
                                            ////                docM.MATNR = dm.MATNR;
                                            ////                //dmm.NUM_DOC = dm.NUM_DOC;
                                            ////                docM.PORC_APOYO = dm.PORC_APOYO;
                                            ////                docM.POS_ID = dm.POS_ID;
                                            ////                docM.VALORH = dm.VALORH;
                                            ////                docM.VIGENCIA_A = dm.VIGENCIA_A;
                                            ////                docM.VIGENCIA_DE = dm.VIGENCIA_DE;


                                            ////                //docP.APOYO_REAL += docM.APOYO_REAL;
                                            ////                //docP.APOYO_EST += docM.APOYO_EST;
                                            ////                //docP.PORC_APOYO += (decimal)docM.PORC_APOYO;
                                            ////            }
                                            ////        }

                                            ////        db.DOCUMENTOMs.Add(docM);
                                            ////        db.SaveChanges();//RSG
                                            ////    }
                                            ////    catch (Exception e)
                                            ////    {

                                            ////    }
                                            ////}

                                            //////if (d.LIGADA == true && d != null)
                                            //////{
                                            //////    foreach (DOCUMENTOP dep in d.DOCUMENTOPs.Where(x => x.POS == docP.POS))
                                            //////    {
                                            //////        docP.APOYO_REAL += dep.APOYO_EST;
                                            //////        //docP.APOYO_EST += dep.;
                                            //////        docP.PORC_APOYO += (decimal)dep.PORC_APOYO;
                                            //////    }

                                            //////    db.SaveChanges();//RSG
                                            //////}
                                        }
                                        //Se agrego para las relacionadas //B20180618 v1 MGC 2018.06.18--------------------------------------
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                }
                            }
                            else
                            {

                                for (int j = 0; j < dOCUMENTO.DOCUMENTOP.Count; j++)
                                {
                                    try
                                    {
                                        DOCUMENTOP docP = new DOCUMENTOP();

                                        docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        docP.POS = dOCUMENTO.DOCUMENTOP.ElementAt(j).POS;
                                        if (dOCUMENTO.DOCUMENTOP.ElementAt(j).MATNR == null)
                                        {
                                            dOCUMENTO.DOCUMENTOP.ElementAt(j).MATNR = "";
                                        }
                                        docP.MATNR = dOCUMENTO.DOCUMENTOP.ElementAt(j).MATNR;
                                        docP.MATNR = new Cadena().completaMaterial(docP.MATNR);
                                        if (dOCUMENTO.DOCUMENTOP.ElementAt(j).MATKL_ID == null)
                                        {
                                            dOCUMENTO.DOCUMENTOP.ElementAt(j).MATKL_ID = "";
                                        }
                                        docP.MATKL = dOCUMENTO.DOCUMENTOP.ElementAt(j).MATKL_ID;
                                        docP.CANTIDAD = 1;
                                        docP.MONTO = dOCUMENTO.DOCUMENTOP.ElementAt(j).MONTO;
                                        docP.PORC_APOYO = dOCUMENTO.DOCUMENTOP.ElementAt(j).PORC_APOYO;
                                        docP.MONTO_APOYO = dOCUMENTO.DOCUMENTOP.ElementAt(j).MONTO_APOYO;
                                        docP.PRECIO_SUG = dOCUMENTO.DOCUMENTOP.ElementAt(j).PRECIO_SUG;
                                        docP.VOLUMEN_EST = dOCUMENTO.DOCUMENTOP.ElementAt(j).VOLUMEN_EST;
                                        docP.VOLUMEN_REAL = dOCUMENTO.DOCUMENTOP.ElementAt(j).VOLUMEN_REAL;
                                        docP.VIGENCIA_DE = dOCUMENTO.DOCUMENTOP.ElementAt(j).VIGENCIA_DE;
                                        docP.VIGENCIA_AL = dOCUMENTO.DOCUMENTOP.ElementAt(j).VIGENCIA_AL;
                                        docP.APOYO_EST = dOCUMENTO.DOCUMENTOP.ElementAt(j).APOYO_EST;
                                        docP.APOYO_REAL = dOCUMENTO.DOCUMENTOP.ElementAt(j).APOYO_REAL;

                                        db.DOCUMENTOPs.Add(docP);
                                        db.SaveChanges();//RSG

                                        //If matnr es "" agregar los materiales de la categoría
                                        List<DOCUMENTOM> docml = new List<DOCUMENTOM>();
                                        if (docP.MATNR == "")
                                        {
                                            string col = "";
                                            if (Convert.ToDecimal(docP.APOYO_EST) > 0)
                                            {
                                                col = "E";
                                            }
                                            else if (Convert.ToDecimal(docP.APOYO_REAL) > 0)
                                            {
                                                col = "R";
                                            }
                                            docml = addCatItems(listcatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                                Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, select_neg, select_dis, totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);
                                        }

                                        //Obtener apoyo estimado
                                        decimal apoyo_esti = 0;
                                        decimal apoyo_real = 0;
                                        //Categoría por monto
                                        if (select_neg == "M")
                                        {
                                            //Obtener el apoyo real o estimado para cada material
                                            var cantmat = 1;
                                            try
                                            {
                                                apoyo_esti = Convert.ToDecimal(docP.APOYO_EST) / cantmat;

                                            }
                                            catch (Exception e)
                                            {
                                                apoyo_esti = 0;
                                            }

                                            try
                                            {
                                                apoyo_real = Convert.ToDecimal(docP.APOYO_REAL) / cantmat;

                                            }
                                            catch (Exception e)
                                            {
                                                apoyo_real = 0;
                                            }

                                            for (int k = 0; k < docml.Count; k++)
                                            {
                                                try
                                                {
                                                    DOCUMENTOM docM = new DOCUMENTOM();
                                                    docM = docml[k];
                                                    docM.POS = k + 1;
                                                    CategoriaMaterial cm = listcatm.Where(x => x.ID == docP.MATKL).FirstOrDefault();
                                                    decimal porc = 0;
                                                    if (cm != null)
                                                    {
                                                        if (cm.TOTALCAT > 0) { porc = cm.MATERIALES.Where(x => x.MATNR == docM.MATNR).FirstOrDefault().VAL / cm.TOTALCAT * 100; }
                                                        docM.PORC_APOYO = porc;
                                                        //apoyo_real = apoyo_real * porc;
                                                        //apoyo_esti = apoyo_esti * porc;
                                                    }
                                                    else if (docP.MATKL == "000")
                                                    {
                                                        decimal suma = 0;
                                                        foreach (CategoriaMaterial cam in listcatm)
                                                            suma += cam.TOTALCAT;
                                                        foreach (CategoriaMaterial cam in listcatm)
                                                            foreach (DOCUMENTOM_MOD dm in cam.MATERIALES)
                                                                if (dm.MATNR == docM.MATNR)
                                                                    if (suma > 0) { porc = dm.VAL / suma * 100; }
                                                        docM.PORC_APOYO = porc;
                                                    }
                                                    docM.APOYO_REAL = apoyo_real * porc / 100;
                                                    docM.APOYO_EST = apoyo_esti * porc / 100;

                                                    db.DOCUMENTOMs.Add(docM);
                                                    db.SaveChanges();//RSG
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.ErrorLogApp(e, "Solicitudes", "Create-DOCUMENTOM");
                                                }
                                            }
                                        }
                                        else if (select_neg == "P")
                                        {
                                            //Categoría por porcentaje
                                            for (int k = 0; k < docml.Count; k++)
                                            {
                                                try
                                                {
                                                    DOCUMENTOM docM = docml[k];
                                                    docM.POS = k + 1;

                                                    db.DOCUMENTOMs.Add(docM);
                                                    db.SaveChanges();//RSG
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.ErrorLogApp(e, "Solicitudes", "Create-DOCUMENTOM");
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Log.ErrorLogApp(e, "Solicitudes", "Create");
                                    }
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Log.ErrorLogApp(e, "Solicitudes", "Create");
                        }



                        //Guardar los documentos f para el documento guardado
                        //jemo 12-07-2018 inicio
                        try
                        {
                            if (dOCUMENTO.DOCUMENTOF.Count == 1)
                            {
                                List<DOCUMENTOF> facts = new List<DOCUMENTOF>();
                                string[] fact = dOCUMENTO.DOCUMENTOF[0].FACTURAK.Split(';');
                                for (int k = 0; k < fact.Length; k++)
                                {
                                    DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[0];
                                    docF.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    docF.POS = k + 1;
                                    if (fact[k].Length > 4000)
                                        fact[k] = fact[k].Substring(0, 4000);
                                    docF.FACTURAK = fact[k];
                                    facts.Add(new DOCUMENTOF
                                    {
                                        NUM_DOC = dOCUMENTO.NUM_DOC,
                                        POS = k + 1,
                                        FACTURA = docF.FACTURA,
                                        FECHA = docF.FECHA,
                                        PROVEEDOR = docF.PROVEEDOR,
                                        CONTROL = docF.CONTROL,
                                        AUTORIZACION = docF.AUTORIZACION,
                                        VENCIMIENTO = docF.VENCIMIENTO,
                                        EJERCICIOK = docF.EJERCICIOK,
                                        BILL_DOC = docF.BILL_DOC,
                                        BELNR = docF.BELNR,
                                        IMPORTE_FAC = docF.IMPORTE_FAC,
                                        PAYER = docF.PAYER,
                                        FACTURAK = fact[k]
                                    });
                                }
                                db.BulkInsert(facts);
                            }
                            else
                            {
                                for (int j = 0; j < dOCUMENTO.DOCUMENTOF.Count; j++)
                                {
                                    try
                                    {
                                        DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[j];
                                        docF.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        db.DOCUMENTOFs.Add(docF);
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.ErrorLogApp(e, "Solicitudes", "Create-DOCUMENTOF");
                                    }
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Log.ErrorLogApp(e, "Solicitudes", "Create-DOCUMENTOF");
                        }
                        //jemo 12-07-2018 fin
                        //Guardar registros de recurrencias  RSG 28.05.2018------------------
                        if (dOCUMENTO.DOCUMENTOREC != null)
                            if (dOCUMENTO.DOCUMENTOREC.Count > 0)
                            {
                                foreach (DOCUMENTOREC drec in dOCUMENTO.DOCUMENTOREC)
                                {
                                    drec.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    if (drec.POS == 1)
                                    {
                                        ////drec.DOC_REF = drec.NUM_DOC;//RSG 09.07.2018
                                        ////drec.ESTATUS = "P";//RSG 09.07.2018
                                        if (dOCUMENTO.TIPO_TECNICO != "P")
                                        {//RSG 29.07.2018 -DELETE
                                         ////Calendario445 c4 = new Calendario445();//RSG 11.06.2018
                                         ////dOCUMENTO.FECHAI_VIG = drec.FECHAF;
                                         //////dOCUMENTO.FECHAF_VIG = drec.FECHAF.Value.AddMonths(1).AddDays(-1);//RSG 11.06.2018
                                         ////int ppp = c4.getPeriodo(drec.FECHAF.Value);
                                         ////dOCUMENTO.FECHAF_VIG = c4.getUltimoDia(drec.FECHAF.Value.Year, ppp);//RSG 11.06.2018
                                         ////dOCUMENTO.TIPO_RECURRENTE = "M";
                                        }
                                        else
                                        {//RSG 29.07.2018 - delete
                                         ////Calendario445 c4 = new Calendario445();//RSG 11.06.2018
                                         ////dOCUMENTO.FECHAI_VIG = c4.getPrimerDia(drec.FECHAF.Value.Year, drec.FECHAF.Value.Month);//RSG 11.06.2018
                                         //////dOCUMENTO.FECHAI_VIG = new DateTime(drec.FECHAF.Value.Year, drec.FECHAF.Value.Month, 1);//RSG 11.06.2018
                                         ////dOCUMENTO.FECHAF_VIG = drec.FECHAF;
                                            dOCUMENTO.TIPO_RECURRENTE = "P";
                                        }
                                        ////foreach (DOCUMENTOP po in dOCUMENTO.DOCUMENTOPs)//RSG 29.07.2018 - delete
                                        ////{
                                        ////    po.VIGENCIA_DE = dOCUMENTO.FECHAI_VIG;
                                        ////    po.VIGENCIA_AL = dOCUMENTO.FECHAF_VIG;
                                        ////}
                                    }
                                    if (drec.MONTO_BASE == null) //RSG 31.05.2018-------------------
                                        drec.MONTO_BASE = 0;
                                    if (drec.PORC == null) //RSG 31.05.2018-------------------
                                        drec.PORC = 0;
                                    dOCUMENTO.TIPO_RECURRENTE = db.TSOLs.Where(x => x.ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().TRECU;
                                    if (dOCUMENTO.TIPO_RECURRENTE == "1" && dOCUMENTO.LIGADA == true)
                                        dOCUMENTO.TIPO_RECURRENTE = "2";
                                    if (dOCUMENTO.TIPO_RECURRENTE != "1" && dOCUMENTO.OBJETIVOQ == true)
                                        dOCUMENTO.TIPO_RECURRENTE = "3";
                                    //RSG 29.07.2018-add----------------------------------
                                    drec.FECHAV = drec.FECHAF;
                                    if (dOCUMENTO.TIPO_RECURRENTE == "1")
                                        drec.FECHAF = cal.getNextViernes((DateTime)drec.FECHAF);
                                    else
                                        drec.FECHAF = cal.getNextLunes((DateTime)drec.FECHAF);
                                    if (dOCUMENTO.DOCUMENTOREC.Count > 1 && dOCUMENTO.LIGADA == true)//RSG 20.12.2018
                                        drec.FECHAF = drec.FECHAF.Value.AddDays(-1);
                                    drec.EJERCICIO = drec.FECHAV.Value.Year;
                                    if (dOCUMENTO.TIPO_RECURRENTE == "1")
                                        drec.PERIODO = cal.getPeriodo(drec.FECHAV.Value);
                                    else
                                        drec.PERIODO = cal.getPeriodoF(drec.FECHAV.Value);
                                    if (dOCUMENTO.TIPO_RECURRENTE == "1")
                                        drec.PERIODO--;
                                    if (drec.PERIODO == 0) drec.PERIODO = 12;
                                    ////int num = int.Parse(sel_nn);
                                    ////int pos = drec.POS % num;
                                    //RSG 29.07.2018-add----------------------------------
                                    if (dOCUMENTO.DOCUMENTORAN != null)
                                        foreach (DOCUMENTORAN dran in dOCUMENTO.DOCUMENTORAN.Where(x => x.POS == drec.POS))
                                        {
                                            dran.NUM_DOC = dOCUMENTO.NUM_DOC;
                                            drec.DOCUMENTORANs.Add(dran);
                                        }

                                    dOCUMENTO.DOCUMENTORECs.Add(drec);
                                }
                                db.SaveChanges();
                            }//Guardar registros de recurrencias  RSG 28.05.2018-------------------
                        if (dOCUMENTO.DOCUMENTOREC == null && dOCUMENTO.LIGADA == true)
                        //if (dOCUMENTO.LIGADA == true)
                        {

                            DOCUMENTOREC drec = new DOCUMENTOREC();
                            drec.NUM_DOC = dOCUMENTO.NUM_DOC;
                            drec.POS = 1;

                            if (drec.MONTO_BASE == null) //RSG 31.05.2018-------------------
                                drec.MONTO_BASE = 0;
                            if (drec.PORC == null) //RSG 31.05.2018-------------------
                                drec.PORC = 0;
                            dOCUMENTO.TIPO_RECURRENTE = db.TSOLs.Where(x => x.ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().TRECU;
                            if (dOCUMENTO.TIPO_RECURRENTE == "1" && dOCUMENTO.LIGADA == true)
                                dOCUMENTO.TIPO_RECURRENTE = "2";
                            if (dOCUMENTO.TIPO_RECURRENTE != "1" && dOCUMENTO.OBJETIVOQ == true)
                                dOCUMENTO.TIPO_RECURRENTE = "3";
                            drec.FECHAF = cal.getUltimoDia(dOCUMENTO.FECHAF_VIG.Value.Year, cal.getPeriodo(dOCUMENTO.FECHAF_VIG.Value));
                            drec.FECHAV = drec.FECHAF;

                            drec.FECHAF = cal.getNextLunes((DateTime)drec.FECHAF);
                            drec.EJERCICIO = drec.FECHAV.Value.Year;
                            drec.PERIODO = cal.getPeriodoF(drec.FECHAV.Value);

                            if (drec.PERIODO == 0) drec.PERIODO = 12;
                            if (dOCUMENTO.DOCUMENTORAN != null)
                            {
                                foreach (DOCUMENTORAN dran in dOCUMENTO.DOCUMENTORAN.Where(x => x.POS == drec.POS))
                                {
                                    dran.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    drec.DOCUMENTORANs.Add(dran);
                                }
                            }
                            else
                            {
                                DOCUMENTORAN dran = new DOCUMENTORAN();
                                dran.NUM_DOC = dOCUMENTO.NUM_DOC;
                                dran.POS = 1;
                                dran.LIN = 1;
                                dran.OBJETIVOI = 0;
                                dran.PORCENTAJE = dOCUMENTO.PORC_APOYO;
                                drec.DOCUMENTORANs.Add(dran);
                            }
                            drec.PORC = dOCUMENTO.PORC_APOYO;
                            drec.DOC_REF = 0;
                            drec.ESTATUS = "";

                            dOCUMENTO.DOCUMENTORECs.Add(drec);

                            db.SaveChanges();
                        }
                        //Guardar los documentos cargados en la sección de soporte
                        var res = "";
                        string errorMessage = "";
                        int numFiles = 0;
                        //Checar si hay archivos para subir
                        try
                        {
                            foreach (HttpPostedFileBase file in files_soporte)
                            {
                                if (file != null)
                                {
                                    if (file.ContentLength > 0)
                                    {
                                        numFiles++;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.ErrorLogApp(e, "Solicitudes", "Create");
                        }

                        if (numFiles > 0)
                        {
                            //Obtener las variables con los datos de sesión y ruta
                            string url = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") && x.ACTIVO).FirstOrDefault().VALUE;
                            //Crear el directorio
                            Log.Info("---Solicitudes-Create-Soporte-save---");
                            var dir = "";
                            try
                            {
                                Log.Info("dir->");
                                dir = new Files().createDir(url, dOCUMENTO.NUM_DOC.ToString(), dOCUMENTO.EJERCICIO.ToString());//RSG 01.08.2018  
                            }
                            catch (Exception e)
                            {
                                Log.ErrorLogApp(e, "Solicitudes", "Create");
                            }
                            //Evaluar que se creo el directorio
                            if (dir.Equals(""))
                            {
                                Log.Info("Inicia guardado de Soporte");
                                int i = 0;
                                int indexlabel = 0;
                                foreach (HttpPostedFileBase file in files_soporte)
                                {
                                    string errorfiles = "";
                                    var clasefile = "";
                                    try
                                    {
                                        clasefile = labels_soporte[indexlabel];
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.ErrorLogApp(ex, "Solicitudes", "Create");
                                        clasefile = "";
                                    }
                                    if (file != null)
                                    {
                                        if (file.ContentLength > 0)
                                        {
                                            string path = "";
                                            string filename = file.FileName;
                                            errorfiles = "";
                                            try
                                            {
                                                res = new Files().SaveFile(file, url, dOCUMENTO.NUM_DOC.ToString(), out errorfiles, out path, dOCUMENTO.EJERCICIO.ToString());//RSG 01.08.2018
                                            }
                                            catch (Exception e)
                                            {
                                                Log.ErrorLogApp(e, "Solicitudes", "Create");
                                            }
                                            if (errorfiles == "")
                                            {
                                                DOCUMENTOA doc = new DOCUMENTOA();
                                                var ext = System.IO.Path.GetExtension(filename);
                                                i++;
                                                doc.NUM_DOC = dOCUMENTO.NUM_DOC;
                                                doc.POS = i;
                                                doc.TIPO = ext.Replace(".", "");
                                                try
                                                {
                                                    var clasefileM = clasefile.ToUpper();
                                                    doc.CLASE = clasefileM.Substring(0, 3);
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.ErrorLogApp(e, "Solicitudes", "Create");
                                                    doc.CLASE = "";
                                                }
                                                doc.STEP_WF = 1;
                                                doc.USUARIO_ID = dOCUMENTO.USUARIOC_ID;
                                                doc.PATH = path;
                                                doc.ACTIVO = true;
                                                try
                                                {
                                                    db.DOCUMENTOAs.Add(doc);
                                                    db.SaveChanges();
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.ErrorLogApp(e, "Solicitudes", "Create");
                                                    errorfiles = "" + filename;
                                                }

                                            }
                                        }
                                    }
                                    indexlabel++;
                                    if (errorfiles != "")
                                    {
                                        errorMessage += "Error con el archivo " + errorfiles;
                                    }



                                }
                            }
                            else
                            {
                                errorMessage = dir;
                            }

                            errorString = errorMessage;
                            //Guardar número de documento creado
                            Session["ERROR_FILES"] = errorMessage;
                        }
                        string rec = "";
                        if (txt_flujo == "B")//ADD RSG 30.10.2018
                            rec = "B";
                        ProcesaFlujo pf = new ProcesaFlujo();
                        //db.DOCUMENTOes.Add(dOCUMENTO);
                        //db.SaveChanges();

                        USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();
                        //int rol = user.MIEMBROS.FirstOrDefault().ROL_ID;
                        try
                        {
                            //WORKFV wf = db.WORKFHs.Where(a => a.BUKRS.Equals(dOCUMENTO.SOCIEDAD_ID) && a.ROL_ID == rol).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                            WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                            if (wf != null)
                            {
                                WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                                FLUJO f = new FLUJO();
                                f.WORKF_ID = wf.ID;
                                f.WF_VERSION = wf.VERSION;
                                f.WF_POS = wp.POS;
                                f.NUM_DOC = dOCUMENTO.NUM_DOC;
                                f.POS = 1;
                                f.LOOP = 1;
                                f.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                                f.USUARIOD_ID = dOCUMENTO.USUARIOD_ID;
                                f.ESTATUS = "I";
                                f.FECHAC = DateTime.Now;
                                f.FECHAM = DateTime.Now;
                                f.COMENTARIO = notas_soporte;//ADD RSG 20.08.2018
                                string c = pf.procesa(f, rec);
                                FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                while (c == "1")
                                {
                                    Email em = new Email();
                                    DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == dOCUMENTO.NUM_DOC).First();
                                    string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                                    string image = Server.MapPath("~/images/logo_kellogg.png");
                                    string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                                    ////em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                                    em.enviaMailC(f.NUM_DOC, true, usuariotextos, UrlDirectory, "Index", image, imageFlag);

                                    if (conta.WORKFP.ACCION.TIPO == "B")
                                    {
                                        WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID && x.VERSION == conta.WF_VERSION && x.POS == conta.WF_POS).FirstOrDefault();
                                        conta.ESTATUS = "A";
                                        conta.FECHAM = DateTime.Now;
                                        c = pf.procesa(conta, "");
                                        conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();

                                    }
                                    else
                                    {
                                        c = "";
                                    }
                                }
                                //Estatus es = new Estatus();//RSG 18.09.2018
                                //DOCUMENTO doc = db.DOCUMENTOes.Find(dOCUMENTO.NUM_DOC);
                                //conta.STATUS = es.getEstatus(doc);
                                //db.Entry(conta).State = EntityState.Modified;
                                //db.SaveChanges();

                                using (TruequeEntities db1 = new TruequeEntities())
                                {
                                    FLUJO ff = db1.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                    Estatus es = new Estatus();//RSG 18.09.2018
                                    DOCUMENTO ddoc = db1.DOCUMENTOes.Find(f.NUM_DOC);
                                    ff.STATUS = es.getEstatus(ddoc);
                                    db1.Entry(ff).State = EntityState.Modified;
                                    db1.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            if (errorString == "")
                            {
                                errorString = ee.Message.ToString();
                            }
                            ViewBag.error = errorString;
                        }
                        //---------------------------------------------------------------------------------TODO CORRECTO
                        //if (dOCUMENTO.DOCUMENTO_REF > 0)
                        if (dOCUMENTO.DOCUMENTO_REF > 0 && txt_flujo != "B")//ADD RSG 02.11.2018
                        {
                            //if (dOCUMENTO.TSOL_ID != "CPR")
                            DOCUMENTO docPadre = db.DOCUMENTOes.Find(dOCUMENTO.DOCUMENTO_REF);
                            List<DOCUMENTO> dd = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == (dOCUMENTO.DOCUMENTO_REF) && a.ESTATUS_C != "C" && a.ESTATUS_WF != "B").ToList();
                            if (!dOCUMENTO.TSOL.REVERSO)
                            {
                                List<DOCUMENTOP> ddr = db.DOCUMENTOPs.Where(a => a.NUM_DOC == (dOCUMENTO.DOCUMENTO_REF)).ToList();
                                ////decimal total = 0;
                                decimal[] totales = new decimal[ddr.Count()];
                                decimal totalRes = new decimal();
                                ////foreach (DOCUMENTOP dr in ddr)
                                ////{
                                //totales[(int)dr.POS - 1] = dr.VOLUMEN_EST * dr.MONTO_APOYO;
                                ////totales[(int)dr.POS - 1] = (decimal)dr.APOYO_EST;
                                foreach (DOCUMENTO d1 in dd)
                                {
                                    //foreach (DOCUMENTOP dp in d1.DOCUMENTOPs)
                                    //{
                                    //    if (dr.POS == dp.POS)
                                    //    {
                                    //        //var suma2 = dp.VOLUMEN_REAL * dp.MONTO_APOYO;
                                    //        var suma2 = dp.APOYO_REAL;

                                    //        totales[(int)dr.POS - 1] = totales[(int)dr.POS - 1] - (decimal)suma2;
                                    //        totalRes += (decimal)suma2;
                                    //    }
                                    //}
                                    totalRes += (decimal)d1.MONTO_DOC_MD;
                                }
                                ////}
                                //RSG 14.06.2018----------------------
                                decimal resto = decimal.Parse("0.00");
                                ////foreach (decimal dec in totales)
                                ////{
                                ////    resto += dec;
                                ////}
                                resto = (decimal)docPadre.MONTO_DOC_MD - totalRes;
                                //////RSG 14.06.2018----------------------
                                ////foreach (decimal dec in totales)
                                ////{
                                ////    if (dec > 0 | (totalRes - docPadre.MONTO_DOC_MD) > 0)
                                ////        return RedirectToAction("Reversa", new { id = dOCUMENTO.DOCUMENTO_REF, resto = resto });
                                ////}
                                if (docPadre.MONTO_DOC_MD - totalRes > 0)
                                    return RedirectToAction("Reversa", new { id = dOCUMENTO.DOCUMENTO_REF, resto = resto });

                            }
                            else
                            {
                                if (dd.Where(x => !x.TSOL.REVERSO && x.ESTATUS_WF != "A").ToList().Count == 0)
                                {
                                    using (TruequeEntities db1 = new TruequeEntities())
                                    {
                                        decimal num_doc = dd.First(x => x.TSOL.REVERSO).NUM_DOC;
                                        FLUJO ff = db1.FLUJOes.Where(x => x.NUM_DOC == num_doc).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                        ff.FECHAM = DateTime.Now;
                                        ff.ESTATUS = "A";
                                        string c = pf.procesa(ff, "C");
                                        FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == ff.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                        while (c == "1")
                                        {
                                            Email em = new Email();
                                            string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                                            string image = Server.MapPath("~/images/logo_kellogg.png");
                                            DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == num_doc).First();
                                            string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                                            ////em.enviaMailC(ff.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                                            em.enviaMailC(ff.NUM_DOC, true, usuariotextos, UrlDirectory, "Index", image, imageFlag);

                                            if (conta.WORKFP.ACCION.TIPO == "B")
                                            {
                                                WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID && x.VERSION == conta.WF_VERSION && x.POS == conta.WF_POS).FirstOrDefault();
                                                conta.ESTATUS = "A";
                                                conta.FECHAM = DateTime.Now;
                                                c = pf.procesa(conta, "");
                                                conta = db.FLUJOes.Where(x => x.NUM_DOC == ff.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();

                                            }
                                            else
                                            {
                                                c = "";
                                            }
                                        }
                                    }
                                }
                            }
                            using (TruequeEntities db1 = new TruequeEntities())
                            {
                                decimal num_ref = (decimal)dOCUMENTO.DOCUMENTO_REF;
                                DOCUMENTO referencia = db1.DOCUMENTOes.Find(num_ref);
                                referencia.ESTATUS = "R";
                                db1.Entry(referencia).State = EntityState.Modified;
                                db1.SaveChanges();
                            }
                        }
                        if (dOCUMENTO.DOCUMENTO_REF != null && d != null)
                        {
                            bool nota = false;
                            DOCUMENTOREC drec = db.DOCUMENTORECs.FirstOrDefault(x => x.DOC_REF == d.NUM_DOC);
                            if (drec == null)
                                drec = db.DOCUMENTORECs.FirstOrDefault(x => x.NUM_DOC_Q == d.NUM_DOC);
                            if (drec != null)
                            {
                                if (drec.DOCUMENTO.OBJETIVOQ == true)
                                {
                                    DOCUMENTORAN dra = drec.DOCUMENTORANs.FirstOrDefault(x => x.LIN == 1);
                                    if (dra.OBJETIVOI == null)
                                    {
                                        dra.OBJETIVOI = 0;
                                    }
                                    DOCUMENTOL dl = d.DOCUMENTOLs.FirstOrDefault(x => x.POS == drec.POS);
                                    if (dl.BACKORDER == null)
                                        dl.BACKORDER = 0;
                                    if ((dl.MONTO_VENTA + dl.BACKORDER) > dra.OBJETIVOI)
                                    {
                                        nota = true;
                                    }
                                    if (nota)
                                    {
                                        decimal n_doc = 0;
                                        Reversa r = new Reversa();
                                        string a = r.creaReversa(drec.NUM_DOC_Q.ToString(), dOCUMENTO.TSOL_ID, ref n_doc, true);
                                    }
                                    else
                                    {
                                        decimal n_doc = 0;
                                        Reversa r = new Reversa();
                                        string a = r.creaReversa(drec.NUM_DOC_Q.ToString(), "CPR", ref n_doc, true);
                                        DOCUMENTO dOCUMENTOR = db.DOCUMENTOes.Find(n_doc);
                                        ////DOCUMENTO docPadre = db.DOCUMENTOes.Find(dOCUMENTOR.DOCUMENTO_REF);
                                        List<DOCUMENTO> dd = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == (dOCUMENTOR.DOCUMENTO_REF) && x.ESTATUS_C != "C" && x.ESTATUS_WF != "B").ToList();
                                        if (dOCUMENTOR.TSOL.REVERSO)
                                        {
                                            if (dd.Where(x => !x.TSOL.REVERSO && x.ESTATUS_WF != "A").ToList().Count == 0)
                                            {
                                                using (TruequeEntities db1 = new TruequeEntities())
                                                {
                                                    decimal num_doc = dd.First(x => x.TSOL.REVERSO).NUM_DOC;
                                                    FLUJO ff = db1.FLUJOes.Where(x => x.NUM_DOC == num_doc).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                                    ff.FECHAM = DateTime.Now;
                                                    ff.ESTATUS = "A";
                                                    string c = pf.procesa(ff, "C");
                                                    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == ff.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                                    while (c == "1")
                                                    {
                                                        Email em = new Email();
                                                        string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                                                        string image = Server.MapPath("~/images/logo_kellogg.png");
                                                        DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == num_doc).First();
                                                        string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                                                        ////em.enviaMailC(ff.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                                                        em.enviaMailC(ff.NUM_DOC, true, usuariotextos, UrlDirectory, "Index", image, imageFlag);

                                                        if (conta.WORKFP.ACCION.TIPO == "B")
                                                        {
                                                            WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID && x.VERSION == conta.WF_VERSION && x.POS == conta.WF_POS).FirstOrDefault();
                                                            conta.ESTATUS = "A";
                                                            conta.FECHAM = DateTime.Now;
                                                            c = pf.procesa(conta, "");
                                                            conta = db.FLUJOes.Where(x => x.NUM_DOC == ff.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();

                                                        }
                                                        else
                                                        {
                                                            c = "";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        using (TruequeEntities db1 = new TruequeEntities())
                                        {
                                            decimal num_ref = (decimal)dOCUMENTOR.DOCUMENTO_REF;
                                            DOCUMENTO referencia = db1.DOCUMENTOes.Find(num_ref);
                                            referencia.ESTATUS = "R";
                                            db1.Entry(referencia).State = EntityState.Modified;
                                            db1.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Solicitudes", "Create");
                        if (errorString == "")
                        {
                            errorString = e.Message.ToString();
                        }
                        ViewBag.error = errorString;

                    }
                }
            }

            int pagina = 202; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {

                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                usuariotextos = user.SPRAS_ID;//B20180801 MGC Textos
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
                //tipo de solicitud
                var id_sol = tiposSolicitudesDao.ComboTiposSolicitudes(user.SPRAS_ID, null)
                        .Select(x => new TSOLT_MOD
                        {
                            SPRAS_ID = user.SPRAS_ID,
                            TSOL_ID = x.Value,
                            TEXT = x.Text
                        }).ToList();
                ViewBag.listtreetsol = tiposSolicitudesDao.TreeTiposSolicitudes(dOCUMENTO.SOCIEDAD_ID, user.SPRAS_ID, "SD");
                ViewBag.TSOL_ID = new SelectList(id_sol, "TSOL_ID", "TEXT", selectedValue: dOCUMENTO.TSOL_ID);
                //                      

                //Select clasificación
                var id_clas = db.TALLs.Where(t => t.ACTIVO == true)
                                .Join(
                                db.TALLTs.Where(tallt => tallt.SPRAS_ID == user.SPRAS_ID),
                                tall => tall.ID,
                                tallt => tallt.TALL_ID,
                                (tall, tallt) => tallt)
                            .ToList();

                var id_clas_sel = db.TALLs.Where(t => t.ID == dOCUMENTO.TALL_ID).FirstOrDefault().GALL_ID;

                //Grupos de solicitud
                var id_grupo = db.GALLs.Where(g => g.ACTIVO == true)
                                .Join(
                                db.GALLTs.Where(gt => gt.SPRAS_ID == user.SPRAS_ID),
                                g => g.ID,
                                gt => gt.GALL_ID,
                                (g, gt) => new
                                {
                                    gt.SPRAS_ID,
                                    gt.GALL_ID,
                                    TEXT = g.DESCRIPCION + " " + gt.TXT50
                                }).ToList();

                var id_grupo_sel = id_grupo.Where(g => g.GALL_ID == id_clas_sel).FirstOrDefault().GALL_ID;

                ViewBag.GALL_ID = new SelectList(id_grupo, "GALL_ID", "TEXT", selectedValue: id_grupo_sel);

                //ViewBag.tall = db.TALLs.ToList();
                ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50", selectedValue: dOCUMENTO.TALL_ID);
                //Datos del país
                id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(dOCUMENTO.PAIS_ID) && soc.ACTIVO).FirstOrDefault();
                var id_pais = db.PAIS.Where(pais => pais.LAND.Equals(id_bukrs.LAND)).FirstOrDefault();

                var id_states = (from st in db.STATES
                                 join co in db.COUNTRIES
                                 on st.COUNTRY_ID equals co.ID
                                 where co.SORTNAME.Equals(id_pais.LAND)
                                 select new
                                 {
                                     st.ID,
                                     st.NAME,
                                     st.COUNTRY_ID
                                 }).ToList();

                //var id_waers = db.MONEDAs.Where(m => m.ACTIVO == true).ToList();//RSG 01.08.2018
                var id_waers = db.MONEDAs.Where(m => m.ACTIVO && (m.WAERS.Equals(id_bukrs.WAERS) | m.WAERS.Equals("USD"))).ToList();//RSG 01.08.2018

                List<TAT001.Entities.CITy> id_city = new List<TAT001.Entities.CITy>();

                //ViewBag.BUKRS = id_bukrs;
                ViewBag.SOCIEDAD_ID = id_bukrs;
                dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;
                ViewBag.PAIS_ID = id_pais;
                ViewBag.STATE_ID = new SelectList(id_states, "ID", dataTextField: "NAME", selectedValue: dOCUMENTO.ESTADO);
                ViewBag.CITY_ID = new SelectList(id_city, "ID", dataTextField: "NAME");
                ViewBag.MONEDA = new SelectList(id_waers, "WAERS", dataTextField: "WAERS", selectedValue: id_bukrs.WAERS);

                //Información del cliente
                var id_clientes = db.CLIENTEs.Where(c => c.LAND.Equals(p) && c.ACTIVO == true).ToList();
                //Obtener datos del payer
                Cadena cad = new Cadena();
                dOCUMENTO.PAYER_ID = cad.completaCliente(dOCUMENTO.PAYER_ID);
                CLIENTE payer = getCliente(dOCUMENTO.PAYER_ID);

                dOCUMENTO.VKORG = payer.VKORG;
                dOCUMENTO.VTWEG = payer.VTWEG;
                dOCUMENTO.SPART = payer.SPART;
                ViewBag.STCD1 = payer.STCD1;
                ViewBag.NAME1 = payer.NAME1;

                try
                {


                    //Obtener y dar formato a fecha
                    var fecha = dOCUMENTO.FECHAD.ToString();
                    string[] words = fecha.Split(' ');
                    //DateTime theTime = DateTime.ParseExact(fecha, //"06/04/2018 12:00:00 a.m."
                    //                        "dd/MM/yyyy hh:mm:ss t.t.",
                    //                        System.Globalization.CultureInfo.InvariantCulture,
                    //                        System.Globalization.DateTimeStyles.None);

                    DateTime theTime = DateTime.ParseExact(words[0], //"06/04/2018 12:00:00 a.m."
                                            "dd/MM/yyyy",
                                            System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None);
                    //ViewBag.FECHAD = theTime.ToString("yyyy-MM-dd");
                    ViewBag.FECHAD = theTime.ToString("dd/MM/yyyy");

                }
                catch (Exception e)
                {
                    ViewBag.FECHAD = "";
                }

                ViewBag.PERIODO = dOCUMENTO.PERIODO;
                ViewBag.EJERCICIO = dOCUMENTO.EJERCICIO;


                ViewBag.PAYER_ID = new SelectList(id_clientes, "KUNNR", dataTextField: "NAME1", selectedValue: dOCUMENTO.PAYER_ID);

                //Distribución
                //Información de categorías

                var id_cat = db.CATEGORIAs.Where(c => c.ACTIVO == true)
                                .Join(
                                db.CATEGORIATs.Where(ct => ct.SPRAS_ID == user.SPRAS_ID),
                                c => c.ID,
                                ct => ct.CATEGORIA_ID,
                                (c, ct) => new
                                {
                                    ct.CATEGORIA_ID,
                                    TEXT = ct.TXT50
                                }).ToList();



                ViewBag.CATEGORIA_ID = new SelectList(id_cat, "CATEGORIA_ID", "TEXT");
                List<TAT001.Entities.CITy> id_cityy = new List<TAT001.Entities.CITy>();
                ViewBag.BASE_ID = new SelectList(id_cityy, "CATEGORIA_ID", "TEXT");
            }

            ViewBag.tcambio = dOCUMENTO.TIPO_CAMBIO;
            ViewBag.MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_ML2;
            if (notas_soporte == null || notas_soporte == "")
            {
                notas_soporte = "";
            }
            ViewBag.notas_soporte = notas_soporte;
            ViewBag.UNAFACTURA = unafact;


            //Relacionada
            string id_d = "" + dOCUMENTO.DOCUMENTO_REF;
            decimal rel = 0;
            try
            {
                //rel = Convert.ToDecimal(Session["rel"].ToString());
                if (id_d == null || id_d.Equals(""))
                {
                    throw new Exception();
                }
                rel = Convert.ToDecimal(id_d);
                ViewBag.relacionada = "prelacionada";
                ViewBag.relacionadan = rel + "";
                ViewBag.TSOL_ANT = dOCUMENTO.TSOL_ID;

            }
            catch
            {
                rel = 0;
                ViewBag.relacionada = "";
                ViewBag.relacionadan = "";
                ViewBag.TSOL_ANT = dOCUMENTO.TSOL_ID;
            }

            //Obtener los valores de tsols
            List<TSOL> tsols_val = new List<TSOL>();
            List<TSOLT_MODBD> tsols_valbd = new List<TSOLT_MODBD>();
            try
            {
                tsols_val = db.TSOLs.ToList();
                tsols_valbd = tsols_val.Select(tsv => new TSOLT_MODBD
                {
                    ID = tsv.ID,
                    FACTURA = tsv.FACTURA
                }).ToList();
            }
            catch (Exception e)
            {

            }
            var tsols_valbdjs = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);
            ViewBag.TSOL_VALUES = tsols_valbdjs;
            ViewBag.TSOL_VALUES2 = tsols_valbdjs; //B20180625 MGC 2018.06.28

            ViewBag.SEL_NEG = select_neg;
            ViewBag.SEL_DIS = select_dis;
            ViewBag.BMONTO_APOYO = bmonto_apoyo;
            ViewBag.CATMAT = catmat;
            ViewBag.MONTO_DIS = monto_ret;

            //----------------------------RSG 18.05.2018
            if (dOCUMENTO.FECHAI_VIG == null)
                dOCUMENTO.FECHAI_VIG = DateTime.Now;
            if (dOCUMENTO.FECHAF_VIG == null)
                dOCUMENTO.FECHAF_VIG = DateTime.Now;
            //----------------------------RSG 18.05.2018
            string spras = FnCommon.ObtenerSprasId(db, User.Identity.Name);
            ViewBag.PERIODOS = new SelectList(db.PERIODOTs.Where(a => a.SPRAS_ID == spras).ToList(), "PERIODO_ID", "TXT50", DateTime.Now.Month);
            List<string> anios = new List<string>();
            int mas = 10;
            for (int i = 0; i < mas; i++)
            {
                if (!string.IsNullOrEmpty(id_d))
                    anios.Add((dOCUMENTO.FECHAI_VIG.Value.Year + i).ToString());
                else
                    anios.Add((DateTime.Now.Year + i).ToString());
            }
            string selYear1 = "";//ADD RSG 04.11.2018-------------------------------
            string selYear2 = "";
            if (string.IsNullOrEmpty(id_d))
            {
                selYear1 = DateTime.Now.Year.ToString();//ADD RSG 04.11.2018-------------------------------
                selYear2 = DateTime.Now.Year.ToString();
            }
            else
            {
                selYear1 = dOCUMENTO.FECHAI_VIG.Value.Year.ToString();//ADD RSG 04.11.2018-------------------------------
                selYear2 = dOCUMENTO.FECHAF_VIG.Value.Year.ToString();
            }

            if (!string.IsNullOrEmpty(id_d))
            {
                if (cal.anioMas(dOCUMENTO.FECHAI_VIG.Value)) selYear1 = (dOCUMENTO.FECHAI_VIG.Value.Year + 1).ToString();
                if (cal.anioMas(dOCUMENTO.FECHAF_VIG.Value)) selYear2 = (dOCUMENTO.FECHAF_VIG.Value.Year + 1).ToString();//ADD RSG 04.11.2018-------------------------------
            }
            ViewBag.ANIOS = new SelectList(anios, selYear1);//ADD RSG 31.10.2018
            ViewBag.ANIOSF = new SelectList(anios, selYear2);//ADD RSG 31.10.2018         
            dOCUMENTO.SOCIEDAD = db.SOCIEDADs.Find(dOCUMENTO.SOCIEDAD_ID);
            //----------------------------RSG 18.05.2018

            ViewBag.borrador = "error"; //MGC B20180625 MGC
            ViewBag.borradore = borrador_param; //B20180625 MGC2 2018.07.04 


            //B20180801 MGC Textos....................................................................................................
            ViewBag.apoyor = lbl_apr;
            ViewBag.volumenr = lbl_volr;
            ViewBag.apoyoe = lbl_ape;
            ViewBag.volumene = lbl_vole;
            //B20180801 MGC Textos....................................................................................................

            //-----------------------------------------------------------------LEJ 09.07.18
            dOCUMENTO.PAI = db.PAIS.Where(a => a.LAND.Equals(dOCUMENTO.PAIS_ID)).FirstOrDefault();
            if (dOCUMENTO.PAI != null)
            {
                ViewBag.miles = dOCUMENTO.PAI.MILES;//LEJGG 090718
                ViewBag.dec = dOCUMENTO.PAI.DECIMAL;//LEJGG 090718
            }
            //-----------------------------------------------------------------LEJ 09.07.18
            ViewBag.horaServer = DateTime.Now.Date.ToString().Split(new[] { ' ' }, 2)[1];//RSG 01.08.2018

            Warning w = new Warning();
            ViewBag.listaValid = w.listaW(dOCUMENTO.SOCIEDAD_ID, usuariotextos);

            var aa = (from n in db.TSOLTs.Where(x => x.SPRAS_ID == usuariotextos)
                      join t in db.TSOL_TREE
                      on n.TSOL_ID equals t.TSOL_ID
                      where n.TSOL.FACTURA && !n.TSOL_ID.StartsWith("O")
                      select new { n.TSOL_ID, n.TXT020 }).DistinctBy(x => x.TSOL_ID).DistinctBy(x => x.TXT020).ToList();
            ViewBag.TSOL_LIG = new SelectList(aa, "TSOL_ID", "TXT020", dOCUMENTO.TSOL_LIG);

            return View(dOCUMENTO);
        }
        [HttpPost]
        public string Borrador([Bind(Include = "NUM_DOC,TSOL_ID,TALL_ID,SOCIEDAD_ID,PAIS_ID,ESTADO,CIUDAD,PERIODO," +
            "EJERCICIO,TIPO_TECNICO,TIPO_RECURRENTE,CANTIDAD_EV,USUARIOC_ID,FECHAD,FECHAC,ESTATUS,ESTATUS_C,ESTATUS_SAP," +
            "ESTATUS_WF,DOCUMENTO_REF,NOTAS,MONTO_DOC_MD,MONTO_FIJO_MD,MONTO_BASE_GS_PCT_MD,MONTO_BASE_NS_PCT_MD,MONTO_DOC_ML," +
            "MONTO_FIJO_ML,MONTO_BASE_GS_PCT_ML,MONTO_BASE_NS_PCT_ML,MONTO_DOC_ML2,MONTO_FIJO_ML2,MONTO_BASE_GS_PCT_ML2," +
            "MONTO_BASE_NS_PCT_ML2,IMPUESTO,FECHAI_VIG,FECHAF_VIG,ESTATUS_EXT,SOLD_TO_ID,PAYER_ID,GRUPO_CTE_ID,CANAL_ID," +
            "MONEDA_ID,TIPO_CAMBIO,NO_FACTURA,FECHAD_SOPORTE,METODO_PAGO,NO_PROVEEDOR,PASO_ACTUAL,AGENTE_ACTUAL,FECHA_PASO_ACTUAL," +
            "VKORG,VTWEG,SPART,HORAC,FECHAC_PLAN,FECHAC_USER,HORAC_USER,CONCEPTO,PORC_ADICIONAL,PAYER_NOMBRE,PAYER_EMAIL," +
            "MONEDAL_ID,MONEDAL2_ID,TIPO_CAMBIOL,TIPO_CAMBIOL2,DOCUMENTOP, DOCUMENTOF, DOCUMENTOREC, GALL_ID, USUARIOD_ID, USUARIOC_ID")] DOCUMENTO dOCUMENTO,
            string notas_soporte, string unafact, string select_neg, string select_dis, string select_negi, string select_disi,
            string bmonto_apoyo, string monedadis, string chk_ligada)
        {

            string errorString = "";
            SOCIEDAD id_bukrs = new SOCIEDAD();
            string p = "";
            string res = "false";
            decimal monto_ret = Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD);
            if (select_neg == null)//RSG 09.07.2018
                select_neg = select_negi;
            if (select_dis == null)//RSG 09.07.2018
                select_dis = select_disi;

            if (ModelState.IsValid)
            {

                try
                {
                    //Obtener datos ocultos o deshabilitados                    
                    try
                    {
                        p = Session["pais"].ToString();
                        ViewBag.pais = p + ".png";
                    }
                    catch
                    {
                        ViewBag.pais = "mx.png";
                    }

                    try
                    {
                        dOCUMENTO.GALL_ID = db.TALLs.Where(t => t.ID == dOCUMENTO.TALL_ID).FirstOrDefault().GALL_ID;
                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Solicitudes", "Borrador");
                    }
                    id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p)).FirstOrDefault();
                    //Obtener el país
                    dOCUMENTO.PAIS_ID = p.ToUpper();//RSG 15.05.2018

                    //Tipo técnico
                    dOCUMENTO.TIPO_TECNICO = select_neg;

                    //B20180625 MGC 2018.06.25
                    //CANTIDAD_EV > 1 si son recurrentes
                    dOCUMENTO.CANTIDAD_EV = 1;

                    //B20180625 MGC 2018.06.25
                    //Obtener usuarioc
                    USUARIO u = db.USUARIOs.Find(User.Identity.Name);//RSG 02/05/2018
                    dOCUMENTO.PUESTO_ID = u.PUESTO_ID;//RSG 02/05/2018
                    dOCUMENTO.USUARIOC_ID = User.Identity.Name;

                    //Obtener SOCIEDAD_ID                     
                    dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;

                    //Fechac
                    dOCUMENTO.FECHAC = DateTime.Now;

                    //Horac
                    dOCUMENTO.HORAC = DateTime.Now.TimeOfDay;

                    //FECHAC_PLAN
                    dOCUMENTO.FECHAC_PLAN = DateTime.Now.Date;

                    //FECHAC_USER
                    dOCUMENTO.FECHAC_USER = DateTime.Now.Date;

                    //HORAC_USER
                    dOCUMENTO.HORAC_USER = DateTime.Now.TimeOfDay;

                    //Estatus
                    dOCUMENTO.ESTATUS = "N";

                    //Estatus wf
                    dOCUMENTO.ESTATUS_WF = "P";

                    ///////////////////Montos
                    //MONTO_DOC_MD
                    var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
                    dOCUMENTO.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);

                    TCambio tcambio = new TCambio();
                    //Obtener el monto de la sociedad
                    dOCUMENTO.MONTO_DOC_ML = tcambio.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);

                    //MONTO_DOC_ML2 
                    var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_ML2;
                    dOCUMENTO.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

                    //MONEDAL_ID moneda de la sociedad
                    dOCUMENTO.MONEDAL_ID = id_bukrs.WAERS;

                    //MONEDAL2_ID moneda en USD
                    dOCUMENTO.MONEDAL2_ID = "USD";

                    //Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
                    dOCUMENTO.TIPO_CAMBIOL = tcambio.getUkurs(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, out errorString);

                    //Tipo cambio dolares TIPO_CAMBIOL2
                    dOCUMENTO.TIPO_CAMBIOL2 = tcambio.getUkursUSD(dOCUMENTO.MONEDA_ID, "USD", out errorString);

                    //Obtener datos del payer
                    Cadena cad = new Cadena();
                    dOCUMENTO.PAYER_ID = cad.completaCliente(dOCUMENTO.PAYER_ID);
                    CLIENTE payer = getCliente(dOCUMENTO.PAYER_ID);

                    //Eliminar borrador anterior 
                    string borre = "";
                    borre = eliminarBorrador(dOCUMENTO);
                    //Guardar el borrador documento
                    if (borre != "")
                    {
                        try
                        {
                            dOCUMENTO.VKORG = payer.VKORG;
                            dOCUMENTO.VTWEG = payer.VTWEG;
                            dOCUMENTO.SPART = payer.SPART;
                        }
                        catch (Exception e)
                        {
                            Log.ErrorLogApp(e, "Solicitudes", "Borrador");
                        }
                        DOCUMENTBORR docb = new DOCUMENTBORR();
                        //docb = guardarBorrador(dOCUMENTO, id_bukrs, select_dis, monedadis, bmonto_apoyo);//RSG 09.07.2018
                        docb = guardarBorrador(dOCUMENTO, id_bukrs, select_dis, monedadis, bmonto_apoyo, chk_ligada);
                        db.DOCUMENTBORRs.Add(docb);
                        db.SaveChanges();
                        //B20180625 MGC 2018.06.27 Almacenar facturas
                        guardarBorradorf(dOCUMENTO, unafact);
                        guardarBorradorp(dOCUMENTO);
                        guardarBorradorn(dOCUMENTO.USUARIOC_ID, notas_soporte);
                        guardarBorradorRec(dOCUMENTO);
                        res = "true";
                    }


                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "Solicitudes", "Borrador");
                }
            }
            return res;
        }

        //public DOCUMENTBORR guardarBorrador(DOCUMENTO doc, SOCIEDAD id_bukrs, string dis, string monedadis, string bmonto_apoyo)
        public DOCUMENTBORR guardarBorrador(DOCUMENTO doc, SOCIEDAD id_bukrs, string dis, string monedadis, string bmonto_apoyo, string ligada)//RSG 09.07.2018
        {
            DOCUMENTBORR docb = new DOCUMENTBORR();
            docb.USUARIOC_ID = doc.USUARIOC_ID;
            docb.TSOL_ID = doc.TSOL_ID;
            docb.TALL_ID = doc.TALL_ID;
            docb.SOCIEDAD_ID = id_bukrs.BUKRS; ;
            docb.PAIS_ID = doc.PAIS_ID;
            docb.ESTADO = doc.ESTADO;
            docb.CIUDAD = doc.CIUDAD;
            docb.PERIODO = doc.PERIODO; //Cambiar tipo en bd
            docb.EJERCICIO = doc.EJERCICIO;
            docb.TIPO_TECNICO = doc.TIPO_TECNICO;
            docb.CANTIDAD_EV = doc.CANTIDAD_EV;
            docb.FECHAD = doc.FECHAD;
            docb.FECHAC = doc.FECHAC;
            docb.HORAC = doc.HORAC;
            //docb.FECHAC_PLAN = doc.FECHAC_PLAN;
            //docb.FECHAC_USER = doc.FECHAC_USER;
            //docb.HORAC_USER = doc.HORAC_USER;
            docb.CONCEPTO = doc.CONCEPTO;
            docb.NOTAS = doc.NOTAS;
            docb.MONTO_DOC_MD = doc.MONTO_DOC_MD;
            docb.MONTO_DOC_ML = doc.MONTO_DOC_ML;
            docb.MONTO_FIJO_ML2 = doc.MONTO_FIJO_ML2;
            docb.FECHAI_VIG = doc.FECHAI_VIG;
            docb.FECHAF_VIG = doc.FECHAF_VIG;
            docb.PAYER_ID = doc.PAYER_ID;
            docb.PAYER_NOMBRE = doc.PAYER_NOMBRE;
            docb.PAYER_EMAIL = doc.PAYER_EMAIL;
            docb.MONEDA_ID = doc.MONEDA_ID;
            docb.MONEDAL_ID = doc.MONEDAL_ID;
            docb.MONEDAL2_ID = doc.MONEDAL2_ID;
            docb.TIPO_CAMBIO = doc.TIPO_CAMBIO;
            docb.VKORG = doc.VKORG;
            docb.VTWEG = doc.VTWEG;
            docb.SPART = doc.SPART;
            docb.TIPO_TECNICO2 = dis;
            docb.MONEDA_DIS = monedadis;
            if (ligada != null && ligada != "off")
                docb.LIGADA = "X";
            try
            {
                docb.PORC_APOYO = Convert.ToDecimal(bmonto_apoyo);
            }
            catch (Exception)
            {
                docb.PORC_APOYO = null;
            }

            return docb;
        }

        public void guardarBorradorf(DOCUMENTO doc, string unafact)
        {
            bool uf = false;

            if (unafact == "true")
            {
                uf = true;
            }
            else if (unafact == "false")
            {
                uf = false;
            }

            try
            {
                for (int i = 0; i < doc.DOCUMENTOF.Count; i++)
                {
                    try
                    {
                        DOCUMENTOBORRF dbf = new DOCUMENTOBORRF();
                        dbf.USUARIOC_ID = doc.USUARIOC_ID;
                        dbf.POS = i + 1;
                        dbf.ACTIVO = uf;
                        dbf.FACTURA = doc.DOCUMENTOF[i].FACTURA;
                        dbf.SOCIEDAD = doc.DOCUMENTOF[i].SOCIEDAD;//jemo 18-07-2018
                        dbf.FECHA = doc.DOCUMENTOF[i].FECHA;
                        dbf.PROVEEDOR = doc.DOCUMENTOF[i].PROVEEDOR;
                        dbf.CONTROL = doc.DOCUMENTOF[i].CONTROL;
                        dbf.AUTORIZACION = doc.DOCUMENTOF[i].AUTORIZACION;
                        dbf.VENCIMIENTO = doc.DOCUMENTOF[i].VENCIMIENTO;
                        dbf.FACTURAK = doc.DOCUMENTOF[i].FACTURAK;
                        dbf.EJERCICIOK = doc.DOCUMENTOF[i].EJERCICIOK;
                        dbf.BILL_DOC = doc.DOCUMENTOF[i].BILL_DOC;
                        dbf.BELNR = doc.DOCUMENTOF[i].BELNR;
                        dbf.NAME1 = doc.DOCUMENTOF[i].DESCRIPCION;//jemo 18-07-2018
                        dbf.PAYER = doc.DOCUMENTOF[i].PAYER;//jemo 18-07-2018
                        dbf.IMPORTE_FAC = doc.DOCUMENTOF[i].IMPORTE_FAC;//jemo 18-07-2018
                        db.DOCUMENTOBORRFs.Add(dbf);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }


                }
            }
            catch (Exception e)
            {

            }
        }
        public void guardarBorradorp(DOCUMENTO doc)
        {
            try
            {

                for (int i = 0; i < doc.DOCUMENTOP.Count; i++)
                {
                    try
                    {
                        DOCUMENTOBORRP dbp = new DOCUMENTOBORRP();
                        dbp.USUARIOC_ID = doc.USUARIOC_ID;
                        dbp.POS = doc.DOCUMENTOP[i].POS;
                        if (doc.DOCUMENTOP[i].MATNR == null)
                        {
                            doc.DOCUMENTOP[i].MATNR = "";
                        }
                        dbp.MATNR = doc.DOCUMENTOP[i].MATNR;
                        if (doc.DOCUMENTOP[i].MATKL_ID == null || !string.IsNullOrEmpty(doc.DOCUMENTOP[i].MATNR))
                        {
                            doc.DOCUMENTOP[i].MATKL_ID = "";
                        }
                        dbp.MATKL = doc.DOCUMENTOP[i].MATKL_ID;
                        dbp.CANTIDAD = 1;
                        dbp.MONTO = doc.DOCUMENTOP[i].MONTO;
                        dbp.PORC_APOYO = doc.DOCUMENTOP[i].PORC_APOYO;
                        dbp.MONTO_APOYO = doc.DOCUMENTOP[i].MONTO_APOYO;
                        dbp.PRECIO_SUG = doc.DOCUMENTOP[i].PRECIO_SUG;
                        dbp.VOLUMEN_EST = doc.DOCUMENTOP[i].VOLUMEN_EST;
                        dbp.VOLUMEN_REAL = doc.DOCUMENTOP[i].VOLUMEN_REAL;
                        dbp.VIGENCIA_DE = doc.DOCUMENTOP[i].VIGENCIA_DE;
                        dbp.VIGENCIA_AL = doc.DOCUMENTOP[i].VIGENCIA_AL;
                        dbp.APOYO_EST = doc.DOCUMENTOP[i].APOYO_EST;
                        dbp.APOYO_REAL = doc.DOCUMENTOP[i].APOYO_REAL;

                        db.DOCUMENTOBORRPs.Add(dbp);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }


                }
            }
            catch (Exception e)
            {

            }
        }
        public void guardarBorradorn(string user, string notas)
        {

            if (notas != "")
            {
                try
                {
                    DOCUMENTOBORRN doc_notas = new DOCUMENTOBORRN();
                    doc_notas.USUARIOC_ID = user;
                    doc_notas.POS = 1;
                    doc_notas.STEP = 1;
                    doc_notas.USUARIO_ID = user;
                    doc_notas.TEXTO = notas.ToString();

                    db.DOCUMENTOBORRNs.Add(doc_notas);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }

        }
        public void guardarBorradorRec(DOCUMENTO doc)
        {
            try
            {

                for (int i = 0; i < doc.DOCUMENTOREC.Count; i++)
                {
                    try
                    {
                        DOCUMENTOBORRREC dbp = new DOCUMENTOBORRREC();
                        dbp.USUARIOC_ID = doc.USUARIOC_ID;
                        dbp.POS = doc.DOCUMENTOREC[i].POS;

                        dbp.DOC_REF = doc.DOCUMENTOREC[i].DOC_REF;
                        dbp.EJERCICIO = doc.DOCUMENTOREC[i].EJERCICIO;
                        dbp.ESTATUS = doc.DOCUMENTOREC[i].ESTATUS;
                        dbp.FECHAF = doc.DOCUMENTOREC[i].FECHAF;
                        dbp.FECHAV = doc.DOCUMENTOREC[i].FECHAV;
                        dbp.MONTO_BASE = doc.DOCUMENTOREC[i].MONTO_BASE;
                        dbp.MONTO_FIJO = doc.DOCUMENTOREC[i].MONTO_FIJO;
                        dbp.MONTO_GRS = doc.DOCUMENTOREC[i].MONTO_GRS;
                        dbp.MONTO_NET = doc.DOCUMENTOREC[i].MONTO_NET;
                        dbp.PERIODO = doc.DOCUMENTOREC[i].PERIODO;
                        dbp.PORC = doc.DOCUMENTOREC[i].PORC;

                        db.DOCUMENTOBORRRECs.Add(dbp);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }


                }
            }
            catch (Exception e)
            {

            }
        }

        public string eliminarBorrador(DOCUMENTO doc)
        {
            string res = "";

            try
            {
                db.DOCUMENTOBORRFs.RemoveRange(db.DOCUMENTOBORRFs.Where(d => d.USUARIOC_ID == doc.USUARIOC_ID));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }

            try
            {
                db.DOCUMENTOBORRPs.RemoveRange(db.DOCUMENTOBORRPs.Where(d => d.USUARIOC_ID == doc.USUARIOC_ID));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }

            try
            {
                db.DOCUMENTOBORRNs.RemoveRange(db.DOCUMENTOBORRNs.Where(d => d.USUARIOC_ID == doc.USUARIOC_ID));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            try//RSG 20.09.2018 ADD
            {
                db.DOCUMENTOBORRRECs.RemoveRange(db.DOCUMENTOBORRRECs.Where(d => d.USUARIOC_ID == doc.USUARIOC_ID));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }

            try
            {
                DOCUMENTBORR docb = new DOCUMENTBORR();
                docb = db.DOCUMENTBORRs.Find(doc.USUARIOC_ID);
                if (docb != null)
                {
                    db.DOCUMENTBORRs.Remove(docb);
                    db.SaveChanges();
                }
                res = "X";
            }
            catch (Exception e)
            {
                //string a = ""; //B20180625 MGC2 2018.07.04
            }
            return res;
        }
        //B20180625 MGC2 2018.07.04
        [HttpPost]
        public string eliminarBorrador(string user)
        {
            string res = "";

            DOCUMENTO doc = new DOCUMENTO();

            doc.USUARIOC_ID = user;

            res = eliminarBorrador(doc);

            return res;
        }

        [HttpPost]
        public FileResult Descargar(string archivo)
        {
            try
            {
                Models.PresupuestoModels carga = new Models.PresupuestoModels();
                string nombre = "", contentyp = "";
                carga.contDescarga(archivo, ref contentyp, ref nombre);
                Log.Info("Solicitudes-Descargar: nombre->" + nombre);
                Log.Info("Solicitudes-Descargar: contentyp->" + contentyp);
                Log.Info("Solicitudes-Descargar: archivo->" + archivo);

                string saveFileDev = ConfigurationManager.AppSettings["saveFileDev"];
                if (saveFileDev == "1")
                {
                    return File(archivo, contentyp, nombre);
                }
                else
                {
                    string serverDocs = ConfigurationManager.AppSettings["serverDocs"],
                    serverDocsUser = ConfigurationManager.AppSettings["serverDocsUser"],
                    serverDocsPass = ConfigurationManager.AppSettings["serverDocsPass"];
                    using (Impersonation.LogonUser(serverDocs, serverDocsUser, serverDocsPass, LogonType.NewCredentials))
                    {
                        Log.Info("Solicitudes-Descargar: Loggin->" + archivo);
                        FileStream fs = new FileStream(archivo, FileMode.Open, FileAccess.Read);
                        byte[] filebytes = new byte[fs.Length];
                        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                        fs.Dispose();
                        return File(filebytes, contentyp, nombre);
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e, "Solicitudes", "Descargar");
                return null;
            }

        }

        void ObtenerAnalisisSolicitud(DOCUMENTO D, decimal? monto = null, string[] categorias = null)
        {
            FormatosC format = new FormatosC();
            decimal montoProv = 0.0M;
            decimal montoApli = 0.0M;
            decimal remanente = 0.0M;
            bool esDocRef = false;
            bool esProv = false;
            bool esNC = false;

            if (monto != null)
            {
                D.MONTO_DOC_MD = monto;
            }

            if (D.DOCUMENTO_REF != null)
            {//Es hijo
                esProv = true;
                montoProv = db.DOCUMENTOes.First(x => x.NUM_DOC == D.DOCUMENTO_REF).MONTO_DOC_MD.Value;
            }
            else if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == D.NUM_DOC && x.ESTATUS_C == null))
            {
                //Es padre
                esProv = true;
                montoProv = D.MONTO_DOC_MD.Value;
            }

            if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == D.NUM_DOC && x.ESTATUS_C == null && x.ESTATUS_WF != "B"))
            {
                esDocRef = true;
                montoApli = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == D.NUM_DOC && x.ESTATUS_C == null && x.ESTATUS_WF != "B").Sum(x => x.MONTO_DOC_MD.Value);
            }
            else if (D.DOCUMENTO_REF != null)
            {
                esDocRef = true;
                if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == D.DOCUMENTO_REF && x.ESTATUS_C == null && x.ESTATUS_WF != "B"))
                {
                    montoApli = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == D.DOCUMENTO_REF && x.ESTATUS_C == null && x.ESTATUS_WF != "B").Sum(x => x.MONTO_DOC_MD.Value);
                }
            }
            if (montoProv > 0 && montoApli > 0)
            {
                remanente = montoProv - montoApli;
            }
            decimal impuesto = FnCommon.ObtenerImpuesto(db, D, ref esNC, categorias);
            if (D.TSOL.REVERSO)
            {
                montoApli = montoApli * -1;
            }
            ViewBag.montoSol = format.toShow(D.MONTO_DOC_MD.Value, ".");
            ViewBag.montoProv = (esProv ? format.toShow(montoProv, ".") : "-");
            ViewBag.montoApli = (esDocRef ? format.toShow(montoApli, ".") : "-");
            ViewBag.remanente = (esProv ? format.toShow(remanente, ".") : "-");
            ViewBag.impuesto = (esNC ? format.toShow(impuesto, ".") : "-");
            ViewBag.montoTotal = format.toShow(D.MONTO_DOC_MD.Value + impuesto, ".");
        }


        public STATE getEstado(int id)
        {
            STATE state = new STATE();

            using (TruequeEntities db = new TruequeEntities())
            {

                state = (from c in db.CITIES
                         join s in db.STATES
                         on c.STATE_ID equals s.ID
                         where s.ID == id
                         select s).FirstOrDefault();

            }

            return state;
        }


        public CLIENTE getCliente(string PAYER_ID)
        {
            CLIENTE payer = new CLIENTE();

            using (TruequeEntities db = new TruequeEntities())
            {

                payer = db.CLIENTEs.Where(c => c.KUNNR.Equals(PAYER_ID)).FirstOrDefault();

            }
            return payer;

        }


        // GET: Solicitudes/Create
        [HttpGet]
        public ActionResult Edit(string id_d)
        {

            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();

            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            var relacionada_neg = "";
            var relacionada_dis = "";
            List<TSOLT_MODBD> tsols_valbd = new List<TSOLT_MODBD>();//RSG 13.06.2018

            DOCUMENTO d = new DOCUMENTO();
            string errorString = "";
            int pagina = 204; //ID EN BASE DE DATOS
            String res = "";//B20180611
            using (TruequeEntities db = new TruequeEntities())
            {
                string p = "";
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller, 202);

                List<TREVERSAT> ldocr = new List<TREVERSAT>();
                decimal rel = 0;
                try
                {
                    if (id_d == null || id_d.Equals(""))
                    {
                        throw new Exception();
                    }
                    rel = Convert.ToDecimal(id_d);
                    ViewBag.relacionada = "prelacionada";
                    ViewBag.relacionadan = rel + "";

                }
                catch
                {
                    rel = 0;
                    ViewBag.relacionada = "";
                    ViewBag.relacionadan = "";
                }

                //Obtener los valores de tsols
                List<TSOL> tsols_val = new List<TSOL>();
                //List<TSOLT_MODBD> tsols_valbd = new List<TSOLT_MODBD>();//RSG 13.06.2018
                try
                {
                    tsols_val = db.TSOLs.ToList();
                    tsols_valbd = tsols_val.Select(tsv => new TSOLT_MODBD
                    {
                        ID = tsv.ID,
                        FACTURA = tsv.FACTURA
                    }).ToList();
                }
                catch (Exception e)
                {

                }
                if (rel > 0)
                {
                    d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).Include(a => a.USUARIO).Include(a => a.FLUJOes).FirstOrDefault();
                }
                //var tsols_valbdjs = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);//RSG 13.06.2018
                //ViewBag.TSOL_VALUES = tsols_valbdjs;
                //Add MGC B20180705 2018.07.05 conocer si se puede agregar renglones a la relacionada
                string addrowst = "X";
                bool addon = true;
                try
                {
                    addon = d.TSOL.ADICIONA;
                }
                catch (Exception)
                {

                }
                if (addon == true)
                {
                    addrowst = "X";
                }
                else
                {
                    addrowst = "";
                }
                ViewBag.addrowt = addrowst; //Add MGC B20180705 2018.07.05 conocer si se puede agregar renglones a la relacionada
                //Validar si es una reversa
                string tsol = "";
                string isrn = "";
                string isr = "";
                var freversa = (dynamic)null;
                try
                {
                    //if (tsol == null || tsol.Equals(""))
                    //{
                    //    throw new Exception();
                    //}
                    TSOL ts = tsols_val.Where(tsb => tsb.ID == d.TSOL_ID && tsb.REVERSO == true).FirstOrDefault();
                    if (ts != null)
                    {
                        isrn = "X";
                        isr = "preversa";
                        //freversa = theTime.ToString("yyyy-MM-dd"); ;
                        freversa = theTime.ToString("dd/MM/yyyy"); ;
                        //Obtener los tipos de reversas
                        try
                        {
                            //ldocr = db.TREVERSAs.Where(t => t.ACTIVO == true)
                            //    .Join(
                            //    db.TREVERSATs.Where(tt => tt.SPRAS_ID == user.SPRAS_ID),
                            //    t => t.ID,
                            //    tt => tt.TREVERSA_ID,
                            //    (t, tt) => new TREVERSAT
                            //    {
                            //        SPRAS_ID = tt.SPRAS_ID,
                            //        TREVERSA_ID = tt.TREVERSA_ID,
                            //        TXT100 = tt.TXT100
                            //    }).ToList();
                            //ldocr = db.TREVERSATs.Where(tt => tt.SPRAS_ID == user.SPRAS_ID).ToList();
                            ldocr = db.TREVERSATs.Where(a => a.TREVERSA.ACTIVO == true && a.SPRAS_ID == user.SPRAS_ID).ToList();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    isrn = "";
                    isr = "";
                    freversa = "";
                }

                ViewBag.reversa = isr;
                ViewBag.reversan = isrn;
                ViewBag.FECHAD_REV = freversa;
                ViewBag.TREVERSA = new SelectList(ldocr, "TREVERSA_ID", "TXT100");

                if (rel == 0)
                {
                    try//RSG 15.05.2018
                    {
                        p = Session["pais"].ToString();
                        ViewBag.pais = p + ".png";
                    }
                    catch
                    {
                        //ViewBag.pais = "mx.png";
                        return RedirectToAction("Pais", "Home");//
                    }
                }

                Session["spras"] = user.SPRAS_ID;

                List<TSOLT_MOD> list_sol = new List<TSOLT_MOD>();
                //tipo de solicitud
                if (ViewBag.reversa == "preversa")
                {

                    list_sol = tiposSolicitudesDao.ComboTiposSolicitudes(user.SPRAS_ID, null, true)
                        .Select(x => new TSOLT_MOD
                        {
                            SPRAS_ID = user.SPRAS_ID,
                            TSOL_ID = x.Value,
                            TEXT = x.Text
                        }).ToList();
                }
                else
                {
                    list_sol = tiposSolicitudesDao.ComboTiposSolicitudes(user.SPRAS_ID, null)
                        .Select(x => new TSOLT_MOD
                        {
                            SPRAS_ID = user.SPRAS_ID,
                            TSOL_ID = x.Value,
                            TEXT = x.Text
                        }).ToList();
                }



                //Obtener los documentos relacionados
                List<DOCUMENTO> docsrel = new List<DOCUMENTO>();

                SOCIEDAD id_bukrs = new SOCIEDAD();
                var id_pais = new PAI();
                var id_waers = db.MONEDAs.Where(m => m.ACTIVO == true).ToList();

                List<TAT001.Models.GALL_MOD> list_grupo = new List<GALL_MOD>();
                //Grupos de solicitud
                list_grupo = db.GALLs.Where(g => g.ACTIVO == true)
                                .Join(
                                db.GALLTs.Where(gt => gt.SPRAS_ID == user.SPRAS_ID),
                                g => g.ID,
                                gt => gt.GALL_ID,
                                (g, gt) => new GALL_MOD
                                {
                                    SPRAS_ID = gt.SPRAS_ID,
                                    GALL_ID = gt.GALL_ID,
                                    TEXT = gt == null ? "" : g.ID + " " + gt.TXT50
                                }).ToList();
                //clasificación
                //MGC B20180611
                List<TALLT_MOD> id_clas = new List<TALLT_MOD>();
                id_clas = db.TALLs.Where(t => t.ACTIVO == true)
                                .Join(
                                db.TALLTs.Where(tallt => tallt.SPRAS_ID == user.SPRAS_ID),
                                tall => tall.ID,
                                tallt => tallt.TALL_ID,
                                (tall, tallt) => new TALLT_MOD
                                {
                                    SPRAS_ID = tallt.SPRAS_ID,
                                    TALL_ID = tallt.TALL_ID,
                                    TXT50 = tallt == null ? "" : tallt.TXT50
                                })
                            .ToList();

                ViewBag.x_ligada = d.LIGADA;//LEJ 30.07.2018
                List<DOCUMENTOA> archivos = new List<DOCUMENTOA>();
                if (rel > 0)
                {
                    //d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).Include(a => a.USUARIO).Include(a => a.FLUJOes).FirstOrDefault();
                    if (d.ESTATUS_WF != "R" && d.ESTATUS_WF != "B" || !(d.USUARIOC_ID == User.Identity.Name || d.USUARIOD_ID == User.Identity.Name))//ADD RSG 30.10.2018
                        return RedirectToAction("Index", "Home");
                    ViewBag.x_ligada = d.LIGADA;//LEJ 30.07.2018
                    tsol = d.TSOL_ID;
                    docsrel = db.DOCUMENTOes.Where(docr => docr.DOCUMENTO_REF == rel).ToList();
                    id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == d.SOCIEDAD_ID && soc.ACTIVO == true).FirstOrDefault();
                    id_pais = db.PAIS.Where(pais => pais.LAND.Equals(d.PAIS_ID)).FirstOrDefault();//RSG 15.05.2018
                    ////d.DOCUMENTO_REF = rel;
                    relacionada_neg = d.TIPO_TECNICO;
                    ViewBag.TSOL_ANT = d.TSOL_ID;

                    if (d != null)
                    {

                        d.TSOL_ID = tsol;
                        ViewBag.TSOL_ID = new SelectList(list_sol, "TSOL_ID", "TEXT", selectedValue: d.TSOL_ID);
                        ViewBag.GALL_ID = new SelectList(list_grupo, "GALL_ID", "TEXT", selectedValue: d.GALL_ID);
                        ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50", selectedValue: d.TALL_ID); //B20180618 v1 MGC 2018.06.18
                        //TSOLT_MOD tsmod = new TSOLT_MOD();//RSG 01.08.2018
                        TSOLT tsmod = new TSOLT();
                        try
                        {
                            //tsmod = list_sol.Where(ids => ids.TSOL_ID.Equals(d.TSOL_ID)).FirstOrDefault();//RSG 01.08.2018
                            tsmod = db.TSOLTs.Where(ids => ids.TSOL_ID.Equals(d.TSOL_ID) && ids.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault();//RSG 01.08.2018

                        }
                        catch
                        {
                            //tsmod.TEXT = "";//RSG 01.08.2018
                            tsmod.TXT50 = "";//RSG 01.08.2018
                        }
                        //ViewBag.TSOL_IDI = tsmod.TEXT.ToString();//RSG 01.08.2018
                        ViewBag.TSOL_IDI = tsmod.TXT50.ToString();//RSG 01.08.2018
                        ////TAT001.Models.GALL_MOD gall_mod = list_grupo.Where(ids => ids.GALL_ID.Equals(d.GALL_ID)).FirstOrDefault();//RSG 07.09.2018
                        ////ViewBag.GALL_IDI = gall_mod.TEXT;//RSG 07.09.2018
                        ////ViewBag.GALL_IDI_VAL = gall_mod.GALL_ID;//RSG 07.09.2018
                        ViewBag.TALL_IDI = id_clas.Where(c => c.TALL_ID == d.TALL_ID).FirstOrDefault().TXT50; //B20180618 v1 MGC 2018.06.18
                        archivos = db.DOCUMENTOAs.Where(x => x.NUM_DOC.Equals(d.NUM_DOC)).ToList();

                        List<DOCUMENTOP> docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == d.NUM_DOC).ToList();//Documentos que se obtienen de la provisión
                        //List<DOCUMENTOP> docsrelp = new List<DOCUMENTOP>();//MGC B20180611

                        List<DOCUMENTOM> docml = new List<DOCUMENTOM>();//MGC B20180611----------------------------
                        decimal totalcatrel = 0;//MGC B20180611
                        if (docpl.Count > 0)
                        {
                            docml = db.DOCUMENTOMs.Where(docm => docm.NUM_DOC == d.NUM_DOC).ToList();
                        }

                        List<DOCUMENTOP> docsrelp = new List<DOCUMENTOP>();
                        List<DOCUMENTOM> docsrelm = new List<DOCUMENTOM>();//MGC B20180611----------------------------
                        //Obtener los documentos de la relacionada
                        if (docsrel.Count > 0)
                        {
                            docsrelp = docsrel
                                .Join(
                                db.DOCUMENTOPs,
                                docsl => docsl.NUM_DOC,
                                docspl => docspl.NUM_DOC,
                                (docsl, docspl) => new DOCUMENTOP
                                {
                                    NUM_DOC = docspl.NUM_DOC,
                                    POS = docspl.POS,
                                    MATNR = docspl.MATNR,
                                    MATKL = docspl.MATKL,
                                    CANTIDAD = docspl.CANTIDAD,
                                    MONTO = docspl.MONTO,
                                    PORC_APOYO = docspl.PORC_APOYO,
                                    MONTO_APOYO = docspl.MONTO_APOYO,
                                    PRECIO_SUG = docspl.PRECIO_SUG,
                                    VOLUMEN_EST = docspl.VOLUMEN_EST,
                                    VOLUMEN_REAL = docspl.VOLUMEN_REAL,
                                    APOYO_REAL = docspl.APOYO_REAL,
                                    APOYO_EST = docspl.APOYO_EST,
                                    VIGENCIA_DE = docspl.VIGENCIA_DE,
                                    VIGENCIA_AL = docspl.VIGENCIA_AL
                                }).ToList();

                            //MGC B20180611 Obtener los documentos en caso de distribución por categoría
                            if (docsrelp.Count > 0)
                            {
                                docsrelm = docsrelp
                                    .Join(
                                    db.DOCUMENTOMs,
                                    docspl => docspl.NUM_DOC,
                                    docsml => docsml.NUM_DOC,
                                    (docspl, docsml) => new DOCUMENTOM
                                    {
                                        NUM_DOC = docsml.NUM_DOC,
                                        POS_ID = docsml.POS_ID,
                                        POS = docsml.POS,
                                        MATNR = docsml.MATNR,
                                        PORC_APOYO = docsml.PORC_APOYO,
                                        APOYO_EST = docsml.APOYO_EST,
                                        APOYO_REAL = docsml.APOYO_REAL,
                                        VIGENCIA_DE = docsml.VIGENCIA_DE,
                                        VIGENCIA_A = docsml.VIGENCIA_A
                                    }).ToList();
                            }

                        }
                        DOCUMENTO dPadre = db.DOCUMENTOes.Find(d.DOCUMENTO_REF);
                        d.NUM_DOC = 0;
                        List<TAT001.Models.DOCUMENTOP_MOD> docsp = new List<DOCUMENTOP_MOD>();
                        var dis = "";
                        for (int j = 0; j < docpl.Count; j++)
                        {
                            try
                            {
                                //Documentos de la provisión
                                DOCUMENTOP_MOD docP = new DOCUMENTOP_MOD();
                                docP.NUM_DOC = d.NUM_DOC;
                                docP.POS = docpl[j].POS;
                                docP.MATNR = docpl[j].MATNR;
                                if (j == 0 && docP.MATNR == "")
                                {
                                    relacionada_dis = "C";
                                }
                                if (docpl[j].MATKL == null)
                                {
                                    docpl[j].MATKL = "";
                                }
                                docP.MATKL = docpl[j].MATKL;
                                docP.MATKL_ID = docpl[j].MATKL;
                                docP.CANTIDAD = 1;
                                docP.MONTO = docpl[j].MONTO;
                                docP.PORC_APOYO = docpl[j].PORC_APOYO;
                                docP.MONTO_APOYO = docpl[j].MONTO_APOYO;
                                docP.PRECIO_SUG = docpl[j].PRECIO_SUG;
                                docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                                docP.VOLUMEN_REAL = docpl[j].VOLUMEN_REAL;//RSG 01.08.2018
                                docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                                docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                                docP.APOYO_EST = docpl[j].APOYO_EST;
                                docP.APOYO_REAL = docpl[j].APOYO_REAL;
                                docP.ORIGINAL = "";
                                if (dPadre != null && dPadre.DOCUMENTOPs.Where(x => x.MATNR == docP.MATNR).ToList().Count > 0)
                                    docP.ORIGINAL = "X";

                                //Verificar si hay materiales en las relacionadas
                                if (docsrelp.Count > 0)
                                {
                                    List<DOCUMENTOP> docrel = new List<DOCUMENTOP>();

                                    if (docP.MATNR != null && docP.MATNR != "")
                                    {
                                        docrel = docsrelp.Where(docrell => docrell.MATNR == docP.MATNR).ToList();
                                    }
                                    else
                                    {
                                        docrel = docsrelp.Where(docrell => docrell.MATKL == docP.MATKL_ID).ToList();
                                        dis = "C";
                                    }

                                    for (int k = 0; k < docrel.Count; k++)
                                    {
                                        //Relacionada se obtiene el 
                                        decimal docr_vr = Convert.ToDecimal(docrel[k].VOLUMEN_REAL);
                                        decimal docr_ar = Convert.ToDecimal(docrel[k].APOYO_REAL);

                                        docP.VOLUMEN_EST -= docr_vr;
                                        docP.APOYO_EST -= docr_ar;

                                        if (dis == "C")
                                        {
                                            totalcatrel += Convert.ToDecimal(docrel[k].APOYO_REAL); //MGC B20180611
                                            //decimal docr_vr = Convert.ToDecimal(docrel[k].);
                                            //decimal docr_ar = Convert.ToDecimal(docrel[k].APOYO_REAL);
                                        }

                                    }
                                }

                                //Siempre tiene que ser igual a 0
                                if (docP.VOLUMEN_EST < 0)
                                {
                                    docP.VOLUMEN_EST = 0;
                                }
                                if (docP.APOYO_EST < 0)
                                {
                                    docP.APOYO_EST = 0;
                                }

                                docP.MATNR = docpl[j].MATNR.TrimStart('0');//RSG 07.06.2018
                                docsp.Add(docP);
                            }
                            catch (Exception e)
                            {
                                Log.ErrorLogApp(e, "Solicitudes", "Edit");
                            }
                        }
                        //MGC B20180611 Obtener las categorias con el detalle de cada material
                        if (docml.Count > 0)
                        {
                            res = grupoMaterialesRel(docpl, docml);
                        }

                        //Restar el valor del documento menos los relacionados
                        d.MONTO_DOC_MD = d.MONTO_DOC_MD - totalcatrel;//MGC B20180611

                        d.DOCUMENTOP = docsp;
                    }
                }
                else
                {
                    DOCUMENTBORR docb = new DOCUMENTBORR();//LEJ 23.07.2018
                    try
                    {
                        docb = db.DOCUMENTBORRs.Find(user.ID);//LEJ 23.07.2018
                        var _vbx = db.DOCUMENTOes.Where(x => x.NUM_DOC == 1000000770).FirstOrDefault();
                        ViewBag.LIGADA = docb.LIGADA;//LEJ 23.07.2018
                    }
                    catch (Exception e)
                    {
                        //
                    }
                    ViewBag.TSOL_ID = new SelectList(list_sol, "TSOL_ID", "TEXT");
                    ViewBag.GALL_ID = new SelectList(list_grupo, "GALL_ID", "TEXT");
                    ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50"); //B20180618 v1 MGC 2018.06.18
                    ViewBag.TSOL_IDI = "";
                    ViewBag.GALL_IDI = "";
                    ViewBag.TALL_IDI = ""; //B20180618 v1 MGC 2018.06.18
                    //id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p) && soc.ACTIVO == true).FirstOrDefault();//RSG 15.05.2018
                    id_pais = db.PAIS.Where(pais => pais.LAND.Equals(p)).FirstOrDefault();//RSG 15.05.2018
                    id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS.Equals(id_pais.SOCIEDAD_ID) && soc.ACTIVO == true).FirstOrDefault();//RSG 15.05.2018
                    ViewBag.TSOL_ANT = "";
                }

                ViewBag.files = archivos;

                //Select clasificación
                //var id_clas = db.TALLs.Where(t => t.ACTIVO == true)
                //                .Join(
                //                db.TALLTs.Where(tallt => tallt.SPRAS_ID == user.SPRAS_ID),
                //                tall => tall.ID,
                //                tallt => tallt.TALL_ID,
                //                (tall, tallt) => tallt)
                //            .ToList();

                //List<TAT001.Entities.GALL> id_clas = new List<TAT001.Entities.GALL>();//MGC B20180611
                //ViewBag.TALL_ID = new SelectList(id_clas, "TALL_ID", "TXT50"); //B20180618 v1 MGC 2018.06.18

                //Datos del país
                //var id_pais = db.PAIS.Where(pais => pais.LAND.Equals(id_bukrs.LAND)).FirstOrDefault();//RSG 15.05.2018

                var id_states = (from st in db.STATES
                                 join co in db.COUNTRIES
                                 on st.COUNTRY_ID equals co.ID
                                 where co.SORTNAME.Equals(id_pais.LAND)
                                 select new
                                 {
                                     st.ID,
                                     st.NAME,
                                     st.COUNTRY_ID
                                 }).ToList();



                List<TAT001.Entities.CITy> id_city = new List<TAT001.Entities.CITy>();
                id_waers = db.MONEDAs.Where(m => m.ACTIVO == true && (m.WAERS.Equals(id_bukrs.WAERS) | m.WAERS.Equals("USD"))).ToList();//RSG 01.08.2018

                ViewBag.SOCIEDAD_ID = id_bukrs;
                ViewBag.PAIS_ID = id_pais;
                ViewBag.STATE_ID = "";// new SelectList(id_states, "ID", dataTextField: "NAME");
                ViewBag.CITY_ID = "";// new SelectList(id_city, "ID", dataTextField: "NAME");
                ViewBag.MONEDA = new SelectList(id_waers, "WAERS", dataTextField: "WAERS", selectedValue: id_bukrs.WAERS); //Duda si cambia en la relacionada

                //Información del cliente
                var id_clientes = db.CLIENTEs.Where(c => c.LAND.Equals(p) && c.ACTIVO == true).ToList();

                ViewBag.PAYER_ID = new SelectList(id_clientes, "KUNNR", dataTextField: "NAME1");

                //Información de categorías
                var id_cat = db.CATEGORIAs.Where(c => c.ACTIVO == true)
                                .Join(
                                db.CATEGORIATs.Where(ct => ct.SPRAS_ID == user.SPRAS_ID),
                                c => c.ID,
                                ct => ct.CATEGORIA_ID,
                                (c, ct) => new
                                {
                                    ct.CATEGORIA_ID,
                                    TEXT = ct.TXT50
                                }).ToList();

                id_cat.RemoveRange(0, id_cat.Count);//RSG 28.05.2018
                ViewBag.CATEGORIA_ID = new SelectList(id_cat, "CATEGORIA_ID", "TEXT");
                List<TAT001.Entities.CITy> id_cityy = new List<TAT001.Entities.CITy>();
                ViewBag.BASE_ID = new SelectList(id_cityy, "CATEGORIA_ID", "TEXT");

                d.SOCIEDAD_ID = id_bukrs.BUKRS;
                d.PAIS_ID = id_pais.LAND;//RSG 18.05.2018
                if (rel > 0)//RSG 20.06.2018
                {
                    ViewBag.pais = d.PAIS_ID + ".png";
                }
                d.MONEDA_ID = id_bukrs.WAERS;
                var date = DateTime.Now.Date;


                ViewBag.tcambio = d.TIPO_CAMBIO;

            }//RSG 13.06.2018

            d.PERIODO = FnCommon.ObtenerPeriodoCalendario445(db, d.SOCIEDAD_ID, d.TSOL_ID, User.Identity.Name);
            d.EJERCICIO = Convert.ToString(DateTime.Now.Year);

            d.FECHAD = theTime;
            //ViewBag.FECHAD = theTime.ToString("yyyy-MM-dd");
            ViewBag.FECHAD = theTime.ToString("dd/MM/yyyy");
            ViewBag.PERIODO = d.PERIODO;
            ViewBag.EJERCICIO = d.EJERCICIO;
            ViewBag.STCD1 = "";
            ViewBag.PARVW = "";
            ViewBag.UNAFACTURA = "false";
            ViewBag.MONTO_DOC_ML2 = "";
            ViewBag.error = errorString;
            ViewBag.NAME1 = "";
            ViewBag.notas_soporte = "";

            //Prueba para agregar soporte a la tabla ahora información

            //DOCUMENTOF DF1 = new DOCUMENTOF();

            //DF1.POS = 1;
            //DF1.FACTURA = "FF1";
            //DF1.PROVEEDOR = "PP1";
            //DF1.FACTURAK = "FFK1";

            //DOCUMENTOF DF2 = new DOCUMENTOF();

            //DF2.POS = 2;
            //DF2.FACTURA = "FF2";
            //DF2.PROVEEDOR = "1000000001";
            //DF2.FACTURAK = "FFK2";

            //List<DOCUMENTOF> LD = new List<DOCUMENTOF>() { DF1, DF2 };

            //d.DOCUMENTOF = LD;

            ViewBag.SEL_NEG = relacionada_neg;
            ViewBag.SEL_DIS = relacionada_dis;
            if (d.PORC_APOYO != null)
                ViewBag.BMONTO_APOYO = Math.Round((decimal)d.PORC_APOYO, 2);
            ViewBag.CATMAT = res; //B20180618 v1 MGC 2018.06.18
            ViewBag.MONTO_DIS = "";

            //----------------------------RSG 31.10.2018
            Calendario445 cal = new Calendario445();
            if (d.FECHAI_VIG == null)
                d.FECHAI_VIG = DateTime.Now;
            if (d.FECHAF_VIG == null)
                d.FECHAF_VIG = DateTime.Now;
            int mesi = cal.getPeriodoF((DateTime)d.FECHAI_VIG);
            int mesf = cal.getPeriodoF((DateTime)d.FECHAF_VIG);

            string spras = Session["spras"].ToString();
            ViewBag.PERIODOS = new SelectList(db.PERIODOTs.Where(a => a.SPRAS_ID == spras).ToList(), "PERIODO_ID", "TXT50", mesi);
            ViewBag.PERIODOSF = new SelectList(db.PERIODOTs.Where(a => a.SPRAS_ID == spras).ToList(), "PERIODO_ID", "TXT50", mesf);
            //----------------------------RSG 31.10.2018
            //----------------------------RSG 18.05.2018
            ////string spras = Session["spras"].ToString();
            ////ViewBag.PERIODOS = new SelectList(db.PERIODOTs.Where(a => a.SPRAS_ID == spras).ToList(), "PERIODO_ID", "TXT50", DateTime.Now.Month);
            List<string> anios = new List<string>();
            int mas = 10;
            for (int i = 0; i < mas; i++)
            {
                anios.Add((d.FECHAI_VIG.Value.Year + i).ToString());
            }
            string selYear1 = d.FECHAI_VIG.Value.Year.ToString();//ADD RSG 04.11.2018-------------------------------
            string selYear2 = d.FECHAF_VIG.Value.Year.ToString();
            if (cal.anioMas(d.FECHAI_VIG.Value)) selYear1 = (d.FECHAI_VIG.Value.Year + 1).ToString();
            if (cal.anioMas(d.FECHAF_VIG.Value)) selYear2 = (d.FECHAF_VIG.Value.Year + 1).ToString();//ADD RSG 04.11.2018-------------------------------
            //ViewBag.ANIOS = new SelectList(anios, DateTime.Now.Year.ToString());
            ViewBag.ANIOS = new SelectList(anios, selYear1);//ADD RSG 31.10.2018
            ViewBag.ANIOSF = new SelectList(anios, selYear2);//ADD RSG 31.10.2018
            d.SOCIEDAD = db.SOCIEDADs.Find(d.SOCIEDAD_ID);
            //----------------------------RSG 18.05.2018
            //----------------------------RSG 12.06.2018
            if (id_d != null)
            {
                decimal numPadre = decimal.Parse(id_d);
                DOCUMENTO padre = db.DOCUMENTOes.Find(numPadre);
                if (padre != null)
                {
                    ViewBag.original = padre.MONTO_DOC_MD;
                    List<DOCUMENTO> dd = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == padre.NUM_DOC).ToList();
                    ViewBag.sumaRel = decimal.Parse("0.00"); ;
                    foreach (DOCUMENTO dos in dd)
                    {
                        ViewBag.sumaRel += (decimal)dos.MONTO_DOC_MD;
                    }
                }
            }
            //----------------------------RSG 12.06.2018


            //RSG 13.06.2018--------------------------------------------------------
            List<TAT001.Entities.DELEGAR> del = db.DELEGARs.Where(a => a.USUARIOD_ID.Equals(User.Identity.Name) && a.FECHAI <= DateTime.Now && a.FECHAF >= DateTime.Now && a.ACTIVO == true).ToList();
            if (del.Count > 0)
            {

                List<Delegados> users = new List<Delegados>();
                //List<PAI> pp = (from P in db.PAIS
                //                join C in db.CREADOR2 on P.LAND equals C.LAND
                //                where P.ACTIVO == true
                //                && C.ID == User.Identity.Name && C.ACTIVO == true
                //                select P).ToList();

                List<PAI> pp = (from P in db.PAIS.ToList()
                                join C in db.CLIENTEs.Where(x => x.ACTIVO == true).ToList()
                                on P.LAND equals C.LAND
                                join U in db.USUARIOFs.Where(x => x.USUARIO_ID == User.Identity.Name && x.ACTIVO == true)
                                on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                                where P.ACTIVO == true
                                select P).DistinctBy(x => x.LAND).ToList();


                List<Delegados> delegados = new List<Delegados>();
                foreach (DELEGAR de in del)
                {
                    //var pd = (from P in db.PAIS
                    //          join C in db.CREADOR2 on P.LAND equals C.LAND
                    //          where P.ACTIVO == true
                    //          && C.ID == de.USUARIO_ID && C.ACTIVO == true
                    //          select P).ToList();

                    List<PAI> pd = (from P in db.PAIS.ToList()
                                    join C in db.CLIENTEs.Where(x => x.ACTIVO == true).ToList()
                                    on P.LAND equals C.LAND
                                    join U in db.USUARIOFs.Where(x => x.USUARIO_ID == de.USUARIO_ID && x.ACTIVO == true)
                                    on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                                    where P.ACTIVO == true
                                    select P).DistinctBy(x => x.LAND).ToList();
                    pp.AddRange(pd);
                    Delegados delegado = new Delegados();
                    delegado.usuario = de.USUARIO_ID;
                    delegado.nombre = de.USUARIO_ID + " - " + de.USUARIO.NOMBRE + " " + de.USUARIO.APELLIDO_P + " " + de.USUARIO.APELLIDO_M;
                    delegado.LISTA = pd;
                    if (delegado.LISTA.Count > 0)
                        delegados.Add(delegado);
                }
                PAI pq = pp.Where(a => a.LAND == d.PAIS_ID).FirstOrDefault();
                if (pq != null)
                {
                    Delegados de = new Delegados();
                    de.usuario = User.Identity.Name;
                    USUARIO uu = db.USUARIOs.Find(User.Identity.Name);
                    de.nombre = User.Identity.Name + " - " + uu.NOMBRE + " " + uu.APELLIDO_P + " " + uu.APELLIDO_M;
                    de.LISTA = new List<PAI>();
                    de.LISTA.Add(pq);
                    users.Add(de);
                }
                foreach (Delegados de in delegados)
                {
                    PAI pqq = de.LISTA.Where(a => a.LAND == d.PAIS_ID).FirstOrDefault();
                    if (pqq != null)
                        users.Add(de);
                }

                ViewBag.USUARIOD_ID = new SelectList(users, "usuario", "nombre", users[0].usuario);
            }

            var tsols_valbdjs = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);
            ViewBag.TSOL_VALUES = tsols_valbdjs;
            List<FACTURASCONF> ffc = db.FACTURASCONFs.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) && a.PAIS_ID.Equals(d.PAIS_ID)).ToList();
            foreach (var item in tsols_valbd)
            {
                FACTURASCONF fc = ffc.FirstOrDefault(a => a.TSOL.Equals(item.ID));
                if (fc == null)
                    item.FACTURA = false;
            }
            ViewBag.TSOL_VALUES2 = JsonConvert.SerializeObject(tsols_valbd, Formatting.Indented);

            //RSG 13.06.2018--------------------------------------------------------
            //}//RSG 13.06.2018--------------------------------------------------------
            d.NUM_DOC = decimal.Parse(id_d);
            //ViewBag.workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            var vbFl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).OrderBy(a => a.POS).ToList();
            FLUJO fvbfl = new FLUJO();
            //recuperamos si existe algun valor en fljunegoc
            var flng = db.FLUJNEGOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).ToList();
            if (flng.Count > 0)
            {
                for (int i = 0; i < flng.Count; i++)
                {
                    var kn = flng[i].KUNNR;
                    var clName = db.CLIENTEs.Where(c => c.KUNNR == kn).Select(s => s.NAME1).FirstOrDefault();
                    fvbfl = new FLUJO();
                    fvbfl.NUM_DOC = flng[i].NUM_DOC;
                    fvbfl.FECHAC = flng[i].FECHAC;
                    fvbfl.FECHAM = flng[i].FECHAM;
                    fvbfl.USUARIOA_ID = clName + "(Cliente)";
                    fvbfl.COMENTARIO = flng[i].COMENTARIO;
                    vbFl.Add(fvbfl);
                }
            }
            ViewBag.workflow = vbFl;

            //string spras = Session["spras"].ToString();
            ViewBag.soportes = (from C in db.CONSOPORTEs
                                join T in db.TSOPORTETs
                                on C.TSOPORTE_ID equals T.TSOPORTE_ID
                                where C.TSOL_ID == d.TSOL_ID
                                && T.SPRAS_ID == spras
                                select new Soporte { TSOPORTE_ID = C.TSOPORTE_ID, OBLIGATORIO = C.OBLIGATORIO, TXT50 = T.TXT50 }).ToList();

            ViewBag.miles = d.PAI.MILES;//LEJGG 090718
            ViewBag.dec = d.PAI.DECIMAL;//LEJGG 090718
            //LEJ 24.07.2018------------------------------------------------------------
            var nmid = decimal.Parse(id_d);
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == nmid).FirstOrDefault();
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                    && a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                    && a.SPART.Equals(dOCUMENTO.SPART)
                                                    && a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
            dOCUMENTO.DOCUMENTOF = db.DOCUMENTOFs.Where(a => a.NUM_DOC.Equals(dOCUMENTO.NUM_DOC)).ToList();

            if (ffc.Any(x => x.TSOL == d.TSOL_ID && x.ACTIVO) && dOCUMENTO.DOCUMENTOF == null)//ADD RSG 10.12.2018
            {
                DOCUMENTOF dff = new DOCUMENTOF();
                dff.POS = 1;

                dOCUMENTO.DOCUMENTOF.Add(dff);
            }
            //LEJ 03.08.2018----i
            for (int i = 0; i < dOCUMENTO.DOCUMENTOF.Count; i++)
            {
                var _x = dOCUMENTO.DOCUMENTOF[i].PAYER;
                dOCUMENTO.DOCUMENTOF[i].DESCRIPCION = db.CLIENTEs.Where(c => c.KUNNR == _x).Select(o => o.NAME1).FirstOrDefault();
                dOCUMENTO.DOCUMENTOF[i].SOCIEDAD = dOCUMENTO.SOCIEDAD_ID;
            }
            //LEJ 03.08.2018----t
            DocumentoFlujo DF = new DocumentoFlujo();
            DF.D = dOCUMENTO;
            ViewBag.df = DF;
            DOCUMENTOREC dr = new DOCUMENTOREC();
            d.DOCUMENTOREC = db.DOCUMENTORECs.Where(x => x.NUM_DOC == dOCUMENTO.NUM_DOC).ToList();
            //LEJ 24.07.2018------------------------------------------------------------
            ViewBag.horaServer = DateTime.Now.Date.ToString().Split(new[] { ' ' }, 2)[1];//RSG 01.08.2018
            Warning w = new Warning();
            ViewBag.listaValid = w.listaW(d.SOCIEDAD_ID, "ES");//RSG 07.09.2018

            DateTime fecha = DateTime.Now.Date;
            List<DELEGAR> backup = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(User.Identity.Name) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO == true).ToList();
            if (backup.Count > 0)
            {
                ViewBag.USUARIO_BACKUPID = backup.First().USUARIOD_ID;
            }
            //ADD RSG 31.10.2018------------------------------
            d.DOCUMENTORAN = new List<DOCUMENTORAN>();
            foreach (DOCUMENTOREC drec in d.DOCUMENTOREC.ToList())
            {
                d.DOCUMENTORAN.AddRange(drec.DOCUMENTORANs.ToList());
            }
            //ADD RSG 31.10.2018------------------------------

            //Tab_Fin Análisis Solicitud
            ObtenerAnalisisSolicitud(d);
            var aa = (from n in db.TSOLTs.Where(x => x.SPRAS_ID == spras)
                      join t in db.TSOL_TREE
                      on n.TSOL_ID equals t.TSOL_ID
                      where n.TSOL.FACTURA && !n.TSOL_ID.StartsWith("O")
                      select new { n.TSOL_ID, n.TXT020 }).DistinctBy(x => x.TSOL_ID).DistinctBy(x => x.TXT020).ToList();
            ViewBag.TSOL_LIG = new SelectList(aa, "TSOL_ID", "TXT020", d.TSOL_LIG);

            return View(d);
        }

        // POST: Solicitudes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NUM_DOC,TSOL_ID,TALL_ID,SOCIEDAD_ID,PAIS_ID,ESTADO,CIUDAD,PERIODO," +
            "EJERCICIO,TIPO_TECNICO,TIPO_RECURRENTE,CANTIDAD_EV,USUARIOC_ID,USUARIOD_ID,FECHAD,FECHAC,ESTATUS,ESTATUS_C,ESTATUS_SAP," +
            "ESTATUS_WF,DOCUMENTO_REF,NOTAS,MONTO_DOC_MD,MONTO_FIJO_MD,MONTO_BASE_GS_PCT_MD,MONTO_BASE_NS_PCT_MD,MONTO_DOC_ML," +
            "MONTO_FIJO_ML,MONTO_BASE_GS_PCT_ML,MONTO_BASE_NS_PCT_ML,MONTO_DOC_ML2,MONTO_FIJO_ML2,MONTO_BASE_GS_PCT_ML2," +
            "MONTO_BASE_NS_PCT_ML2,IMPUESTO,FECHAI_VIG,FECHAF_VIG,ESTATUS_EXT,SOLD_TO_ID,PAYER_ID,GRUPO_CTE_ID,CANAL_ID," +
            "MONEDA_ID,TIPO_CAMBIO,NO_FACTURA,FECHAD_SOPORTE,METODO_PAGO,NO_PROVEEDOR,PASO_ACTUAL,AGENTE_ACTUAL,FECHA_PASO_ACTUAL," +
            "VKORG,VTWEG,SPART,HORAC,FECHAC_PLAN,FECHAC_USER,HORAC_USER,CONCEPTO,PORC_ADICIONAL,PAYER_NOMBRE,PAYER_EMAIL," +
            "MONEDAL_ID,MONEDAL2_ID,TIPO_CAMBIOL,TIPO_CAMBIOL2,DOCUMENTOP, DOCUMENTOF, DOCUMENTOREC, GALL_ID, USUARIOD_ID, OBJQ_PORC, DOCUMENTORAN,TSOL_LIG")] DOCUMENTO dOCUMENTO,
                IEnumerable<HttpPostedFileBase> files_soporte, string notas_soporte, string[] labels_soporte, string unafact,
                string FECHAD_REV, string TREVERSA, string select_neg, string select_dis, string select_negi, string select_disi,
                string bmonto_apoyo, string catmat, string txt_sop_borr, string txt_flujo, string chk_ligada)
        {
            USUARIO user = db.USUARIOs.Find(User.Identity.Name);
            string usuariotextos = user.SPRAS_ID;
            if (ModelState.IsValid)
            {
                DOCUMENTO d = db.DOCUMENTOes.Find(dOCUMENTO.NUM_DOC);
                string errorString = "";

                if (d.ESTATUS_WF == "B")
                {
                    d.PERIODO = dOCUMENTO.PERIODO;
                }
                //ADD RSG 20.08.2018-----------------------------START
                if (d.TSOL.REVERSO)
                {
                    try
                    {
                        if (TREVERSA != null)
                        {
                            DOCUMENTOR docr = db.DOCUMENTORs.Find(dOCUMENTO.NUM_DOC);
                            docr.NUM_DOC = dOCUMENTO.NUM_DOC;
                            docr.TREVERSA_ID = Convert.ToInt32(TREVERSA);
                            docr.USUARIOC_ID = User.Identity.Name;
                            docr.FECHAC = DateTime.Now;
                            docr.COMENTARIO = notas_soporte.ToString();

                            db.Entry(docr).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Solicitudes", "Edit");
                    }
                }
                else
                {

                    //ADD RSG 20.08.2018-----------------------------START

                    d.ESTADO = dOCUMENTO.ESTADO;
                    d.TSOL_LIG = dOCUMENTO.TSOL_LIG;
                    d.CIUDAD = dOCUMENTO.CIUDAD;
                    d.CONCEPTO = dOCUMENTO.CONCEPTO;
                    d.NOTAS = dOCUMENTO.NOTAS;
                    d.TIPO_TECNICO = select_neg;

                    if (chk_ligada == "on")//ADD RSG 02.11.2018
                        d.TIPO_TECNICO = "P";

                    ////if (d.PAYER_ID != dOCUMENTO.PAYER_ID)
                    ////{
                    ////    d.PAYER_ID = dOCUMENTO.PAYER_ID;
                    ////    CLIENTE c = db.CLIENTEs.Where(a => a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).FirstOrDefault();
                    ////    if (c != null)
                    ////    {
                    ////        d.VKORG = c.VKORG;
                    ////        d.VTWEG = c.VTWEG;
                    ////        d.SPART = c.SPART;
                    ////    }
                    ////}
                    d.PAYER_EMAIL = dOCUMENTO.PAYER_EMAIL;

                    d.PAYER_NOMBRE = dOCUMENTO.PAYER_NOMBRE;

                    d.FECHAF_VIG = dOCUMENTO.FECHAF_VIG;
                    d.FECHAI_VIG = dOCUMENTO.FECHAI_VIG;

                    if (d.DOCUMENTO_REF > 0)
                    {
                        DOCUMENTO dr = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == d.DOCUMENTO_REF).FirstOrDefault();
                        //dOCUMENTO.TSOL_ID = d.TSOL_ID;
                        d.ESTADO = dr.ESTADO;
                        d.CIUDAD = dr.CIUDAD;
                        d.PAYER_ID = dr.PAYER_ID;
                        d.CONCEPTO = dr.CONCEPTO;
                        //d.NOTAS = dr.NOTAS;
                        d.FECHAI_VIG = dr.FECHAI_VIG;
                        d.FECHAF_VIG = dr.FECHAF_VIG;
                        //d.PAYER_NOMBRE = dr.PAYER_NOMBRE;
                        //d.PAYER_EMAIL = dr.PAYER_EMAIL;
                        d.TIPO_CAMBIO = dr.TIPO_CAMBIO;
                        d.GALL_ID = dr.GALL_ID;
                        d.TALL_ID = dr.TALL_ID;//RSG 12.06.2018
                        //Obtener el país
                        d.PAIS_ID = dr.PAIS_ID;//RSG 15.05.2018
                        d.TIPO_TECNICO = dr.TIPO_TECNICO; //B20180618 v1 MGC 2018.06.18

                    }
                    ///////////////////Montos
                    //MONTO_DOC_MD
                    var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
                    d.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);
                    if (bmonto_apoyo == "") bmonto_apoyo = "0";//RSG 09.07.2018
                    d.PORC_APOYO = decimal.Parse(bmonto_apoyo);//RSG 29.06.2018

                    //string errorString = "";
                    TCambio tcambio = new TCambio();
                    //Obtener el monto de la sociedad
                    d.MONTO_DOC_ML = tcambio.getValSoc(d.SOCIEDAD.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(d.MONTO_DOC_MD), out errorString);
                    if (!errorString.Equals(""))
                    {
                        throw new Exception();
                    }

                    //MONTO_DOC_ML2 
                    var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_ML2;
                    d.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

                    //MONEDAL_ID moneda de la sociedad
                    d.MONEDAL_ID = d.SOCIEDAD.WAERS;

                    //MONEDAL2_ID moneda en USD
                    d.MONEDAL2_ID = "USD";

                    //Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
                    d.TIPO_CAMBIOL = tcambio.getUkurs(d.SOCIEDAD.WAERS, d.MONEDA_ID, out errorString);

                    //Tipo cambio dolares TIPO_CAMBIOL2
                    d.TIPO_CAMBIOL2 = tcambio.getUkursUSD(d.MONEDA_ID, "USD", out errorString);

                    //Se cambio de pocisión //B20180618 v1 MGC 2018.06.18--------------------------------------
                    //Si la distribución es categoría se obtienen las categorías
                    List<string> listcat = new List<string>();
                    decimal totalcats = 0;
                    List<CategoriaMaterial> listcatm = new List<CategoriaMaterial>();
                    if (select_dis == "C")
                    {
                        for (int j = 0; j < dOCUMENTO.DOCUMENTOP.Count; j++)
                        {
                            string cat = dOCUMENTO.DOCUMENTOP.ElementAt(j).MATKL_ID.ToString();
                            listcat.Add(cat);
                        }

                        listcatm = grupoMaterialesController(listcat, d.VKORG, d.SPART, d.PAYER_ID, d.SOCIEDAD_ID, out totalcats);
                    }

                    d.ESTATUS_WF = txt_flujo;//ADD RSG 30.10.2018

                    //Se cambio de pocisión //B20180618 v1 MGC 2018.06.18--------------------------------------
                    //Guardar los documentos p para el documento guardado
                    try
                    {
                        //Agregar materiales existentes para evitar que en la vista se hayan agregado o quitado
                        List<DOCUMENTOP> docpl = new List<DOCUMENTOP>();
                        if (dOCUMENTO.DOCUMENTO_REF > 0)
                        {
                            docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).ToList();

                            //Add MGC B20180705 2018.07.05 Agregar materiales agregados en la vista
                            if (d.TSOL.ADICIONA && select_disi == "M" && select_negi == "M")
                            {

                                for (int h = 0; h < dOCUMENTO.DOCUMENTOP.Count; h++)
                                {
                                    string mmatnr = dOCUMENTO.DOCUMENTOP[h].MATNR.TrimStart('0');
                                    DOCUMENTOP docmode = docpl.FirstOrDefault(dcp => dcp.MATNR.TrimStart('0') == mmatnr);

                                    //Agregarlo a la lista
                                    if (docmode == null)
                                    {
                                        DOCUMENTOP docadd = new DOCUMENTOP();

                                        docadd.MATNR = dOCUMENTO.DOCUMENTOP[h].MATNR;
                                        if (dOCUMENTO.DOCUMENTOP[h].MATKL_ID == null)
                                        {
                                            dOCUMENTO.DOCUMENTOP[h].MATKL_ID = "";
                                        }
                                        docadd.MATKL = dOCUMENTO.DOCUMENTOP[h].MATKL_ID;
                                        docadd.CANTIDAD = 1;
                                        docadd.MONTO = dOCUMENTO.DOCUMENTOP[h].MONTO;
                                        docadd.PORC_APOYO = dOCUMENTO.DOCUMENTOP[h].PORC_APOYO;
                                        docadd.MONTO_APOYO = dOCUMENTO.DOCUMENTOP[h].MONTO_APOYO;
                                        docadd.PRECIO_SUG = dOCUMENTO.DOCUMENTOP[h].PRECIO_SUG;
                                        docadd.VOLUMEN_EST = dOCUMENTO.DOCUMENTOP[h].VOLUMEN_EST;
                                        docadd.VOLUMEN_REAL = dOCUMENTO.DOCUMENTOP[h].VOLUMEN_REAL;
                                        docadd.VIGENCIA_DE = dOCUMENTO.DOCUMENTOP[h].VIGENCIA_DE;
                                        docadd.VIGENCIA_AL = dOCUMENTO.DOCUMENTOP[h].VIGENCIA_AL;
                                        docadd.APOYO_EST = dOCUMENTO.DOCUMENTOP[h].APOYO_EST;
                                        docadd.APOYO_REAL = dOCUMENTO.DOCUMENTOP[h].APOYO_REAL;

                                        docpl.Add(docadd);
                                    }
                                }
                            }

                            for (int j = 0; j < docpl.Count; j++)
                            {
                                try
                                {
                                    DOCUMENTOP_MOD docmod = new DOCUMENTOP_MOD();
                                    var cat = "";

                                    if (docpl[j].MATNR != null && docpl[j].MATNR != "")
                                    {
                                        string mmatnr = docpl[j].MATNR.TrimStart('0');//RSG 07.06.2018
                                        docmod = dOCUMENTO.DOCUMENTOP.FirstOrDefault(docp => docp.MATNR == mmatnr);
                                    }
                                    else
                                    {
                                        docmod = dOCUMENTO.DOCUMENTOP.FirstOrDefault(docp => docp.MATKL_ID == docpl[j].MATKL);
                                        cat = "C";
                                    }
                                    DOCUMENTOP docP = new DOCUMENTOP();
                                    //Si lo encuentra meter valores de la base de datos y vista
                                    if (docmod != null)
                                    {
                                        docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        docP.POS = docmod.POS;
                                        if (docmod.MATNR == null || docmod.MATNR == "")
                                        {
                                            docmod.MATNR = "";
                                        }
                                        docP.MATNR = docmod.MATNR;
                                        docP.MATNR = new Cadena().completaMaterial(docP.MATNR);//RSG 07.06.2018
                                        docP.MATKL = docmod.MATKL_ID;
                                        docP.CANTIDAD = 1;
                                        docP.MONTO = docmod.MONTO;
                                        docP.PORC_APOYO = docmod.PORC_APOYO;
                                        docP.MONTO_APOYO = docP.MONTO * (docP.PORC_APOYO / 100);
                                        docP.MONTO_APOYO = Math.Round(docP.MONTO_APOYO, 2);//RSG 16.05.2018
                                        docP.PRECIO_SUG = docmod.PRECIO_SUG;
                                        docP.VOLUMEN_EST = docmod.VOLUMEN_EST;
                                        docP.VOLUMEN_REAL = docmod.VOLUMEN_REAL;
                                        docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                                        docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                                        docP.APOYO_EST = docmod.APOYO_EST;
                                        docP.APOYO_REAL = docmod.APOYO_REAL;


                                    }
                                    else
                                    {
                                        docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        docP.POS = docpl[j].POS;
                                        docP.MATNR = docpl[j].MATNR;
                                        docP.MATKL = docpl[j].MATKL;
                                        docP.CANTIDAD = 1;
                                        docP.MONTO = docpl[j].MONTO;
                                        docP.MONTO_APOYO = docP.MONTO * (docpl[j].PORC_APOYO / 100);
                                        docP.MONTO_APOYO = docpl[j].MONTO_APOYO;
                                        docP.PRECIO_SUG = docpl[j].PRECIO_SUG;
                                        docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                                        docP.VOLUMEN_REAL = docpl[j].VOLUMEN_REAL;
                                        docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                                        docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                                        docP.APOYO_EST = docpl[j].APOYO_EST;
                                        docP.APOYO_REAL = docpl[j].APOYO_REAL;
                                    }

                                    //Se agrego para las relacionadas //B20180618 v1 MGC 2018.06.18--------------------------------------
                                    //If matnr es "" agregar los materiales de la categoría
                                    List<DOCUMENTOM> docml = new List<DOCUMENTOM>();
                                    if (docP.MATNR == "")
                                    {
                                        string col = "";
                                        if (Convert.ToDecimal(docP.APOYO_EST) > 0)
                                        {
                                            col = "E";
                                        }
                                        else if (Convert.ToDecimal(docP.APOYO_REAL) > 0)
                                        {
                                            col = "R";
                                        }
                                        docml = addCatItems(listcatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                            Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, select_neg, select_dis, totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);
                                    }

                                    //Obtener apoyo estimado
                                    decimal apoyo_esti = 0;
                                    decimal apoyo_real = 0;
                                    //Categoría por monto
                                    if (select_neg == "M")
                                    {
                                        //Obtener el apoyo real o estimado para cada material
                                        var cantmat = docml.Count;
                                        try
                                        {
                                            apoyo_esti = Convert.ToDecimal(docP.APOYO_EST) / cantmat;

                                        }
                                        catch (Exception)
                                        {
                                            apoyo_esti = 0;
                                        }

                                        try
                                        {
                                            apoyo_real = Convert.ToDecimal(docP.APOYO_REAL) / cantmat;

                                        }
                                        catch (Exception)
                                        {
                                            apoyo_real = 0;
                                        }

                                        for (int k = 0; k < docml.Count; k++)
                                        {
                                            try
                                            {
                                                DOCUMENTOM docM = docml[k];
                                                docM.POS = k + 1;
                                                docM.APOYO_REAL = apoyo_real;
                                                docM.APOYO_EST = apoyo_esti;

                                                //////db.DOCUMENTOMs.Add(docM);
                                                //////db.SaveChanges();//RSG
                                            }
                                            catch (Exception e)
                                            {
                                                Log.ErrorLogApp(e, "Solicitudes", "Edit-DOCUMENTOM");
                                            }
                                        }
                                    }
                                    else if (select_neg == "P")
                                    {
                                        //Categoría por porcentaje
                                        for (int k = 0; k < docml.Count; k++)
                                        {
                                            try
                                            {
                                                DOCUMENTOM docM = docml[k];
                                                docM.POS = k + 1;

                                                //////db.DOCUMENTOMs.Add(docM);
                                                //////db.SaveChanges();//RSG
                                            }
                                            catch (Exception e)
                                            {
                                                Log.ErrorLogApp(e, "Solicitudes", "Edit-DOCUMENTOM");
                                            }
                                        }

                                    }
                                    //Se agrego para las relacionadas //B20180618 v1 MGC 2018.06.18--------------------------------------
                                    dOCUMENTO.DOCUMENTOPs.Add(docP);
                                }
                                catch (Exception e)
                                {
                                    Log.ErrorLogApp(e, "Solicitudes", "Edit-DOCUMENTOP");
                                }

                            }
                        }
                        else
                        {

                            for (int j = 0; j < dOCUMENTO.DOCUMENTOP.Count; j++)
                            {
                                try
                                {
                                    DOCUMENTOP docP = new DOCUMENTOP();

                                    docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    docP.POS = dOCUMENTO.DOCUMENTOP.ElementAt(j).POS;
                                    if (dOCUMENTO.DOCUMENTOP.ElementAt(j).MATNR == null)
                                    {
                                        dOCUMENTO.DOCUMENTOP.ElementAt(j).MATNR = "";
                                    }
                                    docP.MATNR = dOCUMENTO.DOCUMENTOP.ElementAt(j).MATNR;
                                    docP.MATNR = new Cadena().completaMaterial(docP.MATNR);//RSG 07.06.2018
                                    docP.MATKL = dOCUMENTO.DOCUMENTOP.ElementAt(j).MATKL_ID;
                                    docP.CANTIDAD = 1;
                                    docP.MONTO = dOCUMENTO.DOCUMENTOP.ElementAt(j).MONTO;
                                    docP.PORC_APOYO = dOCUMENTO.DOCUMENTOP.ElementAt(j).PORC_APOYO;
                                    docP.MONTO_APOYO = dOCUMENTO.DOCUMENTOP.ElementAt(j).MONTO_APOYO;
                                    docP.PRECIO_SUG = dOCUMENTO.DOCUMENTOP.ElementAt(j).PRECIO_SUG;
                                    docP.VOLUMEN_EST = dOCUMENTO.DOCUMENTOP.ElementAt(j).VOLUMEN_EST;
                                    docP.VOLUMEN_REAL = dOCUMENTO.DOCUMENTOP.ElementAt(j).VOLUMEN_REAL;
                                    docP.VIGENCIA_DE = dOCUMENTO.DOCUMENTOP.ElementAt(j).VIGENCIA_DE;
                                    docP.VIGENCIA_AL = dOCUMENTO.DOCUMENTOP.ElementAt(j).VIGENCIA_AL;
                                    docP.APOYO_EST = dOCUMENTO.DOCUMENTOP.ElementAt(j).APOYO_EST;
                                    docP.APOYO_REAL = dOCUMENTO.DOCUMENTOP.ElementAt(j).APOYO_REAL;

                                    //dOCUMENTO.DOCUMENTOPs.Add(docP);
                                    ////db.SaveChanges();//RSG

                                    //If matnr es "" agregar los materiales de la categoría
                                    List<DOCUMENTOM> docml = new List<DOCUMENTOM>();
                                    if (docP.MATNR == "")
                                    {
                                        string col = "";
                                        if (Convert.ToDecimal(docP.APOYO_EST) > 0)
                                        {
                                            col = "E";
                                        }
                                        else if (Convert.ToDecimal(docP.APOYO_REAL) > 0)
                                        {
                                            col = "R";
                                        }
                                        docml = addCatItems(listcatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                            Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, select_neg, select_dis, totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);
                                    }

                                    //Obtener apoyo estimado
                                    decimal apoyo_esti = 0;
                                    decimal apoyo_real = 0;
                                    //Categoría por monto
                                    if (select_neg == "M")
                                    {
                                        //Obtener el apoyo real o estimado para cada material
                                        var cantmat = 1;
                                        try
                                        {
                                            apoyo_esti = Convert.ToDecimal(docP.APOYO_EST) / cantmat;

                                        }
                                        catch (Exception)
                                        {
                                            apoyo_esti = 0;
                                        }

                                        try
                                        {
                                            apoyo_real = Convert.ToDecimal(docP.APOYO_REAL) / cantmat;

                                        }
                                        catch (Exception)
                                        {
                                            apoyo_real = 0;
                                        }

                                        for (int k = 0; k < docml.Count; k++)
                                        {
                                            try
                                            {
                                                DOCUMENTOM docM = new DOCUMENTOM();
                                                docM = docml[k];
                                                docM.POS = k + 1;

                                                CategoriaMaterial cm = listcatm.Where(x => x.ID == docP.MATKL).FirstOrDefault();
                                                decimal porc = 0;
                                                if (cm != null)
                                                {
                                                    if (cm.TOTALCAT > 0) { porc = cm.MATERIALES.Where(x => x.MATNR == docM.MATNR).FirstOrDefault().VAL / cm.TOTALCAT * 100; }
                                                    docM.PORC_APOYO = porc;
                                                    //apoyo_real = apoyo_real * porc;
                                                    //apoyo_esti = apoyo_esti * porc;
                                                }
                                                else if (docP.MATKL == "000")
                                                {
                                                    decimal suma = 0;
                                                    foreach (CategoriaMaterial cam in listcatm)
                                                        suma += cam.TOTALCAT;
                                                    foreach (CategoriaMaterial cam in listcatm)
                                                        foreach (DOCUMENTOM_MOD dm in cam.MATERIALES)
                                                            if (dm.MATNR == docM.MATNR)
                                                                if (suma > 0) { porc = dm.VAL / suma * 100; }
                                                    docM.PORC_APOYO = porc;
                                                }

                                                docM.APOYO_REAL = apoyo_real * porc / 100;
                                                docM.APOYO_EST = apoyo_esti * porc / 100;

                                                docP.DOCUMENTOMs.Add(docM);
                                                ////db.SaveChanges();//RSG
                                            }
                                            catch (Exception e)
                                            {
                                                Log.ErrorLogApp(e, "Solicitudes", "Edit-DOCUMENTOM");
                                            }
                                        }
                                    }
                                    else if (select_neg == "P")
                                    {
                                        //Categoría por porcentaje
                                        for (int k = 0; k < docml.Count; k++)
                                        {
                                            try
                                            {
                                                DOCUMENTOM docM = new DOCUMENTOM();
                                                docM = docml[k];
                                                docM.POS = k + 1;

                                                docP.DOCUMENTOMs.Add(docM);
                                                ////db.SaveChanges();//RSG
                                            }
                                            catch (Exception e)
                                            {
                                                Log.ErrorLogApp(e, "Solicitudes", "Edit-DOCUMENTOM");
                                            }
                                        }

                                    }

                                    dOCUMENTO.DOCUMENTOPs.Add(docP);
                                }
                                catch (Exception e)
                                {
                                    Log.ErrorLogApp(e, "Solicitudes", "Edit-DOCUMENTOP");
                                }

                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Solicitudes", "Edit");
                    }

                    foreach (DOCUMENTOP dop in d.DOCUMENTOPs.ToList())
                    {
                        foreach (DOCUMENTOM dom in dop.DOCUMENTOMs.ToList())
                        {
                            dop.DOCUMENTOMs.Remove(dom);
                        }
                        d.DOCUMENTOPs.Remove(dop);
                    }
                    foreach (DOCUMENTOP dop in dOCUMENTO.DOCUMENTOPs)
                    {
                        DOCUMENTOP dp = d.DOCUMENTOPs.Where(a => a.POS == dop.POS).FirstOrDefault();
                        if (dp != null)
                        {
                            dp.APOYO_EST = dop.APOYO_EST;
                            dp.APOYO_REAL = dop.APOYO_REAL;
                            dp.CANTIDAD = dop.CANTIDAD;
                            dp.MATKL = dop.MATKL;
                            dp.MATNR = dop.MATNR;
                            dp.MONTO = dop.MONTO;
                            dp.MONTO_APOYO = dop.MONTO_APOYO;
                            dp.NUM_DOC = dop.NUM_DOC;
                            dp.PORC_APOYO = dop.PORC_APOYO;
                            dp.POS = dop.POS;
                            dp.PRECIO_SUG = dop.PRECIO_SUG;
                            dp.VIGENCIA_AL = dop.VIGENCIA_AL;
                            dp.VIGENCIA_DE = dop.VIGENCIA_DE;
                            dp.VOLUMEN_EST = dop.VOLUMEN_EST;
                            dp.VOLUMEN_REAL = dop.VOLUMEN_REAL;
                            foreach (DOCUMENTOM dom in dop.DOCUMENTOMs.ToList())
                            {
                                dp.DOCUMENTOMs.Add(dom);
                            }
                        }
                        else
                        {
                            d.DOCUMENTOPs.Add(dop);
                            foreach (DOCUMENTOM dom in dop.DOCUMENTOMs.ToList())
                            {
                                dop.DOCUMENTOMs.Add(dom);
                            }
                        }

                        //    foreach (DOCUMENTOM dom in dop.DOCUMENTOMs)
                        //{
                        //    DOCUMENTOM dm = d.DOCUMENTOPs.Where(a => a.POS == dom.POS_ID).FirstOrDefault().DOCUMENTOMs.Where(x => x.POS == dom.POS).FirstOrDefault();
                        //    if (dm != null)
                        //    {
                        //        dm.APOYO_EST = dom.APOYO_EST;
                        //        dm.APOYO_REAL = dom.APOYO_REAL;
                        //        dm.NUM_DOC = dom.NUM_DOC;
                        //        dm.PORC_APOYO = dom.PORC_APOYO;
                        //        dm.POS = dom.POS;
                        //        dm.POS_ID = dom.POS_ID;
                        //        dm.VIGENCIA_A = dom.VIGENCIA_A;
                        //        dm.VIGENCIA_DE = dom.VIGENCIA_DE;
                        //    }
                        //    else
                        //        dop.DOCUMENTOMs.Add(dom);
                        //}
                    }
                    if (dOCUMENTO.DOCUMENTOPs.Count < d.DOCUMENTOPs.Count)
                    {
                        for (int i = dOCUMENTO.DOCUMENTOPs.Count; i < d.DOCUMENTOPs.Count; i++)
                        {
                            //if (d.DOCUMENTOPs.ElementAt(i).DOCUMENTOMs.Count > 0)
                            //    for (int j = 0; j < d.DOCUMENTOPs.ElementAt(i).DOCUMENTOMs.Count; j++)
                            //    {
                            //        d.DOCUMENTOPs.ElementAt(i).DOCUMENTOMs.Remove(d.DOCUMENTOPs.ElementAt(i).DOCUMENTOMs.ElementAt(j));
                            //    }

                            d.DOCUMENTOPs.Remove(d.DOCUMENTOPs.ElementAt(i));
                        }
                    }
                    List<DOCUMENTOREC> ddd = new List<DOCUMENTOREC>();
                    ddd.AddRange(d.DOCUMENTORECs);
                    foreach (DOCUMENTOREC dree in ddd)//RSG 01.08.2018
                    {
                        foreach (DOCUMENTORAN dran in dree.DOCUMENTORANs.ToList())//RSG 01.08.2018
                        {
                            d.DOCUMENTORECs.FirstOrDefault(x => x.POS == dree.POS).DOCUMENTORANs.Remove(dran);
                        }
                        d.DOCUMENTORECs.Remove(dree);
                    }

                    if (chk_ligada == "on")
                        d.LIGADA = true;
                    else
                        d.LIGADA = false;

                    db.Entry(d).State = EntityState.Modified;
                    db.SaveChanges();


                    //Guardar registros de recurrencias  RSG 01.08.2018------------------

                    if (dOCUMENTO.DOCUMENTOREC != null)
                        if (dOCUMENTO.DOCUMENTOREC.Count > 0)
                        {
                            foreach (DOCUMENTOREC drec in dOCUMENTO.DOCUMENTOREC)
                            {
                                drec.NUM_DOC = dOCUMENTO.NUM_DOC;
                                if (drec.POS == 1)
                                {
                                    if (dOCUMENTO.TIPO_TECNICO != "P")
                                    {
                                    }
                                    else
                                    {//RSG 29.07.2018 - delete
                                        dOCUMENTO.TIPO_RECURRENTE = "P";
                                    }
                                }
                                if (drec.MONTO_BASE == null)
                                    drec.MONTO_BASE = 0;
                                if (drec.PORC == null)
                                    drec.PORC = 0;
                                dOCUMENTO.TIPO_RECURRENTE = db.TSOLs.Where(x => x.ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().TRECU;
                                if (dOCUMENTO.TIPO_RECURRENTE == "1" && d.LIGADA == true)
                                    dOCUMENTO.TIPO_RECURRENTE = "2";
                                if (dOCUMENTO.TIPO_RECURRENTE != "1" && d.OBJETIVOQ == true)
                                    dOCUMENTO.TIPO_RECURRENTE = "3";
                                //RSG 29.07.2018-add----------------------------------
                                drec.FECHAV = drec.FECHAF;
                                Calendario445 cal = new Calendario445();
                                if (dOCUMENTO.TIPO_RECURRENTE == "1")
                                    drec.FECHAF = cal.getNextViernes((DateTime)drec.FECHAF);
                                else
                                    drec.FECHAF = cal.getNextLunes((DateTime)drec.FECHAF);
                                drec.EJERCICIO = drec.FECHAV.Value.Year;
                                if (dOCUMENTO.TIPO_RECURRENTE == "1")
                                    drec.PERIODO = cal.getPeriodo(drec.FECHAV.Value);
                                else
                                    drec.PERIODO = cal.getPeriodoF(drec.FECHAV.Value);
                                if (dOCUMENTO.TIPO_RECURRENTE == "1")
                                    drec.PERIODO--;
                                if (drec.PERIODO == 0) drec.PERIODO = 12;
                                //RSG 29.07.2018-add----------------------------------
                                if (dOCUMENTO.DOCUMENTORAN != null)
                                    foreach (DOCUMENTORAN dran in dOCUMENTO.DOCUMENTORAN.Where(x => x.POS == drec.POS))
                                    {
                                        dran.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        drec.DOCUMENTORANs.Add(dran);
                                    }

                                d.DOCUMENTORECs.Add(drec);
                            }
                            db.Entry(d).State = EntityState.Modified;
                            db.SaveChanges();
                        }//Guardar registros de recurrencias  RSG 01.08.2018-------------------
                    if (dOCUMENTO.DOCUMENTOREC == null && d.LIGADA == true)
                    //if (dOCUMENTO.LIGADA == true)
                    {

                        DOCUMENTOREC drec = new DOCUMENTOREC();
                        drec.NUM_DOC = d.NUM_DOC;
                        drec.POS = 1;

                        if (drec.MONTO_BASE == null) //RSG 31.05.2018-------------------
                            drec.MONTO_BASE = 0;
                        if (drec.PORC == null) //RSG 31.05.2018-------------------
                            drec.PORC = 0;
                        d.TIPO_RECURRENTE = db.TSOLs.Where(x => x.ID.Equals(d.TSOL_ID)).FirstOrDefault().TRECU;
                        if (d.TIPO_RECURRENTE == "1" && d.LIGADA == true)
                            d.TIPO_RECURRENTE = "2";
                        if (d.TIPO_RECURRENTE != "1" && d.OBJETIVOQ == true)
                            d.TIPO_RECURRENTE = "3";
                        Calendario445 cal = new Calendario445();
                        drec.FECHAF = cal.getUltimoDia(d.FECHAF_VIG.Value.Year, cal.getPeriodo(d.FECHAF_VIG.Value));
                        drec.FECHAV = drec.FECHAF;

                        drec.FECHAF = cal.getNextLunes((DateTime)drec.FECHAF);
                        drec.EJERCICIO = drec.FECHAV.Value.Year;
                        drec.PERIODO = cal.getPeriodoF(drec.FECHAV.Value);

                        if (drec.PERIODO == 0) drec.PERIODO = 12;
                        if (d.DOCUMENTORAN != null)
                        {
                            foreach (DOCUMENTORAN dran in dOCUMENTO.DOCUMENTORAN.Where(x => x.POS == drec.POS))
                            {
                                dran.NUM_DOC = d.NUM_DOC;
                                drec.DOCUMENTORANs.Add(dran);
                            }
                        }
                        else
                        {
                            DOCUMENTORAN dran = new DOCUMENTORAN();
                            dran.NUM_DOC = d.NUM_DOC;
                            dran.POS = 1;
                            dran.LIN = 1;
                            dran.OBJETIVOI = 0;
                            dran.PORCENTAJE = d.PORC_APOYO;
                            drec.DOCUMENTORANs.Add(dran);
                        }
                        drec.PORC = d.PORC_APOYO;
                        drec.DOC_REF = 0;
                        drec.ESTATUS = "";

                        d.DOCUMENTORECs.Add(drec);

                        db.SaveChanges();
                    }


                    try //Guardar los documentosf cargados en la sección de soporte
                    {
                        //lej 26-07-2018 inicio--------------
                        try
                        {
                            //Si hay solo un registro se modifica
                            var _df = db.DOCUMENTOFs.Where(_d => _d.NUM_DOC == dOCUMENTO.NUM_DOC).ToList();
                            if (dOCUMENTO.DOCUMENTOF.Count > 0 && _df.Count == 0)
                            {
                                if (dOCUMENTO.DOCUMENTOF.Count == 1)
                                {
                                    List<DOCUMENTOF> facts = new List<DOCUMENTOF>();
                                    if (dOCUMENTO.DOCUMENTOF[0].FACTURAK == null)
                                        dOCUMENTO.DOCUMENTOF[0].FACTURAK = "";
                                    if (dOCUMENTO.DOCUMENTOF[0].FACTURA == null)
                                        dOCUMENTO.DOCUMENTOF[0].FACTURA = "";
                                    if (dOCUMENTO.DOCUMENTOF[0].PROVEEDOR == null)
                                        dOCUMENTO.DOCUMENTOF[0].PROVEEDOR = "";
                                    string[] fact = dOCUMENTO.DOCUMENTOF[0].FACTURAK.Split(';');
                                    for (int k = 0; k < fact.Length; k++)
                                    {
                                        DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[0];
                                        docF.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        docF.POS = k + 1;
                                        if (fact[k].Length > 4000)
                                            fact[k] = fact[k].Substring(0, 4000);
                                        docF.FACTURAK = fact[k];
                                        facts.Add(new DOCUMENTOF
                                        {
                                            NUM_DOC = dOCUMENTO.NUM_DOC,
                                            POS = k + 1,
                                            FACTURA = docF.FACTURA,
                                            FECHA = docF.FECHA,
                                            PROVEEDOR = docF.PROVEEDOR,
                                            CONTROL = docF.CONTROL,
                                            AUTORIZACION = docF.AUTORIZACION,
                                            VENCIMIENTO = docF.VENCIMIENTO,
                                            EJERCICIOK = docF.EJERCICIOK,
                                            BILL_DOC = docF.BILL_DOC,
                                            BELNR = docF.BELNR,
                                            IMPORTE_FAC = docF.IMPORTE_FAC,
                                            PAYER = docF.PAYER,
                                            FACTURAK = fact[k]
                                        });
                                    }
                                    db.BulkInsert(facts);
                                }
                                else
                                {
                                    for (int j = 0; j < dOCUMENTO.DOCUMENTOF.Count; j++)
                                    {
                                        try
                                        {
                                            DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[j];
                                            docF.NUM_DOC = dOCUMENTO.NUM_DOC;
                                            db.DOCUMENTOFs.Add(docF);
                                            db.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.ErrorLogApp(e, "Solicitudes", "Edit");
                                        }
                                    }
                                }
                            }



                            if (dOCUMENTO.DOCUMENTOF.Count == 1)
                            {
                                if (_df.Count == 1)
                                {
                                    if (dOCUMENTO.DOCUMENTOF[0].FACTURAK == null)
                                        dOCUMENTO.DOCUMENTOF[0].FACTURAK = "";
                                    string[] _f = dOCUMENTO.DOCUMENTOF[0].FACTURAK.Split(';');
                                    for (int k = 0; k < _f.Length; k++)
                                    {
                                        DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[0];
                                        _df[k].NUM_DOC = dOCUMENTO.NUM_DOC;
                                        _df[k].FACTURA = docF.FACTURA;
                                        _df[k].PROVEEDOR = docF.PROVEEDOR;
                                        _df[k].CONTROL = docF.CONTROL;
                                        _df[k].AUTORIZACION = docF.AUTORIZACION;
                                        _df[k].FACTURAK = docF.FACTURAK;
                                        _df[k].EJERCICIOK = docF.EJERCICIOK;
                                        _df[k].BILL_DOC = docF.BILL_DOC;
                                        _df[k].BELNR = docF.BELNR;
                                        _df[k].IMPORTE_FAC = docF.IMPORTE_FAC;
                                        _df[k].PAYER = docF.PAYER;
                                        _df[k].FECHA = docF.FECHA;
                                        //paso a una nueva variable
                                        var _dataFac = _df[k];
                                        db.Entry(_dataFac).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                if (_df.Count > 1)//si hay mas de 1 documentof
                                {
                                    for (var i = 0; i < _df.Count; i++)
                                    {
                                        //Si encuentra solo un registro, lo borro
                                        db.Entry(_df[i]).State = EntityState.Deleted;
                                        db.SaveChanges();
                                    }
                                    string[] _f = dOCUMENTO.DOCUMENTOF[0].FACTURAK.Split(';');
                                    for (int k = 0; k < _f.Length; k++)
                                    {
                                        DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[0];
                                        var _df2 = new DOCUMENTOF();
                                        _df2.POS = docF.POS;
                                        _df2.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        _df2.FACTURA = docF.FACTURA;
                                        _df2.PROVEEDOR = docF.PROVEEDOR;
                                        _df2.CONTROL = docF.CONTROL;
                                        _df2.AUTORIZACION = docF.AUTORIZACION;
                                        _df2.FACTURAK = docF.FACTURAK;
                                        _df2.EJERCICIOK = docF.EJERCICIOK;
                                        _df2.BILL_DOC = docF.BILL_DOC;
                                        _df2.BELNR = docF.BELNR;
                                        _df2.IMPORTE_FAC = docF.IMPORTE_FAC;
                                        _df2.PAYER = docF.PAYER;
                                        _df2.FECHA = docF.FECHA;
                                        db.DOCUMENTOFs.Add(_df2);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else if (dOCUMENTO.DOCUMENTOF.Count > 1)
                            {
                                if (_df.Count == 1)
                                {
                                    //Si encuentra solo un registro, lo borro
                                    db.Entry(_df[0]).State = EntityState.Deleted;
                                    db.SaveChanges();
                                    for (var i = 0; i < dOCUMENTO.DOCUMENTOF.Count; i++)
                                    {
                                        var _df2 = new DOCUMENTOF();
                                        DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[i];
                                        _df2.POS = docF.POS;
                                        _df2.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        _df2.FACTURA = docF.FACTURA;
                                        _df2.PROVEEDOR = docF.PROVEEDOR;
                                        _df2.CONTROL = docF.CONTROL;
                                        _df2.AUTORIZACION = docF.AUTORIZACION;
                                        _df2.FACTURAK = docF.FACTURAK;
                                        _df2.EJERCICIOK = docF.EJERCICIOK;
                                        _df2.BILL_DOC = docF.BILL_DOC;
                                        _df2.BELNR = docF.BELNR;
                                        _df2.IMPORTE_FAC = docF.IMPORTE_FAC;
                                        _df2.PAYER = docF.PAYER;
                                        _df2.FECHA = docF.FECHA;
                                        //paso a una nueva variable
                                        db.DOCUMENTOFs.Add(_df2);
                                        db.SaveChanges();
                                    }
                                }
                                if (_df.Count > 1)//si hay mas de 1 documentof
                                {
                                    for (var i = 0; i < _df.Count; i++)
                                    {
                                        //Si encuentra solo un registro, lo borro
                                        db.Entry(_df[i]).State = EntityState.Deleted;
                                        db.SaveChanges();
                                    }
                                    for (var i = 0; i < dOCUMENTO.DOCUMENTOF.Count; i++)
                                    {
                                        var _df2 = new DOCUMENTOF();
                                        DOCUMENTOF docF = dOCUMENTO.DOCUMENTOF[i];
                                        _df2.POS = docF.POS;
                                        _df2.NUM_DOC = dOCUMENTO.NUM_DOC;
                                        _df2.FACTURA = docF.FACTURA.Trim();
                                        _df2.PROVEEDOR = docF.PROVEEDOR.Trim();
                                        _df2.CONTROL = docF.CONTROL.Trim();
                                        _df2.AUTORIZACION = docF.AUTORIZACION.Trim();
                                        _df2.FACTURAK = docF.FACTURAK.Trim();
                                        _df2.EJERCICIOK = docF.EJERCICIOK.Trim();
                                        _df2.BILL_DOC = docF.BILL_DOC.Trim();
                                        _df2.BELNR = docF.BELNR.Trim();
                                        _df2.IMPORTE_FAC = docF.IMPORTE_FAC;
                                        _df2.PAYER = docF.PAYER.Trim();
                                        _df2.FECHA = docF.FECHA;
                                        db.DOCUMENTOFs.Add(_df2);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.ErrorLogApp(e, "Solicitudes", "Edit");
                        }
                        //lej 26-07-2018 fin-----------------
                    }
                    catch (Exception e)
                    {
                        Log.ErrorLogApp(e, "Solicitudes", "Edit");
                    }



                    string[] borrarSop = txt_sop_borr.Split(',');
                    foreach (string borra in borrarSop)
                    {
                        if (borra != "")
                        {
                            List<DOCUMENTOA> ddab = db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) && a.CLASE.Equals(borra) && a.ACTIVO == true).ToList();
                            if (ddab.Count > 0)
                            {
                                foreach (DOCUMENTOA daa in ddab)
                                {
                                    daa.ACTIVO = false;
                                    db.Entry(daa).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }


                }//ADD RSG 20.08.2018

                //Checar si hay archivos para subir
                int numFiles = 0;
                var res = "";
                string errorMessage = "";
                try
                {
                    foreach (HttpPostedFileBase file in files_soporte)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            numFiles++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "Solicitudes", "Edit");
                }

                //Guardar los documentos cargados en la sección de soporte
                if (numFiles > 0)
                {
                    //Obtener las variables con los datos de sesión y ruta
                    string url = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") && x.ACTIVO).FirstOrDefault().VALUE;
                    //Crear el directorio

                    //var dir = createDir(url, dOCUMENTO.NUM_DOC.ToString());
                    var dir = new Files().createDir(url, dOCUMENTO.NUM_DOC.ToString(), dOCUMENTO.EJERCICIO.ToString());//RSG 01.08.2018


                    //Evaluar que se creo el directorio
                    if (string.IsNullOrEmpty(dir))
                    {

                        int i = 0;
                        int indexlabel = 0;
                        int max = 0;
                        if (db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).Count() > 0)
                            max = db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).Max(a => a.POS);//RSG 15.05.2018

                        foreach (HttpPostedFileBase file in files_soporte)
                        {
                            string errorfiles = "";
                            var clasefile = "";
                            try
                            {
                                clasefile = labels_soporte[indexlabel];
                            }
                            catch (Exception)
                            {
                                clasefile = "";
                            }
                            if (file != null && file.ContentLength > 0)
                            {
                                string path = "";
                                string filename = file.FileName;
                                errorfiles = "";
                                //res = SaveFile(file, url, d.NUM_DOC.ToString(), out errorfiles, out path);//RSG 01.08.2018
                                res = new Files().SaveFile(file, url, d.NUM_DOC.ToString(), out errorfiles, out path, d.EJERCICIO.ToString());//RSG 01.08.2018

                                if (errorfiles == "")
                                {
                                    DOCUMENTOA doc = new DOCUMENTOA();
                                    var ext = System.IO.Path.GetExtension(filename);
                                    i++;
                                    doc.NUM_DOC = d.NUM_DOC;
                                    doc.POS = i;
                                    doc.TIPO = ext.Replace(".", "");
                                    try
                                    {
                                        var clasefileM = clasefile.ToUpper();
                                        doc.CLASE = clasefileM.Substring(0, 3);
                                    }
                                    catch (Exception e)
                                    {
                                        doc.CLASE = "";
                                    }
                                    if (max > 0)
                                    {
                                        doc.POS = max + i;
                                    }
                                    if (max > 0)
                                    {
                                        List<DOCUMENTOA> dda = db.DOCUMENTOAs.Where(a => a.NUM_DOC.Equals(doc.NUM_DOC) && a.CLASE.Equals(doc.CLASE) && a.ACTIVO == true).ToList();
                                        if (dda.Count > 0)
                                        {
                                            foreach (DOCUMENTOA daa in dda)
                                            {
                                                daa.ACTIVO = false;
                                                db.Entry(daa).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    doc.STEP_WF = 1;
                                    doc.USUARIO_ID = User.Identity.Name;
                                    doc.PATH = path;
                                    doc.ACTIVO = true;
                                    try
                                    {
                                        db.DOCUMENTOAs.Add(doc);
                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        errorfiles = "" + filename;
                                    }
                                }

                            }
                            indexlabel++;
                            if (errorfiles != "")
                            {
                                errorMessage += "Error con el archivo " + errorfiles;
                            }



                        }
                    }
                    else
                    {
                        errorMessage = dir;
                    }
                    errorString = errorMessage;
                    //Guardar número de documento creado
                    Session["ERROR_FILES"] = errorMessage;
                }

                string rec = "";//ADD RSG 30.10.2018
                if (txt_flujo == "B")//ADD RSG 30.10.2018
                    rec = "B";//ADD RSG 30.10.2018
                if (rec != "B")//ADD RSG 31.10.2018
                {
                    ProcesaFlujo pf = new ProcesaFlujo();
                    try
                    {
                        FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC == d.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();
                        f.ESTATUS = "A";
                        f.FECHAM = DateTime.Now;
                        string c = pf.procesa(f, rec);
                        FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                        while (c == "1")
                        {
                            Email em = new Email();
                            string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                            string image = Server.MapPath("~/images/logo_kellogg.png");
                            DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == d.NUM_DOC).First();
                            string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                            em.enviaMailC(f.NUM_DOC, true, usuariotextos, UrlDirectory, "Index", image, imageFlag);


                            if (conta.WORKFP.ACCION.TIPO == "B")
                            {
                                WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID && x.VERSION == conta.WF_VERSION && x.POS == conta.WF_POS).FirstOrDefault();
                                conta.ESTATUS = "A";
                                //f1.FECHAC = DateTime.Now;
                                conta.FECHAM = DateTime.Now;
                                c = pf.procesa(conta, "");
                                conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                            }
                            else
                            {
                                c = "";
                            }
                        }

                        using (TruequeEntities db1 = new TruequeEntities())
                        {
                            FLUJO ff = db1.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                            Estatus es = new Estatus();//RSG 18.09.2018
                            DOCUMENTO ddoc = db1.DOCUMENTOes.Find(f.NUM_DOC);
                            ff.STATUS = es.getEstatus(ddoc);
                            db1.Entry(ff).State = EntityState.Modified;
                            db1.SaveChanges();
                        }
                    }
                    catch (Exception ee)
                    {
                        if (errorString == "")
                        {
                            errorString = ee.Message.ToString();
                        }
                        ViewBag.error = errorString;
                    }
                }//ADD RSG 31.10.2018

                if (d.DOCUMENTO_REF > 0 && txt_flujo != "B")//ADD RSG 02.11.2018
                {
                    if (!d.TSOL.REVERSO)
                    {
                        using (TruequeEntities db1 = new TruequeEntities())
                        {
                            DOCUMENTO docPadre = db1.DOCUMENTOes.Find(d.DOCUMENTO_REF);
                            List<DOCUMENTO> dd = db1.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == (d.DOCUMENTO_REF)).ToList();
                            List<DOCUMENTOP> ddr = db1.DOCUMENTOPs.Where(a => a.NUM_DOC == (d.DOCUMENTO_REF)).ToList();
                            ////decimal total = 0;
                            decimal[] totales = new decimal[ddr.Count()];
                            decimal totalRes = new decimal();
                            //foreach (DOCUMENTOP dr in ddr)
                            //{
                            //    //totales[(int)dr.POS - 1] = dr.VOLUMEN_EST * dr.MONTO_APOYO;
                            //    totales[(int)dr.POS - 1] = (decimal)dr.APOYO_EST;
                            foreach (DOCUMENTO d1 in dd)
                            {
                                //foreach (DOCUMENTOP dp in d1.DOCUMENTOPs)
                                //{
                                //    if (dr.POS == dp.POS)
                                //    {
                                //        //var suma2 = dp.VOLUMEN_REAL * dp.MONTO_APOYO;
                                //        var suma2 = dp.APOYO_REAL;

                                //        totales[(int)dr.POS - 1] = totales[(int)dr.POS - 1] - (decimal)suma2;
                                //    }
                                //}
                                totalRes += (decimal)d1.MONTO_DOC_MD;
                                //}
                            }
                            //RSG 14.06.2018----------------------
                            decimal resto = decimal.Parse("0.00");
                            //foreach (decimal dec in totales)
                            //{
                            //    resto += dec;
                            //}
                            resto = (decimal)docPadre.MONTO_DOC_MD - totalRes;
                            ////RSG 14.06.2018----------------------
                            //foreach (decimal dec in totales)
                            //{
                            //    if (dec > 0)
                            //        return RedirectToAction("Reversa", new { id = dOCUMENTO.DOCUMENTO_REF, resto = resto });
                            //}
                            if (docPadre.MONTO_DOC_MD - totalRes > 0)
                                return RedirectToAction("Reversa", new { id = dOCUMENTO.DOCUMENTO_REF, resto = resto });
                        }

                    }
                    using (TruequeEntities db1 = new TruequeEntities())
                    {
                        decimal num_ref = (decimal)d.DOCUMENTO_REF;
                        DOCUMENTO referencia = db1.DOCUMENTOes.Find(num_ref);
                        referencia.ESTATUS = "R";
                        db1.Entry(referencia).State = EntityState.Modified;
                        db1.SaveChanges();
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            ViewBag.TALL_ID = new SelectList(db.TALLs, "ID", "DESCRIPCION", dOCUMENTO.TALL_ID);
            ViewBag.TSOL_ID = new SelectList(db.TSOLs, "ID", "DESCRIPCION", dOCUMENTO.TSOL_ID);
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS", dOCUMENTO.USUARIOC_ID);
            ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1", dOCUMENTO.VKORG);
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", dOCUMENTO.PAIS_ID);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", dOCUMENTO.SOCIEDAD_ID);

            //LEJ 24.07.2018------------------------------------------------------------
            DOCUMENTO dOCUMENTO_ = db.DOCUMENTOes.Find(dOCUMENTO.NUM_DOC);
            if (dOCUMENTO_ == null)
            {
                return HttpNotFound();
            }
            dOCUMENTO_.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                    && a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                    && a.SPART.Equals(dOCUMENTO.SPART)
                                                    && a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
            dOCUMENTO_.DOCUMENTOF = db.DOCUMENTOFs.Where(a => a.NUM_DOC.Equals(dOCUMENTO.NUM_DOC)).ToList();
            DocumentoFlujo DF = new DocumentoFlujo();
            DF.D = dOCUMENTO_;
            ViewBag.df = DF;
            //LEJ 24.07.2018------------------------------------------------------------
            ViewBag.horaServer = DateTime.Now.Date.ToString().Split(new[] { ' ' }, 2)[1];//RSG 01.08.2018

            var aa = (from n in db.TSOLTs.Where(x => x.SPRAS_ID == usuariotextos)
                      join t in db.TSOL_TREE
                      on n.TSOL_ID equals t.TSOL_ID
                      where n.TSOL.FACTURA && !n.TSOL_ID.StartsWith("O")
                      select new { n.TSOL_ID, n.TXT020 }).DistinctBy(x => x.TSOL_ID).DistinctBy(x => x.TXT020).ToList();
            ViewBag.TSOL_LIG = new SelectList(aa, "TSOL_ID", "TXT020", dOCUMENTO.TSOL_LIG);

            return View(dOCUMENTO);
        }

        // GET: Solicitudes/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            return View(dOCUMENTO);
        }

        // POST: Solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            db.DOCUMENTOes.Remove(dOCUMENTO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult CreateNew()
        {

            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.nombre = user.NOMBRE + " " + user.APELLIDO_P + " " + user.APELLIDO_M;
                ViewBag.email = user.EMAIL;
                ViewBag.rol = user.MIEMBROS.FirstOrDefault().ROL.NOMBRE;
                ViewBag.returnUrl = Request.UrlReferrer;
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    return RedirectToAction("Pais", "Home");
                }

            }

            return View();
        }

        public ActionResult Sol_Tipo()
        {

            var btnRadioe = Request.Form["radio_tiposol"];

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".png";
            }
            catch
            {
                return RedirectToAction("Pais", "Home");
            }

            if (btnRadioe != "" || btnRadioe != null)
            {
                Session["sol_tipo"] = null;
                Session["sol_tipo"] = btnRadioe;

                //return RedirectToAction("Informacion", "Solicitud", new { tipo = btnRadioe });
                return RedirectToAction("Create", "Solicitudes");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Cancelar(decimal id)
        {
            Cancelar can = new Cancelar();
            Email em = new Email();
            can.cancela(id, User.Identity.Name);

            string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
            string image = Server.MapPath("~/images/logo_kellogg.png");
            DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).First();
            string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
            em.enviaMailC(id, true, Session["spras"].ToString(), UrlDirectory, "Cancelacion", image, imageFlag, 1);

            return RedirectToAction("Index", "Home");
        }
        public ActionResult GuardarComentario(decimal num_docu, string comentario)
        {
            DOCUMENTO d = db.DOCUMENTOes.Find(num_docu);
            if (d.DOCUMENTO_REF == null)
            {
                FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC == d.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();
                //db.Entry(d).State = EntityState.Modified;

                if (actual != null)
                {
                    FLUJO nuevo = new FLUJO();
                    nuevo.COMENTARIO = comentario;
                    nuevo.DETPOS = actual.DETPOS;
                    nuevo.DETVER = actual.DETVER;
                    nuevo.ESTATUS = "";
                    nuevo.FECHAC = DateTime.Now;
                    nuevo.FECHAM = DateTime.Now;
                    nuevo.LOOP = 0;
                    nuevo.NUM_DOC = actual.NUM_DOC;
                    nuevo.POS = actual.POS + 1;
                    nuevo.USUARIOA_ID = User.Identity.Name;
                    nuevo.WF_POS = actual.WF_POS;
                    nuevo.WF_VERSION = actual.WF_VERSION;
                    nuevo.WORKF_ID = actual.WORKF_ID;
                    db.FLUJOes.Add(nuevo);
                    ////actual.COMENTARIO = comentario;
                    ////db.Entry(actual).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return null;
            //return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SelectCity(int id)
        {

            TruequeEntities db = new TruequeEntities();

            var id_cl = db.CITIES.Where(city => city.STATE_ID.Equals(id)).Select(c => new { ID = c.ID.ToString(), NAME = c.NAME.ToString() }).ToList();

            JsonResult jc = Json(id_cl, JsonRequestBehavior.AllowGet);
            return jc;
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult SelectTall(string id)
        {

            TruequeEntities db = new TruequeEntities();

            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

            var id_clas = db.TALLs.Where(t => t.ACTIVO == true && t.GALL_ID.Equals(id))
                                .Join(
                                db.TALLTs.Where(tallt => tallt.SPRAS_ID == user.SPRAS_ID),
                                tall => tall.ID,
                                tallt => tallt.TALL_ID,
                                (tall, tallt) => new
                                {
                                    ID = tallt.TALL_ID.ToString(),
                                    TEXT = tallt.TXT50.ToString()
                                })
                            .ToList();

            JsonResult jc = Json(id_clas, JsonRequestBehavior.AllowGet);
            return jc;
        }



        [HttpPost]
        [AllowAnonymous]
        public string SelectTcambio(string fcurr)
        {
            string p = "";
            string errorString = "";
            decimal tcambio = 0;
            string tcurr = "USD";
            SOCIEDAD id_bukrs = new SOCIEDAD();
            try
            {
                p = Session["pais"].ToString();
            }
            catch
            {
            }

            TruequeEntities db = new TruequeEntities();
            try
            {
                id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p)).FirstOrDefault();
                var date = DateTime.Now.Date;
                //var tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(date)).FirstOrDefault().UKURS;
                var tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(date)).FirstOrDefault();
                if (tc == null)
                {
                    var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr)).Max(a => a.GDATU);
                    tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(max)).FirstOrDefault();
                }

                tcambio = Convert.ToDecimal(tc.UKURS);

            }
            catch (Exception e)
            {
                errorString = e.Message + "detail: conversion " + fcurr + " to " + tcurr + " in date " + DateTime.Now.Date;
                //throw new System.Exception(errorString);
                return errorString;
            }

            return Convert.ToString(tcambio);
        }

        [HttpPost]
        [AllowAnonymous]
        public string SelectVcambio(string moneda_id, decimal monto_doc_md)
        {
            string p = "";
            string tcurr = "USD";
            string errorString = "";
            decimal monto = 0;
            try
            {
                p = Session["pais"].ToString();
            }
            catch
            {
            }

            TruequeEntities db = new TruequeEntities();

            var id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p)).FirstOrDefault();
            try
            {
                var date = DateTime.Now.Date;
                //var UKURS = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(date)).FirstOrDefault().UKURS;
                var tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(date)).FirstOrDefault();
                if (tc == null)
                {
                    var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(tcurr)).Max(a => a.GDATU);
                    tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(max)).FirstOrDefault();
                }

                decimal uk = Convert.ToDecimal(tc.UKURS);

                if (tc.UKURS > 0)
                {
                    monto = Convert.ToDecimal(monto_doc_md) / uk;
                }
            }
            catch (Exception e)
            {
                errorString = e.Message + "detail: conversion " + moneda_id + " to " + tcurr + " in date " + DateTime.Now.Date;
                //throw new System.Exception(errorString);
                return errorString;
            }
            return Convert.ToString(monto);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoadExcel(string vkorg, string vtweg, string spras)
        {
            List<DOCUMENTOP_MOD> ld = new List<DOCUMENTOP_MOD>();

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                //using (var stream2 = System.IO.File.Open(url, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                //{
                string extension = System.IO.Path.GetExtension(file.FileName);
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                //using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream))
                //{
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                // 2. Use the AsDataSet extension method
                DataSet result = reader.AsDataSet();

                // The result of each spreadsheet is in result.Tables
                // 3.DataSet - Create column names from first row
                DataTable dt = result.Tables[0];

                //Rows
                var rowsc = dt.Rows.Count;
                //columns
                var columnsc = dt.Columns.Count;

                //Columnd and row to start
                var rows = 1; // 2
                              //var cols = 0; // A
                var pos = 1;

                for (int i = rows; i < rowsc; i++)
                {
                    //for (var j = 0; j < columnsc; j++)
                    //{
                    //    var data = dt.Rows[i][j];
                    //}
                    if (i >= 4)
                    {
                        var v = dt.Rows[i][1];
                        if (Convert.ToString(v) == "")
                        {
                            break;
                        }
                    }
                    DOCUMENTOP_MOD doc = new DOCUMENTOP_MOD();

                    //Rows variables
                    double monto = 0;
                    double porc_apoyo = 0;
                    //double monto_apoyo = 0;
                    //double montoc_apoyo = 0;
                    double precio_sug = 0;
                    double volumen_est = 0;
                    //double porc_apoyoest = 0;


                    //var poss = dt.Rows[i][1];
                    string a = Convert.ToString(pos);


                    doc.POS = Convert.ToDecimal(a);
                    try
                    {
                        doc.VIGENCIA_DE = Convert.ToDateTime(dt.Rows[i][0]); //DEL
                    }
                    catch (Exception e)
                    {
                        doc.VIGENCIA_DE = null;
                    }
                    try
                    {
                        doc.VIGENCIA_AL = Convert.ToDateTime(dt.Rows[i][1]); //AL
                    }
                    catch (Exception e)
                    {
                        doc.VIGENCIA_AL = null;
                    }
                    try
                    {
                        doc.MATNR = dt.Rows[i][2].ToString(); //Material
                        MATERIAL mat = material(doc.MATNR);
                        if (mat != null)
                        {
                            List<MATERIAL> materiales = materialesDao.ListaMateriales(doc.MATNR, vkorg, vtweg, User.Identity.Name);
                            mat = materiales.Where(x => x.ID == mat.ID).FirstOrDefault();
                        }
                        if (mat != null && ld.Where(x => x.MATNR.Equals(doc.MATNR)).Count() == 0)//Validar si el material existe
                        {
                            //doc.MATKL = (string)dt.Rows[i][4]; //Categoría se toma de la bd
                            ////CATEGORIAT cat = getCategoriaS(material: doc.MATNR); //Categoría
                            MATERIALGPT cat = getCategoriaS(material: doc.MATNR); //Categoría//RSG 01.08.2018
                            try
                            {
                                doc.MATKL = cat.TXT50.ToString();
                                //doc.MATKL_ID = cat.CATEGORIA_ID.ToString();
                                doc.MATKL_ID = cat.MATERIALGP_ID.ToString();//RSG 01.08.2018
                            }
                            catch (Exception e)
                            {
                                doc.MATKL = "";
                                doc.MATKL_ID = "";
                            }
                            //doc.DESC = (string)dt.Rows[i][5]; //Descripción se toma de la bd
                            doc.DESC = mat.MAKTX.ToString(); //Descripción
                            doc.ACTIVO = true;
                        }
                        else
                        {
                            doc.ACTIVO = false;
                        }
                    }
                    catch (Exception e)
                    {
                        doc.ACTIVO = false;
                    }
                    try
                    {
                        monto = (double)dt.Rows[i][3]; //Costo unitario    
                    }
                    catch (Exception e)
                    {
                        monto = 0;
                    }
                    doc.MONTO = Convert.ToDecimal(monto);
                    //doc.MONTO = Math.Round(doc.MONTO, 2);
                    try
                    {
                        porc_apoyo = (double)dt.Rows[i][4]; //% apoyo
                        porc_apoyo = porc_apoyo * 100;
                    }
                    catch (Exception e)
                    {
                        porc_apoyo = 0;
                    }
                    doc.PORC_APOYO = Convert.ToDecimal(porc_apoyo);
                    //doc.PORC_APOYO = Math.Round(doc.PORC_APOYO, 2);
                    //try
                    //{
                    //    monto_apoyo = (double)dt.Rows[i][8]; //Apoyo por pieza
                    //}
                    //catch (Exception e)
                    //{
                    //    monto_apoyo = 0;
                    //}
                    //    doc.MONTO_APOYO = Convert.ToDecimal(monto_apoyo);
                    //try
                    //{
                    //    montoc_apoyo = (double)dt.Rows[i][9]; //Costo con apoyo
                    //}
                    //catch (Exception e)
                    //{
                    //    montoc_apoyo = 0;
                    //}
                    //    doc.MONTOC_APOYO = Convert.ToDecimal(montoc_apoyo);
                    try
                    {
                        precio_sug = (double)dt.Rows[i][5]; //Precio sugerido
                    }
                    catch (Exception e)
                    {
                        precio_sug = 0;
                    }
                    doc.PRECIO_SUG = Convert.ToDecimal(precio_sug);
                    //doc.PRECIO_SUG = Math.Round(doc.PRECIO_SUG, 2);
                    try
                    {
                        volumen_est = (double)dt.Rows[i][6]; //Volumen estimado
                    }
                    catch (Exception e)
                    {
                        volumen_est = 0;
                    }
                    doc.VOLUMEN_EST = Convert.ToDecimal(volumen_est);
                    //doc.VOLUMEN_EST = Math.Round(doc.VOLUMEN_EST, 2);
                    //try
                    //{
                    //    //porc_apoyoest = (double)dt.Rows[i][12]; //Estimado $ apoyo
                    //}catch(Exception e)
                    //{
                    //    porc_apoyoest = 0;
                    //}
                    //doc.PORC_APOYOEST = Convert.ToDecimal(porc_apoyoest);

                    //RSG 24.05.2018--------------------------------- 
                    try
                    {
                        string apoyo = dt.Rows[i][7].ToString();
                        doc.APOYO_EST = (decimal.Parse(apoyo));//Apoyo
                    }
                    catch
                    {
                        doc.APOYO_EST = 0;
                    }
                    //RSG 24.05.2018----------------------------------
                    if (doc.PORC_APOYO == 0 || doc.MONTO == 0)
                    {
                        try
                        {
                            string apoyoTotal = dt.Rows[i][8].ToString();
                            doc.APOYO_REAL = (decimal.Parse(apoyoTotal));//Apoyo Total
                        }
                        catch
                        {
                            doc.APOYO_REAL = 0;
                        }
                    }
                    ld.Add(doc);
                    pos++;
                }

                reader.Close();

            }
            JsonResult jl = Json(ld, JsonRequestBehavior.AllowGet);
            return jl;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoadExcelSop()
        {
            List<DOCUMENTOF_MOD> ld = new List<DOCUMENTOF_MOD>();

            Cadena cad = new Cadena();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                //using (var stream2 = System.IO.File.Open(url, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                //{
                string extension = System.IO.Path.GetExtension(file.FileName);
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                //using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream))
                //{
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                // 2. Use the AsDataSet extension method
                DataSet result = reader.AsDataSet();

                // The result of each spreadsheet is in result.Tables
                // 3.DataSet - Create column names from first row
                DataTable dt = result.Tables[0];

                //Rows
                var rowsc = dt.Rows.Count;
                //columns
                var columnsc = dt.Columns.Count;

                //Columnd and row to start
                var rows = 1; // 2
                              //var cols = 0; // A
                int pos = 1;

                for (int i = rows; i < rowsc; i++)
                {

                    DOCUMENTOF_MOD doc = new DOCUMENTOF_MOD();

                    doc.POS = pos;
                    try
                    {
                        doc.FACTURA = dt.Rows[i][1].ToString(); //Factura
                    }
                    catch (Exception e)
                    {
                        doc.FACTURA = null;
                    }
                    //jemo 06-08-2018 inicio
                    try
                    {
                        doc.SOCIEDAD = dt.Rows[i][0].ToString(); //Sociedad
                    }
                    catch (Exception e)
                    {
                        doc.SOCIEDAD = null;
                    }
                    //jemo 06-08-2018 fin
                    //jemo 10-17-2018 inicio
                    //try
                    //{
                    //    doc.FECHA = Convert.ToDateTime(dt.Rows[i][1]); //Fecha
                    //}
                    //catch (Exception e)
                    //{
                    //    doc.FECHA = null;
                    //}
                    //try
                    //{
                    //    var provs = dt.Rows[i][2];
                    //    doc.PROVEEDOR = provs + ""; //Proveedor
                    //    PROVEEDOR prov = proveedor(doc.PROVEEDOR);
                    //    if (prov != null)//Validar si el proveedor existe
                    //    {

                    //        doc.PROVEEDOR_TXT = prov.NOMBRE.ToString(); //Descripción
                    //        doc.PROVEEDOR_ACTIVO = true;
                    //    }
                    //    else
                    //    {
                    //        doc.PROVEEDOR_ACTIVO = false;
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    doc.PROVEEDOR_ACTIVO = false;
                    //}
                    //try
                    //{
                    //    doc.CONTROL = (string)dt.Rows[i][3]; //Control   
                    //}
                    //catch (Exception e)
                    //{
                    //    doc.CONTROL = "";
                    //}
                    //try
                    //{
                    //    doc.AUTORIZACION = (string)dt.Rows[i][4]; //Autorización                        
                    //}
                    //catch (Exception e)
                    //{
                    //    doc.AUTORIZACION = "";
                    //}
                    //try
                    //{
                    //    doc.VENCIMIENTO = Convert.ToDateTime(dt.Rows[i][5]); //Vencimiento
                    //}
                    //catch (Exception e)
                    //{
                    //    doc.VENCIMIENTO = null;
                    //}
                    //jemo 10-17-2018 fin
                    try
                    {
                        doc.BILL_DOC = dt.Rows[i][2].ToString(); //Billing
                    }
                    catch (Exception e)
                    {
                        doc.BILL_DOC = null;
                    }
                    try
                    {
                        int ej = Convert.ToInt32(dt.Rows[i][3]);
                        doc.EJERCICIOK = Convert.ToString(ej); //Ejerciciok
                    }
                    catch (Exception e)
                    {
                        doc.EJERCICIOK = null;
                    }
                    try
                    {
                        doc.PAYER = dt.Rows[i][4].ToString(); //Payer
                    }
                    catch (Exception e)
                    {
                        doc.PAYER = null;
                    }
                    try
                    {
                        doc.IMPORTE_FACT = dt.Rows[i][6].ToString(); //Importe factura
                    }
                    catch (Exception e)
                    {
                        doc.IMPORTE_FACT = null;
                    }
                    try
                    {
                        doc.BELNR = dt.Rows[i][7].ToString(); //Folio
                    }
                    catch (Exception e)
                    {
                        doc.BELNR = null;
                    }
                    try
                    {
                        var provs = dt.Rows[i][8];
                        doc.PROVEEDOR = provs + ""; //Proveedor
                        PROVEEDOR prov = proveedor(cad.completaCliente(doc.PROVEEDOR));
                        if (prov != null)//Validar si el proveedor existe
                        {

                            doc.PROVEEDOR_TXT = prov.NOMBRE.ToString(); //Descripción
                            doc.PROVEEDOR_ACTIVO = true;
                        }
                        else
                        {
                            doc.PROVEEDOR_ACTIVO = false;
                        }
                    }
                    catch
                    {
                        doc.PROVEEDOR_ACTIVO = false;
                    }

                    doc.DESCRIPCION = "";
                    ld.Add(doc);
                    pos++;
                }
                //jemo 10-17-2018 inicio
                ////Cadena cad = new Cadena();
                for (int i = 0; i < ld.Count; i++)
                {
                    ld[i].PAYER = cad.completaCliente(ld[i].PAYER);
                }
                List<CLIENTE> cli = db.CLIENTEs.ToList();
                List<CLIENTE> c = (from cl in db.CLIENTEs.ToList()
                                   join v in ld
                                   on cl.KUNNR equals v.PAYER
                                   select cl).ToList();
                //var c = cli.Join(ld, s => s.KUNNR, l => l.PAYER, (s, l) => new { ku = s.KUNNR, na = s.NAME1 }).ToList();
                if (c.Count > 0)
                    for (int i = 0; i < ld.Count; i++)
                    {
                        ld[i].DESCRIPCION = c.Where(x => x.KUNNR == ld[i].PAYER).Select(x => x.NAME1).ToList().FirstOrDefault().ToString();
                    }
                else
                    for (int i = 0; i < ld.Count; i++)
                    {
                        ld[i].DESCRIPCION = "";
                    }
                //jemo 10-17-2018 fin
                reader.Close();

            }
            JsonResult jl = Json(ld, JsonRequestBehavior.AllowGet);
            return jl;
        }

        //LEJ 30.07.2018---------------------------------------------I
        [HttpPost]
        [AllowAnonymous]
        public JsonResult getPeriodo(DateTime fecha)
        {
            Calendario445 c = new Calendario445();
            var _f = c.getPeriodoF(fecha);
            JsonResult jl = Json(_f, JsonRequestBehavior.AllowGet);
            return jl;
        }
        //LEJ 30.07.2018---------------------------------------------T
        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoadConfigSoporte(string sociedad, string pais, string tsol, string nulos, string class_doc)//jemo 09-07-2018
        {
            if (nulos == null)
            {
                nulos = "";
            }
            FACTURASCONF_MOD fc = new FACTURASCONF_MOD();

            try
            {
                FACTURASCONF f = db.FACTURASCONFs.Where(fi => fi.SOCIEDAD_ID.Equals(sociedad) && fi.PAIS_ID.Equals(pais) && fi.TSOL.Equals(tsol)).FirstOrDefault();
                //jemo 09-07-2018 inicio
                if (class_doc == "true")
                {
                    string s = db.TSOLs.Where(x => x.ID == tsol).Select(x => x.TSOLM).SingleOrDefault();
                    if (String.IsNullOrEmpty(s) == false)
                    {
                        f = db.FACTURASCONFs.Where(fi => fi.SOCIEDAD_ID.Equals(sociedad) && fi.PAIS_ID.Equals(pais) && fi.TSOL.Equals(s)).FirstOrDefault();
                    }
                }
                //jemo 09-07-2018 fin
                if (f != null)
                {
                    fc.NUM_DOC = null;
                    fc.POS = true;
                    fc.SOCIEDAD_ID = f.SOCIEDAD_ID;
                    fc.PAIS_ID = f.PAIS_ID;
                    fc.TSOL = f.TSOL;
                    fc.FACTURA = f.FACTURA;
                    fc.SOCIEDAD = f.SOCIEDAD;// jemo 06-08-2018
                    fc.FECHA = f.FECHA;
                    fc.PROVEEDOR = f.PROVEEDOR;
                    fc.PROVEEDOR_TXT = f.PROVEEDOR;
                    fc.CONTROL = f.CONTROL;
                    fc.AUTORIZACION = f.AUTORIZACION;
                    fc.VENCIMIENTO = f.VENCIMIENTO;
                    fc.FACTURAK = f.FACTURAK;
                    fc.EJERCICIOK = f.EJERCICIOK;
                    fc.BILL_DOC = f.BILL_DOC;
                    fc.BELNR = f.BELNR;
                    //jemo 09-07-2018 inicio
                    fc.IMPORTE_FAC = f.IMPORTE_FAC;
                    fc.PAYER = f.PAYER;
                    fc.DESCRIPCION = f.DESCRIPCION;
                    //jemo 09-07-2018 fin
                    //fc.FACTURA = true; 
                    //fc.FECHA = true; 
                    //fc.PROVEEDOR = true;
                    //fc.PROVEEDOR_TXT = true; 
                    //fc.CONTROL = true;
                    //fc.AUTORIZACION = true;
                    //fc.VENCIMIENTO = true;
                    //fc.FACTURAK = true;
                    //fc.EJERCICIOK = true;
                    //fc.BILL_DOC = true;
                    //fc.BELNR = true;
                }
            }
            catch
            {

            }

            if (nulos.Equals("X"))
            {

                fc.PROVEEDOR_TXT = null;
                fc.NUM_DOC = 0;
                fc.SOCIEDAD_ID = null;
                fc.PAIS_ID = null;
                fc.TSOL = null;

            }

            JsonResult jl = Json(fc, JsonRequestBehavior.AllowGet);
            return jl;
        }

        public string grupoMaterialesRel(List<DOCUMENTOP> cats, List<DOCUMENTOM> docsrelm)
        {
            TruequeEntities db = new TruequeEntities();

            List<MATERIAL> lmat = new List<MATERIAL>();
            //Obtener la descripción del material
            lmat = (from m in docsrelm
                    join mt in db.MATERIALs
                    on m.MATNR equals mt.ID into jjcont
                    from co in jjcont.DefaultIfEmpty()
                    select new MATERIAL
                    {
                        ID = m.MATNR,
                        //MTART = co.MTART,
                        //MATKL_ID = co.MATKL_ID,
                        MAKTX = co == null ? String.Empty : co.MAKTX,
                        MAKTG = co == null ? String.Empty : co.MAKTG,
                        //MEINS = co.MEINS,
                        //PUNIT = co.PUNIT,
                        //ACTIVO = co.ACTIVO,
                        //CTGR = co.CTGR,
                        //BRAND = co.BRAND,
                        //MATERIALGP_ID = co.MATERIALGP_ID
                    }).ToList();

            //Obtener las categorías
            var categorias = cats.GroupBy(c => c.MATKL, c => new { ID = c.MATKL.ToString() }).ToList();

            List<CategoriaMaterial> lcatmat = new List<CategoriaMaterial>();

            foreach (DOCUMENTOP item in cats)
            {
                CategoriaMaterial cm = new CategoriaMaterial();
                cm.ID = item.MATKL;

                //Obtener los materiales de la categoría
                List<DOCUMENTOM_MOD> dl = new List<DOCUMENTOM_MOD>();
                dl = docsrelm.Where(c => c.POS_ID == item.POS).Select(c => new DOCUMENTOM_MOD { ID_CAT = item.MATKL, MATNR = c.MATNR, VAL = Convert.ToDecimal(c.VALORH), POR = Convert.ToDecimal(c.PORC_APOYO) }).ToList();

                foreach (DOCUMENTOM_MOD doc in dl)
                {
                    //Buscar la desc
                    string desc = "";
                    desc = lmat.Where(m => m.ID == doc.MATNR).FirstOrDefault().MAKTX;
                    if (desc == "")
                    {
                        desc = lmat.Where(m => m.ID == doc.MATNR).FirstOrDefault().MAKTG;
                    }
                    doc.DESC = desc;
                }
                cm.MATERIALES = dl;
                lcatmat.Add(cm);
            }

            //JsonResult jl = Json(lcatmat,"application/json",Encoding.UTF8, JsonRequestBehavior.AllowGet);
            string d = "";
            d = JsonConvert.SerializeObject(lcatmat, Formatting.Indented);
            return d;
        }


        public List<CategoriaMaterial> grupoMaterialesController(List<string> catstabla, string vkorg, string spart, string kunnr, string soc_id, out decimal total)
        {
            TruequeEntities db = new TruequeEntities();
            if (kunnr == null)
            {
                kunnr = "";
            }
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            List<DOCUMENTOM_MOD> jd = new List<DOCUMENTOM_MOD>();

            //Obtener los materiales
            IEnumerable<MATERIAL> matl = Enumerable.Empty<MATERIAL>();
            try
            {
                matl = db.MATERIALs.Where(m => m.ACTIVO == true);//.Select(m => m.ID).ToList();
            }
            catch (Exception e)
            {

            }

            //Validar si hay materiales
            if (matl != null)
            {

                CLIENTE cli = new CLIENTE();
                List<CLIENTE> clil = new List<CLIENTE>();

                try
                {
                    cli = db.CLIENTEs.Where(c => c.KUNNR == kunnr && c.VKORG == vkorg && c.SPART == spart).FirstOrDefault();

                    //Saber si el cliente es sold to, payer o un grupo
                    if (cli != null)
                    {
                        //Es un soldto
                        if (cli.KUNNR != cli.PAYER && cli.KUNNR != cli.BANNER)
                        {
                            //cli.VKORG = cli.VKORG+" ";
                            clil.Add(cli);
                        }
                    }
                }
                catch (Exception e)
                {

                }

                var cie = clil.Cast<CLIENTE>();
                //    IEnumerable<CLIENTE> cie = clil as IEnumerable<CLIENTE>;
                //Obtener el numero de periodos para obtener el historial
                int? mesesVenta = (db.CONFDIST_CAT.Any(x => x.SOCIEDAD_ID == soc_id) ? db.CONFDIST_CAT.First(x => x.SOCIEDAD_ID == soc_id).PERIODOS : null);
                int nummonths = (mesesVenta != null ? mesesVenta.Value : DateTime.Now.Month);
                int imonths = nummonths * -1;
                //Obtener el rango de los periodos incluyendo el año
                DateTime ff = DateTime.Today;
                DateTime fi = ff.AddMonths(imonths);

                string mi = fi.Month.ToString();//.ToString("MM");
                string ai = fi.Year.ToString();//.ToString("yyyy");

                string mf = ff.Month.ToString();// ("MM");
                string af = ff.Year.ToString();// "yyyy");

                int aii = 0;
                try
                {
                    aii = Convert.ToInt32(ai);
                }
                catch (Exception e)
                {

                }

                int mii = 0;
                try
                {
                    mii = Convert.ToInt32(mi);
                }
                catch (Exception e)
                {

                }

                int aff = 0;
                try
                {
                    aff = Convert.ToInt32(af);
                }
                catch (Exception e)
                {

                }

                int mff = 0;
                try
                {
                    mff = Convert.ToInt32(mf);
                }
                catch (Exception e)
                {

                }

                if (cie != null)
                {
                    ////    //Obtener el historial de compras de los clientesd
                    ////    var matt = matl.ToList();
                    ////    //kunnr = kunnr.TrimStart('0').Trim();
                    ////    var pres = db.PRESUPSAPPs.Where(a => a.VKORG.Equals(vkorg) && a.SPART.Equals(spart) && a.KUNNR == kunnr && (a.GRSLS != null | a.NETLB != null)).ToList();
                    ////    var cat = db.MATERIALGPTs.Where(a => a.SPRAS_ID.Equals("EN")).ToList();
                    ////    //foreach (var c in cie)
                    ////    //{
                    ////    //    c.KUNNR = c.KUNNR.TrimStart('0').Trim();
                    ////    //}

                    ////    CONFDIST_CAT conf = getCatConf(soc_id);
                    ////    if (conf.CAMPO == "GRSLS")
                    ////    {
                    ////        jd = (from ps in pres
                    ////              join cl in cie
                    ////              on ps.KUNNR equals cl.KUNNR
                    ////              join m in matt
                    ////              on ps.MATNR equals m.ID
                    ////              join mk in cat
                    ////              on m.MATERIALGP_ID equals mk.MATERIALGP_ID
                    ////              where (ps.ANIO >= aii && ps.PERIOD >= mii) && (ps.ANIO <= aff && ps.PERIOD <= mff) &&
                    ////              (ps.VKORG == cl.VKORG && ps.VTWEG == cl.VTWEG && ps.SPART == cl.SPART
                    ////              ) && ps.BUKRS == soc_id
                    ////              && ps.GRSLS > 0
                    ////              select new DOCUMENTOM_MOD
                    ////              {
                    ////                  ID_CAT = m.MATERIALGP_ID,
                    ////                  MATNR = ps.MATNR,
                    ////                  //mk.TXT50
                    ////                  VAL = Convert.ToDecimal(ps.GRSLS),
                    ////                  EXCLUIR = mk.MATERIALGP.EXCLUIR // RSG 09.07.2018 ID 156
                    ////              }).ToList();
                    ////    }
                    ////    else
                    ////    {
                    ////        jd = (from ps in pres
                    ////              join cl in cie
                    ////              on ps.KUNNR equals cl.KUNNR
                    ////              join m in matt
                    ////              on ps.MATNR equals m.ID
                    ////              join mk in cat
                    ////              on m.MATERIALGP_ID equals mk.MATERIALGP_ID
                    ////              where (ps.ANIO >= aii && ps.PERIOD >= mii) && (ps.ANIO <= aff && ps.PERIOD <= mff) &&
                    ////              (ps.VKORG == cl.VKORG && ps.VTWEG == cl.VTWEG && ps.SPART == cl.SPART
                    ////              ) && ps.BUKRS == soc_id
                    ////              && ps.NETLB > 0
                    ////              select new DOCUMENTOM_MOD
                    ////              {
                    ////                  ID_CAT = m.MATERIALGP_ID,
                    ////                  MATNR = ps.MATNR,
                    ////                  //mk.TXT50
                    ////                  VAL = Convert.ToDecimal(ps.NETLB),
                    ////                  EXCLUIR = mk.MATERIALGP.EXCLUIR // RSG 09.07.2018 ID 156
                    ////              }).ToList();
                    ////    }
                    ///
                    if (cie != null)
                    {
                        jd = materialesgptDao.ListaMaterialGroupsMateriales(vkorg, spart, kunnr, soc_id, aii, mii, aff, mff, User.Identity.Name);
                    }

                }
            }

            //Obtener las categorías
            var categoriasl = jd.GroupBy(c => c.ID_CAT, c => new { ID = c.ID_CAT.ToString() }).ToList();
            List<string> categorias = new List<string>();
            //Diferencia del método de la vista jquery
            //Tomar en cuenta nada más las categorías que se agregaron a la tabla y que se enviaron en el submmit 
            for (int h = 0; h < catstabla.Count; h++)
            {
                for (int j = 0; j < categoriasl.Count; j++)
                {
                    if (catstabla[h].ToString() == categoriasl[j].Key.ToString() | catstabla[h].ToString() == "000")
                    {
                        categorias.Add(categoriasl[j].Key.ToString());
                    }
                }
            }

            List<CategoriaMaterial> lcatmat = new List<CategoriaMaterial>();
            decimal t = 0;
            foreach (string item in categorias)
            {
                CategoriaMaterial cm = new CategoriaMaterial();
                cm.ID = item;
                cm.EXCLUIR = jd.Where(x => x.ID_CAT.Equals(item)).FirstOrDefault().EXCLUIR;//RSG 09.07.2018 ID 156

                //Obtener los materiales de la categoría
                List<DOCUMENTOM_MOD> dl = new List<DOCUMENTOM_MOD>();
                List<DOCUMENTOM_MOD> dm = new List<DOCUMENTOM_MOD>();
                //dl = jd.Where(c => c.ID_CAT == item).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL}).ToList();//Falta obtener el groupby
                dl = jd.Where(c => c.ID_CAT == item).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, EXCLUIR = c.EXCLUIR }).ToList();//RSG 09.07.2018 ID 156

                //Obtener la descripción de los materiales
                foreach (DOCUMENTOM_MOD d in dl)
                {
                    DOCUMENTOM_MOD dcl = new DOCUMENTOM_MOD();
                    //dcl = dm.Where(z => z.MATNR == d.MATNR).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL }).FirstOrDefault();//RSG 09.07.2018 ID 156
                    dcl = dm.Where(z => z.MATNR == d.MATNR).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, EXCLUIR = c.EXCLUIR }).FirstOrDefault();//RSG 09.07.2018 ID 156

                    if (dcl == null)
                    {
                        DOCUMENTOM_MOD dcll = new DOCUMENTOM_MOD();
                        //No se ha agregado
                        decimal val = dl.Where(y => y.MATNR == d.MATNR).Sum(x => x.VAL);
                        dcll.ID_CAT = item;
                        dcll.MATNR = d.MATNR;

                        //Obtener la descripción del material
                        dcll.DESC = db.MATERIALs.Where(w => w.ID == d.MATNR).FirstOrDefault().MAKTG.ToString();
                        dcll.VAL = val;
                        t += val;
                        cm.TOTALCAT += val;
                        dm.Add(dcll);
                    }
                }

                cm.MATERIALES = dm;
                lcatmat.Add(cm);
            }

            total = t;

            return lcatmat;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult categoriaMateriales(string kunnr, string catid, string soc_id)
        {
            if (kunnr == null)
            {
                kunnr = "";
            }

            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            if (catid == null)
            {
                catid = "";
            }

            List<PRESUPSAPP> jdl = new List<PRESUPSAPP>();

            //Obtener los materiales
            IEnumerable<MATERIAL> matl = Enumerable.Empty<MATERIAL>();
            try
            {
                if (catid != "000")//RSG 05.06.2018
                    matl = db.MATERIALs.Where(m => m.MATERIALGP_ID == catid && m.ACTIVO == true);//.Select(m => m.ID).ToList();
                else
                    matl = db.MATERIALs.Where(m => m.ACTIVO == true);//.Select(m => m.ID).ToList();
            }
            catch (Exception)
            {

            }

            //Validar si hay materiales
            string campoconf = "";
            if (matl != null)
            {

                CLIENTE cli = new CLIENTE();
                List<CLIENTE> clil = new List<CLIENTE>();

                try
                {
                    cli = db.CLIENTEs.Where(c => c.KUNNR == kunnr).FirstOrDefault();

                    //Saber si el cliente es sold to, payer o un grupo
                    if (cli != null)
                    {
                        //Es un soldto
                        if (cli.KUNNR != cli.PAYER && cli.KUNNR != cli.BANNER)
                        {
                            //cli.VKORG = cli.VKORG+" ";
                            clil.Add(cli);
                        }
                    }
                }
                catch (Exception)
                {

                }

                var cie = clil.Cast<CLIENTE>();
                //    IEnumerable<CLIENTE> cie = clil as IEnumerable<CLIENTE>;
                //Obtener el numero de periodos para obtener el historial
                int nummonths = 0;
                int imonths = 0;

                try
                {
                    CONFDIST_CAT conf = getCatConf(soc_id);
                    nummonths = (int)conf.PERIODOS;
                    campoconf = conf.CAMPO.ToString();

                }
                catch (Exception)
                {

                }
                if (nummonths > 0)
                {
                    imonths = nummonths * -1;
                }
                //Obtener el rango de los periodos incluyendo el año
                DateTime ff = DateTime.Today;
                DateTime fi = ff.AddMonths(imonths);

                string mi = fi.Month.ToString();//.ToString("MM");
                string ai = fi.Year.ToString();//.ToString("yyyy");

                string mf = ff.Month.ToString();// ("MM");
                string af = ff.Year.ToString();// "yyyy");

                int aii = 0;
                try
                {
                    aii = Convert.ToInt32(ai);
                }
                catch (Exception)
                {

                }

                int mii = 0;
                try
                {
                    mii = Convert.ToInt32(mi);
                }
                catch (Exception)
                {

                }

                int aff = 0;
                try
                {
                    aff = Convert.ToInt32(af);
                }
                catch (Exception)
                {

                }

                int mff = 0;
                try
                {
                    mff = Convert.ToInt32(mf);
                }
                catch (Exception)
                {

                }

                if (cie != null)
                {
                    //Obtener el historial de compras de los clientesd
                    var matt = matl.ToList();
                    //var pres = db.PRESUPSAPPs.ToList();
                    ////kunnr = kunnr.TrimStart('0').Trim();
                    var pres = db.PRESUPSAPPs.Where(a => a.VKORG.Equals(cli.VKORG) && a.SPART.Equals(cli.SPART) && a.KUNNR == kunnr).ToList();
                    ////foreach (var c in cie)
                    ////{
                    ////    c.KUNNR = c.KUNNR.TrimStart('0').Trim();
                    ////}

                    //jd = (from ps in db.PRESUPSAPPs.ToList()
                    jdl = (from ps in pres
                           join cl in cie
                           on ps.KUNNR equals cl.KUNNR
                           join m in matt
                           on ps.MATNR equals m.ID
                           where (ps.ANIO >= aii && ps.PERIOD >= mii) && (ps.ANIO <= aff && ps.PERIOD <= mff) &&
                           (ps.VKORG == cl.VKORG && ps.VTWEG == cl.VTWEG && ps.SPART == cl.SPART //&& ps.VKBUR == cl.VKBUR &&
                                                                                                 //ps.VKGRP == cl.VKGRP && ps.BZIRK == cl.BZIRK
                           ) && ps.BUKRS == soc_id
                           select new PRESUPSAPP
                           {
                               ID = ps.ID,
                               ANIO = ps.ANIO,
                               POS = ps.POS,
                               PERIOD = ps.PERIOD,
                               MATNR = ps.MATNR,
                               VVX17 = ps.VVX17,
                               CSHDC = ps.CSHDC,
                               RECUN = ps.RECUN,
                               DSTRB = ps.DSTRB,
                               OTHTA = ps.OTHTA,
                               ADVER = ps.ADVER,
                               CORPM = ps.CORPM,
                               POP = ps.POP,
                               OTHER = ps.OTHER,
                               CONPR = ps.CONPR,
                               OHV = ps.OHV,
                               FREEG = ps.FREEG,
                               RSRDV = ps.RSRDV,
                               SPA = ps.SPA,
                               PMVAR = ps.PMVAR,
                               GRSLS = ps.GRSLS,
                               NETLB = ps.NETLB
                           }).ToList();
                }
            }

            //var jll = db.PRESUPSAPPs.Select(psl => new { MATNR = psl.MATNR.ToString() }).Take(7).ToList();

            //List<PRESUPSAPP> lps = jd;

            List<PRESUPSAPP> jdlret = new List<PRESUPSAPP>();

            foreach (PRESUPSAPP p in jdl)
            {
                var pd = p.GetType().GetProperties();

                var v = pd.Where(x => x.Name == campoconf).Single().GetValue(p);

                decimal val = Convert.ToDecimal(v);

                if (val > 0)
                {
                    PRESUPSAPP pp = jdlret.Where(a => a.MATNR == p.MATNR).FirstOrDefault();
                    if (pp == null)
                    {
                        jdlret.Add(p);
                    }
                }
            }



            JsonResult jl = Json(jdlret, JsonRequestBehavior.AllowGet);
            return jl;
        }


        public List<DOCUMENTOM> addCatItems(List<CategoriaMaterial> categorias, string kunnr, string catid,
            string soc_id, decimal numdoc, int posid, DateTime? vig_de, DateTime? vig_a, string neg, string dis, decimal total, decimal totaldoc, string col)
        {
            //if (kunnr == null)
            //{
            //    kunnr = "";
            //}

            //if (catid == null)
            //{
            //    catid = "";
            //}

            ////var jd = (dynamic)null;

            ////List<DOCUMENTOM> jd = new List<DOCUMENTOM>();
            //List<PRESUPSAPP> jdl = new List<PRESUPSAPP>();
            ////Obtener los materiales
            //IEnumerable<MATERIAL> matl = Enumerable.Empty<MATERIAL>();
            //try
            //{
            //    matl = db.MATERIALs.Where(m => m.MATKL_ID == catid && m.ACTIVO == true);//.Select(m => m.ID).ToList();
            //}
            //catch (Exception)
            //{

            //}

            ////Validar si hay materiales
            //string campoconf = "";
            //if (matl != null)
            //{

            //    CLIENTE cli = new CLIENTE();
            //    List<CLIENTE> clil = new List<CLIENTE>();

            //    try
            //    {
            //        cli = db.CLIENTEs.Where(c => c.KUNNR == kunnr).FirstOrDefault();

            //        //Saber si el cliente es sold to, payer o un grupo
            //        if (cli != null)
            //        {
            //            //Es un soldto
            //            if (cli.KUNNR != cli.PAYER && cli.KUNNR != cli.BANNER)
            //            {
            //                clil.Add(cli);
            //            }
            //        }
            //    }
            //    catch (Exception)
            //    {

            //    }

            //    var cie = clil.Cast<CLIENTE>();
            //    //    IEnumerable<CLIENTE> cie = clil as IEnumerable<CLIENTE>;
            //    //Obtener el numero de periodos para obtener el historial
            //    int nummonths = 0;
            //    int imonths = 0;
            //    try
            //    {
            //        CONFDIST_CAT conf = getCatConf(soc_id);
            //        nummonths = (int)conf.PERIODOS;
            //        campoconf = conf.CAMPO.ToString();
            //    }
            //    catch (Exception)
            //    {

            //    }
            //    if (nummonths > 0)
            //    {
            //        imonths = nummonths * -1;
            //    }
            //    //Obtener el rango de los periodos incluyendo el año
            //    DateTime ff = DateTime.Today;
            //    DateTime fi = ff.AddMonths(imonths);

            //    string mi = fi.Month.ToString();//.ToString("MM");
            //    string ai = fi.Year.ToString();//.ToString("yyyy");

            //    string mf = ff.Month.ToString();// ("MM");
            //    string af = ff.Year.ToString();// "yyyy");

            //    int aii = 0;
            //    try
            //    {
            //        aii = Convert.ToInt32(ai);
            //    }
            //    catch (Exception)
            //    {

            //    }

            //    int mii = 0;
            //    try
            //    {
            //        mii = Convert.ToInt32(mi);
            //    }
            //    catch (Exception)
            //    {

            //    }

            //    int aff = 0;
            //    try
            //    {
            //        aff = Convert.ToInt32(af);
            //    }
            //    catch (Exception)
            //    {

            //    }

            //    int mff = 0;
            //    try
            //    {
            //        mff = Convert.ToInt32(mf);
            //    }
            //    catch (Exception)
            //    {

            //    }

            //    if (cie != null)
            //    {
            //        //Obtener el historial de compras de los clientesd
            //        var matt = matl.ToList();
            //        //var pres = db.PRESUPSAPPs.ToList();
            //        kunnr = kunnr.TrimStart('0').Trim();
            //        var pres = db.PRESUPSAPPs.Where(a => a.VKORG.Equals(cli.VKORG) && a.SPART.Equals(cli.SPART) && a.KUNNR == kunnr).ToList();
            //        List<CLIENTE> ciee = new List<CLIENTE>();
            //        foreach (var c in cie)
            //        {
            //            CLIENTE pa = new CLIENTE();
            //            pa.VKORG = c.VKORG;
            //            pa.VTWEG = c.VTWEG;
            //            pa.SPART = c.SPART;
            //            pa.KUNNR = c.KUNNR.TrimStart('0').Trim();
            //            ciee.Add(pa);
            //        }

            //        jdl = (from ps in pres
            //               join cl in ciee
            //               on ps.KUNNR equals cl.KUNNR
            //               join m in matt
            //               on ps.MATNR equals m.ID
            //               where (ps.ANIO >= aii && ps.PERIOD >= mii) && (ps.ANIO <= aff && ps.PERIOD <= mff) &&
            //               (ps.VKORG == cl.VKORG && ps.VTWEG == cl.VTWEG && ps.SPART == cl.SPART //&& ps.VKBUR == cl.VKBUR &&
            //                                                                                     //ps.VKGRP == cl.VKGRP && ps.BZIRK == cl.BZIRK
            //               ) && ps.BUKRS == soc_id
            //               select new PRESUPSAPP
            //               {
            //                   ID = ps.ID,
            //                   ANIO = ps.ANIO,
            //                   POS = ps.POS,
            //                   PERIOD = ps.PERIOD,
            //                   MATNR = ps.MATNR,
            //                   VVX17 = ps.VVX17,
            //                   CSHDC = ps.CSHDC,
            //                   RECUN = ps.RECUN,
            //                   DSTRB = ps.DSTRB,
            //                   OTHTA = ps.OTHTA,
            //                   ADVER = ps.ADVER,
            //                   CORPM = ps.CORPM,
            //                   POP = ps.POP,
            //                   OTHER = ps.OTHER,
            //                   CONPR = ps.CONPR,
            //                   OHV = ps.OHV,
            //                   FREEG = ps.FREEG,
            //                   RSRDV = ps.RSRDV,
            //                   SPA = ps.SPA,
            //                   PMVAR = ps.PMVAR,
            //                   GRSLS = ps.GRSLS,
            //                   NETLB = ps.NETLB
            //               }).ToList();

            //    }
            //}

            //var jll = db.PRESUPSAPPs.Select(psl => new { MATNR = psl.MATNR.ToString() }).Take(7).ToList();

            //List<DOCUMENTOM> jdlret = new List<DOCUMENTOM>();

            //foreach (PRESUPSAPP p in jdl)
            //{
            //    var pd = p.GetType().GetProperties();

            //    var v = pd.Where(x => x.Name == campoconf).Single().GetValue(p);

            //    decimal val = Convert.ToDecimal(v);

            //    if (val > 0)
            //    {
            //        DOCUMENTOM dm = new DOCUMENTOM();
            //        dm = jdlret.Where(a => a.MATNR == p.MATNR).FirstOrDefault();
            //        if (dm == null)
            //        {
            //            dm = new DOCUMENTOM();
            //            dm.NUM_DOC = numdoc;
            //            dm.POS_ID = posid;
            //            dm.MATNR = p.MATNR;
            //            dm.VIGENCIA_DE = vig_de;
            //            dm.VIGENCIA_A = vig_a;

            //            jdlret.Add(dm);
            //        }
            //    }
            //}
            List<DOCUMENTOM> jdlret = new List<DOCUMENTOM>();

            //Negaciación por monto

            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            //Obtener de la lista de categorias los materiales de la categoría del item
            List<CategoriaMaterial> ccategor = new List<CategoriaMaterial>();
            if (catid == "000")//ccategor = categorias.ToList();//RSG 09.07.2018 ID 156
                ccategor = categorias.Where(a => a.EXCLUIR == false).ToList();//RSG 09.07.2018 ID 156
            else
                ccategor = categorias.Where(c => c.ID == catid).ToList();
            foreach (CategoriaMaterial categor in ccategor)
            {
                //CategoriaMaterial categor = categorias.Where(c => c.ID == catid).FirstOrDefault();
                List<DOCUMENTOM_MOD> materiales = new List<DOCUMENTOM_MOD>();
                materiales = categor.MATERIALES;
                foreach (DOCUMENTOM_MOD docm in materiales)
                {
                    DOCUMENTOM dm = new DOCUMENTOM();
                    dm.NUM_DOC = numdoc;
                    dm.POS_ID = posid;
                    dm.MATNR = docm.MATNR;
                    dm.VIGENCIA_DE = vig_de;
                    dm.VIGENCIA_A = vig_a;
                    if (dis == "C")
                    {
                        //dm.APOYO_EST = docm.VAL;
                        dm.VALORH = docm.VAL; //MGC B20180611 Se agregó el valor del material del historico de la provisión
                    }
                    jdlret.Add(dm);
                }
            }


            if (dis == "C")
            {
                foreach (DOCUMENTOM docm in jdlret)
                {
                    //Prcentaje
                    decimal por = 0;
                    try
                    {
                        por = Convert.ToDecimal((docm.VALORH * 100) / total); //B20180618 v1 MGC 2018.06.18
                    }
                    catch (Exception)
                    {

                    }
                    decimal totalmat = 0;

                    try
                    {
                        totalmat = Convert.ToDecimal((por * totaldoc) / 100);
                    }
                    catch (Exception)
                    {

                    }

                    docm.PORC_APOYO = por;
                    docm.APOYO_EST = 0;
                    if (col == "E")
                    {
                        docm.APOYO_EST = totalmat;
                    }
                    else if (col == "R")
                    {
                        docm.APOYO_REAL = totalmat;
                    }

                }
            }

            return jdlret;
        }

        public CONFDIST_CAT getCatConf(string soc)
        {
            CONFDIST_CAT conf = new CONFDIST_CAT();

            try
            {
                conf = db.CONFDIST_CAT.Where(c => c.SOCIEDAD_ID == soc && c.ACTIVO == true).FirstOrDefault();
            }
            catch (Exception)
            {

            }

            return conf;
        }



        [HttpPost]
        [AllowAnonymous]
        public string saveFiles()
        {
            var res = "";
            string error = "";
            string path = "";
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string url = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") && x.ACTIVO).FirstOrDefault().VALUE;
                    HttpPostedFileBase file = Request.Files[i];
                    string filename = file.FileName;
                    //res = SaveFile(file, url, "100", out error, out path);//RSG 01.08.2018
                    res = new Files().SaveFile(file, url, "100", out error, out path, "2018");//RSG 01.08.2018
                }
            }

            return res;
        }

        //public string createDir(string path, string documento, string ejercicio)
        //{

        //    string ex = "";

        //    // Specify the path to save the uploaded file to.
        //    //string savePath = path + documento + "\\";
        //    string savePath = path + ejercicio + "\\" + documento + "\\";//RSG 01.08.2018

        //    // Create the path and file name to check for duplicates.
        //    string pathToCheck = savePath;

        //    try
        //    {
        //        ////using (Impersonation.LogonUser("192.168.1.77", "EQUIPO", "0906", LogonType.NewCredentials))
        //        ////{
        //        if (!System.IO.File.Exists(pathToCheck))
        //        {
        //            //No existe, se necesita crear
        //            DirectoryInfo dir = new DirectoryInfo(pathToCheck);

        //            dir.Create();

        //        }
        //        ////}

        //        //file.SaveAs(Server.MapPath(savePath)); //Guardarlo el cualquier parte dentro del proyecto <add key="URL_SAVE" value="\Archivos\" />
        //        //System.IO.File.Create(savePath,100,FileOptions.DeleteOnClose, )
        //        //System.IO.File.Copy(copyFrom, savePath);
        //        //f.CopyTo(savePath,true);
        //    }
        //    catch (Exception e)
        //    {
        //        ex = "No se puede crear el directorio para guardar los archivos";
        //    }

        //    return ex;
        //}

        //public string SaveFile(HttpPostedFileBase file, string path, string documento, out string exception, out string pathsaved, string ejercicio)
        //{
        //    string ex = "";
        //    //string exdir = "";
        //    // Get the name of the file to upload.
        //    string fileName = file.FileName;//System.IO.Path.GetExtension(file.FileName);    // must be declared in the class above

        //    // Specify the path to save the uploaded file to.
        //    //string savePath = path + documento + "\\";//RSG 01.08.2018
        //    string savePath = path + ejercicio + "\\" + documento + "\\";//RSG 01.08.2018

        //    // Create the path and file name to check for duplicates.
        //    string pathToCheck = savePath;

        //    // Append the name of the file to upload to the path.
        //    savePath += fileName;

        //    // Call the SaveAs method to save the uploaded
        //    // file to the specified directory.
        //    //file.SaveAs(Server.MapPath(savePath));

        //    //file to domain
        //    //Parte para guardar archivo en el servidor
        //    ////using (Impersonation.LogonUser("192.168.1.77", "EQUIPO", "0906", LogonType.NewCredentials))
        //    ////{
        //    //fileName = file.SaveAs(file, Server.MapPath("~/Nueva carpeta/") + file.FileName);
        //    try
        //    {


        //        //Guardar el archivo
        //        file.SaveAs(savePath);


        //    }
        //    catch (Exception e)
        //    {
        //        ex = "";
        //        ex = fileName;
        //    }
        //    ////}

        //    //Guardarlo en la base de datos
        //    if (ex == "")
        //    {

        //    }
        //    pathsaved = savePath;
        //    exception = ex;
        //    return fileName;
        //}

        [HttpPost]
        [AllowAnonymous]
        public string cambioCurr(string fcurr, string tcurr, string monto)
        {
            string p = "";
            string errorString = "";
            decimal montoret = 0;
            try
            {
                p = Session["pais"].ToString();
            }
            catch
            {
            }

            TruequeEntities db = new TruequeEntities();

            var id_bukrs = db.SOCIEDADs.Where(soc => soc.LAND.Equals(p)).FirstOrDefault();
            try
            {
                var date = DateTime.Now.Date;
                //var UKURS = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(date)).FirstOrDefault().UKURS;
                var tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(date)).FirstOrDefault();
                if (tc == null)
                {
                    var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr)).Max(a => a.GDATU);
                    tc = db.TCAMBIOs.Where(t => t.FCURR.Equals(fcurr) && t.TCURR.Equals(tcurr) && t.GDATU.Equals(max)).FirstOrDefault();
                }

                decimal uk = Convert.ToDecimal(tc.UKURS);

                if (tc.UKURS > 0)
                {
                    montoret = Convert.ToDecimal(monto) / uk;
                }
            }
            catch (Exception e)
            {
                errorString = e.Message + "detail: conversion " + fcurr + " to " + tcurr + " in date " + DateTime.Now.Date;
                //throw new System.Exception(errorString);
                return errorString;
            }
            return Convert.ToString(montoret);
        }

        [HttpPost]
        public JsonResult materiales(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";

            TruequeEntities db = new TruequeEntities();

            var c = (from m in db.MATERIALs
                     where m.ID.Contains(Prefix) && m.ACTIVO == true
                     select new { m.ID, m.MAKTX }).ToList();
            if (c.Count == 0)
            {
                var c2 = (from m in db.MATERIALs
                          where m.MAKTX.Contains(Prefix) && m.ACTIVO == true
                          select new { m.ID, m.MAKTX }).ToList();
                c.AddRange(c2);
            }
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]
        public JsonResult proveedores(string Prefix, string kunnr)
        {
            if (Prefix == null)
                Prefix = "";

            TruequeEntities db = new TruequeEntities();
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            var c = (from m in db.PROVEEDORs
                     where m.ID.Contains(Prefix) && m.CLIENTEs.Any(x => x.KUNNR == kunnr)
                     select new { m.ID, m.NOMBRE }).ToList();
            if (c.Count == 0)
            {
                var c2 = (from m in db.PROVEEDORs
                          where m.NOMBRE.Contains(Prefix) && m.CLIENTEs.Any(x => x.KUNNR == kunnr)
                          select new { m.ID, m.NOMBRE }).ToList();
                c.AddRange(c2);
            }
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        public MATERIAL material(string material)
        {
            if (material == null)
                material = "";
            //RSG 07.06.2018---------------------------------------------
            material = new Cadena().completaMaterial(material);
            //RSG 07.06.2018---------------------------------------------

            TruequeEntities db = new TruequeEntities();

            MATERIAL mat = db.MATERIALs.Where(m => m.ID == material).FirstOrDefault();

            return mat;
        }

        public PROVEEDOR proveedor(string proveedor)
        {
            if (proveedor == null)
                proveedor = "";

            PROVEEDOR pro = db.PROVEEDORs.Where(p => p.ID == proveedor).FirstOrDefault();

            return pro;
        }


        [HttpPost]
        public JsonResult getMaterial(string mat)
        {
            if (mat == null)
                mat = "";
            //RSG 07.06.2018---------------------------------------------
            mat = new Cadena().completaMaterial(mat);
            //RSG 07.06.2018---------------------------------------------
            string spras = FnCommon.ObtenerSprasId(db, User.Identity.Name);
            MaterialVal matt = db.MATERIALTs.Where(m => m.MATERIAL_ID == mat && m.SPRAS == spras).Select(m => new MaterialVal { ID = m.MATERIAL_ID.ToString(), MATKL_ID = m.MATERIAL.MATERIALGP_ID.ToString(), MAKTX = m.MAKTX.ToString() }).FirstOrDefault();
            if (matt == null)
                matt = db.MATERIALs.Where(m => m.ID == mat).Select(m => new MaterialVal { ID = m.ID.ToString(), MATKL_ID = m.MATERIALGP_ID.ToString(), MAKTX = m.MAKTX.ToString() }).FirstOrDefault();

            JsonResult cc = Json(matt, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]
        public JsonResult getProveedor(string prov)
        {
            if (prov == null)
                prov = "";

            ProveedorVal provv = db.PROVEEDORs.Where(m => m.ID == prov).Select(m => new ProveedorVal { ID = m.ID.ToString(), NOMBRE = m.NOMBRE.ToString() }).FirstOrDefault();

            JsonResult cc = Json(provv, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //[HttpPost]
        //public JsonResult getPresupuesto(string kunnr)
        //{
        //    PRESUPUESTO_MOD pm = new PRESUPUESTO_MOD();
        //    try
        //    {
        //        if (kunnr == null)
        //            kunnr = "";

        //        //Obtener presupuesto
        //        Calendario445 c445 = new Calendario445();
        //        string mes = c445.getPeriodo(DateTime.Now.Date) + "";
        //        var presupuesto = db.CSP_PRESU_CLIENT(cLIENTE: kunnr, pERIODO: mes).Select(p => new { DESC = p.DESCRIPCION.ToString(), VAL = p.VALOR.ToString() }).ToList();
        //        string clien = db.CLIENTEs.Where(x => x.KUNNR == kunnr).Select(x => x.BANNERG).First();
        //        if (presupuesto != null)
        //        {
        //            if (String.IsNullOrEmpty(clien))
        //            {
        //                pm.P_CANAL = presupuesto[0].VAL;
        //                pm.P_BANNER = presupuesto[1].VAL;
        //                pm.PC_C = (float.Parse(presupuesto[4].VAL) + float.Parse(presupuesto[5].VAL) + float.Parse(presupuesto[6].VAL)).ToString();
        //                pm.PC_A = presupuesto[8].VAL;
        //                pm.PC_P = presupuesto[9].VAL;
        //                pm.PC_T = presupuesto[10].VAL;
        //                pm.CONSU = (float.Parse(presupuesto[1].VAL) - float.Parse(presupuesto[10].VAL)).ToString();
        //            }
        //            else
        //            {
        //                pm.P_CANAL = presupuesto[0].VAL;
        //                pm.P_BANNER = presupuesto[0].VAL;
        //                pm.PC_C = (float.Parse(presupuesto[4].VAL) + float.Parse(presupuesto[5].VAL) + float.Parse(presupuesto[6].VAL)).ToString();
        //                pm.PC_A = presupuesto[8].VAL;
        //                pm.PC_P = presupuesto[9].VAL;
        //                pm.PC_T = presupuesto[10].VAL;
        //                pm.CONSU = (float.Parse(presupuesto[0].VAL) - float.Parse(presupuesto[10].VAL)).ToString();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    JsonResult cc = Json(pm, JsonRequestBehavior.AllowGet);
        //    return cc;
        //}

        [HttpPost]
        [AllowAnonymous]
        public JsonResult getCategoria(string material)
        {
            if (material == null)
                material = "";
            //RSG 07.06.2018---------------------------------------------
            material = new Cadena().completaMaterial(material);
            //RSG 07.06.2018---------------------------------------------

            TruequeEntities db = new TruequeEntities();

            MATERIAL m = db.MATERIALs.Where(mat => mat.ID.Equals(material)).FirstOrDefault();
            //var cat = new CATEGORIAT();
            var cat = (dynamic)null;
            if (m != null && m.MATKL_ID != "")
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

                cat = (from c in db.MATERIALGPs.Where(c => c.ID == m.MATERIALGP_ID && c.ACTIVO == true)
                       select new
                       {
                           CATEGORIA_ID = c.ID.ToString(),
                           DESCRIPCION = c.DESCRIPCION.ToString(),
                           UNICA = c.UNICA

                       })
                        .FirstOrDefault();
            }

            //var catv = cat;
            JsonResult cc = Json(cat, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult getCategoriaDesc(string cate)
        {
            if (cate == null)
                cate = "";

            TruequeEntities db = new TruequeEntities();

            var cat = (dynamic)null;

            try
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

                cat = db.MATERIALGPs.Where(c => c.ID == cate && c.ACTIVO == true) //Cambiar por materialgp //B20180625 MGC 2018.06.28
                            .Join(
                            db.MATERIALGPTs.Where(ct => ct.SPRAS_ID == "EN"),//Cambiar por materialgpt //B20180625 MGC 2018.06.28
                            c => c.ID,
                            ct => ct.MATERIALGP_ID,//B20180625 MGC 2018.06.28
                            (c, ct) => new
                            {
                                SPRAS_ID = ct.SPRAS_ID.ToString(),
                                CATEGORIA_ID = ct.MATERIALGP_ID.ToString(),//B20180625 MGC 2018.06.28
                                TXT50 = ct.TXT50.ToString()
                            })
                        .FirstOrDefault();
            }
            catch (Exception)
            {

            }

            //var catv = cat;
            JsonResult cc = Json(cat, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpPost]
        //public CATEGORIAT getCategoriaS(string material)
        public MATERIALGPT getCategoriaS(string material)
        {
            if (material == null)
                material = "";
            material = new Cadena().completaMaterial(material);
            TruequeEntities db = new TruequeEntities();

            MATERIAL m = db.MATERIALs.Where(mat => mat.ID.Equals(material)).FirstOrDefault();
            //CATEGORIAT cat = new CATEGORIAT();
            MATERIALGPT cat = new MATERIALGPT();//RSG 01.08.2018

            //if (m != null && m.MATKL_ID != "")
            if (m != null && m.MATERIALGP_ID != "")//RSG 01.08.2018
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

                //cat = db.CATEGORIAs.Where(c => c.ID == m.MATKL_ID && c.ACTIVO == true)
                cat = db.MATERIALGPs.Where(c => c.ID == m.MATERIALGP_ID && c.ACTIVO == true)//RSG 01.08.2018
                            .Join(
                            db.MATERIALGPTs.Where(ct => ct.SPRAS_ID == "EN"),
                            c => c.ID,
                            //ct => ct.CATEGORIA_ID,
                            ct => ct.MATERIALGP_ID,//RSG 01.08.2018
                            (c, ct) => ct)
                        .FirstOrDefault();
            }
            return cat;
        }

        [HttpPost]
        public ActionResult getPartialDis(List<TAT001.Models.DOCUMENTOP_MOD> docs)
        {
            DOCUMENTO doc = new DOCUMENTO();

            doc.DOCUMENTOP = docs;
            return PartialView("~/Views/Solicitudes/_PartialDisTr.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialSop(List<DOCUMENTOF> docs)
        {
            DOCUMENTO doc = new DOCUMENTO();

            doc.DOCUMENTOF = docs;
            return PartialView("~/Views/Solicitudes/_PartialSopTr.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialRec(List<DOCUMENTOREC> docs)
        {
            DOCUMENTO doc = new DOCUMENTO();
            foreach (DOCUMENTOREC r in docs)
            {
                r.NUM_DOC = 0;
                r.DOC_REF = 0;
                r.EJERCICIO = 1;
                r.ESTATUS = " ";
                r.MONTO_FIJO = 0;
                r.MONTO_GRS = 0;
                r.MONTO_NET = 0;
                r.PERIODO = 1;
            }

            doc.DOCUMENTOREC = docs;
            return PartialView("~/Views/Solicitudes/_PartialRecTr.cshtml", doc);
        }

        [HttpPost]
        public ActionResult getPartialRan(List<DOCUMENTORAN> docs)
        {
            DOCUMENTO doc = new DOCUMENTO();
            //foreach (DOCUMENTORAN r in docs)
            //{
            //    r.NUM_DOC = 0;
            //    r.POS = 0;
            //    r.LIN = 0;
            //}

            doc.DOCUMENTORAN = docs;
            return PartialView("~/Views/Solicitudes/_PartialRanTr.cshtml", doc);
        }

        [HttpPost]
        public JsonResult getSolicitud(string num, string monto, string tsol_id, string sociedad_id, string[] categorias, string land, bool edit = false)//RSG 07.06.2018---------------------------------------------
        {
            SOLICITUD_MOD sm = new SOLICITUD_MOD();
            FormatosC format = new FormatosC();
            bool reverso = false;
            //Obtener info solicitud
            if (num == null || num == "" || num == "0.00")
            {
                sm.S_NUM = "";

                // Impuesto
                bool esNC = false;
                decimal impuesto = FnCommon.ObtenerImpuesto(db, new DOCUMENTO { NUM_DOC = 0, MONTO_DOC_MD = Convert.ToDecimal(monto), SOCIEDAD_ID = sociedad_id, TSOL_ID = tsol_id, PAIS_ID = land }, ref esNC, categorias);
                if (esNC) { sm.S_IMPA = impuesto.ToString(); }
                else { sm.S_IMPA = "-"; }

                sm.S_TOTAL = (Convert.ToDecimal(monto) + impuesto).ToString();
            }
            else if (edit)
            {
                decimal num_doc = Convert.ToDecimal(num);
                DOCUMENTO D = db.DOCUMENTOes.First(x => x.NUM_DOC == num_doc);
                ObtenerAnalisisSolicitud(D, Convert.ToDecimal(monto), categorias);

                sm.S_MONTOB = ViewBag.montoSol;
                sm.S_MONTOP = ViewBag.montoProv;
                if (D.TSOL.REVERSO)
                {
                    sm.S_MONTOA = format.toNum(ViewBag.montoApli, ",", ".") + format.toNum(ViewBag.montoSol, ",", ".");
                    sm.S_REMA = format.toNum(ViewBag.remanente, ",", ".") - format.toNum(ViewBag.montoSol, ",", ".");
                }
                else
                {
                    sm.S_MONTOA = ViewBag.montoApli;
                    sm.S_REMA = ViewBag.remanente;
                }
                sm.S_IMPA = ViewBag.impuesto;
                sm.S_IMPB = "-";
                sm.S_IMPC = "-";
                sm.S_RET = "-";
                sm.S_TOTAL = ViewBag.montoTotal;
            }
            else
            {
                decimal num_doc = Convert.ToDecimal(num);
                // Impuesto
                bool esNC = false;
                decimal impuesto = FnCommon.ObtenerImpuesto(db, new DOCUMENTO { NUM_DOC = num_doc, MONTO_DOC_MD = Convert.ToDecimal(monto), SOCIEDAD_ID = sociedad_id, TSOL_ID = tsol_id, PAIS_ID = land }, ref esNC, categorias);
                if (esNC) { sm.S_IMPA = impuesto.ToString(); }
                else { sm.S_IMPA = "-"; }

                var rev = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == num_doc && x.ESTATUS_C == null && x.ESTATUS_WF != "B").ToList();
                reverso = db.TSOLs.First(x => x.ID == tsol_id).REVERSO;
                if (rev.Count == 0)
                {
                    //CON UN RELACIONADO 
                    var rev2 = db.DOCUMENTOes.Where(x => x.NUM_DOC == num_doc).FirstOrDefault();
                    decimal montor2 = Convert.ToDecimal(monto);
                    decimal rem2 = (rev2.MONTO_DOC_MD.Value - montor2);

                    sm.S_MONTOB = monto;
                    sm.S_MONTOP = rev2.MONTO_DOC_MD.ToString();
                    sm.S_MONTOA = monto;
                    sm.S_REMA = rem2.ToString();
                    sm.S_IMPB = "-";
                    sm.S_IMPC = "-";
                    sm.S_RET = "-";
                    sm.S_TOTAL = (montor2 + impuesto).ToString();
                }
                else if (rev.Count == 1)
                {
                    //CON DOS RELACIONADOS
                    var rev3 = db.DOCUMENTOes.Where(x => x.NUM_DOC == num_doc).FirstOrDefault();
                    var rev33 = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == num_doc && x.ESTATUS_C == null && x.ESTATUS_WF != "B").First().MONTO_DOC_MD.Value;
                    decimal sumr3Hijos = rev33 + Convert.ToDecimal(monto);
                    decimal rem3 = (rev3.MONTO_DOC_MD.Value - sumr3Hijos);

                    sm.S_MONTOB = monto;
                    sm.S_MONTOP = rev3.MONTO_DOC_MD.ToString();
                    sm.S_MONTOA = sumr3Hijos.ToString();
                    sm.S_REMA = rem3.ToString();
                    sm.S_IMPB = "-";
                    sm.S_IMPC = "-";
                    sm.S_RET = "-";
                    sm.S_TOTAL = (Convert.ToDecimal(monto) + impuesto).ToString();
                }
                else if (rev.Count > 1)
                {
                    var rev4 = db.DOCUMENTOes.Where(x => x.NUM_DOC == num_doc).FirstOrDefault();
                    var rev44 = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == num_doc && x.ESTATUS_C == null && x.ESTATUS_WF != "B").Select(x => x.MONTO_DOC_MD);
                    decimal sum = 0;

                    foreach (var k in rev44)
                    {
                        sum = sum + k.Value;
                    }
                    decimal sumr4Hijos = sum + Convert.ToDecimal(monto);
                    decimal rem4 = (rev4.MONTO_DOC_MD.Value - sumr4Hijos);

                    sm.S_MONTOB = monto;
                    sm.S_MONTOP = rev4.MONTO_DOC_MD.ToString();
                    sm.S_MONTOA = sumr4Hijos.ToString();
                    sm.S_REMA = rem4.ToString();
                    sm.S_IMPB = "-";
                    sm.S_IMPC = "-";
                    sm.S_RET = "-";
                    sm.S_TOTAL = (Convert.ToDecimal(monto) + impuesto).ToString();
                }
                if (sm.S_MONTOA != "-" && reverso)
                {
                    sm.S_MONTOA = "-" + sm.S_MONTOA;
                }


            }


            JsonResult cc = Json(sm, JsonRequestBehavior.AllowGet);
            return cc;
        }

    }
}
