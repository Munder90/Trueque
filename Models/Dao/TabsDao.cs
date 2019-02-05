using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class TabsDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        
        public List<SelectListItem> ComboTabs(string spras_id, bool? activo, string id)
        {
            return db.TABs.Where(x => x.TAB_CAMPO.Any(y => y.ACTIVO == activo.Value || activo == null))
                .Where(x => (x.ACTIVO == activo.Value || activo == null))
                .Join(db.TEXTOes, ta => ta.ID, te => te.CAMPO_ID, (ta, te) => te)
                .Where(x => x.SPRAS_ID == spras_id && x.PAGINA_ID == 202 && (x.CAMPO_ID == id || id == null))
                .Select(x => new SelectListItem
                {
                    Value = x.CAMPO_ID,
                    Text = x.TEXTOS
                }).ToList();
        }
    }
}