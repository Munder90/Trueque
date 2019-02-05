using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using TAT001.Entities;
using System.Data.Entity;
using ClosedXML.Excel;
//using SimpleImpersonation;

namespace TAT001.Models
{
    public class PresupuestoModels
    {
        private TruequeEntities db = new TruequeEntities();
        public void consultSociedad(ref DatosPresupuesto sociedades, string sociedad, string usuario)
        {
            sociedades.sociedad = db.USUARIOs.Where(a => a.ID.Equals(usuario)).FirstOrDefault().SOCIEDADs.ToList();
            if (String.IsNullOrEmpty(sociedad) == false)
            {
                sociedades.cambio = db.CSP_CAMBIO(sociedad).ToList();
            }
        }
        public DatosPresupuesto consultarDatos(string[] sociedad, string anio, string periodo, string cambio, string cpt, string excel, string ruta, string usuario)
        {
            DatosPresupuesto sociedades = new DatosPresupuesto();
            string anioc = "";// periodoc = "";
            string chkcpt = "";
            for (int i = 0; i < sociedad.Length; i++)
            {
                consultSociedad(ref sociedades, sociedad[i], usuario);
                if (String.IsNullOrEmpty(anio) == false)
                {
                    anioc = anio.Substring(2, 2);
                }
                //if (String.IsNullOrEmpty(periodo) == false)
                //{
                //    periodoc = periodo;
                //}
                if (String.IsNullOrEmpty(cpt) == false)
                {
                    chkcpt = "X";
                }
                if (String.IsNullOrEmpty(cambio) == false && sociedades.cambio.Count > 0)
                {
                    string[] moneda = cambio.Split('-');
                    sociedades.presupuesto.AddRange(db.CSP_CONSULTARPRESUPUESTO(sociedad[i], anioc, anio, periodo, periodo, sociedades.cambio[0].FCURR, moneda[1], chkcpt).ToList());
                }
                else
                {
                    sociedades.presupuesto.AddRange(db.CSP_CONSULTARPRESUPUESTO(sociedad[i], anioc, anio, periodo, periodo, "", "", chkcpt).ToList());
                }
            }
            if (excel != null)
            {
                string[] moneda = new string[1];
                if (String.IsNullOrEmpty(cambio) == false)
                {
                    moneda = cambio.Split('-');
                }
                else if (sociedades.cambio.Count > 0)
                {
                    moneda[0] = sociedades.cambio[0].FCURR;
                }
                generarRepPresuExcel(sociedades.presupuesto, sociedad, moneda[0], consultarUCarga(), ruta, cpt);
            }
            return sociedades;
        }
        public string consultarUCarga()
        {
            return db.PRESUPUESTOHs.Max(x => x.FECHAC).ToString();
        }
        public string consultaAnio()
        {
            return db.PRESUPUESTOPs.Min(x => x.ANIO);
        }
        private string mes(string mes)
        {
            string ms = "";
            switch (mes)
            {
                case "01":
                    ms = "Jan";
                    break;
                case "02":
                    ms = "Feb";
                    break;
                case "03":
                    ms = "Mar";
                    break;
                case "04":
                    ms = "Apr";
                    break;
                case "05":
                    ms = "May";
                    break;
                case "06":
                    ms = "Jun";
                    break;
                case "07":
                    ms = "Jul";
                    break;
                case "08":
                    ms = "Aug";
                    break;
                case "09":
                    ms = "Sep";
                    break;
                case "10":
                    ms = "Oct";
                    break;
                case "11":
                    ms = "Nov";
                    break;
                case "12":
                    ms = "Dec";
                    break;
            }
            return ms;
        }
        public void generarRepPresuExcel(List<CSP_CONSULTARPRESUPUESTO_Result> resultado, string[] sociedad, string moneda, string fecha, string ruta, string cpt)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            int contador = 3;
            string sociedades = "";
            string index;
            for (int i = 0; i < sociedad.Length; i++)
            {
                sociedades += sociedad[i] + ", ";
            }
            sociedades = sociedades.Substring(0, (sociedades.Length - 2));
            if (String.IsNullOrEmpty(moneda))
            {
                moneda = "LC";
            }
            if (resultado.Count > 1)
            {
                worksheet.Cell("A1").Value = new[]
                {
                    new
                    {
                        SOCIEDAD = "Sociedad/es: " + sociedades,
                        MONEDA = "Moneda: " + moneda,
                        FECHA = "Ultima carga: " + fecha,
                    }
                };
                if (String.IsNullOrEmpty(cpt) == false)
                {
                    worksheet.Cell("A2").Value = new[]
                 {
                  new {
                      CANAL        = "Canal",
                      CDESCRIPCION = "Descripcion",
                      PPTOC        = "Total Canal",
                      BANNER       = "Banner",
                      BDESCRIPCION = "Descripcion",
                      PPTO         = "PPTO Banner",
                      PERIODO      = "Periodo",
                      VVX17        = "VVX17 - Commercial Discounts",
                      CSHDC        = "CSHDC - Cash Discounts",
                      RECUN        = "RECUN - Unsaleables",
                      DSTRB        = "DSTRB - Distribution Commission",
                      OTHTA        = "OTHTA - Logistic Discount",
                      ADVER        = "ADVER - Trade Promotion-Other",
                      CORPM        = "CORPM - Booklets and Sponsorship",
                      POP          = "POP - Store openings and Info Exchange",
                      PMVAR        = "PMVAR - Growth Program",
                      CONPR        = "CONPR - Everyday Low Price",
                      RSRDV        = "RSRDV - Rollbacks",
                      SPA          = "SPA - Cleareance",
                      FREEG        = "FREEG - Free Goods"
                      },
                    };
                    foreach (CSP_CONSULTARPRESUPUESTO_Result row in resultado)
                    {
                        index = "A";
                        index = index + contador;
                        worksheet.Cell(index).Value = new[]
                     {
                  new {
                      CANAL        = row.CANAL,
                      CDESCRIPCION = row.CDESCRIPCION,
                      PPTOC        = row.PPTOC,
                      BANNER       = row.BANNER,
                      BDESCRIPCION = row.BDESCRIPCION,
                      PPTO         = row.PPTO,
                      PERIODO      = row.PERIODO,
                      VVX17        = row.VVX17,
                      CSHDC        = row.CSHDC,
                      RECUN        = row.RECUN,
                      DSTRB        = row.DSTRB,
                      OTHTA        = row.OTHTA,
                      ADVER        = row.ADVER,
                      CORPM        = row.CORPM,
                      POP          = row.POP,
                      PMVAR        = row.PMVAR,
                      CONPR        = row.CONPR,
                      RSRDV        = row.RSRDV,
                      SPA          = row.SPA,
                      FREEG        = row.FREEG
                      },
                    };
                        contador++;
                    }
                    worksheet.Range("A2:T2").Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#0B2161")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    for (int i = 1; i < 22; i++)
                    {
                        worksheet.Column(i).AdjustToContents();
                    }
                }
                else
                {
                    worksheet.Cell("A2").Value = new[]
                 {
                  new {
                      CANAL        = "Canal",
                      CDESCRIPCION = "Descripcion",
                      PPTOC        = "Total Canal",
                      BANNER       = "Banner",
                      BDESCRIPCION = "Descripcion",
                      PPTO         = "PPTO Banner",
                      PERIODO      = "Periodo",
                      VVX17        = "VVX17 - Commercial Discounts",
                      CSHDC        = "CSHDC - Cash Discounts",
                      RECUN        = "RECUN - Unsaleables",
                      DSTRB        = "DSTRB - Distribution Commission",
                      OTHTA        = "OTHTA - Logistic Discount",
                      ADVER        = "ADVER - Trade Promotion-Other",
                      CORPM        = "CORPM - Booklets and Sponsorship",
                      POP          = "POP - Store openings and Info Exchange",
                      PMVAR        = "PMVAR - Growth Program",
                      CONPR        = "CONPR - Everyday Low Price",
                      RSRDV        = "RSRDV - Rollbacks",
                      SPA          = "SPA - Cleareance",
                      FREEG        = "FREEG - Free Goods",
                      ALLB         = "Allowances Registros Icon & Manuales",
                      ALLF         = "Allowances Facturados",
                      PROCE        = "En Proceso ICON",
                      CONSU        = "Consumido",
                      TOTAL        = "PPTO Disponible"
                      },
                    };
                    foreach (CSP_CONSULTARPRESUPUESTO_Result row in resultado)
                    {
                        index = "A";
                        index = index + contador;
                        worksheet.Cell(index).Value = new[]
                     {
                  new {
                      CANAL        = row.CANAL,
                      CDESCRIPCION = row.CDESCRIPCION,
                      PPTOC        = row.PPTOC,
                      BANNER       = row.BANNER,
                      BDESCRIPCION = row.BDESCRIPCION,
                      PPTO         = row.PPTO,
                      PERIODO      = row.PERIODO,
                      VVX17        = row.VVX17,
                      CSHDC        = row.CSHDC,
                      RECUN        = row.RECUN,
                      DSTRB        = row.DSTRB,
                      OTHTA        = row.OTHTA,
                      ADVER        = row.ADVER,
                      CORPM        = row.CORPM,
                      POP          = row.POP,
                      PMVAR        = row.PMVAR,
                      CONPR        = row.CONPR,
                      RSRDV        = row.RSRDV,
                      SPA          = row.SPA,
                      FREEG        = row.FREEG,
                      ALLB        = row.ALLB,
                      ALLF        = row.ALLF,
                      PROCE       = row.PROCESO,
                      CONSU        = row.CONSU + row.ALLB + row.ALLF + row.PROCESO,
                      TOTAL        = row.TOTAL - ( row.ALLB + row.ALLF + row.PROCESO)
                      },
                    };
                        contador++;
                    }
                    worksheet.Range("A2:Y2").Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#0B2161")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                    for (int i = 1; i <= 25; i++)
                    {
                        worksheet.Column(i).AdjustToContents();
                    }
                }
                workbook.SaveAs(ruta + @"\Presupuesto.xlsx");
            }
        }

