using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CartaVController : Controller
    {
        // GET: CartaV
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

        // GET: CartaV/Details/5
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

        // GET: CartaV/Details/5
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
                ViewBag.miles = d.PAI.MILES;//LEJGG 090718

                List<string> lista = new List<string>();
                List<listacuerpoc> armadoCuerpoTab = new List<listacuerpoc>(); //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                List<int> numfilasTabla = new List<int>();
                HeaderFooter hfc = new HeaderFooter();
                hfc.eliminaArchivos();
                CartaV cv = new CartaV();

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
                //B20180710 MGC 2018.07.12 Modificación 9 y 10 dependiendo del campo de factura en tsol............
                bool fact = db.TSOLs.First(ts => ts.ID == d.TSOL_ID).FACTURA;

                //B20180730 MGC 2018.07.30 Formatos
                //Referencia a formatos
                FormatosC format = new FormatosC();
                string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos

                //B20180720P MGC 2018.07.25
                string trclass = "";
                bool editmonto = false; //B20180710 MGC 2018.07.18 editar el monto en porcentaje categoría
                var cabeza = new List<string>();
                List<string> armadoCuerpoTabStr = null;
                bool varligada = Convert.ToBoolean(d.LIGADA);
                if (!varligada)
                {
                    FnCommonCarta.ObtenerCartaProductos(db,  d,null,null,spras_id,false,
                    ref lista,
                    ref armadoCuerpoTab,
                    ref armadoCuerpoTabStr,
                    ref numfilasTabla,
                    ref cabeza,
                    ref editmonto);
                    if (d.TIPO_TECNICO == "M")
                    {
                        trclass = " total";
                    }
                  
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////DATOS PARA LA TABLA 2 RECURRENCIAS EN LA VISTA///////////////////////////////////////
                var cabeza2 = new List<string>();
                var cabeza3 = new List<string>();
                List<string> armadoCuerpoTab2 = new List<string>();
                List<string> armadoCuerpoTab3 = new List<string>();
                int rowsRecs = 0;
                int rowsObjQs = 0;
                FnCommonCarta.ObtenerCartaRecurrentes(db,d,spras_id,
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
                cv.listaCuerpom = armadoCuerpoTab;////NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE QUE POSTERIORMENTE ES DISTRIBUIDA EN LAS TABLAS //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                cv.numColEncabezado = cabeza;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                cv.secondTab_x = true;
                cv.tercerTab_x = true;
                cv.material_x = (armadoCuerpoTab != null && armadoCuerpoTab.Any() && !string.IsNullOrEmpty(armadoCuerpoTab.First().val));
                cv.costoun_x = true;
                cv.apoyo_x = true;
                cv.apoyop_x = true;
                cv.costoap_x = true;
                cv.precio_x = true;
                //cv.apoyoEst_x = true; //Volumen //B20180726 MGC 2018.07.26
                //cv.apoyoRea_x = true; //Apoyo //B20180726 MGC 2018.07.26
                cv.volumen_x = true; //Volumen //B20180726 MGC 2018.07.26 
                cv.apoyototal_x = true; //Apoyo //B20180726 MGC 2018.07.26
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

                ViewBag.factura = fact;//B20180710 MGC 2018.07.12 Apoyo es real o es estimado
                ViewBag.trclass = trclass;//B20180710 MGC 2018.07.18 total es input o text
                ViewBag.editmonto = editmonto;//B20180710 MGC 2018.07.18 total es input o text

                //B20180720P MGC 2018.07.25
                ViewBag.varligada = varligada;

                //B20180801 MGC Formato
                ViewBag.montoformat = format.toShow(Convert.ToDecimal(d.MONTO_DOC_MD), decimales);

                return View(cv);
            }
        }

        // POST: CartaV/Details/5
        [HttpPost]
        //[ValidateAntiForgeryToken] //B20180710 MGC 2018.07.16 Modificaciones para editar los campos de distribución se agrego los objetos
        public ActionResult Create(CartaV v, string monto_enviar, string guardar_param)
        {
            if (guardar_param== "guardar_desde_visualizar")
            {
                v=(CartaV)TempData["v"];
                monto_enviar=TempData["monto_enviar"]?.ToString();
                guardar_param = "guardar_param";
            }
        
            int pos = 0;//B20180720P MGC Guardar Carta

            //B20180726 MGC 2018.07.26
            bool fact = false;
            DOCUMENTO d = new DOCUMENTO();
            using (TruequeEntities db = new TruequeEntities())
            {
                string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
                try
                {
                    d = db.DOCUMENTOes.Find(v.num_doc);
                    fact = db.TSOLs.Where(ts => ts.ID == d.TSOL_ID).FirstOrDefault().FACTURA;
                    d.PAI = db.PAIS.Where(a => a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "CartaV", "Create");
                }


                //Formatos para numeros

                string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos

                FormatosC format = new FormatosC(); //B20180730 MGC 2018.07.30 Formatos

                CARTA ca = new CARTA
                {
                    NUM_DOC = v.num_doc,
                    CLIENTE = v.cliente,
                    CLIENTEX = v.cliente_x,
                    COMPANY = v.company,
                    COMPANYC = v.companyC,
                    COMPANYCC = v.companyCC,
                    COMPANYCCX = v.companyCC_x,
                    COMPANYCX = v.companyC_x,
                    COMPANYX = v.company_x,
                    CONCEPTO = v.concepto,
                    CONCEPTOX = v.concepto_x,
                    DIRECCION = v.direccion,
                    DIRECCIONX = v.direccion_x,
                    ESTIMADO = v.estimado,
                    ESTIMADOX = v.estimado_x,
                    FOLIO = v.folio,
                    FOLIOX = v.folio_x,
                    LEGAL = v.legal,
                    LEGALX = v.legal_x,
                    LUGARFECH = v.lugarFech,
                    LUGARFECHX = v.lugarFech_x,
                    LUGAR = v.lugar,
                    LUGARX = v.lugar_x,
                    MAIL = v.mail,
                    MAILX = v.mail_x,
                    MECANICA = v.mecanica,
                    MECANICAX = v.mecanica_x,
                    NOMBREC = v.nombreC,
                    NOMBRECX = v.nombreC_x,
                    NOMBREE = v.nombreE,
                    NOMBREEX = v.nombreE_x
                };
                ca.NUM_DOC = v.num_doc;
                ca.PAYER = v.payerId;
                ca.PAYERX = v.payerId_x;
                ca.PUESTO = v.puesto;
                ca.PUESTOC = v.puestoC;
                ca.PUESTOCX = v.puestoC_x;
                ca.PUESTOE = v.puestoE;
                ca.PUESTOEX = v.puestoE_x;
                ca.PUESTOX = v.puestoE_x;
                ca.TAXID = v.taxid;
                ca.TAXIDX = v.taxid_x;
                ca.MONTO = v.monto;
                ca.MONEDA = v.moneda;
                ca.USUARIO_ID = User.Identity.Name;
                ca.FECHAC = DateTime.Now;

                //Agregadas  //B20180720P MGC Guardar Carta
                ca.PAYERNOM = v.payerNom;
                ca.PAYERNOMX = v.payerNom_x;
                ca.COMENTARIO = v.comentarios;
                ca.COMENTARIOX = v.comentarios_x;
                ca.COMPROMISOC = v.compromisoC;
                ca.COMPROMISOCX = v.compromisoC_x;
                ca.COMPROMISOK = v.compromisoK;
                ca.COMPROMISOKX = v.compromisoK_x;

                ca.SECOND_TABX = v.secondTab_x;
                ca.MATERIALX = v.material_x;
                ca.COSTO_UNX = v.costoun_x;
                ca.APOYOX = v.apoyo_x;
                ca.APOYOPX = v.apoyop_x;
                ca.COSTOAPX = v.costoap_x;
                ca.PRECIOX = v.precio_x;
                //B20180726 MGC 2018.07.26
                //ca.APOYO_ESTX = v.apoyoEst_x; //Volumen 
                //ca.APOYO_REAX = v.apoyoRea_x; //Apoyo
                if (fact)
                {
                    ca.VOLUMEN_REAX = v.volumen_x; //Volumen 
                    ca.APOYO_REAX = v.apoyototal_x; //Apoyo
                }
                else
                {
                    ca.VOLUMEN_ESTX = v.volumen_x; //Volumen 
                    ca.APOYO_ESTX = v.apoyototal_x; //Apoyo
                }

                //B20180801 MGC Formato
                decimal montod = 0;
                try
                {
                    montod = Convert.ToDecimal(monto_enviar);
                    ca.MONTO = montod.ToString();
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "CartaV", "Create");
                }

                v.monto = format.toShow(montod, decimales);
                

                var cartas = db.CARTAs.Where(a => a.NUM_DOC.Equals(ca.NUM_DOC)).ToList();
                if (cartas.Count > 0)
                    pos = cartas.OrderByDescending(a => a.POS).First().POS;

                ca.POS = pos + 1;
                pos = ca.POS; //B20180720P MGC Guardar Carta
                if (guardar_param == "guardar_param")//B20180720P MGC Guardar Carta
                {
                    db.CARTAs.Add(ca);
                    db.SaveChanges();
                    TempData["v"] = null;
                    TempData["monto_enviar"] = null;
                    TempData["return"] = "SOL";
                }
                else
                {
                    TempData["v"] = v;
                    TempData["monto_enviar"] = monto_enviar;
                    TempData["return"] = "CAR";
                }

                TempData["lista"] = TempData["vista"];
                TempData["ESTATUS_WF"] = TempData["swf"];

                d = db.DOCUMENTOes.Find(v.num_doc);

                List<string> encabezadoFech = new List<string>();
                List<string> armadoCuerpoTabStr = new List<string>(); //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                List<int> numfilasTab = new List<int>();



                /////////////////////////////////////////////DATOS PARA LA TABLA 1 MATERIALES EN EL PDF///////////////////////////////////////

                //B20180710 MGC 2018.07.17 Modificación 9 y 10 dependiendo del campo de factura en tsol..............
                var cabeza = new List<string>();
                List<listacuerpoc> armadoCuerpoTab = null;
                bool varligada = Convert.ToBoolean(d.LIGADA);
                bool editmonto = false;
                if (!varligada)
                {
                    FnCommonCarta.ObtenerCartaProductos(db, d,null, v, spras_id, (guardar_param == "guardar_param"),
                   ref encabezadoFech,
                   ref armadoCuerpoTab,
                   ref armadoCuerpoTabStr,
                   ref numfilasTab,
                   ref cabeza,
                   ref editmonto);
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////DATOS PARA LA TABLA 2 RECURRENCIAS EN PDF///////////////////////////////////////
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
                    true, v.monto);//ADD RSG 27.12.2018
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //MARCA DE AGUA
                bool aprob = false;
                aprob = (d.ESTATUS_WF.Equals("A") || d.ESTATUS_WF.Equals("S"));

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
                CartaV carta = v;
                CartaVEsqueleto cve = new CartaVEsqueleto();
                string recibeRuta = cve.crearPDF(carta, spras_id, aprob);
                return RedirectToAction("Index", new { ruta = recibeRuta, ids = v.num_doc });

            }
        }

      
        //B20180710 MGC 2018.07.13 Modificaciones para editar los campos de distribución se agrego los objetos
        [HttpPost]
        public ActionResult getPartialMat(List<DOCUMENTOP_MOD> docs)
        {

            CartaV doc = new CartaV
            {
                DOCUMENTOP = docs
            };
            return PartialView("~/Views/CartaV/_PartialMatTr.cshtml", doc);
        }

        //B20180720P MGC 2018.07.23
        // GET: Lista de cartas
        public ActionResult Lista(decimal id, string swf)
        {
            int pagina_id = 230; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = spras_id;
                TempData["id"] = id;

                //B20180720P MGC 2018.07.25 Obtener el statuswf para reedireccionar a d o a v
                TempData["ESTATUS_WF"] = swf;

                var lista = db.CARTAs.Include("DOCUMENTO").Where(a => a.NUM_DOC.Equals(id)).ToList();
                return View(lista);
            }
        }

        //B20180720P MGC 2018.07.23
        // GET: CartaF/Create
        ////DRS 14.10.2018 Se agrego la variable de swf///
        public ActionResult DetailsPos(decimal id, int pos, string swf)
        {
            TempData["v"] = null;
            TempData["monto_enviar"] = null;

            int pagina_id = 232; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {


                string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                ViewBag.de = FnCommonCarta.ObtenerTexto(db, spras_id,"de");
                ViewBag.al = FnCommonCarta.ObtenerTexto(db, spras_id,"a");
                ViewBag.mon = FnCommonCarta.ObtenerTexto(db, spras_id,"monto");

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

                d = db.DOCUMENTOes.Include("SOCIEDAD").Include("USUARIO").Where(a => a.NUM_DOC.Equals(id)).First();

                //B20180720P MGC 2018.07.23
                
                //Formatos para numeros
                d.PAI = db.PAIS.Where(a => a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
                string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos

                FormatosC format = new FormatosC(); //B20180730 MGC 2018.07.30 Formatos

                List<string> lista = new List<string>();
                List<listacuerpoc> armadoCuerpoTab = new List<listacuerpoc>(); //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                List<string> armadoCuerpoTab2 = new List<string>();
                List<int> numfilasTabla = new List<int>();
                int contadorTabla = 0;
                HeaderFooter hfc = new HeaderFooter();
                hfc.eliminaArchivos();
                CartaV cv = new CartaV();

                //B20180720P MGC 2018.07.23
                CARTA cs  = db.CARTAs.Where(a => a.NUM_DOC.Equals(id) && a.POS.Equals(pos)).First();
                ViewBag.legal = db.CARTAs.Where(a => a.NUM_DOC.Equals(id) && a.POS.Equals(pos)).First().LEGAL;


                /////////////////////////////////////////////DATOS PARA LA TABLA 1 MATERIALES EN LA VISTA///////////////////////////////////////
                //B20180720P MGC 2018.07.23
                var con = db.CARTAPs.Select(x => new { x.NUM_DOC, x.POS_ID, x.VIGENCIA_DE, x.VIGENCIA_AL }).Where(a => a.NUM_DOC.Equals(id) && a.POS_ID.Equals(pos)).GroupBy(f => new { f.VIGENCIA_DE, f.VIGENCIA_AL }).ToList();

                //B20180710 MGC 2018.07.12 Modificación 9 y 10 dependiendo del campo de factura en tsol............
                bool fact =  db.TSOLs.Where(ts => ts.ID == d.TSOL_ID).FirstOrDefault().FACTURA;
             
                //B20180710 MGC 2018.07.12 Modificación 9 y 10 dependiendo del campo de factura en tsol..............
                //B20180720P MGC 2018.07.23
                //B20180710 MGC 2018.07.18 total es input o text
                bool varligada = Convert.ToBoolean(d.LIGADA);
                foreach (var item in con)
                {
                    lista.Add(item.Key.VIGENCIA_DE.ToString() + item.Key.VIGENCIA_AL.ToString());
                }

                for (int i = 0; i < lista.Count; i++)
                {
                    contadorTabla = 0;

                    DateTime a1 = DateTime.Parse(lista[i].Remove(lista[i].Length / 2));
                    DateTime a2 = DateTime.Parse(lista[i].Remove(0, lista[i].Length / 2));

                    //B20180720P MGC 2018.07.23
                  
                    var con2 = db.CARTAPs
                                          .Where(x => x.NUM_DOC.Equals(id) && x.POS_ID.Equals(pos) && x.VIGENCIA_DE == a1 && x.VIGENCIA_AL == a2)
                                          .Join(db.MATERIALs, x => x.MATNR, y => y.ID, (x, y) => new
                                          {
                                              x.NUM_DOC,
                                              x.MATNR,
                                             MATKL=db.MATERIALGPs.FirstOrDefault(j=>j.ID==(db.MATERIALs.FirstOrDefault(k=>k.ID==x.MATNR).MATERIALGP_ID)).DESCRIPCION,
                                              y.MAKTX,
                                              x.MONTO,
                                              y.PUNIT,
                                              x.PORC_APOYO,
                                              x.MONTO_APOYO,
                                              resta = (x.MONTO - x.MONTO_APOYO),
                                              x.PRECIO_SUG,
                                              x.APOYO_EST,
                                              x.APOYO_REAL,
                                              x.VOLUMEN_EST,
                                              x.VOLUMEN_REAL
                                          }) //B20180710 MGC 2018.07.10 Se agregó x.VOLUMEN_EST, x.VOLUMEN_REAL
                                          .ToList();

                 

                    if (con2.Count > 0)
                    {
                        foreach (var item2 in con2)
                        {
                            //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                            listacuerpoc lc1 = new listacuerpoc
                            {
                                val = item2.MATNR.TrimStart('0'),
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc1);

                            listacuerpoc lc2 = new listacuerpoc
                            {
                                val = item2.MATKL,
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc2);

                            listacuerpoc lc3 = new listacuerpoc
                            {
                                val = item2.MAKTX,
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc3);

                            //Costo unitario
                            listacuerpoc lc4 = new listacuerpoc
                            {
                                val = format.toShow(Math.Round(item2.MONTO, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc4);

                            //Porcentaje de apoyo
                            listacuerpoc lc5 = new listacuerpoc
                            {
                                val = format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc5);

                            //Apoyo por pieza
                            listacuerpoc lc6 = new listacuerpoc
                            {
                                val = format.toShow(Math.Round(item2.MONTO_APOYO, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc6);

                            //Costo con apoyo
                            listacuerpoc lc7 = new listacuerpoc
                            {
                                val = format.toShow(Math.Round(item2.resta, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc7);

                            //Precio Sugerido
                            listacuerpoc lc8 = new listacuerpoc
                            {
                                //lc8.val = "$" + Math.Round(item2.PRECIO_SUG, 2).ToString();//B20180730 MGC 2018.07.30 Formatos
                                val = format.toShow(Math.Round(item2.PRECIO_SUG, 2), decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc8);

                            //Modificación 9 y 10 dependiendo del campo de factura en tsol
                            //fact = true es real
                            //Volumen
                            listacuerpoc lc9 = new listacuerpoc();
                            if (fact)
                            {
                                lc9.val = format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_REAL), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }
                            else
                            {
                                lc9.val = format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_EST), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }
                            lc9.clase = "ni";
                            armadoCuerpoTab.Add(lc9);

                            //Apoyo estimado
                            listacuerpoc lc10 = new listacuerpoc();
                            if (fact)
                            {
                                lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }
                            else
                            {
                                lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }
                            lc10.clase = "ni";
                            armadoCuerpoTab.Add(lc10);

                            contadorTabla++;
                        }
                    }
                    else
                    {
                        var con3 = db.CARTAPs
                                            .Where(x => x.NUM_DOC.Equals(id) && x.POS_ID.Equals(pos) && x.VIGENCIA_DE == a1 && x.VIGENCIA_AL == a2)
                                            .Join(db.MATERIALGPs, x => x.MATKL, y => y.ID, (x, y) => new
                                            {
                                                x.NUM_DOC,
                                                x.MATNR,
                                                x.MATKL,
                                                y.ID,
                                                x.MONTO,
                                                x.PORC_APOYO,
                                                TXT50 = y.DESCRIPCION,//RSG 03.10.2018
                                                x.MONTO_APOYO,
                                                resta = (x.MONTO - x.MONTO_APOYO),
                                                x.PRECIO_SUG,
                                                x.APOYO_EST,
                                                x.APOYO_REAL,
                                                x.VOLUMEN_EST,
                                                x.VOLUMEN_REAL
                                            }) //B20180710 MGC 2018.07.10 Se agregó x.VOLUMEN_EST, x.VOLUMEN_REAL})
                                            .ToList();
                      


                        foreach (var item2 in con3)
                        {
                            //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                            listacuerpoc lc1 = new listacuerpoc
                            {
                                val = "",
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc1);

                            listacuerpoc lc2 = new listacuerpoc
                            {
                                val = item2.MATKL,
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc2);

                            listacuerpoc lc3 = new listacuerpoc
                            {
                                val = item2.TXT50,
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc3);

                            //Costo unitario
                            listacuerpoc lc4 = new listacuerpoc
                            {
                                val = format.toShow(0, decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc4);

                            //Porcentaje de apoyo
                            listacuerpoc lc5 = new listacuerpoc();
                             //Definición si la distribución es monto o porcentaje
                            if (d.TIPO_TECNICO == "M")
                            {
                                 lc5.val = format.toShowPorc(0, decimales); ;//B20180730 MGC 2018.07.30 Formatos
                            }
                            else if (d.TIPO_TECNICO == "P")
                            {
                                lc5.val = format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }

                            lc5.clase = "ni";
                            armadoCuerpoTab.Add(lc5);

                            //Apoyo por pieza
                            listacuerpoc lc6 = new listacuerpoc
                            {
                                val = format.toShow(0, decimales),//B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc6);

                            //Costo con apoyo
                            listacuerpoc lc7 = new listacuerpoc
                            {
                                val = format.toShow(0, decimales), //B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc7);

                            //Precio Sugerido
                            listacuerpoc lc8 = new listacuerpoc
                            {
                                val = format.toShow(0, decimales), //B20180730 MGC 2018.07.30 Formatos
                                clase = "ni"
                            };
                            armadoCuerpoTab.Add(lc8);
                            //Modificación 9 y 10 dependiendo del campo de factura en tsol
                            //fact = true es real

                            //Volumen
                            listacuerpoc lc9 = new listacuerpoc
                            {
                                val = ""//B20180730 MGC 2018.07.30 Formatos
                            };
                            lc9.val = format.toShowNum(0, decimales);//B20180730 MGC 2018.07.30 Formatos
                           
                            lc9.clase = "ni";
                            armadoCuerpoTab.Add(lc9);

                            //Apoyo
                            listacuerpoc lc10 = new listacuerpoc();
                            if (fact)
                            {
                                lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }
                            else
                            {
                                lc10.val = format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales);//B20180730 MGC 2018.07.30 Formatos
                            }
                            //Definición si la distribución es monto o porcentaje
                            if (d.TIPO_TECNICO == "M")
                            {
                                lc10.clase = "ni";
                            }
                            else if (d.TIPO_TECNICO == "P")
                            {
                                lc10.clase = "ni";
                            }

                            armadoCuerpoTab.Add(lc10);

                            contadorTabla++;
                        }
                    }
                    numfilasTabla.Add(contadorTabla);
                }

                var cabeza = new List<string>
                {
                    FnCommonCarta.ObtenerTexto(db, spras_id, "materialC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "categoriaC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "descripcionC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "costouC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "apoyopoC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "apoyopiC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "costoaC"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "preciosC")
                };
                //B20180710 MGC 2018.07.12 Apoyo es real o es estimado
                //fact = true es real
                //Volumen
                if (fact)
                {
                    cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "volumenrC"));
                }
                else
                {
                    cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "volumeneC"));
                }
                //Apoyo
                if (fact)
                {
                    cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "apoyorC"));
                }
                else
                {
                    cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "apoyoeC"));
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////DATOS PARA LA TABLA 2 RECURRENCIAS EN LA VISTA///////////////////////////////////////
                var cabeza2 = new List<string>
                {
                    FnCommonCarta.ObtenerTexto(db, spras_id, "posC2"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "tipoC2"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "fechaC2")
                };
                if (varligada)
                {
                    cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "objetivo"));
                    cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "porcentajeC2"));
                }
                else
                {
                    if (d.TIPO_TECNICO=="P")
                    {
                        cabeza2.Add("");
                        cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "porcentajeC2"));
                    }
                    else
                    {
                        cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "montoC2"));
                        cabeza2.Add("");
                    }
                    
                }
               

                var con4 = db.DOCUMENTORECs
                                            .Where(x => x.NUM_DOC.Equals(id))
                                            .Join(db.DOCUMENTOes, x => x.NUM_DOC, y => y.NUM_DOC, (x, y) => new { x.POS, y.TSOL_ID, x.FECHAF, x.MONTO_BASE, x.PORC })
                                            .ToList();

                foreach (var item in con4)
                {
                    DateTime a = Convert.ToDateTime(item.FECHAF);

                    armadoCuerpoTab2.Add(item.POS.ToString());
                    armadoCuerpoTab2.Add(db.TSOLs.Where(x => x.ID == item.TSOL_ID).Select(x => x.DESCRIPCION).First());
                    armadoCuerpoTab2.Add(a.ToShortDateString());
                    if (varligada)
                    {
                        DOCUMENTORAN docRan = db.DOCUMENTORANs.First(x => x.LIN == 1 && x.NUM_DOC == id);
                        armadoCuerpoTab2.Add(format.toShow(Math.Round(docRan.OBJETIVOI.Value, 2), decimales));
                        armadoCuerpoTab2.Add(docRan.PORCENTAJE.Value.ToString("##.00"));
                    }
                    else
                    {
                        if (d.TIPO_TECNICO=="P") {
                            armadoCuerpoTab2.Add("");
                            armadoCuerpoTab2.Add(item.PORC.Value.ToString("##.00"));
                        }
                        else {
                            ////armadoCuerpoTab2.Add(format.toShow(Math.Round(item.MONTO_BASE.Value, 2), decimales));
                            armadoCuerpoTab2.Add(format.toShow(Math.Round(decimal.Parse(cs.MONTO), 2), decimales));
                            armadoCuerpoTab2.Add("");
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //TABLA 1 MATERIALES
                cv.listaFechas = lista;//////////////RANGO DE FECHAS QUE DETERMINAN EL NUMERO DE TABLAS
                cv.numfilasTabla = numfilasTabla;////NUMERO FILAS POR TABLA CALCULADA
                cv.listaCuerpom = armadoCuerpoTab;////NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE QUE POSTERIORMENTE ES DISTRIBUIDA EN LAS TABLAS //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                cv.numColEncabezado = cabeza;////////NUMERO DE COLUMNAS PARA LAS TABLAS

                
                //B20180720P MGC 2018.07.23
                cv.secondTab_x = Convert.ToBoolean(cs.SECOND_TABX);
                cv.costoun_x = Convert.ToBoolean(cs.COSTO_UNX);
                cv.material_x = Convert.ToBoolean(cs.MATERIALX == null || cs.MATERIALX.Value);
                cv.apoyo_x = Convert.ToBoolean(cs.APOYOX);
                cv.apoyop_x = Convert.ToBoolean(cs.APOYOPX);
                cv.costoap_x = Convert.ToBoolean(cs.COSTOAPX);
                cv.precio_x = Convert.ToBoolean(cs.PRECIOX);
                if (fact)
                {
                    cv.volumen_x = Convert.ToBoolean(cs.VOLUMEN_REAX); //Volumen
                    cv.apoyototal_x = Convert.ToBoolean(cs.APOYO_REAX); //Apoyo
                }
                else
                {
                    cv.volumen_x = Convert.ToBoolean(cs.VOLUMEN_ESTX); //Volumen
                    cv.apoyototal_x = Convert.ToBoolean(cs.APOYO_ESTX); //Apoyo
                }


                //TABLA 2 RECURRENCIAS
                cv.numColEncabezado2 = cabeza2;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                cv.numfilasTabla2 = con4.Count;//////NUMERO FILAS TOTAL PARA LA TABLA
                cv.listaCuerpoRec = armadoCuerpoTab2;//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
                ///////////////////////////////

              
                cv.num_doc = cs.NUM_DOC;
                cv.pos = pos;
                cv.company = cs.COMPANY;
                cv.company_x = Convert.ToBoolean(cs.COMPANYX);
                cv.taxid = cs.TAXID;
                cv.taxid_x = Convert.ToBoolean(cs.TAXIDX);
                cv.concepto = cs.CONCEPTO;
                cv.concepto_x = Convert.ToBoolean(cs.CONCEPTOX);
                cv.cliente = cs.CLIENTE;
                cv.cliente_x = Convert.ToBoolean(cs.CLIENTEX);
                cv.puesto = cs.PUESTOC;
                cv.puesto_x = Convert.ToBoolean(cs.PUESTOCX);
                cv.direccion = cs.DIRECCION;
                cv.direccion_x = Convert.ToBoolean(cs.DIRECCIONX);
                cv.folio = cs.FOLIO;
                cv.folio_x = Convert.ToBoolean(cs.FOLIOX);
                cv.lugar = cs.LUGAR;
                cv.lugar_x = Convert.ToBoolean(cs.LUGARX);
                cv.lugarFech = cs.LUGARFECH;
                cv.lugarFech_x = Convert.ToBoolean(cs.LUGARFECHX);
                cv.payerId = cs.PAYER;
                cv.payerId_x = Convert.ToBoolean(cs.PAYERX);
                cv.estimado = cs.ESTIMADO;
                cv.estimado_x = Convert.ToBoolean(cs.ESTIMADOX);
                cv.mecanica = cs.MECANICA;
                cv.mecanica_x = Convert.ToBoolean(cs.MECANICAX);

                cv.nombreE = cs.NOMBREE;
                cv.nombreE_x = Convert.ToBoolean(cs.NOMBREEX);
                if (cs.PUESTOE != null)
                    cv.puestoE = cs.PUESTOE;
                cv.puestoE_x = Convert.ToBoolean(cs.PUESTOEX);
                cv.companyC = cs.COMPANYC;
                cv.companyC_x = Convert.ToBoolean(cs.COMPANYCX);
                cv.nombreC = cs.NOMBREC;
                cv.nombreC_x = Convert.ToBoolean(cs.NOMBRECX);
                cv.puestoC = cs.PUESTOC;
                cv.puestoC_x = Convert.ToBoolean(cs.PUESTOCX);
                cv.companyCC = cs.COMPANYCC;
                cv.companyCC_x = Convert.ToBoolean(cs.COMPANYCCX);
                if (ViewBag.legal != null)
                    cv.legal = ViewBag.legal;
                cv.legal_x = Convert.ToBoolean(cs.LEGALX);
                cv.mail = cs.MAIL;
                cv.mail_x = Convert.ToBoolean(cs.MAILX);
                cv.monto_x = true;
                cv.monto =  (cs.MONTO!= null? cs.MONTO.ToString():"");
                cv.moneda = cs.MONEDA;

                //B20180720P MGC Guardar Carta
                cv.payerNom = cs.PAYERNOM;
                cv.payerNom_x = Convert.ToBoolean(cs.PAYERNOMX);
                cv.comentarios = cs.COMENTARIO;
                cv.comentarios_x = Convert.ToBoolean(cs.COMENTARIOX);
                cv.compromisoC = cs.COMPROMISOC;
                cv.compromisoC_x = Convert.ToBoolean(cs.COMPROMISOCX);
                cv.compromisoK = cs.COMPROMISOK;
                cv.compromisoK_x = Convert.ToBoolean(cs.COMPROMISOKX);

               
                //B20180801 MGC Formato
                decimal montod = 0;
                try
                {
                    montod = Convert.ToDecimal(cv.monto);
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "CartV", "DetailsPos");
                }

                cv.monto = format.toShow(montod, decimales);

                ////DRS 14.10.2018///
                TempData["ESTATUS_WF"] = swf;

                return View(cv);
            }
        }
        //B20180720P MGC 2018.07.24
        [HttpPost]
        public ActionResult DetailsPos(CartaV va)
        {
            //Obtener el id y la pos
            decimal id = va.num_doc;
            int pos = va.pos;


            TempData["return"] = "LIST";
            TempData["ESTATUS_WF"] = TempData["swf"];
            //int pagina = 231; //ID EN BASE DE DATOS
            int pagina_id = 232; //ID EN BASE DE DATOS
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

                d = db.DOCUMENTOes.Include("SOCIEDAD").Include("USUARIO").Where(a => a.NUM_DOC.Equals(id)).First();

                //Formatos para numeros
                d.PAI = db.PAIS.Where(a => a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
                string decimales = d.PAI.DECIMAL; //B20180730 MGC 2018.07.30 Formatos

                FormatosC format = new FormatosC(); //B20180730 MGC 2018.07.30 Formatos

                //B20180726 MGC 2018.07.26
                bool fact =  db.TSOLs.Where(ts => ts.ID == d.TSOL_ID).FirstOrDefault().FACTURA;
                bool varligada = Convert.ToBoolean(d.LIGADA);
                List<string> encabezadoFech = new List<string>();
                List<string> armadoCuerpoTab = new List<string>(); //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                List<string> armadoCuerpoTab2 = new List<string>();
                List<int> numfilasTabla = new List<int>();
                int contadorTabla = 0;
                HeaderFooter hfc = new HeaderFooter();
                hfc.eliminaArchivos();
                CartaV v = new CartaV();

                //B20180720P MGC 2018.07.23
                CARTA cs = db.CARTAs.Where(a => a.NUM_DOC.Equals(id) && a.POS.Equals(pos)).First();

                ViewBag.legal = db.CARTAs.Where(a => a.NUM_DOC.Equals(id) && a.POS.Equals(pos)).First().LEGAL;

                v.num_doc = cs.NUM_DOC;
                v.pos = pos;
                v.company = cs.COMPANY;
                v.company_x = Convert.ToBoolean(cs.COMPANYX);
                v.taxid = cs.TAXID;
                v.taxid_x = Convert.ToBoolean(cs.TAXIDX);
                v.concepto = cs.CONCEPTO;
                v.concepto_x = Convert.ToBoolean(cs.CONCEPTOX);
                 v.cliente = cs.CLIENTE;
                v.cliente_x = Convert.ToBoolean(cs.CLIENTEX);
                v.puesto = cs.PUESTOC;
                v.puesto_x = Convert.ToBoolean(cs.PUESTOCX);
                v.direccion = cs.DIRECCION;
                v.direccion_x = Convert.ToBoolean(cs.DIRECCIONX);
                v.folio = cs.FOLIO;
                v.folio_x = Convert.ToBoolean(cs.FOLIOX);
                v.lugar = cs.LUGAR;
                v.lugar_x = Convert.ToBoolean(cs.LUGARX);
                v.lugarFech = cs.LUGARFECH;
                v.lugarFech_x = Convert.ToBoolean(cs.LUGARFECHX);
                v.payerNom = cs.NOMBREC;
                v.payerNom_x = Convert.ToBoolean(cs.NOMBRECX);
                 v.estimado = cs.ESTIMADO;
                v.estimado_x = Convert.ToBoolean(cs.ESTIMADOX);
                v.mecanica = cs.MECANICA;
                v.mecanica_x = Convert.ToBoolean(cs.MECANICAX);

                v.nombreE = cs.NOMBREE;
                v.nombreE_x = Convert.ToBoolean(cs.NOMBREEX);
                if (cs.PUESTOE != null)
                    v.puestoE = cs.PUESTOE;
                v.puestoE_x = Convert.ToBoolean(cs.PUESTOEX);
                v.companyC = cs.COMPANYC;
                v.companyC_x = Convert.ToBoolean(cs.COMPANYCX);
                v.nombreC = cs.NOMBREC;
                v.nombreC_x = Convert.ToBoolean(cs.NOMBRECX);
                v.puestoC = cs.PUESTOC;
                v.puestoC_x = Convert.ToBoolean(cs.PUESTOCX);
                v.companyCC = cs.COMPANYCC;
                v.companyCC_x = Convert.ToBoolean(cs.COMPANYCCX);
                if (ViewBag.legal != null)
                    v.legal = ViewBag.legal;
                v.legal_x = Convert.ToBoolean(cs.LEGALX);
                v.mail = cs.MAIL;
                v.mail_x = Convert.ToBoolean(cs.MAILX);
                v.monto_x = true;
                v.monto = (cs.MONTO!=null? cs.MONTO.ToString():"");
                v.moneda = cs.MONEDA;
                v.payerNom = cs.PAYERNOM;
                v.payerNom_x = Convert.ToBoolean(cs.PAYERNOMX);
                v.comentarios = cs.COMENTARIO;
                v.comentarios_x = Convert.ToBoolean(cs.COMENTARIOX);
                v.compromisoC = cs.COMPROMISOC;
                v.compromisoC_x = Convert.ToBoolean(cs.COMPROMISOCX);
                v.compromisoK = cs.COMPROMISOK;
                v.compromisoK_x = Convert.ToBoolean(cs.COMPROMISOKX);

                //B20180720P MGC 2018.07.23
                v.material_x = Convert.ToBoolean(cs.MATERIALX);
                v.secondTab_x = Convert.ToBoolean(cs.SECOND_TABX);
                v.costoun_x = Convert.ToBoolean(cs.COSTO_UNX);
                v.apoyo_x = Convert.ToBoolean(cs.APOYOX);
                v.apoyop_x = Convert.ToBoolean(cs.APOYOPX);
                v.costoap_x = Convert.ToBoolean(cs.COSTOAPX);
                v.precio_x = Convert.ToBoolean(cs.PRECIOX);
                //B20180726 MGC 2018.07.26
                if (fact)
                {
                    v.volumen_x = Convert.ToBoolean(cs.VOLUMEN_REAX); //Volumen
                    v.apoyototal_x = Convert.ToBoolean(cs.APOYO_REAX); //Apoyo
                }
                else
                {
                    v.volumen_x = Convert.ToBoolean(cs.VOLUMEN_ESTX); //Volumen
                    v.apoyototal_x = Convert.ToBoolean(cs.APOYO_ESTX); //Apoyo
                }


                /////////////////////////////////////////////DATOS PARA LA TABLA 1 MATERIALES EN LA VISTA///////////////////////////////////////
                //B20180720P MGC 2018.07.23
                var con = db.CARTAPs.Select(x => new { x.NUM_DOC, x.POS_ID, x.VIGENCIA_DE, x.VIGENCIA_AL }).Where(a => a.NUM_DOC.Equals(id) && a.POS_ID.Equals(pos)).GroupBy(f => new { f.VIGENCIA_DE, f.VIGENCIA_AL }).ToList();

               

                foreach (var item in con)
                {
                    encabezadoFech.Add(item.Key.VIGENCIA_DE.ToString() + item.Key.VIGENCIA_AL.ToString());
                }

                for (int i = 0; i < encabezadoFech.Count; i++)
                {
                    contadorTabla = 0;

                    DateTime a1 = DateTime.Parse(encabezadoFech[i].Remove(encabezadoFech[i].Length / 2));
                    DateTime a2 = DateTime.Parse(encabezadoFech[i].Remove(0, encabezadoFech[i].Length / 2));

                    var con2 = db.CARTAPs
                                          .Where(x => x.NUM_DOC.Equals(id) && x.POS_ID.Equals(pos) && x.VIGENCIA_DE == a1 && x.VIGENCIA_AL == a2)
                                          .Join(db.MATERIALs, x => x.MATNR, y => y.ID, (x, y) => new
                                          {
                                              x.NUM_DOC,
                                              x.MATNR,
                                              MATKL = db.MATERIALGPs.FirstOrDefault(j => j.ID == (db.MATERIALs.FirstOrDefault(k => k.ID == x.MATNR).MATERIALGP_ID)).DESCRIPCION,
                                              y.MAKTX,
                                              x.MONTO,
                                              y.PUNIT,
                                              x.PORC_APOYO,
                                              x.MONTO_APOYO,
                                              resta = (x.MONTO - x.MONTO_APOYO),
                                              x.PRECIO_SUG,
                                              x.APOYO_EST,
                                              x.APOYO_REAL,
                                              x.VOLUMEN_EST,
                                              x.VOLUMEN_REAL
                                          }) //B20180710 MGC 2018.07.10 Se agregó x.VOLUMEN_EST, x.VOLUMEN_REAL
                                          .ToList();

                    if (con2.Count > 0)
                    {
                        foreach (var item2 in con2)
                        {

                            if (v.material_x)
                            {
                                armadoCuerpoTab.Add(item2.MATNR.TrimStart('0'));
                            }
                            armadoCuerpoTab.Add(item2.MATKL);
                            armadoCuerpoTab.Add(item2.MAKTX);

                            if (v.costoun_x)
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round(item2.MONTO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }
                            if (v.apoyo_x)
                            {
                                armadoCuerpoTab.Add(format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }
                            if (v.apoyop_x)
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round(item2.MONTO_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }

                            if (v.costoap_x)
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round((item2.MONTO - item2.MONTO_APOYO), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }
                            if (v.precio_x)
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round(item2.PRECIO_SUG, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }

                            //B20180726 MGC 2018.07.26
                            if (v.volumen_x )
                            {
                                if (fact)
                                {
                                    armadoCuerpoTab.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                    }
                                else
                                {
                                     armadoCuerpoTab.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                     }
                            }

                            //Apoyo
                            //B20180726 MGC 2018.07.26
                            if (v.apoyototal_x )
                            {
                                if (fact)
                                {
                                      armadoCuerpoTab.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                }
                                else
                                {
                                    armadoCuerpoTab.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                }
                            }
                            contadorTabla++;
                        }
                    }
                    else
                    {
                        //B20180720P MGC 2018.07.23

                        var con3 = db.CARTAPs
                                            .Where(x => x.NUM_DOC.Equals(id) && x.POS_ID.Equals(pos) && x.VIGENCIA_DE == a1 && x.VIGENCIA_AL == a2)
                                            .Join(db.MATERIALGPs, x => x.MATKL, y => y.ID, (x, y) => new
                                            {
                                                x.NUM_DOC,
                                                x.MATNR,
                                                x.MATKL,
                                                y.ID,
                                                x.MONTO,
                                                x.PORC_APOYO,
                                                TXT50 = y.DESCRIPCION,//RSG 03.10.2018
                                                x.MONTO_APOYO,
                                                resta = (x.MONTO - x.MONTO_APOYO),
                                                x.PRECIO_SUG,
                                                x.APOYO_EST,
                                                x.APOYO_REAL,
                                                x.VOLUMEN_EST,
                                                x.VOLUMEN_REAL
                                            }) //B20180710 MGC 2018.07.10 Se agregó x.VOLUMEN_EST, x.VOLUMEN_REAL})
                                            .ToList();

                        foreach (var item2 in con3)
                        {

                            if (v.material_x)
                            {
                                armadoCuerpoTab.Add("");
                            }
                            armadoCuerpoTab.Add(item2.MATKL);
                            armadoCuerpoTab.Add(item2.TXT50);

                            if (v.costoun_x )
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round(item2.MONTO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }
                            if (v.apoyo_x )
                            {
                                armadoCuerpoTab.Add(format.toShowPorc(Math.Round(item2.PORC_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }
                            if (v.apoyop_x )
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round(item2.MONTO_APOYO, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }

                            if (v.costoap_x )
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round((item2.MONTO - item2.MONTO_APOYO), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }
                            if (v.precio_x )
                            {
                                armadoCuerpoTab.Add(format.toShow(Math.Round(item2.PRECIO_SUG, 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                            }

                            //Volumen
                            //B20180726 MGC 2018.07.26
                            if (v.volumen_x )
                            {
                                if (fact)
                                {
                                    armadoCuerpoTab.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                }
                                else
                                {
                                    armadoCuerpoTab.Add(format.toShowNum(Math.Round(Convert.ToDecimal(item2.VOLUMEN_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                               }
                            }

                            //Apoyo
                            //B20180726 MGC 2018.07.26
                            if (v.apoyototal_x )
                            {
                                if (fact)
                                {
                                    armadoCuerpoTab.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_REAL), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                }
                                else
                                {
                                    armadoCuerpoTab.Add(format.toShow(Math.Round(Convert.ToDecimal(item2.APOYO_EST), 2), decimales));//B20180730 MGC 2018.07.30 Formatos
                                }
                            }

                            contadorTabla++;
                        }
                    }
                    numfilasTabla.Add(contadorTabla);
                }

                var cabeza = new List<string>();

                if (v.material_x) { cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "materialC")); }
                cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "categoriaC"));
                cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "descripcionC"));
                
                if (v.costoun_x ) { cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "costouC")); }
                if (v.apoyo_x ) { cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "apoyopoC")); }
                if (v.apoyop_x ) { cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "apoyopiC")); }
                if (v.costoap_x ) { cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "costoaC")); }
                if (v.precio_x ) { cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "preciosC")); }
                //B20180710 MGC 2018.07.12 Apoyo es real o es estimado
                //Volumen
                //B20180726 MGC 2018.07.26
                if (v.volumen_x )
                {
                    if (fact)
                    {
                        cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "volumenrC"));
                    }
                    else
                    {
                        cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "volumeneC"));
                    }
                }
                //Apoyo
                //B20180726 MGC 2018.07.26
                if (v.apoyototal_x )
                {
                    if (fact)
                    {
                        cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "apoyorC"));
                    }
                    else
                    {
                        cabeza.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "apoyoeC"));
                    }
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////DATOS PARA LA TABLA 2 RECURRENCIAS EN LA VISTA///////////////////////////////////////
                var cabeza2 = new List<string>
                {
                    FnCommonCarta.ObtenerTexto(db, spras_id, "posC2"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "tipoC2"),
                    FnCommonCarta.ObtenerTexto(db, spras_id, "fechaC2")
                };
                if (varligada)
                {
                    cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "objetivo"));
                    cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "porcentajeC2"));
                }
                else
                {
                    if (d.TIPO_TECNICO == "P")
                    {
                        
                        cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "porcentajeC2"));
                    }
                    else
                    {
                        cabeza2.Add(FnCommonCarta.ObtenerTexto(db, spras_id, "montoC2"));
                    }
                }
                

                var con4 = db.DOCUMENTORECs
                                            .Where(x => x.NUM_DOC.Equals(id))
                                            .Join(db.DOCUMENTOes, x => x.NUM_DOC, y => y.NUM_DOC, (x, y) => new { x.POS, y.TSOL_ID, x.FECHAF, x.MONTO_BASE, x.PORC })
                                            .ToList();

                foreach (var item in con4)
                {
                    DateTime a = Convert.ToDateTime(item.FECHAF);

                    armadoCuerpoTab2.Add(item.POS.ToString());
                    armadoCuerpoTab2.Add(db.TSOLs.Where(x => x.ID == item.TSOL_ID).Select(x => x.DESCRIPCION).First());
                    armadoCuerpoTab2.Add(a.ToShortDateString());
                    if (varligada)
                    {
                        DOCUMENTORAN docRan = db.DOCUMENTORANs.First(x => x.LIN == 1 && x.NUM_DOC == id);
                        armadoCuerpoTab2.Add(format.toShow(Math.Round(docRan.OBJETIVOI.Value, 2), decimales));
                        armadoCuerpoTab2.Add(docRan.PORCENTAJE.Value.ToString("##.00"));
                    }
                    else
                    {
                        if (d.TIPO_TECNICO == "P")
                        {
                            armadoCuerpoTab2.Add(item.PORC.Value.ToString("##.00"));
                        }
                        else
                        {
                            ////armadoCuerpoTab2.Add(format.toShow(Math.Round(item.MONTO_BASE.Value, 2), decimales));
                            armadoCuerpoTab2.Add(format.toShow(Math.Round(decimal.Parse(v.monto), 2), decimales));
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //TABLA 1 MATERIALES
                v.listaFechas = encabezadoFech;//////////////RANGO DE FECHAS QUE DETERMINAN EL NUMERO DE TABLAS
                v.numfilasTabla = numfilasTabla;////NUMERO FILAS POR TABLA CALCULADA
                v.listaCuerpo = armadoCuerpoTab;////NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE QUE POSTERIORMENTE ES DISTRIBUIDA EN LAS TABLAS //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                v.numColEncabezado = cabeza;////////NUMERO DE COLUMNAS PARA LAS TABLAS


                //TABLA 2 RECURRENCIAS
                v.numColEncabezado2 = cabeza2;////////NUMERO DE COLUMNAS PARA LAS TABLAS
                v.numfilasTabla2 = con4.Count;//////NUMERO FILAS TOTAL PARA LA TABLA
                v.listaCuerpoRec = armadoCuerpoTab2;//NUMERO TOTAL DE FILAS CON LA INFO CORRESPONDIENTE
                                                    ///////////////////////////////


                //MARCA DE AGUA
                bool aprob = false;
                bool apTS = (d.ESTATUS_WF.Equals("P") && db.FLUJOes.Any(x => x.STATUS == "N  PRP  0" && x.NUM_DOC==d.NUM_DOC));
                aprob = (d.ESTATUS_WF.Equals("A") || d.ESTATUS_WF.Equals("S") || apTS);

                //PARA LA TABLA 1 MATERIALES
                v.numColEncabezado = cabeza;
                v.listaFechas = encabezadoFech;
                v.numfilasTabla = numfilasTabla;
                v.listaCuerpo = armadoCuerpoTab;
                //PARA LA TABLA 2 RECURRENCIAS
                v.numColEncabezado2 = cabeza2;
                v.numfilasTabla2 = con4.Count;
                v.listaCuerpoRec = armadoCuerpoTab2;

                //B20180801 MGC Formato
                decimal montod = 0;
                try
                {
                    montod = Convert.ToDecimal(v.monto);
                }
                catch (Exception e)
                {
                    Log.ErrorLogApp(e, "CartaV", "DetailsPos");
                }

                v.monto = format.toShow(montod, decimales);

                CartaV carta = v;
                CartaVEsqueleto cve = new CartaVEsqueleto();
                string recibeRuta = cve.crearPDF(carta, spras_id, aprob);
                return RedirectToAction("Index", new { ruta = recibeRuta, ids = v.num_doc });

            }
        }
        
        
    }
}
