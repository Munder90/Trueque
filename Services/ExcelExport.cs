using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClosedXML.Excel;

namespace TAT001.Services
{
    public class ExcelExport
    {
        public static string getRuta()
        {
            return "~/PdfTemp/";
        }

        public static string generarExcel(List<ExcelExportColumn> columnas, dynamic data, string nombreReporte, string ruta, string[] subfiltros = null)
        {
            // Aplicar subfiltros
            List<bool> valido = new List<bool>();
            bool aplicarSubfiltros = false;
            if(subfiltros != null)
            {
                for (int registro = 0; registro < data.Count; registro++)
                {
                    valido.Add(false);
                    for (int index = 0; index < subfiltros.Length; index++)
                    {
                        if(!string.IsNullOrEmpty(subfiltros[index]))
                        {
                            aplicarSubfiltros = true;
                            valido[registro] = (data[registro].GetType().GetProperty(columnas[index].columnaDatos).GetValue(data[registro], null).ToString() == subfiltros[index].ToString());
                            if (!valido[registro])
                                break;
                        }
                    }
                }
            }

            // Crear excel
            string archivo = "";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            int registros_agregados = 0;
            try
            {
                for (int columna = 0; columna < columnas.Count; columna++)
                {
                    worksheet.Cell(1, columna + 1).Value = columnas[columna].titulo;
                    registros_agregados = 0;
                    for (int registro = 0; registro < data.Count; registro++)
                    {
                        if (!aplicarSubfiltros || (valido.Count > 0 && valido[registro]))
                        {
                            worksheet.Cell(registros_agregados + 2, columna + 1).Value = ((columnas[columna].forceTextFormat) ? "'" : String.Empty) + data[registro].GetType().GetProperty(columnas[columna].columnaDatos).GetValue(data[registro], null);
                            if (!String.IsNullOrEmpty(columnas[columna].hyperlink))
                            {
                                try
                                {
                                    worksheet.Cell(registros_agregados + 2, columna + 1).Hyperlink = new XLHyperlink(new Uri(columnas[columna].hyperlink + "/" + data[registro].GetType().GetProperty(columnas[columna].columnaDatos).GetValue(data[registro], null)));
                                }
                                catch (Exception e) { }
                            }
                            registros_agregados++;
                        }
                    }
                }
                archivo = nombreReporte + DateTime.Now.ToString("HHmmyyyyMMdd") + ".xlsx";
                workbook.SaveAs(ruta + @"\" + archivo);
                return archivo;
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }
            return archivo;
        }

    }

    public class ExcelExportColumn
    {
        public string titulo { get; set; }
        public string columnaDatos { get; set; }
        public string hyperlink { get; set; }
        public bool forceTextFormat { get; set; }

        public ExcelExportColumn(string titulo, string datos)
        {
            this.titulo = titulo;
            this.columnaDatos = datos;
            this.forceTextFormat = false;
        }

        public ExcelExportColumn(string titulo, string datos, bool textFormat)
        {
            this.titulo = titulo;
            this.columnaDatos = datos;
            this.forceTextFormat = textFormat;
        }

        public ExcelExportColumn(string titulo, string datos, string link, bool textFormat)
        {
            this.titulo = titulo;
            this.columnaDatos = datos;
            this.hyperlink = link;
            this.forceTextFormat = textFormat;
        }
    }
}