using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class SociedadesDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public List<SOCIEDAD> ListaSociedades(int accion,string bukrs=null, string usuario_id=null, string Prefix = null)
        {
            if (Prefix == null) { Prefix = ""; }

            List<object> paramsCSP = new List<object>
            {
                new SqlParameter("@ACCION", accion),
                new SqlParameter("@PREFIX", Prefix)
            };

            if (!string.IsNullOrEmpty(bukrs)) { paramsCSP.Add(new SqlParameter("@SOCIEDAD_ID", bukrs)); }
            else { paramsCSP.Add(new SqlParameter("@SOCIEDAD_ID", DBNull.Value)); }

            if (!string.IsNullOrEmpty(usuario_id)) { paramsCSP.Add(new SqlParameter("@USUARIO_ID", usuario_id)); }
            else { paramsCSP.Add(new SqlParameter("@USUARIO_ID", DBNull.Value)); }

            List<SOCIEDAD> sociedades = db.Database.SqlQuery<SOCIEDAD>("CPS_LISTA_SOCIEDADES @ACCION,@PREFIX,@SOCIEDAD_ID,@USUARIO_ID",
          paramsCSP.ToArray()).ToList();
            return sociedades;
        
        }
        public  List<SelectListItem> ComboSociedades(int accion, string bukrs = null, string usuario_id = null)
        {
            return ListaSociedades(accion,bukrs,usuario_id)
                .Select(x => new SelectListItem
                {
                    Value = x.BUKRS,
                    Text = x.BUKRS
                }).ToList();
        }
    }
}