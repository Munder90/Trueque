using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Models.Dao;

namespace TAT001.Services
{
    class Recurrente
    {
        //------------------DAO------------------------------
        readonly MaterialesgptDao materialesgptDao = new MaterialesgptDao();

        public int creaRecurrente(decimal id_d, string tsol, DateTime fechaActual, int posicion, bool objQ)
        {
            int ret = 1;
            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            ////var relacionada_neg = "";
            ////var relacionada_dis = "";

            DOCUMENTO dOCUMENTO = new DOCUMENTO();
            DOCUMENTO dOCpADRE = new DOCUMENTO();
            string errorString = "";
            TruequeEntities db = new TruequeEntities();

            ////string p = "";
            ////List<TREVERSAT> ldocr = new List<TREVERSAT>();
            decimal rel = 0;
            try
            {
                if (id_d == 0)
                {
                    return 0;
                }
                rel = Convert.ToDecimal(id_d);

            }
            catch
            {
                rel = 0;
            }

            //Obtener los documentos relacionados
            ////List<DOCUMENTO> docsrel = new List<DOCUMENTO>();

            SOCIEDAD id_bukrs = new SOCIEDAD();
            var id_pais = new PAI();
            ////var id_waers = db.MONEDAs.Where(m => m.ACTIVO).ToList();

            if (rel > 0)
            {
                dOCpADRE = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).FirstOrDefault();
                if (dOCpADRE.TIPO_RECURRENTE == null)//RSG 28.05.2018----------------------------------------------
                    return 0;
                if (!((dOCpADRE.TIPO_RECURRENTE.Equals("1") || dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3")) && dOCpADRE.ESTATUS.Equals("A") && dOCpADRE.ESTATUS_WF.Equals("A")))//RSG 28.05.2018
                {
                    return 0;
                }

                DOCUMENTOREC drec = dOCpADRE.DOCUMENTORECs.FirstOrDefault(a => a.POS == posicion);
                if (drec == null)
                    return 0;
                else
                {
                    ////DateTime hoy = (DateTime)drec.FECHAF;
                    Calendario445 cal = new Calendario445();
                    int restarMes = 0;
                    if (dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                    {
                        restarMes = 1;
                    }

                    var primer = cal.getPrimerDia(drec.DOCUMENTO.FECHAI_VIG.Value.Year, drec.DOCUMENTO.FECHAI_VIG.Value.Month - restarMes);
                    var ultimo = cal.getUltimoDia(drec.DOCUMENTO.FECHAI_VIG.Value.Year, drec.DOCUMENTO.FECHAI_VIG.Value.Month - restarMes);

                    dOCUMENTO.FECHAI_VIG = primer;
                    dOCUMENTO.FECHAF_VIG = ultimo;
                    dOCUMENTO.MONTO_DOC_MD = (decimal)drec.MONTO_BASE;
                }
                if (tsol != dOCpADRE.TSOL_ID)
                    return 0;
                //RSG 28.05.2018----------------------------------------------
                ////docsrel = db.DOCUMENTOes.Where(docr => docr.DOCUMENTO_REF == rel).ToList();
                id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == dOCpADRE.SOCIEDAD_ID && soc.ACTIVO).FirstOrDefault();
                id_pais = db.PAIS.Where(pais => pais.LAND.Equals(dOCpADRE.PAIS_ID)).FirstOrDefault();//RSG 15.05.2018
                dOCUMENTO.DOCUMENTO_REF = rel;
                ////relacionada_neg = dOCpADRE.TIPO_TECNICO;

                ////if (dOCUMENTO != null)
                ////{

                dOCUMENTO.TSOL_ID = tsol;
                dOCUMENTO.NUM_DOC = 0;
                ////foreach (DOCUMENTOP pos in dOCpADRE.DOCUMENTOPs)
                ////{

                ////}
                ////}
            }

            dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;
            dOCUMENTO.PAIS_ID = id_pais.LAND;//RSG 18.05.2018
            dOCUMENTO.MONEDA_ID = id_bukrs.WAERS;
            dOCUMENTO.PERIODO = Convert.ToInt32(DateTime.Now.ToString("MM"));
            dOCUMENTO.EJERCICIO = Convert.ToString(DateTime.Now.Year);
            dOCUMENTO.TIPO_RECURRENTE = (int.Parse(dOCpADRE.TIPO_RECURRENTE) + 5).ToString();
            dOCUMENTO.FECHAD = theTime;


            ////HTTPPOST
            DOCUMENTO d = new DOCUMENTO();
            if (dOCUMENTO.DOCUMENTO_REF > 0)
            {
                d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).FirstOrDefault();
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
                dOCUMENTO.TSOL_ID = d.TSOL_ID;
            }

            //Tipo técnico
            dOCUMENTO.TIPO_TECNICO = dOCpADRE.TIPO_TECNICO;

            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);//RSG 02/05/2018
            //Obtener el número de documento
            ////Rangos ran = new Rangos();
            ////decimal N_DOC = ran.getSolID(dOCUMENTO.TSOL_ID);
            ////dOCUMENTO.NUM_DOC = N_DOC;


            //Obtener SOCIEDAD_ID                     
            dOCUMENTO.SOCIEDAD_ID = id_bukrs.BUKRS;

            //CANTIDAD_EV > 1 si son recurrentes
            dOCUMENTO.CANTIDAD_EV = 1;

            //Obtener usuarioc
            dOCUMENTO.PUESTO_ID = u.PUESTO_ID;//RSG 02/05/2018
            dOCUMENTO.USUARIOC_ID = u.ID;

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

            ///////////////////////Montos
            //////MONTO_DOC_MD
            ////var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
            ////dOCUMENTO.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);

            //////Obtener el monto de la sociedad
            ////TCambio tc = new TCambio();
            ////dOCUMENTO.MONTO_DOC_ML = tc.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);
            ////if (!String.IsNullOrEmpty(errorString))
            ////{
            ////    return 0;
            ////}

            //////MONTO_DOC_ML2 
            ////var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_ML2;
            ////dOCUMENTO.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

            //////MONEDAL_ID moneda de la sociedad
            ////dOCUMENTO.MONEDAL_ID = id_bukrs.WAERS;

            //////MONEDAL2_ID moneda en USD
            ////dOCUMENTO.MONEDAL2_ID = "USD";

            //////Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
            ////dOCUMENTO.TIPO_CAMBIOL = tc.getUkurs(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, out errorString);

            //////Tipo cambio dolares TIPO_CAMBIOL2
            ////dOCUMENTO.TIPO_CAMBIOL2 = tc.getUkursUSD(dOCUMENTO.MONEDA_ID, "USD", out errorString);
            ////if (!String.IsNullOrEmpty(errorString))
            ////{
            ////    return 0;
            ////}
            //Obtener datos del payer
            CLIENTE payer = getCliente(dOCUMENTO.PAYER_ID);

            dOCUMENTO.VKORG = payer.VKORG;
            dOCUMENTO.VTWEG = payer.VTWEG;
            dOCUMENTO.SPART = payer.SPART;

            dOCUMENTO.DOCUMENTO_REF = null;

            //ADD 04.11.2018---------------------------------------------
            CUENTA cta = db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(dOCUMENTO.SOCIEDAD_ID) && x.PAIS_ID.Equals(dOCUMENTO.PAIS_ID) && x.TALL_ID.Equals(dOCUMENTO.TALL_ID)).FirstOrDefault();
            if (cta != null)
            {
                dOCUMENTO.CUENTAP = cta.ABONO;
                dOCUMENTO.CUENTAPL = cta.CARGO;
                dOCUMENTO.CUENTACL = cta.CLEARING;
            }
            //ADD 04.11.2018---------------------------------------------

            //Guardar el documento       
            ////db.DOCUMENTOes.Add(dOCUMENTO);
            ////db.SaveChanges();

            //////Actualizar el rango
            ////ran.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);

            List<CategoriaMaterial> listcatm = new List<CategoriaMaterial>();
            List<DOCUMENTOM_MOD> listmatm = new List<DOCUMENTOM_MOD>();
            List<string> listcat = new List<string>();
            List<string> listmat = new List<string>();

            decimal venta_total = 0;

            //Guardar los documentos p para el documento guardado
            try
            {
                //Agregar materiales existentes para evitar que en la vista se hayan agregado o quitado
                List<DOCUMENTOP> docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == dOCpADRE.NUM_DOC).ToList();

