using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class UsuariosDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public List<USUARIO> ListaUsuarios(string prefix, int accion, string id=null, string sociedad_id = null)
        {
            if (prefix == null) { prefix = ""; }

            List<object> paramsCSP = new List<object>
            {
                new SqlParameter("@ACCION", accion),
                new SqlParameter("@PREFIX", prefix)
            };

            if (!string.IsNullOrEmpty(id)) { paramsCSP.Add(new SqlParameter("@ID", id)); }
            else { paramsCSP.Add(new SqlParameter("@ID", DBNull.Value)); }

            if (!string.IsNullOrEmpty(sociedad_id)) { paramsCSP.Add(new SqlParameter("@SOCIEDAD_ID", sociedad_id)); }
            else { paramsCSP.Add(new SqlParameter("@SOCIEDAD_ID", DBNull.Value)); }

            List<USUARIO> usuarios = db.Database.SqlQuery<USUARIO>("CPS_LISTA_USUARIOS @ACCION,@PREFIX,@ID,@SOCIEDAD_ID",
                paramsCSP.ToArray()).ToList();
            return usuarios;
        }

        public List<SelectListItem> ComboUsuarios(int accion, string id = null, string sociedad_id = null)
        {
            List<SelectListItem> usuarios = ListaUsuarios(null, accion,id,sociedad_id)
                            .Select(x => new SelectListItem
                            {
                                Value = x.ID,
                                Text = (x.ID + " - " + x.NOMBRE + " " + x.APELLIDO_P + " " + (x.APELLIDO_M ?? ""))
                            }).ToList();
            return usuarios;
        }
    }
}