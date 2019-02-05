using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TAT001.Common;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class MaterialesDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public  List<MATERIAL> ListaMateriales(string prefix, string vkorg, string vtweg, string user_id)
        {
            string spras_id = FnCommon.ObtenerSprasId(db, user_id);
            if (prefix == null) { prefix = ""; }

            List<MATERIAL> materiales = db.Database.SqlQuery<MATERIAL>("CPS_LISTA_MATERIALES @SPRAS_ID,@VKORG,@VTWEG,@PREFIX",
                new SqlParameter("@SPRAS_ID", spras_id),
                new SqlParameter("@VKORG", vkorg),
                new SqlParameter("@VTWEG", vtweg),
                new SqlParameter("@PREFIX", prefix)).ToList();

            return materiales;
        }
    }
}