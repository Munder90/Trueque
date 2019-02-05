using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Xml;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Services;
using TAT001.Controllers;
using ClosedXML.Excel;
using ExcelDataReader;
using TAT001.Models.Masiva;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Web.Security;
using TAT001.Common;
using TAT001.Models.Dao;
using TAT001.Filters;

namespace TAT001.Controllers
{
    [Authorize]//ADD RSG 29.10.2018
    public class MasivaController : Controller
    {
        private TruequeEntities db = new TruequeEntities();
        private FormatosC fc = new FormatosC();
        private Calendario445 cal445 = new Calendario445();
        private TCambio tcambio = new TCambio();
        private Cadena cad = new Cadena();
        readonly UsuarioLogin usuValidateLogin = new UsuarioLogin();

        //------------------DAO------------------------------
        readonly TallsDao tallsDao = new TallsDao();
        readonly ClientesDao clientesDao = new ClientesDao();
        readonly MaterialesDao materialesDao = new MaterialesDao();
        readonly MaterialesgptDao materialesgptDao = new MaterialesgptDao();

        // GET: Masiva

        [LoginActive]
        public ActionResult Index(string mensaje)
        {
            int pagina = 221; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            try
            {

                string usuariotextos = "";
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                usuariotextos = user.SPRAS_ID;
                Warning w = new Warning();
                ViewBag.listaValid = w.listaW(null, usuariotextos);
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";

            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }
            ViewBag.Mensaje = mensaje;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(string a)
        //{
        //    return (null);
        //}

        public ActionResult Archivo(string varArch)//METODO PARA DESCARGAR EL ARCHIVO DEL SERVER
        {
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            string msj = "";
            if (varArch == "X")
            {
                string p = "";
                try
                {
                    p = Session["pais"].ToString();
                    LayoutCargaMasivaViewModels layout = new LayoutCargaMasivaViewModels();
                    layout.layoutMasiva = db.LAYOUT_CARGA.Where(x => x.LAND == p).FirstOrDefault();
                    if (layout.layoutMasiva !=null)
                    {
                        try
                        {
                            var path = Path.Combine(Server.MapPath("~/Archivos/LayoutCargaMasiva/"));
                            string[] nombre = layout.layoutMasiva.RUTA.Split(new[] { "LayoutCargaMasiva" }, StringSplitOptions.None);
                            if (nombre.Length > 1)
                            {
                                path = path + nombre[1];

                                Response.AddHeader("Content-Disposition", "attachment; filename = LAYOUT_CARGA_MASIVA.xlsx");
                                Response.TransmitFile(path);
                                Response.End();
                            }
                        }catch(Exception e)
                        {
                            msj = "Archivo no encontrado";
                        }
                    }
                    else
                    {
                        msj = "El país no tiene asociado un layout de carga masiva";
                    }
                }
                catch
                {
                    msj = "Favor de seleccionar un país.";
                }
            }
            else if (varArch == "L")
            {
                Models.CargaMasivaModels carga = new Models.CargaMasivaModels();
                carga.GenerarListaCliPro(Server.MapPath("~/pdfTemp/"));
                Response.AddHeader("Content-Disposition", "attachment; filename = LISTA_CLIENTE_PROVEEDORES.xlsx");
                Response.TransmitFile(Server.MapPath("~/pdfTemp/ListaClienteProveedores.xlsx"));
                Response.End();
            }
           
            return RedirectToAction("Index",new { mensaje = msj});
        }

        public ActionResult loadExcelMasiva()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];

                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        string fileExtension = System.IO.Path.GetExtension(file.FileName);
                        string extension = System.IO.Path.GetExtension(file.FileName);
                        
                        IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                        DataSet result = reader.AsDataSet();

                        //////CABECERA
                        DataSet dt = new DataSet();
                        DataTable dtt = result.Tables[0].Copy();
                        dt.Tables.Add(dtt);

                        /////DOCUMENTOS RELACIONADOS
                        DataSet dt2 = new DataSet();
                        DataTable dtt2 = result.Tables[1].Copy();
                        dt2.Tables.Add(dtt2);

                        /////DOCUMENTOS RELACIONADOS MULTIPLES
                        DataSet dt3 = new DataSet();
                        DataTable dtt3 = result.Tables[2].Copy();
                        dt3.Tables.Add(dtt3);

                        /////MATERIALES
                        DataSet dt4 = new DataSet();
                        DataTable dtt4 = result.Tables[3].Copy();
                        dt4.Tables.Add(dtt4);

