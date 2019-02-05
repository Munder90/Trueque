using TAT001.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;

namespace TAT001.Models
{
    public class ArchivoContable
    {
        bool unico = false;
        decimal padre;
        int contdoc = 0;
        TAT001Entities db;
        public string generarArchivo(decimal docum, decimal relacion, int pos)
        {
            db = new TAT001Entities();
            try
            {
                contdoc++;
                string dirFile = "";
                DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == docum).Single();
                CONPOSAPH tab;
                CLIENTE clien = new CLIENTE();
                List<DOCUMENTOF> docf = new List<DOCUMENTOF>();
                bool hijo = false;
                try
                {
                    if (relacion == 0)
                    {
                        tab = db.CONPOSAPHs.Where(x => x.TIPO_SOL == doc.TSOL.TSOLC
                        && x.SOCIEDAD == doc.SOCIEDAD_ID
                        && x.FECHA_FINVIG >= doc.FECHAF_VIG
                        && x.FECHA_INIVIG <= DateTime.Today
                        && x.TIPO_DOC != "KG").FirstOrDefault();
                        if (tab == null)
                        {
                            tab = db.CONPOSAPHs.First(x => x.TIPO_SOL == doc.TSOL_ID
                            && x.SOCIEDAD == doc.SOCIEDAD_ID
                            && x.FECHA_FINVIG >= doc.FECHAF_VIG
                            && x.FECHA_INIVIG <= DateTime.Today
                            && x.TIPO_DOC != "KG"
                            );
                        }
                    }
                    else
                    {
                        tab = db.CONPOSAPHs.Where(x => x.SOCIEDAD == doc.SOCIEDAD_ID
                        && x.FECHA_FINVIG >= doc.FECHAF_VIG
                        && x.CONSECUTIVO == relacion
                        ).Single();
                        hijo = true;
                    }
                }
                catch (Exception)
                {
                    return "No se encontro configuracion para generar documento para este tipo de solicitud";
                }

                string txt = "";
                string msj = "";
                string[] cc;
                string cta = "";
                try
                {
                    clien = db.CLIENTEs.Where(x => x.KUNNR == doc.PAYER_ID).Single();
                }
                catch (Exception) { }
                try
                {
                    docf = db.DOCUMENTOFs.Where(x => x.NUM_DOC == docum).ToList();
                }
                catch (Exception) { }

                if (tab.TIPO_DOC == "RN" || tab.TIPO_DOC == "KR")
                {
                    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-2-" + contdoc + ".txt";
                }
                else if (tab.TIPO_DOC == "KG")
                {
                    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-3-" + contdoc + ".txt";
                }
                else if (tab.TIPO_SOL == "NCM")
                {
                    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1-fact" + pos + ".txt";
                }
                else
                {
                    dirFile = ConfigurationManager.AppSettings["URL_SAVE"] + @"POSTING\INBOUND_" + tab.TIPO_SOL.Substring(0, 2) + docum.ToString().PadLeft(10, '0') + "-1-" + contdoc + ".txt";
                }
                cta = doc.GALL_ID;
                doc.GALL_ID = db.GALLs.Where(x => x.ID == doc.GALL_ID).Select(x => x.GRUPO_ALL).Single();
                var ppd = doc.GetType().GetProperties();
                tab.HEADER_TEXT = tab.HEADER_TEXT.Trim();
                if (String.IsNullOrEmpty(tab.HEADER_TEXT) == false)
                {
                    tab.HEADER_TEXT = Referencia(tab.HEADER_TEXT, doc, docf, clien, pos);
                }
                else
                {
                    return "Agrege comando para generar texto de encabezado";
                }

                txt = "";
                tab.REFERENCIA = tab.REFERENCIA.Trim();
                if (String.IsNullOrEmpty(tab.REFERENCIA) == false)
                {
                    tab.REFERENCIA = Referencia(tab.REFERENCIA, doc, docf, clien, pos);
                }
                else
                {
                    return "Agrege comando para generar referencia";
                }
                //tab.REFERENCIA = txt;
                tab.NOTA = tab.NOTA.Trim();
                if (String.IsNullOrEmpty(tab.NOTA) == false)
                {
                    tab.NOTA = Referencia(tab.NOTA, doc, docf, clien, pos);
                }
                tab.CORRESPONDENCIA = tab.CORRESPONDENCIA.Trim();
                if (String.IsNullOrEmpty(tab.CORRESPONDENCIA) == false)
                {
                    tab.CORRESPONDENCIA = Referencia(tab.CORRESPONDENCIA, doc, docf, clien, pos);
                }
                doc.GALL_ID = cta;
                if (String.IsNullOrEmpty(tab.MONEDA))
                {
                    doc.MONEDA_ID = "";
                }

                doc.FECHAC = Fecha(tab.FECHA_CONTAB, Convert.ToDateTime(doc.FECHAC));

                List<DetalleContab> det = new List<DetalleContab>();
                msj = Detalle(doc, ref det, ref tab, docf, hijo, pos);

                if (msj != "")
                {
                    return msj;
                }
                tab.FECHA_DOCU = Periodo(doc);
                if (tab.FECHA_DOCU == "")
                {
                    return "Configure rango para fecha contable";
                }
                if (String.IsNullOrEmpty(clien.EXPORTACION) == false)
                {
                    doc.MONEDA_ID = "USD";
                }
                using (StreamWriter sw = new StreamWriter(dirFile))
                {
                    CONPOSAPH dir = tab;
                    sw.WriteLine(
                        tab.TIPO_DOC + "|" +
                        dir.SOCIEDAD.Trim() + "|"
                        + String.Format("{0:MM.dd.yyyy}", doc.FECHAC).Replace(".", "") + "|"
                        + dir.FECHA_DOCU.Trim() + "|"
                        + doc.MONEDA_ID.Trim() + "|"
                        + dir.HEADER_TEXT.Trim() + "|"
                        + dir.REFERENCIA.Trim() + "|"
                        + dir.CALC_TAXT.ToString().Replace("True", "X").Replace("False", "") + "|"
                        + dir.NOTA.Trim() + "|"
                        + dir.CORRESPONDENCIA.Trim()
                        );
                    sw.WriteLine("");
                    for (int i = 0; i < det.Count; i++)
                    {
                        sw.WriteLine(
                            det[i].POS_TYPE + "|" +
                            det[i].COMP_CODE + "|" +
                            det[i].BUS_AREA + "|" +
                            det[i].POST_KEY + "|" +
                            det[i].ACCOUNT + "|" +
                            det[i].COST_CENTER + "|" +
                            det[i].BALANCE + "|" +
                            det[i].TEXT + "|" +
                            det[i].SALES_ORG + "|" +
                            det[i].DIST_CHANEL + "|" +
                            det[i].DIVISION + "|" +
                            //"|" +
                            //"|" +
                            //"|" +
                            //"|" +
                            //"|" +
                            det[i].INV_REF + "|" +
                            det[i].PAY_TERM + "|" +
                            det[i].JURIS_CODE + "|" +
                            //"|" +
                            det[i].CUSTOMER + "|" +
                            det[i].PRODUCT + "|" +
                            det[i].TAX_CODE + "|" +
                            det[i].PLANT + "|" +
                            det[i].REF_KEY1 + "|" +
                            //det[i].REF_KEY2 + "|" +
                            det[i].REF_KEY3 + "|" +
                            det[i].ASSIGNMENT + "|" +
                            det[i].QTY + "|" +
                            det[i].BASE_UNIT + "|" +
                            det[i].AMOUNT_LC + "|" +
                            det[i].RETENCION_ID + "|"
                            );
                    }
                    sw.Close();
                }
                if (tab.TIPO_SOL == "NIM")
                {
                    pos = 0;
                    padre = Convert.ToInt32(tab.RELACION);
                    unico = true;
                    for (int i = 0; i < docf.Count; i++)
                    {
                        msj = generarArchivo(docum, Convert.ToInt32(tab.RELACION), i);
                    }
                    return msj;
                }
                //if (padre == tab.RELACION && relacion != 0)
                //{
                //    return "";
                //}
                //else if(padre != 0 && tab.TIPO_SOL != "NCIM")
                //{
                //    pos--;
                //}
                if (tab.TIPO_SOL == "NCM")
                {
                    unico = true;
                    for (int i = 0; i < docf.Count; i++)
                    {
                        msj = generarArchivo(docum, Convert.ToInt32(tab.RELACION), i);
                    }
                    return msj;
                }
                else if (tab.RELACION != 0 && tab.RELACION != null)
                {
                    return generarArchivo(docum, Convert.ToInt32(tab.RELACION), pos);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return "Error al generar el documento contable " + e.Message;
            }
        }
        private string Referencia(string campo, DOCUMENTO doc, List<DOCUMENTOF> docf, CLIENTE clien, int pos)
        {
            string[] cc = campo.Trim().Split('+');
            string[] indes = new string[cc.Length];
            string txt = "";
            int index = 0;
            PropertyInfo[] ppdf = new PropertyInfo[1];
            PropertyInfo[] ppd = doc.GetType().GetProperties();
            if (docf.Count > 0)
            {
                ppdf = docf[0].GetType().GetProperties();
            }
            PropertyInfo[] ppdc = clien.GetType().GetProperties();
            try
            {
                foreach (string c in cc)
                {
                    try
                    {
                        txt += ppd.Where(x => x.Name == c).Single().GetValue(doc);
                        //txt += ppd[1].GetValue(doc);
                        indes[index] = "X";
                    }
                    catch (Exception) { }
                    try
                    {
                        if (docf.Count > 0)
                        {
                            if (indes[index] != "X")
                            {
                                if (unico)
                                {
                                    txt += ppdf.Where(x => x.Name == c).Single().GetValue(docf[pos]);
                                    indes[index] = "X";
                                }
                                else
                                {
                                    for (int i = 0; i < docf.Count; i++)
                                    {
                                        txt += ppdf.Where(x => x.Name == c).Single().GetValue(docf[i]) + ",";
                                        indes[index] = "X";
                                    }
                                    txt = txt.Substring(0, txt.Length - 1);
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                    try
                    {
                        if (indes[index] != "X")
                        {
                            txt += ppdc.Where(x => x.Name == c).Single().GetValue(clien);
                            indes[index] = "X";
                        }
                    }
                    catch (Exception) { }
                    index++;
                }
            }
            catch (Exception)
            { }
            if (String.IsNullOrEmpty(txt))
            {
                return "";
            }
            else
            {
                return txt;
            }

        }
        private string Periodo(DOCUMENTO doc)
        {
            DateTime hoy = DateTime.Today;
            hoy = hoy.AddMonths(-1);
            int ant = hoy.Month;
            hoy = hoy.AddMonths(2);
            int sig = hoy.Month;
            hoy = DateTime.Today;
            List<CALENDARIO_AC> calend = db.CALENDARIO_AC.Where(x => x.PERIODO >= ant && x.PERIODO <= sig && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.TSOL_ID == doc.TSOL_ID).ToList();
            List<PERIODO445> peri = db.PERIODO445.Where(x => x.PERIODO >= ant && x.PERIODO <= sig && x.EJERCICIO == hoy.Year).ToList();
            if (calend.Count == 3 && peri.Count == 3)
            {
                DateTime periodo = new DateTime(peri[1].EJERCICIO, peri[1].PERIODO, peri[1].DIA_NATURAL);
                periodo = periodo.Add(calend[1].CIE_TOH);
                DateTime calendario = calend[1].PRE_FROMF;
                if (hoy>= calendario && hoy<= periodo)
                {
                    return hoy.ToString("MM-dd-yyyy").Replace("-","");
                }
                else if (hoy> periodo)
                {
                    return periodo.ToString("MM-dd-yyyy").Replace("-", "");
                }
                else
                {
                    hoy = new DateTime(peri[0].EJERCICIO, peri[0].PERIODO, peri[0].DIA_NATURAL);
                    return hoy.ToString("MM-dd-yyyy").Replace("-", "");
                }
            }
            else
            {
                return "";
            }
        }
        private DateTime Fecha(string id_fecha, DateTime fech)
        {
            DateTime fecha = DateTime.Today;
            switch (id_fecha)
            {
                case "U":
                    fecha = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    fecha = fecha.AddMonths(1).AddDays(-1);
                    break;
                case "P":
                    fecha = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "H":
                    fecha = DateTime.Today;
                    break;
                default:
                    fecha = fech;
                    break;
            }
            return fecha;
        }
        private string Detalle(DOCUMENTO doc, ref List<DetalleContab> contas, ref CONPOSAPH enca, List<DOCUMENTOF> docf, bool hijo, int pos)
        {
            contas = new List<DetalleContab>();
            db = new TAT001Entities();
            List<CONPOSAPP> conp = new List<CONPOSAPP>();
            CLIENTE clien;
            CUENTA cuent;
            TCAMBIO cambio = new TCAMBIO();
            TAXEOH taxh = new TAXEOH();
            List<TAXEOP> taxp = new List<TAXEOP>();
            MATERIAL material;
            string[] grupos;
            string grupo = "";
            string materi = "";
            string factura = "";
            //var iva = (from c in db.CUENTAs
            //           join i in db.IIMPUESTOes on c.IMPUESTO equals i.MWSKZ
            //           where i.LAND == doc.PAIS_ID && c.TALL_ID == doc.TALL_ID && i.ACTIVO == true
            //           select new { i.MWSKZ, i.KBETR }).Single();
            
            try
            {
                try
                {
                    decimal conse = enca.CONSECUTIVO;
                    conp = db.CONPOSAPPs.Where(x => x.CONSECUTIVO == conse).ToList();
                }
                catch (Exception f)
                {
                    return "No se encontro datos de configuracion para detalle contable";
                }
                try
                {
                    clien = db.CLIENTEs.Where(x => x.KUNNR == doc.PAYER_ID).Single();
                }
                catch (Exception g)
                {
                    return "No se encontro datos de cliente para detalle contable";
                }
                try
                {
                    cuent = db.CUENTAs.Where(x => x.PAIS_ID == doc.PAIS_ID && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.TALL_ID == doc.TALL_ID).Single();
                }
                catch (Exception h)
                {
                    return "No se encontro datos de cuenta para detalle contable";
                }
                try
                {
                    string pais = db.TAX_LAND.Where(x => x.ACTIVO == true && x.PAIS_ID == doc.PAIS_ID).Select(x => x.PAIS_ID).Single();
                    if (String.IsNullOrEmpty(pais) == false)
                    {
                        if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "KG")
                        {
                            try
                            {
                                taxh = db.TAXEOHs.Where(x => x.PAIS_ID == doc.PAIS_ID && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.KUNNR == clien.KUNNR && x.CONCEPTO_ID == doc.CONCEPTO_ID).Single();
                            }
                            catch (Exception)
                            {
                                return "No se encontro datos de configuracion de taxeo";
                            }
                        }

                    }
                }
                catch (Exception z)
                {
                }
                try
                {
                    if (doc.PAIS_ID == "CO")
                    {
                        if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB")
                        {
                            taxp = db.TAXEOPs.Where(x => x.PAIS_ID == doc.PAIS_ID && x.SOCIEDAD_ID == doc.SOCIEDAD_ID && x.KUNNR == clien.KUNNR && x.CONCEPTO_ID == doc.CONCEPTO_ID).ToList();
                        }
                    }
                }
                catch (Exception)
                {
                    return "No se encontro configuracion para extraccion de retencion";
                }
                if (docf.Count > 0)
                {
                    factura = docf[0].FACTURA;
                }
                if (String.IsNullOrEmpty(clien.EXPORTACION) == false)
                {
                    try
                    {
                        cambio = db.TCAMBIOs.Where(x => x.FCURR == doc.MONEDA_ID && x.TCURR == "USD" && x.GDATU == DateTime.Today).Single();
                    }
                    catch (Exception)
                    {
                        return "No se encontro el cambio de moneda de la fecha actual.";
                    }
                }

                for (int i = 0; i < conp.Count; i++)
                {
                    if (conp[i].POSICION == 1)
                    {
                        DetalleContab conta = new DetalleContab();
                        conta.POS_TYPE = conp[i].KEY;
                        conta.ACCOUNT = cuent.ABONO.ToString();
                        conta.COMP_CODE = doc.SOCIEDAD_ID;
                        conta.BUS_AREA = conp[i].BUS_AREA;
                        conta.POST_KEY = conp[i].POSTING_KEY;
                        conta.TEXT = doc.CONCEPTO;
                        conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                        if (conp[i].POSTING_KEY == "11")
                        {
                            if (enca.TIPO_DOC == "BB")
                            {
                                if (doc.PAIS_ID == "CO")
                                {
                                    conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                }
                                else
                                {
                                    if (unico)
                                    {
                                        conta.BALANCE = Conversion(Convert.ToDecimal(docf[pos].IMPORTE_FAC + (docf[pos].IMPORTE_FAC * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    }
                                    else
                                    {
                                        conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    }
                                    enca.CALC_TAXT = false;
                                }
                                conta.REF_KEY1 = clien.STCD1;
                                conta.REF_KEY3 = clien.NAME1;
                                if (enca.CALC_TAXT == false)
                                {
                                    conta.TAX_CODE = conp[i].TAX_CODE;
                                }
                            }
                            else
                            {
                                conta.REF_KEY1 = conp[i].REF_KEY1;
                                conta.REF_KEY3 = conp[i].REF_KEY3;
                                if (unico)
                                {
                                    conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                }
                            }                            
                            conta.ACCOUNT = clien.PAYER;
                        }
                        if (conp[i].POSTING_KEY == "31")
                        {
                            conta.ACCOUNT = clien.PROVEEDOR_ID;
                            if (enca.TIPO_DOC == "KR" && doc.PAIS_ID == "CO")
                            {
                                conta.ASSIGNMENT = clien.PAYER;
                                if (enca.CALC_TAXT == false)
                                {
                                    conta.TAX_CODE = taxh.IMPUESTO_ID;
                                }
                            }
                            if (unico && enca.TIPO_DOC == "KR")
                            {
                                conta.ASSIGNMENT = clien.PAYER;
                                if (enca.CALC_TAXT == false)
                                {
                                    conta.TAX_CODE = taxh.IMPUESTO_ID;
                                }
                                conta.BALANCE = Conversion(Convert.ToDecimal(docf[pos].IMPORTE_FAC + (docf[pos].IMPORTE_FAC * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();                                
                            }
                            if (enca.TIPO_DOC == "KG" )
                            {
                                if (doc.PAIS_ID == "CO")
                                {
                                    conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                }
                                else if(unico)
                                {
                                    conta.BALANCE = Conversion(Convert.ToDecimal(docf[pos].IMPORTE_FAC + (docf[pos].IMPORTE_FAC * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                }
                            }
                        }
                        if (conp[i].POSTING_KEY == "21")
                        {
                            conta.ACCOUNT = clien.PROVEEDOR_ID;
                            if (enca.TIPO_DOC == "KG")
                            {
                                conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                            }
                        }
                        if (conp[i].POSTING_KEY == "50" && enca.TIPO_DOC == "RN")
                        {
                            conta.ACCOUNT = cuent.CLEARING.ToString();
                        }
                        if (conp[i].POSTING_KEY == "50" && enca.TIPO_DOC == "SA" && hijo)
                        {
                            conta.ACCOUNT = cuent.CLEARING.ToString();
                        }
                        if (doc.PAIS_ID == "CO" && enca.TIPO_DOC != "SA")
                        {
                            if (taxp.Count > 0)
                            {
                                for (int k = 0; k < taxp.Count; k++)
                                {
                                    conta.RETENCION_ID += taxp[k].RETENCION_ID + ",";
                                }
                                conta.RETENCION_ID = conta.RETENCION_ID.Substring(0, conta.RETENCION_ID.Length - 1);
                            }
                            if (enca.TIPO_SOL == "KG")
                            {
                                conta.ACCOUNT = clien.PROVEEDOR_ID;
                            }
                        }
                        contas.Add(conta);
                    }
                    else
                    {
                        List<DOCUMENTOM> docm = db.DOCUMENTOMs.Where(x => x.NUM_DOC == doc.NUM_DOC).ToList();
                        for (int j = 0; j < docm.Count; j++)
                        {
                            DetalleContab conta = new DetalleContab();
                            conta.POS_TYPE = conp[i].KEY;
                            conta.COMP_CODE = doc.SOCIEDAD_ID;
                            conta.BUS_AREA = conp[i].BUS_AREA;
                            conta.POST_KEY = conp[i].POSTING_KEY;
                            conta.TEXT = doc.CONCEPTO;
                            conta.REF_KEY1 = clien.STCD1;
                            conta.REF_KEY3 = clien.NAME1;
                            conta.SALES_ORG = clien.VKORG;
                            conta.DIST_CHANEL = clien.VTWEG;
                            conta.DIVISION = clien.SPART;
                            if (enca.TIPO_DOC != "RN" && doc.PAIS_ID != "CO")
                            {
                                conta.CUSTOMER = doc.PAYER_ID;
                                conta.PRODUCT = docm[j].MATNR;
                                if (conp[i].QUANTITY != null && conp[i].QUANTITY != 0)
                                {
                                    conta.QTY = conp[i].QUANTITY.ToString();
                                }
                                conta.AMOUNT_LC = conp[i].BASE_UNIT;
                                conta.ACCOUNT = cuent.CARGO.ToString();
                                //conta.BALANCE = (docp[j].MONTO_APOYO * docp[j].VOLUMEN_EST).ToString();

                                //conta.BALANCE = docp[j].APOYO_REAL.ToString();
                                if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "DG")
                                {
                                    //conta.BALANCE = docm[j].APOYO_REAL.ToString(); //KCMX notacredito
                                    if (unico)
                                    {
                                        conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                        if (enca.TIPO_DOC == "DG")
                                        {
                                            conta.REF_KEY2 = docf[pos].BELNR;
                                        }
                                    }
                                    else
                                    {
                                        conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    }
                                }
                                else if (enca.TIPO_DOC == "SA" && hijo)
                                {
                                    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                }
                                if (enca.TIPO_DOC == "KG")
                                {
                                    conta.CUSTOMER =
                                    conta.PRODUCT = "";
                                    if (unico)
                                    {
                                        conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                    }
                                    else
                                    {
                                        conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    }
                                    conta.ACCOUNT = cuent.CLEARING.ToString();
                                    conta.PRODUCT = docm[j].MATNR;
                                    conta.CUSTOMER = doc.PAYER_ID;
                                }
                                //else
                                //{
                                //    //conta.BALANCE = docm[j].APOYO_EST.ToString(); //KCMX solic
                                //    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                //}
                            }
                            else
                            {
                                //conta.SALES_ORG =
                                //conta.DIST_CHANEL =
                                //conta.DIVISION =
                                conta.CUSTOMER =
                                conta.PRODUCT = "";
                                conta.ACCOUNT = cuent.ABONO.ToString();
                                conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "SA")
                                {
                                    if (doc.PAIS_ID == "CO")
                                    {
                                        //conta.SALES_ORG = clien.VKORG;
                                        //conta.DIST_CHANEL = clien.VTWEG;
                                        //conta.DIVISION = clien.SPART;
                                        conta.SALES_DIST = clien.BZIRK;
                                        conta.CUSTOMER = doc.PAYER_ID;
                                        conta.PRODUCT = docm[j].MATNR;
                                        //conta.REF_KEY1 = clien.STCD1;
                                        //conta.REF_KEY3 = clien.NAME1;
                                        conta.ACCOUNT = cuent.CARGO.ToString();
                                        if (enca.TIPO_DOC == "SA")
                                        {
                                            conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                            conta.ASSIGNMENT = doc.PAYER_ID;
                                        }
                                        if (enca.TIPO_DOC == "BB")
                                        {
                                            conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                        }
                                    }

                                }
                                //if (enca.TIPO_DOC != "KG" && doc.PAIS_ID == "CO")
                                //{
                                //    //conta.BALANCE = docm[j].APOYO_EST.ToString();
                                //    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                //}
                                if (enca.TIPO_DOC == "KG" && doc.PAIS_ID == "CO")
                                {
                                    //conta.BALANCE = (doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * tax.PORC / 100)).ToString();
                                    conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    conta.ACCOUNT = cuent.CLEARING.ToString();
                                    conta.PRODUCT = docm[j].MATNR;
                                    conta.CUSTOMER = doc.PAYER_ID;
                                }
                                if (enca.TIPO_DOC == "DG" && doc.PAIS_ID == "CO")
                                {
                                    conta.ACCOUNT = cuent.CARGO.ToString();
                                    conta.BALANCE = Conversion(Convert.ToDecimal(docm[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    conta.PRODUCT = docm[j].MATNR;
                                    conta.CUSTOMER = doc.PAYER_ID;
                                    conta.REF_KEY1 = factura;
                                    conta.REF_KEY3 = clien.NAME1;
                                }
                                if (enca.TIPO_DOC == "RN")
                                {
                                    conta.ACCOUNT = cuent.CLEARING.ToString();
                                }
                            }
                            if (enca.TIPO_DOC == "KR")
                            {
                                if (enca.CALC_TAXT == false)
                                {
                                    conta.TAX_CODE = taxh.IMPUESTO_ID;
                                }
                                if (unico)
                                {
                                    conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                }
                                conta.ASSIGNMENT = clien.PAYER;
                                conta.PRODUCT = docm[j].MATNR;
                                conta.CUSTOMER = doc.PAYER_ID;
                                conta.ACCOUNT = cuent.CLEARING.ToString();
                            }
                            else
                            {
                                if (enca.CALC_TAXT == false)
                                {
                                    materi = docm[j].MATNR;
                                    material = db.MATERIALs.Where(x => x.ID == materi).First();
                                    grupos = conp[i].MATERIALGP.Split('+');
                                    grupo = grupos.Where(x => x == material.MATERIALGP_ID).FirstOrDefault();
                                    if (String.IsNullOrEmpty(grupo) == false)
                                    {
                                        conta.TAX_CODE = conp[i].TAXCODEGP;
                                    }
                                    else
                                    {
                                        conta.TAX_CODE = conp[i].TAX_CODE;
                                    }
                                }
                            }
                            if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB")
                            {
                                if (doc.PAIS_ID != "CO")
                                {
                                    conta.REF_KEY1 = conp[i].REF_KEY1;
                                    conta.REF_KEY3 = conp[i].REF_KEY3;
                                }
                                else if (unico)
                                {
                                    conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                }
                            }
                            contas.Add(conta);
                            if (enca.TIPO_DOC == "RN" || enca.TIPO_DOC == "KR" || enca.TIPO_DOC == "KG")
                            {
                                break;
                            }
                        }
                        List<DOCUMENTOP> docp = db.DOCUMENTOPs.Where(x => x.NUM_DOC == doc.NUM_DOC).ToList();
                        for (int j = 0; j < docp.Count; j++)
                        {
                            DetalleContab conta = new DetalleContab();
                            conta.POS_TYPE = conp[i].KEY;
                            conta.COMP_CODE = doc.SOCIEDAD_ID;
                            conta.BUS_AREA = conp[i].BUS_AREA;
                            conta.POST_KEY = conp[i].POSTING_KEY;
                            conta.TEXT = doc.CONCEPTO;
                            conta.REF_KEY1 = clien.STCD1;
                            conta.REF_KEY3 = clien.NAME1;
                            conta.SALES_ORG = clien.VKORG;
                            conta.DIST_CHANEL = clien.VTWEG;
                            conta.DIVISION = clien.SPART;
                            if (enca.TIPO_DOC != "RN" && doc.PAIS_ID != "CO")
                            {
                                conta.CUSTOMER = doc.PAYER_ID;
                                conta.PRODUCT = docp[j].MATNR;
                                if (conp[i].QUANTITY != null && conp[i].QUANTITY != 0)
                                {
                                    conta.QTY = conp[i].QUANTITY.ToString();
                                }
                                conta.AMOUNT_LC = conp[i].BASE_UNIT;
                                conta.ACCOUNT = cuent.CARGO.ToString();
                                //conta.BALANCE = (docp[j].MONTO_APOYO * docp[j].VOLUMEN_EST).ToString();
                                conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                //conta.BALANCE = docp[j].APOYO_REAL.ToString();
                                if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "DG")
                                {
                                    //conta.BALANCE = docp[j].APOYO_REAL.ToString(); //KCMX notacredito                                    
                                    if (unico)
                                    {
                                        conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                        if (enca.TIPO_DOC == "DG")
                                        {
                                            conta.REF_KEY2 = docf[pos].BELNR;
                                        }                                        
                                    }
                                    else
                                    {
                                        conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    }
                                }
                                else if (enca.TIPO_DOC == "SA" && hijo)
                                {
                                    //conta.BALANCE = docp[j].APOYO_EST.ToString(); //KCMX solic
                                    conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                }
                                if (enca.TIPO_DOC == "KG")
                                {
                                    conta.CUSTOMER =
                                    conta.PRODUCT = "";
                                    conta.ACCOUNT = cuent.ABONO.ToString();
                                    if (unico)
                                    {
                                        conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                    }
                                    else
                                    {
                                        conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    }
                                    conta.ACCOUNT = cuent.CLEARING.ToString();
                                    conta.PRODUCT = docp[j].MATNR;
                                    conta.CUSTOMER = doc.PAYER_ID;
                                }

                            }
                            else
                            {
                                //conta.SALES_ORG =
                                //conta.DIST_CHANEL =
                                //conta.DIVISION =
                                conta.CUSTOMER =
                                conta.PRODUCT = "";
                                conta.ACCOUNT = cuent.ABONO.ToString();
                                conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                if (enca.TIPO_DOC == "BB" || enca.TIPO_DOC == "SA")
                                {
                                    //if (doc.PAIS_ID == "CO")
                                    //{
                                        //conta.SALES_ORG = clien.VKORG;
                                        //conta.DIST_CHANEL = clien.VTWEG;
                                        //conta.DIVISION = clien.SPART;
                                        conta.SALES_DIST = clien.BZIRK;
                                        conta.CUSTOMER = doc.PAYER_ID;
                                        conta.PRODUCT = docp[j].MATNR;
                                        //conta.REF_KEY1 = clien.STCD1;
                                        //conta.REF_KEY3 = clien.NAME1;
                                        conta.ACCOUNT = cuent.CARGO.ToString();
                                        if (enca.TIPO_DOC == "SA")
                                        {
                                            conta.ASSIGNMENT = doc.PAYER_ID;
                                            conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                        }
                                        if (enca.TIPO_DOC == "BB")
                                        {
                                            conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                        }
                                    //}

                                }
                                //if (enca.TIPO_DOC != "KG" && doc.PAIS_ID == "CO")
                                //{
                                //    //conta.BALANCE = docp[j].APOYO_EST.ToString();
                                //    conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_EST), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                //}
                                if (enca.TIPO_DOC == "KG" )
                                {
                                    //conta.BALANCE = (doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * tax.PORC / 100)).ToString();
                                    conta.BALANCE = Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    conta.ACCOUNT = cuent.CLEARING.ToString();
                                    conta.PRODUCT = docp[j].MATNR;
                                    conta.CUSTOMER = doc.PAYER_ID;
                                }
                                if (enca.TIPO_DOC == "DG")
                                {
                                    conta.ACCOUNT = cuent.CARGO.ToString();
                                    conta.BALANCE = Conversion(Convert.ToDecimal(docp[j].APOYO_REAL), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC).ToString();
                                    conta.PRODUCT = docp[j].MATNR;
                                    conta.CUSTOMER = doc.PAYER_ID;
                                    conta.REF_KEY1 = factura;
                                    conta.REF_KEY3 = clien.NAME1;
                                    //conta.SALES_ORG = clien.VKORG;
                                    //conta.DIST_CHANEL = clien.VTWEG;
                                    //conta.DIVISION = clien.SPART;
                                }
                                if (enca.TIPO_DOC == "RN")
                                {
                                    conta.ACCOUNT = cuent.CLEARING.ToString();
                                }
                            }
                            if (enca.TIPO_DOC == "KR")
                            {
                                if (enca.CALC_TAXT == false)
                                {
                                    conta.TAX_CODE = taxh.IMPUESTO_ID;
                                }
                                if (unico)
                                {
                                    conta.BALANCE = docf[pos].IMPORTE_FAC.ToString();
                                }
                                conta.ASSIGNMENT = clien.PAYER;
                                conta.PRODUCT = docp[j].MATNR;
                                conta.CUSTOMER = doc.PAYER_ID;
                                conta.ACCOUNT = cuent.CLEARING.ToString();
                            }
                            else
                            {
                                if (enca.CALC_TAXT == true)
                                {
                                    materi = docp[j].MATNR;
                                    material = db.MATERIALs.Where(y => y.ID == materi).Single();
                                    grupos = conp[i].MATERIALGP.Split('+');
                                    grupo = grupos.Where(x => x == material.MATERIALGP_ID).FirstOrDefault();
                                    if (String.IsNullOrEmpty(grupo) == false)
                                    {
                                        conta.TAX_CODE = conp[i].TAXCODEGP;
                                    }
                                    else
                                    {
                                        //conta.TAX_CODE = conp[i].TAX_CODE;
                                        //if (enca.TIPO_DOC == "DG")
                                        //{
                                        conta.TAX_CODE = conp[i].TAX_CODE;
                                        //}
                                    }
                                }
                            }
                            if (enca.TIPO_DOC == "DG" || enca.TIPO_DOC == "BB")
                            {
                                if (doc.PAIS_ID != "CO")
                                {
                                    conta.REF_KEY1 = conp[i].REF_KEY1;
                                    conta.REF_KEY3 = conp[i].REF_KEY3;
                                }
                            }
                            contas.Add(conta);
                            if (enca.TIPO_DOC == "RN" || enca.TIPO_DOC == "KR" || enca.TIPO_DOC == "KG")
                            {
                                break;
                            }
                        }


                        if (enca.TIPO_DOC == "BB")
                        {
                            DetalleContab conta = new DetalleContab();
                            conta.POS_TYPE = conp[i].KEY;
                            conta.ACCOUNT = cuent.CLEARING.ToString();
                            if (unico)
                            {
                                conta.BALANCE = (Conversion( Convert.ToDecimal(docf[pos].IMPORTE_FAC * taxh.PORC / 100), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC)).ToString();
                            }
                            else
                            {
                                if (doc.PAIS_ID == "CO")
                                {
                                    conta.BALANCE = (Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC) - Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC)).ToString();
                                }
                                else
                                {
                                    conta.BALANCE = (Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD + (doc.MONTO_DOC_MD * taxh.PORC / 100)), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC) - Conversion(Convert.ToDecimal(doc.MONTO_DOC_MD), clien.EXPORTACION, Convert.ToDecimal(cambio.UKURS), ref conta.AMOUNT_LC)).ToString();
                                }
                            }                            
                            conta.COMP_CODE = doc.SOCIEDAD_ID;
                            conta.BUS_AREA = conp[i].BUS_AREA;
                            conta.POST_KEY = conp[i].POSTING_KEY;
                            conta.TEXT = doc.CONCEPTO;
                            conta.REF_KEY1 = clien.STCD1;
                            conta.REF_KEY3 = clien.NAME1;
                            conta.CUSTOMER = doc.PAYER_ID;
                            conta.SALES_ORG = clien.VKORG;
                            conta.DIST_CHANEL = clien.VTWEG;
                            conta.DIVISION = clien.SPART;
                            conta.TAX_CODE = conp[i].TAX_CODE;
                            contas.Add(conta);
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return "Error al obtener detalle contable";
            }
        }
        public decimal Conversion(decimal cantidad, string exportacion, decimal conversion, ref string amount)
        {
            if (String.IsNullOrEmpty(exportacion) == false)
            {
                amount = cantidad.ToString();
                return Decimal.Round(cantidad / conversion, 2);
            }
            else
            {
                amount = "";
                return cantidad;
            }
        }
    }
    public class DetalleContab
    {
        public string POS_TYPE;
        public string COMP_CODE;
        public string BUS_AREA;
        public string POST_KEY;
        public string ACCOUNT;
        public string COST_CENTER;
        public string BALANCE;
        public string TEXT;
        public string SALES_ORG;
        public string DIST_CHANEL;
        public string DIVISION;
        public string INV_REF;
        public string PAY_TERM;
        public string JURIS_CODE;
        public string SALES_DIST;
        public string CUSTOMER;
        public string PRODUCT;
        public string TAX_CODE;
        public string PLANT;
        public string REF_KEY1;
        public string REF_KEY2;
        public string REF_KEY3;
        public string ASSIGNMENT;
        public string QTY;
        public string BASE_UNIT;
        public string AMOUNT_LC;
        public string RETENCION_ID;
    }

}
