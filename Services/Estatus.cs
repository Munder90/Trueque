using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class Estatus
    {

        public string getEstatus(DOCUMENTO d)
        {
            TruequeEntities db = new TruequeEntities();
            decimal doc = decimal.Parse(d.NUM_DOC.ToString());
            string estatus = "";
            ////bool rev = false;
            var estatusPar = (from docs in db.DOCUMENTOes where docs.DOCUMENTO_REF == doc select docs).ToList();
            if (d.ESTATUS != null) { estatus += d.ESTATUS; } else { estatus += " "; }
            if (d.ESTATUS_C != null) { estatus += d.ESTATUS_C; } else { estatus += " "; }
            if (d.ESTATUS_SAP != null) { estatus += d.ESTATUS_SAP; } else { estatus += " "; }
            if (d.ESTATUS_WF != null) { estatus += d.ESTATUS_WF; } else { estatus += " "; }
            ////foreach (var childDoc in estatusPar)
            ////{
            ////    if (childDoc.ESTATUS == "N" && childDoc.ESTATUS_C == null && childDoc.ESTATUS_SAP == null && childDoc.ESTATUS_WF == "P")
            ////    {
            ////        rev = false;
            ////    }
            ////}
            if (d.FLUJOes.Count > 0)
            {
                estatus += d.FLUJOes.OrderByDescending(a => a.POS).FirstOrDefault().WORKFP.ACCION.TIPO;
            }
            else
            {
                estatus += " ";
            }
            if (d.TSOL != null)
                if (d.TSOL.PADRE) { estatus += "P"; } else { estatus += " "; }
            else { estatus += " "; }
            if (d.FLUJOes.Where(x => x.ESTATUS == "R").ToList().Count > 0)
            {
                FLUJO flujo = d.FLUJOes.Where(x => x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault();
                estatus += db.USUARIOs.First(x => x.ID == flujo.USUARIOA_ID).PUESTO_ID;
            }
            else
            {
                estatus += " ";
            }
            if (d.DOCUMENTORECs.Count > 0)
            {
                estatus += "R";
            }
            else
            {
                estatus += " ";
            }
            if (estatusPar.Any(a => a.TSOL.REVERSO && a.DOCUMENTO_SAP != null))
                estatus += "1";
            else
                estatus += "0";
            return estatus;
        }

        public string getHtml(decimal num_doc)
        {
            TruequeEntities db = new TruequeEntities();

            List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO).ToList();

            DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);
            string estatus = getEstatus(d);

            string ret = "";
            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == "ES");
                        if (t != null)
                            ret = "<span class='" + e.CLASS + "' data-badge-caption=''>" + t.TXT050 + "</span>";
                        else
                            ret = "<span class='" + e.CLASS + "' data-badge-caption=''>" + e.DESCRIPCION + "</span>";
                        if (ret != "")
                            return ret;
                    }
                }
            }
            #region oculta1
            ////if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]"))
            ////    ret = "<span class='lbl_cancelled new badge red darken-1 white-text' data-badge-caption=' '>Cancelada</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R]..."))
            ////    ret = "<span class='lbl_ts yellow darken-2 white-text new badge' data-badge-caption=' '>Pendiente validación TS</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]..") && estatus.Contains("NCOp"))
            ////    ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>Pendiente reverso</span></span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A]..."))
            ////    ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>Pendiente aprobador</span></span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][E]..."))
            ////    ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>En espera</span></span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]....") | System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[P][P]..."))
            ////    ret = "<span class='lbl_txt new badge green darken-1 white-text' data-badge-caption=' '>Por gen.txt</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]...."))
            ////    ret = "<span class='lbl_txt new badge green darken-1 white-text' data-badge-caption=' '>Cerrada</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[0]"))
            ////    ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>Pendiente reverso</span></span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[1]"))
            ////    ret = "<span class='lbl_rev new badge green darken-1 white-text' data-badge-caption=' '>Reversado</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]...."))
            ////    ret = "<span class='lbl_contab new badge green darken-1 white-text' data-badge-caption=' '>Por contabilizar</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]...[R]"))
            ////    ret = "";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]....") | System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][F]...."))
            ////    ret = "<span class='lbl_txt new badge green darken-1 white-text' data-badge-caption=' '>Por contabilizar</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[E][A]...."))
            ////    ret = "<span class='lbl_txt new badge red darken-1 white-text' data-badge-caption=' '>Error en contabiización</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P].."))
            ////    ret = "<span class='new badge green darken-1 white-text' data-badge-caption=' '>@item.TSOL.TSOLTs.Where(x => x.SPRAS_ID.Equals(Session['spras'].ToString())).FirstOrDefault().TXT50<span class='lbl_approved'>Abierta</span></span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]...."))
            ////    ret = "<span class='new badge green darken-1 white-text' data-badge-caption=' '>Registrado SAP</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..[8]."))
            ////    ret = "<span class='new badge red darken-1 white-text' data-badge-caption=' '><span class='lbl_rejecte'>Pendiente corrección usuario TS</span> </span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]...."))
            ////    ret = "<span class='new badge red darken-1 white-text' data-badge-caption=' '><span class='lbl_rejected'>Pendiente corrección usuario</span></span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]...."))
            ////    ret = "<span class='lbl_soporte new badge yellow darken-2 white-text' data-badge-caption=' '>Pendiente firma</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]...."))
            ////    ret = "<span class='lbl_tax new badge yellow darken-2 white-text' data-badge-caption=' '>Pendiente tax</span>";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[B]...."))
            ////    ret = "<span class='lbl_borr new badge orange darken-2 white-text' data-badge-caption=' '>Borrador</span>";
            ////else
            ////    ret = "<td></td>";
            #endregion
            return ret;
        }

        public string getHtml(DOCUMENTO d)
        {
            ////TruequeEntities db = new TruequeEntities();
            ////DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);
            string estatus = getEstatus(d);
            string ret = "";

            if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]"))
                ret = "<span class='lbl_cancelled new badge red darken-1 white-text' data-badge-caption=' '>Cancelada</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R]..."))
                ret = "<span class='lbl_ts grey darken-2 white-text new badge' data-badge-caption=' '>Pendiente validación TS</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A]..."))
                ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>Pendiente aprobador</span></span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][E]..."))
                ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>En espera</span></span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]..") && estatus.Contains("NCOp"))
                ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>Pendiente reverso</span></span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]....") || System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[P][P]..."))
                ret = "<span class='lbl_txt new badge green darken-1 white-text' data-badge-caption=' '>Por gen.txt</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]...."))
                ret = "<span class='lbl_txt new badge green darken-1 white-text' data-badge-caption=' '>Cerrada</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[0]"))
                ret = "<span class='new badge grey darken-2 white-text' data-badge-caption=' '><span class='lbl_pending'>Pendiente reverso</span></span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[1]"))
                ret = "<span class='lbl_rev new badge green darken-1 white-text' data-badge-caption=' '>Reversado</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]...."))
                ret = "<span class='lbl_contab new badge green darken-1 white-text' data-badge-caption=' '>Por contabilizar</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]...[R]"))
                ret = "";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]....") || System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][F]...."))
                ret = "<span class='lbl_txt new badge green darken-1 white-text' data-badge-caption=' '>Por contabilizar</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[E][A]...."))
                ret = "<span class='lbl_txt new badge red darken-1 white-text' data-badge-caption=' '>Error en contabiización</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P].."))
                ret = "<span class='new badge green darken-1 white-text' data-badge-caption=' '>@item.TSOL.TSOLTs.Where(x => x.SPRAS_ID.Equals(Session['spras'].ToString())).FirstOrDefault().TXT50<span class='lbl_approved'>Abierta</span></span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]...."))
                ret = "<span class='new badge green darken-1 white-text' data-badge-caption=' '>Registrado SAP</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..[8]."))
                ret = "<span class='new badge red darken-1 white-text' data-badge-caption=' '><span class='lbl_rejecte'>Pendiente corrección usuario TS</span> </span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]...."))
                ret = "<span class='new badge red darken-1 white-text' data-badge-caption=' '><span class='lbl_rejected'>Pendiente corrección usuario</span></span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]...."))
                ret = "<span class='lbl_soporte new badge yellow darken-2 white-text' data-badge-caption=' '>Pendiente firma</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]...."))
                ret = "<span class='lbl_tax new badge yellow darken-2 white-text' data-badge-caption=' '>Pendiente tax</span>";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[B]...."))
                ret = "<span class='lbl_borr new badge orange darken-2 white-text' data-badge-caption=' '>Borrador</span>";
            else
                ret = "<td></td>";

            return ret;
        }
        public string getText(DOCUMENTO d, string spras)
        {
            TruequeEntities db = new TruequeEntities();

            ////DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);
            string estatus = getEstatus(d);
            List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO).ToList();
            string ret = "";

            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == spras);
                        if (t != null)
                            ret = t.TXT050;
                        else
                            ret = e.DESCRIPCION;
                        if (ret != "")
                            return ret;
                    }
                }
            }
            #region oculta2
            ////if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]"))
            ////    ret = "Cancelada ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R]..."))
            ////    ret = " Pendiente validación TS";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A]..."))
            ////    ret = "Pendiente aprobador";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][E]..."))
            ////    ret = "En espera";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]..") && estatus.Contains("NCOp"))
            ////    ret = "Pendiente reverso";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]....") | System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[P][P]..."))
            ////    ret = "Por gen.txt ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]...."))
            ////    ret = "Cerrada";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[0]"))
            ////    ret = "Pendiente reverso";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[1]"))
            ////    ret = "Reversado";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]...."))
            ////    ret = "Por contabilizar ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]...[R]"))
            ////    ret = "";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]....") | System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][F]...."))
            ////    ret = "Por contabilizar ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[E][A]...."))
            ////    ret = "Error en contabiización ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P].."))
            ////    ret = "abierta";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]...."))
            ////    ret = "Registrado SAP";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..[8]."))
            ////    ret = "Pendiente corrección usuario TS ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]...."))
            ////    ret = "Pendiente corrección usuario ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]...."))
            ////    ret = "Pendiente firma ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]...."))
            ////    ret = "Pendiente tax ";
            ////else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[B]...."))
            ////    ret = "Borrador";
            ////else
            ////    ret = " ";
            #endregion

            return ret;
        }
        public string getClass(DOCUMENTO d)
        {
            string estatus = getEstatus(d);
            string ret = "";

            if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]"))
                ret = "lbl_cancelled new badge red darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R]..."))
                ret = "lbl_ts yellow darken-2 white-text new badge";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A]..."))
                ret = "new badge grey darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][E]..."))
                ret = "new badge grey darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]..") && estatus.Contains("NCOp"))
                ret = "new badge grey darken-2 white - text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]....") || System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[P][P]..."))
                ret = "lbl_txt new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]...."))
                ret = "lbl_txt new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[0]"))
                ret = "new badge grey darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[1]"))
                ret = "lbl_rev new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]...."))
                ret = "lbl_contab new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]...R"))
                ret = "";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]....") || System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][F]...."))
                ret = "lbl_txt new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[E][A]...."))
                ret = "lbl_txt new badge red darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P].."))
                ret = "new badge green darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]...."))
                ret = "new badge green darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..[8]."))
                ret = "new badge red darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]...."))
                ret = "new badge red darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]...."))
                ret = "lbl_soporte new badge yellow darken-2 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]...."))
                ret = "lbl_tax new badge yellow darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[B]...."))
                ret = "lbl_borr new badge orange darken-2 white-text";
            else
                ret = " ";

            return ret;
        }

        public string getText(string estatus, decimal num_doc, string spras)
        {
            TruequeEntities db = new TruequeEntities();
            
            List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO).ToList();
            string ret = "";

            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == spras);
                        if (t != null)
                            ret = t.TXT050;
                        else
                            ret = e.DESCRIPCION;
                        if (ret != "")
                            return ret;
                    }
                }
            }
            return ret;
        }
        public string getClass(string estatus, decimal num_doc)
        {
            string ret = "";

            if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]"))
                ret = "lbl_cancelled new badge red darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R]..."))
                ret = "lbl_ts yellow darken-2 white-text new badge";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A]..."))
                ret = "new badge grey darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][E]..."))
                ret = "new badge grey darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]....") || System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[P][P]..."))
                ret = "lbl_txt new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]...."))
                ret = "lbl_txt new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[0]"))
                ret = "new badge grey darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[R]..[A]....[1]"))
                ret = "lbl_rev new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]...."))
                ret = "lbl_contab new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]...[R]"))
                ret = "";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]....") || System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][F]...."))
                ret = "lbl_txt new badge green darken-1 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[E][A]...."))
                ret = "lbl_txt new badge red darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P].."))
                ret = "new badge green darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]...."))
                ret = "new badge green darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..[8]."))
                ret = "new badge red darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]...."))
                ret = "new badge red darken-1 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]...."))
                ret = "lbl_soporte new badge yellow darken-2 white-text ";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]...."))
                ret = "lbl_tax new badge yellow darken-2 white-text";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[B]...."))
                ret = "lbl_borr new badge orange darken-2 white-text";
            else
                ret = " ";

            return ret;
        }

        public string getHtml(decimal num_doc, string spras, List<ESTATU> ee)
        {
            TruequeEntities db = new TruequeEntities();

            DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);
            string estatus = getEstatus(d);

            string ret = "";
            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == spras);
                        if (t != null)
                            ret = "<span class='" + e.CLASS + "' data-badge-caption=''>" + t.TXT050 + "</span>";
                        else
                            ret = "<span class='" + e.CLASS + "' data-badge-caption=''>" + e.DESCRIPCION + "</span>";
                        if (ret != "")
                            return ret;
                    }
                }
            }
            return ret;
        }
        public string getText(string estatus, decimal num_doc, string spras, List<ESTATU> ee)
        {
            string ret = "";

            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == spras);
                        if (t != null)
                            ret = t.TXT050;
                        else
                            ret = e.DESCRIPCION;
                        return ret;
                    }
                }
            }
            return ret;
        }
        public int getID(string estatus, decimal num_doc, string spras, List<ESTATU> ee)
        {
            int ret=0;

            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == spras);
                        if (t != null)
                            ret = t.ESTATUS_ID;
                        else
                            ret = e.ID;
                        return ret;
                    }
                }
            }
            return ret;
        }
        public string getClass(string estatus, decimal num_doc, string spras, List<ESTATU> ee)
        {
            string ret = "";

            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ret = e.CLASS;
                        return ret;
                    }
                }
            }
            return ret;
        }
        public string getText(decimal num_doc, string spras)
        {
            string ret = "";
            TruequeEntities db = new TruequeEntities();

            DOCUMENTO doc = db.DOCUMENTOes.Find(num_doc);
            string estatus = getEstatus(doc);
            List<ESTATU> ee = db.ESTATUS.Where(x => x.ACTIVO).ToList();

            foreach (ESTATU e in ee)
            {
                foreach (ESTATUSR reg in e.ESTATUSRs)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, reg.REGEX))
                    {
                        ESTATUST t = e.ESTATUSTs.FirstOrDefault(x => x.SPRAS_ID == spras);
                        if (t != null)
                            ret = t.TXT050;
                        else
                            ret = e.DESCRIPCION;
                        return ret;
                    }
                }
            }
            return ret;
        }
    }
}