        public void contDescarga(string ruta, ref string contentType, ref string nombre)
        {
            string[] archivo = ruta.Split('\\');
            nombre = archivo[archivo.Length - 1];
            string[] extencion = archivo[archivo.Length - 1].Split('.');
            switch (extencion[extencion.Length - 1].ToLower())
            {
                case "xltx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                    break;
                case "xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "xlsm":
                    contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                    break;
                case "xltm ":
                    contentType = "application/vnd.ms-excel.template.macroEnabled.12";
                    break;
                case "xlam":
                    contentType = "application/vnd.ms-excel.addin.macroEnabled.12";
                    break;
                case "xlsb":
                    contentType = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                    break;
                case "xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xlt":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xla":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "doc":
                    contentType = "application/msword";
                    break;
                case "dot":
                    contentType = "application/msword";
                    break;
                case "docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "dotx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                    break;
                case "docm":
                    contentType = "application/vnd.ms-word.document.macroEnabled.12";
                    break;
                case "dotm":
                    contentType = "application/vnd.ms-word.template.macroEnabled.12";
                    break;
                case "pdf":
                    contentType = "application/pdf";
                    break;
                case "zip":
                    contentType = "application/zip";
                    break;
                case "jpg":
                    contentType = "image/jpeg";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                case "msg":
                    contentType = "application/vnd.ms-outlook";
                    break;
            }
        }
    }
}