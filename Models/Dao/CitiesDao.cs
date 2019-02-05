using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class CitiesDao
    {
        readonly TruequeEntities db = new TruequeEntities();

        public List<CITy> ListaCities(string Prefix, string estado)
        {
            if (Prefix == null) { Prefix = ""; }

            List<object> paramsCSP = new List<object>
            {
                new SqlParameter("@ESTADO", estado),
                new SqlParameter("@PREFIX", Prefix)
            };

            List<CITy> ciudades = db.Database.SqlQuery<CITy>("CPS_LISTA_CITIES @ESTADO,@PREFIX",
          paramsCSP.ToArray()).ToList();
            return ciudades;
        }
    }
}