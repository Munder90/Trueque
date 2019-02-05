using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Models
{
    public class CargaMasivaModels
    {
        private TruequeEntities db = new TruequeEntities();
        public void GenerarListaCliPro(string ruta)
        {
            List<CPS_LISTA_CLI_PRO_Result> lista = db.CPS_LISTA_CLI_PRO().ToList();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            int contador = 2;
            string index;
            try
            {
                worksheet.Cell("A1").Value = new[]
                 {
                  new {
                      KUNNR = "Payer"
                      ,NAME1 = "Cliente"
                      ,PROVEEDOR_ID = "Numero Proveedor"
                      ,NOMBRE = "Proveedor"
                      ,LANDX = "Pais"
                      ,VKORG = "Sales Org."
                      ,CANAL = "Canal"
                      ,CONTAC = "Contacto"
                      ,CONT_EMAIL = "Email"
                      },
                    };
                foreach (CPS_LISTA_CLI_PRO_Result row in lista)
                {
                    index = "A";
                    index = index + contador;
                    worksheet.Cell(index).Value = new[]
                 {
                  new {
                      KUNNR       = row.KUNNR
                      ,NAME1       = row.NAME1
                      ,PROVEEDOR_ID       = row.PROVEEDOR_ID
                      ,NOMBRE       = row.NOMBRE
                      ,LANDX       = row.LANDX
                      ,VKORG       = row.VKORG
                      ,CANAL       = row.CANAL
                      ,CONTAC       = row.CONTAC
                      ,CONT_EMAIL       = row.CONT_EMAIL
                      },
                    };
                    contador++;
                }
                worksheet.Range("A1:I1").Style.Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#0B2161")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                for (int i = 1; i < 22; i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }
                workbook.SaveAs(ruta + @"\ListaClienteProveedores.xlsx");
            }
            catch (Exception)
            {

            }
        }
    }
}