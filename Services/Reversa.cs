using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TAT001.Entities;
using TAT001.Models;

namespace TAT001.Services
{
    public class Reversa
    {
        public string creaReversa(string id_d, string tsol, ref decimal num_doc, bool total = false)
        {
            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            var relacionada_neg = "";
            var relacionada_dis = "";

            DOCUMENTO dOCUMENTO = new DOCUMENTO();
            DOCUMENTO dOCpADRE = new DOCUMENTO();
            string errorString = "";
            TruequeEntities db = new TruequeEntities();

            string p = "";
            List<TREVERSAT> ldocr = new List<TREVERSAT>();
            decimal rel = 0;
            try
            {
                if (id_d == null || id_d.Equals(""))
                {
                    throw new Exception();
                }
                rel = Convert.ToDecimal(id_d);

            }
            catch
            {
                rel = 0;
            }
            //---
            //Obtener los valores de tsols
            List<TSOL> tsols_val = new List<TSOL>();
            //List<TSOLT_MODBD> tsols_valbd = new List<TSOLT_MODBD>();//RSG 13.06.2018
            try
            {
                tsols_val = db.TSOLs.ToList();
                //tsols_valbd = tsols_val.Select(tsv => new TSOLT_MODBD
                //{
                //    ID = tsv.ID,
                //    FACTURA = tsv.FACTURA
                //}).ToList();
            }
            catch (Exception e)
            {

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
                    freversa = theTime.ToString("yyyy-MM-dd"); ;
                    //Obtener los tipos de reversas
                    try
                    {
                        ldocr = db.TREVERSATs.Where(a => a.TREVERSA.ACTIVO == true && a.SPRAS_ID == dOCpADRE.USUARIO.SPRAS_ID).ToList();
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
            //---

            //Obtener los documentos relacionados
            List<DOCUMENTO> docsrel = new List<DOCUMENTO>();

            SOCIEDAD id_bukrs = new SOCIEDAD();
            var id_pais = new PAI();
            var id_waers = db.MONEDAs.Where(m => m.ACTIVO == true).ToList();

            if (rel > 0)
            {
                dOCpADRE = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).FirstOrDefault();
                ////if (dOCpADRE.TIPO_RECURRENTE == null)//RSG 28.05.2018----------------------------------------------
                ////    return 0;
                ////if (!((dOCpADRE.TIPO_RECURRENTE.Equals("M") | dOCpADRE.TIPO_RECURRENTE.Equals("P")) & dOCpADRE.ESTATUS.Equals("A") & dOCpADRE.ESTATUS_WF.Equals("A")))//RSG 28.05.2018
                ////{
                ////    return 0;
                ////}
                //////List<DOCUMENTOREC> ddrec = new List<DOCUMENTOREC>();
                ////DOCUMENTOREC drec = dOCpADRE.DOCUMENTORECs.Where(a => a.ESTATUS == "A").FirstOrDefault();
                ////if (drec == null)
                ////    return 0;
                ////else
                ////{
                ////    DateTime hoy = (DateTime)drec.FECHAF;
                ////    var primer = new DateTime(hoy.Year, hoy.Month, 1);
                ////    var ultimo = primer.AddMonths(1).AddDays(-1);
                ////    dOCUMENTO.FECHAI_VIG = primer;
                ////    dOCUMENTO.FECHAF_VIG = ultimo;
                ////    dOCUMENTO.MONTO_DOC_MD = (decimal)drec.MONTO_BASE;
                ////}
                ////if (tsol != dOCpADRE.TSOL_ID)
                ////    return 0;
                //////RSG 28.05.2018----------------------------------------------
                docsrel = db.DOCUMENTOes.Where(docr => docr.DOCUMENTO_REF == rel).ToList();
                id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == dOCpADRE.SOCIEDAD_ID && soc.ACTIVO == true).FirstOrDefault();
                id_pais = db.PAIS.Where(pais => pais.LAND.Equals(dOCpADRE.PAIS_ID)).FirstOrDefault();//RSG 15.05.2018
                dOCUMENTO.DOCUMENTO_REF = rel;
                relacionada_neg = dOCpADRE.TIPO_TECNICO;
                ////ViewBag.TSOL_ANT = dOCUMENTO.TSOL_ID;

                if (dOCUMENTO != null)
                {

                    dOCUMENTO.TSOL_ID = tsol;
                    dOCUMENTO.NUM_DOC = 0;
                    ////foreach (DOCUMENTOP pos in dOCpADRE.DOCUMENTOPs)
                    ////{

                    ////}
                    List<DOCUMENTOP> docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == dOCpADRE.NUM_DOC).ToList();//Documentos que se obtienen de la provisión
                    List<DOCUMENTOP> docsrelp = new List<DOCUMENTOP>();

                    if (docsrel.Count > 0)
                    {
                        docsrelp = docsrel
                            .Where(x => x.ESTATUS_C != "C" & x.ESTATUS_WF != "B")//ADD RSG 15.11.2018
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
                    }
                    List<TAT001.Models.DOCUMENTOP_MOD> docsp = new List<DOCUMENTOP_MOD>();
                    decimal resta = 0;//ADD RSG 15.11.2018
                    var dis = "";
                    for (int j = 0; j < docpl.Count; j++)
                    {
                        try
                        {
                            //Documentos de la provisión
                            DOCUMENTOP_MOD docP = new DOCUMENTOP_MOD();
                            docP.NUM_DOC = dOCpADRE.NUM_DOC;
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
                            ////docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                            docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                            docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                            ////docP.APOYO_EST = docpl[j].APOYO_EST;
                            docP.APOYO_REAL = docpl[j].APOYO_EST;//RSG 26.12.2018
                            docP.VOLUMEN_REAL = docpl[j].VOLUMEN_EST;//RSG 26.12.2018

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

                                    ////docP.VOLUMEN_EST -= docr_vr;
                                    ////docP.APOYO_EST -= docr_ar;
                                    docP.VOLUMEN_REAL -= docr_vr;//RSG 26.12.2018
                                    docP.APOYO_REAL -= docr_ar;//RSG 26.12.2018

                                    if (dis == "C")
                                    {
                                        //decimal docr_vr = Convert.ToDecimal(docrel[k].);
                                        //decimal docr_ar = Convert.ToDecimal(docrel[k].APOYO_REAL);
                                    }

                                }
                            }

                            //Siempre tiene que ser igual a 0
                            if (docP.VOLUMEN_REAL < 0)
                            {
                                docP.VOLUMEN_REAL = 0;
                            }
                            if (docP.APOYO_REAL < 0)
                            {
                                resta += (decimal)docP.APOYO_REAL;
                                docP.APOYO_REAL = 0;
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
                    
                    //if ((docsp.Count - mayores) > 0)
                    //    resta = resta / (docsp.Count - mayores);
                    while (resta != 0)
                    {
                        foreach (DOCUMENTOP_MOD ddp in docsp)
                        {
                            if (ddp.APOYO_REAL > 0)
                            {
                                ddp.APOYO_REAL += resta;
                                if (ddp.APOYO_REAL <= 0)
                                {
                                    resta = (decimal)ddp.APOYO_REAL;
                                    ddp.APOYO_REAL = 0;
                                }
                                else
                                    resta = 0;
                            }
                        }
                    }
                    //ADD RSG 15.11.2018------------------------------------F
                    dOCUMENTO.DOCUMENTOP = docsp;
                }
            }
            else
            {
            }

            dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;
            dOCUMENTO.PAIS_ID = id_pais.LAND;//RSG 18.05.2018
            dOCUMENTO.MONEDA_ID = id_bukrs.WAERS;
            dOCUMENTO.PERIODO = Convert.ToInt32(DateTime.Now.ToString("MM"));
            dOCUMENTO.EJERCICIO = Convert.ToString(DateTime.Now.Year);

            dOCUMENTO.FECHAD = theTime;

            //----------------------------RSG 18.05.2018

            //dOCUMENTO.SOCIEDAD = db.SOCIEDADs.Find(dOCUMENTO.SOCIEDAD_ID);
            //----------------------------RSG 18.05.2018


            dOCUMENTO.MONTO_DOC_MD = decimal.Parse("0.00");
            foreach (DOCUMENTOP_MOD dpm in dOCUMENTO.DOCUMENTOP)
            {
                DOCUMENTOP ddp = new DOCUMENTOP();
                ddp.MATKL = dpm.MATKL_ID;
                ddp.MATNR = new Cadena().completaMaterial(dpm.MATNR);
                ddp.MONTO = dpm.MONTO;
                ddp.MONTO_APOYO = dpm.MONTO_APOYO;
                ddp.PORC_APOYO = dpm.PORC_APOYO;
                ddp.POS = dpm.POS;
                ddp.PRECIO_SUG = dpm.PRECIO_SUG;
                ddp.VIGENCIA_AL = dpm.VIGENCIA_AL;
                ddp.VIGENCIA_DE = dpm.VIGENCIA_DE;
                ddp.VOLUMEN_EST = dpm.VOLUMEN_EST;
                ddp.VOLUMEN_REAL = dpm.VOLUMEN_REAL;
                ddp.APOYO_EST = dpm.APOYO_EST;
                ddp.APOYO_REAL = dpm.APOYO_REAL;
                ddp.CANTIDAD = dpm.CANTIDAD;

                DOCUMENTOP dpp = dOCpADRE.DOCUMENTOPs.Where(x => x.POS == ddp.POS).FirstOrDefault();
                foreach (DOCUMENTOM dm in dpp.DOCUMENTOMs)
                {
                    DOCUMENTOM dmm = new DOCUMENTOM();
                    ////dmm.APOYO_EST = dm.APOYO_EST;
                    ////dmm.APOYO_REAL = dm.APOYO_REAL;
                    dmm.APOYO_REAL = dm.APOYO_EST;
                    dmm.MATNR = dm.MATNR;
                    //dmm.NUM_DOC = dm.NUM_DOC;
                    dmm.PORC_APOYO = dm.PORC_APOYO;
                    dmm.POS = dm.POS;
                    dmm.POS_ID = dm.POS_ID;
                    dmm.VALORH = dm.VALORH;
                    dmm.VIGENCIA_A = dm.VIGENCIA_A;
                    dmm.VIGENCIA_DE = dm.VIGENCIA_DE;

                    ddp.DOCUMENTOMs.Add(dmm);
                }

                dOCUMENTO.DOCUMENTOPs.Add(ddp);
                ////dOCUMENTO.MONTO_DOC_MD += ddp.APOYO_EST;
                dOCUMENTO.MONTO_DOC_MD += ddp.APOYO_REAL;
            }

            foreach (DOCUMENTOP dpp in dOCpADRE.DOCUMENTOPs)
            {

            }
            ////HTTPPOST
            DOCUMENTO d = new DOCUMENTO();
            if (dOCUMENTO.DOCUMENTO_REF > 0)
            {
                d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).FirstOrDefault();
                //dOCUMENTO.TSOL_ID = d.TSOL_ID;
                id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == d.SOCIEDAD_ID).FirstOrDefault();
                dOCUMENTO.ESTADO = d.ESTADO;
                dOCUMENTO.CIUDAD = d.CIUDAD;
                dOCUMENTO.PAYER_ID = d.PAYER_ID;
                dOCUMENTO.CONCEPTO = d.CONCEPTO;
                dOCUMENTO.NOTAS = d.NOTAS;
                dOCUMENTO.FECHAI_VIG = d.FECHAI_VIG;
                dOCUMENTO.FECHAF_VIG = d.FECHAF_VIG;
                dOCUMENTO.PAYER_NOMBRE = d.PAYER_NOMBRE;
                dOCUMENTO.PAYER_EMAIL = d.PAYER_EMAIL;
                dOCUMENTO.TIPO_CAMBIO = d.TIPO_CAMBIO;
                dOCUMENTO.GALL_ID = d.GALL_ID;
                dOCUMENTO.TALL_ID = d.TALL_ID;
                //Obtener el país
                dOCUMENTO.PAIS_ID = d.PAIS_ID;//RSG 15.05.2018
                //dOCUMENTO.TSOL_ID = d.TSOL_ID;
            }

