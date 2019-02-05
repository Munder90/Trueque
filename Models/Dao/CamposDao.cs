using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Models.Dao
{
    public class CamposDao
    {
        readonly TruequeEntities db = new TruequeEntities();
        public List<SelectListItem> ComboCamposPorTabId(string spras_id, string tab_id, bool? activo, string id)
        {
            return db.TAB_CAMPO
                    .Where(x => x.TAB_ID == tab_id && (x.ACTIVO == activo.Value || activo == null))
                    .Join(db.TEXTOes, tc => tc.CAMPO_ID, te => te.CAMPO_ID, (ta, te) => te)
                    .Where(x => x.SPRAS_ID == spras_id && x.PAGINA_ID == 202 && (x.CAMPO_ID == id || id == null))
                    .Select(x => new SelectListItem
                    {
                        Value = x.CAMPO_ID,
                        Text = x.TEXTOS
                    }).ToList();
        }
    }
}