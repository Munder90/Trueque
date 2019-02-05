using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Services;

namespace TAT001.Controllers
{
    [AllowAnonymous]
    public class CorreosController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Correos
        public ActionResult Index(decimal id, bool? mail, string spras) //B20180803 MGC Correos
        {
            if (spras == "" | spras == null)
            {
                spras = "ES";
            }

            string pathReplace = "";
            string[] pathReplaceArr;
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
            ViewBag.Pos = flujo[0];
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            pathReplace = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplaceArr = pathReplace.Split(new string[] { "?spras" },StringSplitOptions.None);
            ViewBag.url = pathReplaceArr[0];
            //ViewBag.miles = dOCUMENTOes.PAI.MILES;//LEJGG 090718
            //ViewBag.dec = dOCUMENTOes.PAI.DECIMAL;//LEJGG 090718
            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL) + " " + dOCUMENTO.MONEDA_ID;
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............

            //B20180803 MGC Presupuesto............
            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();

            dOCUMENTO.PAI = db.PAIS.Where(a => a.LAND.Equals(dOCUMENTO.PAIS_ID)).FirstOrDefault();
            if (dOCUMENTO.PAI != null)
            {
                ViewBag.miles = dOCUMENTO.PAI.MILES;//LEJGG 090718
                ViewBag.dec = dOCUMENTO.PAI.DECIMAL;//LEJGG 090718
            }

            CLIENTE_MOD cli = new CLIENTE_MOD();

            cli = SelectCliente(dOCUMENTO.PAYER_ID);

            ViewBag.kunnr = cli.KUNNR + "-" + cli.NAME1;
            ViewBag.vtweg = cli.VTWEG;

            Services.FormatosC format = new FormatosC();

            PRESUPUESTO_MOD presu = new PRESUPUESTO_MOD();
            presu = getPresupuesto(dOCUMENTO.PAYER_ID, dOCUMENTO.PERIODO.ToString());

