using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models
{
    public class ReportesModel
    {
        public class TrackingTS
        {
            public FLUJO f { get; set; }
            public int WF_POS { get; set; }
            public decimal NUMERO_SOLICITUD { get; set; }
            public string CO_CODE { get; set; }
            public string PAIS { get; set; }
            public string NUMERO_CLIENTE { get; set; }
            public string CLIENTE { get; set; }
            public string TIPO_SOLICITUD { get; set; }
            public string ESTATUS { get; set; }
            public string ESTATUS_C { get; set; }
            public string ESTATUS_SAP { get; set; }
            public string ESTATUS_WF { get; set; }
            public string TIPO { get; set; }
            public bool PADRE { get; set; }
            public DateTime FECHA { get; set; }
            public int PERIODO { get; set; }
            public string ANIO { get; set; }
            public string USUARIO { get; set; }
            public string COMENTARIO { get; set; }
            public string ROL { get; set; }
            public double TIEMPO_TRANSCURRIDO { get; set; }
            public int SEMANA { get; set; }
            public int NUMERO_CORRECCIONES { get; set; }
            public string STATUS_STRING
            {
                get
                {
                    string estatus = "";

                    // FORMAR LA CADENA DE STATUS PARA IDENTIFICAR EL MENSAJE A DESPLEGAR
                    if (ESTATUS != null) { estatus += ESTATUS; } else { estatus += " "; }
                    if (ESTATUS_C != null) { estatus += ESTATUS_C; } else { estatus += " "; }
                    if (ESTATUS_SAP != null) { estatus += ESTATUS_SAP; } else { estatus += " "; }
                    if (ESTATUS_WF != null) { estatus += ESTATUS_WF; } else { estatus += " "; }
                    if (TIPO != null) { estatus += TIPO; } else { estatus += " "; }
                    if (PADRE) { estatus += "P"; } else { estatus += " "; }

                    // ESTABLECER EL MENSAJE DE STATUS QUE SE MOSTRARÁ
                    if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]")) { return "Cancelada"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R].")) { return "Pendiente validación TS"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A].")) { return "Pendiente aprobador"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]..")) { return "Por gen .txt"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]..")) { return "Por contabilizar"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]..")) { return "Por gen .txt"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]..")) { return "Por contabilizar"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[X][A]..")) { return "Error en contabilización"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]")) { return TIPO_SOLICITUD + "Abierta"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]..")) { return "Registrada en SAP"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..")) { return "Pendiente corrección usuario"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R][S].")) { return "Pendiente corrección usuario TS"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]..")) { return "Pendiente firma"; }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]..")) { return "Pendiente tax"; }
                    else { return ""; }
                }
            }

            public string FECHA_STRING
            {
                get
                {
                    return FECHA.ToString("dd/MM/yyyy");
                }
            }
            public string HORA_STRING
            {
                get
                {
                    return FECHA.ToString("HH:mm:ss");
                }
            }
            public int TIEMPO_TRANSCURRIDO_STRING
            {
                get
                {
                    return Convert.ToInt32(TIEMPO_TRANSCURRIDO);
                }
            }
            public string NUMERO_CORRECCIONES_STRING
            {
                get
                {
                    return (NUMERO_CORRECCIONES == 0) ? "" : NUMERO_CORRECCIONES.ToString();
                }
            }

            public int POS { get; internal set; }
            public DateTime? FECHA_SOLICITUD { get; internal set; }
            public string USUARIO_ACCION { get; internal set; }
        }

        public class MRLTS
        {
            public DOCUMENTO d;

            public string CO_CODE { get; set; }
            public string PAIS { get; set; }
            public decimal NUMERO_SOLICITUD { get; set; }
            public DateTime FECHA_SOLICITUD { get; set; }
            public int PERIODO_CONTABLE { get; set; }
            public string ANIO_CONTABLE { get; internal set; }
            public string NUMERO_DOCUMENTO_SAP { get; set; }
            public DateTime FECHA_REVERSO { get; set; }
            public string PERIODO_CONTABLE_REVERSO { get; set; }
            public string COMENTARIOS_REVERSO_PROVISION { get; set; }
            public string TIPO_SOLICITUD { get; set; }
            public string STATUS { get; set; }
            public string CONCEPTO_SOLICITUD { get; set; }
            public DateTime DE { get; set; }
            public DateTime A { get; set; }
            public string CLASIFICACION { get; set; }
            public string NUMERO_CLIENTE { get; set; }
            public string CLIENTE { get; set; }
            public decimal MONTO { get; set; }
            public string MONEDA { get; set; }
            public decimal TIPO_CAMBIO { get; set; }
            public string TIPO_SOLICITUD_ID { get; internal set; }
            public string TIPO_SOLICITUD_CODE { get; internal set; }
            public string STATUSS1 { get; internal set; }
            public string STATUSS2 { get; internal set; }
            public string STATUSS3 { get; internal set; }
            public string STATUSS4 { get; internal set; }
            public string STATUSS { get; internal set; }
            public string ESTATUS_STRING { get; internal set; }
            public decimal MONTO_2 { get; internal set; }
            public decimal EXPENSE_RECOGNITION { get; internal set; }
            public bool ES_REVERSO { get; internal set; }
            public string BENEFICIO_IMPACTO_MRL { get; set; }
            public string BENEFICIO_IMPACTO_MRL_USD { get; set; }
            public string NUMERO_REVERSO_SAP { get; set; }

            public string PERIODO_CONTABLE_STRING
            {
                get
                {
                    return this.PERIODO_CONTABLE.ToString() + "-" + this.ANIO_CONTABLE.ToString();
                }
            }
            public string COMENTARIOS_REVERSO_PROVISION_STRING
            {
                get
                {
                    if (this.ES_REVERSO)
                        return this.COMENTARIOS_REVERSO_PROVISION;
                    else
                        return String.Empty;
                }
            }
            public string FECHA_SOLICITUD_STRING
            {
                get
                {
                    return this.FECHA_SOLICITUD.ToShortDateString();
                }
            }
            public string FECHA_REVERSO_STRING
            {
                get
                {
                    if (this.ES_REVERSO)
                        return this.FECHA_REVERSO.ToShortDateString();
                    else
                        return String.Empty;
                }
            }
            public string DE_STRING
            {
                get
                {
                    return this.DE.ToString("MM-yyyy");
                }
            }
            public string A_STRING
            {
                get
                {
                    return this.A.ToString("MM-yyyy");
                }
            }
            public string MONTO_PROVISION_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("PR"))
                        return String.Format("{0:C}", this.MONTO);
                    else
                        return String.Empty;
                }
            }
            public string MONTO_NCOP_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("NC") || this.TIPO_SOLICITUD_ID.StartsWith("OP"))
                        return String.Format("{0:C}", this.MONTO);
                    else
                        return String.Empty;
                }
            }
            public string MONTO_REVERSO_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("RP"))
                        return String.Format("{0:C}", this.MONTO);
                    else
                        return String.Empty;
                }
            }
            public string EXPENSE_RECOGNITION_STRING
            {
                get
                {
                    if (this.EXPENSE_RECOGNITION > 0)
                        return String.Format("{0:C}", this.EXPENSE_RECOGNITION);
                    else
                        return String.Empty;
                }
            }
            public string MONTO_PROVISION_USD_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("PR"))
                        return String.Format("{0:C}", this.MONTO_2);
                    else
                        return String.Empty;
                }
            }
            public string MONTO_NCOP_USD_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("NC") || this.TIPO_SOLICITUD_ID.StartsWith("OP"))
                        return String.Format("{0:C}", this.MONTO_2);
                    else
                        return String.Empty;
                }
            }
            public string MONTO_REVERSO_USD_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("RP"))
                        return String.Format("{0:C}", this.MONTO_2);
                    else
                        return String.Empty;
                }
            }
            public string BENEFICIO_IMPACTO_MRL_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("PR"))
                        return String.Format("{0:C}", this.MONTO);
                    else if (this.TIPO_SOLICITUD_ID.StartsWith("NC") || this.TIPO_SOLICITUD_ID.StartsWith("OP"))
                        return "(" + String.Format("{0:C}", this.MONTO) + ")";
                    else if (this.TIPO_SOLICITUD_ID.StartsWith("RP"))
                        return "(" + String.Format("{0:C}", this.MONTO) + ")";
                    else
                        return String.Empty;
                }
            }
            public string BENEFICIO_IMPACTO_MRL_USD_STRING
            {
                get
                {
                    if (this.TIPO_SOLICITUD_ID.StartsWith("PR"))
                        return String.Format("{0:C}", this.MONTO_2);
                    else if (this.TIPO_SOLICITUD_ID.StartsWith("NC") || this.TIPO_SOLICITUD_ID.StartsWith("OP"))
                        return "(" + String.Format("{0:C}", this.MONTO_2) + ")";
                    else if (this.TIPO_SOLICITUD_ID.StartsWith("RP"))
                        return "(" + String.Format("{0:C}", this.MONTO_2) + ")";
                    else
                        return String.Empty;
                }
            }
        }

        public class AllowancesB
        {
            public string CUENTA_DE_BALANCE { get; set; }
            public string DESCRIPCION { get; set; }
            public string FUENTE { get; set; }
            public decimal KCMX { get; set; }
            public decimal KLCA { get; set; }
            public decimal LCCR { get; set; }
            public decimal LPKP { get; set; }
            public decimal KLSV { get; set; }
            public decimal KCAR { get; set; }
            public decimal KPRS { get; set; }
            public decimal KLCO { get; set; }
            public decimal LEKE { get; set; }
            public decimal LAGA { get; set; }
            public decimal KLCH { get; set; }
            public bool IS_DOLAR { get; set; }
            public decimal KCMX_USD { get; set; }
            public decimal KLCA_USD { get; set; }
            public decimal LCCR_USD { get; set; }
            public decimal LPKP_USD { get; set; }
            public decimal KLSV_USD { get; set; }
            public decimal KCAR_USD { get; set; }
            public decimal KPRS_USD { get; set; }
            public decimal KLCO_USD { get; set; }
            public decimal LEKE_USD { get; set; }
            public decimal LAGA_USD { get; set; }
            public decimal KLCH_USD { get; set; }
            public int ORDEN { get; internal set; }
            public string KCMX_STRING { get { return ((this.KCMX == 0) ? "-" : String.Format("{0:C}", (this.KCMX))); } }
            public string KLCA_STRING { get { return ((this.KLCA == 0) ? "-" : String.Format("{0:C}", (this.KLCA))); } }
            public string LCCR_STRING { get { return ((this.LCCR == 0) ? "-" : String.Format("{0:C}", (this.LCCR))); } }
            public string LPKP_STRING { get { return ((this.LPKP == 0) ? "-" : String.Format("{0:C}", (this.LPKP))); } }
            public string KLSV_STRING { get { return ((this.KLSV == 0) ? "-" : String.Format("{0:C}", (this.KLSV))); } }
            public string KCAR_STRING { get { return ((this.KCAR == 0) ? "-" : String.Format("{0:C}", (this.KCAR))); } }
            public string KPRS_STRING { get { return ((this.KPRS == 0) ? "-" : String.Format("{0:C}", (this.KPRS))); } }
            public string KLCO_STRING { get { return ((this.KLCO == 0) ? "-" : String.Format("{0:C}", (this.KLCO))); } }
            public string LEKE_STRING { get { return ((this.LEKE == 0) ? "-" : String.Format("{0:C}", (this.LEKE))); } }
            public string LAGA_STRING { get { return ((this.LAGA == 0) ? "-" : String.Format("{0:C}", (this.LAGA))); } }
            public string KLCH_STRING { get { return ((this.KLCH == 0) ? "-" : String.Format("{0:C}", (this.KLCH))); } }
            public string KCMX_USD_STRING { get { return ((this.KCMX_USD == 0) ? "-" : String.Format("{0:C}", (this.KCMX_USD))); } }
            public string KLCA_USD_STRING { get { return ((this.KLCA_USD == 0) ? "-" : String.Format("{0:C}", (this.KLCA_USD))); } }
            public string LCCR_USD_STRING { get { return ((this.LCCR_USD == 0) ? "-" : String.Format("{0:C}", (this.LCCR_USD))); } }
            public string LPKP_USD_STRING { get { return ((this.LPKP_USD == 0) ? "-" : String.Format("{0:C}", (this.LPKP_USD))); } }
            public string KLSV_USD_STRING { get { return ((this.KLSV_USD == 0) ? "-" : String.Format("{0:C}", (this.KLSV_USD))); } }
            public string KCAR_USD_STRING { get { return ((this.KCAR_USD == 0) ? "-" : String.Format("{0:C}", (this.KCAR_USD))); } }
            public string KPRS_USD_STRING { get { return ((this.KPRS_USD == 0) ? "-" : String.Format("{0:C}", (this.KPRS_USD))); } }
            public string KLCO_USD_STRING { get { return ((this.KLCO_USD == 0) ? "-" : String.Format("{0:C}", (this.KLCO_USD))); } }
            public string LEKE_USD_STRING { get { return ((this.LEKE_USD == 0) ? "-" : String.Format("{0:C}", (this.LEKE_USD))); } }
            public string LAGA_USD_STRING { get { return ((this.LAGA_USD == 0) ? "-" : String.Format("{0:C}", (this.LAGA_USD))); } }
            public string KLCH_USD_STRING { get { return ((this.KLCH_USD == 0) ? "-" : String.Format("{0:C}", (this.KLCH_USD))); } }
        }

        public class AllowancesPL
        {
            public string PERIODO { get; set; }
            public string YEAR { get; set; }
            public string BU { get; set; }
            public string PAYER { get; set; }
            public string CLIENTE { get; set; }
            public string CANAL { get; set; }
            public string CATEGORIA { get; set; }
            public decimal CD_EN_PROCESO { get; set; }
            public decimal CD_ALLOWANCE_TAT { get; set; }
            public decimal CD_ALLOWANCE_FACT { get; set; }
            public decimal CD_AJUSTES { get; set; }
            public decimal CD_TOTALES { get { return CD_EN_PROCESO + CD_ALLOWANCE_TAT + CD_ALLOWANCE_FACT + CD_AJUSTES; } }
            public decimal C_EN_PROCESO { get; set; }
            public decimal C_ALLOWANCE_TAT { get; set; }
            public decimal C_ALLOWANCE_FACT { get; set; }
            public decimal C_AJUSTES { get; set; }
            public decimal C_TOTALES { get { return C_EN_PROCESO + C_ALLOWANCE_TAT + C_ALLOWANCE_FACT + C_AJUSTES; } }
            public decimal COD_EN_PROCESO { get; set; }
            public decimal COD_ALLOWANCE_TAT { get; set; }
            public decimal COD_ALLOWANCE_FACT { get; set; }
            public decimal COD_AJUSTES { get; set; }
            public decimal COD_TOTALES { get { return COD_EN_PROCESO + COD_ALLOWANCE_TAT + COD_ALLOWANCE_FACT + COD_AJUSTES; } }
            public decimal DPS_EN_PROCESO { get; set; }
            public decimal DPS_ALLOWANCE_TAT { get; set; }
            public decimal DPS_ALLOWANCE_FACT { get; set; }
            public decimal DPS_AJUSTES { get; set; }
            public decimal DPS_TOTALES { get { return DPS_EN_PROCESO + DPS_ALLOWANCE_TAT + DPS_ALLOWANCE_FACT + DPS_AJUSTES; } }
            public decimal DC_EN_PROCESO { get; set; }
            public decimal DC_ALLOWANCE_TAT { get; set; }
            public decimal DC_ALLOWANCE_FACT { get; set; }
            public decimal DC_AJUSTES { get; set; }
            public decimal DC_TOTALES { get { return DC_EN_PROCESO + DC_ALLOWANCE_TAT + DC_ALLOWANCE_FACT + DC_AJUSTES; } }
            public decimal ELP_EN_PROCESO { get; set; }
            public decimal ELP_ALLOWANCE_TAT { get; set; }
            public decimal ELP_ALLOWANCE_FACT { get; set; }
            public decimal ELP_AJUSTES { get; set; }
            public decimal ELP_TOTALES { get { return ELP_EN_PROCESO + ELP_ALLOWANCE_TAT + ELP_ALLOWANCE_FACT + ELP_AJUSTES; } }
            public decimal FG_EN_PROCESO { get; set; }
            public decimal FG_ALLOWANCE_TAT { get; set; }
            public decimal FG_ALLOWANCE_FACT { get; set; }
            public decimal FG_AJUSTES { get; set; }
            public decimal FG_TOTALES { get { return FG_EN_PROCESO + FG_ALLOWANCE_TAT + FG_ALLOWANCE_FACT + FG_AJUSTES; } }
            public decimal GD_EN_PROCESO { get; set; }
            public decimal GD_ALLOWANCE_TAT { get; set; }
            public decimal GD_ALLOWANCE_FACT { get; set; }
            public decimal GD_AJUSTES { get; set; }
            public decimal GD_TOTALES { get { return GD_EN_PROCESO + GD_ALLOWANCE_TAT + GD_ALLOWANCE_FACT + GD_AJUSTES; } }
            public decimal GP_EN_PROCESO { get; set; }
            public decimal GP_ALLOWANCE_TAT { get; set; }
            public decimal GP_ALLOWANCE_FACT { get; set; }
            public decimal GP_AJUSTES { get; set; }
            public decimal GP_TOTALES { get { return GP_EN_PROCESO + GP_ALLOWANCE_TAT + GP_ALLOWANCE_FACT + GP_AJUSTES; } }
            public decimal LD_EN_PROCESO { get; set; }
            public decimal LD_ALLOWANCE_TAT { get; set; }
            public decimal LD_ALLOWANCE_FACT { get; set; }
            public decimal LD_AJUSTES { get; set; }
            public decimal LD_TOTALES { get { return LD_EN_PROCESO + LD_ALLOWANCE_TAT + LD_ALLOWANCE_FACT + LD_AJUSTES; } }
            public decimal MS_EN_PROCESO { get; set; }
            public decimal MS_ALLOWANCE_TAT { get; set; }
            public decimal MS_ALLOWANCE_FACT { get; set; }
            public decimal MS_AJUSTES { get; set; }
            public decimal MS_TOTALES { get { return MS_EN_PROCESO + MS_ALLOWANCE_TAT + MS_ALLOWANCE_FACT + MS_AJUSTES; } }
            public decimal R_EN_PROCESO { get; set; }
            public decimal R_ALLOWANCE_TAT { get; set; }
            public decimal R_ALLOWANCE_FACT { get; set; }
            public decimal R_AJUSTES { get; set; }
            public decimal R_TOTALES { get { return R_EN_PROCESO + R_ALLOWANCE_TAT + R_ALLOWANCE_FACT + R_AJUSTES; } }
            public decimal TP_EN_PROCESO { get; set; }
            public decimal TP_ALLOWANCE_TAT { get; set; }
            public decimal TP_ALLOWANCE_FACT { get; set; }
            public decimal TP_AJUSTES { get; set; }
            public decimal TP_TOTALES { get { return TP_EN_PROCESO + TP_ALLOWANCE_TAT + TP_ALLOWANCE_FACT + TP_AJUSTES; } }
            public decimal SIS_EN_PROCESO { get; set; }
            public decimal SIS_ALLOWANCE_TAT { get; set; }
            public decimal SIS_ALLOWANCE_FACT { get; set; }
            public decimal SIS_AJUSTES { get; set; }
            public decimal SIS_TOTALES { get { return SIS_EN_PROCESO + SIS_ALLOWANCE_TAT + SIS_ALLOWANCE_FACT + SIS_AJUSTES; } }
            public decimal SO_EN_PROCESO { get; set; }
            public decimal SO_ALLOWANCE_TAT { get; set; }
            public decimal SO_ALLOWANCE_FACT { get; set; }
            public decimal SO_AJUSTES { get; set; }
            public decimal SO_TOTALES { get { return SO_EN_PROCESO + SO_ALLOWANCE_TAT + SO_ALLOWANCE_FACT + SO_AJUSTES; } }
            public decimal TPIS_EN_PROCESO { get; set; }
            public decimal TPIS_ALLOWANCE_TAT { get; set; }
            public decimal TPIS_ALLOWANCE_FACT { get; set; }
            public decimal TPIS_AJUSTES { get; set; }
            public decimal TPIS_TOTALES { get { return TPIS_EN_PROCESO + TPIS_ALLOWANCE_TAT + TPIS_ALLOWANCE_FACT + TPIS_AJUSTES; } }
            public decimal TPO_EN_PROCESO { get; set; }
            public decimal TPO_ALLOWANCE_TAT { get; set; }
            public decimal TPO_ALLOWANCE_FACT { get; set; }
            public decimal TPO_AJUSTES { get; set; }
            public decimal TPO_TOTALES { get { return TPO_EN_PROCESO + TPO_ALLOWANCE_TAT + TPO_ALLOWANCE_FACT + TPO_AJUSTES; } }
            public decimal U_EN_PROCESO { get; set; }
            public decimal U_ALLOWANCE_TAT { get; set; }
            public decimal U_ALLOWANCE_FACT { get; set; }
            public decimal U_AJUSTES { get; set; }
            public decimal U_TOTALES { get { return U_EN_PROCESO + U_ALLOWANCE_TAT + U_ALLOWANCE_FACT + U_AJUSTES; } }
            public decimal WA_EN_PROCESO { get; set; }
            public decimal WA_ALLOWANCE_TAT { get; set; }
            public decimal WA_ALLOWANCE_FACT { get; set; }
            public decimal WA_AJUSTES { get; set; }
            public decimal WA_TOTALES { get { return WA_EN_PROCESO + WA_ALLOWANCE_TAT + WA_ALLOWANCE_FACT + WA_AJUSTES; } }
            public decimal PROCESO_TAT_TOTAL { get { return CD_EN_PROCESO + C_EN_PROCESO + COD_EN_PROCESO + DPS_EN_PROCESO + DC_EN_PROCESO + ELP_EN_PROCESO + FG_EN_PROCESO + GD_EN_PROCESO + GP_EN_PROCESO + LD_EN_PROCESO + MS_EN_PROCESO + R_EN_PROCESO + TP_EN_PROCESO + SIS_EN_PROCESO + SO_EN_PROCESO + TPIS_EN_PROCESO + TPO_EN_PROCESO + U_EN_PROCESO; } }
            public decimal ALLOWANCE_TAT_TOTAL { get { return CD_ALLOWANCE_TAT + C_ALLOWANCE_TAT + COD_ALLOWANCE_TAT + DPS_ALLOWANCE_TAT + DC_ALLOWANCE_TAT + ELP_ALLOWANCE_TAT + FG_ALLOWANCE_TAT + GD_ALLOWANCE_TAT + GP_ALLOWANCE_TAT + LD_ALLOWANCE_TAT + MS_ALLOWANCE_TAT + R_ALLOWANCE_TAT + TP_ALLOWANCE_TAT + SIS_ALLOWANCE_TAT + SO_ALLOWANCE_TAT + TPIS_ALLOWANCE_TAT + TPO_ALLOWANCE_TAT + U_ALLOWANCE_TAT; } }
            public decimal ALLOWANCE_FACT_TOTAL { get { return CD_ALLOWANCE_FACT + C_ALLOWANCE_FACT + COD_ALLOWANCE_FACT + DPS_ALLOWANCE_FACT + DC_ALLOWANCE_FACT + ELP_ALLOWANCE_FACT + FG_ALLOWANCE_FACT + GD_ALLOWANCE_FACT + GP_ALLOWANCE_FACT + LD_ALLOWANCE_FACT + MS_ALLOWANCE_FACT + R_ALLOWANCE_FACT + TP_ALLOWANCE_FACT + SIS_ALLOWANCE_FACT + SO_ALLOWANCE_FACT + TPIS_ALLOWANCE_FACT + TPO_ALLOWANCE_FACT + U_ALLOWANCE_FACT; } }
            public decimal AJUSTES { get { return CD_AJUSTES + C_AJUSTES + COD_AJUSTES + DPS_AJUSTES + DC_AJUSTES + ELP_AJUSTES + FG_AJUSTES + GD_AJUSTES + GP_AJUSTES + LD_AJUSTES + MS_AJUSTES + R_AJUSTES + TP_AJUSTES + SIS_AJUSTES + SO_AJUSTES + TPIS_AJUSTES + TPO_AJUSTES + U_AJUSTES; } }
            public decimal ALLOWANCE_TOTALES { get { return PROCESO_TAT_TOTAL + ALLOWANCE_TAT_TOTAL + ALLOWANCE_FACT_TOTAL + AJUSTES; } }
            public string CD_EN_PROCESO_STRING { get { return ((this.CD_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.CD_EN_PROCESO))); } }
            public string CD_ALLOWANCE_TAT_STRING { get { return ((this.CD_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.CD_ALLOWANCE_TAT))); } }
            public string CD_ALLOWANCE_FACT_STRING { get { return ((this.CD_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.CD_ALLOWANCE_FACT))); } }
            public string CD_AJUSTES_STRING { get { return ((this.CD_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.CD_AJUSTES))); } }
            public string CD_TOTALES_STRING { get { return ((this.CD_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.CD_TOTALES))); } }
            public string C_EN_PROCESO_STRING { get { return ((this.C_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.C_EN_PROCESO))); } }
            public string C_ALLOWANCE_TAT_STRING { get { return ((this.C_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.C_ALLOWANCE_TAT))); } }
            public string C_ALLOWANCE_FACT_STRING { get { return ((this.C_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.C_ALLOWANCE_FACT))); } }
            public string C_AJUSTES_STRING { get { return ((this.C_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.C_AJUSTES))); } }
            public string C_TOTALES_STRING { get { return ((this.C_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.C_TOTALES))); } }
            public string COD_EN_PROCESO_STRING { get { return ((this.COD_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.COD_EN_PROCESO))); } }
            public string COD_ALLOWANCE_TAT_STRING { get { return ((this.COD_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.COD_ALLOWANCE_TAT))); } }
            public string COD_ALLOWANCE_FACT_STRING { get { return ((this.COD_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.COD_ALLOWANCE_FACT))); } }
            public string COD_AJUSTES_STRING { get { return ((this.COD_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.COD_AJUSTES))); } }
            public string COD_TOTALES_STRING { get { return ((this.COD_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.COD_TOTALES))); } }
            public string DPS_EN_PROCESO_STRING { get { return ((this.DPS_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.DPS_EN_PROCESO))); } }
            public string DPS_ALLOWANCE_TAT_STRING { get { return ((this.DPS_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.DPS_ALLOWANCE_TAT))); } }
            public string DPS_ALLOWANCE_FACT_STRING { get { return ((this.DPS_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.DPS_ALLOWANCE_FACT))); } }
            public string DPS_AJUSTES_STRING { get { return ((this.DPS_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.DPS_AJUSTES))); } }
            public string DPS_TOTALES_STRING { get { return ((this.DPS_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.DPS_TOTALES))); } }
            public string DC_EN_PROCESO_STRING { get { return ((this.DC_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.DC_EN_PROCESO))); } }
            public string DC_ALLOWANCE_TAT_STRING { get { return ((this.DC_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.DC_ALLOWANCE_TAT))); } }
            public string DC_ALLOWANCE_FACT_STRING { get { return ((this.DC_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.DC_ALLOWANCE_FACT))); } }
            public string DC_AJUSTES_STRING { get { return ((this.DC_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.DC_AJUSTES))); } }
            public string DC_TOTALES_STRING { get { return ((this.DC_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.DC_TOTALES))); } }
            public string ELP_EN_PROCESO_STRING { get { return ((this.ELP_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.ELP_EN_PROCESO))); } }
            public string ELP_ALLOWANCE_TAT_STRING { get { return ((this.ELP_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.ELP_ALLOWANCE_TAT))); } }
            public string ELP_ALLOWANCE_FACT_STRING { get { return ((this.ELP_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.ELP_ALLOWANCE_FACT))); } }
            public string ELP_AJUSTES_STRING { get { return ((this.ELP_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.ELP_AJUSTES))); } }
            public string ELP_TOTALES_STRING { get { return ((this.ELP_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.ELP_TOTALES))); } }
            public string FG_EN_PROCESO_STRING { get { return ((this.FG_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.FG_EN_PROCESO))); } }
            public string FG_ALLOWANCE_TAT_STRING { get { return ((this.FG_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.FG_ALLOWANCE_TAT))); } }
            public string FG_ALLOWANCE_FACT_STRING { get { return ((this.FG_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.FG_ALLOWANCE_FACT))); } }
            public string FG_AJUSTES_STRING { get { return ((this.FG_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.FG_AJUSTES))); } }
            public string FG_TOTALES_STRING { get { return ((this.FG_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.FG_TOTALES))); } }
            public string GD_EN_PROCESO_STRING { get { return ((this.GD_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.GD_EN_PROCESO))); } }
            public string GD_ALLOWANCE_TAT_STRING { get { return ((this.GD_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.GD_ALLOWANCE_TAT))); } }
            public string GD_ALLOWANCE_FACT_STRING { get { return ((this.GD_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.GD_ALLOWANCE_FACT))); } }
            public string GD_AJUSTES_STRING { get { return ((this.GD_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.GD_AJUSTES))); } }
            public string GD_TOTALES_STRING { get { return ((this.GD_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.GD_TOTALES))); } }
            public string GP_EN_PROCESO_STRING { get { return ((this.GP_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.GP_EN_PROCESO))); } }
            public string GP_ALLOWANCE_TAT_STRING { get { return ((this.GP_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.GP_ALLOWANCE_TAT))); } }
            public string GP_ALLOWANCE_FACT_STRING { get { return ((this.GP_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.GP_ALLOWANCE_FACT))); } }
            public string GP_AJUSTES_STRING { get { return ((this.GP_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.GP_AJUSTES))); } }
            public string GP_TOTALES_STRING { get { return ((this.GP_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.GP_TOTALES))); } }
            public string LD_EN_PROCESO_STRING { get { return ((this.LD_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.LD_EN_PROCESO))); } }
            public string LD_ALLOWANCE_TAT_STRING { get { return ((this.LD_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.LD_ALLOWANCE_TAT))); } }
            public string LD_ALLOWANCE_FACT_STRING { get { return ((this.LD_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.LD_ALLOWANCE_FACT))); } }
            public string LD_AJUSTES_STRING { get { return ((this.LD_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.LD_AJUSTES))); } }
            public string LD_TOTALES_STRING { get { return ((this.LD_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.LD_TOTALES))); } }
            public string MS_EN_PROCESO_STRING { get { return ((this.MS_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.MS_EN_PROCESO))); } }
            public string MS_ALLOWANCE_TAT_STRING { get { return ((this.MS_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.MS_ALLOWANCE_TAT))); } }
            public string MS_ALLOWANCE_FACT_STRING { get { return ((this.MS_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.MS_ALLOWANCE_FACT))); } }
            public string MS_AJUSTES_STRING { get { return ((this.MS_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.MS_AJUSTES))); } }
            public string MS_TOTALES_STRING { get { return ((this.MS_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.MS_TOTALES))); } }
            public string R_EN_PROCESO_STRING { get { return ((this.R_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.R_EN_PROCESO))); } }
            public string R_ALLOWANCE_TAT_STRING { get { return ((this.R_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.R_ALLOWANCE_TAT))); } }
            public string R_ALLOWANCE_FACT_STRING { get { return ((this.R_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.R_ALLOWANCE_FACT))); } }
            public string R_AJUSTES_STRING { get { return ((this.R_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.R_AJUSTES))); } }
            public string R_TOTALES_STRING { get { return ((this.R_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.R_TOTALES))); } }
            public string TP_EN_PROCESO_STRING { get { return ((this.TP_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.TP_EN_PROCESO))); } }
            public string TP_ALLOWANCE_TAT_STRING { get { return ((this.TP_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.TP_ALLOWANCE_TAT))); } }
            public string TP_ALLOWANCE_FACT_STRING { get { return ((this.TP_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.TP_ALLOWANCE_FACT))); } }
            public string TP_AJUSTES_STRING { get { return ((this.TP_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.TP_AJUSTES))); } }
            public string TP_TOTALES_STRING { get { return ((this.TP_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.TP_TOTALES))); } }
            public string SIS_EN_PROCESO_STRING { get { return ((this.SIS_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.SIS_EN_PROCESO))); } }
            public string SIS_ALLOWANCE_TAT_STRING { get { return ((this.SIS_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.SIS_ALLOWANCE_TAT))); } }
            public string SIS_ALLOWANCE_FACT_STRING { get { return ((this.SIS_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.SIS_ALLOWANCE_FACT))); } }
            public string SIS_AJUSTES_STRING { get { return ((this.SIS_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.SIS_AJUSTES))); } }
            public string SIS_TOTALES_STRING { get { return ((this.SIS_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.SIS_TOTALES))); } }
            public string SO_EN_PROCESO_STRING { get { return ((this.SO_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.SO_EN_PROCESO))); } }
            public string SO_ALLOWANCE_TAT_STRING { get { return ((this.SO_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.SO_ALLOWANCE_TAT))); } }
            public string SO_ALLOWANCE_FACT_STRING { get { return ((this.SO_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.SO_ALLOWANCE_FACT))); } }
            public string SO_AJUSTES_STRING { get { return ((this.SO_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.SO_AJUSTES))); } }
            public string SO_TOTALES_STRING { get { return ((this.SO_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.SO_TOTALES))); } }
            public string TPIS_EN_PROCESO_STRING { get { return ((this.TPIS_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.TPIS_EN_PROCESO))); } }
            public string TPIS_ALLOWANCE_TAT_STRING { get { return ((this.TPIS_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.TPIS_ALLOWANCE_TAT))); } }
            public string TPIS_ALLOWANCE_FACT_STRING { get { return ((this.TPIS_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.TPIS_ALLOWANCE_FACT))); } }
            public string TPIS_AJUSTES_STRING { get { return ((this.TPIS_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.TPIS_AJUSTES))); } }
            public string TPIS_TOTALES_STRING { get { return ((this.TPIS_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.TPIS_TOTALES))); } }
            public string TPO_EN_PROCESO_STRING { get { return ((this.TPO_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.TPO_EN_PROCESO))); } }
            public string TPO_ALLOWANCE_TAT_STRING { get { return ((this.TPO_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.TPO_ALLOWANCE_TAT))); } }
            public string TPO_ALLOWANCE_FACT_STRING { get { return ((this.TPO_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.TPO_ALLOWANCE_FACT))); } }
            public string TPO_AJUSTES_STRING { get { return ((this.TPO_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.TPO_AJUSTES))); } }
            public string TPO_TOTALES_STRING { get { return ((this.TPO_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.TPO_TOTALES))); } }
            public string U_EN_PROCESO_STRING { get { return ((this.U_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.U_EN_PROCESO))); } }
            public string U_ALLOWANCE_TAT_STRING { get { return ((this.U_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.U_ALLOWANCE_TAT))); } }
            public string U_ALLOWANCE_FACT_STRING { get { return ((this.U_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.U_ALLOWANCE_FACT))); } }
            public string U_AJUSTES_STRING { get { return ((this.U_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.U_AJUSTES))); } }
            public string U_TOTALES_STRING { get { return ((this.U_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.U_TOTALES))); } }
            public string WA_EN_PROCESO_STRING { get { return ((this.WA_EN_PROCESO == 0) ? "-" : String.Format("{0:C}", (this.WA_EN_PROCESO))); } }
            public string WA_ALLOWANCE_TAT_STRING { get { return ((this.WA_ALLOWANCE_TAT == 0) ? "-" : String.Format("{0:C}", (this.WA_ALLOWANCE_TAT))); } }
            public string WA_ALLOWANCE_FACT_STRING { get { return ((this.WA_ALLOWANCE_FACT == 0) ? "-" : String.Format("{0:C}", (this.WA_ALLOWANCE_FACT))); } }
            public string WA_AJUSTES_STRING { get { return ((this.WA_AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.WA_AJUSTES))); } }
            public string WA_TOTALES_STRING { get { return ((this.WA_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.WA_TOTALES))); } }
            public string PROCESO_TAT_TOTAL_STRING { get { return ((this.PROCESO_TAT_TOTAL == 0) ? "-" : String.Format("{0:C}", (this.PROCESO_TAT_TOTAL))); } }
            public string ALLOWANCE_TAT_TOTAL_STRING { get { return ((this.ALLOWANCE_TAT_TOTAL == 0) ? "-" : String.Format("{0:C}", (this.ALLOWANCE_TAT_TOTAL))); } }
            public string ALLOWANCE_FACT_TOTAL_STRING { get { return ((this.ALLOWANCE_FACT_TOTAL == 0) ? "-" : String.Format("{0:C}", (this.ALLOWANCE_FACT_TOTAL))); } }
            public string AJUSTES_STRING { get { return ((this.AJUSTES == 0) ? "-" : String.Format("{0:C}", (this.AJUSTES))); } }
            public string ALLOWANCE_TOTALES_STRING { get { return ((this.ALLOWANCE_TOTALES == 0) ? "-" : String.Format("{0:C}", (this.ALLOWANCE_TOTALES))); } }
        }

        public class Concentrado
        {
            public List<FLUJO> workflow;
            public FLUJO acciones;
            public List<DOCUMENTOA> files;
            public string pais;
            public List<TS_FORM> ts;
            public List<DOCUMENTOT> tts;
            public List<CARTAP> cartap;
            public string ultMod;
            public string miles;
            public string dec;
            public object DOCSREFREVERSOS;
            public decimal remanente;
            public int porcentaje_remanente;

            public DOCUMENTO documento { set; get; }
            public FLUJO flujo { set; get; }
            public string Title { get; set; }
            public List<TSOL> TSOL_RELA { get; set; }
            public CANAL CANAL { get; set; }
            public decimal CUENTA_ABONO { get; set; }
            public decimal CUENTA_CARGO { get; set; }
            public decimal CUENTA_CLEARING { get; set; }
            public decimal CUENTA_LIMITE { get; set; }
            public string CUENTA_CARGO_NOMBRE { get; set; }
            public PRESUPUESTO_MOD PRESUPUESTO { get; set; }
            public string PROVEEDOR_NOMBRE { get; set; }
            public string ESTATUS_STRING { get; set; }
            public string STATUS { get; internal set; }
            public string STATUSS1 { get; internal set; }
            public string STATUSS3 { get; internal set; }
            public string STATUSS2 { get; internal set; }
            public string STATUSS4 { get; internal set; }
            public string STATUSS { get; internal set; }
            public int SEMANA { get; internal set; }


            public string CoCode_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.SOCIEDAD_ID);
                    }
                    catch {
                        return String.Empty;
                    }
                }
            }
            public string Pais_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.PAIS_ID);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Subregion_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CLIENTE.REGION);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Estado_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.ESTADO);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Ciudad_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CIUDAD);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumeroSolicitud_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.NUM_DOC.ToString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string FechaSolicitud_STRING
            {
                get
                {
                    try
                    {
                        return (((DateTime) this.documento.FECHAC).ToShortDateString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string HoraSolicitud_STRING
            {
                get
                {
                    try
                    {
                        return (((DateTime) this.documento.FECHAC).ToString("HH:mm:ss"));
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string SemanaPeriodo_STRING
            {
                get
                {
                    try
                    {
                        return (this.SEMANA.ToString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string PeriodoContable_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.PERIODO.ToString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Anio_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.EJERCICIO);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string TipoSolicitud_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(this.documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT020);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string TipoProvision_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("PR"))
                            return (this.documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(this.documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50);
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string TipoNC_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("NC"))
                            return (this.documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(this.documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50);
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string TipoOP_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("OP"))
                            return (this.documento.TSOL.TSOLTs.Where(a => a.SPRAS_ID.Equals(this.documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50);
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Status_STRING
            {
                get
                {
                    try
                    {
                        return (this.ESTATUS_STRING);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Concepto_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CONCEPTO);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Mecanica_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.NOTAS);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string De_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FECHAI_VIG.Value.ToShortDateString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string A_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FECHAF_VIG.Value.ToShortDateString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string DePeriodo_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FECHAI_VIG.Value.ToString("MM-yyyy"));
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string APeriodo_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FECHAF_VIG.Value.ToString("MM-yyyy"));
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Clasificacion_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.GALL.GALLTs.Where(a => a.SPRAS_ID.Equals(this.documento.USUARIO.SPRAS_ID)).FirstOrDefault().TXT50);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string CuentaContableGasto_STRING
            {
                get
                {
                    try
                    {
                        return (this.CUENTA_CARGO.ToString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NombreCuenta_STRING
            {
                get
                {
                    try
                    {
                        return (this.CUENTA_CARGO_NOMBRE);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Cliente_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CLIENTE.KUNNR);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Nombre_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CLIENTE.NAME1);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string TipoCliente_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.SOLD_TO_ID != null)
                            return "Sold To";
                        else
                            return "Payer";
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string OrganizacionVentas_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CLIENTE.VKORG);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string TaxID_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CLIENTE.STCD1);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Canal_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.CLIENTE.CANAL);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Descripcion_STRING
            {
                get
                {
                    try
                    {
                        return ((this.CANAL == null) ? "" : this.CANAL.CDESCRIPCION);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NombreContacto_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.PAYER_NOMBRE);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string EmailContacto_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.PAYER_EMAIL);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoOriginalProvision_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("PR"))
                            return (String.Format("{0:C}", this.documento.MONTO_DOC_MD));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoNCOP_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("NC") || this.documento.TSOL_ID.StartsWith("OP"))
                            return (String.Format("({0:C})", this.documento.MONTO_DOC_MD));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoAplicado_STRING
            {
                get
                {
                    try
                    {
                        return ((this.PRESUPUESTO.PC_T == 0) ? string.Empty : String.Format("({0:C})", this.PRESUPUESTO.PC_T));
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string SaldoRemanenteProvision_STRING
            {
                get
                {
                    try
                    {
                        return ((this.PRESUPUESTO.CONSU == 0) ? string.Empty : String.Format("{0:C}", this.PRESUPUESTO.CONSU));
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoReversoProvision_STRING
            {
                get
                {
                    try
                    {
                        if (this.DOCREVERSOS2 != null)
                            return (String.Format("{0:C}", this.DOCREVERSOS2.MONTO_DOC_ML));
                            //return (String.Format("{0:C}", this.DOCREVERSOS2.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null), null).GetType().GetProperty("MONTO_DOC_MD").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null), null), null)));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string PorcentajeReverso_STRING
            {
                get
                {
                    try
                    {
                        //if (this.DOCSREFREVERSOS != null)
                        //    return ((Convert.ToDecimal(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null), null).GetType().GetProperty("MONTO_DOC_MD").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("DOCUMENTO").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null), null), null)) * 100) / this.documento.MONTO_DOC_MD).ToString();
                        if (this.DOCREVERSOS2 != null)
                            return Convert.ToInt32((this.DOCREVERSOS2.MONTO_DOC_ML * 100) / this.documento.MONTO_DOC_ML).ToString();
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoExcesoProvision_STRING
            {
                get
                {
                    try
                    {
                        return (String.Format("{0:C}", this.remanente));
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string PorcentajeExcesoProvision_STRING
            {
                get
                {
                    try
                    {
                        return (this.porcentaje_remanente.ToString());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Impactos_STRING
            {
                get
                {
                    try
                    {
                        return (String.Empty);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumRegistroProvision_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTO_SAP != null) ? this.documento.DOCUMENTO_SAP.ToString() : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumRegistroNCOP_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("NC") || this.documento.TSOL_ID.StartsWith("OP"))
                            return ((this.documento.DOCUMENTO_SAP != null) ? this.documento.DOCUMENTO_SAP.ToString() : "");
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumRegistroReverso_STRING
            {
                get
                {
                    try
                    {
                        if (this.DOCSREFREVERSOS != null)
                            return ((this.documento.DOCUMENTO_SAP != null) ? this.documento.DOCUMENTO_SAP.ToString() : "");
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string FechaReverso_STRING
            {
                get
                {
                    try
                    {
                        if (this.DOCSREFREVERSOS != null)
                            return (this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("FECHAC").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null), null).ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string PeriodoContableReverso_STRING
            {
                get
                {
                    try
                    {
                        if (this.DOCSREFREVERSOS != null)
                            return (this.DOCSREFREVERSOS.GetType().GetProperty("d").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("PERIODO").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("d").GetValue(this.DOCSREFREVERSOS, null), null).ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string RazonReverso_STRING
            {
                get
                {
                    try
                    {
                        if (this.DOCSREFREVERSOS != null)
                            return (this.DOCSREFREVERSOS.GetType().GetProperty("tr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("TXT100").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("tr").GetValue(this.DOCSREFREVERSOS, null), null).ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ComentarioReverso_STRING
            {
                get
                {
                    try
                    {
                        if (this.DOCSREFREVERSOS != null)
                            return (this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null).GetType().GetProperty("COMENTARIO").GetValue(this.DOCSREFREVERSOS.GetType().GetProperty("dr").GetValue(this.DOCSREFREVERSOS, null), null).ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Usuario_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIO.NOMBRE + " " + this.documento.USUARIO.APELLIDO_P);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Backup_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIO.BACKUP_ID);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string CreadoPor_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIO.NOMBRE + " " + this.documento.USUARIO.APELLIDO_P);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string CreadoPorID_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIO.ID);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string SolicitadoPor_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIO.NOMBRE + " " + this.documento.USUARIO.APELLIDO_P);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string SolicitadoPorID_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIO.ID);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ModificadoPor_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIOD_ID);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ModificadoPorID_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.USUARIOD_ID);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string AprobadorN1_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FLUJOes.Where(u => u.WF_POS == 1).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string AprobadorN2_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FLUJOes.Where(u => u.WF_POS == 2).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string AprobadorN3_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FLUJOes.Where(u => u.WF_POS == 3).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string AprobadorN4_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FLUJOes.Where(u => u.WF_POS == 4).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string AprobadorN5_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.FLUJOes.Where(u => u.WF_POS == 5).Select(u => u.USUARIO.NOMBRE + " " + u.USUARIO.APELLIDO_P).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Proveedor_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.DOCUMENTOFs.Select(df => df.PROVEEDOR).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NombreProveedor_STRING
            {
                get
                {
                    try
                    {
                        return (this.PROVEEDOR_NOMBRE);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumeroFacturaProveedor_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.DOCUMENTOFs.Select(df => df.BILL_DOC).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumeroFacturaK_STRING
            {
                get
                {
                    try
                    {
                        return (this.documento.DOCUMENTOFs.Select(df => df.FACTURAK).FirstOrDefault());
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumeroNC_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("NC"))
                            return (this.documento.NUM_DOC.ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NumeroOP_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("OP"))
                            return (this.documento.NUM_DOC.ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ExpRecognitionPeriodo_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 1 && (bool)x.CHECKS).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ExpRecognitionEjercicio_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 2 && (bool) x.CHECKS).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string SoporteIncorrecto_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 3 && (bool)x.CHECKS).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string SoporteValidado_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOTS.Where(x => x.TSFORM_ID == 4 && (bool) x.CHECKS).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Carta_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("CAR")).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Contrato_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("CON")).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string JBP_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("JBP")).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Factura_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("FAC")).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Otros_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.DOCUMENTOAs.Where(da => da.CLASE.Equals("OTR")).Count() > 0) ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NegociacionMonto_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_TECNICO == "M") ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NegociacionPorcentaje_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_TECNICO == "P") ? "X" : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string DistribucionMaterial_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTOPs.Count > 0)
                            return ((this.documento.DOCUMENTOPs.FirstOrDefault().MATNR != "") ? "X" : "");
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string DistribucionCategoria_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTOPs.Count > 0)
                            return ((this.documento.DOCUMENTOPs.FirstOrDefault().MATNR == "") ? "X" : "");
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoBase_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_TECNICO == "M") ? String.Format("{0:C}", this.documento.MONTO_DOC_MD) : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoPorcentaje_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_TECNICO == "P") ? String.Format("{0:C}", this.documento.MONTO_DOC_MD) : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Recurrente_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_RECURRENTE != null) ? "Si" : "No");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string RecurrentePorcentaje_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_RECURRENTE != null) ? ((this.documento.TIPO_TECNICO == "P") ? "X" : "") : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string RecurrenteMonto_STRING
            {
                get
                {
                    try
                    {
                        return ((this.documento.TIPO_RECURRENTE != null) ? ((this.documento.TIPO_TECNICO == "M") ? "X" : "") : "");
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string RecurrenteCancelada_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTORECs.Count > 0)
                            return ((this.documento.DOCUMENTORECs.FirstOrDefault().ESTATUS == "C") ? "X" : "");
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ObjetivoInicio_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTORAN.Count > 0)
                            return (String.Format("{0:C}", this.documento.DOCUMENTORAN.FirstOrDefault().OBJETIVOI));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string ObjetivoLimite_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTORAN.Count > 0)
                            return (this.documento.DOCUMENTORAN.FirstOrDefault().PORCENTAJE.ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Estatus_STRING
            {
                get
                {
                    try
                    {
                        return (this.ESTATUS_STRING);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string VentaPeriodo_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTOLs.Count > 0)
                            return (String.Format("{0:C}", this.documento.DOCUMENTOLs.FirstOrDefault().MONTO_VENTA));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string MontoProvision_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTOLs.Count > 0 && this.documento.DOCUMENTORAN.Count > 0)
                            return (String.Format("{0:C}", this.documento.DOCUMENTOLs.FirstOrDefault().MONTO_VENTA * (this.documento.DOCUMENTORAN.FirstOrDefault().PORCENTAJE / 100)));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string BackOrder_STRING
            {
                get
                {
                    try
                    {
                        //return (((this.documento.ESTATUS.Equals("A")) && (this.documento.TSOL.PADRE) && (this.documento.DOCUMENTOLs.Count > 0)) ? String.Format("{0:C}", this.documento.MONTO_DOC_MD) : "");
                        if (this.DOCBACKORDER != null)
                            return (String.Format("{0:C}", Convert.ToDecimal(this.DOCBACKORDER.BACKORDER)));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string VentaRealBackOrder_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTOLs.Count > 0)
                            return (String.Format("{0:C}", this.documento.DOCUMENTOLs.FirstOrDefault().MONTO_VENTA + this.documento.DOCUMENTOLs.FirstOrDefault().BACKORDER));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string NCOPReverso_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.TSOL_ID.StartsWith("NC") || this.documento.TSOL_ID.StartsWith("OP") || this.documento.TSOL_ID.StartsWith("RP"))
                            return (this.documento.NUM_DOC.ToString());
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            public string Monto_STRING
            {
                get
                {
                    try
                    {
                        if (this.documento.DOCUMENTOLs.Count > 0 && this.documento.DOCUMENTORAN.Count > 0)
                            return (String.Format("{0:C}", (this.documento.DOCUMENTOLs.FirstOrDefault().MONTO_VENTA + this.documento.DOCUMENTOLs.FirstOrDefault().BACKORDER) * (this.documento.DOCUMENTORAN.FirstOrDefault().OBJETIVOI / 100)));
                        else
                            return String.Empty;
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }

            public DOCUMENTO DOCREVERSOS2 { get; internal set; }
            public DOCUMENTOL DOCBACKORDER { get; internal set; }
        }
    }
}