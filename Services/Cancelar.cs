using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class Cancelar
    {
        public void cancela(decimal id, string user)//RSG 07.06.2018---------------------------------------------
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                DOCUMENTO d = db.DOCUMENTOes.Find(id);
                d.ESTATUS_C = "C";
                FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC == id).OrderByDescending(a => a.POS).FirstOrDefault();
                db.Entry(d).State = EntityState.Modified;

                if (d.DOCUMENTO_REF != null && !d.TSOL.REVERSO)//Se cancela una relacionada
                {
                    DOCUMENTO dRef = db.DOCUMENTOes.Find(d.DOCUMENTO_REF);//Se abre de nuevo la provisión
                    dRef.ESTATUS = "A";
                    db.Entry(dRef).State = EntityState.Modified;

                    if (actual != null)
                    {
                        FLUJO nuevo = new FLUJO();
                        WORKFP fin = db.WORKFPs.Where(a => a.ID == actual.WORKF_ID && a.VERSION == actual.WF_VERSION && a.NEXT_STEP == 99).FirstOrDefault();
                        if (fin != null)
                        {
                            nuevo.COMENTARIO = "";
                            nuevo.DETPOS = 0;
                            nuevo.DETVER = 0;
                            nuevo.ESTATUS = "A";
                            nuevo.FECHAC = DateTime.Now;
                            nuevo.FECHAM = nuevo.FECHAC;
                            nuevo.LOOP = 0;
                            nuevo.NUM_DOC = actual.NUM_DOC;
                            nuevo.POS = actual.POS + 1;
                            nuevo.USUARIOA_ID = user;
                            nuevo.WF_POS = fin.POS;
                            nuevo.WF_VERSION = fin.VERSION;
                            nuevo.WORKF_ID = fin.ID;
                            db.FLUJOes.Add(nuevo);
                        }
                    }
                    DOCUMENTO rev = db.DOCUMENTOes.FirstOrDefault(a => a.DOCUMENTO_REF == dRef.NUM_DOC && a.ESTATUS_C != "C" && a.ESTATUS_WF != "B" && a.TSOL.REVERSO);
                    if (rev != null)
                    {
                        cancela(rev.NUM_DOC, user);
                    }
                }
                else if (actual != null)
                {
                    if (d.DOCUMENTO_REF != null)//Se cancela una relacionada
                    {
                        DOCUMENTO dRef = db.DOCUMENTOes.Find(d.DOCUMENTO_REF);//Se abre de nuevo la provisión
                        dRef.ESTATUS = "A";
                        db.Entry(dRef).State = EntityState.Modified;
                    }
                    FLUJO nuevo = new FLUJO();
                    WORKFP fin = db.WORKFPs.Where(a => a.ID == actual.WORKF_ID && a.VERSION == actual.WF_VERSION && a.NEXT_STEP == 99).FirstOrDefault();
                    if (fin != null)
                    {
                        nuevo.COMENTARIO = "";
                        nuevo.DETPOS = 0;
                        nuevo.DETVER = 0;
                        nuevo.ESTATUS = "A";
                        nuevo.FECHAC = DateTime.Now;
                        nuevo.FECHAM = nuevo.FECHAC;
                        nuevo.LOOP = 0;
                        nuevo.NUM_DOC = actual.NUM_DOC;
                        nuevo.POS = actual.POS + 1;
                        nuevo.USUARIOA_ID = user;
                        nuevo.WF_POS = fin.POS;
                        nuevo.WF_VERSION = fin.VERSION;
                        nuevo.WORKF_ID = fin.ID;
                        db.FLUJOes.Add(nuevo);
                    }
                }

                db.SaveChanges();
            }
        }
    }
}