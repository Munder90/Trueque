using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class ContactosDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public List<CONTACTOC> ListaContactos(string prefix, string vkorg, string vtweg, string kunnr)
        {
            if (prefix == null) { prefix = ""; }
            if(vkorg==null || vtweg == null && db.CLIENTEs.Any(x=>x.KUNNR==kunnr))
            {
                CLIENTE cliente = db.CLIENTEs.First(x => x.KUNNR == kunnr);
                vkorg = cliente.VKORG;
                vtweg = cliente.VTWEG;
            }

            List<object> paramsCSP = new List<object>
            {
                new SqlParameter("@KUNNR", kunnr),
                new SqlParameter("@VKORG", vkorg),
                new SqlParameter("@VTWEG", vtweg),
                new SqlParameter("@PREFIX", prefix)
            };

            List<CONTACTOC> contactos = db.Database.SqlQuery<CONTACTOC>("CPS_LISTA_CONTACTOS @KUNNR,@VKORG,@VTWEG,@PREFIX",
           paramsCSP.ToArray()).ToList();
            return contactos;
        }
    }
}