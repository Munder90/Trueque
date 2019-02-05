using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class ClientesDao
    {
        readonly TruequeEntities db = new TruequeEntities();

        public List<CLIENTE> ListaClientes(string prefix, string usuario_id, string pais)
        {
            if (prefix == null) { prefix = ""; }

            List<object> paramsCSP = new List<object>();

            if (!string.IsNullOrEmpty(usuario_id)) { paramsCSP.Add(new SqlParameter("@USUARIO_ID", usuario_id)); }
            else { paramsCSP.Add(new SqlParameter("@USUARIO_ID", DBNull.Value)); }

            if (!string.IsNullOrEmpty(pais)) { paramsCSP.Add(new SqlParameter("@PAIS", pais)); }
            else { paramsCSP.Add(new SqlParameter("@PAIS", DBNull.Value)); }

            paramsCSP.Add(new SqlParameter("@PREFIX", prefix));

            List<CLIENTE> clientes = db.Database.SqlQuery<CLIENTE>("CPS_LISTA_CLIENTES @USUARIO_ID,@PAIS,@PREFIX",
            paramsCSP.ToArray()).ToList();
            return clientes;
        }
    }
}