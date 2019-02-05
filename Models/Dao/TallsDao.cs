using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class TallsDao
    {
        readonly TruequeEntities db = new TruequeEntities();


        public  List<TALLT> ListaTallsConCuenta(int accion,string prefix,string spras_id, string pais_id, int ejercicio, string sociedad_id)
        {
            if (prefix == null) { prefix = ""; }
            List<object> paramsCSP = new List<object>
            {
              new SqlParameter("@SPRAS_ID", spras_id)
            };
            if (!string.IsNullOrEmpty(pais_id)) { paramsCSP.Add(new SqlParameter("@PAIS_ID", pais_id)); }
            else { paramsCSP.Add(new SqlParameter("@PAIS_ID", DBNull.Value)); }

            paramsCSP.Add(new SqlParameter("@EJERCICIO", ejercicio));

            if (!string.IsNullOrEmpty(sociedad_id)) { paramsCSP.Add(new SqlParameter("@SOCIEDAD_ID", sociedad_id)); }
            else { paramsCSP.Add(new SqlParameter("@SOCIEDAD_ID", DBNull.Value)); }

            paramsCSP.Add(new SqlParameter("@PREFIX", prefix));
            paramsCSP.Add(new SqlParameter("@ACCION", accion));

            List<TALLT> talls = db.Database.SqlQuery<TALLT>("CSP_LISTA_TALL_CUENTA @SPRAS_ID,@PAIS_ID,@EJERCICIO,@SOCIEDAD_ID,@PREFIX,@ACCION",
               paramsCSP.ToArray()).ToList();

            return talls;
        }
    }
}