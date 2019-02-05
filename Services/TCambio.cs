using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class TCambio
    {
        public decimal getValSoc(string waers, string moneda_id, decimal monto_doc_md, out string errorString)
        {
            decimal val = 0;

            //Siempre la conversión va a la sociedad    

            var UKURS = getUkurs(waers, moneda_id, out errorString);

            if (errorString.Equals(""))
            {

                decimal uk = Convert.ToDecimal(UKURS);

                if (UKURS > 0)
                {
                    val = uk * Convert.ToDecimal(monto_doc_md);
                }
            }

            return val;
        }
        public decimal getUkurs(string waers, string moneda_id, out string errorString)
        {
            decimal ukurs = 0;
            errorString = string.Empty;
            if (waers != moneda_id)
                using (TruequeEntities db = new TruequeEntities())
                {
                    try
                    {
                        //Siempre la conversión va a la sociedad    
                        var date = DateTime.Now.Date;
                        var tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(waers) && t.GDATU.Equals(date)).FirstOrDefault();
                        if (tcambio == null)
                        {
                            var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(waers)).Max(a => a.GDATU);
                            tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(moneda_id) && t.TCURR.Equals(waers) && t.GDATU.Equals(max)).FirstOrDefault();
                        }

                        ukurs = Convert.ToDecimal(tcambio.UKURS);

                    }
                    catch (Exception e)
                    {
                        errorString = "detail: conversion " + moneda_id + " to " + waers + " in date " + DateTime.Now.Date;
                        return 0;
                    }
                }
            else
                ukurs = 1;

            return ukurs;
        }

        public decimal getUkursUSD(string waers, string waersusd, out string errorString)
        {
            decimal ukurs = 0;
            errorString = string.Empty;
            if (waers != waersusd)
                using (TruequeEntities db = new TruequeEntities())
                {
                    try
                    {
                        var date = DateTime.Now.Date;
                        var tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(waers) && t.TCURR.Equals(waersusd) && t.GDATU.Equals(date)).FirstOrDefault();
                        if (tcambio == null)
                        {
                            var max = db.TCAMBIOs.Where(t => t.FCURR.Equals(waers) && t.TCURR.Equals(waersusd)).Max(a => a.GDATU);
                            tcambio = db.TCAMBIOs.Where(t => t.FCURR.Equals(waers) && t.TCURR.Equals(waersusd) && t.GDATU.Equals(max)).FirstOrDefault();
                        }

                        ukurs = Convert.ToDecimal(tcambio.UKURS);
                    }
                    catch (Exception e)
                    {
                        errorString = "detail: conversion " + waers + " to " + waersusd + " in date " + DateTime.Now.Date;
                        return 0;
                    }

                }
            else
                ukurs = 1;

            return ukurs;
        }

    }
}