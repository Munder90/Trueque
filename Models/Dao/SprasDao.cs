using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class SprasDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public List<SPRA> ListaSpras()
        {
            List<SPRA> spras = db.Database.SqlQuery<SPRA>("CPS_LISTA_SPRAS").ToList();
            return spras;

        }


        public List<SelectListItem> ComboSpras()
        {
            return ListaSpras()
                .Select(x => new SelectListItem
                {
                    Value = x.ID,
                    Text = x.ID+" - "+x.DESCRIPCION
                }).ToList();
        }
    }
}