            decimal pcanal = 0;
            try
            {
                pcanal = Convert.ToDecimal(presu.P_CANAL) / 1;
            }
            catch (Exception)
            {

            }
            decimal pbanner = 0;
            try
            {
                pbanner = Convert.ToDecimal(presu.P_BANNER) / 1;
            }
            catch (Exception)
            {

            }
            decimal pcc = 0;
            try
            {
                pcc = Convert.ToDecimal(presu.PC_C) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pca = 0;
            try
            {
                pca = Convert.ToDecimal(presu.PC_A) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pcp = 0;
            try
            {
                pcp = Convert.ToDecimal(presu.PC_P) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pct = 0;
            try
            {
                pct = Convert.ToDecimal(presu.PC_T) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal consu = 0;
            try
            {
                consu = Convert.ToDecimal(presu.CONSU) / 1;
            }
            catch (Exception)
            {

            }
            ViewBag.pcan = format.toShowG(pcanal, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pban = format.toShowG(pbanner, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcc = format.toShowG(pcc, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pca = format.toShowG(pca, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcp = format.toShowG(pcp, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pct = format.toShowG(pct, dOCUMENTO.PAI.DECIMAL);
            ViewBag.consu = format.toShowG(consu, dOCUMENTO.PAI.DECIMAL);

            SOLICITUD_MOD sol = new SOLICITUD_MOD();
            if (dOCUMENTO.DOCUMENTO_REF == null)
                sol = getSolicitud("0.00", dOCUMENTO.MONTO_DOC_MD + "", dOCUMENTO.PAI.DECIMAL);
            else
                sol = getSolicitud(dOCUMENTO.DOCUMENTO_REF + "", dOCUMENTO.MONTO_DOC_MD + "", dOCUMENTO.PAI.DECIMAL);

            ViewBag.S_IMPA = sol.S_IMPA;
            ViewBag.S_IMPB = sol.S_IMPB;
            ViewBag.S_IMPC = sol.S_IMPC;
            ViewBag.S_MONTOA = sol.S_MONTOA;
            ViewBag.S_MONTOB = sol.S_MONTOB;
            ViewBag.S_MONTOP = sol.S_MONTOP;
            ViewBag.S_NUM = sol.S_NUM;
            ViewBag.S_REMA = sol.S_REMA;
            ViewBag.rema_color = "";
            if (format.toNum(sol.S_REMA, dOCUMENTO.PAI.MILES, dOCUMENTO.PAI.DECIMAL) < 0)
                ViewBag.rema_color = "#F44336 !important";

            ViewBag.S_RET = sol.S_RET;
            ViewBag.S_TOTAL = sol.S_TOTAL;

            //B20180803 MGC Presupuesto............
            ViewBag.nombreUsuario = db.USUARIOs.Where(x => x.ID == dOCUMENTO.USUARIOC_ID).FirstOrDefault().NOMBRE;
            ViewBag.idUsuario = dOCUMENTO.USUARIOC_ID;
            ObtenerAnalisisSolicitud(dOCUMENTO);//ADD RSG 13.11.2018


            //ADD LGPP 13.12.2018
            ViewBag.nombreUsuario = db.USUARIOs.Where(x => x.ID == dOCUMENTO.USUARIOC_ID).FirstOrDefault().NOMBRE;
            ViewBag.idUsuario = dOCUMENTO.USUARIOC_ID;
            ViewBag.bandera = dOCUMENTO.PAIS_ID;
            //SECCION DE CABECERAS - APROBACION
            ViewBag.tituloCabeceraAprobacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_aprobacion").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabeceraAprobacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_aprobacion").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabeceraAprobacion1 = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_aprobacion1").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabeceraAprobacion2 = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_aprobacion2").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabeceraAprobacion3 = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_aprobacion3").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabeceraAprobacion4 = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_aprobacion4").FirstOrDefault().TEXTOS;
            ViewBag.btnAceptar = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "btn_aceptar").FirstOrDefault().TEXTOS;
            ViewBag.btnCancelar = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "btn_cancelar").FirstOrDefault().TEXTOS;

            //SECCION DE CABECERAS - NOTIFICACION
            ViewBag.tituloCabeceraNotificacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_notificacion").FirstOrDefault().TEXTOS;
            ViewBag.cabeceraSoporte = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_notificacion2").FirstOrDefault().TEXTOS;
            ViewBag.cabeceraReverso = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_notificacion3").FirstOrDefault().TEXTOS;
            ViewBag.soporteExterno = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_notificacion1").FirstOrDefault().TEXTOS;
            ViewBag.reverso = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "SubHead_notificacion2").FirstOrDefault().TEXTOS;

            //SECCION DE INFORMACION DEL USUARIO
            ViewBag.usuario = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_usuario").FirstOrDefault().TEXTOS;
            ViewBag.usuarioNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_nombreUsu").FirstOrDefault().TEXTOS;
            ViewBag.tipoSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_tsol").FirstOrDefault().TEXTOS;
            ViewBag.clasificacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_clasificacion").FirstOrDefault().TEXTOS;
            ViewBag.cliente = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cliente").FirstOrDefault().TEXTOS;
            ViewBag.clienteNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_nombreCli").FirstOrDefault().TEXTOS;
            //SECCION DE ANALISIS DE SOLICITUD
            ViewBag.analisisSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_analisis").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoSolicitud").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoProvision = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoProvision").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoAplicado = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoAplicado").FirstOrDefault().TEXTOS;
            ViewBag.lblRemanente = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_remanente").FirstOrDefault().TEXTOS;
            ViewBag.lblImpuesto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_impuesto").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoTotal = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoTotal").FirstOrDefault().TEXTOS;
            //SECCION DE VIGENCIA
            ViewBag.vigencia = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_vigencia").FirstOrDefault().TEXTOS;
            ViewBag.vigenciaDe = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_vigenciaDe").FirstOrDefault().TEXTOS;
            ViewBag.vigenciaA = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_vigenciaA").FirstOrDefault().TEXTOS;
            ViewBag.concepto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_concepto").FirstOrDefault().TEXTOS;
            ViewBag.montoDoc = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoDoc").FirstOrDefault().TEXTOS;
            //SECCION DE ANALISIS DE PRESUPUESTO
            ViewBag.analisisPresupuesto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_presupuesto").FirstOrDefault().TEXTOS;
            ViewBag.fechaCarga = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_ultFechCarga").FirstOrDefault().TEXTOS;
            ViewBag.presupuestoCanal = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_presupuestoCanal").FirstOrDefault().TEXTOS;
            ViewBag.preCliBaAgrupador = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_preCliBaAgrupador").FirstOrDefault().TEXTOS;
            ViewBag.registroSap = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_registroSap").FirstOrDefault().TEXTOS;
            ViewBag.procesoIcon = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_enProceso").FirstOrDefault().TEXTOS;
            ViewBag.consumido = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_consumido").FirstOrDefault().TEXTOS;
            ViewBag.excedido = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_preDisExcedido").FirstOrDefault().TEXTOS;
            //SECCION RAZON DE REVERSO
            ViewBag.headReverso = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_reverso").FirstOrDefault().TEXTOS;
            ViewBag.razonReverso = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_razonReverso").FirstOrDefault().TEXTOS;
            ViewBag.tipoReverso = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_tipoReverso").FirstOrDefault().TEXTOS;
            //SECCION PARA DOCUMENTOS RELACIONADOS EN EL REVERSO
            ViewBag.relacionados = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tab_relacionados").FirstOrDefault().TEXTOS;
            ViewBag.relTipo = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tabhead_tipo").FirstOrDefault().TEXTOS;
            ViewBag.relDocumento = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tabhead_documento").FirstOrDefault().TEXTOS;
            ViewBag.relSap = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tabhead_sap").FirstOrDefault().TEXTOS;
            ViewBag.relSociedad = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tabhead_sociedad").FirstOrDefault().TEXTOS;
            ViewBag.relPeriodo = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tabhead_periodo").FirstOrDefault().TEXTOS;
            ViewBag.relEjercicio = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "tabhead_ejercicio").FirstOrDefault().TEXTOS;
            //SECCION PIE DE LA CANCELACION
            ViewBag.pieAprobacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "pie_aprobacion").FirstOrDefault().TEXTOS;
            ViewBag.pieSoporte = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "pie_soporte").FirstOrDefault().TEXTOS;
            //END LGPP 13.12.2018
            return View(dOCUMENTO);
        }

        public ActionResult Cancelacion(decimal id, bool? mail, string spras)
        {
            if (spras == "" | spras == null)
            {
                spras = "ES";
            }

            string pathReplace = "";
            string[] pathReplaceArr;
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
            ViewBag.Pos = flujo[0];
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplace = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplaceArr = pathReplace.Split(new string[] { "?spras" }, StringSplitOptions.None);
            ViewBag.url = pathReplaceArr[0];
            //ViewBag.miles = dOCUMENTOes.PAI.MILES;//LEJGG 090718
            //ViewBag.dec = dOCUMENTOes.PAI.DECIMAL;//LEJGG 090718
            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL) + " " + dOCUMENTO.MONEDA_ID;
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............

            //B20180803 MGC Presupuesto............
            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();

            dOCUMENTO.PAI = db.PAIS.Where(a => a.LAND.Equals(dOCUMENTO.PAIS_ID)).FirstOrDefault();
            if (dOCUMENTO.PAI != null)
            {
                ViewBag.miles = dOCUMENTO.PAI.MILES;//LEJGG 090718
                ViewBag.dec = dOCUMENTO.PAI.DECIMAL;//LEJGG 090718
            }

            CLIENTE_MOD cli = new CLIENTE_MOD();

            cli = SelectCliente(dOCUMENTO.PAYER_ID);

            ViewBag.kunnr = cli.KUNNR + "-" + cli.NAME1;
            ViewBag.vtweg = cli.VTWEG;

            Services.FormatosC format = new FormatosC();

            PRESUPUESTO_MOD presu = new PRESUPUESTO_MOD();
            presu = getPresupuesto(dOCUMENTO.PAYER_ID, dOCUMENTO.PERIODO.ToString());

            decimal pcanal = 0;
            try
            {
                pcanal = Convert.ToDecimal(presu.P_CANAL) / 1;
            }
            catch (Exception)
            {

            }
            decimal pbanner = 0;
            try
            {
                pbanner = Convert.ToDecimal(presu.P_BANNER) / 1;
            }
            catch (Exception)
            {

            }
            decimal pcc = 0;
            try
            {
                pcc = Convert.ToDecimal(presu.PC_C) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pca = 0;
            try
            {
                pca = Convert.ToDecimal(presu.PC_A) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pcp = 0;
            try
            {
                pcp = Convert.ToDecimal(presu.PC_P) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pct = 0;
            try
            {
                pct = Convert.ToDecimal(presu.PC_T) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal consu = 0;
            try
            {
                consu = Convert.ToDecimal(presu.CONSU) / 1;
            }
            catch (Exception)
            {

            }
            ViewBag.pcan = format.toShowG(pcanal, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pban = format.toShowG(pbanner, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcc = format.toShowG(pcc, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pca = format.toShowG(pca, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcp = format.toShowG(pcp, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pct = format.toShowG(pct, dOCUMENTO.PAI.DECIMAL);
            ViewBag.consu = format.toShowG(consu, dOCUMENTO.PAI.DECIMAL);

            SOLICITUD_MOD sol = new SOLICITUD_MOD();
            if (dOCUMENTO.DOCUMENTO_REF == null)
                sol = getSolicitud("0.00", dOCUMENTO.MONTO_DOC_MD + "", dOCUMENTO.PAI.DECIMAL);
            else
                sol = getSolicitud(dOCUMENTO.DOCUMENTO_REF + "", dOCUMENTO.MONTO_DOC_MD + "", dOCUMENTO.PAI.DECIMAL);

            ViewBag.S_IMPA = sol.S_IMPA;
            ViewBag.S_IMPB = sol.S_IMPB;
            ViewBag.S_IMPC = sol.S_IMPC;
            ViewBag.S_MONTOA = sol.S_MONTOA;
            ViewBag.S_MONTOB = sol.S_MONTOB;
            ViewBag.S_MONTOP = sol.S_MONTOP;
            ViewBag.S_NUM = sol.S_NUM;
            ViewBag.S_REMA = sol.S_REMA;
            ViewBag.rema_color = "";
            if (format.toNum(sol.S_REMA, dOCUMENTO.PAI.MILES, dOCUMENTO.PAI.DECIMAL) < 0)
                ViewBag.rema_color = "#F44336 !important";

            ViewBag.S_RET = sol.S_RET;
            ViewBag.S_TOTAL = sol.S_TOTAL;

            //B20180803 MGC Presupuesto............

            //ADD LGPP 13.12.2018
            ViewBag.nombreUsuario = db.USUARIOs.Where(x => x.ID == dOCUMENTO.USUARIOC_ID).FirstOrDefault().NOMBRE;
            ViewBag.idUsuario = dOCUMENTO.USUARIOC_ID;
            ViewBag.bandera = dOCUMENTO.PAIS_ID;
            //SECCION DE CABECERAS
            ViewBag.tituloCabecera = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cabCancelacion").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabecera = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cabInfCancelacion").FirstOrDefault().TEXTOS;
            //SECCION DE INFORMACION DEL USUARIO
            ViewBag.usuario = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_usuario").FirstOrDefault().TEXTOS;
            ViewBag.usuarioNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_nombreUsu").FirstOrDefault().TEXTOS;
            ViewBag.tipoSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_tsol").FirstOrDefault().TEXTOS;
            ViewBag.clasificacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_clasificacion").FirstOrDefault().TEXTOS;
            ViewBag.cliente = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cliente").FirstOrDefault().TEXTOS;
            ViewBag.clienteNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_nombreCli").FirstOrDefault().TEXTOS;
            //SECCION DE ANALISIS DE SOLICITUD
            ViewBag.analisisSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_analisis").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoSolicitud").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoProvision = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoProvision").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoAplicado = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoAplicado").FirstOrDefault().TEXTOS;
            ViewBag.lblRemanente = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_remanente").FirstOrDefault().TEXTOS;
            ViewBag.lblImpuesto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_impuesto").FirstOrDefault().TEXTOS;
            ViewBag.lblMontoTotal = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoTotal").FirstOrDefault().TEXTOS;
            //SECCION DE VIGENCIA
            ViewBag.vigencia = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_vigencia").FirstOrDefault().TEXTOS;
            ViewBag.vigenciaDe = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_vigenciaDe").FirstOrDefault().TEXTOS;
            ViewBag.vigenciaA = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_vigenciaA").FirstOrDefault().TEXTOS;
            ViewBag.concepto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_concepto").FirstOrDefault().TEXTOS;
            ViewBag.montoDoc = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoDoc").FirstOrDefault().TEXTOS;
            //SECCION DE ANALISIS DE PRESUPUESTO
            ViewBag.analisisPresupuesto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "head_presupuesto").FirstOrDefault().TEXTOS;
            ViewBag.fechaCarga = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_ultFechCarga").FirstOrDefault().TEXTOS;
            ViewBag.presupuestoCanal = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_presupuestoCanal").FirstOrDefault().TEXTOS;
            ViewBag.preCliBaAgrupador = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_preCliBaAgrupador").FirstOrDefault().TEXTOS;
            ViewBag.registroSap = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_registroSap").FirstOrDefault().TEXTOS;
            ViewBag.procesoIcon = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_enProceso").FirstOrDefault().TEXTOS;
            ViewBag.consumido = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_consumido").FirstOrDefault().TEXTOS;
            ViewBag.excedido = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_preDisExcedido").FirstOrDefault().TEXTOS;
            //SECCION PIE DE LA CANCELACION
            ViewBag.pie = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_pieCancelacion").FirstOrDefault().TEXTOS;
            //END LGPP 13.12.2018

            ObtenerAnalisisSolicitud(dOCUMENTO);//ADD RSG 13.11.2018
            return View(dOCUMENTO);
        }

        public ActionResult Backup(string usu, string usu2, string spras)
        {
            USUARIO usuario1 = db.USUARIOs.Where(x => x.ID == usu).FirstOrDefault();

            if (spras == "" | spras == null)
            {
                spras = "ES";
            }

            List<CSP_DOCUMENTOSXUSER2_Result> dOCUMENTOes = db.CSP_DOCUMENTOSXUSER2(usu, spras).ToList();

            dOCUMENTOes = dOCUMENTOes.Where(x => x.ESTATUS == "P").ToList();
            //List<CSP_DOCUMENTOSXUSER2_Result> dOCUMENTOes2;

            //foreach (CSP_DOCUMENTOSXUSER2_Result doc in dOCUMENTOes)
            //{
            //    if (doc.ESTATUS == "P")
            //    {
            //        dOCUMENTOes2.Add(doc);
            //    }
            //}

            string pathReplace = "";
            string[] pathReplaceArr;

            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplace = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplaceArr = pathReplace.Split(new string[] { "?usu" }, StringSplitOptions.None);
            ViewBag.url = pathReplaceArr[0];

            //ADD LGPP 13.12.2018
            ViewBag.nombreUsuario = db.USUARIOs.Where(x => x.ID == usu).FirstOrDefault().NOMBRE;
            ViewBag.idUsuario = usu;
            ViewBag.bandera = db.SOCIEDADs.Where(x => x.BUKRS == usuario1.BUNIT).FirstOrDefault().LAND;
            //SECCION DE CABECERAS
            ViewBag.tituloCabecera = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cabBackup").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabecera = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cabInfBackup").FirstOrDefault().TEXTOS;
            //SECCION DE LA TABLA DE BACKUP
            ViewBag.headNum = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headNum").FirstOrDefault().TEXTOS;
            ViewBag.headCocode = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headCocode").FirstOrDefault().TEXTOS;
            ViewBag.headPeriodo = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headPeriodo").FirstOrDefault().TEXTOS;
            ViewBag.headEstatus = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headEstatus").FirstOrDefault().TEXTOS;
            ViewBag.headClienteId = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headClienteId").FirstOrDefault().TEXTOS;
            ViewBag.headClienteNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headClienteNombre").FirstOrDefault().TEXTOS;
            ViewBag.headTsol = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headTsol").FirstOrDefault().TEXTOS;
            ViewBag.headClasificacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headClasificacion").FirstOrDefault().TEXTOS;
            ViewBag.headImporte = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "headImporte").FirstOrDefault().TEXTOS;
            //SECCION PIE DE LA CANCELACION
            ViewBag.pie = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_pieBackup").FirstOrDefault().TEXTOS;
            //END LGPP 13.12.2018

            return View(dOCUMENTOes);
        }

        public ActionResult Taxeo(decimal id, bool? mail)
        {
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
            ViewBag.Pos = flujo[0];
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            //ViewBag.miles = dOCUMENTOes.PAI.MILES;//LEJGG 090718
            //ViewBag.dec = dOCUMENTOes.PAI.DECIMAL;//LEJGG 090718
            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL) + " " + dOCUMENTO.MONEDA_ID;
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............

            //B20180803 MGC Presupuesto............
            Models.PresupuestoModels carga = new Models.PresupuestoModels();
            ViewBag.ultMod = carga.consultarUCarga();

            dOCUMENTO.PAI = db.PAIS.Where(a => a.LAND.Equals(dOCUMENTO.PAIS_ID)).FirstOrDefault();
            if (dOCUMENTO.PAI != null)
            {
                ViewBag.miles = dOCUMENTO.PAI.MILES;//LEJGG 090718
                ViewBag.dec = dOCUMENTO.PAI.DECIMAL;//LEJGG 090718
            }

            CLIENTE_MOD cli = new CLIENTE_MOD();

            cli = SelectCliente(dOCUMENTO.PAYER_ID);

            ViewBag.kunnr = cli.KUNNR + "-" + cli.NAME1;
            ViewBag.vtweg = cli.VTWEG;

            Services.FormatosC format = new FormatosC();

            PRESUPUESTO_MOD presu = new PRESUPUESTO_MOD();
            presu = getPresupuesto(dOCUMENTO.PAYER_ID, dOCUMENTO.PERIODO.ToString());

            decimal pcanal = 0;
            try
            {
                pcanal = Convert.ToDecimal(presu.P_CANAL) / 1;
            }
            catch (Exception)
            {

            }
            decimal pbanner = 0;
            try
            {
                pbanner = Convert.ToDecimal(presu.P_BANNER) / 1;
            }
            catch (Exception)
            {

            }
            decimal pcc = 0;
            try
            {
                pcc = Convert.ToDecimal(presu.PC_C) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pca = 0;
            try
            {
                pca = Convert.ToDecimal(presu.PC_A) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pcp = 0;
            try
            {
                pcp = Convert.ToDecimal(presu.PC_P) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal pct = 0;
            try
            {
                pct = Convert.ToDecimal(presu.PC_T) / 1 * -1;
            }
            catch (Exception)
            {

            }
            decimal consu = 0;
            try
            {
                consu = Convert.ToDecimal(presu.CONSU) / 1;
            }
            catch (Exception)
            {

            }
            ViewBag.pcan = format.toShowG(pcanal, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pban = format.toShowG(pbanner, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcc = format.toShowG(pcc, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pca = format.toShowG(pca, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pcp = format.toShowG(pcp, dOCUMENTO.PAI.DECIMAL);
            ViewBag.pct = format.toShowG(pct, dOCUMENTO.PAI.DECIMAL);
            ViewBag.consu = format.toShowG(consu, dOCUMENTO.PAI.DECIMAL);

            SOLICITUD_MOD sol = new SOLICITUD_MOD();
            if (dOCUMENTO.DOCUMENTO_REF == null)
                sol = getSolicitud("0.00", dOCUMENTO.MONTO_DOC_MD + "", dOCUMENTO.PAI.DECIMAL);
            else
                sol = getSolicitud(dOCUMENTO.DOCUMENTO_REF + "", dOCUMENTO.MONTO_DOC_MD + "", dOCUMENTO.PAI.DECIMAL);

            ViewBag.S_IMPA = sol.S_IMPA;
            ViewBag.S_IMPB = sol.S_IMPB;
            ViewBag.S_IMPC = sol.S_IMPC;
            ViewBag.S_MONTOA = sol.S_MONTOA;
            ViewBag.S_MONTOB = sol.S_MONTOB;
            ViewBag.S_MONTOP = sol.S_MONTOP;
            ViewBag.S_NUM = sol.S_NUM;
            ViewBag.S_REMA = sol.S_REMA;
            ViewBag.rema_color = "";
            if (format.toNum(sol.S_REMA, dOCUMENTO.PAI.MILES, dOCUMENTO.PAI.DECIMAL) < 0)
                ViewBag.rema_color = "#F44336 !important";

            ViewBag.S_RET = sol.S_RET;
            ViewBag.S_TOTAL = sol.S_TOTAL;

            //B20180803 MGC Presupuesto............

            ObtenerAnalisisSolicitud(dOCUMENTO);//ADD RSG 13.11.2018
            return View(dOCUMENTO);
        }

        void ObtenerAnalisisSolicitud(DOCUMENTO D)
        {
            FormatosC format = new FormatosC();
            decimal montoProv = 0.0M;
            decimal montoApli = 0.0M;
            decimal remanente = 0.0M;
            decimal impuesto = 0.0M;
            bool esDocRef = false;
            bool esProv = false;
            bool esNC = false;


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

            if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == D.NUM_DOC && x.ESTATUS_C == null))
            {
                esDocRef = true;
                montoApli = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == D.NUM_DOC && x.ESTATUS_C == null).Sum(x => x.MONTO_DOC_MD.Value);
            }
            else if (D.DOCUMENTO_REF != null)
            {
                esDocRef = true;
                if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == D.DOCUMENTO_REF && x.ESTATUS_C == null))
                {
                    montoApli = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == D.DOCUMENTO_REF && x.ESTATUS_C == null).Sum(x => x.MONTO_DOC_MD.Value);
                }
            }
            if (montoProv > 0 && montoApli > 0)
            {
                remanente = montoProv - montoApli;
            }
            string[] tsolImp = new string[] { "NC", "NCA", "NCAS", "NCAM", "NCASM", "NCS", "NCI", "NCIA", "NCIAS", "NCIS" };
            if (tsolImp.Contains(D.TSOL_ID))
            {
                decimal KBETR = 0.0M;
                esNC = true;
                if (db.DOCUMENTOPs.Any(x => (x.MATKL == "605" || x.MATKL == "207") && x.NUM_DOC == D.NUM_DOC))
                {
                    KBETR = db.IIMPUESTOes.First(x => x.MWSKZ == "A0").KBETR.Value;
                }
                else
                {
                    decimal concecutivo = db.CONPOSAPHs.First(x => x.TIPO_SOL == "NC" && x.SOCIEDAD == D.SOCIEDAD_ID && (x.TIPO_DOC == "YG" || x.TIPO_DOC == "DG")).CONSECUTIVO;
                    string tax_code = db.CONPOSAPPs.First(x => x.CONSECUTIVO == concecutivo).TAX_CODE;
                    KBETR = db.IIMPUESTOes.First(x => x.MWSKZ == tax_code).KBETR.Value;
                }
                impuesto = (D.MONTO_DOC_MD.Value * KBETR);
            }
            ViewBag.montoSol = format.toShow(D.MONTO_DOC_MD.Value, ".");
            ViewBag.montoProv = (esProv ? format.toShow(montoProv, ".") : "-");
            ViewBag.montoApli = (esDocRef ? format.toShow(montoApli, ".") : "-");
            ViewBag.remanente = ((montoProv > 0 && montoApli > 0) ? format.toShow(remanente, ".") : "-");
            ViewBag.impuesto = (esNC ? format.toShow(impuesto, ".") : "-");
            ViewBag.montoTotal = format.toShow(D.MONTO_DOC_MD.Value, ".");
        }

        // GET: Correos/Details/5
        public ActionResult Details(decimal id, bool? mail, string spras)
        {
            if (spras == "" | spras == null)
            {
                spras = "ES";
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string pathReplace = "";
            string[] pathReplaceArr;
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            ViewBag.workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            ViewBag.acciones = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P") & a.USUARIOA_ID.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplace = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            pathReplaceArr = pathReplace.Split(new string[] { "?spras" }, StringSplitOptions.None);
            ViewBag.url = pathReplaceArr[0];
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            //ViewBag.miles = dOCUMENTO.PAI.MILES;//LEJGG 090718
            //ViewBag.dec = dOCUMENTO.PAI.DECIMAL;//LEJGG 090718
            FormatosC fc = new FormatosC();
            //ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL);
            ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL) + " " + dOCUMENTO.MONEDA_ID;
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;

            //ADD LGPP 13.12.2018
            ViewBag.nombreUsuario = db.USUARIOs.Where(x => x.ID == dOCUMENTO.USUARIOC_ID).FirstOrDefault().NOMBRE;
            ViewBag.idUsuario = dOCUMENTO.USUARIOC_ID;
            ViewBag.bandera = dOCUMENTO.PAIS_ID;
            //SECCION DE CABECERAS
            ViewBag.tituloCabecera = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cabeceraDet").FirstOrDefault().TEXTOS;
            ViewBag.tituloBajoCabecera = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_tituloDet").FirstOrDefault().TEXTOS;
            ViewBag.opcionDet = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_opcionesDet").FirstOrDefault().TEXTOS;
            ViewBag.ligaDet = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_ligaDet").FirstOrDefault().TEXTOS;
            //SECCION DE INFORMACION DEL USUARIO
            ViewBag.usuario = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_usuario").FirstOrDefault().TEXTOS;
            ViewBag.usuarioNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_nombreUsu").FirstOrDefault().TEXTOS;
            ViewBag.tipoSolicitud = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_tsol").FirstOrDefault().TEXTOS;
            ViewBag.clasificacion = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_clasificacion").FirstOrDefault().TEXTOS;
            ViewBag.cliente = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_cliente").FirstOrDefault().TEXTOS;
            ViewBag.clienteNombre = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_nombreCli").FirstOrDefault().TEXTOS;
            ViewBag.concepto = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_concepto").FirstOrDefault().TEXTOS;
            ViewBag.montoDoc = db.TEXTOes.Where(x => x.PAGINA_ID == 254 & x.SPRAS_ID == spras & x.CAMPO_ID == "lbl_montoDoc").FirstOrDefault().TEXTOS;
            //END LGPP 13.12.2018

            return View(dOCUMENTO);
        }

        // GET: Correos
        public ActionResult Recurrente(decimal id, bool? mail)
        {
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
            ViewBag.Pos = flujo[0];
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");

            DOCUMENTOL dl = dOCUMENTO.DOCUMENTOLs.OrderByDescending(x => x.POS).FirstOrDefault();
            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL);
            ViewBag.mes = dl.FECHAF.Value.Month;
            ViewBag.venta = fc.toShow((decimal)dl.MONTO_VENTA, dOCUMENTO.PAI.DECIMAL);
            DOCUMENTOREC dr = db.DOCUMENTORECs.Where(x => x.DOC_REF == dOCUMENTO.NUM_DOC).FirstOrDefault();
            ViewBag.objetivo = fc.toShow((decimal)dr.MONTO_BASE, dOCUMENTO.PAI.DECIMAL);
            ViewBag.porc = fc.toShowPorc((decimal)dr.PORC, dOCUMENTO.PAI.DECIMAL);
            if (dl.MONTO_VENTA < dr.MONTO_BASE)
            {
                ViewBag.tsol = dOCUMENTO.TSOL.TSOLR;
                ViewBag.nota = false;
            }
            else
            {
                ViewBag.tsol = "";
                ViewBag.nota = true;
            }
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............
            return View(dOCUMENTO);
        }

        // GET: Correos
        public ActionResult Backorder(decimal id, bool? mail)
        {
            var dOCUMENTO = db.DOCUMENTOes.Where(x => x.NUM_DOC == id).FirstOrDefault();
            var flujo = db.FLUJOes.Where(x => x.NUM_DOC == id).OrderByDescending(o => o.POS).Select(s => s.POS).ToList();
            ViewBag.Pos = flujo[0];
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");

            DOCUMENTOL dl = dOCUMENTO.DOCUMENTOLs.OrderByDescending(x => x.POS).FirstOrDefault();
            FormatosC fc = new FormatosC();
            ViewBag.monto = fc.toShow((decimal)dOCUMENTO.MONTO_DOC_MD, dOCUMENTO.PAI.DECIMAL);
            ViewBag.mes = dl.FECHAF.Value.Month;
            ViewBag.venta = fc.toShow((decimal)dl.MONTO_VENTA, dOCUMENTO.PAI.DECIMAL);
            DOCUMENTOREC dr = db.DOCUMENTORECs.Where(x => x.DOC_REF == dOCUMENTO.NUM_DOC).FirstOrDefault();
            ViewBag.objetivo = fc.toShow((decimal)dr.MONTO_BASE, dOCUMENTO.PAI.DECIMAL);
            ViewBag.porc = fc.toShowPorc((decimal)dr.PORC, dOCUMENTO.PAI.DECIMAL);
            if (dl.MONTO_VENTA < dr.MONTO_BASE)
            {
                ViewBag.tsol = dOCUMENTO.TSOL.TSOLR;
                ViewBag.nota = false;
            }
            else
            {
                ViewBag.tsol = "";
                ViewBag.nota = true;
            }
            if (mail == null)
                mail = true;
            //B20180803 MGC Correos............
            string mailv = "";
            if (mail != null)
            {
                if (mail == true)
                {
                    mailv = "X";
                }
            }

            ViewBag.mail = mailv;
            //B20180803 MGC Correos............
            return View(dOCUMENTO);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public CLIENTE_MOD SelectCliente(string kunnr)
        {

            TruequeEntities db = new TruequeEntities();

            CLIENTE_MOD id_cl = (from c in db.CLIENTEs
                                 join co in db.CONTACTOCs
                                 on new { c.VKORG, c.VTWEG, c.SPART, c.KUNNR } equals new { co.VKORG, co.VTWEG, co.SPART, co.KUNNR } into jjcont
                                 from co in jjcont.DefaultIfEmpty()
                                 where (c.KUNNR == kunnr & co.DEFECTO == true)
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
                //CANAL canal = db.CANALs.Where(ca => ca.BANNER == id_cl.BANNER && ca.KUNNR == kunnr).FirstOrDefault();
                CANAL canal = db.CANALs.Where(ca => ca.CANAL1 == id_cl.CANAL).FirstOrDefault();
                id_cl.VTWEG = "";
                //if (canal == null)
                //{
                //    string kunnrwz = kunnr.TrimStart('0');
                //    string bannerwz = id_cl.BANNER.TrimStart('0');
                //    canal = db.CANALs.Where(ca => ca.BANNER == bannerwz && ca.KUNNR == kunnrwz).FirstOrDefault();
                //}

                if (canal != null)
                {
                    //id_cl.VTWEG = canal.CANAL1 + " - " + canal.CDESCRIPCION;
                    id_cl.VTWEG = canal.CDESCRIPCION;
                }

                //Obtener el tipo de cliente
                var clientei = (from c in db.TCLIENTEs
                                join ct in db.TCLIENTETs
                                on c.ID equals ct.PARVW_ID
                                where c.ID == id_cl.PARVW && c.ACTIVO == true
                                select ct).FirstOrDefault();
                id_cl.PARVW = "";
                if (clientei != null)
                {
                    id_cl.PARVW = clientei.TXT50;
                }

            }

            return id_cl;
        }

        public PRESUPUESTO_MOD getPresupuesto(string kunnr, string periodo)
        {
            PRESUPUESTO_MOD pm = new PRESUPUESTO_MOD();
            Presupuesto pr = new Presupuesto();
            pm = pr.getPresupuesto(kunnr, periodo);

            return pm;
        }

        public SOLICITUD_MOD getSolicitud(string num, string num2, string d)//RSG 07.06.2018---------------------------------------------
        {
            TAT001.Models.SOLICITUD_MOD sm = new SOLICITUD_MOD();
            Services.FormatosC format = new FormatosC();

            //Obtener info solicitud
            if (num == null | num == "" | num == "0.00")
            {
                sm.S_NUM = num = "";
                sm.S_MONTOB = format.toShow(Convert.ToDecimal(num2), d);
                sm.S_MONTOP = sm.S_MONTOB;
                sm.S_MONTOA = "-";
                sm.S_REMA = "-";
                sm.S_IMPA = "-";
                sm.S_IMPB = "-";
                sm.S_IMPC = "-";
                sm.S_RET = "-";
                sm.S_TOTAL = format.toShow(Convert.ToDecimal(num2), d); ;
            }
            else
            {
                decimal hola = Convert.ToDecimal(num);
                var rev = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == hola).ToList();
                ;

                if (rev.Count() == 0)
                {
                    //CON UN RELACIONADO 
                    var rev2 = db.DOCUMENTOes.Where(x => x.NUM_DOC == hola).FirstOrDefault();
                    decimal? rem2 = (rev2.MONTO_DOC_MD - Convert.ToDecimal(num2));

                    sm.S_MONTOB = format.toShow(Convert.ToDecimal(num2), d);
                    sm.S_MONTOP = format.toShow(0, d);
                    sm.S_MONTOA = "-";
                    sm.S_REMA = format.toShow((decimal)rem2, d);
                    sm.S_IMPA = "-";
                    sm.S_IMPB = "-";
                    sm.S_IMPC = "-";
                    sm.S_RET = "-";
                    sm.S_TOTAL = format.toShow(Convert.ToDecimal(num2), d); ;
                }
                else if (rev.Count() == 1)
                {
                    //CON DOS RELACIONADOS
                    var rev3 = db.DOCUMENTOes.Where(x => x.NUM_DOC == hola).FirstOrDefault();
                    var rev33 = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == hola).FirstOrDefault();
                    decimal? rem3 = ((rev3.MONTO_DOC_MD - rev33.MONTO_DOC_MD) - (Convert.ToDecimal(num2)));

                    sm.S_MONTOB = format.toShow(Convert.ToDecimal(num2), d);
                    sm.S_MONTOP = format.toShow((decimal)rev3.MONTO_DOC_MD, d);
                    sm.S_MONTOA = format.toShow((decimal)rev33.MONTO_DOC_MD - Convert.ToDecimal(num2), d);
                    sm.S_REMA = format.toShow((decimal)rem3 + Convert.ToDecimal(num2), d);
                    sm.S_IMPA = "-";
                    sm.S_IMPB = "-";
                    sm.S_IMPC = "-";
                    sm.S_RET = "-";
                    sm.S_TOTAL = format.toShow(Convert.ToDecimal(num2), d); ;
                }
                else if (rev.Count() > 1)
                {
                    var rev4 = db.DOCUMENTOes.Where(x => x.NUM_DOC == hola).FirstOrDefault();
                    var rev44 = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == hola).Select(x => x.MONTO_DOC_MD);
                    decimal sum = 0;

                    foreach (var k in rev44)
                    {
                        sum = sum + k.Value;
                    }
                    decimal? rem4 = ((rev4.MONTO_DOC_MD - sum) - (Convert.ToDecimal(num2)));

                    sm.S_MONTOB = format.toShow(Convert.ToDecimal(num2), d);
                    sm.S_MONTOP = format.toShow((decimal)rev4.MONTO_DOC_MD, d);
                    sm.S_MONTOA = format.toShow(sum - Convert.ToDecimal(num2), d);
                    sm.S_REMA = format.toShow((-sum + (decimal)rev4.MONTO_DOC_MD + Convert.ToDecimal(num2)), d);
                    sm.S_IMPA = "-";
                    sm.S_IMPB = "-";
                    sm.S_IMPC = "-";
                    sm.S_RET = "-";
                    sm.S_TOTAL = format.toShow(Convert.ToDecimal(num2), d);
                }
            }

            //JsonResult cc = Json(sm, JsonRequestBehavior.AllowGet);
            return sm;
        }
    }
}
