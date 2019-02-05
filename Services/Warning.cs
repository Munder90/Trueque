using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class Warning
    {
        public string listaW(string bukrs, string spras)//RSG 07.06.2018---------------------------------------------
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                List<WARNINGP> lwp = db.WARNINGPs.Where(x => x.SOCIEDAD_ID.Equals(bukrs) | x.SOCIEDAD_ID == null).ToList();
                List<WARNINGPT> lwpt = db.WARNINGPTs.Where(x => x.SPRAS_ID.Equals(spras)).ToList();
                List<WARNING_COND> lwc = db.WARNING_COND.Where(x => x.ACTIVO.Equals(true)).ToList();
                string val = "[";
                int cont = 1;
                foreach (WARNINGP wp in lwp)
                {
                    if (lwc.Where(x => x.TAB_ID == wp.TAB_ID & x.WARNING_ID == wp.ID).ToList().Count > 0)
                    {
                        if (cont != 1)
                            val += ", ";
                        val += "{ ID: '" + wp.CAMPOVAL_ID + "'";
                        val += ", BUKRS: '" + wp.SOCIEDAD_ID + "'";
                        val += ", TSOL: '" + wp.TSOL_ID + "'";
                        val += ", TAB: '" + wp.TAB_ID + "'";
                        val += ", ELEM: '" + wp.CAMPO_ID + "'";
                        WARNINGPT pt = lwpt.Where(x => x.TAB_ID == wp.TAB_ID & x.WARNING_ID == wp.ID).FirstOrDefault();
                        if (pt == null)
                            val += ", MSG: '" + wp.DESCR + "'";
                        else
                            val += ", MSG: '" + lwpt.Where(x => x.TAB_ID == wp.TAB_ID & x.WARNING_ID == wp.ID).FirstOrDefault().TXT100 + "'";
                        if (wp.TIPO == "E")
                            val += ", TIPO: 'error', COLOR: 'red'";
                        else
                            val += ", TIPO: 'info', COLOR: 'yellow'";
                        val += ", COND: [";
                        int cont2 = 0;
                        foreach (WARNING_COND wc in lwc.Where(x => x.TAB_ID == wp.TAB_ID & x.WARNING_ID == wp.ID).ToList())
                        {
                            if (cont2 != 0)
                                val += ", ";
                            val += "{ andor: '" + wc.ANDOR + "'";
                            val += ", comp: '" + wc.CONDICION.COND + "'";
                            val += ", val2: '" + wc.VALOR_COMP + "'";
                            val += ", orand: '" + wc.ORAND + "'";
                            val += "}";
                            cont2++;
                        }
                        val += "]";
                        val += ", ACTION: '" + wp.ACCION + "'";
                        val += ", NUM: " + cont + "}";
                        cont++;
                    }
                }
                val += "]";

                return val;
            }
        }
    }
}