                        Session["ds1"] = dt;//////CABECERA
                        Session["ds2"] = dt2;/////DOCUMENTOS RELACIONADOS
                        Session["ds3"] = dt3;/////DOCUMENTOS RELACIONADOS MULTIPLES
                        Session["ds4"] = dt4;/////MATERIALES
                        reader.Close();
                    }
                }
            }
            return (null);
        }

        public JsonResult validaHoja1()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            DataSet ds1 = (DataSet)Session["ds1"];
            List<object> lp = new List<object>();
            List<object> validacion = new List<object>();
            if (ds1 != null)
            {
                var numColumnas = ds1.Tables[0].Columns.Count;

                List<PAI> pp = (from P in db.PAIS.ToList()//ADD RSG 01.11.2018--------------------------------------------------
                                join C in db.CLIENTEs.Where(x => x.ACTIVO).ToList()
                                on P.LAND equals C.LAND
                                join U in db.USUARIOFs.Where(x => x.USUARIO_ID == User.Identity.Name && x.ACTIVO == true)
                                on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                                where P.ACTIVO
                                select P).DistinctBy(x => x.LAND).ToList();

                if (numColumnas == 16)
                {
                    for (int i = 2; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        Encabezado doc = new Encabezado();

                        string num_doc = ds1.Tables[0].Rows[i][0].ToString().Trim();

                        if (getNumberDocument(num_doc) == -1)
                            continue;
                       
                        string t_sol = ds1.Tables[0].Rows[i][1].ToString().Trim();
                        string gall_id = ds1.Tables[0].Rows[i][2].ToString().Trim();
                        string bukrs = ds1.Tables[0].Rows[i][3].ToString().Trim();
                        string land = ds1.Tables[0].Rows[i][4].ToString().Trim();
                        string estado = ds1.Tables[0].Rows[i][5].ToString().Trim();
                        string ciudad = ds1.Tables[0].Rows[i][6].ToString().Trim();
                        string concepto = ds1.Tables[0].Rows[i][7].ToString().Trim();
                        string notas = ds1.Tables[0].Rows[i][8].ToString().Trim();
                        string payer_id = ds1.Tables[0].Rows[i][9].ToString().Trim();
                        if (payer_id.Length < 10) { payer_id = cad.completaCliente(payer_id); }
                        string payer_nombre = ds1.Tables[0].Rows[i][10].ToString().Trim();
                        string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

                        string vkorg = "", vtweg = "", spart = "";
                        string Paisdecimales = ".";
                        string Paismiles = ","; 
                        if (db.CLIENTEs.Where(x => x.KUNNR == payer_id).Count() > 0)
                        {
                            CLIENTE clienteH1 = db.CLIENTEs.Where(x => x.KUNNR == payer_id).FirstOrDefault();
                            vkorg = clienteH1.VKORG;
                            vtweg = clienteH1.VTWEG;
                            spart = clienteH1.SPART;
                        }
                        else
                        {
                            vkorg = "";
                            vtweg = "";
                            spart = "";
                        }

                        string contacto_nombre = ds1.Tables[0].Rows[i][11].ToString().Trim();
                        string contacto_email = ds1.Tables[0].Rows[i][12].ToString().Trim();
                        string fechai_vig = ds1.Tables[0].Rows[i][13].ToString().Trim();
                        string[] fechai_vig2 = fechai_vig.Split(' ');
                        string fechaf_vig = ds1.Tables[0].Rows[i][14].ToString().Trim();
                        string[] fechaf_vig2 = fechaf_vig.Split(' ');
                        string moneda_id = ds1.Tables[0].Rows[i][15].ToString().Trim();

                        doc.NUM_DOC = num_doc;
                        doc.TSOL_ID = t_sol;
                        doc.TALL_NAME = gall_id;
                        doc.SOCIEDAD_ID = bukrs;
                        doc.PAIS_NAME = land;

                        PAI p = pp.FirstOrDefault(a => a.LANDX == land);
                        string pLand = null;
                        if (p != null)
                        {
                            doc.PAIS_ID = p.LAND;//ADD RSG 01.11.2018--------------------------------------------------
                            pLand = p.LAND;
                            Paisdecimales = p.DECIMAL;
                            Paismiles=p.MILES;
                        }
                        List<TALLT> list = tallsDao.ListaTallsConCuenta(TATConstantes.ACCION_LISTA_TALLTCONCUENTA, null, spras_id, pLand, DateTime.Now.Year, bukrs);
                        if (list.Any(x => x.TXT50 == gall_id))
                        {
                            doc.TALL_ID = list.Where(x => x.TXT50 == gall_id).FirstOrDefault().TALL_ID;
                        }
                        doc.ESTADO = estado;
                        doc.CIUDAD = ciudad;
                        doc.CONCEPTO = concepto;
                        doc.NOTAS = notas;
                        doc.PAYER_ID = payer_id.TrimStart('0');
                        doc.VKORG = vkorg;
                        doc.VTWEG = vtweg;
                        doc.SPART = spart;
                        doc.Decimales = Paisdecimales;
                        doc.Miles = Paismiles;
                        //var existeCliente = db.CLIENTEs.Where(x => x.KUNNR == payer_id).FirstOrDefault().NAME1;

                        //if (existeCliente != null | existeCliente != "")
                        //{
                        //    doc.PAYER_NOMBRE = existeCliente;
                        //}
                        var existeCliente = db.CLIENTEs.Where(x => x.KUNNR == payer_id).FirstOrDefault();

                        if (existeCliente != null)
                        {
                            doc.PAYER_NOMBRE = existeCliente.NAME1;
                        }
                        else
                        {
                            doc.PAYER_NOMBRE = "";
                        }

                        doc.CONTACTO_NOMBRE = contacto_nombre;
                        doc.CONTACTO_EMAIL = contacto_email;
                        doc.FECHAI_VIG = fechai_vig2[0];
                        doc.FECHAF_VIG = fechaf_vig2[0];
                        doc.MONEDA_ID = moneda_id;

                        lp.Add(doc);
                        validacion.Add(cargaInicialH1(ds1.Tables[0].Rows[i], pp));


                    }
                    if (validacion.Count() == 0)
                    {
                        lp.Add("El contenido de la hoja 1 no fue procesado");
                    }
                    else
                    {
                        lp.Add(validacion);
                    }
                }
                else
                {
                    lp.Add("Error de formato en hoja 1");
                }

            }
            else
            {
                lp.Add("La hoja 1 no se pudo procesar, favor de intentarlo nuevamente");
            }
                JsonResult jl = Json(lp, JsonRequestBehavior.AllowGet);
                return jl;
            
        } 

        public JsonResult validaHoja2()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            DataSet ds2 = (DataSet)Session["ds2"];
            List<object> lp = new List<object>();
            List<object> general = new List<object>();
            List<object> errores = new List<object>();
            List<object> warnings = new List<object>();
            if (ds2 != null)
            {
                var numColumnas = ds2.Tables[0].Columns.Count;

                DataSet ds1 = (DataSet)Session["ds1"];
                List<DOCUMENTO> dd = new List<DOCUMENTO>();

                List<PAI> pp = (from P in db.PAIS.ToList()//ADD RSG 01.11.2018--------------------------------------------------
                                join C in db.CLIENTEs.Where(x => x.ACTIVO).ToList()
                                on P.LAND equals C.LAND
                                join U in db.USUARIOFs.Where(x => x.USUARIO_ID == User.Identity.Name && x.ACTIVO == true)
                                on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                                where P.ACTIVO
                                select P).DistinctBy(x => x.LAND).ToList();

                for (int i = 2; i < ds1.Tables[0].Rows.Count; i++)
                {
                    string num_doc = ds1.Tables[0].Rows[i][0].ToString().Trim();
                    string tsol = ds1.Tables[0].Rows[i][1].ToString().Trim();
                    string bukrs = ds1.Tables[0].Rows[i][3].ToString().Trim();
                    string land = ds1.Tables[0].Rows[i][4].ToString().Trim();
                    CLIENTE clienteH1 = null;
                    string payer_id = ds1.Tables[0].Rows[i][9].ToString().Trim();
                    if (payer_id.Length < 10) { payer_id = cad.completaCliente(payer_id); }
                    if (db.CLIENTEs.Where(x => x.KUNNR == payer_id).Count() > 0)
                    {
                         clienteH1= db.CLIENTEs.Where(x => x.KUNNR == payer_id).FirstOrDefault();
                    }

                    PAI p = pp.Where(a => a.LANDX == land).FirstOrDefault();
                    if (getNumberDocument(num_doc) == -1)
                        continue;

                    DOCUMENTO d = new DOCUMENTO();
                    d.NUM_DOC = decimal.Parse(num_doc);
                    d.TSOL_ID = tsol;
                    d.SOCIEDAD_ID = bukrs;
                    d.CLIENTE = clienteH1;
                    if (p != null)
                        d.PAIS_ID = p.LAND;//ADD RSG 01.11.2018--------------------------------------------------
                    dd.Add(d);
                }

                if (numColumnas == 9)
                {
                    foreach (DOCUMENTO d in dd)
                    {
                        List<Relacionada> rel = new List<Relacionada>();
                        List<DataRow> listRows = getRowsByDocument(ds2.Tables[0], d.NUM_DOC.ToString(), 0);
                        foreach (DataRow row in listRows)
                        {
                            string num_doc = row[0].ToString().Trim();
                            if (getNumberDocument(num_doc) == -1)
                                continue;

                            if (d.NUM_DOC == decimal.Parse(num_doc))
                            {
                                Relacionada doc = new Relacionada();

                                string factura = row[1].ToString().Trim();
                                string fecha_factura = row[2].ToString().Trim();
                                string[] fecha_factura2 = fecha_factura.Split(' ');
                                string proveedor = row[3].ToString().Trim();
                                string proveedor_nombre = row[4].ToString().Trim();
                                string autorizacion = row[5].ToString().Trim();
                                string vencimiento = row[6].ToString().Trim();
                                string[] vencimiento2 = vencimiento.Split(' ');
                                string facturak = row[7].ToString().Trim();
                                string ejerciciok = row[8].ToString().Trim();

                                var con = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == d.SOCIEDAD_ID && x.PAIS_ID == d.PAIS_ID && x.TSOL == d.TSOL_ID).FirstOrDefault();

                                if (con != null)
                                {
                                    doc.NUM_DOC = num_doc;

                                    if (con.FACTURA) { doc.FACTURA = factura; }
                                    else { doc.FACTURA = ""; }

                                    if (con.FECHA) { doc.FECHA = fecha_factura2[0]; }
                                    else { doc.FECHA = ""; }

                                    if (con.PROVEEDOR) { doc.PROVEEDOR = proveedor.TrimStart('0'); }
                                    else { doc.PROVEEDOR = ""; }

                                    if (con.PROVEEDOR) { doc.PROVEEDOR_NOMBRE = proveedor_nombre; }
                                    else { doc.PROVEEDOR_NOMBRE = ""; }

                                    if (con.AUTORIZACION) { doc.AUTORIZACION = autorizacion; }
                                    else { doc.AUTORIZACION = ""; }

                                    if (con.VENCIMIENTO) { doc.VENCIMIENTO = vencimiento2[0]; }
                                    else { doc.VENCIMIENTO = ""; }

                                    if (con.FACTURAK) { doc.FACTURAK = facturak; }
                                    else { doc.FACTURAK = ""; }

                                    if (con.EJERCICIOK) { doc.EJERCICIOK = ejerciciok; }
                                    else { doc.EJERCICIOK = ""; }
                                }
                                else
                                {
                                    doc.NUM_DOC = num_doc;
                                    doc.FACTURA = factura;
                                    doc.FECHA = fecha_factura2[0];
                                    doc.PROVEEDOR = proveedor.TrimStart('0');
                                    doc.PROVEEDOR_NOMBRE = proveedor_nombre;
                                    doc.AUTORIZACION = autorizacion;
                                    doc.VENCIMIENTO = vencimiento2[0];
                                    doc.FACTURAK = facturak;
                                    doc.EJERCICIOK = ejerciciok;
                                }

                                rel.Add(doc);
                                lp.Add(doc);
                                errores.Add(cargaInicialH2(row, d.CLIENTE));
                                warnings.Add(cargaInicialH2W(row));
                            }
                        }
                        if (rel.Where(x => x.NUM_DOC == d.NUM_DOC.ToString()).Count() == 0)
                        {
                            FACTURASCONF fcon = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == d.SOCIEDAD_ID && x.PAIS_ID == d.PAIS_ID && x.TSOL == d.TSOL_ID).FirstOrDefault();
                            if (fcon != null)
                            {
                                Relacionada doc = new Relacionada();
                                doc.NUM_DOC = d.NUM_DOC.ToString();
                                doc.FACTURA = "";
                                doc.FECHA = "";
                                doc.PROVEEDOR = "";
                                doc.PROVEEDOR_NOMBRE = "";
                                doc.AUTORIZACION = "";
                                doc.VENCIMIENTO = "";
                                doc.FACTURAK = "";
                                doc.EJERCICIOK = "";
                                //lp.Add(doc);
                                List<string> err = new List<string>();
                                for (int i = 0; i < 9; i++)
                                {
                                    err.Add("red white-text rojo");
                                }
                                List<string> war = new List<string>();
                                for (int i = 0; i < 9; i++)
                                {
                                    war.Add("");
                                }
                                //errores.Add(err);
                                //warnings.Add(war);
                            }
                        }
                    }
                    lp.Add(errores);
                    lp.Add(warnings);
                }
                else
                {
                    lp.Add("Error de formato en hoja 2");
                }
            }
            else
            {
                lp.Add("MSJ:La hoja 2 no se pudo procesar, favor de intentarlo nuevamente");
            }

            JsonResult jl = Json(lp, JsonRequestBehavior.AllowGet);
            return jl;
        }

        public JsonResult validaHoja3()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            DataSet ds1 = (DataSet)Session["ds1"];
            DataSet ds3 = (DataSet)Session["ds3"];
            List<object> lp = new List<object>();
            List<object> validacion = new List<object>();
            if (ds1 != null && ds3 != null)
            {
                var columnas = ds3.Tables[0].Columns.Count;

                if (columnas == 8)
                {
                    for (int i = 2; i < ds3.Tables[0].Rows.Count; i++)
                    {
                        Multiple doc = new Multiple();

                        string num_doc = ds3.Tables[0].Rows[i][0].ToString().Trim();
                        if (getNumberDocument(num_doc) == -1)
                            continue;
                        string factura = ds3.Tables[0].Rows[i][1].ToString().Trim();
                        string bill_doc = ds3.Tables[0].Rows[i][2].ToString().Trim();
                        string ejerciciok = ds3.Tables[0].Rows[i][3].ToString().Trim();
                        string payer_id = ds3.Tables[0].Rows[i][4].ToString().Trim();
                        if (payer_id.Length < 10) { payer_id = cad.completaCliente(payer_id); }
                        string payerSinCero = payer_id.TrimStart('0');
                        string payer_nombre = ds3.Tables[0].Rows[i][5].ToString().Trim();
                        string importe_fac = ds3.Tables[0].Rows[i][6].ToString().Trim();
                        string belnr = ds3.Tables[0].Rows[i][7].ToString().Trim();

                        for (int j = 2; j < ds1.Tables[0].Rows.Count; j++)
                        {
                            string num_docH1 = ds1.Tables[0].Rows[j][0].ToString().Trim();
                            string sociedad = ds1.Tables[0].Rows[j][3].ToString().Trim();
                            string pais = ds1.Tables[0].Rows[j][4].ToString().Trim();

                            string decimales = db.PAIS.Where(x => x.LANDX.Contains(pais) & x.SOCIEDAD_ID == sociedad).FirstOrDefault().DECIMAL;

                            if (num_docH1 == num_doc)
                            {
                                doc.NUM_DOC = num_doc;
                                doc.FACTURA = factura;
                                doc.BILL_DOC = bill_doc;
                                doc.EJERCICIOK = ejerciciok;
                                doc.PAYER = payer_id.TrimStart('0');

                                var existeCliente = db.CLIENTEs.Where(x => x.KUNNR == payer_id).FirstOrDefault();

                                if (existeCliente != null)
                                {
                                    doc.PAYER_NOMBRE = existeCliente.NAME1;
                                }
                                else
                                {
                                    doc.PAYER_NOMBRE = "";
                                }

                                doc.IMPORTE_FAC = fc.toShow(Convert.ToDecimal(importe_fac), decimales);
                                doc.BELNR = belnr;
                            }
                        }

                        lp.Add(doc);
                        validacion.Add(cargaInicialH3(ds3.Tables[0].Rows[i]));
                    }
                    if (validacion.Count() == 0)
                    {
                        lp.Add("El contenido de la hoja 3 no fue procesado");
                    }
                    else
                    {
                        lp.Add(validacion);
                    }
                    
                }
                else
                {
                    lp.Add("Error de formato en hoja 3");
                }

            }
            else
            {
                lp.Add("MSJ:La hoja 3 no se pudo procesar, favor de intentarlo nuevamente");
            }
            JsonResult jl = Json(lp, JsonRequestBehavior.AllowGet);
            return jl;
        }

        public JsonResult validaHoja4()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            DataSet ds1 = (DataSet)Session["ds1"];
            DataSet ds4 = (DataSet)Session["ds4"];
            List<object> ld = new List<object>();
            List<object> validacion = new List<object>();
            if (ds1 != null && ds4 != null)
            {
                var columnas = ds4.Tables[0].Columns.Count;

                if (columnas == 14)
                {
                    for (int j = 2; j < ds1.Tables[0].Rows.Count; j++)
                    {
                        Encabezado objInformacion = new Encabezado();
                        objInformacion.NUM_DOC = ds1.Tables[0].Rows[j][0].ToString().Trim();
                        objInformacion.SOCIEDAD_ID = ds1.Tables[0].Rows[j][3].ToString().Trim();
                        objInformacion.PAIS_NAME = ds1.Tables[0].Rows[j][4].ToString().Trim();
                        objInformacion.FECHAI_VIG = ds1.Tables[0].Rows[j][13].ToString().Trim();
                        string[] vigencia_de2 = objInformacion.FECHAI_VIG.Split(' ');
                        objInformacion.FECHAF_VIG = ds1.Tables[0].Rows[j][14].ToString().Trim();
                        string[] vigencia_al2 = objInformacion.FECHAF_VIG.Split(' ');
                        objInformacion.PAYER_ID = ds1.Tables[0].Rows[j][9].ToString().Trim();
                        if (objInformacion.PAYER_ID.Length < 10) { objInformacion.PAYER_ID = cad.completaCliente(objInformacion.PAYER_ID); }


                        PAI pais_ = db.PAIS.Where(x => x.LANDX.ToUpper() == objInformacion.PAIS_NAME.ToUpper() && x.SOCIEDAD_ID == objInformacion.SOCIEDAD_ID).FirstOrDefault();
                        string decimales = ".";
                        if (pais_ != null)
                        {
                            decimales = pais_.DECIMAL;
                        }
                        string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

                        string vkorg = "", vtweg = "", spart = "";
                        CLIENTE clienteH1 = db.CLIENTEs.Where(x => x.KUNNR == objInformacion.PAYER_ID).FirstOrDefault();
                        if (clienteH1 != null)
                        {
                            vkorg = clienteH1.VKORG;
                            vtweg = clienteH1.VTWEG;
                            spart = clienteH1.SPART;
                        }
                        else
                        {
                            vkorg = "";
                            vtweg = "";
                            spart = "";
                        }

                        objInformacion.VKORG = vkorg;
                        objInformacion.VTWEG = vtweg;
                        objInformacion.SPART = spart;

                        List<MATERIALGPT> listCategorias = materialesgptDao.CategoriasCliente(vkorg, spart, objInformacion.PAYER_ID, objInformacion.SOCIEDAD_ID);
                        List<DataRow> listRows = getRowsByDocument(ds4.Tables[0], objInformacion.NUM_DOC, 0);

                        foreach (DataRow row in listRows)
                        {
                            Distribucion objDistribucion = new Distribucion();

                            string num_doc = row[0].ToString();
                            string ligada = row[1].ToString().Trim();
                            string matnr = row[4].ToString().Trim();
                            if (matnr.Length < 18) { matnr = cad.completaMaterial(matnr); }
                            string materialSinCero = matnr.TrimStart('0');
                            string matkl = row[5].ToString().Trim();
                            string descripcion = row[6].ToString().Trim();
                            string monto = row[7].ToString().Trim();
                            string porc_apoyo = row[8].ToString().Trim();
                            string apoyo_pieza = row[9].ToString().Trim();
                            string costo_apoyo = row[10].ToString().Trim();
                            string precio_sug = row[11].ToString().Trim();
                            string volumen_real = row[12].ToString().Trim();
                            string apoyo = row[13].ToString().Trim();

                            string fechaI = row[2].ToString().Trim();
                            string[] fechaInicio = fechaI.Split(' ');
                            string fechaF = row[3].ToString().Trim();
                            string[] fechaFin = fechaF.Split(' ');

                            if (objInformacion.NUM_DOC == num_doc)
                            {
                                objDistribucion.NUM_DOC = num_doc;
                                objDistribucion.LIGADA = ligada;
                                if (fechaInicio[0] == ""){
                                    objDistribucion.VIGENCIA_DE = vigencia_de2[0];
                                }
                                else{
                                    objDistribucion.VIGENCIA_DE = fechaInicio[0];
                                }
                                if (fechaFin[0] == ""){
                                    objDistribucion.VIGENCIA_AL = vigencia_al2[0];
                                }
                                else{
                                    objDistribucion.VIGENCIA_AL = fechaFin[0];
                                }
                                
                                if (matnr == "" && matkl != "")
                                {
                                    MATERIALGPT categoria = listCategorias.Where(x => x.TXT50.ToUpper() == matkl.ToUpper()).FirstOrDefault();
                                    if (categoria!=null)
                                    {
                                        objDistribucion.MATNR = "";
                                        objDistribucion.MATKL = categoria.TXT50;
                                        objDistribucion.MATKL_ID = categoria.MATERIALGP_ID;
                                        objDistribucion.DESCRIPCION = "";
                                    }
                                    else
                                    {
                                        objDistribucion.MATNR = "";
                                        objDistribucion.MATKL = matkl;
                                        objDistribucion.MATKL_ID = "";
                                        objDistribucion.DESCRIPCION = "";
                                    }
                                }
                                else if ((matnr != "" && matkl != "")||(matnr != "" && matkl == ""))
                                {

                                    List<MATERIAL> ListMateriales = materialesDao.ListaMateriales(matnr, vkorg, vtweg, User.Identity.Name);
                                    MATERIAL material = ListMateriales.Where(x => x.ID == matnr).FirstOrDefault();
                                    if (material!=null)
                                    {
                                        
                                        objDistribucion.MATNR = material.ID.TrimStart('0');
                                        var idMatGp = material.MATERIALGP_ID;
                                        objDistribucion.MATKL = db.MATERIALGPs.Where(x => x.ID == idMatGp).FirstOrDefault().DESCRIPCION;
                                        objDistribucion.MATKL_ID = idMatGp;
                                        objDistribucion.DESCRIPCION = material.MAKTX;
                                    }
                                    else
                                    {
                                        objDistribucion.MATNR = materialSinCero;
                                        objDistribucion.MATKL = "";
                                        objDistribucion.DESCRIPCION = descripcion;
                                    }
                                }
                                if (ligada != "")
                                {
                                    objDistribucion.MONTO = fc.toShow(Convert.ToDecimal(0), decimales);
                                    if (IsNumeric(porc_apoyo))
                                    {
                                        objDistribucion.PORC_APOYO = fc.toShowPorc(Convert.ToDecimal(porc_apoyo) * 100, decimales);
                                    }
                                    else
                                    {
                                        objDistribucion.PORC_APOYO = fc.toShowPorc(Convert.ToDecimal(0), decimales);
                                    }
                                    objDistribucion.APOYO_PIEZA = fc.toShow(Convert.ToDecimal(0), decimales);
                                    objDistribucion.COSTO_APOYO = fc.toShow(Convert.ToDecimal(0), decimales);
                                    objDistribucion.PRECIO_SUG = fc.toShow(Convert.ToDecimal(0), decimales);
                                    objDistribucion.VOLUMEN_REAL = fc.toShowNum(Convert.ToDecimal(0), decimales);
                                    objDistribucion.APOYO = fc.toShow(Convert.ToDecimal(0), decimales);
                                }
                                else
                                {
                                    if (!IsNumeric(monto)) { monto = "0"; }
                                    if (!IsNumeric(porc_apoyo)) { porc_apoyo = "0"; } else { porc_apoyo = (Convert.ToDecimal(porc_apoyo) * 100).ToString(); }
                                    if (!IsNumeric(apoyo_pieza)) { apoyo_pieza = "0"; }
                                    if (!IsNumeric(costo_apoyo)) { costo_apoyo = "0"; }
                                    if (!IsNumeric(precio_sug)) { precio_sug = "0"; }
                                    if (!IsNumeric(volumen_real)) { volumen_real = "0"; }
                                    if (!IsNumeric(apoyo)) { apoyo = "0"; }

                                    if (apoyo != "" && apoyo != "0")
                                    {
                                        if ((monto != "" && monto != "0") && (porc_apoyo != "" && porc_apoyo != "0") && (volumen_real != "" && volumen_real != "0"))
                                        {
                                            objDistribucion.MONTO = fc.toShow(Convert.ToDecimal(monto), decimales);
                                            objDistribucion.PORC_APOYO = fc.toShowPorc(Convert.ToDecimal(porc_apoyo), decimales);
                                            objDistribucion.APOYO_PIEZA = fc.toShow(Convert.ToDecimal(apoyo_pieza), decimales);
                                            objDistribucion.COSTO_APOYO = fc.toShow(Convert.ToDecimal(costo_apoyo), decimales);
                                            objDistribucion.PRECIO_SUG = fc.toShow(Convert.ToDecimal(precio_sug), decimales);
                                            objDistribucion.VOLUMEN_REAL = fc.toShowNum(Convert.ToDecimal(volumen_real), decimales);
                                            objDistribucion.APOYO = fc.toShow(Convert.ToDecimal(apoyo), decimales);
                                        }
                                        else
                                        {
                                            objDistribucion.MONTO = "";
                                            objDistribucion.PORC_APOYO = "";
                                            objDistribucion.APOYO_PIEZA = "";
                                            objDistribucion.COSTO_APOYO = "";
                                            objDistribucion.PRECIO_SUG = "";
                                            objDistribucion.VOLUMEN_REAL = "";
                                            objDistribucion.APOYO = fc.toShow(Convert.ToDecimal(apoyo), decimales);
                                        }

                                    }
                                    else
                                    {
                                        if ((monto != "" && monto != "0") && (porc_apoyo != "" && porc_apoyo != "0") && (volumen_real != "" && volumen_real != "0"))
                                        {
                                            objDistribucion.MONTO = fc.toShow(Convert.ToDecimal(monto), decimales);
                                            objDistribucion.PORC_APOYO = fc.toShowPorc(Convert.ToDecimal(porc_apoyo), decimales);
                                            objDistribucion.APOYO_PIEZA = fc.toShow(Convert.ToDecimal(apoyo_pieza), decimales);
                                            objDistribucion.COSTO_APOYO = fc.toShow(Convert.ToDecimal(costo_apoyo), decimales);
                                            objDistribucion.PRECIO_SUG = fc.toShow(Convert.ToDecimal(precio_sug), decimales);
                                            objDistribucion.VOLUMEN_REAL = fc.toShowNum(Convert.ToDecimal(volumen_real), decimales);

                                            decimal total = (Convert.ToDecimal(monto) * (Convert.ToDecimal(porc_apoyo) / 100));
                                            total = total * Convert.ToDecimal(volumen_real);
                                            objDistribucion.APOYO = fc.toShow(total, decimales);
                                        }
                                        else
                                        {
                                            objDistribucion.MONTO = "";
                                            objDistribucion.PORC_APOYO = "";
                                            objDistribucion.APOYO_PIEZA = "";
                                            objDistribucion.COSTO_APOYO = "";
                                            objDistribucion.PRECIO_SUG = "";
                                            objDistribucion.VOLUMEN_REAL = "";
                                            objDistribucion.APOYO = "";
                                        }
                                    }
                                }

                                ld.Add(objDistribucion);
                                validacion.Add(cargaInicialH4(objInformacion, objDistribucion, listCategorias, listRows));
                            }
                        }
                        
                    }

                    if (validacion.Count() == 0)
                    {
                        ld.Add("El contenido de la hoja 4 no fue procesado");
                    }
                    else
                    {
                        ld.Add(validacion);
                    }
                }
                else
                {
                    ld.Add("Error de formato en hoja 4");
                }

            }
            else
            {
                ld.Add("La hoja 4 no se pudo procesar, favor de intentarlo nuevamente");
            }
            JsonResult jl = Json(ld, JsonRequestBehavior.AllowGet);
            return jl;
        }

        public JsonResult validaHoja5()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            string idioma = FnCommon.ObtenerSprasId(db, User.Identity.Name);
            DataSet ds5 = (DataSet)Session["ds1"];
            List<object> la = new List<object>();
            List<object> tsol = new List<object>();
            List<string> textos = new List<string>();
            if (ds5 != null)
            {
                for (int i = 2; i < ds5.Tables[0].Rows.Count; i++)
                {
                    string num_doc = ds5.Tables[0].Rows[i][0].ToString().Trim();
                    if (getNumberDocument(num_doc) == -1)
                        continue;

                    string t_sol = ds5.Tables[0].Rows[i][1].ToString().Trim();

                    var con = db.TSOPORTEs
                        .Join(db.TSOPORTETs, x => x.ID, y => y.TSOPORTE_ID, (x, y) => new { x, y })
                        .Join(db.CONSOPORTEs, t => t.x.ID, z => z.TSOPORTE_ID, (t, z) => new { t, z })
                        .Where(xy => xy.t.x.ID != "REV" & xy.t.x.ACTIVO == true & xy.t.y.SPRAS_ID == idioma & xy.z.TSOL_ID == t_sol & xy.z.ACTIVO == true)
                        .Select(xyz => new { xyz.t.y.TXT50, xyz.z.OBLIGATORIO }).ToList();

                    //var con2 = db.DOCUMENTOes
                    //        .Join(db.CUENTAs, d => d.TALL_ID, c => c.TALL_ID, (d, c) => new { d, c })
                    //        .Join(db.IIMPUESTOes, x => x.c.IMPUESTO, imp => imp.IMPUESTO, (x, imp) => new { x, imp })

                    foreach (var item in con)
                    {
                        la.Add(num_doc);
                        textos.Add(item.TXT50);
                        tsol.Add(item.OBLIGATORIO);
                    }
                    //tsol.Add(obligatorioSub);
                }

                //var query = db.TSOPORTEs.Where(x => x.ID != "REV" & x.ACTIVO == true);
                la.Add(tsol);
                la.Add(textos);
            }
            else
            {
                la.Add("La hoja 5 no se pudo procesar, favor de intentarlo nuevamente");
            }
            JsonResult jl = Json(la, JsonRequestBehavior.AllowGet);
            return jl;
        }

        //COSULTA DE AJAX PARA EL NUMERO DE DOCUMENTO
        public JsonResult numdoc(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 10)
            {
                if (IsNumeric(Prefix))
                {
                    c = true;
                }
                else
                {
                    c = false;
                }
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        #region JSON PARA LA HOJA 1 Y 3
        //COSULTA DE AJAX PARA EL TIPO DE SOLICITUD T_SOL
        public JsonResult tipoSol(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";

            var c = (from t in db.TSOLs
                     where t.ID.Contains(Prefix)
                     select new { t.ID }).ToList();

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }



        //COSULTA DE AJAX PARA LA SOCIEDAD
        //public JsonResult sociedad(string Prefix)
        public JsonResult sociedad(string Prefix, string user)//ADD RSG 01.11.2018
        {
            if (Prefix == null)
                Prefix = "";

            //----------------BEGIN OF RSG 01.11.2018
            List<PAI> pp = (from P in db.PAIS.ToList()
                            join C in db.CLIENTEs.Where(x => x.ACTIVO == true).ToList()
                            on P.LAND equals C.LAND
                            join U in db.USUARIOFs.Where(x => x.USUARIO_ID == user & x.ACTIVO == true)
                            on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                            where P.ACTIVO == true
                            select P).DistinctBy(x => x.LAND).ToList();
            if (pp != null)
            {
                var c = (from s in db.SOCIEDADs.ToList()
                         join p in pp
                         on s.BUKRS equals p.SOCIEDAD_ID
                         where s.BUKRS.Contains(Prefix.ToUpper()) && s.ACTIVO == true
                         select new { s.BUKRS }).ToList();

                JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
                return cc;
            }
            else
                return null;
            //----------------BEGIN OF RSG 01.11.2018

            ////var c = (from s in db.SOCIEDADs
            ////         where s.BUKRS.Contains(Prefix) && s.ACTIVO == true
            ////         select new { s.BUKRS }).ToList();

            //JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            //return cc;
        }

        //COSULTA DE AJAX PARA EL TIPO DE PAIS
        public JsonResult pais(string Prefix, string Sociedad)
        {
            if (Prefix == null)
                Prefix = "";

            var c = (from p in db.PAIS
                     where p.LANDX.Contains(Prefix) && p.SOCIEDAD_ID == Sociedad && p.ACTIVO == true
                     //select new { p.LANDX }).ToList();
                     select new { p.LAND, p.LANDX }).ToList();

            if (c.Count == 0)
            {
                var c2 = (from p in db.PAIS
                          where p.LAND.Contains(Prefix) && p.SOCIEDAD_ID == Sociedad && p.ACTIVO == true
                          //select new { p.LANDX }).ToList();
                          select new { p.LAND, p.LANDX }).ToList();
                c.AddRange(c2);
            }
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }



        //COSULTA DE AJAX PARA EL TAMAÑO DEL CONCEPTO
        public JsonResult concepto(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 100)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL TAMAÑO DE LAS NOTAS
        public JsonResult notas(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 255)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL TAMAÑO DEL NOMBRE DEL CONTACTO
        public JsonResult contactoNombre(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 50)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL TAMAÑO DEL EMAIL DEL CONTACTO
        public JsonResult contactoEmail(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 255)
            {
                if (validaCorreo(Prefix))
                {
                    c = true;
                }
                else
                {
                    c = false;
                }
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //////COSULTA DE AJAX PARA EL TIPO DE CLIENTE
        ////public JsonResult cliente(string Prefix)
        ////{
        ////    if (Prefix == null)
        ////        Prefix = "";

        ////    var c = (from cliente in db.CLIENTEs
        ////             where cliente.KUNNR.Contains(Prefix) && cliente.ACTIVO == true
        ////             select new { cliente.KUNNR, cliente.NAME1 }).ToList();

        ////    if (c.Count == 0)
        ////    {
        ////        var c2 = (from cliente in db.CLIENTEs
        ////                  where cliente.NAME1.Contains(Prefix) && cliente.ACTIVO == true
        ////                  select new { cliente.KUNNR, cliente.NAME1 }).ToList();
        ////        c.AddRange(c2);
        ////    }
        ////    JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
        ////    return cc;
        ////}
        [HttpPost]
        public JsonResult cliente(string Prefix, string usuario, string pais)
        {
            var clientes = clientesDao.ListaClientes(Prefix, usuario, pais);
            JsonResult cc = Json(clientes, JsonRequestBehavior.AllowGet);
            return cc;


        }

        //COSULTA DE AJAX PARA VALIDAR LOS RANGOS DE FECHA 
        public JsonResult validaFechaH1(string Fecha1, string Fecha2)
        {
            if (Fecha1 == null)
                Fecha1 = "";

            if (Fecha2 == null)
                Fecha2 = "";

            var c = false;

            resultadoFechas restultado = new resultadoFechas();
            restultado=validaRangoFecha(Fecha1, Fecha2);
           JsonResult cc = Json(restultado, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL TIPO DE MONEDA 
        public JsonResult moneda(string Prefix, string SociedadH1)
        {
            if (Prefix == null)
                Prefix = "";

            var c = (from m in db.MONEDAs
                     join so in db.SOCIEDADs on m.WAERS equals so.WAERS
                     where (so.BUKRS == SociedadH1 && m.KTEXT.Contains(Prefix) && m.ACTIVO == true) || (m.WAERS == "USD" && m.ACTIVO == true)
                     group m by m.WAERS into g
                     select new { WAERS = g.Key }).ToList();

            if (c.Count <= 1)
            {
                c = (from m in db.MONEDAs
                     join so in db.SOCIEDADs on m.WAERS equals so.WAERS
                     where (so.BUKRS == SociedadH1 && m.WAERS.Contains(Prefix) && m.ACTIVO == true) || (m.WAERS == "USD" && m.ACTIVO == true)
                     group m by m.WAERS into g
                     select new { WAERS = g.Key }).ToList();
                //c.AddRange(c2);
            }
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        #endregion 

        #region JSON PARA LA HOJA 2 Y 4
        //COSULTA DE AJAX PARA LA FACTURA
        public JsonResult factura(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 50)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA LA FECHA H2
        public JsonResult validaFechaH2(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            c = validaFormatoFechaH2(Prefix);

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL TIPO DE PROVEEDOR
        public JsonResult proveedor(string Prefix, string Cliente)
        {
            if (Prefix == null)
                Prefix = "";

            if (Cliente.Length < 10) { Cliente = cad.completaCliente(Cliente); }

            //var c = (from p in db.PROVEEDORs
            //         where p.ID.Contains(Prefix) && p.SOCIEDAD_ID == Sociedad && p.ACTIVO == true
            //         select new { p.ID, p.NOMBRE }).ToList();

            //if (c.Count == 0)
            //{
            //    var c2 = (from p in db.PROVEEDORs
            //              where p.NOMBRE.Contains(Prefix) && p.SOCIEDAD_ID == Sociedad && p.ACTIVO == true
            //              select new { p.ID, p.NOMBRE }).ToList();
            //    c.AddRange(c2);
            //}

            var con = (from p in db.PROVEEDORs
                       join c in db.CLIENTEs on p.ID equals c.PROVEEDOR_ID
                       where p.ID.Contains(Prefix) && p.ACTIVO == true && c.KUNNR == Cliente
                       group p by new { p.ID, p.NOMBRE } into g
                       select new { ID = g.Key.ID, NOMBRE = g.Key.NOMBRE }).ToList();

            if (con.Count == 0)
            {
                var con2 = (from p in db.PROVEEDORs
                            join c in db.CLIENTEs on p.ID equals c.PROVEEDOR_ID
                            where p.NOMBRE.Contains(Prefix) && p.ACTIVO == true && c.KUNNR == Cliente
                            group p by new { p.ID, p.NOMBRE } into g
                            select new { ID = g.Key.ID, NOMBRE = g.Key.NOMBRE }).ToList();
                con.AddRange(con2);
            }

            JsonResult cc = Json(con, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA LA AUTORIZACION
        public JsonResult autorizacion(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 50)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA LA FACTURA KELLOGS
        public JsonResult facturak(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 4000)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL EJERCICIO KELLOGS
        public JsonResult ejerciciok(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length == 4)
            {
                if (IsNumeric(Prefix))
                {
                    c = true;
                }
                else
                {
                    c = false;
                }
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        #endregion

        #region JSON PARA LA HOJA 3
        //COSULTA DE AJAX PARA BILL_DOC H3
        public JsonResult bill(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 10)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL IMPORTE
        public JsonResult importe(string Prefix, string Pais)
        {
            //var c = false;
            var decimales = db.PAIS.Where(x => x.LANDX == Pais).FirstOrDefault().DECIMAL;
            List<object> c = new List<object>();

            if (Prefix == null)
                Prefix = "";

            if (IsNumeric(Prefix))
            {
                c.Add(fc.toShow(Convert.ToDecimal(Prefix), decimales));
                c.Add(true);
            }
            else
            {
                c.Add(fc.toShow(Convert.ToDecimal("0"), decimales));
                c.Add(false);
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA BELNR H3
        public JsonResult belnr(string Prefix)
        {
            var c = false;
            if (Prefix == null)
                Prefix = "";

            if (Prefix.Length <= 10)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        #endregion


        //COSULTA DE AJAX PARA EL TIPO DE DECIMAL
        public JsonResult getDecimal(string Pais)
        {
            if (Pais == null)
                Pais = "";

            var dec = db.PAIS.Where(x => x.LANDX == Pais).FirstOrDefault().DECIMAL;

            JsonResult cc = Json(dec, JsonRequestBehavior.AllowGet);
            return cc;
        }

        //COSULTA DE AJAX PARA EL TIPO DE DECIMAL Y MILES
        public JsonResult getDecimalMil(string Pais)
        {
            List<object> c = new List<object>();
            if (Pais == null)
                Pais = "";

            var dec = db.PAIS.Where(x => x.LANDX == Pais).FirstOrDefault();

            c.Add(dec.MILES);
            c.Add(dec.DECIMAL);

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        
        

        public List<object> cargaInicialH1(DataRow fila, List<PAI> pp)//, DOCUMENTOP docp)
        {
            List<object> regresaRowH1 = new List<object>();

            string num_doc = fila[0].ToString().Trim();
            string t_sol = fila[1].ToString().Trim();
            string gall_id = fila[2].ToString().Trim();
            string bukrs = fila[3].ToString().Trim();
            string land = fila[4].ToString().Trim();
            string estado = fila[5].ToString().Trim();
            string ciudad = fila[6].ToString().Trim();
            string concepto = fila[7].ToString().Trim();
            string notas = fila[8].ToString().Trim();
            string payer_id = fila[9].ToString().Trim();
            if (payer_id.Length < 10) { payer_id = cad.completaCliente(payer_id); }
            string payer_nombre = fila[10].ToString().Trim();
            string contacto_nombre = fila[11].ToString().Trim();
            string contacto_email = fila[12].ToString().Trim();
            string fechai_vig = fila[13].ToString().Trim();
            string[] fechai_vig2 = fechai_vig.Split(' ');
            fechai_vig = fechai_vig2[0];
            string fechaf_vig = fila[14].ToString().Trim();
            string[] fechaf_vig2 = fechaf_vig.Split(' ');
            fechaf_vig = fechaf_vig2[0];
            string moneda_id = fila[15].ToString().Trim();
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            //fechai_vig = validaFech(fechai_vig, "x");
            //fechaf_vig = validaFech(fechaf_vig, "");

            if (IsNumeric(num_doc))
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Solo se aceptan números");
            }

            string land_id = "";
            PAI p = pp.Where(a => a.LANDX == land).FirstOrDefault();//ADD RSG 01.11.2018
            SOCIEDAD soc = new SOCIEDAD();
            string pLand = null;
            if (p != null)//ADD RSG 01.11.2018
            {
                land_id = p.LAND;//ADD RSG 01.11.2018
                pLand = p.LAND;
            }

            if (t_sol.Length <= 10)
            {
                if (db.TSOLs.Where(x => x.ID == t_sol).Count() > 0)
                {
                    regresaRowH1.Add("");
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo  | Tipo de solicitud no encontrada"); 
                }
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (10)");
            }

            List<TALLT> list =tallsDao.ListaTallsConCuenta(TATConstantes.ACCION_LISTA_TALLTCONCUENTA, null,spras_id, land_id, DateTime.Now.Year, bukrs);
            if (list.Any(x => x.TXT50 == gall_id))
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Clasificación no encontrada");
            }

            if (land_id != null && land_id != "")
                soc = db.SOCIEDADs.Find(bukrs);
            if (bukrs.Length == 4)
            {
                //if (db.SOCIEDADs.Where(x => x.BUKRS == bukrs).Count() > 0)
                if (soc != null)
                {
                    if (soc.BUKRS == bukrs)
                    {
                        regresaRowH1.Add("");
                    }
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo  | Sociedad no encontrada"); 
                }
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (4)");
            }
            if (land.Length <= 50)
            {
                //if (db.PAIS.Where(x => x.LANDX == land & x.SOCIEDAD_ID == bukrs).Select(x => x.LAND).Count() > 0)
                if (p != null)
                {
                    if (p.LANDX == land)
                    {
                        regresaRowH1.Add("");
                    }
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo | País no encontrado");
                }
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (50)");
            }

            if (estado.Length <= 50)
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (50)");
            }

            if (ciudad.Length <= 50)
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (50)");
            }

            if (concepto.Length <= 100)
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (100)");
            }

            if (notas.Length <= 255)
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (255)");
            }

            if (payer_id.Length <= 10)
            {
                List<CLIENTE> listClientes=clientesDao.ListaClientes(null,User.Identity.Name, pLand);
                //if (db.CLIENTEs.Where(x => x.KUNNR == payer_id && x.ACTIVO && x.LAND==p.LAND).Count() > 0)
                if(listClientes.Any(x=>x.KUNNR==payer_id))
                {
                    regresaRowH1.Add("");
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo | Cliente no encontrado");
                }
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (10)");
            }

            regresaRowH1.Add("");
            //if (db.CLIENTEs.Where(x => x.KUNNR == payer_id & x.NAME1 == payer_nombre & x.ACTIVO == true).Count() > 0)
            //{
            //    regresaRowH1.Add("");
            //}
            //else
            //{
            //    regresaRowH1.Add("red white-text rojo");
            //}

            if (contacto_nombre.Length <= 50)
            {
                regresaRowH1.Add("");
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (50)");
            }

            if (contacto_email.Length <= 255)
            {
                if (validaCorreo(contacto_email))
                {
                    regresaRowH1.Add("");
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo | Formato invalido");
                }
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (255)");
            }

            resultadoFechas validarFechas = new resultadoFechas();
            validarFechas = validaRangoFecha(fechai_vig, fechaf_vig);
            if (validarFechas.status)
            {
                regresaRowH1.Add("");
                regresaRowH1.Add("");
            }
            else
            {
                if (validarFechas.tipo == "Rango")
                {
                    regresaRowH1.Add("red white-text rojo |Rango invalido");
                    regresaRowH1.Add("red white-text rojo |Rango invalido");
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo | Formato invalido");
                    regresaRowH1.Add("red white-text rojo | Formato invalido");
                }
            }

            if (moneda_id.Length == 3)
            {
                if (db.MONEDAs.Where(x => x.WAERS == moneda_id).Count() > 0)
                {
                    regresaRowH1.Add("");
                }
                else
                {
                    regresaRowH1.Add("red white-text rojo | Moneda no encontrada");
                }
            }
            else
            {
                regresaRowH1.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (3)");
            }

            return regresaRowH1;
        }

        public List<object> cargaInicialH2(DataRow fila, CLIENTE cliente)
        {
            DataSet ds1 = (DataSet)Session["ds1"];
            List<object> regresaRowH2 = new List<object>();//REGRESA LA LISTA DE ERRORES

            string num_doc = fila[0].ToString().Trim();
            string factura = fila[1].ToString().Trim();
            string fecha_factura = fila[2].ToString().Trim();
            string[] fecha_factura2 = fecha_factura.Split(' ');
            fecha_factura = fecha_factura2[0];
            string proveedor = fila[3].ToString().Trim();
            if (proveedor.Length < 10) { proveedor = cad.completaCliente(proveedor); }
            string proveedor_nombre = fila[4].ToString().Trim();
            string autorizacion = fila[5].ToString().Trim();
            string fecha_vencimiento = fila[6].ToString().Trim();
            string[] fecha_vencimiento2 = fecha_vencimiento.Split(' ');
            fecha_vencimiento = fecha_vencimiento2[0];
            string facturak = fila[7].ToString().Trim();
            string ejerciciok = fila[8].ToString().Trim();

            for (int i = 2; i < ds1.Tables[0].Rows.Count; i++)
            {
                string num_docH1 = ds1.Tables[0].Rows[i][0].ToString().Trim();
                string t_sol = ds1.Tables[0].Rows[i][1].ToString().Trim();
                string sociedad = ds1.Tables[0].Rows[i][3].ToString().Trim();
                string pais = ds1.Tables[0].Rows[i][4].ToString().Trim();
                pais = db.PAIS.Where(x => x.SOCIEDAD_ID == sociedad && x.LANDX == pais).FirstOrDefault().LAND;

                if (num_doc == num_docH1)
                {
                    var con = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == sociedad && x.PAIS_ID == pais && x.TSOL == t_sol).FirstOrDefault();

                    if (IsNumeric(num_doc))
                    {
                        regresaRowH2.Add("");
                    }
                    else
                    {
                        regresaRowH2.Add("red white-text rojo | Solo se aceptan números");
                    }

                    //if (factura.Length <= 50)
                    //{
                    //    regresaRowH2.Add("");
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.FACTURA)
                    {
                        if (factura.Length > 0 && factura.Length <= 50)
                        {
                            regresaRowH2.Add("");
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (factura == "")
                        //{

                        //}
                        //else
                        //{
                        //    if (factura.Length <= 50)
                        //    {
                        //        regresaRowH2.Add("");
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }

                    //if (fecha_factura.Length <= 10)
                    //{
                    //    if (validaFormatoFecha(fecha_factura))
                    //    {
                    //        regresaRowH2.Add("");
                    //    }
                    //    else
                    //    {
                    //        regresaRowH2.Add("red white-text rojo");
                    //    }
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.FECHA)
                    {
                        if (fecha_factura.Length > 0 & fecha_factura.Length <= 10)
                        {
                            if (validaFormatoFecha(fecha_factura))
                            {
                                regresaRowH2.Add("");
                            }
                            else
                            {
                                regresaRowH2.Add("red white-text rojo");
                            }
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (fecha_factura == "")
                        //{
                        //    regresaRowH2.Add("");
                        //}
                        //else
                        //{
                        //    if (fecha_factura.Length <= 10)
                        //    {
                        //        if (validaFormatoFecha(fecha_factura))
                        //        {
                        //            regresaRowH2.Add("");
                        //        }
                        //        else
                        //        {
                        //            regresaRowH2.Add("red white-text rojo");
                        //        }
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }

                    //if (proveedor.Length <= 10)
                    //{
                    //    if (db.PROVEEDORs.Where(x => x.ID == proveedor & x.SOCIEDAD_ID == sociedad).Count() > 0)
                    //    {
                    //        regresaRowH2.Add("");
                    //    }
                    //    else
                    //    {
                    //        regresaRowH2.Add("red white-text rojo");
                    //    }
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.PROVEEDOR)
                    {
                        if (proveedor.Length > 0 && proveedor.Length <= 10)
                        {
                            //if (db.PROVEEDORs.Where(x => x.ID == proveedor && x.SOCIEDAD_ID == sociedad).Count() > 0)
                            if(cliente.PROVEEDOR_ID==proveedor)
                            {
                                regresaRowH2.Add("");
                            }
                            else
                            {
                                regresaRowH2.Add("red white-text rojo | El cliente no tiene un proveedor asignado");
                            }
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (proveedor == "")
                        //{
                        //    regresaRowH2.Add("");
                        //}
                        //else
                        //{
                        //    if (proveedor.Length <= 10)
                        //    {
                        //        if (db.PROVEEDORs.Where(x => x.ID == proveedor & x.SOCIEDAD_ID == sociedad).Count() > 0)
                        //        {
                        //            regresaRowH2.Add("");
                        //        }
                        //        else
                        //        {
                        //            regresaRowH2.Add("red white-text rojo");
                        //        }
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }

                    regresaRowH2.Add("");//DEBAJO ESTA LA VALIDACION DEL PROOVEDOR
                    //if (proveedor_nombre == "")
                    //{
                    //    regresaRowH2.Add("");
                    //}
                    //else
                    //{
                    //    if (proveedor_nombre.Length <= 250)
                    //    {
                    //        if (db.PROVEEDORs.Where(x => x.ID == proveedor & x.NOMBRE == proveedor_nombre & x.SOCIEDAD_ID == sociedad).Count() > 0)
                    //        {
                    //            regresaRowH2.Add("");
                    //        }
                    //        else
                    //        {
                    //            regresaRowH2.Add("red white-text rojo");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        regresaRowH2.Add("red white-text rojo");
                    //    }
                    //}

                    //if (autorizacion.Length <= 50)
                    //{
                    //    regresaRowH2.Add("");
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.AUTORIZACION)
                    {
                        if (autorizacion.Length > 0 & autorizacion.Length <= 50)
                        {
                            regresaRowH2.Add("");
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (autorizacion == "")
                        //{
                        //    regresaRowH2.Add("");
                        //}
                        //else
                        //{
                        //    if (autorizacion.Length <= 50)
                        //    {
                        //        regresaRowH2.Add("");
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }

                    //if (fecha_vencimiento.Length <= 10)
                    //{
                    //    if (validaFormatoFecha(fecha_vencimiento))
                    //    {
                    //        regresaRowH2.Add("");
                    //    }
                    //    else
                    //    {
                    //        regresaRowH2.Add("red white-text rojo");
                    //    }
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.VENCIMIENTO)
                    {
                        if (fecha_vencimiento.Length > 0 & fecha_vencimiento.Length <= 10)
                        {
                            if (validaFormatoFecha(fecha_vencimiento))
                            {
                                regresaRowH2.Add("");
                            }
                            else
                            {
                                regresaRowH2.Add("red white-text rojo");
                            }
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (fecha_vencimiento == "")
                        //{
                        //    regresaRowH2.Add("");
                        //}
                        //else
                        //{
                        //    if (fecha_vencimiento.Length <= 10)
                        //    {
                        //        if (validaFormatoFecha(fecha_vencimiento))
                        //        {
                        //            regresaRowH2.Add("");
                        //        }
                        //        else
                        //        {
                        //            regresaRowH2.Add("red white-text rojo");
                        //        }
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }

                    //if (facturak.Length <= 4000)
                    //{
                    //    regresaRowH2.Add("");
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.FACTURAK)
                    {
                        if (facturak.Length > 0 & facturak.Length <= 4000)
                        {
                            regresaRowH2.Add("");
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (facturak == "")
                        //{
                        //    regresaRowH2.Add("");
                        //}
                        //else
                        //{
                        //    if (facturak.Length <= 4000)
                        //    {
                        //        regresaRowH2.Add("");
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }

                    //if (ejerciciok.Length == 4)
                    //{
                    //    if (IsNumeric(ejerciciok))
                    //    {
                    //        regresaRowH2.Add("");
                    //    }
                    //    else
                    //    {
                    //        regresaRowH2.Add("red white-text rojo");
                    //    }
                    //}
                    //else
                    //{
                    //    regresaRowH2.Add("red white-text rojo");
                    //}

                    if (con.EJERCICIOK)
                    {
                        if (ejerciciok.Length > 0 & ejerciciok.Length == 4)
                        {
                            if (IsNumeric(ejerciciok))
                            {
                                regresaRowH2.Add("");
                            }
                            else
                            {
                                regresaRowH2.Add("red white-text rojo");
                            }
                        }
                        else
                        {
                            regresaRowH2.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        regresaRowH2.Add("");
                        //if (ejerciciok == "")
                        //{
                        //    regresaRowH2.Add("");
                        //}
                        //else
                        //{
                        //    if (ejerciciok.Length <= 4)
                        //    {
                        //        if (IsNumeric(ejerciciok))
                        //        {
                        //            regresaRowH2.Add("");
                        //        }
                        //        else
                        //        {
                        //            regresaRowH2.Add("red white-text rojo");
                        //        }
                        //    }
                        //    else
                        //    {
                        //        regresaRowH2.Add("red white-text rojo");
                        //    }
                        //}
                    }
                }
            }

            return regresaRowH2;
        }

        public List<object> cargaInicialH2W(DataRow fila)
        {
            DataSet ds1 = (DataSet)Session["ds1"];
            List<object> regresaRowH2W = new List<object>();//REGRESA LA LISTA DE WARNINGS CON CAMPOS OBLIGATORIOS

            string num_doc = fila[0].ToString().Trim();
            string factura = fila[1].ToString().Trim();
            string fecha_factura = fila[2].ToString().Trim();
            string[] fecha_factura2 = fecha_factura.Split(' ');
            fecha_factura = fecha_factura2[0];
            string proveedor = fila[3].ToString().Trim();
            if (proveedor.Length < 10) { proveedor = cad.completaCliente(proveedor); }
            string proveedor_nombre = fila[4].ToString().Trim();
            string autorizacion = fila[5].ToString().Trim();
            string fecha_vencimiento = fila[6].ToString().Trim();
            string[] fecha_vencimiento2 = fecha_vencimiento.Split(' ');
            fecha_vencimiento = fecha_vencimiento2[0];
            string facturak = fila[7].ToString().Trim();
            string ejerciciok = fila[8].ToString().Trim();

            for (int i = 2; i < ds1.Tables[0].Rows.Count; i++)
            {
                string num_docH1 = ds1.Tables[0].Rows[i][0].ToString().Trim();
                string t_sol = ds1.Tables[0].Rows[i][1].ToString().Trim();
                string sociedad = ds1.Tables[0].Rows[i][3].ToString().Trim();
                string pais = ds1.Tables[0].Rows[i][4].ToString().Trim();
                pais = db.PAIS.Where(x => x.SOCIEDAD_ID == sociedad & x.LANDX == pais).FirstOrDefault().LAND;

                if (num_doc == num_docH1)
                {
                    var con = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == sociedad & x.PAIS_ID == pais & x.TSOL == t_sol).FirstOrDefault();

                    if (IsNumeric(num_doc))
                    {
                        regresaRowH2W.Add("");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.FACTURA)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.FECHA)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.PROVEEDOR)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (proveedor_nombre.Length <= 250)
                    {
                        regresaRowH2W.Add("");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.AUTORIZACION)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.VENCIMIENTO)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.FACTURAK)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }

                    if (con.EJERCICIOK)
                    {
                        regresaRowH2W.Add("yelloww blue");
                    }
                    else
                    {
                        regresaRowH2W.Add("");
                    }
                }
            }

            return regresaRowH2W;
        }

        public List<object> cargaInicialH3(DataRow fila)
        {
            DataSet ds1 = (DataSet)Session["ds1"];
            List<object> regresaRowH3 = new List<object>();

            string num_doc = fila[0].ToString().Trim();
            string factura = fila[1].ToString().Trim();
            string bill_doc = fila[2].ToString().Trim();
            string ejerciciok = fila[3].ToString().Trim();
            string payer_id = fila[4].ToString().Trim();
            if (payer_id.Length < 10) { payer_id = cad.completaCliente(payer_id); }
            string payer_nombre = fila[5].ToString().Trim();
            string importe_fac = fila[6].ToString().Trim();
            string belnr = fila[7].ToString().Trim();

            for (int i = 2; i < ds1.Tables[0].Rows.Count; i++)
            {
                string num_docH1 = ds1.Tables[0].Rows[i][0].ToString().Trim();
                string sociedad = ds1.Tables[0].Rows[i][3].ToString().Trim();

                if (num_doc == num_docH1)
                {
                    if (IsNumeric(num_doc))
                    {
                        regresaRowH3.Add("");
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo | Solo se aceptan números");
                    }

                    if (factura.Length <= 50)
                    {
                        regresaRowH3.Add("");
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo");
                    }

                    if (bill_doc.Length <= 10)
                    {
                        regresaRowH3.Add("");
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo");
                    }

                    if (ejerciciok.Length == 4)
                    {
                        regresaRowH3.Add("");
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo");
                    }

                    if (payer_id.Length <= 10)
                    {
                        if (db.CLIENTEs.Where(x => x.KUNNR == payer_id & x.ACTIVO).Count() > 0)
                        {
                            regresaRowH3.Add("");
                        }
                        else
                        {
                            regresaRowH3.Add("red white-text rojo | Cliente no encontrado");
                        }
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo | Se superó la cantidad maxima de caracteres (4)");
                    }

                    regresaRowH3.Add("");
                    //if (db.CLIENTEs.Where(x => x.KUNNR == payer_id & x.NAME1 == payer_nombre & x.ACTIVO == true).Count() > 0)
                    //{
                    //    regresaRowH3.Add("");
                    //}
                    //else
                    //{
                    //    regresaRowH3.Add("red white-text rojo");
                    //}

                    if (IsNumeric(importe_fac))
                    {
                        regresaRowH3.Add("");
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo | Solo se aceptan números");
                    }

                    if (belnr.Length <= 10)
                    {
                        regresaRowH3.Add("");
                    }
                    else
                    {
                        regresaRowH3.Add("red white-text rojo");
                    }
                }
            }
            return regresaRowH3;
        }

        public List<string> cargaInicialH4(Encabezado objInformacion, Distribucion objDistribucion, List<MATERIALGPT> listCategorias, List<DataRow> listRows)
        {
            DataSet ds4 = (DataSet)Session["ds4"];
            List<string> regresaRowH4 = new List<string>();
            List<bool> categoriasHealty = new List<bool>();
            List<string> categoriasNum = new List<string>();
            List<bool> materialesHealty = new List<bool>();
            List<string> materialesNum = new List<string>();
            List<string> listLigadas = new List<string>();
            List<object> id = new List<object>();

            List<string> ListMaterialesDoc = new List<string>();
            List<string> ListCategoriasDoc = new List<string>();

            string[] vigenciaInfo_de2 = objInformacion.FECHAI_VIG.Split(' ');
            string[] vigenciaInfo_al2 = objInformacion.FECHAF_VIG.Split(' ');

            string[] vigenciaDis_de = objDistribucion.VIGENCIA_DE.Split(' ');
            string[] vigenciaDis_al = objDistribucion.VIGENCIA_AL.Split(' ');

            string colorLigada = "";
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            PAI pais_ = db.PAIS.Where(x => x.LANDX.ToUpper() == objInformacion.PAIS_NAME.ToUpper()).FirstOrDefault();
            string decimales = ".";
            string miles = ",";
            if (pais_ != null) {
                 miles = pais_.MILES;
                 decimales = pais_.DECIMAL;
            }

            if (objDistribucion != null)
            {
                if (IsNumeric(objDistribucion.NUM_DOC))
                {
                    regresaRowH4.Add("");
                }
                else
                {
                    regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                }

                regresaRowH4.Add("nada");

                resultadoFechas FechasDistribucion = new resultadoFechas();
                FechasDistribucion = validaRangoFecha(vigenciaDis_de[0], vigenciaDis_al[0]);

                if (FechasDistribucion.status)
                {
                    resultadoFechas FechaIniInfoDis = new resultadoFechas();
                    FechaIniInfoDis = validaRangoFecha(vigenciaInfo_de2[0], vigenciaDis_de[0]);

                    if (!FechaIniInfoDis.status)
                    {
                        if (FechaIniInfoDis.tipo == "Rango")
                        {
                            regresaRowH4.Add("red white-text rojo |  No debe ser menor a la fecha inicial de la solicitud");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo |  Formato invalido");
                        }
                    }
                    else
                    {
                        regresaRowH4.Add("");
                    }

                    resultadoFechas FechaFinInfoDis = new resultadoFechas();
                    FechaFinInfoDis = validaRangoFecha(vigenciaDis_al[0], vigenciaInfo_al2[0]);

                    if (!FechaFinInfoDis.status)
                    {
                        if (FechaIniInfoDis.tipo == "Rango")
                        {
                            regresaRowH4.Add("red white-text rojo | No debe ser mayor a la fecha final de la solicitud");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Formato invalido");
                        }
                      
                    }
                    else
                    {
                        regresaRowH4.Add("");
                    }
                }
                else
                {
                    if (FechasDistribucion.tipo == "Rango")
                    {
                        regresaRowH4.Add("red white-text rojo |Rango invalido");
                        regresaRowH4.Add("red white-text rojo |Rango invalido");
                    }
                    else
                    {
                        regresaRowH4.Add("red white-text rojo | Formato invalido");
                        regresaRowH4.Add("red white-text rojo | Formato invalido");
                    }
                }


                if ((objDistribucion.MATNR != "" && objDistribucion.MATKL != "") || (objDistribucion.MATNR != "" && objDistribucion.MATKL == ""))
                {
                    foreach (DataRow row in listRows)
                    {
                        string miNum_docH4 = row[0].ToString().Trim();
                        string miMaterialH4 = row[4].ToString().Trim();
                        string CategoriaH4 = row[5].ToString().Trim();
                        if (miMaterialH4.Length < 18) { miMaterialH4 = cad.completaMaterial(miMaterialH4); }
                        string num_material = "";
                        bool materiall = false;

                        if (miNum_docH4 != "" && miNum_docH4 == objDistribucion.NUM_DOC)
                        {
                            //MATERIALES
                            try
                            {
                               
                                List<MATERIAL> ListMateriales = materialesDao.ListaMateriales(miMaterialH4, objInformacion.VKORG, objInformacion.VTWEG, User.Identity.Name);
                                MATERIAL miMaterial = ListMateriales.Where(x => x.ID == miMaterialH4).FirstOrDefault();

                                if (miMaterial!=null)
                                {
                                    num_material = miMaterial.ID.ToString();
                                    var paso = db.MATERIALGPs.Where(x => x.ID == miMaterial.MATERIALGP_ID).FirstOrDefault();
                                    materiall = paso.UNICA;
                                }
                                else
                                {
                                    num_material = "";
                                    miMaterialH4 = "";
                                }
                                
                            }
                            catch
                            {
                                num_material = "";
                                miMaterialH4 = "";
                            }
                            materialesNum.Add(num_material);
                            materialesHealty.Add(materiall);
                            if (miMaterialH4 == "" && CategoriaH4 != "")
                            {
                                ListCategoriasDoc.Add(CategoriaH4);
                            }
                            else if (miMaterialH4 != "")
                            {

                                ListMaterialesDoc.Add(miMaterialH4);
                            }
                            
                        }
                    }
                    var matDistribucion = objDistribucion.MATNR;
                    if (matDistribucion.Length < 18) { matDistribucion = cad.completaMaterial(matDistribucion); }
                    List<MATERIAL> ListMateriales_ = materialesDao.ListaMateriales(matDistribucion, objInformacion.VKORG, objInformacion.VTWEG, User.Identity.Name);
                    MATERIAL material = ListMateriales_.Where(x => x.ID == matDistribucion).FirstOrDefault();
                    if (material!=null)
                    {
                        bool tieneHealtyMat = false;
                        List<object> num_mat = new List<object>();

                        for (int k = 0; k < materialesNum.Count; k++)
                        {
                            if (materialesNum[k].Contains(material.ID.ToString()))
                                num_mat.Add(materialesNum[k]);
                        }

                        if (materialesHealty.Distinct().Skip(1).Any())
                        {
                            tieneHealtyMat = true;
                        }

                        if (tieneHealtyMat || num_mat.Count() > 1)
                        {
                            regresaRowH4.Add("red white-text rojo |  Las categorías unicas no se pueden mezclar con otras categorias y/o Materiales");
                        }
                        else if (ListCategoriasDoc.Count()>0 && ListMaterialesDoc.Count()>0)
                        {
                            regresaRowH4.Add("red white-text rojo |  Los materiales no se pueden mezclar con categorias");
                        }
                        else
                        {
                            regresaRowH4.Add("");
                        }

                        if (material.MATERIALGP_ID != null)
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Categoría no encontrado");
                        }
                    }
                    else
                    {
                        regresaRowH4.Add("red white-text rojo | Material no encontrado");
                    }
                    regresaRowH4.Add("");
                }
                else if (objDistribucion.MATNR == "" && objDistribucion.MATKL != "")
                {
                    regresaRowH4.Add("");

                    foreach (DataRow row in listRows)
                    {
                        string miNum_docH4 = row[0].ToString().Trim();
                        string mat = row[4].ToString().Trim();
                        string categoriaa = row[5].ToString().Trim();
                        string num_categoria = "";
                        bool categoriaUnica = false;

                        if (miNum_docH4 != "" && miNum_docH4 == objDistribucion.NUM_DOC)
                        {
                            //CATEGORIAS
                            try
                            {
                                MATERIALGPT categoriagpt = listCategorias.Where(x => x.TXT50.ToUpper() == categoriaa.ToUpper()).FirstOrDefault();
                                if (categoriagpt != null)
                                {
                                    var miCategoria = db.MATERIALGPs.Where(x => x.ID == categoriagpt.MATERIALGP_ID).FirstOrDefault();
                                    num_categoria = miCategoria.ID.ToString();
                                    categoriaUnica = miCategoria.UNICA;
                                }
                                else
                                {
                                    num_categoria = "";
                                }


                            }
                            catch
                            {
                                num_categoria = "";
                            }

                            //id.Add(miNum_docH4);
                            categoriasNum.Add(num_categoria);
                            categoriasHealty.Add(categoriaUnica);
                            
                            if (mat == "" && categoriaa != "")
                            {
                                ListCategoriasDoc.Add(categoriaa);
                            }else if (mat != "")
                            {
                                ListMaterialesDoc.Add(mat);
                            }
                        }
                    }


                    MATERIALGPT categoria_ = listCategorias.Where(x => x.TXT50.ToUpper() == objDistribucion.MATKL.ToUpper()).FirstOrDefault();
                    if (categoria_!=null)
                    {
                        MATERIALGP categoria = db.MATERIALGPs.Where(x => x.ID == categoria_.MATERIALGP_ID).FirstOrDefault();

                        List<object> tieneHealty = new List<object>();
                        List<object> num_cat = new List<object>();
                        bool tieneHealtyCat = false;
                        
                        for (int k = 0; k < categoriasNum.Count; k++)
                        {
                            if (categoriasNum[k].Contains(categoria.ID.ToString()))
                                num_cat.Add(categoriasNum[k]);
                        }

                        if (categoriasHealty.Distinct().Skip(1).Any())
                        {
                            tieneHealtyCat = true;
                        }

                        if ((tieneHealtyCat))
                        {
                            regresaRowH4.Add("red white-text rojo | Las categorías unicas no se pueden mezclar con otras categorias y/o Materiales.");
                        }
                        else if (num_cat.Count() > 1)
                        {
                            regresaRowH4.Add("red white-text rojo | Una categoría no puede ser agregada más de una vez en la misma solicitud.");
                        }
                        else if (ListCategoriasDoc.Count() > 0 && ListMaterialesDoc.Count() > 0)
                        {
                            regresaRowH4.Add("red white-text rojo |  Los materiales no se pueden mezclar con categorías");
                        }
                        else
                        {
                            regresaRowH4.Add("");
                        }
                    }
                    else
                    {
                        regresaRowH4.Add("red white-text rojo | Categoría no encontrada");
                    }
                    regresaRowH4.Add("");
                }
                else
                {
                    regresaRowH4.Add("red white-text rojo");
                    regresaRowH4.Add("red white-text rojo");
                    regresaRowH4.Add("");
                }
              
                if (objDistribucion.LIGADA != "")
                {
                    regresaRowH4.Add("");//MONTO

                    if (IsNumeric(fc.toNum(objDistribucion.PORC_APOYO, miles, decimales)))
                    {
                        foreach (DataRow row in listRows)
                        {
                            string miNum_docH4 = row[0].ToString().Trim();
                            string miLigadaH4 = row[1].ToString().Trim();
                            string miPorc_apoyoH4 = row[8].ToString().Trim();

                            if (miNum_docH4 != "" && miNum_docH4 == objDistribucion.NUM_DOC)
                            {
                                listLigadas.Add(miNum_docH4 + miLigadaH4 + miPorc_apoyoH4);
                            }
                        }

                        bool kk = listLigadas.Distinct().Skip(1).Any();

                        if (Convert.ToDecimal(fc.toNum(objDistribucion.PORC_APOYO, miles, decimales)) > 0 && !kk)
                        {
                            colorLigada = "";
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            colorLigada = "red white-text rojo";
                            regresaRowH4.Add("red white-text rojo");
                        }
                    }
                    else
                    {
                        foreach (DataRow row in listRows)
                        {
                            string miNum_docH4 = row[0].ToString().Trim();
                            string miLigadaH4 = row[1].ToString().Trim();
                            string miPorc_apoyoH4 = row[8].ToString().Trim();

                            if (miNum_docH4 != "" && miNum_docH4 == objDistribucion.NUM_DOC)
                            {
                                listLigadas.Add(miNum_docH4 + miLigadaH4 + miPorc_apoyoH4);
                            }
                        }

                        bool kk = listLigadas.Distinct().Skip(1).Any();

                        if (!kk && objDistribucion.PORC_APOYO != "")
                        {
                            colorLigada = "";
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            colorLigada = "red white-text rojo";
                            regresaRowH4.Add("red white-text rojo");
                        }
                    }

                    regresaRowH4.Add("");//APOYO PIEZA
                    regresaRowH4.Add("");//COSTO APOYO
                    regresaRowH4.Add("");//PRECIO SUGERIDO
                    regresaRowH4.Add("");//VOLUMEN
                    regresaRowH4.Add("");//APOYO
                }
                else
                {
                    if (objDistribucion.APOYO != "")
                    {
                        regresaRowH4.Add("");//MONTO

                        foreach (DataRow row in listRows)
                        {
                            string miNum_docH4 = row[0].ToString().Trim();
                            string miLigadaH4 = row[1].ToString().Trim();
                            string miPorc_apoyoH4 = row[8].ToString().Trim();

                            if (miNum_docH4 != "" && miNum_docH4 == objDistribucion.NUM_DOC)
                            {
                                if (miLigadaH4 != "")
                                {
                                    listLigadas.Add("true");
                                }
                                else
                                {
                                    listLigadas.Add("false");
                                }
                            }
                        }

                        bool validaLigadaList = listLigadas.Distinct().Skip(1).Any();
                        bool existeLigada = listLigadas.Contains("true");

                        if (existeLigada)
                        {
                            if (!validaLigadaList)
                            {
                                colorLigada = "";
                                regresaRowH4.Add("");//PORCENTAJE APOYO
                            }
                            else
                            {
                                colorLigada = "red white-text rojo";
                                regresaRowH4.Add("red white-text rojo");//PORCENTAJE APOYO
                            }
                        }
                        else
                        {
                            colorLigada = "";
                            regresaRowH4.Add("");//PORCENTAJE APOYO
                        }

                        regresaRowH4.Add("");//APOYO PIEZA
                        regresaRowH4.Add("");//COSTO APOYO
                        regresaRowH4.Add("");//PRECIO SUGERIDO
                        regresaRowH4.Add("");//VOLUMEN
                        if (IsNumeric(fc.toNum(objDistribucion.APOYO, miles, decimales)))
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                        }
                    }
                    else
                    {
                        if (IsNumeric(fc.toNum(objDistribucion.MONTO, miles, decimales)))
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                        }

                        
                        if (IsNumeric(fc.toNum(objDistribucion.PORC_APOYO, miles, decimales)))
                        {
                            foreach (DataRow row in listRows)
                            {
                                string miNum_docH4 = row[0].ToString().Trim();
                                string miLigadaH4 = row[1].ToString().Trim();
                                string miPorc_apoyoH4 = row[8].ToString().Trim();

                                if (miLigadaH4 != "")
                                {
                                    listLigadas.Add("true");
                                }
                                else
                                {
                                    listLigadas.Add("false");
                                }
                            }

                            bool validaLigadaList = listLigadas.Distinct().Skip(1).Any();
                            bool existeLigada = listLigadas.Contains("true");

                            if (existeLigada)
                            {
                                if (!validaLigadaList)
                                {
                                    colorLigada = "";
                                    regresaRowH4.Add("");//PORCENTAJE APOYO
                                }
                                else
                                {
                                    colorLigada = "red white-text rojo";
                                    regresaRowH4.Add("red white-text rojo");//PORCENTAJE APOYO
                                }
                            }
                            else
                            {
                                colorLigada = "";
                                regresaRowH4.Add("");//PORCENTAJE APOYO
                            }
                        }
                        else
                        {
                            foreach (DataRow row in listRows)
                            {
                                string miNum_docH4 = row[0].ToString().Trim();
                                string miLigadaH4 = row[1].ToString().Trim();
                                string miPorc_apoyoH4 = row[8].ToString().Trim();

                                if (miLigadaH4 != "")
                                {
                                    listLigadas.Add("true");
                                }
                                else
                                {
                                    listLigadas.Add("false");
                                }
                            }

                            bool validaLigadaList = listLigadas.Distinct().Skip(1).Any();
                            bool existeLigada = listLigadas.Contains("true");

                            if (existeLigada)
                            {
                                if (!validaLigadaList)
                                {
                                    colorLigada = "";
                                    regresaRowH4.Add("");//PORCENTAJE APOYO
                                }
                                else
                                {
                                    colorLigada = "red white-text rojo";
                                    regresaRowH4.Add("red white-text rojo");//PORCENTAJE APOYO
                                }
                            }
                            else
                            {
                                colorLigada = "";
                                regresaRowH4.Add("");//PORCENTAJE APOYO
                            }
                        }

                        if (IsNumeric(fc.toNum(objDistribucion.APOYO_PIEZA, miles, decimales)))
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                        }

                        if (IsNumeric(fc.toNum(objDistribucion.COSTO_APOYO, miles, decimales)))
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                        }

                        if (IsNumeric(fc.toNum(objDistribucion.PRECIO_SUG, miles, decimales)))
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                        }

                        if (IsNumeric(fc.toNum(objDistribucion.VOLUMEN_REAL, miles, decimales)))
                        {
                            regresaRowH4.Add("");
                        }
                        else
                        {
                            regresaRowH4.Add("red white-text rojo | Solo se aceptan números");
                        }

                        regresaRowH4.Add("");
                    }
                }

                for (int j = 0; j < regresaRowH4.Count; j++)
                {
                    if (regresaRowH4[j].Contains("nada"))
                        regresaRowH4[j] = colorLigada;
                }
            }

            return regresaRowH4;
        }

        public JsonResult setArchivos()
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            bool c;
            HttpPostedFileBase[] archivosArr = new HttpPostedFileBase[Request.Files.Count];

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                archivosArr[i] = file;
            }
            IEnumerable<HttpPostedFileBase> archivosLi = (IEnumerable<HttpPostedFileBase>)archivosArr;

            if (archivosLi.Count() > 0)
            {
                c = true;
            }
            else
            {
                c = false;
            }

            Session["archivosSoporte"] = archivosLi;
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        public JsonResult setDatos(List<object> h1, List<object> h2, List<object> h3, List<object> h4, List<object> h5, List<object> notas)
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }

            DataTable dtt = ConvertToDatatable(h1, "h1");
            DataTable dtt2 = ConvertToDatatable(h2, "h2");
            DataTable dtt3 = ConvertToDatatable(h3, "h3");
            DataTable dtt4 = ConvertToDatatable(h4, "h4");
            DataTable dtt5 = ConvertToDatatable(h5, "h5");
            DataTable dtt6 = ConvertToDatatable(notas, "notas");

            //////CABECERA
            DataSet dt = new DataSet();
            dt.Tables.Add(dtt);

            /////DOCUMENTOS RELACIONADOS
            DataSet dt2 = new DataSet();
            dt2.Tables.Add(dtt2);

            /////DOCUMENTOS RELACIONADOS MULTIPLES
            DataSet dt3 = new DataSet();
            dt3.Tables.Add(dtt3);

            /////MATERIALES
            DataSet dt4 = new DataSet();
            dt4.Tables.Add(dtt4);

            /////ARCHIVOS
            DataSet dt5 = new DataSet();
            dt5.Tables.Add(dtt5);

            /////NOTAS
            DataSet dt6 = new DataSet();
            dt6.Tables.Add(dtt6);

            var c = guardaDatos(dt, dt2, dt3, dt4, dt5, dt6);

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        public List<object> guardaDatos(DataSet ds1, DataSet ds2, DataSet ds3, DataSet ds4, DataSet ds5, DataSet ds6)
        {
            string u = User.Identity.Name;
            string errorString;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
            IEnumerable<HttpPostedFileBase> archivos = (IEnumerable<HttpPostedFileBase>)(Session["archivosSoporte"]);
            List<DOCUMENTO> listD = new List<DOCUMENTO>();
            List<object> iDs = new List<object>();

            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                DOCUMENTO docu = new DOCUMENTO();
                string num_doc = ds1.Tables[0].Rows[i][0].ToString();
                string t_sol = ds1.Tables[0].Rows[i][1].ToString().Trim();
                string tall_id = ds1.Tables[0].Rows[i][2].ToString().Trim();
                string gall_id = db.TALLs.Where(x => x.ID == tall_id & x.ACTIVO == true).Select(x => x.GALL_ID).FirstOrDefault();
                string bukrs = ds1.Tables[0].Rows[i][3].ToString().Trim();
                string land = ds1.Tables[0].Rows[i][4].ToString().Trim();
                string estado = ds1.Tables[0].Rows[i][5].ToString().Trim();
                string ciudad = ds1.Tables[0].Rows[i][6].ToString().Trim();
                string concepto = ds1.Tables[0].Rows[i][7].ToString().Trim();
                string notas = ds1.Tables[0].Rows[i][8].ToString().Trim();
                string payer_id = ds1.Tables[0].Rows[i][9].ToString().Trim();
                if (payer_id.Length < 10) { payer_id = cad.completaCliente(payer_id); }
                string payer_nombre = ds1.Tables[0].Rows[i][10].ToString().Trim();
                string contacto_nombre = ds1.Tables[0].Rows[i][11].ToString().Trim();
                string contacto_email = ds1.Tables[0].Rows[i][12].ToString().Trim();
                string fechai_vig = ds1.Tables[0].Rows[i][13].ToString().Trim();
                fechai_vig = validaPeriodoFecha(fechai_vig, "x");
                string fechaf_vig = ds1.Tables[0].Rows[i][14].ToString().Trim();
                fechaf_vig = validaPeriodoFecha(fechaf_vig, "");
                string moneda_id = ds1.Tables[0].Rows[i][15].ToString().Trim();

                var sociedad = db.SOCIEDADs.Where(x => x.BUKRS == bukrs).FirstOrDefault();
                string miles = db.PAIS.Where(x => x.LAND == land).FirstOrDefault().MILES;
                string decimales = db.PAIS.Where(x => x.LAND == land).FirstOrDefault().DECIMAL;

                var per = (from P in db.PAIS.ToList()
                           join C in db.CLIENTEs.Where(x => x.ACTIVO == true).ToList()
                           on P.LAND equals C.LAND
                           join U in db.USUARIOFs.Where(x => x.USUARIO_ID == u & x.ACTIVO == true)
                           on new { C.VKORG, C.VTWEG, C.SPART, C.KUNNR } equals new { U.VKORG, U.VTWEG, U.SPART, U.KUNNR }
                           where P.ACTIVO == true
                           select P).DistinctBy(x => x.LAND);

                var permiso = per.Where(x => x.SOCIEDAD_ID == bukrs & x.LAND == land).FirstOrDefault();


                if (permiso != null)
                {
                    docu.NUM_DOC = Convert.ToDecimal(num_doc);
                    docu.TSOL_ID = t_sol;
                    docu.TALL_ID = tall_id;
                    //docu.TALL_ID = db.TALLs.Where(x => x.DESCRIPCION == gall_id2).FirstOrDefault().ID;
                    docu.SOCIEDAD_ID = bukrs;
                    docu.PAIS_ID = land;
                    docu.ESTADO = estado;
                    docu.CIUDAD = ciudad;
                    docu.PERIODO = FnCommon.ObtenerPeriodoCalendario445(db,docu.SOCIEDAD_ID, docu.TSOL_ID,User.Identity.Name );
                    docu.EJERCICIO = Convert.ToString(System.DateTime.Now.Year);

                    docu.TIPO_TECNICO = "M";///////////////////////////////////////////////CHECK
                    docu.TIPO_RECURRENTE = null;
                    docu.CANTIDAD_EV = 2;
                    docu.USUARIOC_ID = user.ID;
                    docu.USUARIOD_ID = user.ID;
                    docu.FECHAD = System.DateTime.Today;
                    docu.FECHAC = System.DateTime.Today;
                    docu.HORAC = System.DateTime.Now.TimeOfDay;
                    docu.FECHAC_PLAN = System.DateTime.Today;
                    docu.FECHAC_USER = System.DateTime.Today;
                    docu.HORAC_USER = System.DateTime.Now.TimeOfDay;
                    docu.ESTATUS = "N";
                    docu.ESTATUS_C = null;
                    docu.ESTATUS_SAP = null;
                    docu.ESTATUS_WF = "P";
                    docu.DOCUMENTO_REF = null;
                    docu.CONCEPTO = concepto;
                    docu.NOTAS = notas;
                    docu.MONTO_FIJO_MD = null;
                    docu.MONTO_BASE_GS_PCT_MD = null;
                    docu.MONTO_BASE_NS_PCT_MD = null;
                    docu.MONTO_FIJO_ML = null;
                    docu.MONTO_BASE_GS_PCT_ML = null;
                    docu.MONTO_BASE_NS_PCT_ML = null;
                    docu.MONTO_FIJO_ML2 = null;
                    docu.MONTO_BASE_GS_PCT_ML2 = null;
                    docu.MONTO_BASE_NS_PCT_ML2 = null;
                    docu.PORC_ADICIONAL = null;
                    docu.IMPUESTO = null;
                    docu.FECHAI_VIG = Convert.ToDateTime(fechai_vig);
                    docu.FECHAF_VIG = Convert.ToDateTime(fechaf_vig);
                    docu.ESTATUS_EXT = null;
                    docu.SOLD_TO_ID = null;
                    docu.PAYER_ID = payer_id;
                    docu.PAYER_NOMBRE = contacto_nombre;
                    docu.PAYER_EMAIL = contacto_email;
                    docu.GRUPO_CTE_ID = null;
                    docu.CANAL_ID = null;
                    docu.MONEDA_ID = moneda_id;
                    docu.MONEDAL_ID = sociedad.WAERS;
                    docu.MONEDAL2_ID = "USD";
                    docu.TIPO_CAMBIO = 0;
                    docu.TIPO_CAMBIOL = 0;
                    docu.TIPO_CAMBIOL2 = 0;
                    docu.NO_FACTURA = null;
                    docu.FECHAD_SOPORTE = null;
                    docu.METODO_PAGO = null;
                    docu.NO_PROVEEDOR = null;
                    docu.PASO_ACTUAL = null;
                    docu.AGENTE_ACTUAL = null;
                    docu.FECHA_PASO_ACTUAL = null;
                    docu.VKORG = db.CLIENTEs.Where(x => x.KUNNR == payer_id).Select(x => x.VKORG).First();
                    docu.VTWEG = db.CLIENTEs.Where(x => x.KUNNR == payer_id).Select(x => x.VTWEG).First();
                    docu.SPART = db.CLIENTEs.Where(x => x.KUNNR == payer_id).Select(x => x.SPART).First();
                    docu.PUESTO_ID = user.PUESTO_ID;
                    docu.GALL_ID = gall_id;
                    docu.CONCEPTO_ID = null;
                    docu.DOCUMENTO_SAP = null;
                    docu.PORC_APOYO = 0;/////////////////////////////////////////////////////cantidad check
                    docu.LIGADA = null;///////////////////////check
                    docu.OBJETIVOQ = false;
                    docu.FRECUENCIA_LIQ = 1;
                    docu.OBJQ_PORC = 0;
                    docu.CUENTAP = null;
                    docu.CUENTAPL = null;
                    docu.EXCEDE_PRES = "";////////////////////////////checar presupusto
                    docu.MONTO_DOC_MD = 0; //ADD RSG 04.11.2018

                    CUENTA cta = db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(docu.SOCIEDAD_ID) & x.PAIS_ID.Equals(docu.PAIS_ID) & x.TALL_ID.Equals(docu.TALL_ID)).FirstOrDefault();
                    if (cta != null)
                    {
                        docu.CUENTAP = cta.ABONO;
                        docu.CUENTAPL = cta.CARGO;
                        docu.CUENTACL = cta.CLEARING;
                    }

                    listD.Add(docu);
                }
                DOCUMENTO dop = listD.Where(x => x.NUM_DOC == docu.NUM_DOC).FirstOrDefault();
                //FIN DE LA SECCION 1

                if (dop != null)
                {
                    //SECCION 2.- VALIDACION DE DATOS DE LA SEGUNDA HOJA DEL EXCEL (RELACIONADA)
                    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                    {
                        string num_docH2 = ds2.Tables[0].Rows[j][0].ToString().Trim();
                        num_docH2 = num_docH2.TrimStart('0').Trim();
                        string factura = ds2.Tables[0].Rows[j][1].ToString().Trim();
                        string fecha_factura = ds2.Tables[0].Rows[j][2].ToString().Trim();
                        string proveedor = ds2.Tables[0].Rows[j][3].ToString().Trim();
                        if (proveedor.Length < 10) { proveedor = cad.completaCliente(proveedor); }
                        string proveedor_nombre = ds2.Tables[0].Rows[j][4].ToString().Trim();
                        string autorizacion = ds2.Tables[0].Rows[j][5].ToString().Trim();
                        string fecha_vencimiento = ds2.Tables[0].Rows[j][6].ToString().Trim();
                        string facturak = ds2.Tables[0].Rows[j][7].ToString().Trim();
                        string ejerciciok = ds2.Tables[0].Rows[j][8].ToString().Trim();

                        if (num_docH2 == num_doc)
                        {
                            DOCUMENTOF docupF = new DOCUMENTOF();

                            var facturasConf = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == bukrs & x.PAIS_ID == land & x.TSOL == t_sol).FirstOrDefault();
                            if (facturasConf != null)
                            {
                                docupF.NUM_DOC = Convert.ToDecimal(num_docH2);
                                docupF.POS = j;

                                if (facturasConf.FACTURA == true)
                                { docupF.FACTURA = factura; }
                                else
                                { docupF.FACTURA = "0"; }

                                if (facturasConf.FECHA == true)
                                { docupF.FECHA = Convert.ToDateTime(fecha_factura); }
                                else
                                { docupF.FECHA = null; }

                                if (facturasConf.PROVEEDOR == true)
                                { docupF.PROVEEDOR = proveedor; }
                                else
                                { docupF.PROVEEDOR = "0"; }

                                docupF.CONTROL = "0";

                                if (facturasConf.AUTORIZACION == true)
                                { docupF.AUTORIZACION = autorizacion; }
                                else
                                { docupF.AUTORIZACION = "0"; }

                                if (facturasConf.VENCIMIENTO == true)
                                { docupF.VENCIMIENTO = Convert.ToDateTime(fecha_vencimiento); }
                                else { docupF.VENCIMIENTO = null; }

                                if (facturasConf.FACTURAK == true)
                                { docupF.FACTURAK = facturak; }
                                else
                                { docupF.FACTURAK = "0"; }

                                if (facturasConf.EJERCICIOK == true)
                                { docupF.EJERCICIOK = ejerciciok; }
                                else
                                { docupF.EJERCICIOK = "0"; }

                                docupF.BILL_DOC = "0";
                                docupF.BELNR = "0";
                                docupF.IMPORTE_FAC = 0;
                                docupF.PAYER = "0";

                                dop.DOCUMENTOFs.Add(docupF);
                            }
                        }

                    }
                    //FIN DE LA SECCION DOS

                    //SECCION 3.- VALIDACION DE DATOS DE LA TERCER HOJA DE EXCEL(RELACIONADA MULTIPLE)
                    var num_docH2V = dop.DOCUMENTOFs.Select(x => x.NUM_DOC).FirstOrDefault();
                    if (num_docH2V == 0)
                    {
                        for (int m = 0; m < ds3.Tables[0].Rows.Count; m++)
                        {
                            DOCUMENTOF docupF = new DOCUMENTOF();
                            string num_docH3 = ds3.Tables[0].Rows[m][0].ToString().Trim();
                            string factura = ds3.Tables[0].Rows[m][1].ToString().Trim();
                            string bill_doc = ds3.Tables[0].Rows[m][2].ToString().Trim();
                            string ejerciciok = ds3.Tables[0].Rows[m][3].ToString().Trim();
                            string payer_idH3 = ds3.Tables[0].Rows[m][4].ToString().Trim();
                            string payer_nombreH3 = ds3.Tables[0].Rows[m][5].ToString().Trim();
                            string importe_fac = ds3.Tables[0].Rows[m][6].ToString().Trim();
                            importe_fac = fc.toNum(importe_fac, miles, decimales).ToString();
                            string belnr = ds3.Tables[0].Rows[m][7].ToString().Trim();

                            num_docH3 = num_docH3.TrimStart('0').Trim();

                            if (num_docH3 == num_doc)
                            {
                                var confMultiple = db.TSOLs.Where(x => x.ID == dop.TSOL_ID).FirstOrDefault();
                                string tsolMultiple = "";

                                if (confMultiple != null)
                                {
                                    tsolMultiple = confMultiple.TSOLM;
                                }
                                else
                                {
                                    tsolMultiple = t_sol;
                                }

                                var facturasConf = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == bukrs & x.PAIS_ID == land & x.TSOL == tsolMultiple).FirstOrDefault();
                                if (facturasConf != null)
                                {
                                    docupF.NUM_DOC = Convert.ToDecimal(num_docH3);
                                    docupF.POS = m;

                                    if (facturasConf.FACTURA == true)
                                    { docupF.FACTURA = factura; }
                                    else
                                    { docupF.FACTURA = "0"; }

                                    docupF.FECHA = null;
                                    docupF.PROVEEDOR = "0";
                                    docupF.CONTROL = "0";
                                    docupF.AUTORIZACION = "0";
                                    docupF.VENCIMIENTO = null;
                                    docupF.FACTURAK = "0";

                                    if (facturasConf.EJERCICIOK == true)
                                    { docupF.EJERCICIOK = ejerciciok; }
                                    else
                                    { docupF.EJERCICIOK = "0"; }

                                    if (facturasConf.BILL_DOC == true)
                                    { docupF.BILL_DOC = bill_doc; }
                                    else
                                    { docupF.BILL_DOC = "0"; }

                                    if (facturasConf.BELNR == true)
                                    { docupF.BELNR = belnr; }
                                    else
                                    { docupF.BELNR = "0"; }

                                    if (facturasConf.IMPORTE_FAC == true)
                                    { docupF.IMPORTE_FAC = Convert.ToDecimal(importe_fac); }
                                    else
                                    { docupF.IMPORTE_FAC = 0; }

                                    if (facturasConf.PAYER == true)
                                    { docupF.PAYER = payer_id; }
                                    else
                                    { docupF.PAYER = "0"; }

                                    dop.DOCUMENTOFs.Add(docupF);
                                }
                            }
                        }
                    }
                    //FIN DE LA SECCION 3

                    //SECCION 4.- VALIDACION DE DATOS DE LA CUARTA HOJA DEL EXCEL(DISTRIBUCION)
                    List<MaterialCategoria> listMC = new List<MaterialCategoria>();
                    List<string> listcat = new List<string>();
                    decimal totalcats = 0;
                    string select_dis = "";
                    List<CategoriaMaterial> listcatm = new List<CategoriaMaterial>();

                    ///VALIDAR SI ES POR CATEGORIA
                    for (int k = 0; k < ds4.Tables[0].Rows.Count; k++)
                    {
                        string num_docH4 = ds4.Tables[0].Rows[k][0].ToString().Trim();
                        num_docH4 = num_docH4.TrimStart('0').Trim();

                        if (num_docH4 == num_doc)
                        {
                            MaterialCategoria objMC = new MaterialCategoria();
                            string matnr = ds4.Tables[0].Rows[k][4].ToString().Trim();
                            if (matnr.Length < 18) { matnr = cad.completaMaterial(matnr); }
                            string matklId = ds4.Tables[0].Rows[k][5].ToString().Trim();

                            objMC.Material = matnr;
                            objMC.Categoria = matklId;
                            listMC.Add(objMC);
                            listcat.Add(matklId);

                        }
                    }
                    int totalMC = listMC.Count();
                    int totalCategoria = listMC.FindAll(x => string.IsNullOrEmpty(x.Material) && !string.IsNullOrEmpty(x.Categoria)).Count();

                    if (totalMC == totalCategoria)
                    {
                        listcatm = grupoMaterialesController(listcat, docu.VKORG, docu.SPART, docu.PAYER_ID, docu.SOCIEDAD_ID, out totalcats);
                        select_dis = "C";
                    }



                    for (int k = 0; k < ds4.Tables[0].Rows.Count; k++)
                    {
                        string num_docH4 = ds4.Tables[0].Rows[k][0].ToString().Trim();
                        num_docH4 = num_docH4.TrimStart('0').Trim();

                        if (num_docH4 == num_doc)
                        {
                            DOCUMENTOP docup = new DOCUMENTOP();

                            string ligada = ds4.Tables[0].Rows[k][1].ToString().Trim();
                            string vigencia_de = ds4.Tables[0].Rows[k][2].ToString().Trim();
                            vigencia_de = validaPeriodoFecha(vigencia_de, "x");
                            string vigencia_al = ds4.Tables[0].Rows[k][3].ToString().Trim();
                            vigencia_al = validaPeriodoFecha(vigencia_al, "");
                            string matnr = ds4.Tables[0].Rows[k][4].ToString().Trim();
                            if (matnr.Length < 18) { matnr = cad.completaMaterial(matnr); }
                            string matklId = ds4.Tables[0].Rows[k][5].ToString().Trim();
                            string descripcion = ds4.Tables[0].Rows[k][6].ToString().Trim();
                            string monto = ds4.Tables[0].Rows[k][7].ToString().Trim();
                            monto = fc.toNum(monto, miles, decimales).ToString();
                            string porc_apoyo = ds4.Tables[0].Rows[k][8].ToString().Trim();
                            porc_apoyo = fc.toNum(porc_apoyo, miles, decimales).ToString();
                            string apoyo_pieza = ds4.Tables[0].Rows[k][9].ToString().Trim();
                            apoyo_pieza = fc.toNum(apoyo_pieza, miles, decimales).ToString();
                            string costo_apoyo = ds4.Tables[0].Rows[k][10].ToString().Trim();
                            costo_apoyo = fc.toNum(costo_apoyo, miles, decimales).ToString();
                            string precio_sug = ds4.Tables[0].Rows[k][11].ToString().Trim();
                            precio_sug = fc.toNum(precio_sug, miles, decimales).ToString();
                            string volumen_real = ds4.Tables[0].Rows[k][12].ToString().Trim();
                            volumen_real = fc.toNum(volumen_real, miles, decimales).ToString();
                            string apoyo = ds4.Tables[0].Rows[k][13].ToString().Trim();
                            apoyo = fc.toNum(apoyo, miles, decimales).ToString();

                            DateTime vigencia_de2 = Convert.ToDateTime(vigencia_de);
                            DateTime vigencia_al2 = Convert.ToDateTime(vigencia_al);

                            //AQUI VALIDAMOS SI TIENE REAL
                            //DE NO TENERLO HACE EL CALCULO CON COSTO, PORCENTAJE, PRECIO Y VOLUMEN

                            docup.NUM_DOC = Convert.ToDecimal(num_docH4);
                            docup.POS = k;

                            if (ligada.Trim() != "false")
                            {
                                dop.PORC_APOYO = Convert.ToDecimal(porc_apoyo);
                            }

                            if (matnr != "")
                            {
                                docup.MATNR = matnr;
                                docup.MATKL = "";
                            }
                            else
                            {
                                docup.MATNR = "";
                                docup.MATKL = matklId;
                            }
                            docup.CANTIDAD = 0;
                            docup.MONTO = Convert.ToDecimal(monto);
                            docup.PORC_APOYO = Convert.ToDecimal(porc_apoyo);
                            docup.MONTO_APOYO = 0;///////////////////////////////////
                            docup.PRECIO_SUG = Convert.ToDecimal(precio_sug);
                            docup.VIGENCIA_DE = vigencia_de2;
                            docup.VIGENCIA_AL = vigencia_al2;

                            if (db.TSOLs.Where(x => x.ID == t_sol).FirstOrDefault().FACTURA == true)
                            {
                                docup.VOLUMEN_EST = 0;
                                docup.APOYO_EST = 0;
                                docup.VOLUMEN_REAL = Convert.ToDecimal(volumen_real);
                                docup.APOYO_REAL = Convert.ToDecimal(apoyo);
                                dop.MONTO_DOC_MD += docup.APOYO_REAL;
                                dop.MONTO_DOC_ML = tcambio.getValSoc(sociedad.WAERS, moneda_id, Convert.ToDecimal(apoyo), out errorString);
                                dop.TIPO_CAMBIOL = tcambio.getUkurs(sociedad.WAERS, moneda_id, out errorString);
                                dop.TIPO_CAMBIOL2 = tcambio.getUkursUSD(moneda_id, "USD", out errorString);
                                dop.TIPO_CAMBIO = tcambio.getUkursUSD(moneda_id, "USD", out errorString);
                                dop.MONTO_DOC_ML2 = (dop.MONTO_DOC_MD / dop.TIPO_CAMBIOL2);
                                //var TIPO_CAMBIOL2 = tcambio.getUkursUSD(moneda_id, "USD", out errorString);
                                //dop.MONTO_DOC_ML2 = (TIPO_CAMBIOL2 * dop.MONTO_DOC_MD);
                            }
                            else
                            {
                                docup.VOLUMEN_REAL = 0;
                                docup.APOYO_REAL = 0;
                                docup.VOLUMEN_EST = Convert.ToDecimal(volumen_real);
                                docup.APOYO_EST = Convert.ToDecimal(apoyo);
                                dop.MONTO_DOC_MD += docup.APOYO_EST;
                                dop.MONTO_DOC_ML = tcambio.getValSoc(sociedad.WAERS, moneda_id, Convert.ToDecimal(apoyo), out errorString);
                                dop.TIPO_CAMBIOL = tcambio.getUkurs(sociedad.WAERS, moneda_id, out errorString);
                                dop.TIPO_CAMBIOL2 = tcambio.getUkursUSD(moneda_id, "USD", out errorString);
                                dop.TIPO_CAMBIO = tcambio.getUkursUSD(moneda_id, "USD", out errorString);
                                dop.MONTO_DOC_ML2 = (dop.MONTO_DOC_MD / dop.TIPO_CAMBIOL2);
                                //var TIPO_CAMBIOL2 = tcambio.getUkursUSD(moneda_id, "USD", out errorString);
                                //dop.MONTO_DOC_ML2 = (TIPO_CAMBIOL2 * dop.MONTO_DOC_MD);
                            }
                            //-------------------------------------------------ADD RSG 04.11.2018

                            if (ligada.Trim() != "false" && dop.DOCUMENTORECs.Count == 0)//ES LIGADA
                            {
                                dop.LIGADA = true;
                                dop.TIPO_TECNICO = "P";
                                DOCUMENTOREC drec = new DOCUMENTOREC();
                                drec.NUM_DOC = dop.NUM_DOC;
                                drec.POS = 1;

                                if (drec.MONTO_BASE == null) //RSG 31.05.2018-------------------
                                    drec.MONTO_BASE = 0;
                                if (drec.PORC == null) //RSG 31.05.2018-------------------
                                    drec.PORC = 0;
                                dop.TIPO_RECURRENTE = db.TSOLs.Where(x => x.ID.Equals(dop.TSOL_ID)).FirstOrDefault().TRECU;
                                if (dop.TIPO_RECURRENTE == "1" & dop.LIGADA == true)
                                    dop.TIPO_RECURRENTE = "2";
                                //if (dop.TIPO_RECURRENTE != "1" & dop.OBJETIVOQ == true)
                                //    dop.TIPO_RECURRENTE = "3";
                                Calendario445 cal = new Calendario445();
                                drec.FECHAF = cal.getUltimoDia(dop.FECHAF_VIG.Value.Year, cal.getPeriodo(dop.FECHAF_VIG.Value));
                                drec.FECHAV = drec.FECHAF;

                                drec.FECHAF = cal.getNextLunes((DateTime)drec.FECHAF);
                                drec.EJERCICIO = drec.FECHAV.Value.Year;
                                drec.PERIODO = cal.getPeriodoF(drec.FECHAV.Value);

                                if (drec.PERIODO == 0) drec.PERIODO = 12;
                                if (dop.DOCUMENTORAN != null)
                                {
                                    //foreach (DOCUMENTORAN dran in dOCUMENTO.DOCUMENTORAN.Where(x => x.POS == drec.POS))
                                    //{
                                    //    dran.NUM_DOC = dOCUMENTO.NUM_DOC;
                                    //    drec.DOCUMENTORANs.Add(dran);
                                    //}
                                }
                                else
                                {
                                    DOCUMENTORAN dran = new DOCUMENTORAN();
                                    dran.NUM_DOC = dop.NUM_DOC;
                                    dran.POS = 1;
                                    dran.LIN = 1;
                                    dran.OBJETIVOI = 0;
                                    dran.PORCENTAJE = dop.PORC_APOYO;
                                    drec.DOCUMENTORANs.Add(dran);
                                }
                                drec.PORC = dop.PORC_APOYO;
                                drec.DOC_REF = 0;
                                drec.ESTATUS = "";

                                dop.DOCUMENTORECs.Add(drec);

                                //db.SaveChanges();
                            }
                            //-------------------------------------------------ADD RSG 04.11.2018
                           

                            List<DOCUMENTOM> docml = new List<DOCUMENTOM>();
                            if (docup.MATNR == "")
                            {
                                string col = "";
                                if (Convert.ToDecimal(docup.APOYO_EST) > 0)
                                {
                                    col = "E";
                                }
                                else if (Convert.ToDecimal(docup.APOYO_REAL) > 0)
                                {
                                    col = "R";
                                }
                                docml = addCatItems(listcatm, docu.PAYER_ID, docup.MATKL, docu.SOCIEDAD_ID, docu.NUM_DOC,
                                    Convert.ToInt16(docup.POS), docup.VIGENCIA_DE, docup.VIGENCIA_AL, null, select_dis, totalcats, Convert.ToDecimal(docu.MONTO_DOC_MD), col);
                            }
                            //Obtener apoyo estimado
                            decimal apoyo_esti = 0;
                            decimal apoyo_real = 0;

                            //Validar si es por porcentaje (si es ligada) o por monto 
                            //Categoría por monto
                            if (ligada.Trim() == "false")
                            {
                                //Obtener el apoyo real o estimado para cada material
                                var cantmat = 1;
                                try
                                {
                                    apoyo_esti = Convert.ToDecimal(docup.APOYO_EST) / cantmat;

                                }
                                catch (Exception e)
                                {
                                    apoyo_esti = 0;
                                }

                                try
                                {
                                    apoyo_real = Convert.ToDecimal(docup.APOYO_REAL) / cantmat;

                                }
                                catch (Exception e)
                                {
                                    apoyo_real = 0;
                                }

                                for (int d = 0; d < docml.Count; d++)
                                {
                                    
                                        DOCUMENTOM docM = new DOCUMENTOM();
                                        docM = docml[d];
                                        docM.POS = d + 1;
                                        CategoriaMaterial cm = listcatm.Where(x => x.ID == docup.MATKL).FirstOrDefault();
                                        decimal porc = 0;
                                        if (cm != null)
                                        {
                                            if (cm.TOTALCAT > 0) { porc = cm.MATERIALES.Where(x => x.MATNR == docM.MATNR).FirstOrDefault().VAL / cm.TOTALCAT * 100; }
                                            docM.PORC_APOYO = porc;
                                        }
                                        else if (docup.MATKL == "000")
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

                                    docup.DOCUMENTOMs.Add(docM);
                                }
                            }
                            else
                            {
                                //Categoría por porcentaje
                                for (int cp = 0; cp < docml.Count; cp++)
                                {
                                    
                                        DOCUMENTOM docM = new DOCUMENTOM();
                                        docM = docml[cp];
                                        docM.POS = cp + 1;

                                    docup.DOCUMENTOMs.Add(docM);
                                }

                            }
                            dop.DOCUMENTOPs.Add(docup);
                        }
                    }
                    //FIN DE LA SECCION 4

                    ////SECCION 5.-VALIDAMOS LOS ARCHIVOS RECIBIDOS DE SOPORTE
                    if (archivos.Count() > 0)
                    {
                        List<HttpPostedFileBase> listaArchivos = new List<HttpPostedFileBase>();
                        List<DOCUMENTOA> docupA = new List<DOCUMENTOA>();
                        //VALIDACION DE DATOS DE LA CUARTA HOJA DEL EXCEL (DISTRIBUCION)

                        for (int l = 0; l < ds5.Tables[0].Rows.Count; l++)
                        {
                            string num_docH5 = ds5.Tables[0].Rows[l][0].ToString().Trim();
                            string tipo = ds5.Tables[0].Rows[l][1].ToString().Trim();
                            string nombre_archivo = ds5.Tables[0].Rows[l][2].ToString().Trim();
                            num_docH5 = num_docH5.TrimStart('0').Trim();


                            if (num_docH5 == num_doc)
                            {
                                foreach (HttpPostedFileBase file in archivos)
                                {
                                    var clasefile = "";
                                    try
                                    {
                                        clasefile = tipo;
                                    }
                                    catch (Exception ex)
                                    {
                                        clasefile = "";
                                    }

                                    if (file.ContentLength > 0)
                                    {
                                        string nombre_archivoInList = file.FileName.ToUpper();
                                        string nombre_comparacion = nombre_archivoInList + file.ContentLength;
                                        if (nombre_comparacion == nombre_archivo.ToUpper())
                                        {
                                            tipo = clasefile.ToUpper().Substring(0, 3);
                                            //VERIFICAMOS EL TIPO DE SOPORTE

                                            var exist = docupA.Where(x => x.NUM_DOC == Convert.ToDecimal(num_docH5) && x.CLASE == tipo).FirstOrDefault();

                                            //SI YA EXISTE UN TIPO DE SOPORTE FACTURA NO INSERTAMOS UNO NUEVO
                                            if (exist == null)
                                            {
                                                DOCUMENTOA doc = new DOCUMENTOA();
                                                doc.NUM_DOC = Convert.ToInt32(num_docH5);
                                                doc.POS = l;
                                                doc.TIPO = Path.GetExtension(nombre_archivoInList).Replace(".", "");
                                                try
                                                {
                                                    doc.CLASE = clasefile.ToUpper().Substring(0, 3);
                                                }
                                                catch (Exception e)
                                                {
                                                    doc.CLASE = "";
                                                }

                                                doc.STEP_WF = 1;
                                                doc.USUARIO_ID = u;
                                                doc.PATH = nombre_archivoInList;
                                                doc.ACTIVO = true;

                                                docupA.Add(doc);
                                                listaArchivos.Add(file);
                                            }
                                        }
                                    }
                                }//LLAVE FOREACH
                            }
                        }

                        dop.DOCUMENTOAs = docupA;

                        List<object> archivosToSave = (List<object>)Session["archivosSave"];

                        if (archivosToSave == null)
                        {
                            archivosToSave = new List<object>();
                        }
                        archivosToSave.Add(listaArchivos);
                        Session["archivosSave"] = archivosToSave;
                    }
                    //FIN DE LA SECCION 5

                }//FIN DE LA VALIDACION DOP
                iDs.Add(dop.NUM_DOC);
            }//FIN DEL FOR GLOBAL (CABECERA)

            List<string> li = new List<string>();
            int contadorArchivos = 0;
            int contadorNotas = 0;

            foreach (DOCUMENTO doc in listD)
            {
                int pos = 1;
                foreach (DOCUMENTOP docp in doc.DOCUMENTOPs)
                {
                    docp.POS = pos;
                    pos++;
                }

                pos = 1;
                foreach(DOCUMENTOA docA in doc.DOCUMENTOAs)
                {
                    docA.POS = pos;
                    pos++;
                }
            }

            foreach (DOCUMENTO doc in listD)
            {
                string cierrePeriodo = cierre(doc.SOCIEDAD_ID, doc.TSOL_ID, doc.PERIODO.ToString(), User.Identity.Name);
                if (cierrePeriodo == "X") { 
                    if (doc.DOCUMENTOPs.Count() != 0)
                    {
                        var num_docTemp = doc.NUM_DOC;
                        decimal N_DOC = getSolID(doc.TSOL_ID);
                        doc.NUM_DOC = N_DOC;
                        db.DOCUMENTOes.Add(doc);

                        try
                        {
                            db.SaveChanges();
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
                        guardaArchivos(N_DOC, contadorArchivos);//ALMACENAMOS LOS ARCHIVOS DE SOPORTE
                        contadorArchivos++;
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
                                try
                                {
                                    for (int j = 0; j < ds6.Tables[0].Rows.Count; j++)
                                    {
                                        string num_docNotas = ds6.Tables[0].Rows[j][0].ToString();

                                        if (num_docTemp.ToString() == num_docNotas)
                                        {
                                            f.COMENTARIO = ds6.Tables[0].Rows[j][1].ToString().Trim();
                                        }
                                    }
                                }
                                catch { f.COMENTARIO = ""; }
                                string c = pf.procesa(f, "");
                                FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                                while (c == "1")
                                {
                                    string image = Server.MapPath("~/images/logo_kellogg.png");
                                    Email em = new Email();
                                    string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                                    DOCUMENTO docc = db.DOCUMENTOes.Where(x => x.NUM_DOC == doc.NUM_DOC).First();
                                    string imageFlag = Server.MapPath("~/images/flags/mini/" + docc.PAIS_ID + ".png");
                                    em.enviaMailC(f.NUM_DOC, true, spras_id, UrlDirectory, "Index", image, imageFlag);

                                    if (conta.WORKFP.ACCION.TIPO == "B")
                                    {
                                        WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID & x.VERSION == conta.WF_VERSION & x.POS == conta.WF_POS).FirstOrDefault();
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
                                Estatus es = new Estatus();//RSG 18.09.2018
                                DOCUMENTO docE = db.DOCUMENTOes.Find(f.NUM_DOC);
                                conta.STATUS = es.getEstatus(docE);
                                db.Entry(conta).State = EntityState.Modified;
                                db.SaveChanges();

                                contadorNotas++;
                            }
                        }
                        catch (Exception ee) { }
                    }
                    else
                    {
                        li.Add("<" + doc.NUM_DOC.ToString());
                    }
                }
                else
                {
                    li.Add("periodoCerrado" + doc.NUM_DOC.ToString());
                }
            }

            //TempData["docs_masiva"] = li;

            iDs.Add(li);

            return iDs;
        }

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        public bool validaCorreo(string correo)
        {
            string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if (Regex.IsMatch(correo, expresion))
            {
                if (Regex.Replace(correo, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string validaPeriodoFecha(string date, string tipo)
        {
            Calendario445 cale = new Calendario445();
            DateTime fecha;
            int anio, mes;

            if (date.Length == 7)
            {
                mes = Convert.ToInt32(date.Substring(0, 2));
                anio = Convert.ToInt32(date.Substring(3, 4));

                if (tipo != "")
                {
                    fecha = cale.getPrimerDia(anio, mes);
                }
                else
                {
                    fecha = cale.getUltimoDia(anio, mes);
                }
            }
            else
            {
                fecha = Convert.ToDateTime(date);
            }

            return Convert.ToString(fecha);
        }

        public bool validaFormatoFecha(string fecha)
        {
            DateTime parsedDate;

            if (DateTime.TryParseExact(fecha, "dd-MM-yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
            else if (DateTime.TryParseExact(fecha, "dd/MM/yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
            else if (DateTime.TryParseExact(fecha, "MM-yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
            else if (DateTime.TryParseExact(fecha, "MM/yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool validaFormatoFechaH2(string fecha)
        {
            DateTime parsedDate;

            if (DateTime.TryParseExact(fecha, "dd-MM-yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
            else if (DateTime.TryParseExact(fecha, "dd/MM/yyyy", null, DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public resultadoFechas validaRangoFecha(string fecha1, string fecha2)
        {
            resultadoFechas resultado = new resultadoFechas();
            bool fecha1_1, fecha2_2 = false;
            int tamanio = 0, tamanio2 = 0;

            if (fecha1.Length <= 10)
            {
                if (validaFormatoFecha(fecha1))
                {
                    fecha1_1 = true;
                    tamanio = fecha1.Length;
                }
                else
                {
                    fecha1_1 = false;
                }
            }
            else
            {
                fecha1_1 = false;
            }

            if (fecha2.Length <= 10)
            {
                if (validaFormatoFecha(fecha2))
                {
                    fecha2_2 = true;
                    tamanio2 = fecha2.Length;
                }
                else
                {
                    fecha2_2 = false;
                }
            }
            else
            {
                fecha2_2 = false;
            }

            if (fecha1_1 && fecha2_2)
            {
                DateTime fec1, fec2;
                if (tamanio == 7 || tamanio2 == 7)
                {
                    fecha1 = validaPeriodoFecha(fecha1, "x");
                    fecha2 = validaPeriodoFecha(fecha2, "");

                    fec1 = Convert.ToDateTime(fecha1);
                    fec2 = Convert.ToDateTime(fecha2);

                    if (fec1 <= fec2)
                    {
                        resultado.status = true;
                        resultado.tipo = "Rango";
                    }
                    else
                    {
                        resultado.status = false;
                        resultado.tipo = "Rango";
                    }
                }
                else
                {
                    fec1 = Convert.ToDateTime(fecha1);
                    fec2 = Convert.ToDateTime(fecha2);

                    if (fec1 <= fec2)
                    {
                        resultado.status = true;
                        resultado.tipo = "Rango";
                    }
                    else
                    {
                        resultado.status = false;
                        resultado.tipo = "Rango";
                    }
                }
            }
            else
            {
                resultado.status = false;
                resultado.tipo = "Formato";
            }

            return resultado;
        }

        public string validaPeso(string cantidad, bool valor)
        {
            if (cantidad != "")
            {
                if (valor)
                {
                    cantidad = "$" + cantidad;
                }
                else
                {
                    cantidad = "%" + cantidad;
                }
            }
            else
            {
                cantidad = "";
            }

            return cantidad;
        }

        public DataTable ConvertToDatatable(List<object> list, string hoja)
        {
            DataTable dt = new DataTable();

            if (hoja == "h1")
            {
                DataSet ds1 = (DataSet)Session["ds1"];
                for (int i = 0; i < 1; i++)
                {
                    dt.Columns.Add(ds1.Tables[0].Rows[i][0].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][1].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][2].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][3].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][4].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][5].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][6].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][7].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][8].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][9].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][10].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][11].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][12].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][13].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][14].ToString().Trim());
                    dt.Columns.Add(ds1.Tables[0].Rows[i][15].ToString().Trim());
                }

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string[] miArreglo = (string[])item;
                        dt.Rows.Add(miArreglo);
                    }
                }
            }

            else if (hoja == "h2")
            {
                DataSet ds2 = (DataSet)Session["ds2"];
                for (int i = 0; i < 1; i++)
                {
                    dt.Columns.Add(ds2.Tables[0].Rows[i][0].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][1].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][2].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][3].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][4].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][5].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][6].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][7].ToString().Trim());
                    dt.Columns.Add(ds2.Tables[0].Rows[i][8].ToString().Trim());
                }

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string[] miArreglo = (string[])item;
                        dt.Rows.Add(miArreglo);
                    }
                }
            }

            else if (hoja == "h3")
            {
                DataSet ds3 = (DataSet)Session["ds3"];
                for (int i = 0; i < 1; i++)
                {
                    dt.Columns.Add(ds3.Tables[0].Rows[i][0].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][1].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][2].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][3].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][4].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][5].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][6].ToString().Trim());
                    dt.Columns.Add(ds3.Tables[0].Rows[i][7].ToString().Trim());
                }

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string[] miArreglo = (string[])item;
                        dt.Rows.Add(miArreglo);
                    }
                }
            }

            else if (hoja == "h4")
            {
                DataSet ds4 = (DataSet)Session["ds4"];
                for (int i = 0; i < 1; i++)
                {
                    dt.Columns.Add(ds4.Tables[0].Rows[i][0].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][1].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][2].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][3].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][4].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][5].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][6].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][7].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][8].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][9].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][10].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][11].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][12].ToString().Trim());
                    dt.Columns.Add(ds4.Tables[0].Rows[i][13].ToString().Trim());
                }

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string[] miArreglo = (string[])item;
                        dt.Rows.Add(miArreglo);
                    }
                }
            }

            else if (hoja == "h5")
            {
                DataSet ds5 = new DataSet();
                for (int i = 0; i < 1; i++)
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("TIPO");
                    dt.Columns.Add("NOMBRE");
                }

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string[] miArreglo = (string[])item;
                        string contenido = miArreglo[0];
                        string[] miArreglo2 = contenido.Split('*');
                        dt.Rows.Add(miArreglo2[0], miArreglo2[1], miArreglo2[2]);
                    }
                }
            }

            else if (hoja == "notas")
            {
                DataSet ds6 = new DataSet();
                for (int i = 0; i < 1; i++)
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("NOTA");
                }

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        string nota = item.ToString();
                        string[] contenido = nota.Split('*');
                        dt.Rows.Add(contenido[0], contenido[1]);
                    }
                }
            }

            return dt;
        }

        public List<DOCUMENTOA> subeArchivo(IEnumerable<HttpPostedFileBase> archivos, string u, string t_sol, DataSet ds5, decimal num_doc)
        {
            List<HttpPostedFileBase> archivos2 = new List<HttpPostedFileBase>();
            List<DOCUMENTOA> docupA = new List<DOCUMENTOA>();
            int numFiles = 0;

            //REVISAR SI HAY ARCHIVOS POR SUBIR
            try
            {
                foreach (HttpPostedFileBase file in archivos)
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

            for (int i = 1; i < ds5.Tables[0].Rows.Count; i++)
            {
                string cadena = ds5.Tables[0].Rows[i][0].ToString().Trim();
                string[] cadena2 = cadena.Split('*');
                string num_docH5 = cadena2[0];
                string tipo = cadena2[1];
                num_docH5 = num_docH5.TrimStart('0').Trim();


                if (num_docH5 == num_doc.ToString())
                {
                    if (numFiles > 0)
                    {
                        foreach (HttpPostedFileBase file in archivos)
                        {
                            var clasefile = "";
                            try
                            {
                                clasefile = tipo;
                            }
                            catch (Exception ex)
                            {
                                clasefile = "";
                            }

                            if (file != null)
                            {
                                if (file.ContentLength > 0)
                                {
                                    string nombre_archivo = file.FileName.ToUpper();
                                    tipo = clasefile.ToUpper().Substring(0, 3);
                                    //VERIFICAMOS EL TIPO DE SOPORTE
                                    if (tipo == "FAC")
                                    {
                                        var exist = docupA.Where(x => x.NUM_DOC == Convert.ToDecimal(num_docH5) & x.CLASE == tipo).FirstOrDefault();

                                        //SI YA EXISTE UN TIPO DE SOPORTE FACTURA NO INSERTAMOS UNO NUEVO
                                        if (exist == null)
                                        {
                                            DOCUMENTOA doc = new DOCUMENTOA();
                                            doc.NUM_DOC = Convert.ToInt32(num_docH5);
                                            doc.POS = i;
                                            doc.TIPO = Path.GetExtension(nombre_archivo).Replace(".", "");
                                            try
                                            {
                                                doc.CLASE = clasefile.ToUpper().Substring(0, 3);
                                            }
                                            catch (Exception e)
                                            {
                                                doc.CLASE = "";
                                            }

                                            doc.STEP_WF = 1;
                                            doc.USUARIO_ID = u;
                                            doc.PATH = nombre_archivo;
                                            doc.ACTIVO = true;

                                            docupA.Add(doc);
                                            archivos2.Add(file);
                                        }
                                    }
                                    else
                                    {
                                        DOCUMENTOA doc = new DOCUMENTOA();
                                        doc.NUM_DOC = Convert.ToInt32(num_docH5);
                                        doc.POS = i;
                                        doc.TIPO = Path.GetExtension(nombre_archivo).Replace(".", "");
                                        try
                                        {
                                            doc.CLASE = clasefile.ToUpper().Substring(0, 3);
                                        }
                                        catch (Exception e)
                                        {
                                            doc.CLASE = "";
                                        }

                                        doc.STEP_WF = 1;
                                        doc.USUARIO_ID = u;
                                        doc.PATH = nombre_archivo;
                                        doc.ACTIVO = true;

                                        docupA.Add(doc);
                                        archivos2.Add(file);
                                    }

                                }
                            }
                        }//LLAVE FOREACH
                    }//IF NUMERO DE ARCHUVOS MAYOR A 0
                }
            }//LLAVE DE FOR PARA LOS REGISTROS DEL EXCEL
            List<HttpPostedFileBase> archivosRENew = (List<HttpPostedFileBase>)Session["arcReales"];

            if (archivosRENew == null)
            {
                archivosRENew = new List<HttpPostedFileBase>();
            }
            archivosRENew.AddRange(archivos2);
            Session["arcReales"] = archivosRENew;
            return docupA;
        }

        public void guardaArchivos(decimal num_doc, int contador)
        {
            //IEnumerable<HttpPostedFileBase> archivosToSave = (IEnumerable<HttpPostedFileBase>)Session["archivosSave"];
            List<object> archivosToSave = (List<object>)Session["archivosSave"];
            if (archivosToSave != null)//ADD RSG 01.11.2018
            {
                object[] arrArchivostoSave = new object[archivosToSave.Count];
                int indice = 0;

                foreach (List<HttpPostedFileBase> listFile in archivosToSave)
                {
                    arrArchivostoSave[indice] = listFile.ToArray();
                    indice++;
                }

                HttpPostedFileBase[] arregloCopia = new HttpPostedFileBase[contador];
                arregloCopia = (HttpPostedFileBase[])arrArchivostoSave[contador];

                List<HttpPostedFileBase> archivosSaveFinal = new List<HttpPostedFileBase>();
                for (int i = 0; i < arregloCopia.Length; i++)
                {
                    archivosSaveFinal.Add(arregloCopia[i]);
                }

                string errorString = "";
                var res = "";
                string errorMessage = "";
                int numFiles = 0;

                try
                {
                    foreach (HttpPostedFileBase file in archivosSaveFinal)
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
                    string url = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") && x.ACTIVO).FirstOrDefault().VALUE;
                    //string url = "C:\\Users\\EQUIPO\\Desktop\\Nueva carpeta\\";
                    var dir = new Files().createDir(url, num_doc.ToString(), DateTime.Now.Year.ToString());

                    //Evaluar que se creo el directorio
                    if (dir.Equals(""))
                    {
                        foreach (HttpPostedFileBase file in archivosSaveFinal)
                        {
                            string errorfiles = "";
                            string path = "";
                            errorfiles = "";
                            res = new Files().SaveFile(file, url, num_doc.ToString(), out errorfiles, out path, DateTime.Now.Year.ToString());

                            var cambio = db.DOCUMENTOAs.Where(x => x.NUM_DOC == num_doc & x.PATH == file.FileName).FirstOrDefault();
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
            }//ADD RSG 01.11.2018
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

        public JsonResult ValidarCategoriasMateriles(List<MaterialCategoria> materialCategorias)
        {

            if (materialCategorias.Count() > 0)
            {

                foreach (MaterialCategoria objMC in materialCategorias)
                {
                    bool unico = false;
                    objMC.Error = false;
                    objMC.Msj = "";
                    objMC.Unico = false;
                    if (!string.IsNullOrEmpty(objMC.Material))
                    {

                        if (objMC.Material.Length < 18) { objMC.Material = cad.completaMaterial(objMC.Material); }
                        var MATERIALGP_ID = db.MATERIALs.Where(x => x.ID == objMC.Material).FirstOrDefault().MATERIALGP_ID;
                        unico = db.MATERIALGPs.Where(x => x.ID == MATERIALGP_ID).FirstOrDefault().UNICA;
                        objMC.Unico = unico;
                    }
                    else if (string.IsNullOrEmpty(objMC.Material) && !string.IsNullOrEmpty(objMC.Categoria))
                    {
                        unico = db.MATERIALGPs.Where(x => x.ID == objMC.Categoria).FirstOrDefault().UNICA;
                        objMC.Unico = unico;
                    }



                }
                foreach (MaterialCategoria objMC in materialCategorias)
                {
                    int totalCategoria = materialCategorias.FindAll(x => string.IsNullOrEmpty(x.Material) && !string.IsNullOrEmpty(x.Categoria)).Count();
                    int totalMateriales = materialCategorias.FindAll(x => !string.IsNullOrEmpty(x.Material)).Count();
                    int totalHealty = materialCategorias.FindAll(x => x.Unico).Count();
                    if (!string.IsNullOrEmpty(objMC.Material))
                    {
                        if (totalMateriales > 0 && totalCategoria > 0)
                        {
                            objMC.Error = true;
                            objMC.Msj = " Los materiales no se pueden mezclar con categorias";
                        }
                        else if (totalMateriales > 1 && totalHealty > 0)
                        {
                            objMC.Error = true;
                            objMC.Msj = " Las categorías unicas no se pueden mezclar con otras categorias y/o Materiales";
                        }
                        else
                        {
                            objMC.Error = false;
                            objMC.Msj = "";
                        }

                    }
                    else if (string.IsNullOrEmpty(objMC.Material) && !string.IsNullOrEmpty(objMC.Categoria))
                    {

                        int totalCategoriasRepetidas = materialCategorias.FindAll(x => x.Categoria == objMC.Categoria).Count();

                        if (totalMateriales > 0 && totalCategoria > 0)
                        {
                            objMC.Error = true;
                            objMC.Msj = " Los materiales no se pueden mezclar con categorias";
                        }
                        else if (totalCategoria > 1 && totalHealty > 0)
                        {
                            objMC.Error = true;
                            objMC.Msj = " Las categorías unicas no se pueden mezclar con otras categorias y/o Materiales";
                        }
                        else if (totalCategoriasRepetidas > 1)
                        {
                            objMC.Error = true;
                            objMC.Msj = "Una categoría no puede ser agregada más de una vez en la misma solicitud";
                        }
                        else
                        {
                            objMC.Error = false;
                            objMC.Msj = "";
                        }
                    }

                }


            }

            JsonResult list = Json(materialCategorias, JsonRequestBehavior.AllowGet);
            return list;
        }

        public List<DataRow> getRowsByDocument(DataTable Pestania, string NumberContract, int ColumnNumber = 0)
        {
            List<IGrouping<bool, DataRow>> Result;
            List<DataRow> Response = new List<DataRow>();

            if (Pestania == null || Pestania.Rows.Count <= 0)
                return Response;

            int con = 0;
            foreach (DataRow aux in Pestania.Rows)
            {
                if (con > 1)
                    aux.SetField(ColumnNumber, getNumberDocument(aux[ColumnNumber].ToString().Trim()).ToString());

                con++;
            }

            string ColumnName = string.Empty;
            int TempNumDoc = getNumberDocument(NumberContract);

            if (TempNumDoc == -1)
                return Response;

            ColumnName = Pestania.Columns[ColumnNumber].ColumnName;

            if (string.IsNullOrEmpty(ColumnName))
                return Response;

            Result = Pestania.AsEnumerable().GroupBy(p => p[ColumnName].ToString() == TempNumDoc.ToString())?.ToList();

            if (Result == null || Result.Count != 2 || Result.Last()?.First() == null)
                return Response;

            Response = Result.Last().ToList();
            return Response;
        }

        public int getNumberDocument(string numDocument)
        {
            numDocument = numDocument.Trim();
            int TempNumDoc = -1;
            if (IsNumeric(numDocument))
            {
                 TempNumDoc = int.Parse(Math.Truncate(string.IsNullOrEmpty(numDocument) ? -1 : double.Parse(numDocument)).ToString());
            }
            return TempNumDoc;
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
                matl = db.MATERIALs.Where(m => m.ACTIVO == true);
            }
            catch (Exception e)
            {
                matl = null;
            }

            //Validar si hay materiales
            if (matl != null)
            {

                CLIENTE cli = new CLIENTE();
                List<CLIENTE> clil = new List<CLIENTE>();

                try
                {
                    cli = db.CLIENTEs.Where(c => c.KUNNR == kunnr & c.VKORG == vkorg & c.SPART == spart).FirstOrDefault();

                    //Saber si el cliente es sold to, payer o un grupo
                    if (cli != null)
                    {
                        //Es un soldto
                        if (cli.KUNNR != cli.PAYER && cli.KUNNR != cli.BANNER)
                        {
                            clil.Add(cli);
                        }
                    }
                }
                catch (Exception e)
                {
                    cli = null;
                }

                var cie = clil.Cast<CLIENTE>();
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
                    aii = 0;
                }

                int mii = 0;
                try
                {
                    mii = Convert.ToInt32(mi);
                }
                catch (Exception e)
                {
                    mii = 0;
                }

                int aff = 0;
                try
                {
                    aff = Convert.ToInt32(af);
                }
                catch (Exception e)
                {
                    aff = 0;
                }

                int mff = 0;
                try
                {
                    mff = Convert.ToInt32(mf);
                }
                catch (Exception e)
                {
                    mff =0;
                }

                 if (cie != null)
                    {
                        jd = materialesgptDao.ListaMaterialGroupsMateriales(vkorg, spart, kunnr, soc_id, aii, mii, aff, mff, User.Identity.Name);
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

        public List<DOCUMENTOM> addCatItems(List<CategoriaMaterial> categorias, string kunnr, string catid,
           string soc_id, decimal numdoc, int posid, DateTime? vig_de, DateTime? vig_a, string neg, string dis, decimal total, decimal totaldoc, string col)
        {
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
                        por = 0;
                    }
                    decimal totalmat = 0;

                    try
                    {
                        totalmat = Convert.ToDecimal((por * totaldoc) / 100);
                    }
                    catch (Exception)
                    {
                        totalmat = 0;
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

        [HttpPost]
        public JsonResult DescargarExcel(List<object> h1, List<object> h2, List<object> h3, List<object> h4)
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }

            DataTable dtt = ConvertToDatatable(h1, "h1");
            DataTable dtt2 = ConvertToDatatable(h2, "h2");
            DataTable dtt3 = ConvertToDatatable(h3, "h3");
            DataTable dtt4 = ConvertToDatatable(h4, "h4");

            //////CABECERA
            DataSet dt1 = new DataSet();
            dt1.Tables.Add(dtt);

            /////DOCUMENTOS RELACIONADOS
            DataSet dt2 = new DataSet();
            dt2.Tables.Add(dtt2);

            /////DOCUMENTOS RELACIONADOS MULTIPLES
            DataSet dt3 = new DataSet();
            dt3.Tables.Add(dtt3);

            /////MATERIALES
            DataSet dt4 = new DataSet();
            dt4.Tables.Add(dtt4);
            string path = Server.MapPath("~/PdfTemp/DocMasivas/" + User.Identity.Name + ".xlsx");
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            generarExcelHome(dt1, dt2, dt3, dt4, Server.MapPath("~/PdfTemp/"));
            JsonResult result = Json(path, JsonRequestBehavior.AllowGet);
            return result;
        }

        public void generarExcelHome(DataSet dt1, DataSet dt2, DataSet dt3, DataSet dt4, string ruta)
        { 

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("INFORMACIÓN");
            var WksRelacionada = workbook.Worksheets.Add("RELACIONADA");
            var WksFacturasmul = workbook.Worksheets.Add("FACTURAS MÚLTIPLES");
            var WksDistribucion = workbook.Worksheets.Add("DISTRIBUCIÓN");
            try
            {
                //Creamos el encabezado informacion
                worksheet.Cell("A1").Value = new[]{new {BANNER = "Número de Documento" } };
                worksheet.Cell("B1").Value = new[]{new {BANNER = "Tipo de Solicitud" } };
                worksheet.Cell("C1").Value = new[] { new { BANNER = "Clasificación" } };
                worksheet.Cell("D1").Value = new[] { new { BANNER = "Company Code" } };
                worksheet.Cell("E1").Value = new[] { new { BANNER = "País" } };
                worksheet.Cell("F1").Value = new[] { new { BANNER = "Estado" } };
                worksheet.Cell("G1").Value = new[] { new { BANNER = "Ciudad" } };
                worksheet.Cell("H1").Value = new[] { new { BANNER = "Concepto" } };
                worksheet.Cell("I1").Value = new[] { new { BANNER = "Mecánica de Negociación" } };
                worksheet.Cell("J1").Value = new[] { new { BANNER = "Cliente" } };
                worksheet.Cell("K1").Value = new[] { new { BANNER = "Nombre" } };
                worksheet.Cell("L1").Value = new[] { new { BANNER = "Nombre de Contacto" } };
                worksheet.Cell("M1").Value = new[] { new { BANNER = "Email de Contacto" } };
                worksheet.Cell("N1").Value = new[] { new { BANNER = "Vigencia (dd/mm/aaaa) o Periodo (mm-aaaa) De" } };
                worksheet.Cell("O1").Value = new[] { new { BANNER = "Vigencia (dd/mm/aaaa) o Periodo (mm-aaaa) A" } };
                worksheet.Cell("P1").Value = new[] { new { BANNER = "Moneda" } };

                worksheet.Cell("A" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("B" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("C" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("D" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("E" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("F" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("G" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("H" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("I" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("J" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("K" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("L" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("M" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("N" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("O" + 2).Value = new[] { new { BANNER = "" } };
                worksheet.Cell("P" + 2).Value = new[] { new { BANNER = "" } };
                int cInf= 3;
                for (int i = 0; i <dt1.Tables[0].Rows.Count; i++)
                {
                    DataRow Row = dt1.Tables[0].Rows[i];
                    worksheet.Cell("A" + cInf).Value = new[] { new { BANNER = Row[0].ToString() } };
                    worksheet.Cell("B" + cInf).Value = new[] { new { BANNER = Row[1].ToString() } };
                    worksheet.Cell("C" + cInf).Value = new[] { new { BANNER = Row[2].ToString() } };
                    worksheet.Cell("D" + cInf).Value = new[] { new { BANNER = Row[3].ToString() } };
                    worksheet.Cell("E" + cInf).Value = new[] { new { BANNER = Row[4].ToString() } };
                    worksheet.Cell("F" + cInf).Value = new[] { new { BANNER = Row[5].ToString() } };
                    worksheet.Cell("G" + cInf).Value = new[] { new { BANNER = Row[6].ToString() } };
                    worksheet.Cell("H" + cInf).Value = new[] { new { BANNER = Row[7].ToString() } };
                    worksheet.Cell("I" + cInf).Value = new[] { new { BANNER = Row[8].ToString() } };
                    worksheet.Cell("J" + cInf).Value = new[] { new { BANNER = Row[9].ToString() } };
                    worksheet.Cell("K" + cInf).Value = new[] { new { BANNER = Row[10].ToString() } };
                    worksheet.Cell("L" + cInf).Value = new[] { new { BANNER = Row[11].ToString() } };
                    worksheet.Cell("M" + cInf).Value = new[] { new { BANNER = Row[12].ToString() } };
                    worksheet.Cell("N" + cInf).Value = new[] { new { BANNER = Row[13].ToString() } };
                    worksheet.Cell("O" + cInf).Value = new[] { new { BANNER = Row[14].ToString() } };
                    worksheet.Cell("P" + cInf).Value = new[] { new { BANNER = Row[15].ToString() } };
                    cInf++;
                }

                //Creamos el encabezado Relacionada
                WksRelacionada.Cell("A1").Value = new[] { new { BANNER = "Número de Documento" } };
                WksRelacionada.Cell("B1").Value = new[] { new { BANNER = "Factura" } };
                WksRelacionada.Cell("C1").Value = new[] { new { BANNER = "Fecha Factura" } };
                WksRelacionada.Cell("D1").Value = new[] { new { BANNER = "Proveedor" } };
                WksRelacionada.Cell("E1").Value = new[] { new { BANNER = "Nombre del Proveedor" } };
                WksRelacionada.Cell("F1").Value = new[] { new { BANNER = "Número Autorización" } };
                WksRelacionada.Cell("G1").Value = new[] { new { BANNER = "Fecha Vencimiento" } };
                WksRelacionada.Cell("H1").Value = new[] { new { BANNER = "Factura Kellogg" } };
                WksRelacionada.Cell("I1").Value = new[] { new { BANNER = "Ejercicio Kellogg" } };

                WksRelacionada.Cell("A" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("B" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("C" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("D" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("E" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("F" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("G" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("H" + 2).Value = new[] { new { BANNER = "" } };
                WksRelacionada.Cell("I" + 2).Value = new[] { new { BANNER = "" } };
                int cRel = 3;
                for (int i = 0; i <dt2.Tables[0].Rows.Count; i++)
                {
                    DataRow Row = dt2.Tables[0].Rows[i];
                    WksRelacionada.Cell("A" + cRel).Value = new[] { new { BANNER = Row[0].ToString() } };
                    WksRelacionada.Cell("B" + cRel).Value = new[] { new { BANNER = Row[1].ToString() } };
                    WksRelacionada.Cell("C" + cRel).Value = new[] { new { BANNER = Row[2].ToString() } };
                    WksRelacionada.Cell("D" + cRel).Value = new[] { new { BANNER = Row[3].ToString() } };
                    WksRelacionada.Cell("E" + cRel).Value = new[] { new { BANNER = Row[4].ToString() } };
                    WksRelacionada.Cell("F" + cRel).Value = new[] { new { BANNER = Row[5].ToString() } };
                    WksRelacionada.Cell("G" + cRel).Value = new[] { new { BANNER = Row[6].ToString() } };
                    WksRelacionada.Cell("H" + cRel).Value = new[] { new { BANNER = Row[7].ToString() } };
                    WksRelacionada.Cell("I" + cRel).Value = new[] { new { BANNER = Row[8].ToString() } };
                    cRel++;
                }

                //Creamos el encabezado Facturas Multiples 
                WksFacturasmul.Cell("A1").Value = new[] { new { BANNER = "Número de Documento" } };
                WksFacturasmul.Cell("B1").Value = new[] { new { BANNER = "No. Factura Fiscal" } };
                WksFacturasmul.Cell("C1").Value = new[] { new { BANNER = "Billing" } };
                WksFacturasmul.Cell("D1").Value = new[] { new { BANNER = "Año" } };
                WksFacturasmul.Cell("E1").Value = new[] { new { BANNER = "Payer" } };
                WksFacturasmul.Cell("F1").Value = new[] { new { BANNER = "Customer" } };
                WksFacturasmul.Cell("G1").Value = new[] { new { BANNER = "Importe por factura" } };
                WksFacturasmul.Cell("H1").Value = new[] { new { BANNER = "Folio" } };

                WksFacturasmul.Cell("A" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("B" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("C" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("D" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("E" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("F" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("G" + 2).Value = new[] { new { BANNER = "" } };
                WksFacturasmul.Cell("H" + 2).Value = new[] { new { BANNER = "" } };
                int cFac = 3;
                for (int i = 0; i < dt3.Tables[0].Rows.Count; i++)
                {
                    DataRow Row = dt3.Tables[0].Rows[i];
                    string importeFactura = Row[6].ToString() != "" ? Row[6].ToString().Replace("$", "") : "";
                    WksFacturasmul.Cell("A" + cFac).Value = new[] { new { BANNER = Row[0].ToString() } };
                    WksFacturasmul.Cell("B" + cFac).Value = new[] { new { BANNER = Row[1].ToString() } };
                    WksFacturasmul.Cell("C" + cFac).Value = new[] { new { BANNER = Row[2].ToString() } };
                    WksFacturasmul.Cell("D" + cFac).Value = new[] { new { BANNER = Row[3].ToString() } };
                    WksFacturasmul.Cell("E" + cFac).Value = new[] { new { BANNER = Row[4].ToString() } };
                    WksFacturasmul.Cell("F" + cFac).Value = new[] { new { BANNER = Row[5].ToString() } };
                    WksFacturasmul.Cell("G" + cFac).Value = new[] { new { BANNER = importeFactura } };
                    WksFacturasmul.Cell("H" + cFac).Value = new[] { new { BANNER = Row[7].ToString() } };
                    cFac++;
                }

                //Creamos el encabezado distribucion
                WksDistribucion.Cell("A1").Value = new[] { new { BANNER = "Número de Documento" } };
                WksDistribucion.Cell("B1").Value = new[] { new { BANNER = "Ligada a la facturación" } };
                WksDistribucion.Cell("C1").Value = new[] { new { BANNER = "Vigencia de" } };
                WksDistribucion.Cell("D1").Value = new[] { new { BANNER = "Vigencia al" } };
                WksDistribucion.Cell("E1").Value = new[] { new { BANNER = "Material" } };
                WksDistribucion.Cell("F1").Value = new[] { new { BANNER = "Categoría" } };
                WksDistribucion.Cell("G1").Value = new[] { new { BANNER = "Descripción" } };
                WksDistribucion.Cell("H1").Value = new[] { new { BANNER = "Costo Unitario $" } };
                WksDistribucion.Cell("I1").Value = new[] { new { BANNER = "% de Apoyo" } };
                WksDistribucion.Cell("J1").Value = new[] { new { BANNER = "Apoyo por pieza $" } };
                WksDistribucion.Cell("K1").Value = new[] { new { BANNER = "Costo con Apoyo $" } };
                WksDistribucion.Cell("L1").Value = new[] { new { BANNER = "Precio Sugerido $" } };
                WksDistribucion.Cell("M1").Value = new[] { new { BANNER = "Volumen" } };
                WksDistribucion.Cell("N1").Value = new[] { new { BANNER = "Apoyo $" } };

                WksDistribucion.Cell("A" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("B" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("C" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("D" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("E" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("F" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("G" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("H" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("I" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("J" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("K" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("L" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("M" + 2).Value = new[] { new { BANNER = "" } };
                WksDistribucion.Cell("N" + 2).Value = new[] { new { BANNER = "" } };
                int cDis = 3;

                for (int i = 0; i < dt4.Tables[0].Rows.Count; i++)
                {
                    DataRow Row = dt4.Tables[0].Rows[i];
                    string ligada = Row[1].ToString();
                    string costoUnitario = Row[7].ToString() != "" ? Row[7].ToString().Replace("$", ""):"";
                    string porcentajeApoyo = Row[8].ToString() != "" ? Row[8].ToString().Replace("%", "") : "";
                    if (porcentajeApoyo != "")
                    {
                        if (IsNumeric(porcentajeApoyo))
                        {
                            porcentajeApoyo = (Convert.ToDecimal(porcentajeApoyo) /100).ToString();
                        }
                        else
                        {
                            porcentajeApoyo = "";
                        }
                        
                    }
                    
                  
                    string apoyoPieza = Row[9].ToString() != "" ? Row[9].ToString().Replace("$", "") : "";
                    string costoApoyo = Row[10].ToString() != "" ? Row[10].ToString().Replace("$", "") : "";
                    string precioSugerido = Row[11].ToString() != "" ? Row[11].ToString().Replace("$", "") : "";
                    string apoyo = Row[13].ToString() != "" ? Row[13].ToString().Replace("$", "") : "";
                    if (ligada == "true")
                    {
                        ligada = "x";
                    }
                    else
                    {
                        ligada = "";
                    }

                    WksDistribucion.Cell("A" + cDis).Value = new[] { new { BANNER = Row[0].ToString() } };
                    WksDistribucion.Cell("B" + cDis).Value = new[] { new { BANNER = ligada } };
                    WksDistribucion.Cell("C" + cDis).Value = new[] { new { BANNER = Row[2].ToString() } };
                    WksDistribucion.Cell("D" + cDis).Value = new[] { new { BANNER = Row[3].ToString() } };
                    WksDistribucion.Cell("E" + cDis).Value = new[] { new { BANNER = Row[4].ToString() } };
                    WksDistribucion.Cell("F" + cDis).Value = new[] { new { BANNER = Row[5].ToString() } };
                    WksDistribucion.Cell("G" + cDis).Value = new[] { new { BANNER = Row[6].ToString() } };
                    WksDistribucion.Cell("H" + cDis).Value = new[] { new { BANNER = costoUnitario } };
                    WksDistribucion.Cell("I" + cDis).Value = new[] { new { BANNER = porcentajeApoyo } };
                    WksDistribucion.Cell("J" + cDis).Value = new[] { new { BANNER = apoyoPieza } };
                    WksDistribucion.Cell("K" + cDis).Value = new[] { new { BANNER = costoApoyo } };
                    WksDistribucion.Cell("L" + cDis).Value = new[] { new { BANNER = precioSugerido } };
                    WksDistribucion.Cell("M" + cDis).Value = new[] { new { BANNER = Row[12].ToString() } };
                    WksDistribucion.Cell("N" + cDis).Value = new[] { new { BANNER = apoyo } };
                    cDis++;
                }

                var rt = ruta + @"\DocMasivas\" + User.Identity.Name + ".xlsx";
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }

        }
        public string cierre(string sociedad_id, string tsol_id, string periodo_id, string usuario_id)
        {
            int periodo = int.Parse(periodo_id);
            bool a = FnCommon.ValidarPeriodoEnCalendario445(db, sociedad_id, tsol_id, periodo, "PRE", usuario_id) ||
                FnCommon.ValidarPeriodoEnCalendario445(db, sociedad_id, tsol_id, periodo, "CI");
            if (a)
                return "X";
            else
                return "";
        }
        public JsonResult validarFechasDis(string FechaIniDis, string FechaFinDis, string FechaInfoIni, string FechaInfoFin)
        {
            List<string> regresaRowH4 = new List<string>();
            resultadoFechas FechasDistribucion = new resultadoFechas();
            FechasDistribucion = validaRangoFecha(FechaIniDis, FechaFinDis);

            if (FechasDistribucion.status)
            {
                resultadoFechas FechaIniInfoDis = new resultadoFechas();
                FechaIniInfoDis = validaRangoFecha(FechaInfoIni, FechaIniDis);

                if (!FechaIniInfoDis.status)
                {
                    if (FechaIniInfoDis.tipo == "Rango")
                    {
                        regresaRowH4.Add("red white-text rojo |  No debe ser menor a la fecha inicial de la solicitud");
                    }
                    else
                    {
                        regresaRowH4.Add("red white-text rojo |  Formato invalido");
                    }
                }
                else
                {
                    regresaRowH4.Add("");
                }

                resultadoFechas FechaFinInfoDis = new resultadoFechas();
                FechaFinInfoDis = validaRangoFecha(FechaFinDis, FechaInfoFin);

                if (!FechaFinInfoDis.status)
                {
                    if (FechaIniInfoDis.tipo == "Rango")
                    {
                        regresaRowH4.Add("red white-text rojo | No debe ser mayor a la fecha final de la solicitud");
                    }
                    else
                    {
                        regresaRowH4.Add("red white-text rojo | Formato invalido");
                    }

                }
                else
                {
                    regresaRowH4.Add("");
                }
            }
            else
            {
                if (FechasDistribucion.tipo == "Rango")
                {
                    regresaRowH4.Add("red white-text rojo |Rango invalido");
                    regresaRowH4.Add("red white-text rojo |Rango invalido");
                }
                else
                {
                    regresaRowH4.Add("red white-text rojo | Formato invalido");
                    regresaRowH4.Add("red white-text rojo | Formato invalido");
                }
            }

            JsonResult cc = Json(regresaRowH4, JsonRequestBehavior.AllowGet);
            return cc;
        }
    }

}