using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
	public class StatesDao
	{
        readonly TruequeEntities db = new TruequeEntities();
        public List<STATE> ListaStates(string Prefix, string pais, string sociedad_id)
        {
            if (Prefix == null) { Prefix = ""; }

            if (pais == null)
            {
                pais = db.SOCIEDADs.Any(x => x.BUKRS == sociedad_id) ? db.SOCIEDADs.First(x => x.BUKRS == sociedad_id).LAND : "";
            }
            string p = pais.Split('.')[0].ToUpper();
            List<object> paramsCSP = new List<object>
            {
                new SqlParameter("@PAIS", p),
                new SqlParameter("@PREFIX", Prefix)
            };
           
            List<STATE> estados = db.Database.SqlQuery<STATE>("CPS_LISTA_STATES @PAIS,@PREFIX",
          paramsCSP.ToArray()).ToList();
            return estados;

        }
	}
}