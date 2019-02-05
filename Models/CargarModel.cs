using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using TAT001.Entities;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.MappingAPI;
using ClosedXML.Excel;

namespace TAT001.Models
{

    public class CargarModel
    {
        private TruequeEntities db = new TruequeEntities();

        public List<PRESUPUESTOP> cargarPresupuestoCPT(HttpPostedFileBase file, string[] sociedad, string[] periodo, string[] anio, ref string mensaje, string idioma, int pagina)
        {
            TruequeEntities db = new TruequeEntities();
            List<PRESUPUESTOP> pRESUPUESTOPS = new List<PRESUPUESTOP>();
            PRESUPUESTOP pRESUPUESTOP = new PRESUPUESTOP();
            List<string[]> datosPresu = new List<string[]>();
            StreamReader strem = new StreamReader(file.InputStream);
            string[] lines;
            bool prilinea = true, primero = true;
            string material = "";
            int i = 1, ide = 0;
            List<REGION> sociedades = db.REGIONs.Where(x => sociedad.Contains(x.SOCIEDAD)).ToList();
            try
            {
                while (strem.Peek() > -1)
                {
                    if (prilinea)
                    {
                        prilinea = false;
                        lines = strem.ReadLine().Insert(4, ",").Replace("\"", "").Split(',');
                    }
                    else
                    {
                        lines = strem.ReadLine().Insert(4, ",").Replace("\"", "").Split(',');
                        if (primero)
                        {
                            material = lines[5].Trim();
                            pRESUPUESTOP.ANIO = lines[1].Trim();
                            pRESUPUESTOP.POS = i;
                            pRESUPUESTOP.MES = mes(lines[0].Trim());
                            pRESUPUESTOP.VERSION = lines[2].Trim();
                            if (lines[3].Length == 12)
                            {
                                pRESUPUESTOP.PAIS = lines[3].Substring(0, 6).Trim();
                                pRESUPUESTOP.REGION = lines[3].Substring(6, 6).Trim();
                            }
                            else
                            {
                                pRESUPUESTOP.REGION = lines[3].Substring(0, 6).Trim();
                            }
                            pRESUPUESTOP.MONEDA = lines[4].Trim();
                            pRESUPUESTOP.MATERIAL = lines[5].Trim();
                            pRESUPUESTOP.BANNER = lines[6].Trim();
                            primero = false;
                        }
                        if (material != lines[5])
                        {
                            material = lines[5].Trim();
                            if (filtrocarga(pRESUPUESTOP.REGION, pRESUPUESTOP.ANIO, pRESUPUESTOP.MES, sociedades, periodo, anio, true))
                            {
                                pRESUPUESTOPS.Add(new PRESUPUESTOP
                                {
                                    ID = pRESUPUESTOP.ID,
                                    ANIO = pRESUPUESTOP.ANIO,
                                    POS = pRESUPUESTOP.POS,
                                    MES = pRESUPUESTOP.MES,
                                    VERSION = pRESUPUESTOP.VERSION,
                                    PAIS = pRESUPUESTOP.PAIS,
                                    REGION = pRESUPUESTOP.REGION,
                                    MONEDA = pRESUPUESTOP.MONEDA,
                                    MATERIAL = pRESUPUESTOP.MATERIAL,
                                    BANNER = pRESUPUESTOP.BANNER.PadLeft(10, '0'),
                                    NETLB = pRESUPUESTOP.NETLB,
                                    TOTCS = pRESUPUESTOP.TOTCS,
                                    ADVER = pRESUPUESTOP.ADVER,
                                    DIRLB = pRESUPUESTOP.DIRLB,
                                    OVHDF = pRESUPUESTOP.OVHDF,
                                    OVHDV = pRESUPUESTOP.OVHDV,
                                    PKGMT = pRESUPUESTOP.PKGMT,
                                    RAWMT = pRESUPUESTOP.RAWMT,
                                    CONPR = pRESUPUESTOP.CONPR,
                                    POP = pRESUPUESTOP.POP,
                                    DSTRB = pRESUPUESTOP.DSTRB,
                                    GRSLS = pRESUPUESTOP.GRSLS,
                                    CSHDC = pRESUPUESTOP.CSHDC,
                                    FREEG = pRESUPUESTOP.FREEG,
                                    PMVAR = pRESUPUESTOP.PMVAR,
                                    PURCH = pRESUPUESTOP.PURCH,
                                    RECUN = pRESUPUESTOP.RECUN,
                                    RSRDV = pRESUPUESTOP.RSRDV,
                                    VVX17 = pRESUPUESTOP.VVX17,
                                    OTHTA = pRESUPUESTOP.OTHTA,
                                    CORPM = pRESUPUESTOP.CORPM,
                                    SPA = pRESUPUESTOP.SPA
                                });
                            }
                            pRESUPUESTOP = new PRESUPUESTOP();
                            i++;
                            pRESUPUESTOP.ANIO = lines[1].Trim();
                            pRESUPUESTOP.POS = i;
                            pRESUPUESTOP.MES = mes(lines[0].Trim());
                            pRESUPUESTOP.VERSION = lines[2].Trim();
                            if (lines[3].Length == 12)
                            {
                                pRESUPUESTOP.PAIS = lines[3].Substring(0, 6).Trim();
                                pRESUPUESTOP.REGION = lines[3].Substring(6, 6).Trim();
                            }
                            else
                            {
                                pRESUPUESTOP.REGION = lines[3].Substring(0, 6).Trim();
                            }
                            pRESUPUESTOP.MONEDA = lines[4].Trim();
                            pRESUPUESTOP.MATERIAL = lines[5].Trim();
                            pRESUPUESTOP.BANNER = lines[6].Trim();
                            decide(lines[7].Trim(), ref pRESUPUESTOP, Convert.ToDouble(lines[8].Trim()));
                        }
                        else
                        {
                            decide(lines[7].Trim(), ref pRESUPUESTOP, Convert.ToDouble(lines[8].Trim()));
                        }
                    }
                }
                if (filtrocarga(pRESUPUESTOP.REGION, pRESUPUESTOP.ANIO, pRESUPUESTOP.MES, sociedades, periodo, anio, true))
                {
                    pRESUPUESTOPS.Add(new PRESUPUESTOP
                    {
                        ID = pRESUPUESTOP.ID,
                        ANIO = pRESUPUESTOP.ANIO,
                        POS = pRESUPUESTOP.POS,
                        MES = pRESUPUESTOP.MES,
                        VERSION = pRESUPUESTOP.VERSION,
                        PAIS = pRESUPUESTOP.PAIS,
                        REGION = pRESUPUESTOP.REGION,
                        MONEDA = pRESUPUESTOP.MONEDA,
                        MATERIAL = pRESUPUESTOP.MATERIAL,
                        BANNER = pRESUPUESTOP.BANNER.PadLeft(10, '0'),
                        NETLB = pRESUPUESTOP.NETLB,
                        TOTCS = pRESUPUESTOP.TOTCS,
                        ADVER = pRESUPUESTOP.ADVER,
                        DIRLB = pRESUPUESTOP.DIRLB,
                        OVHDF = pRESUPUESTOP.OVHDF,
                        OVHDV = pRESUPUESTOP.OVHDV,
                        PKGMT = pRESUPUESTOP.PKGMT,
                        RAWMT = pRESUPUESTOP.RAWMT,
                        CONPR = pRESUPUESTOP.CONPR,
                        POP = pRESUPUESTOP.POP,
                        DSTRB = pRESUPUESTOP.DSTRB,
                        GRSLS = pRESUPUESTOP.GRSLS,
                        CSHDC = pRESUPUESTOP.CSHDC,
                        FREEG = pRESUPUESTOP.FREEG,
                        PMVAR = pRESUPUESTOP.PMVAR,
                        PURCH = pRESUPUESTOP.PURCH,
                        RECUN = pRESUPUESTOP.RECUN,
                        RSRDV = pRESUPUESTOP.RSRDV,
                        VVX17 = pRESUPUESTOP.VVX17,
                        OTHTA = pRESUPUESTOP.OTHTA,
                        CORPM = pRESUPUESTOP.CORPM,
                        SPA = pRESUPUESTOP.SPA
                    });
                }
                pRESUPUESTOP = new PRESUPUESTOP();
                if (pRESUPUESTOPS.Count == 0)
                {
                    mensaje = mensajes(5, idioma, pagina);//"No se encontraron datos en el archivo CPT de acuerdo al filtro de datos";
                }
            }
            catch (Exception e)
            {
                mensaje = mensajes(7, idioma, pagina);//"Formato de archivo para carga CPT incorrecto";
            }
            return pRESUPUESTOPS;
        }
        public List<PRESUPSAPP> cargarPresupuestoSAP(HttpPostedFileBase[] file, string[] sociedad, string[] periodo, string[] anio, ref string mensaje, string idioma, int pagina)
        {
            TruequeEntities db = new TruequeEntities();
            List<PRESUPSAPP> pRESUPUESTOPS = new List<PRESUPSAPP>();
            PRESUPSAPP pRESUPUESTOP = new PRESUPSAPP();
            List<string[]> datosPresu = new List<string[]>();
            StreamReader strem;
            string soc2 = sociedad[0];
            List<REGION> sociedades = db.REGIONs.Where(x => x.SOCIEDAD == soc2).ToList();
            string[] lines;
            bool prilinea = false;
            int i = 1;
            try
            {
                for (int j = 0; j < file.Length; j++)
                {
                    strem = new StreamReader(file[j].InputStream);
                    while (strem.Peek() > -1)
                    {
                        if (prilinea)
                        {
                            prilinea = false;
                            lines = strem.ReadLine().Split('|');
                        }
                        else
                        {
                            lines = strem.ReadLine().Split('|');

                            pRESUPUESTOP.ANIO = Convert.ToInt32(lines[0]);
                            pRESUPUESTOP.POS = i;
                            pRESUPUESTOP.PERIOD = Convert.ToInt32(lines[1]);
                            pRESUPUESTOP.TYPE = lines[2];
                            pRESUPUESTOP.BUKRS = lines[3];
                            pRESUPUESTOP.VKORG = lines[4];
                            pRESUPUESTOP.VTWEG = lines[5];
                            pRESUPUESTOP.SPART = lines[6];
                            pRESUPUESTOP.VKBUR = lines[7];
                            pRESUPUESTOP.VKGRP = lines[8];
                            pRESUPUESTOP.BZIRK = lines[9];
                            pRESUPUESTOP.MATNR = lines[10];
                            pRESUPUESTOP.PRDHA = lines[11];
                            pRESUPUESTOP.KUNNR = lines[12];
                            pRESUPUESTOP.KUNNR_P = lines[14];
                            pRESUPUESTOP.BANNER = lines[16];
                            pRESUPUESTOP.BANNER_CALC = lines[17];
                            pRESUPUESTOP.KUNNR_PAY = lines[18];
                            pRESUPUESTOP.FECHAP = lines[20].Substring(4, 2) + "-" + lines[20].Substring(6, 2) + "-" + lines[20].Substring(0, 4);
                            pRESUPUESTOP.UNAME = lines[21];
                            pRESUPUESTOP.XBLNR = lines[22];

                            //pRESUPUESTOP.RECSL = Convert.ToDecimal(lines[24]);
                            //pRESUPUESTOP.INDLB = Convert.ToDecimal(lines[25]);
                            //pRESUPUESTOP.FRGHT = Convert.ToDecimal(lines[26]);
                            //pRESUPUESTOP.PURCH = Convert.ToDecimal(lines[27]);
                            //pRESUPUESTOP.RAWMT = Convert.ToDecimal(lines[28]);
                            //pRESUPUESTOP.PKGMT = Convert.ToDecimal(lines[29]);
                            //pRESUPUESTOP.OVHDV = Convert.ToDecimal(lines[30]);
                            //pRESUPUESTOP.OVHDF = Convert.ToDecimal(lines[31]);
                            //pRESUPUESTOP.DIRLB = Convert.ToDecimal(lines[32]);
                            pRESUPUESTOP.VVX17 = Convert.ToDecimal(lines[23]);
                            pRESUPUESTOP.CSHDC = Convert.ToDecimal(lines[24]);
                            pRESUPUESTOP.RECUN = Convert.ToDecimal(lines[25]);
                            pRESUPUESTOP.DSTRB = Convert.ToDecimal(lines[26]);
                            pRESUPUESTOP.OTHTA = Convert.ToDecimal(lines[27]);
                            pRESUPUESTOP.ADVER = Convert.ToDecimal(lines[28]);
                            pRESUPUESTOP.CORPM = Convert.ToDecimal(lines[29]);
                            pRESUPUESTOP.POP = Convert.ToDecimal(lines[30]);
                            pRESUPUESTOP.PMVAR = Convert.ToDecimal(lines[31]);
                            pRESUPUESTOP.CONPR = Convert.ToDecimal(lines[32]);
                            pRESUPUESTOP.RSRDV = Convert.ToDecimal(lines[33]);
                            pRESUPUESTOP.SPA = Convert.ToDecimal(lines[34]);
                            pRESUPUESTOP.FREEG = Convert.ToDecimal(lines[35]);
                            pRESUPUESTOP.GRSLS = Convert.ToDecimal(lines[36]);
                            //pRESUPUESTOP.PKGDS = Convert.ToDecimal(lines[38]);
                            pRESUPUESTOP.NETLB = Convert.ToDecimal(lines[37]);
                            //pRESUPUESTOP.SLLBS = Convert.ToDecimal(lines[46]);
                            //pRESUPUESTOP.SLCAS = Convert.ToDecimal(lines[47]);
                            //pRESUPUESTOP.PRCAS = Convert.ToDecimal(lines[48]);
                            //pRESUPUESTOP.NPCAS = Convert.ToDecimal(lines[49]); 
                            //pRESUPUESTOP.ILVAR = Convert.ToDecimal(lines[51]);
                            //pRESUPUESTOP.BILBK = Convert.ToDecimal(lines[52]);
                            //pRESUPUESTOP.OVHVV = Convert.ToDecimal(lines[53]);
                            //pRESUPUESTOP.OHV = Convert.ToDecimal(lines[50]);
                            if (filtrocarga(pRESUPUESTOP.BUKRS, pRESUPUESTOP.ANIO.ToString(), pRESUPUESTOP.PERIOD.ToString(), sociedades, periodo, anio, false))
                            {
                                pRESUPUESTOPS.Add(new PRESUPSAPP
                                {
                                    ANIO = pRESUPUESTOP.ANIO,
                                    POS = pRESUPUESTOP.POS,
                                    PERIOD = pRESUPUESTOP.PERIOD,
                                    TYPE = pRESUPUESTOP.TYPE,
                                    BUKRS = pRESUPUESTOP.BUKRS,
                                    VKORG = pRESUPUESTOP.VKORG,
                                    VTWEG = pRESUPUESTOP.VTWEG,
                                    SPART = pRESUPUESTOP.SPART,
                                    VKBUR = pRESUPUESTOP.VKBUR,
                                    VKGRP = pRESUPUESTOP.VKGRP,
                                    BZIRK = pRESUPUESTOP.BZIRK,
                                    MATNR = pRESUPUESTOP.MATNR,
                                    PRDHA = pRESUPUESTOP.PRDHA,
                                    KUNNR = pRESUPUESTOP.KUNNR,
                                    KUNNR_P = pRESUPUESTOP.KUNNR_P,
                                    BANNER = pRESUPUESTOP.BANNER,
                                    BANNER_CALC = pRESUPUESTOP.BANNER_CALC,
                                    KUNNR_PAY = pRESUPUESTOP.KUNNR_PAY,
                                    FECHAP = pRESUPUESTOP.FECHAP,
                                    UNAME = pRESUPUESTOP.UNAME,
                                    XBLNR = pRESUPUESTOP.XBLNR,
                                    GRSLS = pRESUPUESTOP.GRSLS,
                                    //RECSL = pRESUPUESTOP.RECSL,
                                    //INDLB = pRESUPUESTOP.INDLB,
                                    //FRGHT = pRESUPUESTOP.FRGHT,
                                    //PURCH = pRESUPUESTOP.PURCH,
                                    //RAWMT = pRESUPUESTOP.RAWMT,
                                    //PKGMT = pRESUPUESTOP.PKGMT,
                                    //OVHDV = pRESUPUESTOP.OVHDV,
                                    //OVHDF = pRESUPUESTOP.OVHDF,
                                    //DIRLB = pRESUPUESTOP.DIRLB,
                                    CSHDC = pRESUPUESTOP.CSHDC,
                                    RECUN = pRESUPUESTOP.RECUN,
                                    OTHTA = pRESUPUESTOP.OTHTA,
                                    SPA = pRESUPUESTOP.SPA,
                                    FREEG = pRESUPUESTOP.FREEG,
                                    //PKGDS = pRESUPUESTOP.PKGDS,
                                    CONPR = pRESUPUESTOP.CONPR,
                                    RSRDV = pRESUPUESTOP.RSRDV,
                                    CORPM = pRESUPUESTOP.CORPM,
                                    POP = pRESUPUESTOP.POP,
                                    PMVAR = pRESUPUESTOP.PMVAR,
                                    ADVER = pRESUPUESTOP.ADVER,
                                    NETLB = pRESUPUESTOP.NETLB,
                                    //SLLBS = pRESUPUESTOP.SLLBS,
                                    //SLCAS = pRESUPUESTOP.SLCAS,
                                    //PRCAS = pRESUPUESTOP.PRCAS,
                                    //NPCAS = pRESUPUESTOP.NPCAS,
                                    DSTRB = pRESUPUESTOP.DSTRB,
                                    //ILVAR = pRESUPUESTOP.ILVAR,
                                    //BILBK = pRESUPUESTOP.BILBK,
                                    //OVHVV = pRESUPUESTOP.OVHVV
                                    VVX17 = pRESUPUESTOP.VVX17,
                                    OHV = pRESUPUESTOP.OHV
                                });
                            }
                            //sw.WriteLine(pRESUPUESTOP.ID + "," + pRESUPUESTOP.ANIO + "," + pRESUPUESTOP.POS + "," + pRESUPUESTOP.MES + "," + pRESUPUESTOP.VERSION + "," + pRESUPUESTOP.PAIS + "," + pRESUPUESTOP.MONEDA + "," + pRESUPUESTOP.MATERIAL + "," + pRESUPUESTOP.BANNER + "," + pRESUPUESTOP.ADVER + "," + pRESUPUESTOP.CONPR + "," + pRESUPUESTOP.CSHDC + "," + pRESUPUESTOP.DIRLB + "," + pRESUPUESTOP.DSTRB + "," + pRESUPUESTOP.FREEG + "," + pRESUPUESTOP.GRSLS + "," + pRESUPUESTOP.NETLB + "," + pRESUPUESTOP.OVHDF + "," + pRESUPUESTOP.OVHDV + "," + pRESUPUESTOP.PKGMT + "," + pRESUPUESTOP.PMVAR + "," + pRESUPUESTOP.POP + "," + pRESUPUESTOP.PURCH + "," + pRESUPUESTOP.RAWMT + "," + pRESUPUESTOP.RECUN + "," + pRESUPUESTOP.RSRDV + "," + pRESUPUESTOP.TOTCS);
                            i++;
                        }
                    }
                }
                if (pRESUPUESTOPS.Count == 0)
                {
                    mensaje = mensajes(8, idioma, pagina);//"No se encontraron datos en el archivo SAP de acuerdo al filtro de datos";
                }
            }
            catch (Exception)
            {
                mensaje = mensajes(9, idioma, pagina); //"Formato de archivo para carga de SAP es incorrecto.";
            }

            return pRESUPUESTOPS;
        }
        public string guardarPresupuesto(ref DatosPresupuesto presupuesto, string[] sociedadcpt, string[] periodocpt, string[] sociedadsap, string[] periodosap, string usuario, string opciong, string idioma, int pagina)
        {
            TruequeEntities db = new TruequeEntities();
            string mensaje = "", soc = "", pre = "";
            int ide = 0;
            string opc = "1";
            string sociedad = "";
            if (opciong != "on")
            {
                opc = "2";
            }
            if (presupuesto.presupuestoCPT.Count > 0)
            {
                soc = ""; pre = "";
                sociedadPeriodo(sociedadcpt, periodocpt, true, ref soc, ref pre);
                var id = db.CSP_PRESUPUESTO_ADD(Convert.ToInt32(presupuesto.presupuestoCPT[0].ANIO), soc, pre, usuario, "0", 1).ToList();
                if (id.Count > 0)
                {
                    ide = Convert.ToInt32(id[0].ToString());
                    if (ide != 0)
                    {
                        for (int i = 0; i < presupuesto.presupuestoCPT.Count; i++)
                        {
                            presupuesto.presupuestoCPT[i].ID = ide;
                        }
                        db.BulkInsert(presupuesto.presupuestoCPT);
                        for (int i = 0; i < sociedadcpt.Length; i++)
                        {
                            sociedad += sociedadcpt[i] + ",";
                        }
                        presupuesto.bannerscanal = db.CSP_BANNERSINCANAL(sociedad).ToList();
                        mensaje = mensajes(15, idioma, pagina); //"Guardado Correctamente CPT.";
                    }
                    else
                    {
                        mensaje = mensajes(10, idioma, pagina); //"El usuario con el que se esta cargando los datos no exitene en el sistema.";
                    }
                }
                else
                {
                    mensaje = mensajes(11, idioma, pagina); //"Ocurrio algo mientra se guardaba.";
                }
            }
            if (presupuesto.presupuestoSAP.Count > 0)
            {
                soc = ""; pre = "";
                sociedadPeriodo(sociedadsap, periodosap, false, ref soc, ref pre);
                var id = db.CSP_PRESUPUESTO_ADD(presupuesto.presupuestoSAP[0].ANIO, soc, pre, usuario, opc, 2).ToList();//0 remplazar 1 añadir
                if (id.Count > 0)
                {
                    ide = Convert.ToInt32(id[0].ToString());
                    if (ide != 0)
                    {
                        for (int i = 0; i < presupuesto.presupuestoSAP.Count; i++)
                        {
                            presupuesto.presupuestoSAP[i].ID = ide;
                        }
                        db.BulkInsert(presupuesto.presupuestoSAP);
                        mensaje = mensajes(12, idioma, pagina); //"Guardado Correctamente SAP.";
                    }
                    else
                    {
                        mensaje = mensajes(10, idioma, pagina); //"El usuario con el que se esta cargando los datos no exitene en el sistema.";
                    }
                }
                else
                {
                    mensaje = mensajes(13, idioma, pagina); //"Ocurrio algo mientra se guardaba.";
                }
            }
            return mensaje;
        }
        private void decide(string concepto, ref PRESUPUESTOP presu, double data)
        {
            switch (concepto)
            {
                case "NETLB":
                    if (presu.NETLB == null)
                    {
                        presu.NETLB = data;
                    }
                    else
                    {
                        presu.NETLB += data;
                    }
                    break;
                case "TOTCS":
                    if (presu.TOTCS == null)
                    {
                        presu.TOTCS = data;
                    }
                    else
                    {
                        presu.TOTCS += data;
                    }
                    break;
                case "ADVER":
                    if (presu.ADVER == null)
                    {
                        presu.ADVER = data;
                    }
                    else
                    {
                        presu.ADVER += data;
                    }
                    break;
                case "DIRLB":
                    if (presu.DIRLB == null)
                    {
                        presu.DIRLB = data;
                    }
                    else
                    {
                        presu.DIRLB += data;
                    }
                    break;
                case "OVHDF":
                    if (presu.OVHDF == null)
                    {
                        presu.OVHDF = data;
                    }
                    else
                    {
                        presu.OVHDF += data;
                    }
                    break;
                case "OVHDV":
                    if (presu.OVHDV == null)
                    {
                        presu.OVHDV = data;
                    }
                    else
                    {
                        presu.OVHDV += data;
                    }
                    break;
                case "PKGMT":
                    if (presu.PKGMT == null)
                    {
                        presu.PKGMT = data;
                    }
                    else
                    {
                        presu.PKGMT += data;
                    }
                    break;
                case "RAWMT":
                    if (presu.RAWMT == null)
                    {
                        presu.RAWMT = data;
                    }
                    else
                    {
                        presu.RAWMT += data;
                    }
                    break;
                case "CONPR":
                    if (presu.CONPR == null)
                    {
                        presu.CONPR = data;
                    }
                    else
                    {
                        presu.CONPR += data;
                    }
                    break;
                case "POP":
                    if (presu.POP == null)
                    {
                        presu.POP = data;
                    }
                    else
                    {
                        presu.POP += data;
                    }
                    break;
                case "DSTRB":
                    if (presu.DSTRB == null)
                    {
                        presu.DSTRB = data;
                    }
                    else
                    {
                        presu.DSTRB += data;
                    }
                    break;
                case "GRSLS":
                    if (presu.GRSLS == null)
                    {
                        presu.GRSLS = data;
                    }
                    else
                    {
                        presu.GRSLS += data;
                    }
                    break;
                case "CSHDC":
                    if (presu.CSHDC == null)
                    {
                        presu.CSHDC = data;
                    }
                    else
                    {
                        presu.CSHDC += data;
                    }
                    break;
                case "FREEG":
                    if (presu.FREEG == null)
                    {
                        presu.FREEG = data;
                    }
                    else
                    {
                        presu.FREEG += data;
                    }
                    break;
                case "PMVAR":
                    if (presu.PMVAR == null)
                    {
                        presu.PMVAR = data;
                    }
                    else
                    {
                        presu.PMVAR += data;
                    }
                    break;
                case "PURCH":
                    if (presu.PURCH == null)
                    {
                        presu.PURCH = data;
                    }
                    else
                    {
                        presu.PURCH += data;
                    }
                    break;
                case "RECUN":
                    if (presu.RECUN == null)
                    {
                        presu.RECUN = data;
                    }
                    else
                    {
                        presu.RECUN += data;
                    }
                    break;
                case "RSRDV":
                    if (presu.RSRDV == null)
                    {
                        presu.RSRDV = data;
                    }
                    else
                    {
                        presu.RSRDV += data;
                    }
                    break;
                case "VVX17":
                    if (presu.VVX17 == null)
                    {
                        presu.VVX17 = data;
                    }
                    else
                    {
                        presu.VVX17 += data;
                    }
                    break;
                case "OTHTA":
                    if (presu.OTHTA == null)
                    {
                        presu.OTHTA = data;
                    }
                    else
                    {
                        presu.OTHTA += data;
                    }
                    break;
                case "CORPM":
                    if (presu.CORPM == null)
                    {
                        presu.CORPM = data;
                    }
                    else
                    {
                        presu.CORPM += data;
                    }
                    break;
                case "SPA":
                    if (presu.SPA == null)
                    {
                        presu.SPA = data;
                    }
                    else
                    {
                        presu.SPA += data;
                    }
                    break;
                default:
                    break;
            }
        }
        private bool filtrocarga(string sociedad, string anio, string periodo, List<REGION> sociedades, string[] periodos, string[] anioss, bool cpt)
        {
            string[] soc;
            string[] pre;
            string anios = "";
            string anios2 = "";
            if (cpt)
            {
                anios = anioss[0].Substring(2, 2);
                if (anioss.Length == 2)
                {
                    anios2 = anioss[0].Substring(2, 2);
                }
            }
            else
            {
                anios = anioss[0];
            }
            if (anio == anios || anio == anios2)
            {
                if (sociedades != null)
                {
                    if (cpt)
                    {
                        soc = sociedades.Where(x => x.REGION1 == sociedad).Select(y => y.REGION1).ToArray();
                    }
                    else
                    {
                        soc = sociedades.Where(x => x.SOCIEDAD == sociedad).Select(y => y.REGION1).ToArray();
                    }
                    if (soc.Length > 0)
                    {
                        if (periodos != null)
                        {
                            //if (cpt)
                            //{
                            //    periodo = mes(periodo);
                            //}
                            pre = periodos.Where(x => x == periodo).ToArray();
                            if (pre.Length > 0)
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
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }
        private string mes(string mes)
        {
            string ms = "";
            switch (mes)
            {
                case "Jan":
                    ms = "1";
                    break;
                case "Feb":
                    ms = "2";
                    break;
                case "Mar":
                    ms = "3";
                    break;
                case "Apr":
                    ms = "4";
                    break;
                case "May":
                    ms = "5";
                    break;
                case "Jun":
                    ms = "6";
                    break;
                case "Jul":
                    ms = "7";
                    break;
                case "Aug":
                    ms = "8";
                    break;
                case "Sep":
                    ms = "9";
                    break;
                case "Oct":
                    ms = "10";
                    break;
                case "Nov":
                    ms = "11";
                    break;
                case "Dec":
                    ms = "12";
                    break;
            }
            return ms;
        }
        private void sociedadPeriodo(string[] sociedades, string[] periodos, bool cpt, ref string sociedad, ref string periodo)
        {
            System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.DateTimeFormatInfo formatoFecha = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
            if (sociedades != null)
            {
                for (int i = 0; i < sociedades.Length; i++)
                {
                    sociedad += sociedades[i] + ",";
                }
            }
            if (cpt)
            {
                if (periodos != null)
                {
                    for (int i = 0; i < periodos.Length; i++)
                    {
                        //periodo += myTI.ToTitleCase(formatoFecha.GetMonthName(int.Parse(periodos[i]))).Substring(0, 3) + ",";
                        periodo += periodos[i] + ",";
                    }
                }
            }
            else
            {
                if (periodos != null)
                {
                    for (int i = 0; i < periodos.Length; i++)
                    {
                        periodo += periodos[i] + ",";
                    }
                }
            }
        }
        public DatosPresupuesto consultSociedad(string user)
        {
            DatosPresupuesto sociedades = new DatosPresupuesto();
            sociedades.sociedad = db.USUARIOs.Where(a => a.ID.Equals(user)).FirstOrDefault().SOCIEDADs.ToList();
            return sociedades;
        }
        public string bannres(string ruta, string[] sociedadcpt, string idioma, int pagina)
        {

            string sociedad = "";
            for (int i = 0; i < sociedadcpt.Length; i++)
            {
                sociedad += sociedadcpt[i] + ",";
            }
            List<CSP_BANNERSINCANAL_Result> bannerscanal = db.CSP_BANNERSINCANAL(sociedad).ToList();
            if (bannerscanal.Count > 0)
            {
                generarExcelBanner(bannerscanal, ruta);
                return "";
            }
            else
            {
                return mensajes(14, idioma, pagina); //"No se pudo obtener los banners sin canal";
            }
        }
        public void generarExcelBanner(List<CSP_BANNERSINCANAL_Result> banners, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            int contador = 2;
            string index;
            try
            {
                worksheet.Cell("A1").Value = new[]
                 {
                  new {
                       BANNER = "Banner",
                      SOCIEDAD = "Sociedad"
                      },
                    };
                foreach (CSP_BANNERSINCANAL_Result row in banners)
                {
                    index = "A";
                    index = index + contador;
                    worksheet.Cell(index).Value = new[]
                 {
                  new {
                      BANNER       = row.BANNER,
                      SOCIEDAD       = row.SOCIEDAD
                      },
                    };
                    contador++;
                }
                worksheet.Range("A1:B1").Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#0B2161")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                for (int i = 1; i < 22; i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }
                workbook.SaveAs(ruta + @"\Banners sin canal.xlsx");
            }
            catch (Exception)
            {

            }
        }
        public string mensajes(int id, string idioma, int pagina)
        {
            return db.MENSAJES.Where(x => x.ID_MENSAJE == id && x.SPRAS == idioma && x.PAGINA_ID == pagina).Select(x => x.DESCRIPCION).SingleOrDefault();
        }
    }

    public class DatosPresupuesto
    {
        public List<PRESUPUESTOP> presupuestoCPT = new List<PRESUPUESTOP>();
        public List<PRESUPSAPP> presupuestoSAP = new List<PRESUPSAPP>();
        public List<SOCIEDAD> sociedad = new List<SOCIEDAD>();
        public List<CSP_CAMBIO_Result> cambio = new List<CSP_CAMBIO_Result>();
        public List<CSP_CONSULTARPRESUPUESTO_Result> presupuesto = new List<CSP_CONSULTARPRESUPUESTO_Result>();
        //public List<string> bannerscanal = new List<string>();
        public List<CSP_BANNERSINCANAL_Result> bannerscanal = new List<CSP_BANNERSINCANAL_Result>();
        //public DatosPresupuesto(List<PRESUPUESTOP> PresupuestoSAP, List<PRESUPSAPP> PresupuestoCPT)
        //{
        //    this.presupuestoSAP = PresupuestoSAP;
        //    this.presupuestoCPT = PresupuestoCPT;            
        //}
    }
}