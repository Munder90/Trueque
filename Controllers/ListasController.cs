using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Models.Dao;
using TAT001.Services;

namespace TAT001.Controllers
{
  
    public class ListasController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();
       
        //------------------DAO------------------------------
        readonly UsuariosDao usuariosDao = new UsuariosDao();
        readonly SolicitudesDao solicitudesDao = new SolicitudesDao();
        readonly SociedadesDao sociedadesDao = new SociedadesDao();
        readonly ContactosDao contactosDao = new ContactosDao();
        readonly ClientesDao clientesDao = new ClientesDao();
        readonly TallsDao tallsDao = new TallsDao();
        readonly StatesDao statesDao = new StatesDao();
        readonly CitiesDao citiesDao = new CitiesDao();
        readonly MaterialesgptDao materialesgptDao = new MaterialesgptDao();
        readonly MaterialesDao materialesDao = new MaterialesDao();

        // GET: Listas
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult ReportesFiltroCliente(string Prefix, string cocodes)
        {
            if (Prefix == null)
                Prefix = "";
            string[] comcodessplit = { };
            string cocode = cocodes.ToString();
            if (!string.IsNullOrEmpty(cocode))
            {
                comcodessplit = cocode.Split(',');
            }

            if (comcodessplit.Any())
            {
                var c = (from cl in db.CLIENTEs
                         join p in db.PAIS on cl.LAND equals p.LAND
                         where cl.KUNNR.Contains(Prefix) && comcodessplit.Contains(p.SOCIEDAD_ID) && cl.ACTIVO && p.ACTIVO
                         orderby cl.NAME1
                         select new { cl.KUNNR, cl.NAME1 }).ToList();
                if (c.Count == 0)
                {
                    var c2 = (from cl in db.CLIENTEs
                              join p in db.PAIS on cl.LAND equals p.LAND
                              where cl.NAME1.Contains(Prefix) && comcodessplit.Contains(p.SOCIEDAD_ID) && cl.ACTIVO && p.ACTIVO
                              orderby cl.NAME1
                              select new { cl.KUNNR, cl.NAME1 }).ToList();
                    c.AddRange(c2);
                }
                JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
                return cc;
            }
            else
            {
                var c = (from cl in db.CLIENTEs
                         join p in db.PAIS on cl.LAND equals p.LAND
                         where cl.KUNNR.Contains(Prefix) && cl.ACTIVO && p.ACTIVO
                         orderby cl.NAME1
                         select new { cl.KUNNR, cl.NAME1 }).ToList();
                if (c.Count == 0)
                {
                    var c2 = (from cl in db.CLIENTEs
                              join p in db.PAIS on cl.LAND equals p.LAND
                              where cl.NAME1.Contains(Prefix) && cl.ACTIVO && p.ACTIVO
                              orderby cl.NAME1
                              select new { cl.KUNNR, cl.NAME1 }).ToList();
                    c.AddRange(c2);
                }
                JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
                return cc;
            }
        }
        [HttpGet]
        public JsonResult ReportesFiltroPaises(string cocodes)
        {
            string[] comcodessplit = { };
            string cocode = cocodes.ToString();
            if (!string.IsNullOrEmpty(cocode))
            {
                comcodessplit = cocode.Split(',');
            }
            

            var c = (from D in db.PAIS
                     where comcodessplit.Contains(D.SOCIEDAD_ID) && D.ACTIVO
                     orderby D.LANDX
                     select new { D.LAND, D.LANDX });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpGet]
        public JsonResult ReportesFiltroPayer(string Prefix, string cocodes)
        {
            string[] comcodessplit = { };
            string cocode = cocodes.ToString();
            if (!string.IsNullOrEmpty(cocode))
            {
                comcodessplit = cocode.Split(',');
            }
            

            var c = (from cl in db.CLIENTEs
                     join p in db.PAIS on cl.LAND equals p.LAND
                     where comcodessplit.Contains(p.SOCIEDAD_ID) && cl.NAME1.Contains(Prefix) && cl.ACTIVO && p.ACTIVO
                     orderby cl.NAME1
                     select new { cl.KUNNR, cl.NAME1 });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpGet]
        public JsonResult Clientes(string Prefix, string usuario=null, string pais= null,string sociedad_id=null)
        {
            if (sociedad_id!=null)
            {
                pais = db.SOCIEDADs.Find(sociedad_id).LAND;
            }
            var clientes = clientesDao.ListaClientes(Prefix,usuario,pais);
            JsonResult cc = Json(clientes, JsonRequestBehavior.AllowGet);
            return cc;
           

        }
        [HttpGet]
        public JsonResult Solicitudes(string Prefix,string sociedad_id,decimal? num_doci,decimal? num_docf, bool? autorizador,string usuario_id)
        {
            List<DOCUMENTO> solicitudes;
            if (autorizador != null && autorizador.Value)
            {
                 solicitudes = solicitudesDao.ListaSolicitudes(TATConstantes.ACCION_LISTA_SOLSPORAPROBADOR,Prefix, sociedad_id, num_doci, num_docf, usuario_id);
            }
            else
            {
                solicitudes = solicitudesDao.ListaSolicitudes(TATConstantes.ACCION_LISTA_SOLICITUDES, Prefix, sociedad_id, num_doci, num_docf);
            }
            JsonResult cc = Json(solicitudes, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpGet]
        public JsonResult Usuarios(string Prefix,string sociedad_id=null, int? autorizador=null)
        {
            //1-ACCION_LISTA_AUTORIZADOR (FLUJO)
            //2-ACCION_LISTA_AUTORIZADOR (USUARIOS)
            List<USUARIO> usuarios;
            if (autorizador!= null && autorizador.Value==1) {
                 usuarios = usuariosDao.ListaUsuarios(Prefix, TATConstantes.ACCION_LISTA_AUTORIZADOR_F,null, sociedad_id);
            }
            else if (autorizador != null && autorizador.Value == 2)
            {
                usuarios = usuariosDao.ListaUsuarios(Prefix, TATConstantes.ACCION_LISTA_AUTORIZADOR_U,null, sociedad_id);
            }
            else
            {
                usuarios = usuariosDao.ListaUsuarios(Prefix, TATConstantes.ACCION_LISTA_USUARIO,null, sociedad_id);
            }
            JsonResult cc = Json(usuarios, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpGet]
        public JsonResult Estados(string Prefix,string pais, string sociedad_id=null)
        {
            List<STATE> estados = statesDao.ListaStates(Prefix,pais, sociedad_id);
            JsonResult cc = Json(estados, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpGet]
        public JsonResult Ciudades(string Prefix,string estado)
        {
            List<CITy> ciudades = citiesDao.ListaCities(Prefix,estado);
            JsonResult cc = Json(ciudades, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpGet]
        public JsonResult Det_Aprob(string bukrs, string puesto, string spras)
        {
            int p = Int16.Parse(puesto);
            var c = (from N in db.DET_APROB
                     join St in db.PUESTOTs
                     on N.PUESTOA_ID equals St.PUESTO_ID
                     where N.BUKRS.Equals(bukrs) && N.PUESTOC_ID.Equals(p) && St.SPRAS_ID.Equals(spras)
                     //where N.BUKRS.Equals(bukrs) 
                     select new { N.PUESTOA_ID, St.TXT50 });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpGet]
        public JsonResult Det_Aprob2(string bukrs, string puesto, string spras)
        {
            int p = Int16.Parse(puesto);
            DET_APROBH dh = db.DET_APROBH.Where(a => a.SOCIEDAD_ID.Equals(bukrs) && a.PUESTOC_ID == p).OrderByDescending(a => a.VERSION).FirstOrDefault();
            if (dh != null)
            {
                var c = (from N in db.DET_APROBP
                         join St in db.PUESTOTs
                         on N.PUESTOA_ID equals St.PUESTO_ID
                         where N.SOCIEDAD_ID.Equals(bukrs) && N.PUESTOC_ID.Equals(p) && St.SPRAS_ID.Equals(spras) && N.VERSION.Equals(dh.VERSION)
                         //where N.BUKRS.Equals(bukrs) 
                         select new { N.POS, N.PUESTOA_ID.Value, St.TXT50, N.MONTO, PRESUPUESTO = N.PRESUPUESTO.Value }).ToList();

                TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(bukrs) && a.ACTIVO == true).FirstOrDefault();
                if (tl != null)
                {
                    var col = (from St in db.PUESTOTs
                               where St.PUESTO_ID == 9 && St.SPRAS_ID.Equals(spras)
                               //where N.BUKRS.Equals(bukrs) 
                               select new { POS = 98, Value = St.PUESTO_ID, St.TXT50, PRESUPUESTO = false });
                    foreach (var coll in col)
                    {
                        var colll = new { POS = 98,  coll.Value, coll.TXT50, MONTO = (decimal?)decimal.Parse("-1"), PRESUPUESTO = false };
                        c.Add(colll);
                    }
                }
                var ca = c.OrderBy(a => a.POS);
                JsonResult cc = Json(ca, JsonRequestBehavior.AllowGet);
                return cc;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public JsonResult UsuariosPuesto(string puesto, string Prefix)
        {
            //int p = Int16.Parse(puesto);
            var c = (from N in db.USUARIOs
                     where //N.PUESTO_ID == p & 
                     N.ID.Contains(Prefix)
                     //where N.BUKRS.Equals(bukrs) 
                     select new { N.ID, NOMBRE = N.ID + " - " + N.NOMBRE + " " + N.APELLIDO_P + " " + N.APELLIDO_M });
            if (c.Count() == 0)
            {
                c = (from N in db.USUARIOs
                     where //N.PUESTO_ID == p & 
                     N.NOMBRE.Contains(Prefix) || N.APELLIDO_P.Contains(Prefix) || N.APELLIDO_M.Contains(Prefix)
                     //where N.BUKRS.Equals(bukrs) 
                     select new { N.ID, NOMBRE = N.ID + " - " + N.NOMBRE + " " + N.APELLIDO_P + " " + N.APELLIDO_M });
            }
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpGet]
        public JsonResult Grupos(string pais, string user, string vkorg, string vtweg, string spart, string kunnr)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);
            
            var c = (from N in db.DET_AGENTEC
                     where N.PAIS_ID == pais
                     && N.USUARIOC_ID.Equals(user)
                     && N.VKORG.Equals(vkorg)
                     && N.VTWEG.Equals(vtweg)
                     && N.SPART.Equals(spart)
                     && N.KUNNR.Equals(kunnr)
                     select new { N.POS });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpPost]
        public JsonResult getPresupuesto(string kunnr, string periodo)
        {
            PRESUPUESTO_MOD pm;
            Presupuesto pr = new Presupuesto();
            Cadena c = new Cadena();
            pm = pr.getPresupuesto(c.completaCliente(kunnr), periodo);
            

            JsonResult cc = Json(pm, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpGet]
        public JsonResult Relacionados(string num_doc, string spras)
        {
            //var c = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF.Equals(num_doc));
            decimal num = (decimal.Parse(num_doc));
            List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO == true).ToList();
            DOCUMENTO d = db.DOCUMENTOes.Find(num);
            if (d.DOCUMENTORECs.Count > 0)
            {
                //Estatus e = new Estatus();
                //List<DOCUMENTO_MOD> c = (from DR in db.DOCUMENTORECs
                //                         join D in db.DOCUMENTOes
                //                         on DR.DOC_REF equals D.NUM_DOC
                //                         join T in db.TSOLTs
                //                         on D.TSOL_ID equals T.TSOL_ID
                //                         join TA in db.TALLs
                //                         on D.TALL_ID equals TA.ID
                //                         join G in db.GALLTs
                //                         on TA.GALL_ID equals G.GALL_ID
                //                         where DR.NUM_DOC == num
                //                         && T.SPRAS_ID == spras
                //                         && G.SPRAS_ID == spras
                //                         && D.NUM_DOC != 0
                //                         select new DOCUMENTO_MOD
                //                         {
                //                             NUM_DOC = D.NUM_DOC,
                //                             TSOL_ID = T.TXT020,
                //                             GALL_ID = G.TXT50,
                //                             ESTADO = D.FECHAD.Value.Year + "/" + D.FECHAD.Value.Month + "/" + D.FECHAD.Value.Day,
                //                             CIUDAD = D.HORAC.Value.ToString(),
                //                             ESTATUS = D.ESTATUS,
                //                             CONCEPTO = D.CONCEPTO,
                //                             MONTO_DOC_ML = D.MONTO_DOC_ML
                //                         }).ToList();
                //foreach (DOCUMENTO_MOD ddd in c)
                //{
                //    ddd.ESTATUS = e.getHtml(ddd.NUM_DOC);
                //}
                List<CSP_DOCUMENTOSRECCXSOL_Result> dOCUMENTOes = db.CSP_DOCUMENTOSRECCXSOL(num.ToString(), spras).ToList();
                List<Documento> listaDocs = new List<Documento>();

                foreach (CSP_DOCUMENTOSRECCXSOL_Result item in dOCUMENTOes)
                {
                    Documento ld = new Documento();
                    ld.DOCUMENTO_REF = 0;
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
                    Estatus eS = new Estatus();
                    ld.ESTATUS = eS.getText(item.ESTATUSS, ld.NUM_DOC, spras, ee);
                    ld.ESTATUS_CLASS = eS.getClass(item.ESTATUSS, ld.NUM_DOC, spras, ee);

                    ld.PAYER_ID = item.PAYER_ID;

                    ld.CLIENTE = item.NAME1;
                    ld.CANAL = item.CANAL;
                    ld.TSOL = item.TXT020;
                    ld.TALL = item.TXT50;
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

                    ld.USUARIOC_ID = String.IsNullOrEmpty(item.USUARIOD_ID)?"":item.USUARIOD_ID;
                    ld.USUARIOM_ID = String.IsNullOrEmpty(item.USUARIOD_ID) ? "" : item.USUARIOD_ID;

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
                        ld.NUM_CUENTA = "";
                        ld.NUM_PAYER = "";
                        ld.NUM_CLIENTE = "";
                        ld.NUM_IMPORTE = "";
                        ld.BLART = "";
                        ld.NUM_PRO = "";
                        ld.NUM_AP = "";
                        ld.NUM_NC = "";
                        ld.NUM_REV = "";
                    }

                    listaDocs.Add(ld);
                }
                JsonResult cc = Json(listaDocs, JsonRequestBehavior.AllowGet);
                return cc;
            }
            else//Contiene solicitudes relacionadas
            {
                //Estatus e = new Estatus();
                //List<DOCUMENTO_MOD> c = (from D in db.DOCUMENTOes
                //                         join T in db.TSOLTs
                //                         on D.TSOL_ID equals T.TSOL_ID
                //                         join TA in db.TALLs
                //                         on D.TALL_ID equals TA.ID
                //                         join G in db.GALLTs
                //                         on TA.GALL_ID equals G.GALL_ID
                //                         where D.DOCUMENTO_REF == num
                //                         && T.SPRAS_ID == spras
                //                         && G.SPRAS_ID == spras
                //                         select new DOCUMENTO_MOD
                //                         {
                //                             NUM_DOC = D.NUM_DOC,
                //                             SOCIEDAD_ID=D.SOCIEDAD_ID,
                //                             PAIS_ID=D.PAIS_ID,
                //                             TSOL_ID = T.TXT020,
                //                             GALL_ID = G.TXT50,
                //                             ESTADO = D.FECHAD.Value.Year + "/" + D.FECHAD.Value.Month + "/" + D.FECHAD.Value.Day,
                //                             CIUDAD = D.HORAC.Value.ToString(),
                //                             ESTATUS = D.ESTATUS,
                //                             CONCEPTO = D.CONCEPTO,
                //                             MONTO_DOC_ML = D.MONTO_DOC_ML
                //                         }).ToList();
                //foreach (DOCUMENTO_MOD ddd in c)
                //{
                //    ddd.ESTATUS = e.getHtml(ddd.NUM_DOC);
                //}
                List<CSP_DOCUMENTOSRELXSOL_Result> dOCUMENTOes = db.CSP_DOCUMENTOSRELXSOL(num.ToString(), spras).ToList();
                List<Documento> listaDocs = new List<Documento>();

                foreach (CSP_DOCUMENTOSRELXSOL_Result item in dOCUMENTOes)
                {
                    Documento ld = new Documento();
                    ld.DOCUMENTO_REF = item.DOCUMENTO_REF;
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
                    Estatus eS = new Estatus();
                    ld.ESTATUS = eS.getText(item.ESTATUSS, ld.NUM_DOC, spras, ee);
                    ld.ESTATUS_CLASS = eS.getClass(item.ESTATUSS, ld.NUM_DOC, spras, ee);

                    ld.PAYER_ID = item.PAYER_ID;

                    ld.CLIENTE = item.NAME1;
                    ld.CANAL = item.CANAL;
                    ld.TSOL = item.TXT020;
                    ld.TALL = item.TXT50;
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
                        ld.NUM_CUENTA = "";
                        ld.NUM_PAYER = "";
                        ld.NUM_CLIENTE = "";
                        ld.NUM_IMPORTE = "";
                        ld.BLART = "";
                        ld.NUM_PRO = "";
                        ld.NUM_AP = "";
                        ld.NUM_NC = "";
                        ld.NUM_REV = "";
                    }

                    listaDocs.Add(ld);
                }
                JsonResult cc = Json(listaDocs, JsonRequestBehavior.AllowGet);
                return cc;
            }
        }
        [HttpGet]
        public JsonResult Paises(string bukrs = null,string Prefix=null)
        {
            if (!string.IsNullOrEmpty(bukrs))
            {
               var c = (from D in db.PAIS
                     where D.SOCIEDAD_ID == bukrs
                     select new { D.LAND, D.LANDX });
                return Json(c, JsonRequestBehavior.AllowGet);
            }
            else {
                if (Prefix == null)
                    Prefix = "";
                 var c = (from sp in db.PAIS
                         where (sp.LAND.Contains(Prefix) || sp.LANDX.Contains(Prefix))
                         select new { sp.LAND,  sp.LANDX });
                return Json(c, JsonRequestBehavior.AllowGet);

            }
           
           
        }
        [HttpGet]
        public JsonResult selectTaxeo(string bukrs, string pais, string vkorg, string vtweg, string spart, string kunnr, string spras)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            var c = (from T in db.TAXEOHs
                     join TX in db.TX_CONCEPTOT
                     on T.CONCEPTO_ID equals TX.CONCEPTO_ID
                     where T.SOCIEDAD_ID == bukrs
                     && T.PAIS_ID == pais
                     && T.VKORG == vkorg
                     && T.VTWEG == vtweg
                     && T.SPART == spart
                     && T.KUNNR == kunnr
                     && TX.SPRAS_ID == spras
                     select new { T.CONCEPTO_ID, TX.TXT50 });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpPost]
        public JsonResult selectConcepto(string bukrs, string pais, string vkorg, string vtweg, string spart, string kunnr, string concepto, string spras)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);
            int co = int.Parse(concepto);
            var c = (from T in db.TAXEOHs
                     join TX in db.TX_NOTAT
                     on T.TNOTA_ID equals TX.TNOTA_ID
                     where T.SOCIEDAD_ID == bukrs
                     && T.PAIS_ID == pais
                     && T.VKORG == vkorg
                     && T.VTWEG == vtweg
                     && T.SPART == spart
                     && T.KUNNR == kunnr
                     && T.CONCEPTO_ID == co
                     && TX.SPRAS_ID == spras
                     select new { T.TNOTA_ID, TX.TXT50 });
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpPost]
        public JsonResult selectImpuesto(string bukrs, string pais, string vkorg, string vtweg, string spart, string kunnr, string concepto, string spras)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);
            int co = int.Parse(concepto);
            var c = (from T in db.TAXEOHs
                     where T.SOCIEDAD_ID == bukrs
                     && T.PAIS_ID == pais
                     && T.VKORG == vkorg
                     && T.VTWEG == vtweg
                     && T.SPART == spart
                     && T.KUNNR == kunnr
                     && T.CONCEPTO_ID == co
                     select new { T.IMPUESTO_ID, T.PORC, TXT50 = "IVA" }).ToList();

            var c2 = (from T in db.TAXEOPs
                      join R in db.TRETENCIONTs
                      on T.TRETENCION_ID equals R.TRETENCION_ID
                      where T.SOCIEDAD_ID == bukrs
                      && T.PAIS_ID == pais
                      && T.VKORG == vkorg
                      && T.VTWEG == vtweg
                      && T.SPART == spart
                      && T.KUNNR == kunnr
                      && T.CONCEPTO_ID == co
                      && R.SPRAS_ID == spras
                      select new { IMPUESTO_ID = T.RETENCION_ID.ToString(), T.PORC, R.TXT50 }).ToList();

            c.AddRange(c2);

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpPost]
        public JsonResult soportes(string tsol, string spras, string bo)
        {
            if (bo != "X")
            {
                var c = (from C in db.CONSOPORTEs
                         join T in db.TSOPORTETs
                         on C.TSOPORTE_ID equals T.TSOPORTE_ID
                         where C.TSOL_ID == tsol
                         && T.SPRAS_ID == spras
                         select new { C.TSOPORTE_ID, C.OBLIGATORIO, T.TXT50 });
                JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
                return cc;
            }
            else
            {
                var c = (from C in db.CONSOPORTEs
                         join T in db.TSOPORTETs
                         on C.TSOPORTE_ID equals T.TSOPORTE_ID
                         where (C.TSOL_ID == tsol
                         && T.SPRAS_ID == spras)
                         select new { C.TSOPORTE_ID, C.OBLIGATORIO, T.TXT50 }).ToList();
                var ca = (from T in db.TSOPORTETs
                         where  T.TSOPORTE_ID == "BO" && T.SPRAS_ID == spras
                          select new { T.TSOPORTE_ID, OBLIGATORIO = true, T.TXT50 }).ToList();
                c.AddRange(ca);
                JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
                return cc;
            }
        }
        [HttpPost]
        public JsonResult clearing(string bukrs, string land, string gall, string ejercicio,
            string tsol_id,decimal monto, decimal num_doc,string[] categorias=null)
        {
            bool esNC = false;
            decimal impuesto = FnCommon.ObtenerImpuesto(db, new DOCUMENTO {NUM_DOC = num_doc, MONTO_DOC_MD = Convert.ToDecimal(monto), SOCIEDAD_ID = bukrs, TSOL_ID = tsol_id, PAIS_ID = land}, ref esNC, categorias);
            decimal ejer = decimal.Parse(ejercicio);
            

            var c = (from C in db.CUENTAs
                     where C.SOCIEDAD_ID == bukrs
                     && C.PAIS_ID == land
                     && C.TALL_ID == gall
                     && C.EJERCICIO == ejer
                     select new { C.ABONO, NOMBREA = C.CUENTAGL.NOMBRE, C.CARGO, NOMBREC = C.CUENTAGL1.NOMBRE, C.CLEARING, NOMBRECL= C.CUENTAGL2.NOMBRE, C.LIMITE,IMPUESTO=impuesto }).FirstOrDefault();

            JsonResult cc = Json("", JsonRequestBehavior.AllowGet);
            if (c != null)
            {
                cc = Json(c, JsonRequestBehavior.AllowGet);
            }
            return cc;
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult categoriasCliente(string vkorg, string spart, string kunnr, string soc_id)
        {

            List<MATERIALGPT> list = materialesgptDao.CategoriasCliente(vkorg,spart,kunnr,soc_id);
            JsonResult jl = Json(list, JsonRequestBehavior.AllowGet);
            return jl;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult grupoMateriales(string vkorg, string spart, string kunnr, string soc_id)
        {
            List<CategoriaMaterial> lcatmat =materialesgptDao.GrupoMateriales(vkorg,spart,kunnr,soc_id,User.Identity.Name);
            

            JsonResult jl = Json(lcatmat, JsonRequestBehavior.AllowGet);
            return jl;
        }
        public CONFDIST_CAT getCatConf(string soc)
        {
            CONFDIST_CAT conf = new CONFDIST_CAT();

            try
            {
                conf = db.CONFDIST_CAT.Where(c => c.SOCIEDAD_ID == soc && c.ACTIVO == true).FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e,"Listas", "getCatConf");
            }

            return conf;
        }
        [HttpPost]
        public JsonResult getPeriodo(string fecha)
        {
            string f = "";
            Calendario445 c4 = new Calendario445();
            string[] ff = fecha.Split('/');
            f = c4.getPeriodo(new DateTime(int.Parse(ff[2]), int.Parse(ff[1]), int.Parse(ff[0]))).ToString();

            JsonResult jl = Json(f, JsonRequestBehavior.AllowGet);
            return jl;
        }
        [HttpPost]
        public JsonResult getPrimerDia(string ejercicio, string periodo)
        {
            int e = int.Parse(ejercicio);
            int p = int.Parse(periodo);
            Calendario445 c4 = new Calendario445();
            DateTime f = c4.getPrimerDia(e, p);

            JsonResult jl = Json(f.ToShortDateString(), JsonRequestBehavior.AllowGet);
            return jl;
        }
        [HttpPost]
        public JsonResult getPrimerViernes(string ejercicio, string periodo)
        {
            int e = int.Parse(ejercicio);
            int p = int.Parse(periodo);
            Calendario445 c4 = new Calendario445();
            DateTime f = c4.getPrimerDia(e, p);
            int daysUntilFriday = ((int)DayOfWeek.Friday - (int)f.DayOfWeek + 7) % 7;
            DateTime nextFridat = f.AddDays(daysUntilFriday);

            JsonResult jl = Json(nextFridat.ToShortDateString(), JsonRequestBehavior.AllowGet);
            return jl;
        }
        [HttpPost]
        public JsonResult getPrimerLunes(string ejercicio, string periodo)
        {
            int e = int.Parse(ejercicio);
            int p = int.Parse(periodo);
            if (p > 12)
            {
                e = e + (p / 12);
                p = p - 12;
            }
            Calendario445 c4 = new Calendario445();
            DateTime f = c4.getPrimerDia(e, p);
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)f.DayOfWeek + 7) % 7;

            DateTime nextMonday = f.AddDays(daysUntilMonday);

            JsonResult jl = Json(nextMonday.ToShortDateString(), JsonRequestBehavior.AllowGet);
            return jl;
        }

        [HttpPost]
        public JsonResult getUltimoDia(string ejercicio, string periodo)
        {
            int e = int.Parse(ejercicio);
            int p = int.Parse(periodo);
            Calendario445 c4 = new Calendario445();
            DateTime f = c4.getUltimoDia(e, p);

            JsonResult jl = Json(f.ToShortDateString(), JsonRequestBehavior.AllowGet);
            return jl;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SelectCliente(string kunnr,bool? esBorrador)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            CLIENTE_MOD id_cl = (from c in db.CLIENTEs
                                 join co in db.CONTACTOCs
                                 on new { c.VKORG, c.VTWEG, c.SPART, c.KUNNR } equals new { co.VKORG, co.VTWEG, co.SPART, co.KUNNR } into jjcont
                                 from co in jjcont.DefaultIfEmpty()
                                 where (c.KUNNR == kunnr && co.DEFECTO == true)
                                 select new CLIENTE_MOD
                                 {
                                     VKORG = c.VKORG,
                                     VTWEG = c.VTWEG,
                                     VTWEG2 = c.VTWEG,//RSG 05.07.2018
                                     SPART = c.SPART,//RSG 28.05.2018-------------------
                                     NAME1 = c.NAME1,
                                     KUNNR = c.KUNNR,
                                     STCD1 = c.STCD1,
                                     PARVW = c.PARVW,
                                     BANNER = c.BANNER,
                                     CANAL = c.CANAL,
                                     PAYER_NOMBRE = co == null ? String.Empty : co.NOMBRE,
                                     PAYER_EMAIL = co == null ? String.Empty : co.EMAIL,
                                 }).FirstOrDefault();

            if (id_cl == null)
            {
                id_cl = (from c in db.CLIENTEs
                         where (c.KUNNR == kunnr)
                         select new CLIENTE_MOD
                         {
                             VKORG = c.VKORG,
                             VTWEG = c.VTWEG,
                             VTWEG2 = c.VTWEG,//RSG 05.07.2018
                             SPART = c.SPART,//RSG 28.05.2018-------------------
                             NAME1 = c.NAME1,
                             KUNNR = c.KUNNR,
                             STCD1 = c.STCD1,
                             PARVW = c.PARVW,
                             BANNER = c.BANNER,
                             CANAL = c.CANAL,
                             PAYER_NOMBRE = String.Empty,
                             PAYER_EMAIL = String.Empty,
                         }).FirstOrDefault();
            }

            if (id_cl != null)
            {
                //Obtener el cliente
                CANAL canal = db.CANALs.Where(ca => ca.CANAL1 == id_cl.CANAL).FirstOrDefault();
                id_cl.VTWEG = "";

                if (canal != null)
                {
                    id_cl.VTWEG = canal.CANAL1 + " - " + canal.CDESCRIPCION;
                }

                //Obtener el tipo de cliente
                var clientei = (from c in db.TCLIENTEs
                                join ct in db.TCLIENTETs
                                on c.ID equals ct.PARVW_ID
                                where c.ID == id_cl.PARVW && c.ACTIVO
                                select ct).FirstOrDefault();
                id_cl.PARVW = "";
                if (clientei != null)
                {
                    id_cl.PARVW = clientei.TXT50;
                }

            }
            //Si es borrador asignar datos de contacto a cliente
            if (id_cl != null && esBorrador != null && esBorrador.Value)
            {
                DOCUMENTBORR doc = db.DOCUMENTBORRs.Where(x => x.USUARIOC_ID== User.Identity.Name && x.PAYER_ID== kunnr).FirstOrDefault();
                if (doc != null)
                {
                    id_cl.PAYER_EMAIL = doc.PAYER_EMAIL;
                    id_cl.PAYER_NOMBRE = doc.PAYER_NOMBRE;
                }
            }
            //Asignar Manager
            if (id_cl != null && db.CLIENTEFs.Any(x => x.KUNNR == id_cl.KUNNR && x.ACTIVO))
            {
                id_cl.MANAGER = db.CLIENTEFs.Where(x => x.KUNNR == id_cl.KUNNR &&  x.ACTIVO).First().USUARIO1_ID;
            }
            JsonResult jc = Json(id_cl, JsonRequestBehavior.AllowGet);
            return jc;
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult SelectClienteDup(string kunnr, bool? esBorrador, string num_doc, string bukrs)
        {
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

            CLIENTE_MOD id_cl = (from c in db.CLIENTEs
                                 join co in db.CONTACTOCs
                                 on new { c.VKORG, c.VTWEG, c.SPART, c.KUNNR } equals new { co.VKORG, co.VTWEG, co.SPART, co.KUNNR } into jjcont
                                 from co in jjcont.DefaultIfEmpty()
                                 where (c.KUNNR == kunnr && co.DEFECTO == true)
                                 select new CLIENTE_MOD
                                 {
                                     VKORG = c.VKORG,
                                     VTWEG = c.VTWEG,
                                     VTWEG2 = c.VTWEG,//RSG 05.07.2018
                                     SPART = c.SPART,//RSG 28.05.2018-------------------
                                     NAME1 = c.NAME1,
                                     KUNNR = c.KUNNR,
                                     STCD1 = c.STCD1,
                                     PARVW = c.PARVW,
                                     BANNER = c.BANNER,
                                     CANAL = c.CANAL,
                                     PAYER_NOMBRE = co == null ? String.Empty : co.NOMBRE,
                                     PAYER_EMAIL = co == null ? String.Empty : co.EMAIL,
                                 }).FirstOrDefault();

            if (id_cl == null)
            {
                id_cl = (from c in db.CLIENTEs
                         where (c.KUNNR == kunnr)
                         select new CLIENTE_MOD
                         {
                             VKORG = c.VKORG,
                             VTWEG = c.VTWEG,
                             VTWEG2 = c.VTWEG,//RSG 05.07.2018
                             SPART = c.SPART,//RSG 28.05.2018-------------------
                             NAME1 = c.NAME1,
                             KUNNR = c.KUNNR,
                             STCD1 = c.STCD1,
                             PARVW = c.PARVW,
                             BANNER = c.BANNER,
                             CANAL = c.CANAL,
                             PAYER_NOMBRE = String.Empty,
                             PAYER_EMAIL = String.Empty,
                         }).FirstOrDefault();
            }

            if (id_cl != null)
            {
                //Obtener el cliente
                CANAL canal = db.CANALs.Where(ca => ca.CANAL1 == id_cl.CANAL).FirstOrDefault();
                id_cl.VTWEG = "";

                if (canal != null)
                {
                    id_cl.VTWEG = canal.CANAL1 + " - " + canal.CDESCRIPCION;
                }

                //Obtener el tipo de cliente
                var clientei = (from c in db.TCLIENTEs
                                join ct in db.TCLIENTETs
                                on c.ID equals ct.PARVW_ID
                                where c.ID == id_cl.PARVW && c.ACTIVO
                                select ct).FirstOrDefault();
                id_cl.PARVW = "";
                if (clientei != null)
                {
                    id_cl.PARVW = clientei.TXT50;
                }

            }
            //Si es borrador asignar datos de contacto a cliente
            if (id_cl != null && esBorrador != null && esBorrador.Value)
            {
                DOCUMENTBORR doc = db.DOCUMENTBORRs.Where(x => x.USUARIOC_ID == User.Identity.Name && (x.PAYER_ID == kunnr || x.PAYER_ID == null)).FirstOrDefault();
                if (doc != null && !String.IsNullOrEmpty(doc.PAYER_NOMBRE) && !String.IsNullOrEmpty(doc.PAYER_EMAIL))
                {
                    id_cl.PAYER_EMAIL = doc.PAYER_EMAIL;
                    id_cl.PAYER_NOMBRE = doc.PAYER_NOMBRE;
                }
                else//ADD RSG 01.11.2018
                {
                    DOCUMENTO dp = db.DOCUMENTOes.Find(decimal.Parse(num_doc));
                    id_cl.PAYER_EMAIL = dp.PAYER_EMAIL;
                    id_cl.PAYER_NOMBRE = dp.PAYER_NOMBRE;
                }
            }
            //Asignar Manager
            if (id_cl != null && db.CLIENTEFs.Any(x => x.KUNNR == id_cl.KUNNR && x.ACTIVO))
            {
                ////id_cl.MANAGER = db.CLIENTEFs.Where(x => x.KUNNR == id_cl.KUNNR && x.ACTIVO).First().USUARIO1_ID;
                USUARIO user = db.USUARIOs.Find(User.Identity.Name);
                DET_APROBH dah = db.DET_APROBH.Where(a => a.SOCIEDAD_ID == bukrs && a.PUESTOC_ID == user.PUESTO_ID && a.ACTIVO)
                                    .OrderByDescending(a => a.VERSION).FirstOrDefault();
                if (dah != null)
                {
                    CLIENTEF cf = db.CLIENTEFs.Where(a => a.KUNNR.Equals(id_cl.KUNNR) && a.ACTIVO
                                   ).OrderByDescending(a => a.VERSION).FirstOrDefault();
                    ProcesaFlujo pf = new ProcesaFlujo();
                    id_cl.MANAGER = pf.determinaAgenteI(cf, dah).USUARIOA_ID;
                }

            }
            JsonResult jc = Json(id_cl, JsonRequestBehavior.AllowGet);
            return jc;
        }

        [HttpPost]
        public JsonResult materiales(string Prefix, string vkorg, string vtweg)
        {
            List<MATERIAL> materiales = materialesDao.ListaMateriales( Prefix, vkorg, vtweg, User.Identity.Name);
            
            JsonResult cc = new JsonResult()
            {
                Data = materiales,
                MaxJsonLength = Int32.MaxValue
            };
            return cc;
        }

        public string tipoRecurrencia(string tsol)
        {
            string tipo = "";
            var aa = db.TSOLs.Where(a => a.ID.Equals(tsol)).FirstOrDefault();
            if (aa != null)
            {
                tipo = aa.TRECU;
            }
            return tipo;
        }

        //B20180710 MGC 2018.07.13 Modificaciones para editar los campos de distribución se agrego los objetos
        [HttpPost]
        [AllowAnonymous]
        public ActionResult getPartialMat(List<DOCUMENTOP_MOD> docs)
        {

            CartaV doc = new CartaV();

            doc.DOCUMENTOP = docs;
            return PartialView("~/Views/CartaV/_PartialMatTr.cshtml", doc);
        }
        [HttpGet]
        public JsonResult contactos(string Prefix, string kunnr, string vkorg = null, string vtweg = null)
        {
            
            Cadena cad = new Cadena();
            kunnr = cad.completaCliente(kunnr);

           var contactos= contactosDao.ListaContactos(Prefix,vkorg,vtweg,kunnr);
            JsonResult cc = Json(contactos, JsonRequestBehavior.AllowGet);
            return cc;
        }


        [HttpGet]
        public JsonResult tiposSolicitud(string Prefix)
        {
            string spras_id = FnCommon.ObtenerSprasId(db,User.Identity.Name);
            
            var tsr = (from ts in db.TSOLTs
                        where spras_id == ts.SPRAS_ID && (ts.TSOL_ID.Contains(Prefix)|| ts.TXT50.Contains(Prefix))
                        select new { ts.TSOL_ID, TXT50=(ts.TSOL_ID+" - "+ts.TXT50) }).ToList();
          
            return Json(tsr, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult soportes(string Prefix)
        {
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            var c = (from st in db.TSOPORTETs
                     where st.SPRAS_ID == spras_id && (st.TXT50.Contains(Prefix) || st.TSOPORTE_ID.Contains(Prefix))
                     select new { st.TSOPORTE_ID, TXT50=(st.TSOPORTE_ID + " - "+st.TXT50) });

            return Json(c, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
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
        [HttpPost]
        public int periodo(string sociedad_id, string tsol_id, string usuario_id)
        {
            return FnCommon.ObtenerPeriodoCalendario445(db,sociedad_id,tsol_id,usuario_id);
        }


        [HttpGet]
        public JsonResult Sociedades(string Prefix)
        {
            var sociedades=sociedadesDao.ListaSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES, null,null,Prefix);
            return Json(sociedades, JsonRequestBehavior.AllowGet);
        }
        

        [HttpGet]
        public JsonResult tipoAllowance(string Prefix)
        {
            var c = (from st in db.TALLs
                     where (st.ID.Contains(Prefix)|| st.DESCRIPCION.Contains(Prefix))
                     select new { st.ID, TXT50 = (st.DESCRIPCION) });

            return Json(c, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult cuentas(string Prefix)
        {
            var c = (from st in db.CUENTAGLs
                     where st.ACTIVO==true && (st.ID.ToString().Contains(Prefix)||st.NOMBRE.Contains(Prefix))
                     select new { st.ID, TXT50 = (st.ID + " - " + st.NOMBRE) });

            return Json(c, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult impuestos(string Prefix)
        {
            var c = (from st in db.IMPUESTOes
                     where st.ACTIVO && (st.MWSKZ.Contains(Prefix))
                     select new { ID = st.MWSKZ, TXT50 = (st.MWSKZ) });

            return Json(c, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Talls(string Prefix, string sociedad_id)
        {
            string spras_id = FnCommon.ObtenerSprasId(db,User.Identity.Name);
            int ejercicio = DateTime.Now.Year;
            string pais_id = db.SOCIEDADs.Any(x => x.BUKRS == sociedad_id) ? db.SOCIEDADs.First(x => x.BUKRS == sociedad_id).LAND:null ;

            var talls = tallsDao.ListaTallsConCuenta(TATConstantes.ACCION_LISTA_TALLTCONCUENTA, Prefix, spras_id, pais_id,ejercicio,sociedad_id);
            JsonResult cc = Json(talls, JsonRequestBehavior.AllowGet);
            return cc;


        }
        [HttpGet]
        public JsonResult TiposClientes(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";
            

            var c = (from x in db.TCLIENTEs
                     where x.ID.Contains(Prefix) && x.ACTIVO
                     select new { x.ID }).ToList();

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpGet]
        public JsonResult Spras(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";
            
            var c = (from x in db.SPRAS
                     where (x.ID.Contains(Prefix) || x.DESCRIPCION.Contains(Prefix))&& x.ID!="PT"
                     select new { x.ID, x.DESCRIPCION }).ToList();
            
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        [HttpGet]
        public JsonResult Vendors(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";
            

            var c = (from x in db.PROVEEDORs
                     where (x.ID.Contains(Prefix) || x.NOMBRE.Contains(Prefix)) && x.ACTIVO 
                     select new { x.ID, x.NOMBRE }).ToList();

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        [HttpGet]
        public JsonResult Canales(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";
            
            var c = (from x in db.CANALs
                     where (x.CANAL1.Contains(Prefix) || x.CDESCRIPCION.Contains(Prefix))
                     select new { x.CANAL1, x.CDESCRIPCION }).ToList();
           
            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
    }
}