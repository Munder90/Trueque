using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class PeriodosDao
    {
        readonly TruequeEntities db = new TruequeEntities();

        public  List<PERIODOT> ListaPeriodos(  string spras_id, int? id)
        {
            return db.PERIODOes
                .Join(db.PERIODOTs, p => p.ID, pt => pt.PERIODO_ID, (p, pt) => pt)
                .Where(x => x.SPRAS_ID == spras_id && (x.PERIODO_ID == id || id == null)).ToList();
        }
        public  List<SelectListItem> ComboPeriodos(string spras_id, int? id)
        {
            return ListaPeriodos(spras_id,id)
                .Select(x => new SelectListItem
                {
                    Value = x.PERIODO_ID.ToString(),
                    Text = (x.PERIODO_ID.ToString() + " - " + x.TXT50)
                }).ToList();
        }
    }
}