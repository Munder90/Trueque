using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TAT001.Entities;
using TAT001.Models;

namespace TAT001.Services
{
    public class ProcesaFlujo
    {
        public string procesa(FLUJO f, string recurrente)
        {
            //ADD RSG 01.11.2018----------------------
            bool recurrent = false;
            bool draft = false;
            bool reve = false;
            bool liga = false;
            bool cero = false;
            if (recurrente == "X")
                recurrent = true;
            if (recurrente == "B")
                draft = true;
            if (recurrente == "C" && f.WORKFP.ACCION.TIPO == "E")
                reve = true;
            if (recurrente == "L")
                liga = true;
            if (recurrente == "0")
                cero = true;
            //ADD RSG 01.11.2018----------------------
            string correcto = String.Empty;
            TruequeEntities db = new TruequeEntities();
            FLUJO actual = new FLUJO();
            if (reve)
                f.ESTATUS = "I";
            if (f.ESTATUS.Equals("I"))//---------------------------NUEVO REGISTRO
            {
                if (reve)
                    f.ESTATUS = "A";
                actual.NUM_DOC = f.NUM_DOC;
                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                actual.COMENTARIO = f.COMENTARIO;
                actual.ESTATUS = f.ESTATUS;
                actual.FECHAC = f.FECHAC;
                actual.FECHAM = f.FECHAM;
                actual.LOOP = f.LOOP;
                actual.NUM_DOC = f.NUM_DOC;
                actual.POS = f.POS;

                DET_APROBH dah = db.DET_APROBH.Where(a => a.SOCIEDAD_ID == d.SOCIEDAD_ID && a.PUESTOC_ID == d.PUESTO_ID && a.ACTIVO)
                                    .OrderByDescending(a => a.VERSION).FirstOrDefault();
                if (dah == null)
                    return "0";
                CLIENTEF cf = db.CLIENTEFs.Where(a => a.VKORG.Equals(d.VKORG) && a.VTWEG.Equals(d.VTWEG) && a.SPART.Equals(d.SPART) && a.KUNNR.Equals(d.PAYER_ID) && a.ACTIVO
                               ).OrderByDescending(a => a.VERSION).FirstOrDefault();

                actual.DETPOS = 1;
                actual.DETVER = dah.VERSION;
                actual.USUARIOA_ID = f.USUARIOA_ID;
                actual.USUARIOD_ID = f.USUARIOD_ID;
                actual.WF_POS = f.WF_POS;
                actual.WF_VERSION = f.WF_VERSION;
                actual.WORKF_ID = f.WORKF_ID;
                f.ESTATUS = "A";
                actual.ESTATUS = f.ESTATUS;
                if (!reve)
                    db.FLUJOes.Add(actual);
                else
                {
                    FLUJO ff = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC & x.POS == f.POS).FirstOrDefault();
                    ff.ESTATUS = "A";
                    db.Entry(ff).State = EntityState.Modified;
                }

                if (!draft)//NO ES BORRADOR
                {
                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                    int next_step_a = 0;
                    if (paso_a.NEXT_STEP != null)
                        next_step_a = (int)paso_a.NEXT_STEP;

                    WORKFP next;

                    if (!recurrent && !liga)
                    {
                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();
                    }
                    else
                    {
                        if (d.TIPO_RECURRENTE == "1" || liga || d.TIPO_RECURRENTE == "6" || d.TIPO_RECURRENTE == "7" || d.TIPO_RECURRENTE == "8")
                        {
                            WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.ACCION_ID == 5).FirstOrDefault();
                            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == autoriza.NS_ACCEPT).FirstOrDefault();
                        }
                        else
                        {
                            int acc = 14;
                            WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.ACCION_ID == acc).FirstOrDefault();
                            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == autoriza.POS).FirstOrDefault();
                        }

                    }
                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                    {
                        d.ESTATUS_WF = "A";
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "2";
                    }
                    else
                    {
                        FLUJO nuevo = new FLUJO();
                        nuevo.WORKF_ID = next.ID;
                        nuevo.WF_VERSION = next.VERSION;
                        nuevo.WF_POS = next.POS;
                        nuevo.NUM_DOC = actual.NUM_DOC;
                        nuevo.POS = actual.POS + 1;
                        nuevo.LOOP = 1;

                        if (next.ACCION.TIPO == "E")
                        {
                            nuevo.USUARIOA_ID = null;
                            nuevo.DETPOS = 0;
                            nuevo.DETVER = 0;
                        }
                        else
                        {
                            if (!recurrent)
                            {
                                FLUJO detA = determinaAgenteI(cf, dah);
                                nuevo.USUARIOA_ID = detA.USUARIOA_ID;
                                nuevo.USUARIOD_ID = nuevo.USUARIOA_ID;

                                DateTime fecha = DateTime.Now.Date;
                                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).FirstOrDefault();
                                if (del != null)
                                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                else
                                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                                nuevo.DETPOS = detA.DETPOS;
                                nuevo.DETVER = cf.VERSION;
                            }
                            else
                            {
                                if (d.TIPO_RECURRENTE == "1")
                                {
                                    nuevo.USUARIOA_ID = null;
                                }
                                else
                                {
                                    DOCUMENTOREC docPadre = db.DOCUMENTORECs.Where(x => x.DOC_REF == actual.NUM_DOC).FirstOrDefault();
                                    if(docPadre==null)
                                        docPadre = db.DOCUMENTORECs.Where(x => x.NUM_DOC_Q == actual.NUM_DOC).FirstOrDefault();
                                    FLUJO ff = db.FLUJOes.Where(x => x.NUM_DOC == docPadre.NUM_DOC && x.WORKFP.ACCION_ID == 5).FirstOrDefault();
                                    nuevo.USUARIOA_ID = ff.USUARIOA_ID;
                                    nuevo.USUARIOD_ID = ff.USUARIOD_ID;
                                }
                                nuevo.DETPOS = 0;
                                nuevo.DETVER = 0;
                            }
                        }
                        nuevo.ESTATUS = "P";
                        nuevo.FECHAC = DateTime.Now;
                        nuevo.FECHAM = DateTime.Now;

                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "1";
                        d.ESTATUS_WF = "P";

                        db.FLUJOes.Add(nuevo);
                        db.Entry(d).State = EntityState.Modified;
                    }//ADD RSG 30.10.2018

                    db.SaveChanges();
                }
                else
                {
                    correcto = "2";
                    db.SaveChanges();
                }
            }
            else if (f.ESTATUS.Equals("A"))   //---------------------EN PROCESO DE APROBACIÓN
            {
                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC) && a.POS == f.POS).OrderByDescending(a => a.POS).FirstOrDefault();

                if (actual.POS == 1)//-------------------ES BORRADOR
                {
                    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                    DET_APROBH dah = db.DET_APROBH.Where(a => a.SOCIEDAD_ID == d.SOCIEDAD_ID && a.PUESTOC_ID == d.PUESTO_ID && a.ACTIVO)
                                        .OrderByDescending(a => a.VERSION).FirstOrDefault();
                    if (dah == null)
                        return "0";
                    CLIENTEF cf = db.CLIENTEFs.Where(a => a.VKORG.Equals(d.VKORG) && a.VTWEG.Equals(d.VTWEG) && a.SPART.Equals(d.SPART) && a.KUNNR.Equals(d.PAYER_ID) && a.ACTIVO
                                   ).OrderByDescending(a => a.VERSION).FirstOrDefault();

                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                    int next_step_a = 0;
                    if (paso_a.NEXT_STEP != null)
                        next_step_a = (int)paso_a.NEXT_STEP;

                    WORKFP next;
                    if (!recurrent)
                    {
                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();
                    }
                    else
                    {
                        if (d.TIPO_RECURRENTE == "1")
                        {
                            WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.ACCION_ID == 5).FirstOrDefault();
                            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == autoriza.NS_ACCEPT).FirstOrDefault();
                        }
                        else
                        {
                            int acc = 14;
                            WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.ACCION_ID == acc).FirstOrDefault();
                            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == autoriza.POS).FirstOrDefault();
                        }
                    }
                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                    {
                        d.ESTATUS_WF = "A";
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "2";
                    }
                    else
                    {
                        FLUJO nuevo = new FLUJO();
                        nuevo.WORKF_ID = next.ID;
                        nuevo.WF_VERSION = next.VERSION;
                        nuevo.WF_POS = next.POS;
                        nuevo.NUM_DOC = actual.NUM_DOC;
                        nuevo.POS = actual.POS + 1;
                        nuevo.LOOP = 1;

                        if (next.ACCION.TIPO == "E")
                        {
                            nuevo.USUARIOA_ID = null;
                            nuevo.DETPOS = 0;
                            nuevo.DETVER = 0;
                        }
                        else
                        {
                            if (!recurrent)
                            {
                                FLUJO detA = determinaAgenteI(cf, dah);
                                nuevo.USUARIOA_ID = detA.USUARIOA_ID;
                                nuevo.USUARIOD_ID = nuevo.USUARIOA_ID;

                                DateTime fecha = DateTime.Now.Date;
                                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).FirstOrDefault();
                                if (del != null)
                                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                else
                                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                                nuevo.DETPOS = detA.DETPOS;
                                nuevo.DETVER = cf.VERSION;
                            }
                            else
                            {
                                nuevo.USUARIOA_ID = null;
                                nuevo.DETPOS = 0;
                                nuevo.DETVER = 0;
                            }
                        }
                        nuevo.ESTATUS = "P";
                        nuevo.FECHAC = DateTime.Now;
                        nuevo.FECHAM = DateTime.Now;

                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "1";
                        if (paso_a.EMAIL.Equals("X") && paso_a.ACCION.TIPO == "A" && next.ACCION.TIPO == "S")
                            correcto = "";
                        d.ESTATUS_WF = "P";

                        db.FLUJOes.Add(nuevo);
                        db.Entry(d).State = EntityState.Modified;
                    }//ADD RSG 30.10.2018

                    db.SaveChanges();
                }
                else if (!actual.ESTATUS.Equals("P"))
                    return "1";//-----------------YA FUE PROCESADA
                else
                {
                    ////WORKFP wf = actual.WORKFP;
                    actual.ESTATUS = f.ESTATUS;
                    actual.FECHAM = f.FECHAM;
                    actual.COMENTARIO = f.COMENTARIO;
                    actual.USUARIOA_ID = f.USUARIOA_ID;
                    db.Entry(actual).State = EntityState.Modified;

                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                    bool ban = true;
                    while (ban)         //--------------PARA LOOP
                    {
                        int next_step_a = 0;
                        ////int next_step_r = 0;
                        if (paso_a.NEXT_STEP != null)
                            next_step_a = (int)paso_a.NEXT_STEP;
                        if (paso_a.NS_ACCEPT != null)
                            next_step_a = (int)paso_a.NS_ACCEPT;
                        ////if (paso_a.NS_REJECT != null)
                        ////    next_step_r = (int)paso_a.NS_REJECT;

                        WORKFP next;
                        if (paso_a.ACCION.TIPO == "A" || paso_a.ACCION.TIPO == "N" || paso_a.ACCION.TIPO == "R" || paso_a.ACCION.TIPO == "T" || paso_a.ACCION.TIPO == "E" || paso_a.ACCION.TIPO == "B" || paso_a.ACCION.TIPO == "M" || paso_a.ACCION.TIPO == "O")//Si está en proceso de aprobación
                        {
                            if (f.ESTATUS.Equals("A") || f.ESTATUS.Equals("N") || f.ESTATUS.Equals("M"))//APROBAR SOLICITUD
                            {
                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();

                                FLUJO nuevo = new FLUJO();
                                nuevo.WORKF_ID = paso_a.ID;
                                nuevo.WF_VERSION = paso_a.VERSION;
                                nuevo.WF_POS = next.POS;
                                nuevo.NUM_DOC = actual.NUM_DOC;
                                nuevo.POS = actual.POS + 1;
                                nuevo.LOOP = 1;

                                FLUJO detA;
                                if (paso_a.ACCION.TIPO == "N")
                                    actual.DETPOS = actual.DETPOS - 1;
                                int sop = 99;
                                if (next.ACCION.TIPO == "S")
                                    sop = 98;

                                CLIENTEF cf = db.CLIENTEFs.Where(a => a.VKORG.Equals(d.VKORG) && a.VTWEG.Equals(d.VTWEG) && a.SPART.Equals(d.SPART) && a.KUNNR.Equals(d.PAYER_ID)
                                                ).OrderByDescending(a => a.VERSION).FirstOrDefault();

                                detA = determinaAgenteC(d, cf, actual.DETPOS, sop, paso_a.ACCION.TIPO);


                                nuevo.USUARIOD_ID = detA.USUARIOA_ID;

                                DateTime fecha = DateTime.Now.Date;
                                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).FirstOrDefault();
                                if (del != null)
                                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                else
                                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;





                                nuevo.DETPOS = detA.DETPOS;
                                nuevo.DETVER = actual.DETVER;
                                if (paso_a.ACCION.TIPO == "N")
                                {
                                    nuevo.DETPOS = nuevo.DETPOS + 1;
                                    actual.DETPOS = actual.DETPOS + 1;
                                }

                                if (d.DOCUMENTORECs.Count > 0)
                                    recurrent = true;

                                if (nuevo.DETPOS == 0 || nuevo.DETPOS == 99)
                                {
                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();
                                    if (recurrent && next.ACCION.TIPO.Equals("P"))
                                    {
                                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();
                                        if (next.NEXT_STEP != null)
                                            next_step_a = (int)next.NEXT_STEP;
                                        if (next.NS_ACCEPT != null)
                                            next_step_a = (int)next.NS_ACCEPT;
                                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();
                                    }
                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                                    {
                                        d.ESTATUS_WF = "A";
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "2";

                                        if (recurrent)
                                        {
                                            FLUJO nuevos = new FLUJO();
                                            nuevos.WORKF_ID = paso_a.ID;
                                            nuevos.WF_VERSION = paso_a.VERSION;
                                            nuevos.WF_POS = next.POS;
                                            nuevos.NUM_DOC = actual.NUM_DOC;
                                            nuevos.POS = actual.POS + 1;
                                            nuevos.ESTATUS = "A";
                                            nuevos.FECHAC = DateTime.Now;
                                            nuevos.FECHAM = DateTime.Now;

                                            d.ESTATUS = "A";

                                            db.FLUJOes.Add(nuevos);
                                            ban = false;
                                        }
                                    }
                                    else
                                    {
                                        nuevo.WORKF_ID = next.ID;
                                        nuevo.WF_VERSION = next.VERSION;
                                        nuevo.WF_POS = next.POS + detA.POS;
                                        nuevo.NUM_DOC = actual.NUM_DOC;
                                        nuevo.POS = actual.POS + 1;
                                        nuevo.LOOP = 1;//-----------------------------------
                                        
                                        bool finR = false;
                                        d.ESTATUS_WF = "P";
                                        if (next.ACCION.TIPO.Equals("T"))
                                        {
                                            TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) && a.PAIS_ID.Equals(d.PAIS_ID) && a.ACTIVO == true).FirstOrDefault();
                                            if (tl != null && cf.USUARIO7_ID != null)
                                            {
                                                ////nuevo.USUARIOA_ID = db.DET_TAXEOC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) && a.PAIS_ID.Equals(d.PAIS_ID) && a.KUNNR == d.PAYER_ID && a.ACTIVO).FirstOrDefault().USUARIOA_ID;
                                                nuevo.USUARIOA_ID = cf.USUARIO7_ID;
                                                d.ESTATUS_WF = "T";
                                            }
                                            else
                                            {
                                                nuevo.WF_POS = nuevo.WF_POS + 1;
                                                nuevo.USUARIOA_ID = null;
                                                d.ESTATUS_WF = "A";
                                                d.ESTATUS_SAP = "P";

                                                if (recurrent)
                                                {
                                                    nuevo.WF_POS++;
                                                    d.ESTATUS_SAP = "";
                                                    finR = true;
                                                }
                                            }
                                        }
                                        else if (paso_a.ACCION.TIPO == "E")
                                        {
                                            nuevo.USUARIOA_ID = null;
                                        }
                                        else
                                        {
                                            if (nuevo.DETPOS == 0)
                                            {
                                                nuevo.USUARIOA_ID = null;
                                                d.ESTATUS_WF = "A";
                                                d.ESTATUS_SAP = "P";
                                            }
                                        }
                                        nuevo.ESTATUS = "P";
                                        nuevo.FECHAC = DateTime.Now;
                                        nuevo.FECHAM = DateTime.Now;

                                        if (finR)
                                        {
                                            nuevo.ESTATUS = "A";
                                            d.ESTATUS = "A";
                                        }

                                        db.FLUJOes.Add(nuevo);
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "1";
                                    }
                                }
                                else
                                {
                                    nuevo.ESTATUS = "P";
                                    nuevo.FECHAC = DateTime.Now;
                                    nuevo.FECHAM = DateTime.Now;
                                    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

                                    db.FLUJOes.Add(nuevo);
                                    if (paso_a.EMAIL.Equals("X"))
                                        correcto = "1";

                                    d.ESTATUS_WF = "P";
                                }
                                if (nuevo.DETPOS.Equals(98))
                                    d.ESTATUS_WF = "S";
                                db.Entry(d).State = EntityState.Modified;

                                db.SaveChanges();
                                ban = false;
                            }
                        }
                        else if (paso_a.ACCION.TIPO == "P")
                        {
                            if (f.ESTATUS.Equals("A") || f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
                            {
                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);

                                ArchivoContable sa = new ArchivoContable();
                                List<ArchivoC> ac = new List<ArchivoC>();
                                ////string file = sa.generarArchivo(d.NUM_DOC, 0, 0);
                                string file = "";
                                if(!cero)
                                file = sa.generarArchivo(d.NUM_DOC, 0, 0, "F", ref ac);
                                ////string file = "";

                                if (file == "")
                                {
                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();

                                    FLUJO nuevo = new FLUJO();
                                    nuevo.WORKF_ID = paso_a.ID;
                                    nuevo.WF_VERSION = paso_a.VERSION;
                                    nuevo.WF_POS = next.POS;
                                    nuevo.NUM_DOC = actual.NUM_DOC;
                                    nuevo.POS = actual.POS + 1;
                                    nuevo.ESTATUS = "A";
                                    nuevo.FECHAC = DateTime.Now;
                                    nuevo.FECHAM = DateTime.Now;

                                    d.ESTATUS = "A";
                                    correcto = file;

                                    db.FLUJOes.Add(nuevo);
                                    db.SaveChanges();
                                    ban = false;
                                }
                                else
                                {
                                    ban = false;
                                    correcto = file;
                                }
                            }
                        }
                        else if (paso_a.ACCION.TIPO == "S" && (f.ESTATUS.Equals("A") || f.ESTATUS.Equals("N")))//APROBAR SOLICITUD
                            {
                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();

                                FLUJO nuevo = new FLUJO();
                                nuevo.WORKF_ID = paso_a.ID;
                                nuevo.WF_VERSION = paso_a.VERSION;
                                nuevo.WF_POS = next.POS;
                                nuevo.NUM_DOC = actual.NUM_DOC;
                                nuevo.POS = actual.POS + 1;
                                nuevo.LOOP = 1;//-----------------------------------cc

                                if (paso_a.ACCION.TIPO == "N")
                                    actual.DETPOS = actual.DETPOS - 1;


                                CLIENTEF cf = db.CLIENTEFs.Where(a => a.VKORG.Equals(d.VKORG) && a.VTWEG.Equals(d.VTWEG) && a.SPART.Equals(d.SPART) && a.KUNNR.Equals(d.PAYER_ID)
                                                ).OrderByDescending(a => a.VERSION).FirstOrDefault();
                                FLUJO detA = determinaAgenteC(d, cf, 98, 98, "S");





                                nuevo.USUARIOD_ID = detA.USUARIOA_ID;

                                DateTime fecha = DateTime.Now.Date;
                                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).FirstOrDefault();
                                if (del != null)
                                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                                else
                                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;





                                nuevo.DETPOS = detA.DETPOS;
                                nuevo.DETVER = actual.DETVER;
                                if (paso_a.ACCION.TIPO == "N")
                                {
                                    nuevo.DETPOS = nuevo.DETPOS + 1;
                                    actual.DETPOS = actual.DETPOS + 1;
                                }


                                if (nuevo.DETPOS == 0 || nuevo.DETPOS == 99)
                                {
                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_a).FirstOrDefault();
                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                                    {
                                        d.ESTATUS_WF = "A";
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "2";
                                    }
                                    else
                                    {
                                        nuevo.WORKF_ID = next.ID;
                                        nuevo.WF_VERSION = next.VERSION;
                                        nuevo.WF_POS = next.POS + detA.POS;
                                        nuevo.NUM_DOC = actual.NUM_DOC;
                                        nuevo.POS = actual.POS + 1;
                                        nuevo.LOOP = 1;//-----------------------------------

                                        d.ESTATUS_WF = "P";
                                        if (nuevo.DETPOS == 0)
                                        {
                                            nuevo.USUARIOA_ID = null;
                                            d.ESTATUS_WF = "A";
                                            d.ESTATUS_SAP = "P";
                                        }
                                        nuevo.ESTATUS = "P";
                                        nuevo.FECHAC = DateTime.Now;
                                        nuevo.FECHAM = DateTime.Now;

                                        db.FLUJOes.Add(nuevo);
                                        if (paso_a.EMAIL.Equals("X"))
                                            correcto = "1";
                                    }
                                }
                                else
                                {
                                    nuevo.ESTATUS = "P";
                                    nuevo.FECHAC = DateTime.Now;
                                    nuevo.FECHAM = DateTime.Now;
                                    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

                                    db.FLUJOes.Add(nuevo);
                                    if (paso_a.EMAIL.Equals("X"))
                                        correcto = "1";

                                    d.ESTATUS_WF = "P";
                                }
                                db.Entry(d).State = EntityState.Modified;

                                db.SaveChanges();
                                ban = false;
                            }
                        

                    }
                }
            }
            else if (f.ESTATUS.Equals("R"))//Rechazada
            {
                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS.Equals(actual.WF_POS)).FirstOrDefault();

                ////int next_step_a = 0;
                int next_step_r = 0;
                ////if (paso_a.NEXT_STEP != null)
                ////    next_step_a = (int)paso_a.NEXT_STEP;
                ////if (paso_a.NS_ACCEPT != null)
                ////    next_step_a = (int)paso_a.NS_ACCEPT;
                if (paso_a.NS_REJECT != null)
                    next_step_r = (int)paso_a.NS_REJECT;

                WORKFP next;
                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == next_step_r).FirstOrDefault();

                correcto = "3";
                actual.ESTATUS = f.ESTATUS;
                actual.FECHAM = f.FECHAM;
                actual.COMENTARIO = f.COMENTARIO;

                FLUJO nuevo = new FLUJO();
                nuevo.WORKF_ID = next.ID;
                nuevo.WF_VERSION = next.VERSION;
                nuevo.WF_POS = next.POS;
                nuevo.NUM_DOC = actual.NUM_DOC;
                nuevo.POS = actual.POS + 1;
                nuevo.DETPOS = 1;
                nuevo.DETVER = actual.DETVER;
                nuevo.LOOP = 1;//-----------------------------------
                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                nuevo.USUARIOD_ID = d.USUARIOD_ID;
                DateTime fecha = DateTime.Now.Date;
                DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).FirstOrDefault();
                if (del != null)
                    nuevo.USUARIOA_ID = del.USUARIOD_ID;
                else
                    nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                nuevo.ESTATUS = "P";
                nuevo.FECHAC = DateTime.Now;
                nuevo.FECHAM = DateTime.Now;

                db.FLUJOes.Add(nuevo);
                db.Entry(actual).State = EntityState.Modified;
                d.ESTATUS_WF = "R";
                if (next.ACCION.TIPO == "S")
                {
                    d.ESTATUS = "R";
                    d.ESTATUS_WF = "S";
                }

                db.SaveChanges();

            }

            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            if (string.IsNullOrEmpty(correcto))
            {
                FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                string corr = "";
                if (conta.WORKFP.ACCION.TIPO == "P")
                {
                    conta.ESTATUS = "A";
                    conta.FECHAM = DateTime.Now;
                    corr = procesa(conta, recurrente);
                }
                Console.Write(corr);
                ////if (corr == "")
                ////{
                ////    if (f.DOCUMENTO.DOCUMENTO_REF != null)
                ////    {
                ////        f.DOCUMENTO.TSOL = db.DOCUMENTOes.Where(x => x.NUM_DOC == f.NUM_DOC).FirstOrDefault().TSOL;
                ////        if (f.DOCUMENTO.TSOL.REVERSO == false)
                ////        {
                ////            List<DOCUMENTO> rels = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == f.DOCUMENTO.DOCUMENTO_REF && !x.TSOL.REVERSO && x.ESTATUS_C != "C" && x.ESTATUS_WF != "B").ToList();
                ////            bool ban = true;
                ////            foreach (DOCUMENTO re in rels)
                ////            {
                ////                if(re.ESTATUS_WF)
                ////            }
                ////            DOCUMENTO rel = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == f.DOCUMENTO.DOCUMENTO_REF && x.TSOL.REVERSO == true).FirstOrDefault();
                ////            if (rel != null && rel.ESTATUS_WF == "A")
                ////            {
                ////                FLUJO rev = db.FLUJOes.Where(x => x.NUM_DOC == rel.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                ////                rev.ESTATUS = "A";
                ////                rev.FECHAM = DateTime.Now;
                ////                corr = procesa(rev, recurrente);
                ////            }
                ////        }
                ////    }
                ////}
            }
            ////else if (correcto.Equals("1"))
            ////{
            ////    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
            ////    string corr = "";
            ////    if (conta.WORKFP.ACCION.TIPO == "B")
            ////    {
            ////        Email em = new Email();
            ////        em.enviaMailC(f.NUM_DOC, false, null, )
            ////    }
            ////}

            return correcto;
        }

        public FLUJO determinaAgenteI(CLIENTEF cf, DET_APROBH dah)
        {
            FLUJO f = new FLUJO();
            f.DETPOS = 1;

            DET_APROBP ddp = dah.DET_APROBP.OrderBy(x => x.POS).FirstOrDefault();//oBTIENE TABLA DE FLUJO
            if ((ddp.PUESTOA_ID - 1) == 0)
            {
                f.USUARIOA_ID = cf.USUARIO1_ID;
            }
            if ((ddp.PUESTOA_ID - 1) == 1)
            {
                f.USUARIOA_ID = cf.USUARIO2_ID;
            }
            if ((ddp.PUESTOA_ID - 1) == 2)
            {
                f.USUARIOA_ID = cf.USUARIO3_ID;
            }
            if ((ddp.PUESTOA_ID - 1) == 3)
            {
                f.USUARIOA_ID = cf.USUARIO4_ID;
            }
            if ((ddp.PUESTOA_ID - 1) == 4)
            {
                f.USUARIOA_ID = cf.USUARIO5_ID;
            }

            return f;
        }

        public FLUJO determinaAgenteC(DOCUMENTO d, CLIENTEF cf, int pos, int sop, string tipo)//, string next)
        {
            FLUJO f = new FLUJO();
            List<DET_APROBP> ddp = new List<DET_APROBP>();
            using (TruequeEntities db = new TruequeEntities())
            {
                DET_APROBH dh = db.DET_APROBH.Where(a => a.SOCIEDAD_ID == d.SOCIEDAD_ID && a.PUESTOC_ID == d.PUESTO_ID && a.ACTIVO)
                                    .OrderByDescending(a => a.VERSION).FirstOrDefault();
                if (dh != null)
                    ddp = db.DET_APROBP.Where(a => a.SOCIEDAD_ID.Equals(dh.SOCIEDAD_ID) && a.PUESTOC_ID == dh.PUESTOC_ID && a.VERSION == dh.VERSION && a.ACTIVO).ToList();//oBTIENE TABLA DE FLUJO

                if (tipo == "N")//------Si es modificación por rechazo de manager
                {
                    FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) && a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
                    f = ffl;
                }
            }
            int ppos = 0;
            DET_APROBP dp = ddp.FirstOrDefault(a => a.POS == pos);//Obtiene nivel de autorización
            if (dp != null)//----Si existe posición
            {
                int detp = 0;
                int detpu = 0;
                if (tipo != "B" && tipo != "M")//---Si no es notificación ni modificación por rechazo de TS
                {
                    if (dp.MONTO != null)
                        if (d.MONTO_DOC_MD > dp.MONTO)
                        {
                            ppos--;
                            detp = dp.N_MONTO == null ? (pos + 1) : (int)dp.N_MONTO;
                            detpu = (int)ddp.FirstOrDefault(a => a.POS == detp).PUESTOA_ID;
                        }
                    if (ppos == 0)
                        if (dp.PRESUPUESTO != null && d.EXCEDE_PRES != null)
                            if (d.EXCEDE_PRES == "X" && dp.PRESUPUESTO == true)
                            {
                                ppos--;
                                detp = dp.N_PRESUP == null ? (pos + 1) : (int)dp.N_PRESUP;
                                detpu = (int)ddp.FirstOrDefault(a => a.POS == detp).PUESTOA_ID;
                            }
                }
                else
                {
                    pos = 98;
                }


                if (pos == 99)
                {
                    f.USUARIOA_ID = null;
                }
                else if (ppos == 0 && sop == 98)//Cuando va a soporte(Termina autorización)
                {
                    f.USUARIOA_ID = d.USUARIOD_ID;
                    f.DETPOS = sop;
                }
                else if (ppos == 0 && sop == 99)//Cuando va a TS(Termina autorización)
                {
                    f.USUARIOA_ID = cf.USUARIO6_ID;
                    f.DETPOS = sop;
                }
                else
                {
                    if ((detpu - 1) == 1)
                    {
                        f.USUARIOA_ID = cf.USUARIO2_ID;
                        if (cf.USUARIO2_ID != null)
                            f.DETPOS = detp;
                        else
                        {
                            ppos++;
                            f.DETPOS = sop;
                            if (sop == 99)
                                f.USUARIOA_ID = cf.USUARIO6_ID;
                            else
                                f.USUARIOA_ID = d.USUARIOD_ID;
                        }
                    }
                    if ((detpu - 1) == 2)
                    {
                        f.USUARIOA_ID = cf.USUARIO3_ID;
                        if (cf.USUARIO3_ID != null)
                            f.DETPOS = detp;
                        else
                        {
                            ppos++;
                            f.DETPOS = sop;
                            if (sop == 99)
                                f.USUARIOA_ID = cf.USUARIO6_ID;
                            else
                                f.USUARIOA_ID = d.USUARIOD_ID;
                        }
                    }
                    if ((detpu - 1) == 3)
                    {
                        f.USUARIOA_ID = cf.USUARIO4_ID;
                        if (cf.USUARIO4_ID != null)
                            f.DETPOS = detp;
                        else
                        {
                            ppos++;
                            f.DETPOS = sop;
                            if (sop == 99)
                                f.USUARIOA_ID = cf.USUARIO6_ID;
                            else
                                f.USUARIOA_ID = d.USUARIOD_ID;
                        }
                    }
                    if ((detpu - 1) == 4)
                    {
                        f.USUARIOA_ID = cf.USUARIO5_ID;
                        if (cf.USUARIO5_ID != null)
                            f.DETPOS = detp;
                        else
                        {
                            ppos++;
                            f.DETPOS = sop;
                            if (sop == 99)
                                f.USUARIOA_ID = cf.USUARIO6_ID;
                            else
                                f.USUARIOA_ID = d.USUARIOD_ID;
                        }
                    }
                    if (pos == 98)//Despues de rechazo de TS para PRA - regresa a TS
                    {
                        f.USUARIOA_ID = cf.USUARIO6_ID;
                        f.DETPOS = 99;
                    }
                }
            }
            else if (pos == 98)
            {
                f.USUARIOA_ID = cf.USUARIO6_ID;
                f.DETPOS = 99;
            }
            else if (pos == 0)//Despues de rechazo de TS para PR - regresa a TS
            {
                if (f.DETPOS == 99)
                    ppos = 1;
                f.DETPOS--;
                f.POS = ppos;
            }
            f.POS = ppos;

            return f;
        }
        public FLUJO borrador(FLUJO actual, CLIENTEF cf, DET_APROBH dah)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                ////WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                ////int next_step_a = 0;
                ////if (paso_a.NEXT_STEP != null)
                ////    next_step_a = (int)paso_a.NEXT_STEP;

                WORKFP next;
                WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.ACCION_ID == 5).FirstOrDefault();
                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) && a.VERSION.Equals(actual.WF_VERSION) && a.POS == autoriza.NS_ACCEPT).FirstOrDefault();

                if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                {
                    return null;
                }
                else
                {
                    FLUJO nuevo = new FLUJO();
                    nuevo.WORKF_ID = next.ID;
                    nuevo.WF_VERSION = next.VERSION;
                    nuevo.WF_POS = next.POS;
                    nuevo.NUM_DOC = actual.NUM_DOC;
                    nuevo.POS = actual.POS + 1;
                    nuevo.LOOP = 1;

                    if (next.ACCION.TIPO == "E")
                    {
                        nuevo.USUARIOA_ID = null;
                        nuevo.DETPOS = 0;
                        nuevo.DETVER = 0;
                    }
                    else
                    {
                        ////if (recurrente != "X")
                        ////{
                        FLUJO detA = determinaAgenteI(cf, dah);
                        nuevo.USUARIOA_ID = detA.USUARIOA_ID;
                        nuevo.USUARIOD_ID = nuevo.USUARIOA_ID;

                        DateTime fecha = DateTime.Now.Date;
                        DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) && a.FECHAI <= fecha && a.FECHAF >= fecha && a.ACTIVO).FirstOrDefault();
                        if (del != null)
                            nuevo.USUARIOA_ID = del.USUARIOD_ID;
                        else
                            nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                        nuevo.DETPOS = detA.DETPOS;
                        nuevo.DETVER = cf.VERSION;
                        ////}
                        ////else
                        ////{
                        ////    nuevo.USUARIOA_ID = null;
                        ////    nuevo.DETPOS = 0;
                        ////    nuevo.DETVER = 0;
                        ////}
                    }
                    nuevo.ESTATUS = "P";
                    nuevo.FECHAC = DateTime.Now;
                    nuevo.FECHAM = DateTime.Now;
                    return nuevo;

                }//ADD RSG 30.10.2018

            }
        }
    }
}


