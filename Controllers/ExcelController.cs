using ExcelDataReader;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Services;
using TAT001.Controllers;
using ClosedXML.Excel;
using TAT001.Filters;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class ExcelController : Controller
    {
        private TruequeEntities db = new TruequeEntities();
        // GET: Excel
        public ActionResult Index(int? miSop, int? miMas)
        {
            int pagina = 221; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
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
                //return RedirectToAction("Pais", "Home");
            }
            Session["spras"] = user.SPRAS_ID;

            if (miSop == 1 & miMas == 1)
            {
                Session["indSop"] = 0;
                Session["indMas"] = 0;
                return RedirectToAction("Index2", new { hola = "hola" });
            }
            else
            {
                if (miSop == 1)
                {
                    ViewBag.contenidoSop = 1;
                }
                else if (miSop == 0 | miSop == null)
                {
                    ViewBag.contenidoSop = 0;
                }

                if (miMas == 1)
                {
                    ViewBag.contenidoMas = 1;
                }
                else if (miMas == 0 | miMas == null)
                {
                    ViewBag.contenidoMas = 0;
                }

                return View();
            }
        }
        //jemo inicio
        public FileResult Index3()
        {
            int pagina = 221; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
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
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }
            Session["spras"] = user.SPRAS_ID;
            Models.CargaMasivaModels carga = new Models.CargaMasivaModels();
            carga.GenerarListaCliPro(Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/ListaClienteProveedores.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListaClienteProveedores.xlsx");
        }
        //jemo fin 4/07/2018

        public ActionResult Index2(string hola)
        {
            int pagina = 221; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch { }

            Session["spras"] = user.SPRAS_ID;

            List<string> li = new List<string>();
            HttpPostedFileBase file;
            if (hola != null)
            {
                file = (HttpPostedFileBase)Session["archivoMas"];
            }
            else
            {
                file = null;
            }


            if (file != null)
            {
                //if (Request.Files["file"].ContentLength > 0)
                if (file.ContentLength > 0)
                {
                    //string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string extension = System.IO.Path.GetExtension(file.FileName);
                        IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                        DataSet result = reader.AsDataSet();

                        DataSet dt = new DataSet();
                        DataTable dtt = result.Tables[0].Copy();
                        dt.Tables.Add(dtt);

                        DataSet dt1 = new DataSet();
                        DataTable dtt1 = result.Tables[1].Copy();
                        dt1.Tables.Add(dtt1);

                        DataSet dt3 = new DataSet();
                        DataTable dtt3 = result.Tables[2].Copy();
                        dt3.Tables.Add(dtt3);

                        DataSet dt4 = new DataSet();
                        DataTable dtt4 = result.Tables[3].Copy();
                        dt4.Tables.Add(dtt4);

                        DataSet dt5 = new DataSet();
                        DataTable dtt5 = result.Tables[4].Copy();
                        dt5.Tables.Add(dtt5);

                        Session["ds1"] = dt;//////CABECERA
                        Session["ds2"] = dt1;/////MATERIALES
                        Session["ds3"] = dt3;/////DOCUMENTOS RELACIONADOS
                        Session["ds4"] = dt4;/////ARCHIVOS SOPORTES
                        Session["ds5"] = dt5;/////RELACIONADOS 2
                    }
                }
                return RedirectToAction("Details");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details()
        {
            int pagina = 222; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch { }

            Session["spras"] = user.SPRAS_ID;

            ViewBag.data1 = Session["ds1"];
            ViewBag.data2 = Session["ds2"];
            ViewBag.data3 = Session["ds3"];
            ViewBag.data4 = Session["ds4"];
            ViewBag.data5 = Session["ds5"];
            //DataSet hoja1 = (DataSet)(Session["ds1"]);
            //DataSet hoja2 = (DataSet)(Session["ds2"]);
            //DataSet hoja3 = (DataSet)(Session["ds3"]);
            //DataSet hoja4 = (DataSet)(Session["ds4"]);
            //DataSet hoja5 = (DataSet)(Session["ds5"]);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string NUM_DOC)
        {
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            DataSet dsHoja1 = (DataSet)(Session["ds1"]);
            DataSet dsHoja2 = (DataSet)(Session["ds2"]);
            DataSet dsHoja3 = (DataSet)(Session["ds3"]);
            DataSet dsHoja4 = (DataSet)(Session["ds4"]);
            DataSet dsHoja5 = (DataSet)(Session["ds5"]);
            IEnumerable<HttpPostedFileBase> archivos = (IEnumerable<HttpPostedFileBase>)(Session["archivosSop"]);

            List<DOCUMENTO> listD = new List<DOCUMENTO>();
            //List<string> li = new List<string>();

            //LECTURA DEL CONENIDO DEL EXCEL PARA LA CARGA MASIVA
            int colHoja1 = dsHoja1.Tables[0].Columns.Count;
            int colHoja2 = dsHoja2.Tables[0].Columns.Count;
            int colHoja3 = dsHoja3.Tables[0].Columns.Count;
            int colHoja4 = dsHoja4.Tables[0].Columns.Count;
            int colHoja5 = dsHoja5.Tables[0].Columns.Count;

            //SECCION 1.- LOGICA PARA LA INSERCION DE LA PRIMER HOJA (Las hojas posteriores dependen de la validacion de est)
            for (int i = 1; i < dsHoja1.Tables[0].Rows.Count; i++)
            {
                DOCUMENTO docu = new DOCUMENTO();
                string numC = dsHoja1.Tables[0].Rows[i][0].ToString();
                string a = dsHoja1.Tables[0].Rows[i][1].ToString().Trim();
                if (a.Length > 10) { a = ""; }
                string b = dsHoja1.Tables[0].Rows[i][2].ToString().Trim();
                b = db.TALLs.Where(x => x.DESCRIPCION == b & x.ACTIVO == true).Select(x => x.GALL_ID).FirstOrDefault();
                string bbb = dsHoja1.Tables[0].Rows[i][2].ToString().Trim();
                string c = dsHoja1.Tables[0].Rows[i][3].ToString().Trim();
                if (c.Length > 4) { c = ""; }
                string d = dsHoja1.Tables[0].Rows[i][4].ToString().Trim();
                d = db.PAIS.Where(x => x.LANDX == d).Select(x => x.LAND).FirstOrDefault();
                string est = dsHoja1.Tables[0].Rows[i][5].ToString().Trim();
                if (est.Length > 50) { est = ""; }
                string ciu = dsHoja1.Tables[0].Rows[i][6].ToString().Trim();
                if (ciu.Length > 50) { ciu = ""; }
                var e = db.TSOLs.Where(x => x.ID == a).Select(x => x.ID);
                var f = db.GALLs.Where(x => x.ID == b).Select(x => x.ID);
                var g = db.SOCIEDADs.Where(x => x.BUKRS == c).Select(x => x.BUKRS);
                var h = db.PAIS.Where(x => x.LAND == d).Select(x => x.LAND);
                string con = dsHoja1.Tables[0].Rows[i][7].ToString().Trim();
                if (con.Length > 100) { con = ""; }
                string not = dsHoja1.Tables[0].Rows[i][8].ToString().Trim();
                if (not.Length > 255) { not = ""; }
                string dd = dsHoja1.Tables[0].Rows[i][9].ToString().Trim();
                if (dd.Length > 10) { dd = ""; }
                string pan = dsHoja1.Tables[0].Rows[i][10].ToString().Trim();
                if (pan.Length > 50) { pan = ""; }
                string pae = dsHoja1.Tables[0].Rows[i][11].ToString().Trim();
                if (pae.Length > 255) { pae = ""; }

                string fec1 = dsHoja1.Tables[0].Rows[i][12].ToString().Trim();
                string fec2 = dsHoja1.Tables[0].Rows[i][13].ToString().Trim();
                string mone = dsHoja1.Tables[0].Rows[i][14].ToString().Trim();
                if (mone.Length > 3) { mone = ""; }
                var ee = db.CLIENTEs.Where(x => x.KUNNR == dd);
                var monee = db.MONEDAs.Where(x => x.WAERS == mone).Select(x => x.WAERS);

                var per = from P in db.PAIS
                          join C in db.CREADOR2 on P.LAND equals C.LAND
                          where P.ACTIVO == true
                          & C.ID == u & C.ACTIVO == true
                          select P;

                var perm = per.Where(x => x.SOCIEDAD_ID == c & x.LAND == d).FirstOrDefault();

                if (e.Count() > 0 && f.Count() > 0 && g.Count() > 0 && h.Count() > 0 & ee.Count() > 0 & monee.Count() > 0)
                {
                    if (perm != null)
                    {
                        numC = numC.TrimStart('0').Trim();
                        if (IsNumeric(numC))
                        {
                            fec1 = validaFechI(fec1);
                            fec2 = validaFechF(fec2);

                            docu.NUM_DOC = Convert.ToDecimal(numC);
                            docu.TSOL_ID = a;
                            docu.GALL_ID = b;
                            docu.SOCIEDAD_ID = c;
                            docu.PAIS_ID = d;
                            docu.ESTADO = est;
                            docu.CIUDAD = ciu;
                            docu.PERIODO = Convert.ToInt32(System.DateTime.Now.Month);
                            docu.EJERCICIO = Convert.ToString(System.DateTime.Now.Year);
                            docu.CANTIDAD_EV = 1;
                            docu.USUARIOC_ID = user.ID;
                            docu.PUESTO_ID = user.PUESTO_ID;
                            docu.FECHAD = System.DateTime.Today;
                            docu.FECHAC = System.DateTime.Today;
                            docu.HORAC = System.DateTime.Now.TimeOfDay;
                            docu.FECHAC_PLAN = System.DateTime.Today;
                            docu.FECHAC_USER = System.DateTime.Today;
                            docu.HORAC_USER = System.DateTime.Now.TimeOfDay;
                            docu.ESTATUS = "N";
                            docu.ESTATUS_WF = "P";
                            docu.CONCEPTO = con;
                            docu.NOTAS = not;
                            docu.VKORG = db.CLIENTEs.Where(x => x.KUNNR == dd).Select(x => x.VKORG).First();
                            docu.VTWEG = db.CLIENTEs.Where(x => x.KUNNR == dd).Select(x => x.VTWEG).First();
                            docu.SPART = db.CLIENTEs.Where(x => x.KUNNR == dd).Select(x => x.SPART).First();
                            docu.PAYER_ID = dd;
                            docu.PAYER_NOMBRE = pan;
                            docu.PAYER_EMAIL = pae;
                            docu.FECHAI_VIG = Convert.ToDateTime(fec1);
                            docu.FECHAF_VIG = Convert.ToDateTime(fec2);
                            docu.MONEDA_ID = mone;
                            docu.MONTO_DOC_MD = 0;
                            docu.TALL_ID = db.TALLs.Where(x => x.DESCRIPCION == bbb).FirstOrDefault().ID;
                            listD.Add(docu);
                        }
                    }
                    DOCUMENTO dop = listD.Where(x => x.NUM_DOC == docu.NUM_DOC).FirstOrDefault();
                    //FIN DE LA SECCION 1

                    //SI LOS DATOS DE CABECERA FUERON CORRECTOS PASA LA CONDICION
                    if (dop != null)
                    {
                        int ind1 = 0;

                        //SECCION 2.- VALIDACION DE DATOS DE LA SEGUNDA HOJA DEL EXCEL (DISTRIBUCION)
                        for (int k = 1; k < dsHoja2.Tables[0].Rows.Count; k++)
                        {
                            string numM = dsHoja2.Tables[0].Rows[k][0].ToString().Trim();
                            numM = numM.TrimStart('0').Trim();
                            if (IsNumeric(numM))
                            {
                                decimal num = Convert.ToDecimal(numM);
                                if (num == docu.NUM_DOC)
                                {
                                    DOCUMENTOP docup = new DOCUMENTOP();
                                    int j = k;

                                    string feD = dsHoja2.Tables[0].Rows[k][1].ToString().Trim();
                                    string feA = dsHoja2.Tables[0].Rows[k][2].ToString().Trim();
                                    string mat = dsHoja2.Tables[0].Rows[k][3].ToString().Trim();
                                    if (mat.Length > 18) { mat = ""; }
                                    string mat2 = dsHoja2.Tables[0].Rows[k][4].ToString().Trim();
                                    if (mat2.Length > 9) { mat2 = ""; }
                                    string mon = dsHoja2.Tables[0].Rows[k][5].ToString().Trim();
                                    string porA = dsHoja2.Tables[0].Rows[k][6].ToString().Trim();
                                    string preS = dsHoja2.Tables[0].Rows[k][7].ToString().Trim();
                                    string volR = dsHoja2.Tables[0].Rows[k][8].ToString().Trim();
                                    string apo = dsHoja2.Tables[0].Rows[k][9].ToString().Trim();
                                    apo = apo.TrimStart('0').Trim();
                                    mon = mon.TrimStart('0').Trim();
                                    porA = porA.TrimStart('0').Trim();
                                    volR = volR.TrimStart('0').Trim();

                                    if (docu.DOCUMENTOPs.Count == 0)
                                    {
                                        if ((mat != null | mat != "") & (mat2 != null | mat2 != ""))
                                        {
                                            ind1 = 1;
                                        }
                                        else if ((mat != null | mat != "") & (mat2 == null | mat2 == ""))
                                        {
                                            ind1 = 1;
                                        }
                                        else if ((mat == null | mat == "") & (mat2 != null | mat2 != ""))
                                        {
                                            ind1 = 0;
                                        }
                                    }

                                    if (ind1 == 1)
                                    {
                                        DateTime fechD = Convert.ToDateTime(feD);
                                        DateTime fechA = Convert.ToDateTime(feA);
                                        var mate = db.MATERIALs.Where(x => x.ID == mat);

                                        if (mate.Count() > 0 && fechA > fechD)
                                        {
                                            //AQUI VALIDAMOS SI TIENE REAL 
                                            //DE NO TENERLO HACE EL CALCULO CON COSTO, PORCENTAJE, PRECIO Y VOLUMEN
                                            if (apo != "")
                                            {
                                                if (IsNumeric(apo) == true)
                                                {
                                                    docup.NUM_DOC = num;
                                                    docup.POS = Convert.ToInt32(dop.DOCUMENTOPs.Count() + 1);
                                                    docup.VIGENCIA_DE = fechD;
                                                    docup.VIGENCIA_AL = fechA;
                                                    docup.MATNR = mat;
                                                    docup.MATKL = "";

                                                    if (db.TSOLs.Where(ar => ar.ID == dop.TSOL_ID).FirstOrDefault().FACTURA == true)
                                                    {
                                                        docup.APOYO_REAL = Convert.ToDecimal(apo);
                                                        dop.MONTO_DOC_MD += docup.APOYO_REAL;
                                                        docup.VOLUMEN_REAL = 0;
                                                    }
                                                    else
                                                    {
                                                        docup.APOYO_EST = Convert.ToDecimal(apo);
                                                        dop.MONTO_DOC_MD += docup.APOYO_EST;
                                                        docup.VOLUMEN_EST = 0;
                                                    }

                                                    docup.CANTIDAD = 0;
                                                    dop.DOCUMENTOPs.Add(docup);
                                                }
                                            }
                                            else
                                            {
                                                if (IsNumeric(mon) & IsNumeric(porA) & IsNumeric(volR))
                                                {
                                                    docup.NUM_DOC = num;
                                                    docup.POS = Convert.ToInt32(dop.DOCUMENTOPs.Count() + 1);
                                                    docup.VIGENCIA_DE = fechD;
                                                    docup.VIGENCIA_AL = fechA;
                                                    docup.MATNR = mat;
                                                    docup.MATKL = "";
                                                    docup.MONTO = Convert.ToDecimal(mon);
                                                    docup.PORC_APOYO = Convert.ToDecimal(porA);
                                                    docup.PRECIO_SUG = Convert.ToDecimal(preS);
                                                    docup.VOLUMEN_REAL = Convert.ToDecimal(volR);

                                                    if (docup.VOLUMEN_REAL == null)
                                                        docup.VOLUMEN_REAL = 0;
                                                    decimal hola = (Convert.ToDecimal(mon) * Convert.ToDecimal(porA));
                                                    hola = (Convert.ToDecimal(mon) - hola) * (Convert.ToDecimal(volR));

                                                    if (db.TSOLs.Where(ar => ar.ID == dop.TSOL_ID).FirstOrDefault().FACTURA == true)
                                                    {
                                                        docup.APOYO_REAL = hola;
                                                        dop.MONTO_DOC_MD += docup.APOYO_REAL;
                                                    }
                                                    else
                                                    {
                                                        docup.APOYO_EST = hola;
                                                        docup.VOLUMEN_EST = (decimal)docup.VOLUMEN_REAL;
                                                        docup.VOLUMEN_REAL = 0;
                                                        dop.MONTO_DOC_MD += docup.APOYO_EST;
                                                    }
                                                    docup.CANTIDAD = 0;
                                                    dop.DOCUMENTOPs.Add(docup);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DateTime fechD = Convert.ToDateTime(feD);
                                        DateTime fechA = Convert.ToDateTime(feA);
                                        var mate = db.MATERIALs.Where(x => x.ID == mat);

                                        if (mate.Count() > 0 && fechA > fechD)
                                        {
                                            //AQUI VALIDAMOS SI TIENE REAL 
                                            //DE NO TENERLO HACE EL CALCULO CON COSTO, PORCENTAJE, PRECIO Y VOLUMEN
                                            if (apo != "")
                                            {
                                                if (IsNumeric(apo) == true)
                                                {
                                                    docup.NUM_DOC = num;
                                                    docup.POS = Convert.ToInt32(dop.DOCUMENTOPs.Count() + 1);
                                                    docup.VIGENCIA_DE = fechD;
                                                    docup.VIGENCIA_AL = fechA;
                                                    docup.MATNR = "";
                                                    string desMat = mat2;
                                                    desMat = db.MATERIALGPs.Where(x => x.DESCRIPCION == desMat).Select(x => x.ID).FirstOrDefault();
                                                    docup.MATKL = desMat;

                                                    if (db.TSOLs.Where(ar => ar.ID == dop.TSOL_ID).FirstOrDefault().FACTURA == true)
                                                    {
                                                        docup.APOYO_REAL = Convert.ToDecimal(apo);
                                                        dop.MONTO_DOC_MD += docup.APOYO_REAL;
                                                        docup.VOLUMEN_REAL = 0;
                                                    }
                                                    else
                                                    {
                                                        docup.APOYO_EST = Convert.ToDecimal(apo);
                                                        dop.MONTO_DOC_MD += docup.APOYO_EST;
                                                        docup.VOLUMEN_REAL = 0;
                                                    }

                                                    docup.CANTIDAD = 0;
                                                    dop.DOCUMENTOPs.Add(docup);
                                                }
                                            }
                                            else
                                            {
                                                if (IsNumeric(mon) & IsNumeric(porA) & IsNumeric(volR))
                                                {
                                                    docup.NUM_DOC = num;
                                                    docup.POS = Convert.ToInt32(dop.DOCUMENTOPs.Count() + 1);
                                                    docup.VIGENCIA_DE = fechD;
                                                    docup.VIGENCIA_AL = fechA;
                                                    docup.MATNR = "";
                                                    string desMat = mat2;
                                                    desMat = db.MATERIALGPs.Where(x => x.DESCRIPCION == desMat).Select(x => x.ID).FirstOrDefault();
                                                    docup.MATKL = desMat;
                                                    docup.MONTO = Convert.ToDecimal(mon);
                                                    docup.PORC_APOYO = Convert.ToDecimal(porA);
                                                    docup.PRECIO_SUG = Convert.ToDecimal(preS);
                                                    docup.VOLUMEN_REAL = Convert.ToDecimal(volR);
                                                    decimal hola = (Convert.ToDecimal(mon) * Convert.ToDecimal(porA));
                                                    hola = (Convert.ToDecimal(mon) - hola) * (Convert.ToDecimal(volR));


                                                    if (db.TSOLs.Where(ar => ar.ID == dop.TSOL_ID).FirstOrDefault().FACTURA == true)
                                                    {
                                                        docup.APOYO_REAL = hola;
                                                        dop.MONTO_DOC_MD += docup.APOYO_REAL;
                                                    }
                                                    else
                                                    {
                                                        docup.APOYO_EST = hola;
                                                        docup.VOLUMEN_EST = (decimal)docup.VOLUMEN_REAL;
                                                        docup.VOLUMEN_REAL = null;
                                                        dop.MONTO_DOC_MD += docup.APOYO_EST;
                                                    }
                                                    docup.CANTIDAD = 0;
                                                    dop.DOCUMENTOPs.Add(docup);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //var sumM1 = dop.DOCUMENTOPs.Where(x => x.NUM_DOC == docu.NUM_DOC).Select(x => x.APOYO_REAL).Sum();
                        //if (sumM1 == 0 | sumM1 == null)
                        //{
                        //    dop.DOCUMENTOPs.Clear();
                        //}
                        //FIN DE LA SECCION 2


                        //SECCION 3.- VALIDACION DE DATOS DE LA TERCER HOJA DEL EXCEL (RELACIONADA)
                        for (int m = 1; m < dsHoja3.Tables[0].Rows.Count; m++)
                        {
                            string numR = dsHoja3.Tables[0].Rows[m][0].ToString().Trim();
                            numR = numR.TrimStart('0').Trim();
                            if (IsNumeric(numR))
                            {
                                string ind = "";
                                decimal num = Convert.ToDecimal(numR);
                                if (num == docu.NUM_DOC)
                                {
                                    DOCUMENTOF docupF = new DOCUMENTOF();
                                    int j = m;

                                    var exist = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == c & x.PAIS_ID == d & x.TSOL == a).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        docupF.NUM_DOC = num;
                                        docupF.POS = m;

                                        if (exist.FACTURA == true)
                                        {
                                            string fac = dsHoja3.Tables[0].Rows[m][1].ToString().Trim();
                                            if (fac == null | fac == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (fac.Length > 50) { docupF.FACTURA = "0"; }
                                                else { docupF.FACTURA = fac; }
                                            }
                                        }
                                        else { docupF.FACTURA = "0"; }

                                        if (exist.FECHA == true)
                                        {
                                            string fech = dsHoja3.Tables[0].Rows[m][2].ToString().Trim();
                                            if (fech == null | fech == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                docupF.FECHA = Convert.ToDateTime(fech);
                                            }
                                        }
                                        else { docupF.FECHA = null; }

                                        if (exist.PROVEEDOR == true)
                                        {
                                            string prov = dsHoja3.Tables[0].Rows[m][3].ToString().Trim();
                                            if (prov == null | prov == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (prov.Length > 10) { docupF.PROVEEDOR = "0"; }
                                                else { docupF.PROVEEDOR = prov; }
                                            }
                                        }
                                        else { docupF.PROVEEDOR = "0"; }

                                        if (exist.CONTROL == true)
                                        {
                                            string cont = dsHoja3.Tables[0].Rows[m][4].ToString().Trim();
                                            if (cont == null | cont == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (cont.Length > 50) { docupF.CONTROL = "0"; }
                                                else { docupF.CONTROL = cont; }
                                            }
                                        }
                                        else { docupF.CONTROL = "0"; }

                                        if (exist.AUTORIZACION == true)
                                        {
                                            string aut = dsHoja3.Tables[0].Rows[m][5].ToString().Trim();
                                            if (aut == null | aut == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (aut.Length > 50) { docupF.AUTORIZACION = "0"; }
                                                else { docupF.AUTORIZACION = aut; }
                                            }
                                        }
                                        else { docupF.AUTORIZACION = "0"; }

                                        if (exist.VENCIMIENTO == true)
                                        {
                                            string venc = dsHoja3.Tables[0].Rows[m][6].ToString().Trim();
                                            if (venc == null | venc == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                docupF.VENCIMIENTO = Convert.ToDateTime(venc);
                                            }
                                        }
                                        else { docupF.VENCIMIENTO = null; }

                                        if (exist.FACTURAK == true)
                                        {
                                            string facK = dsHoja3.Tables[0].Rows[m][7].ToString().Trim();
                                            if (facK == null | facK == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (facK.Length > 50) { docupF.FACTURAK = "0"; }
                                                else { docupF.FACTURAK = facK; }
                                            }
                                        }
                                        else { docupF.FACTURAK = "0"; }

                                        if (exist.EJERCICIOK == true)
                                        {
                                            string ejeK = dsHoja3.Tables[0].Rows[m][8].ToString().Trim();
                                            if (ejeK == null | ejeK == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (ejeK.Length > 4) { docupF.EJERCICIOK = "0"; }
                                                else { docupF.EJERCICIOK = ejeK; }
                                            }
                                        }
                                        else { docupF.EJERCICIOK = "0"; }

                                        if (exist.BILL_DOC == true)
                                        {
                                            string bilD = dsHoja3.Tables[0].Rows[m][9].ToString().Trim();
                                            if (bilD == null | bilD == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (bilD.Length > 10) { docupF.BILL_DOC = "0"; }
                                                else { docupF.BILL_DOC = bilD; }
                                            }
                                        }
                                        else { docupF.BILL_DOC = "0"; }

                                        if (exist.BELNR == true)
                                        {
                                            string beln = dsHoja3.Tables[0].Rows[m][10].ToString().Trim();
                                            if (beln == null | beln == "")
                                            {
                                                ind = "no";
                                            }
                                            else
                                            {
                                                if (beln.Length > 10) { docupF.BELNR = "0"; }
                                                else { docupF.BELNR = beln; }
                                            }
                                        }
                                        else { docupF.BELNR = "0"; }

                                        if (ind != "no")
                                        {
                                            if (dop.DOCUMENTOFs.Count() < 1)
                                            {
                                                dop.DOCUMENTOFs.Add(docupF);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //FIN DE LA SECCION TRES

                        ////SECCION 4.-VALIDAMOS LOS ARCHIVOS RECIBIDOS DE SOPORTE
                        if (archivos.Count() > 0)
                        {
                            //VALIDACION DE DATOS DE LA CUARTA HOJA DEL EXCEL (DISTRIBUCION)
                            dop.DOCUMENTOAs = subeArchivo(archivos, u, a, dsHoja4, docu.NUM_DOC);
                        }
                        //FIN DE LA SECCION 4


                        //SECCION 5.- VALIDACION DE DATOS DE LA QUINTA HOJA DE EXCEL (RELACIONADA MULTIPLE)
                        var numDocM = dop.DOCUMENTOFs.Select(x => x.NUM_DOC).FirstOrDefault();
                        if (numDocM == 0)
                        {
                            if (dop.DOCUMENTOPs.Count() != 0)
                            {
                                for (int m = 1; m < dsHoja5.Tables[0].Rows.Count; m++)
                                {
                                    DOCUMENTOF docupF = new DOCUMENTOF();
                                    string numDo = dsHoja5.Tables[0].Rows[m][0].ToString().Trim();
                                    string fac = dsHoja5.Tables[0].Rows[m][1].ToString().Trim();
                                    string bil = dsHoja5.Tables[0].Rows[m][2].ToString().Trim();
                                    string año = dsHoja5.Tables[0].Rows[m][3].ToString().Trim();
                                    string pay = dsHoja5.Tables[0].Rows[m][4].ToString().Trim();
                                    string imp = dsHoja5.Tables[0].Rows[m][5].ToString().Trim();
                                    string fol = dsHoja5.Tables[0].Rows[m][6].ToString().Trim();
                                    string tsolM = db.TSOLs.Where(x => x.ID == a).Select(x => x.TSOLM).FirstOrDefault();
                                    string ind = "";

                                    numDo = numDo.TrimStart('0').Trim();
                                    if (IsNumeric(numDo))
                                    {
                                        if (numDo == docu.NUM_DOC.ToString())
                                        {
                                            var exist = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == c & x.PAIS_ID == d & x.TSOL == tsolM).FirstOrDefault();
                                            if (exist != null)
                                            {
                                                docupF.NUM_DOC = Convert.ToDecimal(numDo);
                                                docupF.POS = m;

                                                if (exist.FACTURA == true)
                                                {
                                                    if (fac != null | fac != "")
                                                    {
                                                        if (fac.Length > 50) { docupF.FACTURA = "0"; }
                                                        else { docupF.FACTURA = fac; }
                                                    }
                                                    else
                                                    {
                                                        ind = "no";
                                                    }
                                                }
                                                else { docupF.FACTURA = "0"; }

                                                docupF.FECHA = null;
                                                docupF.PROVEEDOR = "0";
                                                docupF.CONTROL = "0";
                                                docupF.AUTORIZACION = "0";
                                                docupF.VENCIMIENTO = null;
                                                docupF.FACTURAK = "0";

                                                if (exist.EJERCICIOK == true)
                                                {
                                                    if (año != null | año != "")
                                                    {
                                                        if (año.Length > 4) { docupF.EJERCICIOK = "0"; }
                                                        else { docupF.EJERCICIOK = año; }
                                                    }
                                                    else
                                                    {
                                                        ind = "no";
                                                    }
                                                }
                                                else { docupF.EJERCICIOK = "0"; }

                                                if (exist.BILL_DOC == true)
                                                {
                                                    if (bil != null | bil != "")
                                                    {
                                                        if (bil.Length > 10) { docupF.BILL_DOC = "0"; }
                                                        else { docupF.BILL_DOC = bil; }
                                                    }
                                                    else
                                                    {
                                                        ind = "no";
                                                    }
                                                }
                                                else { docupF.BILL_DOC = "0"; }

                                                if (exist.BELNR == true)
                                                {
                                                    if (fol != null | fol != "")
                                                    {
                                                        if (fol.Length > 10) { docupF.BELNR = "0"; }
                                                        else { docupF.BELNR = fol; }
                                                    }
                                                    else
                                                    {
                                                        ind = "no";
                                                    }
                                                }
                                                else { docupF.BELNR = "0"; }

                                                if (exist.IMPORTE_FAC == true)
                                                {
                                                    if (imp != null | imp != "")
                                                    {
                                                        if (imp.Length > 13) { docupF.IMPORTE_FAC = 0; }
                                                        else { docupF.IMPORTE_FAC = Convert.ToDecimal(imp); }
                                                    }
                                                    else
                                                    {
                                                        ind = "no";
                                                    }
                                                }
                                                else { docupF.IMPORTE_FAC = 0; }

                                                if (exist.PAYER == true)
                                                {
                                                    if (pay != null | pay != "")
                                                    {
                                                        if (pay.Length > 10) { docupF.PAYER = "0"; }
                                                        else { docupF.PAYER = pay; }
                                                    }
                                                    else
                                                    {
                                                        ind = "no";
                                                    }
                                                }
                                                else { docupF.PAYER = "0"; }

                                                if (ind != "no")
                                                {
                                                    dop.DOCUMENTOFs.Add(docupF);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (dop.DOCUMENTOPs.Count() != 0 & dop.DOCUMENTOFs.Count() != 0)
                            {
                                var sumM = dop.DOCUMENTOPs.Where(x => x.NUM_DOC == docu.NUM_DOC).Select(x => x.APOYO_REAL).Sum();
                                var sumR2 = dop.DOCUMENTOFs.Where(x => x.NUM_DOC == docu.NUM_DOC).Select(x => x.IMPORTE_FAC).Sum();

                                if (sumR2 != 0 & sumR2 != sumM)
                                {
                                    dop.DOCUMENTOFs.Clear();
                                }
                            }
                        }
                        //FIN DE LA SECCION 5
                    }
                    //else { li.Add("<" + numC); }
                }
                //else { li.Add("<" + numC); }
            }


            List<string> li = new List<string>();
            foreach (DOCUMENTO doc in listD)
            {
                TruequeEntities db1 = new TruequeEntities();
                if (doc.DOCUMENTOPs.Count() != 0 & doc.DOCUMENTOFs.Count() != 0)
                {
                    decimal N_DOC = getSolID(doc.TSOL_ID);
                    doc.NUM_DOC = N_DOC;
                    db1.DOCUMENTOes.Add(doc);

                    try
                    {
                        db1.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }

                    updateRango(doc.TSOL_ID, doc.NUM_DOC);
                    guardaArchivos(N_DOC);//ALMACENAMOS LOS ARCHIVOS DE SOPORTE
                    li.Add(doc.NUM_DOC.ToString());
                    ProcesaFlujo pf = new ProcesaFlujo();

                    try
                    {
                        WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(doc.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                        if (wf != null)
                        {
                            WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                            FLUJO f = new FLUJO();
                            f.WORKF_ID = wf.ID;
                            f.WF_VERSION = wf.VERSION;
                            f.WF_POS = wp.POS;
                            f.NUM_DOC = doc.NUM_DOC;
                            f.POS = 1;
                            f.LOOP = 1;
                            f.USUARIOA_ID = doc.USUARIOC_ID;
                            f.ESTATUS = "I";
                            f.FECHAC = DateTime.Now;
                            f.FECHAM = DateTime.Now;
                            string c = pf.procesa(f, "");
                            if (c == "1")
                            {
                                string image = Server.MapPath("~/images/logo_kellogg.png");
                                Email em = new Email();
                                string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                                //em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                            }
                        }
                    }
                    catch (Exception ee) { }
                }
                else
                {
                    li.Add("<" + doc.NUM_DOC.ToString());
                }
            }
            TempData["docs_masiva"] = li;
            return RedirectToAction("Index", "Home");
        }


        public string validaFechI(string date)
        {
            Calendario445 cale = new Calendario445();
            DateTime fecha;
            int anio, mes;

            if (date.Length == 7)
            {

                mes = Convert.ToInt32(date.Substring(0, 2));
                anio = Convert.ToInt32(date.Substring(3, 4));

                fecha = cale.getPrimerDia(anio, mes);
            }
            else
            {
                fecha = Convert.ToDateTime(date);
            }

            return Convert.ToString(fecha);
        }

        public string validaFechF(string date)
        {
            Calendario445 cale = new Calendario445();
            DateTime fecha;
            int anio, mes;

            if (date.Length == 7)
            {
                mes = Convert.ToInt32(date.Substring(0, 2));
                anio = Convert.ToInt32(date.Substring(3, 4));

                fecha = cale.getUltimoDia(anio, mes);
            }
            else
            {
                fecha = Convert.ToDateTime(date);
            }

            return Convert.ToString(fecha);
        }

        public ActionResult Archivo()//METODO PARA DESCARGAR EL ARCHIVO DEL SERVER
        {
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=LAYOUT CARGA MASIVA.xlsx");
            Response.TransmitFile(Server.MapPath("~/files/masiva.xlsx"));
            Response.End();

            return RedirectToAction("Index");
        }

        public ActionResult ArchivoA(HttpPostedFileBase file)//METODO PARA DESCARGAR EL ARCHIVO DEL SERVER
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    string fileName = System.IO.Path.GetFileName(file.FileName.Substring(0, 6));

                    if ((fileExtension == ".xls" || fileExtension == ".xlsx") & fileName == "masiva")
                    {
                        string ruta = Server.MapPath("~/files/masiva.xlsx");
                        System.IO.File.Delete(ruta);

                        var fileName2 = Path.GetFileName(file.FileName);
                        file.SaveAs(Server.MapPath("~/files/" + file.FileName));
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public decimal getSolID(string TSOL_ID)
        {

            decimal id = 0;

            RANGO rango = getRango(TSOL_ID);

            if (rango.ACTUAL > rango.INICIO && rango.ACTUAL < rango.FIN)
            {
                rango.ACTUAL++;
                id = (decimal)rango.ACTUAL;
            }

            return id;
        }

        public void updateRango(string TSOL_ID, decimal actual)
        {
            TruequeEntities db2 = new TruequeEntities();
            RANGO rango = getRango(TSOL_ID);

            if (rango.ACTUAL > rango.INICIO && rango.ACTUAL < rango.FIN)
            {
                rango.ACTUAL = actual;
            }

            db2.Entry(rango).State = EntityState.Modified;
            db2.SaveChanges();
            db2.Dispose();
        }

        public RANGO getRango(string TSOL_ID)
        {
            RANGO rango = new RANGO();
            using (TruequeEntities db = new TruequeEntities())
            {

                rango = (from r in db.RANGOes
                         join s in db.TSOLs
                         on r.ID equals s.RANGO_ID
                         where s.ID == TSOL_ID && r.ACTIVO == true
                         select r).FirstOrDefault();
            }
            return rango;
        }
        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        public List<DOCUMENTOA> subeArchivo(IEnumerable<HttpPostedFileBase> files_soporte, string u, string tso, DataSet ds4, decimal numDo)
        {
            List<HttpPostedFileBase> archiArr = new List<HttpPostedFileBase>();
            List<DOCUMENTOA> docupA = new List<DOCUMENTOA>();
            int numFiles = 0;

            //REVISAR SI HAY ARCHIVOS POR SUBIR
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
            catch (Exception e) { }

            for (int jj = 1; jj < ds4.Tables[0].Rows.Count; jj++)
            {
                string num = ds4.Tables[0].Rows[jj][0].ToString().Trim();
                string descrip = ds4.Tables[0].Rows[jj][1].ToString().Trim();
                string ruta = ds4.Tables[0].Rows[jj][2].ToString().Trim();

                num = num.TrimStart('0').Trim();
                if (IsNumeric(num))
                {
                    if (num == numDo.ToString())
                    {
                        if (numFiles > 0)
                        {
                            foreach (HttpPostedFileBase file in files_soporte)
                            {
                                var clasefile = "";
                                try
                                {
                                    clasefile = descrip;
                                }
                                catch (Exception ex)
                                {
                                    clasefile = "";
                                }

                                if (file != null)
                                {
                                    if (file.ContentLength > 0)
                                    {
                                        string nombreV = file.FileName.ToUpper();

                                        if (nombreV == ruta.ToUpper())
                                        {
                                            string miDes = clasefile.ToUpper().Substring(0, 3);
                                            //VERIFICAMOS EL TIPO DE SOPORTE
                                            if (miDes == "FAC")
                                            {
                                                var exist = docupA.Where(x => x.NUM_DOC == Convert.ToDecimal(num) & x.CLASE == miDes).FirstOrDefault();

                                                //SI YA EXISTE UN TIPO DE SOPORTE FACTURA NO INSERTAMOS UNO NUEVO
                                                if (exist == null)
                                                {
                                                    string path = "";
                                                    DOCUMENTOA doc = new DOCUMENTOA();
                                                    doc.NUM_DOC = Convert.ToInt32(num);
                                                    doc.POS = jj;
                                                    doc.TIPO = Path.GetExtension(nombreV).Replace(".", "");
                                                    try
                                                    {
                                                        var clasefileM = clasefile.ToUpper();
                                                        doc.CLASE = clasefileM.Substring(0, 3);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        doc.CLASE = "";
                                                    }

                                                    doc.STEP_WF = 1;
                                                    doc.USUARIO_ID = u;
                                                    doc.PATH = nombreV;
                                                    doc.ACTIVO = true;

                                                    docupA.Add(doc);
                                                    archiArr.Add(file);
                                                }
                                            }
                                            else
                                            {
                                                string path = "";
                                                DOCUMENTOA doc = new DOCUMENTOA();
                                                doc.NUM_DOC = Convert.ToInt32(num);
                                                doc.POS = jj;
                                                doc.TIPO = Path.GetExtension(nombreV).Replace(".", "");
                                                try
                                                {
                                                    var clasefileM = clasefile.ToUpper();
                                                    doc.CLASE = clasefileM.Substring(0, 3);
                                                }
                                                catch (Exception e)
                                                {
                                                    doc.CLASE = "";
                                                }

                                                doc.STEP_WF = 1;
                                                doc.USUARIO_ID = u;
                                                doc.PATH = nombreV;
                                                doc.ACTIVO = true;

                                                docupA.Add(doc);
                                                archiArr.Add(file);
                                            }
                                        }
                                    }
                                }
                            }//LLAVE FOREACH
                        }//IF NUMERO DE ARCHUVOS MAYOR A 0
                    }
                }
            }//LLAVE DE FOR PARA LOS REGISTROS DEL EXCEL
            List<HttpPostedFileBase> archivosRENew = (List<HttpPostedFileBase>)Session["arcReales"];

            if (archivosRENew == null)
            {
                archivosRENew = new List<HttpPostedFileBase>();
            }
            archivosRENew.AddRange(archiArr);
            Session["arcReales"] = archivosRENew;
            return docupA;
        }

        public void guardaArchivos(decimal numDoc)
        {
            IEnumerable<HttpPostedFileBase> archivosRE = (IEnumerable<HttpPostedFileBase>)Session["arcReales"];
            Files sc = new Files();
            string errorString = "";
            //Guardar los documentos cargados en la sección de soporte
            var res = "";
            string errorMessage = "";
            int numFiles = 0;
            //Checar si hay archivos para subir
            try
            {
                foreach (HttpPostedFileBase file in archivosRE)
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
            catch (Exception e) { }

            if (numFiles > 0)
            {
                //Obtener las variables con los datos de sesión y ruta
                string url = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") && x.ACTIVO).FirstOrDefault().VALUE;
                //Crear el directorio
                string nomNum = numDoc.ToString();
                var dir = sc.createDir(url, nomNum, DateTime.Now.Year.ToString());

                //Evaluar que se creo el directorio
                if (dir.Equals(""))
                {
                    foreach (HttpPostedFileBase file in archivosRE)
                    {
                        string errorfiles = "";
                        string path = "";
                        errorfiles = "";
                        res = sc.SaveFile(file, url, nomNum, out errorfiles, out path, DateTime.Now.Year.ToString());
                        var cambio = db.DOCUMENTOAs.Where(x => x.NUM_DOC == numDoc & x.PATH == file.FileName).FirstOrDefault();
                        if (cambio != null)
                        {
                            cambio.PATH = path;
                            db.SaveChanges();
                        }

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
                Session["ERROR_FILES"] = errorMessage;
            }
        }

        [HttpPost]
        public ActionResult Soportes(HttpPostedFileBase[] file2)
        {
            int sop;

            if (file2[0] != null)
            {
                Session["archivosSop"] = file2;
                ViewBag.contenidoSop = 1;
                sop = 1;
            }
            else
            {
                ViewBag.contenidoSop = 0;
                sop = 0;
            }

            int? masSop = (int?)Session["indMas"];

            if (masSop == null)
            {
                masSop = 0;
            }

            Session["indSop"] = sop;
            return RedirectToAction("Index", new { miSop = sop, miMas = masSop });
        }

        [HttpPost]
        public ActionResult Masiva(HttpPostedFileBase file)
        {
            int mas;

            if (file != null)
            {
                Session["archivoMas"] = file;
                ViewBag.contenidoMas = 1;
                mas = 1;
            }
            else
            {
                ViewBag.contenidoMas = 0;
                mas = 0;
            }

            int? sopMas = (int?)Session["indSop"];

            if (sopMas == null)
            {
                sopMas = 0;
            }

            Session["indMas"] = mas;
            return RedirectToAction("Index", new { miSop = sopMas, miMas = mas });
        }

        [HttpGet]
        public JsonResult Doc(string NUM_DOC)
        {
            List<DOCUMENTOP> ldp = new List<DOCUMENTOP>();
            DataSet tab = (DataSet)(Session["ds2"]);
            int ind1 = 0;

            if (tab.Tables[0].Columns.Count == 10)
            {
                for (var i = 1; i < tab.Tables[0].Rows.Count; i++)
                {
                    if (tab.Tables[0].Rows[i][0].ToString() == NUM_DOC)
                    {
                        DOCUMENTOP docup = new DOCUMENTOP();
                        int j = i;
                        docup.NUM_DOC = Convert.ToInt32(tab.Tables[0].Rows[i][0].ToString().Trim());
                        docup.POS = 0;
                        string mat = tab.Tables[0].Rows[i][3].ToString().Trim();
                        string mat2 = tab.Tables[0].Rows[i][4].ToString().Trim();

                        if ((mat != null | mat != "") & (mat2 != null | mat2 != ""))
                        {
                            ind1 = 1;
                        }
                        else if ((mat != null | mat != "") & (mat2 == null | mat2 == ""))
                        {
                            ind1 = 1;
                        }
                        else if ((mat == null | mat == "") & (mat2 != null | mat2 != ""))
                        {
                            ind1 = 0;
                        }

                        if (ind1 == 1)
                        {
                            //mat = mat.TrimStart('0').Trim();
                            if (IsNumeric(mat))
                            {
                                var e = db.MATERIALs.Where(x => x.ID == mat).Select(x => x.ID);
                                if (e.Count() > 0)
                                {
                                    docup.MATNR = "<td>" + mat + "</td>";
                                    docup.MATKL = "<td></td>";
                                }
                                else
                                {
                                    docup.MATNR = "<td class='red white-text'>" + mat + "</td>";
                                    docup.MATKL = "<td></td>";
                                }
                            }
                        }
                        else
                        {
                            var a = db.MATERIALs.Where(x => x.MATKL_ID == mat2).Select(x => x.MATKL_ID);
                            if (a.Count() > 0)
                            {
                                docup.MATNR = "<td></td>";
                                docup.MATKL = "<td>" + mat2 + "</td>";
                            }
                            else
                            {
                                docup.MATNR = "<td></td>";
                                docup.MATKL = "<td class='red white-text'>" + mat2 + "</td>";
                            }
                        }

                        //if (mat == "")
                        //{ docup.MATNR = "<td class='red white-text'>" + mat + "</td>"; }
                        //else
                        //{
                        //    if (IsNumeric(mat))
                        //    {
                        //        var e = db.MATERIALs.Where(x => x.ID == mat).Select(x => x.ID);
                        //        if (e.Count() > 0)
                        //        {
                        //            docup.MATNR = "<td>" + mat + "</td>";
                        //        }
                        //        else
                        //        {
                        //            docup.MATNR = "<td class='red white-text'>" + mat + "</td>";
                        //            ldp2.Add(i);
                        //        }
                        //    }
                        //}


                        //if (mat2 == "")
                        //{ docup.MATKL = "<td class='red white-text'>" + mat2 + "</td>"; }
                        //else
                        //{
                        //    var a = db.MATERIALs.Where(x => x.MATKL_ID == mat2).Select(x => x.MATKL_ID);
                        //    if (a.Count() > 0)
                        //    {
                        //        docup.MATKL = "<td>" + mat2 + "</td>";
                        //    }
                        //    else
                        //    {
                        //        docup.MATKL = "<td class='red white-text'>" + mat2 + "</td>";
                        //    }
                        //}

                        string mon = tab.Tables[0].Rows[i][5].ToString().Trim();
                        if (mon == "") { docup.MONTO = 0.00M; }
                        else { docup.MONTO = Convert.ToDecimal(mon); }

                        string por = tab.Tables[0].Rows[i][6].ToString().Trim();
                        if (por == "") { docup.PORC_APOYO = 0.00M; }
                        else { docup.PORC_APOYO = Convert.ToDecimal(por); }

                        string apo = tab.Tables[0].Rows[i][9].ToString().Trim();
                        if (apo == "") { docup.APOYO_EST = 0.00M; }
                        else { docup.APOYO_EST = Convert.ToDecimal(apo); }

                        string pre = tab.Tables[0].Rows[i][7].ToString().Trim();
                        if (pre == "") { docup.PRECIO_SUG = 0.00M; }
                        else { docup.PRECIO_SUG = Convert.ToDecimal(pre); }

                        string vol = tab.Tables[0].Rows[i][8].ToString().Trim();
                        if (vol == "") { docup.VOLUMEN_EST = 0.00M; }
                        else { docup.VOLUMEN_EST = Convert.ToDecimal(vol); }

                        docup.VIGENCIA_DE = Convert.ToDateTime(tab.Tables[0].Rows[i][1].ToString().Trim());
                        docup.VIGENCIA_AL = Convert.ToDateTime(tab.Tables[0].Rows[i][2].ToString().Trim());
                        ldp.Add(docup);
                    }
                }
            }
            else
            {
                ldp = null;
            }

            JsonResult jr = Json(ldp, JsonRequestBehavior.AllowGet);
            return jr;
        }

        [HttpGet]
        public JsonResult Hoja3(string NUM_DOC, string a, string c, string d)//a = TIPO SOLICITUD, c = SOCIEDAD, d = PAIS
        {
            List<object> ldf = new List<object>();
            List<string> ldf2 = new List<string>();
            List<string> ldf3 = new List<string>();
            List<object> ldf4 = new List<object>();
            //List<DOCUMENTOF> ldf = new List<DOCUMENTOF>();
            DataSet tab = (DataSet)(Session["ds3"]);
            d = db.PAIS.Where(x => x.LANDX == d).Select(x => x.LAND).FirstOrDefault();
            string cabeza = "";

            if (tab.Tables[0].Columns.Count == 11)
            {
                for (int m = 1; m < tab.Tables[0].Rows.Count; m++)
                {
                    string ind = "";
                    string numDo = tab.Tables[0].Rows[m][0].ToString().Trim();
                    if (numDo == NUM_DOC)
                    {
                        DOCUMENTOF docupF = new DOCUMENTOF();

                        var exist = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == c & x.PAIS_ID == d & x.TSOL == a).FirstOrDefault();
                        if (exist != null)
                        {
                            docupF.NUM_DOC = Convert.ToInt32(tab.Tables[0].Rows[m][0].ToString().Trim());
                            docupF.POS = 0;

                            if (exist.FACTURA == true)
                            {
                                if (tab.Tables[0].Rows[m][1].ToString() == null | tab.Tables[0].Rows[m][1].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("1");
                                }
                                else
                                {
                                    docupF.FACTURA = tab.Tables[0].Rows[m][1].ToString().Trim();
                                }
                            }
                            else { docupF.FACTURA = "0"; }

                            if (exist.FECHA == true)
                            {
                                if (tab.Tables[0].Rows[m][2].ToString() == null | tab.Tables[0].Rows[m][2].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("2");
                                }
                                else
                                {
                                    docupF.FECHA = Convert.ToDateTime(tab.Tables[0].Rows[m][2].ToString().Trim());
                                }
                            }
                            else { docupF.FECHA = null; }

                            if (exist.PROVEEDOR == true)
                            {
                                if (tab.Tables[0].Rows[m][3].ToString() == null | tab.Tables[0].Rows[m][3].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("3");
                                }
                                else
                                {
                                    docupF.PROVEEDOR = tab.Tables[0].Rows[m][3].ToString().Trim();
                                }
                            }
                            else { docupF.PROVEEDOR = "0"; }

                            if (exist.CONTROL == true)
                            {
                                if (tab.Tables[0].Rows[m][4].ToString() == null | tab.Tables[0].Rows[m][4].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("4");
                                }
                                else
                                {
                                    docupF.CONTROL = tab.Tables[0].Rows[m][4].ToString().Trim();
                                }
                            }
                            else { docupF.CONTROL = "0"; }

                            if (exist.AUTORIZACION == true)
                            {
                                if (tab.Tables[0].Rows[m][5].ToString() == null | tab.Tables[0].Rows[m][5].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("5");
                                }
                                else
                                {
                                    docupF.AUTORIZACION = tab.Tables[0].Rows[m][5].ToString().Trim();
                                }
                            }
                            else { docupF.AUTORIZACION = "0"; }

                            if (exist.VENCIMIENTO == true)
                            {
                                if (tab.Tables[0].Rows[m][6].ToString() == null | tab.Tables[0].Rows[m][6].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("6");
                                }
                                else
                                {
                                    docupF.VENCIMIENTO = Convert.ToDateTime(tab.Tables[0].Rows[m][6].ToString().Trim());
                                }
                            }
                            else { docupF.VENCIMIENTO = null; }

                            if (exist.FACTURAK == true)
                            {
                                if (tab.Tables[0].Rows[m][7].ToString() == null | tab.Tables[0].Rows[m][7].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("7");
                                }
                                else
                                {
                                    docupF.FACTURAK = tab.Tables[0].Rows[m][7].ToString().Trim();
                                }
                            }
                            else { docupF.FACTURAK = "0"; }

                            if (exist.EJERCICIOK == true)
                            {
                                if (tab.Tables[0].Rows[m][8].ToString() == null | tab.Tables[0].Rows[m][8].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("8");
                                }
                                else
                                {
                                    docupF.EJERCICIOK = tab.Tables[0].Rows[m][8].ToString().Trim();
                                }
                            }
                            else { docupF.EJERCICIOK = "0"; }

                            if (exist.BILL_DOC == true)
                            {
                                if (tab.Tables[0].Rows[m][9].ToString() == null | tab.Tables[0].Rows[m][9].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("9");
                                }
                                else
                                {
                                    docupF.BILL_DOC = tab.Tables[0].Rows[m][9].ToString().Trim();
                                }
                            }
                            else { docupF.BILL_DOC = "0"; }

                            if (exist.BELNR == true)
                            {
                                if (tab.Tables[0].Rows[m][10].ToString() == null | tab.Tables[0].Rows[m][10].ToString() == "")
                                {
                                    ind = "no";
                                    ldf3.Add("10");
                                }
                                else
                                {
                                    docupF.BELNR = tab.Tables[0].Rows[m][10].ToString().Trim();
                                }
                            }
                            else { docupF.BELNR = "0"; }

                            if (ind != "no")
                            {
                                cabeza = "<tr class='green white-text'>";
                            }
                            else
                            {
                                cabeza = "<tr class='red white-text'>";
                            }


                            ldf.Add(docupF);
                            ldf2.Add(cabeza);
                            //ldf.Add(docupF);
                        }
                    }
                }
                ldf.Add(ldf2);
            }
            else
            {
                ldf = null;
            }

            JsonResult jr = Json(ldf, JsonRequestBehavior.AllowGet);
            return jr;
        }

        [HttpGet]
        public JsonResult Hoja4(string NUM_DOC)
        {
            List<object> lda = new List<object>();
            List<object> lda3 = new List<object>();
            DataSet tab = (DataSet)(Session["ds4"]);
            IEnumerable<HttpPostedFileBase> archivos = (IEnumerable<HttpPostedFileBase>)(Session["archivosSop"]);
            string ind1 = "", ind2 = "";

            if (tab.Tables[0].Columns.Count == 3)
            {
                for (int m = 1; m < tab.Tables[0].Rows.Count; m++)
                {
                    DOCUMENTOA docupA = new DOCUMENTOA();
                    List<object> lda2 = new List<object>();
                    string numDo = tab.Tables[0].Rows[m][0].ToString().Trim();
                    string nom = tab.Tables[0].Rows[m][1].ToString().Trim();
                    string rut = tab.Tables[0].Rows[m][2].ToString().Trim();

                    numDo = numDo.TrimStart('0').Trim();
                    if (IsNumeric(numDo))
                    {
                        if (numDo == NUM_DOC)
                        {
                            docupA.NUM_DOC = Convert.ToDecimal(numDo);

                            var tsop = db.TSOPORTEs.Where(x => x.DESCRIPCION == nom & x.ACTIVO == true).FirstOrDefault();
                            if (tsop != null)
                            {
                                docupA.USUARIO_ID = (nom);
                                ind1 = "";
                            }
                            else
                            {
                                docupA.USUARIO_ID = ("<" + nom);
                                ind1 = "r";
                            }

                            foreach (HttpPostedFileBase file in archivos)
                            {
                                if (file != null)
                                {
                                    if (file.ContentLength > 0)
                                    {
                                        string nombreV = file.FileName.ToUpper();
                                        if (nombreV == rut.ToUpper())
                                        {
                                            docupA.PATH = rut;
                                            ind2 = "";
                                            break;
                                        }
                                        else
                                        {
                                            docupA.PATH = ("<" + rut);
                                            ind2 = "r";
                                        }
                                    }
                                }
                            }

                            lda.Add(docupA);
                            lda2.Add(ind1);
                            lda2.Add(ind2);
                            lda3.Add(lda2);
                        }
                    }
                }
                //lda.Add(ind1);
                //lda.Add(ind2);
                lda.Add(lda3);
            }
            else
            {
                lda = null;
            }
            JsonResult jr = Json(lda, JsonRequestBehavior.AllowGet);
            return jr;
        }

        [HttpGet]
        public JsonResult Hoja5(string NUM_DOC)
        {
            List<object> ldf = new List<object>();
            List<object> listaTextosPayer = new List<object>();
            List<object> validacionExt1 = new List<object>();
            DataSet tabRel = (DataSet)(Session["ds3"]);
            DataSet tab = (DataSet)(Session["ds5"]);
            List<DOCUMENTOF> listD = new List<DOCUMENTOF>();
            DOCUMENTOF docupFRel = new DOCUMENTOF();

            //DOCUMENTO dop = listD.Where(x => x.NUM_DOC == docu.NUM_DOC).FirstOrDefault();

            for (int i = 1; i < tabRel.Tables[0].Rows.Count; i++)
            {
                string numDo = tabRel.Tables[0].Rows[i][0].ToString().Trim();

                if (numDo == NUM_DOC)
                {
                    docupFRel.NUM_DOC = Convert.ToDecimal(numDo);
                    docupFRel.POS = 0;
                    docupFRel.FACTURA = "0";
                    docupFRel.FECHA = null;
                    docupFRel.PROVEEDOR = "0";
                    docupFRel.CONTROL = "0";
                    docupFRel.AUTORIZACION = "0";
                    docupFRel.VENCIMIENTO = null;
                    docupFRel.FACTURAK = "0";
                    docupFRel.EJERCICIOK = "0";
                    docupFRel.BILL_DOC = "0";
                    docupFRel.BELNR = "0";

                    listD.Add(docupFRel);
                }
            }



            string jaja = "";
            var numDocM = listD.Where(x => x.NUM_DOC == Convert.ToDecimal(NUM_DOC)).FirstOrDefault();
            if (numDocM != null)
            {
                jaja = "jaja";
            }
            else
            {
                jaja = "";
            }

            if (tab.Tables[0].Columns.Count == 7)
            {
                for (int m = 1; m < tab.Tables[0].Rows.Count; m++)
                {
                    DOCUMENTOF docupF = new DOCUMENTOF();
                    List<object> textoPayer = new List<object>();
                    List<object> validacionInt1 = new List<object>();
                    string numDo = tab.Tables[0].Rows[m][0].ToString().Trim();
                    string fac = tab.Tables[0].Rows[m][1].ToString().Trim();
                    string bil = tab.Tables[0].Rows[m][2].ToString().Trim();
                    string año = tab.Tables[0].Rows[m][3].ToString().Trim();
                    string pay = tab.Tables[0].Rows[m][4].ToString().Trim();
                    string imp = tab.Tables[0].Rows[m][5].ToString().Trim();
                    string fol = tab.Tables[0].Rows[m][6].ToString().Trim();
                    string ind = "";

                    string cus = db.CLIENTEs.Where(x => x.KUNNR == pay).Select(x => x.NAME1).FirstOrDefault();

                    numDo = numDo.TrimStart('0').Trim();
                    if (IsNumeric(numDo))
                    {
                        if (numDo == NUM_DOC)
                        {
                            docupF.NUM_DOC = Convert.ToDecimal(numDo);
                            docupF.FACTURA = fac;
                            if (fac.Length > 50)
                            { validacionInt1.Add("red"); ind = ":V"; }
                            else { validacionInt1.Add("gre"); }

                            docupF.BILL_DOC = bil;
                            if (bil.Length > 10)
                            { validacionInt1.Add("red"); ind = ":V"; }
                            else { validacionInt1.Add("gre"); }

                            docupF.EJERCICIOK = año;
                            if (año.Length > 4)
                            { validacionInt1.Add("red"); ind = ":V"; }
                            else { validacionInt1.Add("gre"); }

                            docupF.PAYER = pay;
                            if (pay.Length > 10)
                            { validacionInt1.Add("red"); ind = ":V"; }
                            else { validacionInt1.Add("gre"); }

                            textoPayer.Add(cus);
                            docupF.IMPORTE_FAC = Convert.ToDecimal(imp);
                            if (imp.Length > 13)
                            { validacionInt1.Add("red"); ind = ":V"; }
                            else { validacionInt1.Add("gre"); }

                            docupF.BELNR = fol;
                            if (fol.Length > 10)
                            { validacionInt1.Add("red"); ind = ":V"; }
                            else { validacionInt1.Add("gre"); }

                            validacionInt1.Add(ind);

                            ldf.Add(docupF);
                            listaTextosPayer.Add(textoPayer);
                            validacionExt1.Add(validacionInt1);
                        }
                    }
                }
                ldf.Add(listaTextosPayer);
                ldf.Add(validacionExt1);
                ldf.Add(jaja);
            }
            else
            {
                ldf = null;
            }


            JsonResult jr = Json(ldf, JsonRequestBehavior.AllowGet);
            return jr;
        }
    }
}