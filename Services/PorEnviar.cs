using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Common;
using TAT001.Entities;

namespace TAT001.Services
{

    class PorEnviar
    {
        public bool seEnvia(DOCUMENTO d, TruequeEntities db, Log log)
        {
            bool ret = false;
            DOCUMENTOA dz = null;
            var de = d.NUM_DOC;
            if (d.DOCUMENTO_REF == null && d.DOCUMENTOAs.Count > 0)
            {
                dz = d.DOCUMENTOAs.FirstOrDefault(x => x.NUM_DOC == de && x.CLASE == "OTR");
                if (dz == null && d.TSOL.NEGO)//para el ultimo filtro
                {
                    Estatus es = new Estatus();
                    string estatus = es.getEstatus(d);

                    List<int> ee = new List<int>();
                    ee.Add(20);//
                    ee.Add(90);//
                    ee.Add(100);//
                    ee.Add(110);//
                    ee.Add(120);//
                    ee.Add(130);//
                    ee.Add(160);//

                    List<ESTATUSR> ess = (from e in db.ESTATUSRs.ToList()
                                              ////join n in ee
                                              ////on e.ESTATUS_ID equals n
                                          select e).ToList();

                    foreach (ESTATUSR e in ess)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(estatus, e.REGEX))
                        {
                            if (ee.Contains(e.ESTATUS_ID))
                            {
                                ////log.escribeLog("NUM_DOC: " + d.NUM_DOC);
                                ret = true;
                            }
                            break;
                        }
                    }
                }
            }
            return ret;
        }
    }
}