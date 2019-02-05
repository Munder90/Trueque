using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Services; //B20180730 MGC 2018.07.30 Formatos

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class CartaDController : Controller
    {
        // GET: CartaD
        public ActionResult Index(string ruta, decimal ids)
        {
            int pagina_id = 230; //ID EN BASE DE DATOS
            TempData["ESTATUS_WF"] = TempData["swf"];
            TempData["lista"] = TempData["vista"];
            using (TruequeEntities db = new TruequeEntities())
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            }
            ViewBag.url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + HostingEnvironment.ApplicationVirtualPath + "/" + ruta;
            ViewBag.miNum = ids;

            return View();
        }

        // GET: CartaD/Details/5
        public ActionResult Details(string ruta)
        {
            int pagina_id = 230; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            }
            ViewBag.url = ruta;
            return View();
        }

        // GET: CartaD/Details/5
        public ActionResult Create(decimal id, bool? Viewlista)
        {
            int pagina_id = 232; //ID EN BASE DE DATOS

            if (Viewlista == true)
            {
                TempData["return"] = "LIST";
                TempData["ESTATUS_WF"] = TempData["swf"];
            }
            using (TruequeEntities db = new TruequeEntities())
            {
                string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

                ViewBag.de = FnCommonCarta.ObtenerTexto(db, spras_id, "de");
                ViewBag.al = FnCommonCarta.ObtenerTexto(db, spras_id, "a");
                ViewBag.mon = FnCommonCarta.ObtenerTexto(db, spras_id, "monto");

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

                DOCUMENTO d = new DOCUMENTO();
                PUESTOT pp = new PUESTOT();
                d = db.DOCUMENTOes.Include("SOCIEDAD").Include("USUARIO").Where(a => a.NUM_DOC.Equals(id)).First();

                ViewBag.dec = d.PAI.DECIMAL;//LEJGG 090718

                List<string> lista = new List<string>();
                List<string> armadoCuerpoTabStr = new List<string>();
                List<int> numfilasTabla = new List<int>();
                HeaderFooter hfc = new HeaderFooter();
                hfc.eliminaArchivos();
                CartaD cv = new CartaD();

                if (d != null)
                {
                    d.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(d.VKORG)
                                                              && a.VTWEG.Equals(d.VTWEG)
                                                            && a.SPART.Equals(d.SPART)
                                                            && a.KUNNR.Equals(d.PAYER_ID)).First();
                    pp = db.PUESTOTs.Where(a => a.SPRAS_ID.Equals(spras_id) && a.PUESTO_ID == d.USUARIO.PUESTO_ID).FirstOrDefault();
                }
                ViewBag.legal = db.LEYENDAs.Where(a => a.PAIS_ID.Equals(d.PAIS_ID) && a.ACTIVO == true).FirstOrDefault();


                /////////////////////////////////////////////DATOS PARA LA TABLA 1 MATERIALES EN LA VISTA///////////////////////////////////////
                
                FormatosC format = new FormatosC();
                string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos

                //B20180720P MGC 2018.07.25
                bool editmonto = false;
                var cabeza = new List<string>();
                List<listacuerpoc> armadoCuerpoTab = null;
                bool varligada = Convert.ToBoolean(d.LIGADA);
                if (!varligada)
                {
                    FnCommonCarta.ObtenerCartaProductos(db, d, null,null, spras_id, false,
                    ref lista,
                    ref armadoCuerpoTab,
                    ref armadoCuerpoTabStr,
                    ref numfilasTabla,
                    ref cabeza,
                    ref editmonto);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                /////////////////////////////////////////////DATOS PARA LA TABLA 2 RECURRENCIAS EN LA VISTA///////////////////////////////////////
                var cabeza2 = new List<string>();
                var cabeza3 = new List<string>();
                List<string> armadoCuerpoTab2 = new List<string>();
                List<string> armadoCuerpoTab3 = new List<string>();
                int rowsRecs = 0;
                int rowsObjQs = 0;
                FnCommonCarta.ObtenerCartaRecurrentes(db, d, spras_id, 
                    ref cabeza2, 
                    ref armadoCuerpoTab2, 
                    ref rowsRecs,
                    ref cabeza3, 
                    ref armadoCuerpoTab3,
                    ref rowsObjQs,
                    false, format.toShow((decimal)d.MONTO_DOC_MD, decimales));//RSG 27.12.2018

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //TABLA 1 MATERIALES
                cv.listaFechas = lista;//////////////RANGO DE FECHAS QUE DETERMINAN EL NUMERO DE TABLAS
                cv.numfilasTabla = numfilasTabla;////NUMERO FILAS POR TABLA CALCULADA
                cv.listaCuerpo = armadoCuerpoTabStr;////NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE QUE POSTERIORMENTE ES DISTRIBUIDA EN LAS TABLAS
                cv.numColEncabezado = cabeza;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                cv.secondTab_x = true;
                cv.tercerTab_x = true;
                cv.material_x = (armadoCuerpoTabStr != null && armadoCuerpoTabStr.Any() && !string.IsNullOrEmpty(armadoCuerpoTabStr.First()));
                cv.costoun_x = true;
                cv.apoyo_x = true;
                cv.apoyop_x = true;
                cv.costoap_x = true;
                cv.precio_x = true;
                //B20180726 MGC 2018.07.26
                cv.volumen_x = true;
                cv.apoyototal_x = true;
                /////////////////////////////////

                //TABLA 2 RECURRENCIAS
                cv.numColEncabezado2 = cabeza2;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                cv.numfilasTabla2 = rowsRecs;//////NUMERO FILAS TOTAL PARA LA TABLA
                cv.listaCuerpoRec = armadoCuerpoTab2;//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
                                                     ///////////////////////////////

                //TABLA 3 OBJECTIVO Q
                cv.numColEncabezado3 = cabeza3;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                cv.numfilasTabla3 = rowsObjQs;//////NUMERO FILAS TOTAL PARA LA TABLA
                cv.listaCuerpoObjQ = armadoCuerpoTab3;//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
                ///////////////////////////////

                cv.num_doc = id;
                cv.company = d.SOCIEDAD.BUTXT;
                cv.company_x = true;
                cv.taxid = d.SOCIEDAD.LAND;
                cv.taxid_x = true;
                cv.concepto = d.CONCEPTO;
                cv.concepto_x = true;
                cv.cliente = d.PAYER_NOMBRE;
                cv.cliente_x = true;
                cv.puesto = " ";
                cv.puesto_x = false;
                cv.direccion = d.CLIENTE.STRAS_GP;
                cv.direccion_x = true;
                cv.folio = d.NUM_DOC.ToString();
                cv.folio_x = true;
                cv.lugar = d.CIUDAD.Trim() + ", " + d.ESTADO.Trim();
                cv.lugar_x = true;
                cv.lugarFech = DateTime.Now.ToShortDateString();
                cv.lugarFech_x = true;
                cv.payerId = d.CLIENTE.PAYER;
                cv.payerId_x = true;
                cv.payerNom = d.CLIENTE.NAME1;
                cv.payerNom_x = true;
                cv.estimado = d.PAYER_NOMBRE;
                cv.estimado_x = true;
                cv.mecanica = d.NOTAS;
                cv.mecanica_x = true;
                cv.nombreE = d.USUARIO.NOMBRE + " " + d.USUARIO.APELLIDO_P + " " + d.USUARIO.APELLIDO_M;
                cv.nombreE_x = true;
                if (pp != null)
                    cv.puestoE = pp.TXT50;
                cv.puestoE_x = true;
                cv.companyC = cv.company;
                cv.companyC_x = true;
                cv.nombreC = d.PAYER_NOMBRE;
                cv.nombreC_x = true;
                cv.puestoC = " ";
                cv.puestoC_x = false;
                cv.companyCC = d.CLIENTE.NAME1;
                cv.companyCC_x = true;
                if (ViewBag.legal != null)
                    cv.legal = ViewBag.legal.LEYENDA1;
                cv.legal_x = true;
                cv.mail = FnCommonCarta.ObtenerTexto(db, spras_id, "correo") + " " + d.PAYER_EMAIL;
                cv.mail_x = true;
                cv.comentarios = "";
                cv.comentarios_x = true;
                cv.compromisoK = "";
                cv.compromisoK_x = true;
                cv.compromisoC = "";
                cv.compromisoC_x = true;
                cv.monto_x = true;
                cv.monto = d.MONTO_DOC_MD.ToString();
                cv.moneda = d.MONEDA_ID;

                //B20180720P MGC 2018.07.25
                ViewBag.varligada = varligada;

                //B20180801 MGC Formato
                decimal montod = 0;
                try
                {
                    montod = Convert.ToDecimal(cv.monto);
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e,"CartaD","Create");
                }

                ViewBag.montoformat = format.toShow(montod, decimales);

                return View(cv);
            }
        }

        // POST: CartaD/Details/5
        [HttpPost]
        public ActionResult Create(CartaD v)
        {
            TempData["lista"] = TempData["vista"];
            using (TruequeEntities db = new TruequeEntities())
            {
                string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

                List<string> encabezadoFech = new List<string>();
                List<string> armadoCuerpoTabStr = new List<string>();
                List<int> numfilasTab = new List<int>();

                DOCUMENTO d = db.DOCUMENTOes.Find(v.num_doc);
                

                //Formatos para numeros
                d.PAI = db.PAIS.Where(a => a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
               // string miles = d.PAI.MILES; //B20180730 MGC 2018.07.30 Formatos
                string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos

                FormatosC format = new FormatosC(); //B20180730 MGC 2018.07.30 Formatos

                /////////////////////////////////////////////DATOS PARA LA TABLA 1 MATERIALES EN EL PDF///////////////////////////////////////
                //B20180720P MGC 2018.07.25
                bool editmonto = false;
                var cabeza = new List<string>();
                List<listacuerpoc> armadoCuerpoTab = null;
                bool varligada = Convert.ToBoolean(d.LIGADA);
                if (!varligada)
                {
                    FnCommonCarta.ObtenerCartaProductos(db, d, v,null, spras_id, false,
                    ref encabezadoFech,
                    ref armadoCuerpoTab,
                    ref armadoCuerpoTabStr,
                    ref numfilasTab,
                    ref cabeza,
                    ref editmonto);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                /////////////////////////////////////////////DATOS PARA LA TABLA 2 RECURRENCIAS EN LA VISTA///////////////////////////////////////
                var cabeza2 = new List<string>();
                var cabeza3 = new List<string>();
                List<string> armadoCuerpoTab2 = new List<string>();
                List<string> armadoCuerpoTab3 = new List<string>();
                int rowsRecs = 0;
                int rowsObjQs = 0;
                FnCommonCarta.ObtenerCartaRecurrentes(db, d, spras_id, 
                    ref cabeza2, 
                    ref armadoCuerpoTab2, 
                    ref rowsRecs,
                     ref cabeza3,
                    ref armadoCuerpoTab3,
                    ref rowsObjQs,
                    true, format.toShow((decimal)d.MONTO_DOC_MD, decimales));//RSG 27.12.2018

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //MARCA DE AGUA
                bool aprob = false;
                bool apTS = (d.ESTATUS_WF.Equals("P") && db.FLUJOes.Any(x=>x.STATUS== "N  PRP  0" && x.NUM_DOC == d.NUM_DOC));
                aprob = (d.ESTATUS_WF.Equals("A") || d.ESTATUS_WF.Equals("S") || apTS);

                //PARA LA TABLA 1 MATERIALES
                v.numColEncabezado = cabeza;
                v.listaFechas = encabezadoFech;
                v.numfilasTabla = numfilasTab;
                v.listaCuerpo = armadoCuerpoTabStr;
                //PARA LA TABLA 2 RECURRENCIAS
                v.numColEncabezado2 = cabeza2;
                v.numfilasTabla2 = rowsRecs;
                v.listaCuerpoRec = armadoCuerpoTab2;

                //TABLA 3 OBJECTIVO Q
                v.numColEncabezado3 = cabeza3;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                v.numfilasTabla3 = rowsObjQs;//////NUMERO FILAS TOTAL PARA LA TABLA
                v.listaCuerpoObjQ = armadoCuerpoTab3;//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
                ///////////////////////////////
                //B20180801 MGC Formato
                decimal montod = 0;
                try
                {
                    montod = Convert.ToDecimal(v.monto);
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "CartaD", "Create");
                }

                v.monto = format.toShow(montod, decimales);

                CartaD carta = v;
                CartaDEsqueleto cve = new CartaDEsqueleto();
                string recibeRuta = cve.crearPDF(carta, spras_id, aprob);
                return RedirectToAction("Index", new { ruta = recibeRuta, ids = v.num_doc });
            }
        }
    }
}

       
 