                for (int j = 0; j < docpl.Count; j++)
                {
                    try
                    {
                        ////var cat = "";
                        ////if (docpl[j].MATNR != null && docpl[j].MATNR != "")
                        ////{
                        ////    cat = "M";
                        ////}
                        ////else
                        ////{
                        ////    cat = "C";
                        ////}

                        if (dOCpADRE.TIPO_RECURRENTE.Equals("1") || dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                        {
                            DOCUMENTOP docP = new DOCUMENTOP();
                            docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                            docP.POS = docpl[j].POS;
                            if (docpl[j].MATNR == null || docpl[j].MATNR == "")
                            {
                                docpl[j].MATNR = "";
                            }
                            docP.MATNR = docpl[j].MATNR;
                            docP.MATKL = docpl[j].MATKL;
                            docP.CANTIDAD = 1;
                            docP.MONTO = docpl[j].MONTO;
                            docP.PORC_APOYO = docpl[j].PORC_APOYO;
                            ////docP.MONTO_APOYO = docmod.MONTO_APOYO;
                            docP.MONTO_APOYO = docP.MONTO * (docP.PORC_APOYO / 100);
                            docP.MONTO_APOYO = Math.Round(docP.MONTO_APOYO, 2);//docP.MONTO_APOYO = Math.Round(docP.MONTO_APOYO, 2);
                            docP.PRECIO_SUG = docpl[j].PRECIO_SUG;
                            docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                            docP.VOLUMEN_REAL = docpl[j].VOLUMEN_REAL;
                            ////docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                            ////docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                            docP.VIGENCIA_DE = dOCUMENTO.FECHAI_VIG;
                            docP.VIGENCIA_AL = dOCUMENTO.FECHAF_VIG;
                            docP.APOYO_EST = docpl[j].APOYO_EST;
                            docP.APOYO_REAL = docpl[j].APOYO_REAL;
                            dOCUMENTO.DOCUMENTOPs.Add(docP);
                        }

                        ////Agregarlo a la bd
                        ////db.DOCUMENTOPs.Add(docP);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }

                }

                if (dOCpADRE.TIPO_RECURRENTE.Equals("1") || dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                {
                    //Si la distribución es categoría se obtienen las categorías
                    decimal totalcats = 0;
                    for (int j = 0; j < dOCUMENTO.DOCUMENTOPs.Count; j++)
                    {
                        if (dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATNR == null || dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATNR == "")
                        {
                            string cat = dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATKL.ToString();
                            listcat.Add(cat);
                        }
                        else
                        {
                            string mat = dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATNR.ToString();
                            listmat.Add(mat);
                        }
                    }
                    if (listcat.Count > 0)
                        listcatm = grupoCategoriasController(listcat, d.VKORG, d.SPART, d.PAYER_ID, d.SOCIEDAD_ID, out totalcats, fechaActual, dOCpADRE.TIPO_RECURRENTE);
                    else
                        listmatm = grupoMaterialesController(listmat, d.VKORG, d.SPART, d.PAYER_ID, d.SOCIEDAD_ID, out totalcats, fechaActual, dOCpADRE.TIPO_RECURRENTE);

                    venta_total = totalcats;
                    //----------------------------RSG 23.07.2018

                    dOCUMENTO.LIGADA = dOCpADRE.LIGADA;
                    if (dOCUMENTO.LIGADA == true)
                    {
                        ////dOCUMENTO.MONTO_DOC_MD = 0;
                        ////foreach (CategoriaMaterial catm in listcatm)
                        ////{
                        ////    foreach (DOCUMENTOM_MOD mats in catm.MATERIALES)
                        ////        dOCUMENTO.MONTO_DOC_MD += mats.VAL;
                        ////}
                        dOCUMENTO.MONTO_DOC_MD = totalcats;
                        bool sinO = false;
                        foreach (DOCUMENTORAN dran in dOCpADRE.DOCUMENTORECs.FirstOrDefault(x => x.POS == posicion).DOCUMENTORANs)
                        {
                            if (dOCUMENTO.MONTO_DOC_MD > dran.OBJETIVOI)
                            {
                                dOCUMENTO.MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD * dran.PORCENTAJE / 100;
                                dOCUMENTO.PORC_APOYO = dran.PORCENTAJE;
                                sinO = true;
                                if (objQ)
                                {
                                    if (dran.POS == 1 || dran.POS != 1)
                                    {
                                        dOCUMENTO.MONTO_DOC_MD = dOCpADRE.DOCUMENTORECs.FirstOrDefault(x => x.POS == posicion).DOCUMENTORANs.FirstOrDefault(x => x.LIN == 1).OBJETIVOI * dOCpADRE.OBJQ_PORC / 100;
                                        dOCUMENTO.PORC_APOYO = dOCpADRE.OBJQ_PORC;
                                    }
                                }
                                break;
                            }
                        }
                        if (!sinO)
                        {
                            dOCUMENTO.MONTO_DOC_MD = 0;
                            dOCUMENTO.PORC_APOYO = 0;
                            ret = 0;
                        }
                    }
                    decimal suma_monto = 0;
                    if (!(dOCpADRE.TIPO_RECURRENTE == "1" && listcat.Count == 0))
                    {
                        foreach (DOCUMENTOP docP in dOCUMENTO.DOCUMENTOPs)
                        {
                            string col = "";
                            ////if (dOCpADRE.TSOL.PADRE)
                            ////{
                            ////    col = "E";
                            ////}
                            ////else if (dOCpADRE.TSOL.PADRE)
                            ////{
                            ////    col = "R";
                            ////}

                            if (dOCpADRE.TSOL.FACTURA)
                                col = "R";
                            else
                                col = "E";

                            docP.APOYO_REAL = 0;
                            docP.APOYO_EST = 0;
                            docP.PORC_APOYO = 0;
                            List<DOCUMENTOM> docml;//// = new List<DOCUMENTOM>();

                            if (listcat.Count > 0)
                            {
                                docml = addCatItems(listcatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                    Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, "P", "C", totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);
                            }
                            else
                            {
                                docml = addMatItems(listmatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                    Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, "P", "C", totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);

                            }
                            if (docml.Count > 0)
                            {
                                if (listcat.Count > 0)
                                {
                                    //Categoría por porcentaje
                                    for (int k = 0; k < docml.Count; k++)
                                    {
                                        try
                                        {
                                            DOCUMENTOM docM = new DOCUMENTOM();
                                            docM = docml[k];
                                            docM.POS = k + 1;

                                            docP.APOYO_REAL += docM.APOYO_REAL;
                                            docP.APOYO_EST += docM.APOYO_EST;
                                            docP.PORC_APOYO += (decimal)docM.PORC_APOYO;
                                            if (docM.APOYO_REAL != null)
                                                suma_monto += (decimal)docM.APOYO_REAL;
                                            if (docM.APOYO_EST != null)
                                                suma_monto += (decimal)docM.APOYO_EST;
                                            docP.DOCUMENTOMs.Add(docM);
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                else
                                { //Categoría por porcentaje
                                  //for (int k = 0; k < docml.Count; k++)
                                    int cont = 1;
                                    foreach (DOCUMENTOM ddd in docml.Where(x => x.MATNR == docP.MATNR))
                                    {
                                        try
                                        {
                                            DOCUMENTOM docM = new DOCUMENTOM();
                                            docM = ddd;
                                            docM.POS = cont;

                                            docP.APOYO_REAL += docM.APOYO_REAL;
                                            docP.APOYO_EST += docM.APOYO_EST;
                                            docP.PORC_APOYO += (decimal)docM.PORC_APOYO;
                                            if (docM.APOYO_REAL != null)
                                                suma_monto += (decimal)docM.APOYO_REAL;
                                            if (docM.APOYO_EST != null)
                                                suma_monto += (decimal)docM.APOYO_EST;

                                            cont++;
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }

                            }
                            else
                            {

                            }
                        }
                    }
                    //if (dOCUMENTO.MONTO_DOC_MD != suma_monto && dOCUMENTO.TIPO_RECURRENTE != "6")
                    if ((dOCUMENTO.MONTO_DOC_MD - suma_monto) > decimal.Parse("0.00000001") && dOCUMENTO.TIPO_RECURRENTE != "6")
                        dOCUMENTO.MONTO_DOC_MD = suma_monto;
                    suma_monto = 0;
                    foreach (DOCUMENTOP docP in dOCUMENTO.DOCUMENTOPs)
                    {
                        suma_monto += (decimal)docP.APOYO_EST + (decimal)docP.APOYO_REAL;
                    }
                    if (suma_monto == 0)
                        dOCUMENTO.MONTO_DOC_MD = 0;
                }
            }
            catch (Exception e)
            {

            }
            ///////////////////Montos
            //MONTO_DOC_MD
            var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
            dOCUMENTO.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);

            //Obtener el monto de la sociedad
            TCambio tc = new TCambio();
            dOCUMENTO.MONTO_DOC_ML = tc.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);
            if (!String.IsNullOrEmpty(errorString))
            {
                return 0;
            }
            
            //MONEDAL_ID moneda de la sociedad
            dOCUMENTO.MONEDAL_ID = id_bukrs.WAERS;

            //MONEDAL2_ID moneda en USD
            dOCUMENTO.MONEDAL2_ID = "USD";

            //Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
            dOCUMENTO.TIPO_CAMBIOL = tc.getUkurs(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, out errorString);

            //Tipo cambio dolares TIPO_CAMBIOL2
            dOCUMENTO.TIPO_CAMBIOL2 = tc.getUkursUSD(dOCUMENTO.MONEDA_ID, "USD", out errorString);
            if (!String.IsNullOrEmpty(errorString))
            {
                return 0;
            }
            //MONTO_DOC_ML2 
            var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_MD / dOCUMENTO.TIPO_CAMBIOL2;
            dOCUMENTO.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);
            //RSG 26.10.2018------------------------------------------
            if (dOCpADRE.DOCUMENTOFs != null)
            {
                foreach (DOCUMENTOF df in dOCpADRE.DOCUMENTOFs)
                {
                    DOCUMENTOF dnf = new DOCUMENTOF();
                    dnf.AUTORIZACION = df.AUTORIZACION;
                    dnf.BELNR = df.BELNR;
                    dnf.BILL_DOC = df.BILL_DOC;
                    dnf.CONTROL = df.CONTROL;
                    dnf.DESCRIPCION = df.DESCRIPCION;
                    dnf.EJERCICIOK = df.EJERCICIOK;
                    dnf.FACTURA = df.FACTURA;
                    dnf.FACTURAK = df.FACTURAK;
                    dnf.FECHA = df.FECHA;
                    dnf.IMPORTE_FAC = df.IMPORTE_FAC;
                    dnf.NUM_DOC = df.NUM_DOC;
                    dnf.PAYER = df.PAYER;
                    dnf.POS = df.POS;
                    dnf.PROVEEDOR = df.PROVEEDOR;
                    dnf.SOCIEDAD = df.SOCIEDAD;
                    dnf.VENCIMIENTO = df.VENCIMIENTO;
                    dOCUMENTO.DOCUMENTOFs.Add(dnf);
                }
            }
            //RSG 26.10.2018------------------------------------------

            Rangos ran = new Rangos();
            decimal N_DOC = ran.getSolID(dOCUMENTO.TSOL_ID);
            dOCUMENTO.NUM_DOC = N_DOC;
            db.DOCUMENTOes.Add(dOCUMENTO);
            ran.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);
            db.SaveChanges();

            //RSG 28.05.2018----------------------------------------------
            string recurrente = "";
            List<DOCUMENTOREC> ddrec = new List<DOCUMENTOREC>();
            DOCUMENTOREC drecc = d.DOCUMENTORECs.Where(a => a.POS == posicion).FirstOrDefault();
            if (drecc == null)
                return 0;
            else
            {
                DateTime hoy = drecc.FECHAF.Value;
                //var primer = new DateTime(hoy.Year, hoy.Month, 1);
                //var ultimo = primer.AddMonths(1).AddDays(-1);
                int restarMes = 0;
                if (dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                {
                    restarMes = 1;
                }

                Calendario445 cal = new Calendario445();
                //var primer = cal.getPrimerDia(hoy.Year, hoy.Month - restarMes);
                //var ultimo = cal.getUltimoDia(hoy.Year, hoy.Month - restarMes);
                var primer = cal.getPrimerDia(hoy.Year, cal.getPeriodo(hoy) - restarMes);
                var ultimo = cal.getUltimoDia(hoy.Year, cal.getPeriodo(hoy) - restarMes);

                dOCUMENTO.FECHAI_VIG = primer;
                dOCUMENTO.FECHAF_VIG = ultimo;
                if (!objQ)
                    drecc.MONTO_BASE = dOCUMENTO.MONTO_DOC_MD;
                ////dOCUMENTO.PORC_APOYO = drecc.PORC;
                dOCUMENTO.FECHAD = DateTime.Now;
                if (d.DOCUMENTORECs.Count > 1)
                    recurrente = "X";
                else
                    recurrente = "L";
            }
            if (!objQ)
            {
                drecc.DOC_REF = dOCUMENTO.NUM_DOC;
                drecc.ESTATUS = "P";
                if ((dOCpADRE.TIPO_RECURRENTE) != "1" && dOCpADRE.DOCUMENTORECs.Count > 1)
                {
                    drecc.FECHAF = drecc.FECHAF.Value.AddDays(1);
                    drecc.ESTATUS = "E";
                }
            }
            else
            {
                drecc.NUM_DOC_Q = dOCUMENTO.NUM_DOC;
                drecc.ESTATUS_Q = "P";
                if ((dOCpADRE.TIPO_RECURRENTE) != "1" && dOCpADRE.DOCUMENTORECs.Count > 1)
                {
                    drecc.ESTATUS_Q = "E";
                }
            }
            db.Entry(drecc).State = EntityState.Modified;

            if (!objQ)
            {
                if (dOCpADRE.TIPO_RECURRENTE == "2" || dOCpADRE.TIPO_RECURRENTE == "3")
                {
                    DOCUMENTOL dl = new DOCUMENTOL();
                    dl.ESTATUS = null;
                    dl.FECHAF = drecc.FECHAF.Value.AddDays(4);
                    if (dOCUMENTO.PORC_APOYO > 0)
                        dl.MONTO_VENTA = (dOCUMENTO.MONTO_DOC_MD / dOCUMENTO.PORC_APOYO) * 100;
                    if (dl.MONTO_VENTA != venta_total)
                        dl.MONTO_VENTA = venta_total;
                    dl.NUM_DOC = dOCUMENTO.NUM_DOC;
                    dl.POS = 1;
                    dOCUMENTO.DOCUMENTOLs.Add(dl);
                }
            }

            db.SaveChanges();//RSG
            ////db.SaveChanges();
            //RSG 28.05.2018----------------------------------------------

            decimal total = 0;
            //RSG 28.05.2018-----------------------------------------------------
            //RSG 28.05.2018-----------------------------------------------------

            ProcesaFlujo pf = new ProcesaFlujo();
            //db.DOCUMENTOes.Add(dOCUMENTO);
            //db.SaveChanges();

            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(d.USUARIOC_ID)).FirstOrDefault();
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
                    f.USUARIOD_ID = dOCUMENTO.USUARIOC_ID;
                    f.ESTATUS = "I";
                    f.FECHAC = DateTime.Now;
                    f.FECHAM = DateTime.Now;
                    string c = pf.procesa(f, recurrente);

                    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    Estatus es = new Estatus();//RSG 18.09.2018
                    DOCUMENTO doc = db.DOCUMENTOes.Find(f.NUM_DOC);
                    conta.STATUS = es.getEstatus(doc);
                    db.Entry(conta).State = EntityState.Modified;
                    db.SaveChanges();
                    //RSG 28.05.2018 -----------------------------------
                    if (c == "1")
                    {
                        //Email em = new Email();
                        //em.enviaMail(f.NUM_DOC, true);
                    }
                    if (dOCpADRE.TIPO_RECURRENTE == "1" || recurrente == "L")
                    {
                        conta = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
                        conta.USUARIOA_ID = user.ID;
                        conta.ESTATUS = "A";
                        conta.FECHAM = DateTime.Now;
                        string cero = "";
                        if (dOCUMENTO.MONTO_DOC_MD == 0)
                        {
                            cero = "0";
                        }
                        pf.procesa(conta, cero);
                        //RSG 28.05.2018 -----------------------------------
                        conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                        doc = db.DOCUMENTOes.Find(f.NUM_DOC);
                        conta.STATUS = es.getEstatus(doc);
                        db.Entry(conta).State = EntityState.Modified;
                        if (dOCUMENTO.MONTO_DOC_MD == 0)
                        {
                            doc.ESTATUS_C = "C";
                            db.Entry(doc).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                        doc = db.DOCUMENTOes.Find(f.NUM_DOC);
                        conta.STATUS = es.getEstatus(doc);
                        db.Entry(conta).State = EntityState.Modified;
                        doc.ESTATUS = "P";
                        doc.ESTATUS_SAP = "P";
                        doc.ESTATUS_WF = "A";
                        db.Entry(doc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ee)
            {
                if (errorString == "")
                {
                    errorString = ee.Message.ToString();
                    return 0;
                }
                //ViewBag.error = errorString;
            }

            return ret;
        }

        public int calcDistribucion(decimal id_d, string tsol, DateTime fechaActual, int posicion, bool objQ)
        {
            int ret = 1;
            string dates = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime theTime = DateTime.ParseExact(dates, //"06/04/2018 12:00:00 a.m."
                                        "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        System.Globalization.DateTimeStyles.None);

            ////var relacionada_neg = "";
            ////var relacionada_dis = "";

            DOCUMENTO dOCUMENTO = new DOCUMENTO();
            DOCUMENTO dOCpADRE = new DOCUMENTO();
            string errorString = "";
            TruequeEntities db = new TruequeEntities();

            ////string p = "";
            ////List<TREVERSAT> ldocr = new List<TREVERSAT>();
            decimal rel = 0;
            try
            {
                if (id_d == 0)
                {
                    return 0;
                }
                rel = Convert.ToDecimal(id_d);

            }
            catch
            {
                rel = 0;
            }

            //Obtener los documentos relacionados
            ////List<DOCUMENTO> docsrel = new List<DOCUMENTO>();

            SOCIEDAD id_bukrs = new SOCIEDAD();
            var id_pais = new PAI();
            ////var id_waers = db.MONEDAs.Where(m => m.ACTIVO).ToList();

            if (rel > 0)
            {
                dOCpADRE = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == rel).FirstOrDefault();
                if (dOCpADRE.TIPO_RECURRENTE == null)//RSG 28.05.2018----------------------------------------------
                    return 0;
                if (!((dOCpADRE.TIPO_RECURRENTE.Equals("1") || dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3")) && dOCpADRE.ESTATUS.Equals("A") && dOCpADRE.ESTATUS_WF.Equals("A")))//RSG 28.05.2018
                {
                    return 0;
                }

                DOCUMENTOREC drec = dOCpADRE.DOCUMENTORECs.FirstOrDefault(a => a.POS == posicion && a.DOC_REF != 0);
                if (drec == null)
                    return 0;
                else
                if (tsol != dOCpADRE.TSOL_ID)
                    return 0;

                id_bukrs = db.SOCIEDADs.Where(soc => soc.BUKRS == dOCpADRE.SOCIEDAD_ID && soc.ACTIVO).FirstOrDefault();
                id_pais = db.PAIS.Where(pais => pais.LAND.Equals(dOCpADRE.PAIS_ID)).FirstOrDefault();//RSG 15.05.2018
                if (!objQ)
                    dOCUMENTO = db.DOCUMENTOes.Find(drec.DOC_REF);
                else
                    dOCUMENTO = db.DOCUMENTOes.Find(drec.NUM_DOC_Q);


                foreach (DOCUMENTOP dp in dOCUMENTO.DOCUMENTOPs.ToList())
                {
                    foreach (DOCUMENTOM dm in dp.DOCUMENTOMs.ToList())
                    {
                        dp.DOCUMENTOMs.Remove(dm);
                    }
                    dOCUMENTO.DOCUMENTOPs.Remove(dp);
                }
                db.Entry(dOCUMENTO).State = EntityState.Modified;
                db.SaveChanges();
            }
            ////HTTPPOST
            DOCUMENTO d = new DOCUMENTO();
            if (dOCUMENTO.DOCUMENTO_REF > 0)
            {
                d = db.DOCUMENTOes.Where(doc => doc.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).FirstOrDefault();
            }

            ///////////////////////Montos
            //////MONTO_DOC_MD
            ////var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
            ////dOCUMENTO.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);

            //////Obtener el monto de la sociedad
            ////TCambio tc = new TCambio();
            ////dOCUMENTO.MONTO_DOC_ML = tc.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);
            ////if (!String.IsNullOrEmpty(errorString))
            ////{
            ////    return 0;
            ////}

            //////MONTO_DOC_ML2 
            ////var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_ML2;
            ////dOCUMENTO.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

            //////MONEDAL_ID moneda de la sociedad
            ////dOCUMENTO.MONEDAL_ID = id_bukrs.WAERS;

            //////MONEDAL2_ID moneda en USD
            ////dOCUMENTO.MONEDAL2_ID = "USD";

            //////Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
            ////dOCUMENTO.TIPO_CAMBIOL = tc.getUkurs(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, out errorString);

            //////Tipo cambio dolares TIPO_CAMBIOL2
            ////dOCUMENTO.TIPO_CAMBIOL2 = tc.getUkursUSD(dOCUMENTO.MONEDA_ID, "USD", out errorString);
            ////if (!String.IsNullOrEmpty(errorString))
            ////{
            ////    return 0;
            ////}

            List<CategoriaMaterial> listcatm = new List<CategoriaMaterial>();
            List<DOCUMENTOM_MOD> listmatm = new List<DOCUMENTOM_MOD>();
            List<string> listcat = new List<string>();
            List<string> listmat = new List<string>();

            decimal venta_total = 0;

            //Guardar los documentos p para el documento guardado
            try
            {
                //Agregar materiales existentes para evitar que en la vista se hayan agregado o quitado
                List<DOCUMENTOP> docpl = db.DOCUMENTOPs.Where(docp => docp.NUM_DOC == dOCpADRE.NUM_DOC).ToList();

                for (int j = 0; j < docpl.Count; j++)
                {
                    try
                    {
                        ////var cat = "";
                        ////if (docpl[j].MATNR != null && docpl[j].MATNR != "")
                        ////{
                        ////    cat = "M";
                        ////}
                        ////else
                        ////{
                        ////    cat = "C";
                        ////}

                        if (dOCpADRE.TIPO_RECURRENTE.Equals("1") || dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                        {
                            DOCUMENTOP docP = new DOCUMENTOP();
                            docP.NUM_DOC = dOCUMENTO.NUM_DOC;
                            docP.POS = docpl[j].POS;
                            if (docpl[j].MATNR == null || docpl[j].MATNR == "")
                            {
                                docpl[j].MATNR = "";
                            }
                            docP.MATNR = docpl[j].MATNR;
                            docP.MATKL = docpl[j].MATKL;
                            docP.CANTIDAD = 1;
                            docP.MONTO = docpl[j].MONTO;
                            docP.PORC_APOYO = docpl[j].PORC_APOYO;
                            ////docP.MONTO_APOYO = docmod.MONTO_APOYO;
                            docP.MONTO_APOYO = docP.MONTO * (docP.PORC_APOYO / 100);
                            docP.MONTO_APOYO = Math.Round(docP.MONTO_APOYO, 2);//docP.MONTO_APOYO = Math.Round(docP.MONTO_APOYO, 2);
                            docP.PRECIO_SUG = docpl[j].PRECIO_SUG;
                            docP.VOLUMEN_EST = docpl[j].VOLUMEN_EST;
                            docP.VOLUMEN_REAL = docpl[j].VOLUMEN_REAL;
                            ////docP.VIGENCIA_DE = docpl[j].VIGENCIA_DE;
                            ////docP.VIGENCIA_AL = docpl[j].VIGENCIA_AL;
                            docP.VIGENCIA_DE = dOCUMENTO.FECHAI_VIG;
                            docP.VIGENCIA_AL = dOCUMENTO.FECHAF_VIG;
                            docP.APOYO_EST = docpl[j].APOYO_EST;
                            docP.APOYO_REAL = docpl[j].APOYO_REAL;
                            dOCUMENTO.DOCUMENTOPs.Add(docP);//22.12.2018
                        }

                        ////Agregarlo a la bd
                        ////db.DOCUMENTOPs.Add(docP);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }

                }

                if (dOCpADRE.TIPO_RECURRENTE.Equals("1") || dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                {
                    //Si la distribución es categoría se obtienen las categorías
                    decimal totalcats = 0;
                    for (int j = 0; j < dOCUMENTO.DOCUMENTOPs.Count; j++)
                    {
                        if (dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATNR == null || dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATNR == "")
                        {
                            string cat = dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATKL.ToString();
                            listcat.Add(cat);
                        }
                        else
                        {
                            string mat = dOCUMENTO.DOCUMENTOPs.ElementAt(j).MATNR.ToString();
                            listmat.Add(mat);
                        }
                    }
                    if (listcat.Count > 0)
                        listcatm = grupoCategoriasController(listcat, dOCUMENTO.VKORG, dOCUMENTO.SPART, dOCUMENTO.PAYER_ID, dOCUMENTO.SOCIEDAD_ID, out totalcats, fechaActual, dOCpADRE.TIPO_RECURRENTE);
                    else
                        listmatm = grupoMaterialesController(listmat, dOCUMENTO.VKORG, dOCUMENTO.SPART, dOCUMENTO.PAYER_ID, dOCUMENTO.SOCIEDAD_ID, out totalcats, fechaActual, dOCpADRE.TIPO_RECURRENTE);

                    venta_total = totalcats;

                    if (dOCUMENTO.LIGADA == true)
                    {
                        ////dOCUMENTO.MONTO_DOC_MD = 0;
                        ////foreach (CategoriaMaterial catm in listcatm)
                        ////{
                        ////    foreach (DOCUMENTOM_MOD mats in catm.MATERIALES)
                        ////        dOCUMENTO.MONTO_DOC_MD += mats.VAL;
                        ////}
                        dOCUMENTO.MONTO_DOC_MD = totalcats;
                        bool sinO = false;
                        foreach (DOCUMENTORAN dran in dOCpADRE.DOCUMENTORECs.FirstOrDefault(x => x.POS == posicion).DOCUMENTORANs)
                        {
                            if (dOCUMENTO.MONTO_DOC_MD > dran.OBJETIVOI)
                            {
                                dOCUMENTO.MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD * dran.PORCENTAJE / 100;
                                dOCUMENTO.PORC_APOYO = dran.PORCENTAJE;
                                sinO = true;
                                if (objQ)
                                {
                                    dOCUMENTO.MONTO_DOC_MD = dOCpADRE.DOCUMENTORECs.FirstOrDefault(x => x.POS == posicion).DOCUMENTORANs.FirstOrDefault(x => x.LIN == 1).OBJETIVOI * dOCpADRE.OBJQ_PORC / 100;
                                    dOCUMENTO.PORC_APOYO = dOCpADRE.OBJQ_PORC;
                                }
                                break;
                            }
                        }
                        if (!sinO)
                        {
                            dOCUMENTO.MONTO_DOC_MD = 0;
                            dOCUMENTO.PORC_APOYO = 0;
                            ret = 0;
                        }
                    }
                    decimal suma_monto = 0;
                    if (!(dOCpADRE.TIPO_RECURRENTE == "1" && listcat.Count == 0))
                    {
                        foreach (DOCUMENTOP docP in dOCUMENTO.DOCUMENTOPs)
                        {
                            string col = "";
                            ////if (dOCpADRE.TSOL.PADRE)
                            ////{
                            ////    col = "E";
                            ////}
                            ////else if (dOCpADRE.TSOL.PADRE)
                            ////{
                            ////    col = "R";
                            ////}

                            if (dOCpADRE.TSOL.FACTURA)
                                col = "R";
                            else
                                col = "E";

                            docP.APOYO_REAL = 0;
                            docP.APOYO_EST = 0;
                            docP.PORC_APOYO = 0;
                            List<DOCUMENTOM> docml;//// = new List<DOCUMENTOM>();

                            if (listcat.Count > 0)
                            {
                                docml = addCatItems(listcatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                    Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, "P", "C", totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);
                            }
                            else
                            {
                                docml = addMatItems(listmatm, dOCUMENTO.PAYER_ID, docP.MATKL, dOCUMENTO.SOCIEDAD_ID, dOCUMENTO.NUM_DOC,
                                    Convert.ToInt16(docP.POS), docP.VIGENCIA_DE, docP.VIGENCIA_AL, "P", "C", totalcats, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), col);

                            }
                            if (docml.Count > 0)
                            {
                                if (listcat.Count > 0)
                                {
                                    //Categoría por porcentaje
                                    for (int k = 0; k < docml.Count; k++)
                                    {
                                        try
                                        {
                                            DOCUMENTOM docM = new DOCUMENTOM();
                                            docM = docml[k];
                                            docM.POS = k + 1;

                                            if (docM.APOYO_REAL != null)
                                            {
                                                docP.APOYO_REAL += docM.APOYO_REAL;
                                                suma_monto += (decimal)docM.APOYO_REAL;
                                            }
                                            if (docM.APOYO_EST != null)
                                            {
                                                suma_monto += (decimal)docM.APOYO_EST;
                                                docP.APOYO_EST += docM.APOYO_EST;
                                            }
                                            docP.PORC_APOYO += (decimal)docM.PORC_APOYO;
                                            docP.DOCUMENTOMs.Add(docM);
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                else
                                { //Categoría por porcentaje
                                  //for (int k = 0; k < docml.Count; k++)
                                    int cont = 1;
                                    foreach (DOCUMENTOM ddd in docml.Where(x => x.MATNR == docP.MATNR))
                                    {
                                        try
                                        {
                                            DOCUMENTOM docM = new DOCUMENTOM();
                                            docM = ddd;
                                            docM.POS = cont;

                                            docP.PORC_APOYO += (decimal)docM.PORC_APOYO;
                                            if (docM.APOYO_REAL != null)
                                            {
                                                docP.APOYO_REAL += docM.APOYO_REAL;
                                                suma_monto += (decimal)docM.APOYO_REAL;
                                            }
                                            if (docM.APOYO_EST != null)
                                            {
                                                suma_monto += (decimal)docM.APOYO_EST;
                                                docP.APOYO_EST += docM.APOYO_EST;
                                            }

                                            cont++;
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }

                            }
                            else
                            {

                            }
                        }
                    }
                    if ((dOCUMENTO.MONTO_DOC_MD - suma_monto) > decimal.Parse("0.00000001") && dOCUMENTO.TIPO_RECURRENTE != "6")
                        dOCUMENTO.MONTO_DOC_MD = suma_monto;
                    suma_monto = 0;
                    foreach (DOCUMENTOP docP in dOCUMENTO.DOCUMENTOPs)
                    {
                        suma_monto += (decimal)docP.APOYO_EST + (decimal)docP.APOYO_REAL;
                    }
                    if (suma_monto == 0)
                        dOCUMENTO.MONTO_DOC_MD = 0;
                }
            }
            catch (Exception e)
            {

            }

            ///////////////////Montos
            //MONTO_DOC_MD
            var MONTO_DOC_MD = dOCUMENTO.MONTO_DOC_MD;
            dOCUMENTO.MONTO_DOC_MD = Convert.ToDecimal(MONTO_DOC_MD);

            //Obtener el monto de la sociedad
            TCambio tc = new TCambio();
            dOCUMENTO.MONTO_DOC_ML = tc.getValSoc(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, Convert.ToDecimal(dOCUMENTO.MONTO_DOC_MD), out errorString);
            if (!String.IsNullOrEmpty(errorString))
            {
                return 0;
            }


            //MONEDAL_ID moneda de la sociedad
            dOCUMENTO.MONEDAL_ID = id_bukrs.WAERS;

            //MONEDAL2_ID moneda en USD
            dOCUMENTO.MONEDAL2_ID = "USD";

            //Tipo cambio de la moneda de la sociedad TIPO_CAMBIOL
            dOCUMENTO.TIPO_CAMBIOL = tc.getUkurs(id_bukrs.WAERS, dOCUMENTO.MONEDA_ID, out errorString);

            //Tipo cambio dolares TIPO_CAMBIOL2
            dOCUMENTO.TIPO_CAMBIOL2 = tc.getUkursUSD(dOCUMENTO.MONEDA_ID, "USD", out errorString);
            if (!String.IsNullOrEmpty(errorString))
            {
                return 0;
            }
            //MONTO_DOC_ML2 
            var MONTO_DOC_ML2 = dOCUMENTO.MONTO_DOC_MD / dOCUMENTO.TIPO_CAMBIOL2;
            dOCUMENTO.MONTO_DOC_ML2 = Convert.ToDecimal(MONTO_DOC_ML2);

            ////Rangos ran = new Rangos();
            ////decimal N_DOC = ran.getSolID(dOCUMENTO.TSOL_ID);
            ////dOCUMENTO.NUM_DOC = N_DOC;
            ////db.DOCUMENTOes.Add(dOCUMENTO);
            ////ran.updateRango(dOCUMENTO.TSOL_ID, dOCUMENTO.NUM_DOC);
            ////db.SaveChanges();

            //RSG 28.05.2018----------------------------------------------
            string recurrente = "";
            List<DOCUMENTOREC> ddrec = new List<DOCUMENTOREC>();
            DOCUMENTOREC drecc = db.DOCUMENTORECs.FirstOrDefault(a => a.NUM_DOC == dOCpADRE.NUM_DOC && a.POS == posicion);
            if (drecc == null)
                return 0;
            else
            {
                DateTime hoy = drecc.FECHAF.Value;
                int restarMes = 0;
                if (dOCpADRE.TIPO_RECURRENTE.Equals("2") || dOCpADRE.TIPO_RECURRENTE.Equals("3"))
                {
                    restarMes = 1;
                }

                Calendario445 cal = new Calendario445();
                //var primer = cal.getPrimerDia(hoy.Year, hoy.Month - restarMes);
                //var ultimo = cal.getUltimoDia(hoy.Year, hoy.Month - restarMes);
                var primer = cal.getPrimerDia(hoy.Year, cal.getPeriodo(hoy) - restarMes);
                var ultimo = cal.getUltimoDia(hoy.Year, cal.getPeriodo(hoy) - restarMes);

                if (!objQ)
                    drecc.MONTO_BASE = dOCUMENTO.MONTO_DOC_MD;
                if (dOCpADRE.DOCUMENTORECs.Count > 1)
                    recurrente = "X";
                else
                    recurrente = "L";
            }

            if (dOCpADRE.FRECUENCIA_LIQ == 1)
            {
                if (!objQ)
                    drecc.ESTATUS = "P";
                else
                    drecc.ESTATUS_Q = "P";
            }
            else if (dOCpADRE.FRECUENCIA_LIQ > 1)
            {
                if (drecc.POS % dOCpADRE.FRECUENCIA_LIQ == 0)
                {
                    if (!objQ)
                        drecc.ESTATUS = "P";
                    else
                        drecc.ESTATUS_Q = "P";
                    List<DOCUMENTOREC> dreccc = db.DOCUMENTORECs.Where(a => a.NUM_DOC == dOCpADRE.NUM_DOC && a.POS < posicion && a.ESTATUS == "B").ToList();
                    foreach (DOCUMENTOREC dr in dreccc)
                    {
                        if (!objQ)
                            dr.ESTATUS = "P";
                        else
                            dr.ESTATUS_Q = "P";
                        db.Entry(dr).State = EntityState.Modified;
                    }
                }
                else
                {
                    if (!objQ)
                        drecc.ESTATUS = "B";
                    else
                        drecc.ESTATUS_Q = "B";
                }
            }
            ////if ((dOCpADRE.TIPO_RECURRENTE) != "1" && dOCpADRE.DOCUMENTORECs.Count > 1)
            ////{
            ////    drecc.FECHAF = drecc.FECHAF.Value.AddDays(1);
            ////    drecc.ESTATUS = "E";
            ////}
            db.Entry(drecc).State = EntityState.Modified;

            ////if (dOCpADRE.TIPO_RECURRENTE == "2" || dOCpADRE.TIPO_RECURRENTE == "3")
            ////{
            ////    DOCUMENTOL dl = new DOCUMENTOL();
            ////    dl.ESTATUS = null;
            ////    dl.FECHAF = drecc.FECHAF.Value.AddDays(4);
            ////    if (dOCUMENTO.PORC_APOYO > 0)
            ////        dl.MONTO_VENTA = (dOCUMENTO.MONTO_DOC_MD / dOCUMENTO.PORC_APOYO) * 100;
            ////    dl.NUM_DOC = dOCUMENTO.NUM_DOC;
            ////    dl.POS = 1;
            ////    dOCUMENTO.DOCUMENTOLs.Add(dl);
            ////}

            db.SaveChanges();//22.12.2018

            decimal total = 0;

            ProcesaFlujo pf = new ProcesaFlujo();

            USUARIO user = db.USUARIOs.Where(a => a.ID.Equals(d.USUARIOC_ID)).FirstOrDefault();
            try
            {
                WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                if (wf != null)
                {
                    ////WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                    ////FLUJO f = new FLUJO();
                    ////f.WORKF_ID = wf.ID;
                    ////f.WF_VERSION = wf.VERSION;
                    ////f.WF_POS = wp.POS;
                    ////f.NUM_DOC = dOCUMENTO.NUM_DOC;
                    ////f.POS = 1;
                    ////f.LOOP = 1;
                    ////f.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                    ////f.USUARIOD_ID = dOCUMENTO.USUARIOC_ID;
                    ////f.ESTATUS = "I";
                    ////f.FECHAC = DateTime.Now;
                    ////f.FECHAM = DateTime.Now;
                    ////string c = pf.procesa(f, recurrente);

                    ////FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    ////Estatus es = new Estatus();//RSG 18.09.2018
                    ////DOCUMENTO doc = db.DOCUMENTOes.Find(f.NUM_DOC);
                    ////conta.STATUS = es.getEstatus(doc);
                    ////db.Entry(conta).State = EntityState.Modified;
                    ////db.SaveChanges();

                    FLUJO conta = db.FLUJOes.Where(a => a.NUM_DOC.Equals(dOCUMENTO.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
                    Estatus es = new Estatus();//RSG 18.09.2018
                    ////conta.USUARIOA_ID = user.ID;
                    conta.ESTATUS = "A";
                    conta.FECHAM = DateTime.Now;
                    string cero = "";
                    if (dOCUMENTO.MONTO_DOC_MD == 0)
                    {
                        cero = "0";
                    }
                    pf.procesa(conta, cero);
                    //RSG 28.05.2018 -----------------------------------
                    conta = db.FLUJOes.Where(x => x.NUM_DOC == dOCUMENTO.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    DOCUMENTO doc = db.DOCUMENTOes.Find(dOCUMENTO.NUM_DOC);
                    conta.STATUS = es.getEstatus(doc);
                    db.Entry(conta).State = EntityState.Modified;
                    if (dOCUMENTO.MONTO_DOC_MD == 0)
                    {
                        doc.ESTATUS_C = "C";
                        db.Entry(doc).State = EntityState.Modified;
                    }
                    db.SaveChanges();//22.12.2018

                }
            }
            catch (Exception ee)
            {
                if (errorString == "")
                {
                    errorString = ee.Message.ToString();
                    return 0;
                }
                //ViewBag.error = errorString;
            }

            return ret;
        }


        public CLIENTE getCliente(string PAYER_ID)
        {
            ////CLIENTE payer = new CLIENTE();

            using (TruequeEntities db = new TruequeEntities())
            {

                return db.CLIENTEs.Where(c => c.KUNNR.Equals(PAYER_ID)).FirstOrDefault();

            }
            ////return payer;

        }
        public List<DOCUMENTOM_MOD> grupoMaterialesController(List<string> catstabla, string vkorg, string spart, string kunnr, string soc_id, out decimal total, DateTime fechaActual, string tipo)
        {
            TruequeEntities db = new TruequeEntities();
            if (kunnr == null)
            {
                kunnr = "";
            }

            List<DOCUMENTOM_MOD> jd = new List<DOCUMENTOM_MOD>();

            //Obtener los materiales
            IEnumerable<MATERIAL> matl;
            try
            {
                matl = db.MATERIALs.Where(m => m.ACTIVO == true);////.Select(m => m.ID).ToList();
            }
            catch (Exception)
            {
                matl = null;
            }

            //Validar si hay materiales
            if (matl != null)
            {

                CLIENTE cli;
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
                            ////cli.VKORG = cli.VKORG+" ";
                            clil.Add(cli);
                        }
                    }
                }
                catch (Exception)
                {
                    total = 0;
                    return new List<DOCUMENTOM_MOD>();
                }

                ////var cie = clil.Cast<CLIENTE>();
                CONFDIST_CAT conf = getCatConf(soc_id);
                if (conf != null)
                {
                    ////    IEnumerable<CLIENTE> cie = clil as IEnumerable<CLIENTE>;
                    //Obtener el numero de periodos para obtener el historial
                    int nummonths = 0;
                    if (conf.PERIODOS != null)
                        nummonths = (int)conf.PERIODOS;
                    if (tipo != "1")
                        nummonths = 0;
                    int imonths = nummonths * -1;
                    //Obtener el rango de los periodos incluyendo el año
                    ////DateTime ff = DateTime.Today;
                    if (tipo != "1")
                        fechaActual = fechaActual.AddDays(-7);

                    DateTime ff = fechaActual;
                    DateTime fi = ff.AddMonths(imonths);

                    ////string mi = fi.Month.ToString();
                    string ai = fi.Year.ToString();

                    ////string mf = ff.Month.ToString();
                    string af = ff.Year.ToString();
                    Calendario445 cal = new Calendario445();//RSG 09.07.2018

                    int aii;
                    try
                    {
                        aii = Convert.ToInt32(ai);
                    }
                    catch (Exception)
                    {
                        aii = 0;
                    }

                    int mii = 0;
                    try
                    {
                        ////mii = Convert.ToInt32(mi);
                        mii = cal.getPeriodo(fi);//RSG 09.07.2018
                    }
                    catch (Exception)
                    {
                        mii = 0;
                    }

                    int aff = 0;
                    try
                    {
                        aff = Convert.ToInt32(af);
                    }
                    catch (Exception)
                    {
                        aff = 0;
                    }

                    int mff = 0;
                    try
                    {
                        ////mff = Convert.ToInt32(mf);
                        mff = cal.getPeriodo(ff);//RSG 09.07.2018
                    }
                    catch (Exception)
                    {
                        mff = 0;
                    }

                    if (clil != null)
                    {
                        //////Obtener el historial de compras de los clientesd
                        ////var matt = matl.ToList();
                        //////kunnr = kunnr.TrimStart('0').Trim();
                        ////var pres = db.PRESUPSAPPs.Where(a => a.VKORG.Equals(vkorg) && a.SPART.Equals(spart) && a.KUNNR == kunnr && (a.GRSLS != null || a.NETLB != null)).ToList();
                        //////var cat = db.MATERIALGPTs.Where(a => a.SPRAS_ID.Equals(spras)).ToList();//RSG 09.07.2018 
                        //////var cat = db.MATERIALGPs.Where(a => a.ACTIVO == true).ToList();
                        //////foreach (var c in cie)
                        //////{
                        //////    c.KUNNR = c.KUNNR.TrimStart('0').Trim();
                        //////}

                        ////if (conf.CAMPO == "GRSLS")
                        ////{
                        ////    jd = (from ps in pres
                        ////          join cl in cie
                        ////          on ps.KUNNR equals cl.KUNNR
                        ////          join m in matt
                        ////          on ps.MATNR equals m.ID
                        ////          join mk in catstabla
                        ////          //on m.MATERIALGP_ID equals mk.MATERIALGP_ID
                        ////          on m.ID equals mk//RSG 09.07.2018 
                        ////          where (ps.ANIO >= aii && ps.PERIOD >= mii) && (ps.ANIO <= aff && ps.PERIOD <= mff) &&
                        ////          (ps.VKORG == cl.VKORG && ps.VTWEG == cl.VTWEG && ps.SPART == cl.SPART
                        ////          ) && ps.BUKRS == soc_id
                        ////          && ps.GRSLS > 0
                        ////          select new DOCUMENTOM_MOD
                        ////          {
                        ////              ID_CAT = m.MATERIALGP_ID,
                        ////              MATNR = ps.MATNR,
                        ////              //mk.TXT50
                        ////              VAL = Convert.ToDecimal(ps.GRSLS),
                        ////          }).ToList();
                        ////}
                        ////else
                        ////{
                        ////    jd = (from ps in pres
                        ////          join cl in cie
                        ////          on ps.KUNNR equals cl.KUNNR
                        ////          join m in matt
                        ////          on ps.MATNR equals m.ID
                        ////          join mk in catstabla
                        ////          //on m.MATERIALGP_ID equals mk.MATERIALGP_ID
                        ////          on m.ID equals mk//RSG 09.07.2018 
                        ////          where (ps.ANIO >= aii && ps.PERIOD >= mii) && (ps.ANIO <= aff && ps.PERIOD <= mff) &&
                        ////          (ps.VKORG == cl.VKORG && ps.VTWEG == cl.VTWEG && ps.SPART == cl.SPART
                        ////          ) && ps.BUKRS == soc_id
                        ////          && ps.NETLB > 0
                        ////          select new DOCUMENTOM_MOD
                        ////          {
                        ////              ID_CAT = m.MATERIALGP_ID,
                        ////              MATNR = ps.MATNR,
                        ////              //mk.TXT50
                        ////              VAL = Convert.ToDecimal(ps.NETLB)
                        ////          }).ToList();
                        ////}

                        jd = materialesgptDao.ListaMaterialGroupsMateriales(vkorg, spart, kunnr, soc_id, aii, mii, aff, mff, "admin");
                    }
                }

            }
            //Obtener las categorías
            var categoriasl = jd.GroupBy(c => c.MATNR, c => new { ID = c.MATNR.ToString() }).ToList();
            List<string> categorias = new List<string>();
            //Diferencia del método de la vista jquery
            //Tomar en cuenta nada más las categorías que se agregaron a la tabla y que se enviaron en el submmit 
            for (int h = 0; h < catstabla.Count; h++)
            {
                for (int j = 0; j < categoriasl.Count; j++)
                {
                    if (catstabla[h].ToString() == categoriasl[j].Key.ToString() || catstabla[h].ToString() == "000")
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
                cm.EXCLUIR = jd.FirstOrDefault(x => x.MATNR.Equals(item)).EXCLUIR;//RSG 09.07.2018 ID 156

                //Obtener los materiales de la categoría
                List<DOCUMENTOM_MOD> dl;
                List<DOCUMENTOM_MOD> dm = new List<DOCUMENTOM_MOD>();
                //dl = jd.Where(c => c.ID_CAT == item).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL}).ToList();//Falta obtener el groupby
                dl = jd.Where(c => c.MATNR == item).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, EXCLUIR = c.EXCLUIR }).ToList();//RSG 09.07.2018 ID 156

                //Obtener la descripción de los materiales
                foreach (DOCUMENTOM_MOD d in dl)
                {
                    DOCUMENTOM_MOD dcl;
                    //dcl = dm.Where(z => z.MATNR == d.MATNR).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL }).FirstOrDefault();//RSG 09.07.2018 ID 156
                    dcl = dm.Where(z => z.MATNR == d.MATNR).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, EXCLUIR = c.EXCLUIR }).FirstOrDefault();//RSG 09.07.2018 ID 156

                    if (dcl == null)
                    {
                        DOCUMENTOM_MOD dcll = new DOCUMENTOM_MOD();
                        //No se ha agregado
                        decimal val = dl.Where(y => y.MATNR == d.MATNR).Sum(x => x.VAL);
                        dcll.ID_CAT = "000";
                        dcll.MATNR = d.MATNR;

                        //Obtener la descripción del material
                        dcll.DESC = db.MATERIALs.Where(w => w.ID == d.MATNR).FirstOrDefault().MAKTG.ToString();
                        dcll.VAL = val;
                        t += val;
                        dm.Add(dcll);
                    }
                }

                cm.MATERIALES = dm;
                lcatmat.Add(cm);
            }

            total = t;
            List<DOCUMENTOM_MOD> ret = new List<DOCUMENTOM_MOD>();
            foreach (CategoriaMaterial cc in lcatmat)
            {
                ret.AddRange(cc.MATERIALES);
            }
            return ret;
        }

        public List<CategoriaMaterial> grupoCategoriasController(List<string> catstabla, string vkorg, string spart, string kunnr, string soc_id, out decimal total, DateTime fechaActual, string tipo)
        {
            TruequeEntities db = new TruequeEntities();
            if (kunnr == null)
            {
                kunnr = "";
            }

            List<DOCUMENTOM_MOD> jd = new List<DOCUMENTOM_MOD>();

            //Obtener los materiales
            IEnumerable<MATERIAL> matl;//// = Enumerable.Empty<MATERIAL>();
            try
            {
                matl = db.MATERIALs.Where(m => m.ACTIVO == true);////.Select(m => m.ID).ToList();
            }
            catch (Exception)
            {
                total = 0;
                return new List<CategoriaMaterial>();
            }

            //Validar si hay materiales
            if (matl != null)
            {

                CLIENTE cli;
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
                            ////cli.VKORG = cli.VKORG+" ";
                            clil.Add(cli);
                        }
                    }
                }
                catch (Exception)
                {
                    total = 0;
                    return new List<CategoriaMaterial>();
                }

                ////var cie = clil.Cast<CLIENTE>();
                CONFDIST_CAT conf = getCatConf(soc_id);
                if (conf != null)
                {
                    ////    IEnumerable<CLIENTE> cie = clil as IEnumerable<CLIENTE>;
                    //Obtener el numero de periodos para obtener el historial
                    int nummonths = 0;
                    ////////////////////////////////////if (conf.PERIODOS != null)
                    ////////////////////////////////////    nummonths = (int)conf.PERIODOS;
                    ////TEST 03.12.2018 if (tipo != "1")
                    nummonths = 0;
                    int imonths = nummonths * -1;
                    //Obtener el rango de los periodos incluyendo el año
                    ////DateTime ff = DateTime.Today;
                    ////TEST 03.12.2018 if (tipo != "1")//
                    fechaActual = fechaActual.AddDays(-7);

                    DateTime ff = fechaActual;
                    DateTime fi = ff.AddMonths(imonths);

                    ////string mi = fi.Month.ToString();
                    string ai = fi.Year.ToString();

                    ////string mf = ff.Month.ToString();
                    string af = ff.Year.ToString();
                    Calendario445 cal = new Calendario445();//RSG 09.07.2018

                    int aii = 0;
                    try
                    {
                        aii = Convert.ToInt32(ai);
                    }
                    catch (Exception)
                    {
                        aii = 0;
                    }

                    int mii = 0;
                    try
                    {
                        ////mii = Convert.ToInt32(mi);
                        mii = cal.getPeriodo(fi);//RSG 09.07.2018
                    }
                    catch (Exception)
                    {
                        mii = 0;
                    }

                    int aff = 0;
                    try
                    {
                        aff = Convert.ToInt32(af);
                    }
                    catch (Exception)
                    {
                        aff = 0;
                    }

                    int mff = 0;
                    try
                    {
                        ////mff = Convert.ToInt32(mf);
                        mff = cal.getPeriodo(ff);//RSG 09.07.2018
                    }
                    catch (Exception)
                    {
                        mff = 0;
                    }

                    ////if (cie != null)
                    if (clil != null)
                    {
                        //Obtener el historial de compras de los clientesd
                        ////    var matt = matl.ToList();
                        ////    //kunnr = kunnr.TrimStart('0').Trim();
                        ////    var pres = db.PRESUPSAPPs.Where(a => a.VKORG.Equals(vkorg) && a.SPART.Equals(spart) && a.KUNNR == kunnr && (a.GRSLS != null || a.NETLB != null)).ToList();
                        ////    //var cat = db.MATERIALGPTs.Where(a => a.SPRAS_ID.Equals(spras)).ToList();//RSG 09.07.2018 
                        ////    var cat = db.MATERIALGPs.Where(a => a.ACTIVO == true).ToList();
                        ////    //foreach (var c in cie)
                        ////    //{
                        ////    //    c.KUNNR = c.KUNNR.TrimStart('0').Trim();
                        ////    //}

                        ////    if (conf.CAMPO == "GRSLS")
                        ////    {
                        ////        jd = (from ps in pres
                        ////              join cl in cie
                        ////              on ps.KUNNR equals cl.KUNNR
                        ////              join m in matt
                        ////              on ps.MATNR equals m.ID
                        ////              join mk in cat
                        ////              //on m.MATERIALGP_ID equals mk.MATERIALGP_ID
                        ////              on m.MATERIALGP_ID equals mk.ID//RSG 09.07.2018 
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
                        ////                  EXCLUIR = mk.EXCLUIR // RSG 09.07.2018 ID 156
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
                        ////              //on m.MATERIALGP_ID equals mk.MATERIALGP_ID
                        ////              on m.MATERIALGP_ID equals mk.ID//RSG 09.07.2018 
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
                        ////                  EXCLUIR = mk.EXCLUIR // RSG 09.07.2018 ID 156
                        ////              }).ToList();
                        ////    }
                        ////}

                        jd = materialesgptDao.ListaMaterialGroupsMateriales(vkorg, spart, kunnr, soc_id, aii, mii, aff, mff, "admin");
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
                    if (catstabla[h].ToString() == categoriasl[j].Key.ToString() || catstabla[h].ToString() == "000")
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
                cm.EXCLUIR = jd.FirstOrDefault(x => x.ID_CAT.Equals(item)).EXCLUIR;//RSG 09.07.2018 ID 156

                //Obtener los materiales de la categoría
                List<DOCUMENTOM_MOD> dl;
                List<DOCUMENTOM_MOD> dm = new List<DOCUMENTOM_MOD>();
                //dl = jd.Where(c => c.ID_CAT == item).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL}).ToList();//Falta obtener el groupby
                dl = jd.Where(c => c.ID_CAT == item).Select(c => new DOCUMENTOM_MOD { ID_CAT = c.ID_CAT, MATNR = c.MATNR, VAL = c.VAL, EXCLUIR = c.EXCLUIR }).ToList();//RSG 09.07.2018 ID 156

                if (!(catstabla[0] == "000" && cm.EXCLUIR))
                {
                    //Obtener la descripción de los materiales
                    foreach (DOCUMENTOM_MOD d in dl)
                    {
                        DOCUMENTOM_MOD dcl;
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
                            dm.Add(dcll);
                        }
                    }
                }

                cm.MATERIALES = dm;
                lcatmat.Add(cm);
            }

            total = t;

            return lcatmat;
        }

        public CONFDIST_CAT getCatConf(string soc)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                CONFDIST_CAT conf = new CONFDIST_CAT();

                try
                {
                    conf = db.CONFDIST_CAT.Where(c => c.SOCIEDAD_ID == soc && c.ACTIVO == true).FirstOrDefault();
                }
                catch (Exception)
                {
                    return conf;
                }

                return conf;
            }
        }


        public List<DOCUMENTOM> addCatItems(List<CategoriaMaterial> categorias, string kunnr, string catid,
            string soc_id, decimal numdoc, int posid, DateTime? vig_de, DateTime? vig_a, string neg, string dis, decimal total, decimal totaldoc, string col)
        {
            List<DOCUMENTOM> jdlret = new List<DOCUMENTOM>();

            //Negaciación por monto


            //Obtener de la lista de categorias los materiales de la categoría del item
            List<CategoriaMaterial> ccategor;//// = new List<CategoriaMaterial>();
            if (catid == "000")//ccategor = categorias.ToList();//RSG 09.07.2018 ID 156
                ccategor = categorias.Where(a => !a.EXCLUIR).ToList();//RSG 09.07.2018 ID 156
            else
                ccategor = categorias.Where(c => c.ID == catid).ToList();
            foreach (CategoriaMaterial categor in ccategor)
            {
                ////CategoriaMaterial categor = categorias.Where(c => c.ID == catid).FirstOrDefault();
                List<DOCUMENTOM_MOD> materiales;//// = new List<DOCUMENTOM_MOD>();
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
                        ////dm.APOYO_EST = docm.VAL;
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


        public List<DOCUMENTOM> addMatItems(List<DOCUMENTOM_MOD> categorias, string kunnr, string catid,
            string soc_id, decimal numdoc, int posid, DateTime? vig_de, DateTime? vig_a, string neg, string dis, decimal total, decimal totaldoc, string col)
        {
            List<DOCUMENTOM> jdlret = new List<DOCUMENTOM>();

            //Negaciación por monto



            foreach (DOCUMENTOM_MOD docm in categorias)
            {

                DOCUMENTOM dm = new DOCUMENTOM();
                dm.NUM_DOC = numdoc;
                dm.POS_ID = posid;
                dm.MATNR = docm.MATNR;
                dm.VIGENCIA_DE = vig_de;
                dm.VIGENCIA_A = vig_a;
                if (dis == "C")
                {
                    ////dm.APOYO_EST = docm.VAL;
                    dm.VALORH = docm.VAL; //MGC B20180611 Se agregó el valor del material del historico de la provisión
                }
                jdlret.Add(dm);
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

    }
}