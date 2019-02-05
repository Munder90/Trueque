using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Models;

namespace TAT001.Services
{
    public class Presupuesto
    {
     readonly   TruequeEntities db = new TruequeEntities();
        public PRESUPUESTO_MOD getPresupuesto(string kunnr , string mes)//RSG 07.06.2018---------------------------------------------
        {
            PRESUPUESTO_MOD pm = new PRESUPUESTO_MOD();
            try
            {
                if (kunnr == null)
                    kunnr = "";

                //Obtener presupuesto
                //var presupuesto = FnCommon.ObtenerPresupuestoCliente(db,kunnr, mes);// db.CSP_PRESU_CLIENT(cLIENTE: kunnr, pERIODO: mes).ToList();
                string clien = db.CLIENTEs.Where(x => x.KUNNR == kunnr).Select(x => x.BANNERG).First();
                var clien2 = db.CLIENTEs.Where(x => x.KUNNR == kunnr).FirstOrDefault();
                string desCanal = db.CANALs.Where(x => x.CANAL1 == clien2.CANAL).FirstOrDefault().CDESCRIPCION;

                //if (presupuesto != null)
                //{
                //    pm.CANAL = desCanal;
                //    pm.CLIENTE = clien2.NAME1;
                //    if (String.IsNullOrEmpty(clien))
                //    {
                //        pm.P_CANAL = decimal.Parse(presupuesto[0].VALOR.ToString());
                //        pm.P_BANNER = decimal.Parse(presupuesto[1].VALOR.ToString());
                //        pm.PC_C = (decimal.Parse(presupuesto[4].VALOR.ToString()) + decimal.Parse(presupuesto[5].VALOR.ToString()) + decimal.Parse(presupuesto[6].VALOR.ToString()));
                //        pm.PC_A = decimal.Parse(presupuesto[8].VALOR.ToString());
                //        pm.PC_P = decimal.Parse(presupuesto[9].VALOR.ToString());
                //        pm.PC_T = pm.PC_C + pm.PC_A + pm.PC_P;
                //        pm.CONSU = (decimal.Parse(presupuesto[1].VALOR.ToString()) - pm.PC_T);
                //    }
                //    else
                //    {
                //        pm.P_CANAL = decimal.Parse(presupuesto[0].VALOR.ToString());
                //        pm.P_BANNER = decimal.Parse(presupuesto[1].VALOR.ToString());
                //        pm.PC_C = (decimal.Parse(presupuesto[4].VALOR.ToString()) + decimal.Parse(presupuesto[5].VALOR.ToString()) + decimal.Parse(presupuesto[6].VALOR.ToString()));
                //        pm.PC_A = decimal.Parse(presupuesto[8].VALOR.ToString());
                //        pm.PC_P = decimal.Parse(presupuesto[9].VALOR.ToString());
                //        pm.PC_T = pm.PC_C + pm.PC_A + pm.PC_P;
                //        pm.CONSU = (decimal.Parse(presupuesto[1].VALOR.ToString()) - pm.PC_T);
                //    }
                //}
            }
            catch(Exception e)
            {
                Log.ErrorLogApp(e,"Presupuesto","getPresupuesto");
            }
            return pm;
        }
    }
}