            //Tipo técnico
            dOCUMENTO.TIPO_TECNICO = relacionada_neg;

            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);//RSG 02/05/2018
            Rangos rangos = new Rangos();//RSG 01.08.2018
            //Obtener el número de documento
            decimal N_DOC = rangos.getSolID(dOCUMENTO.TSOL_ID);
            dOCUMENTO.NUM_DOC = N_DOC;

            //Obtener SOCIEDAD_ID                     
            dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;

            ////Obtener el país
            //dOCUMENTO.PAIS_ID = p.ToUpper();

            //CANTIDAD_EV > 1 si son recurrentes
            dOCUMENTO.CANTIDAD_EV = 1;

            //Obtener usuarioc
            dOCUMENTO.PUESTO_ID = u.PUESTO_ID;//RSG 02/05/2018
            dOCUMENTO.USUARIOC_ID = u.ID;
            dOCUMENTO.USUARIOD_ID = u.ID;

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

            TCambio tcambio = new TCambio();//RSG 01.08.2018
            //Obtener el monto de la sociedad
            dOCUMENTO.MONTO_DOC_ML = tcambio.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);
            if (!errorString.Equals(""))
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
            CLIENTE payer = getCliente(dOCUMENTO.PAYER_ID);

            dOCUMENTO.VKORG = payer.VKORG;
            dOCUMENTO.VTWEG = payer.VTWEG;
            dOCUMENTO.SPART = payer.SPART;

            //dOCUMENTO.DOCUMENTO_REF = null;
            dOCUMENTO.TSOL_ID = tsol;
            //Guardar el documento
            db.DOCUMENTOes.Add(dOCUMENTO);
            db.SaveChanges();

            //Actualizar el rango
            rangos.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);


            DOCUMENTO referencia = db.DOCUMENTOes.Find(dOCUMENTO.DOCUMENTO_REF);
            referencia.ESTATUS = "R";
            db.Entry(referencia).State = EntityState.Modified;
            db.SaveChanges();
            //Guardar los documentos p para el documento guardado

            ProcesaFlujo pf = new ProcesaFlujo();
            //db.DOCUMENTOes.Add(dOCUMENTO);
            //db.SaveChanges();

            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(d.USUARIOC_ID)).FirstOrDefault();
            //int rol = user.MIEMBROS.FirstOrDefault().ROL_ID;
            try
            {
                //WORKFV wf = db.WORKFHs.Where(a => a.BUKRS.Equals(dOCUMENTO.SOCIEDAD_ID) & a.ROL_ID == rol).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
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
                    f.ESTATUS = "I";
                    f.FECHAC = DateTime.Now;
                    f.FECHAM = DateTime.Now;
                    string c = pf.procesa(f, "");
                    //RSG 28.05.2018 -----------------------------------
                    //if (c == "1")
                    //{
                    //    Email em = new Email();
                    //    em.enviaMail(f.NUM_DOC, true);
                    //}
                    ////FLUJO conta = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
                    ////conta.USUARIOA_ID = user.ID;
                    ////conta.ESTATUS = "A";
                    ////conta.FECHAM = DateTime.Now;
                    ////pf.procesa(conta, "");
                    //RSG 28.05.2018 -----------------------------------
                    //RSG 24.12.2018 -----------------------------------

                    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    Estatus es = new Estatus();//RSG 18.09.2018
                    DOCUMENTO doc = db.DOCUMENTOes.Find(f.NUM_DOC);
                    conta.STATUS = es.getEstatus(doc);
                    db.Entry(conta).State = EntityState.Modified;
                    db.SaveChanges();

                    num_doc = f.NUM_DOC;
                    return c;
                }
            }
            catch (Exception ee)
            {
                if (errorString == "")
                {
                    errorString = ee.Message.ToString();
                    return "0";
                }
                //ViewBag.error = errorString;
            }
            return "";
        }


        //public RANGO getRango(string TSOL_ID)
        //{
        //    RANGO rango = new RANGO();
        //    using (TruequeEntities db = new TruequeEntities())
        //    {

        //        rango = (from r in db.RANGOes
        //                 join s in db.TSOLs
        //                 on r.ID equals s.RANGO_ID
        //                 where s.ID == TSOL_ID && r.ACTIVO == true
        //                 select r).FirstOrDefault();

        //    }

        //    return rango;

        //}

        //public decimal getSolID(string TSOL_ID)
        //{

        //    decimal id = 0;

        //    RANGO rango = getRango(TSOL_ID);

        //    if (rango.ACTUAL > rango.INICIO && rango.ACTUAL < rango.FIN)
        //    {
        //        rango.ACTUAL++;
        //        id = (decimal)rango.ACTUAL;
        //    }

        //    return id;
        //}
        //public decimal getValSoc(string waers, string moneda_id, decimal monto_doc_md, out string errorString)
        //{
        //    decimal val = 0;

        //    //Siempre la conversión va a la sociedad    

        //    var UKURS = getUkurs(waers, moneda_id, out errorString);

        //    if (errorString.Equals(""))
        //    {

        //        decimal uk = Convert.ToDecimal(UKURS);

        //        if (UKURS > 0)
        //        {
        //            val = uk * Convert.ToDecimal(monto_doc_md);
        //        }
        //    }

        //    return val;
        //}

        //public decimal getUkurs(string waers, string moneda_id, out string errorString)
        //{
        //    decimal ukurs = 0;
        //    errorString = string.Empty;
        //    using (TruequeEntities db = new TruequeEntities())
        //    {
        //        try
        //        {
        //            //Siempre la conversión va a la sociedad    
        //            var date = DateTime.Now.Date;
        //            var tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(waers) && t.GDATU.Equals(date)).FirstOrDefault();
        //            if (tcambio == null)
        //            {
        //                var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(waers)).Max(a => a.GDATU);
        //                tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(waers) && t.GDATU.Equals(max)).FirstOrDefault();
        //            }

        //            ukurs = Convert.ToDecimal(tcambio.UKURS);

        //        }
        //        catch (Exception e)
        //        {
        //            errorString = "detail: conversion " + moneda_id + " to " + waers + " in date " + DateTime.Now.Date;
        //            return 0;
        //        }
        //    }

        //    return ukurs;
        //}

        //public decimal getUkursUSD(string waers, string waersusd, out string errorString)
        //{
        //    decimal ukurs = 0;
        //    errorString = string.Empty;
        //    using (TruequeEntities db = new TruequeEntities())
        //    {
        //        try
        //        {
        //            var date = DateTime.Now.Date;
        //            var tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(waers) && t.TCURR.Equals(waersusd) && t.GDATU.Equals(date)).FirstOrDefault();
        //            if (tcambio == null)
        //            {
        //                var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(waers) && t.TCURR.Equals(waersusd)).Max(a => a.GDATU);
        //                tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(waers) && t.TCURR.Equals(waersusd) && t.GDATU.Equals(max)).FirstOrDefault();
        //            }

        //            ukurs = Convert.ToDecimal(tcambio.UKURS);
        //        }
        //        catch (Exception e)
        //        {
        //            errorString = "detail: conversion " + waers + " to " + waersusd + " in date " + DateTime.Now.Date;
        //            return 0;
        //        }

        //    }

        //    return ukurs;
        //}
        public CLIENTE getCliente(string PAYER_ID)
        {
            CLIENTE payer = new CLIENTE();

            using (TruequeEntities db = new TruequeEntities())
            {

                payer = db.CLIENTEs.Where(c => c.KUNNR.Equals(PAYER_ID)).FirstOrDefault();

            }
            return payer;

        }


        //public void updateRango(string TSOL_ID, decimal actual)
        //{
        //    TruequeEntities db = new TruequeEntities();
        //    RANGO rango = getRango(TSOL_ID);

        //    if (rango.ACTUAL > rango.INICIO && rango.ACTUAL < rango.FIN)
        //    {
        //        rango.ACTUAL = actual;
        //    }

        //    db.Entry(rango).State = EntityState.Modified;
        //    db.SaveChanges();

        //}
        //private string completaMaterial(string material)//RSG 07.06.2018---------------------------------------------
        //{
        //    string m = material;
        //    try
        //    {
        //        long matnr1 = long.Parse(m);
        //        int l = 18 - m.Length;
        //        for (int i = 0; i < l; i++)
        //        {
        //            m = "0" + m;
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return m;
        //}
    }
}