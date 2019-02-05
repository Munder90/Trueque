using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class Rangos
    {
        public RANGO getRango(string TSOL_ID)
        {
            RANGO rango = new RANGO();
            using (TruequeEntities db = new TruequeEntities())
            {

                rango = (from r in db.RANGOes
                         join s in db.TSOLs
                         on r.ID equals s.RANGO_ID
                         where s.ID == TSOL_ID && r.ACTIVO == true
                         select r).FirstOrDefault();

            }

            return rango;

        }
        public decimal getSolID(string TSOL_ID)
        {

            decimal id = 0;

            RANGO rango = getRango(TSOL_ID);

            if (rango.ACTUAL > rango.INICIO && rango.ACTUAL < rango.FIN)
            {
                rango.ACTUAL++;
                id = (decimal)rango.ACTUAL;
            }

            return id;
        }

        public void updateRango(string TSOL_ID, decimal actual)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                RANGO rango = getRango(TSOL_ID);

                if (rango.ACTUAL > rango.INICIO && rango.ACTUAL < rango.FIN)
                {
                    rango.ACTUAL = actual;
                }

                db.Entry(rango).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}