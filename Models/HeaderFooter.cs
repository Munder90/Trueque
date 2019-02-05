using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class HeaderFooter : PdfPageEventHelper
    {
        //public static CartaF cf;
        public static CartaV cv;
        public PdfContentByte cb;
        public HeaderFooter() { }
        //public HeaderFooter(CartaV c)
        //{
        //    cv = c;
        //}
        public HeaderFooter(CartaV v)
        {
            cv = v;
        }
        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);
            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            PdfPTable tabCabecera = new PdfPTable(1);
            PdfPTable tabPie = new PdfPTable(1);
            PdfPCell celLineaCabecera = new PdfPCell();
            PdfPCell celLegal = new PdfPCell();
            PdfPCell celEmail = new PdfPCell();
            PdfPCell celLineaPie = new PdfPCell();

            //CABECERA
            iTextSharp.text.Image imagen2 = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/images/logo_kellogg.png"));
            celLineaCabecera.Image = imagen2;
            celLineaCabecera.BackgroundColor = new BaseColor(181, 25, 70);
            celLineaCabecera.Border = 0;
            celLineaCabecera.Padding = 3;
            celLineaCabecera.FixedHeight = 30f;
            celLineaCabecera.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            tabCabecera.AddCell(celLineaCabecera);
            tabCabecera.TotalWidth = document.PageSize.Width;
            tabCabecera.WriteSelectedRows(0, -1, 0, document.PageSize.Top, writer.DirectContent);

            //FOOTER
            //if (cf != null)
            //{
            //    if (cf.legal_x == true)
            //    { celLegal.AddElement(new Paragraph(cf.legal, FontFactory.GetFont(FontFactory.HELVETICA, 8))); celLegal.PaddingLeft = 30; celLegal.PaddingRight = 30; celLegal.Border = 0; tabPie.AddCell(celLegal); }
            //    else
            //    { celLegal.AddElement(new Paragraph("", FontFactory.GetFont(FontFactory.HELVETICA, 8))); celLegal.Border = 0; tabPie.AddCell(celLegal); }

            //    if (cf.mail_x == true)
            //    { celEmail.AddElement(new Paragraph(cf.mail, FontFactory.GetFont(FontFactory.HELVETICA, 8))); celEmail.PaddingLeft = 30; celEmail.PaddingRight = 30; celEmail.Border = 0; tabPie.AddCell(celEmail); }
            //    else
            //    { celEmail.AddElement(new Paragraph("", FontFactory.GetFont(FontFactory.HELVETICA, 8))); celEmail.Border = 0; tabPie.AddCell(celEmail); }

            //    celLineaPie.AddElement(new Chunk(""));
            //    celLineaPie.BackgroundColor = new BaseColor(181, 25, 70);
            //    celLineaPie.Border = 0;
            //    celLineaPie.FixedHeight = 30f;
            //    tabPie.AddCell(celLineaPie);

            //    tabPie.TotalWidth = document.PageSize.Width;
            //    tabPie.WriteSelectedRows(0, -1, 0, document.PageSize.GetBottom(tabPie.TotalHeight), writer.DirectContent);
            //}
            //
            //else 
            if (cv != null)
            {
                if (cv.legal_x == true)
                { celLegal.AddElement(new Paragraph(cv.legal, FontFactory.GetFont(FontFactory.HELVETICA, 8))); celLegal.PaddingLeft = 30; celLegal.PaddingRight = 30; celLegal.Border = 0; tabPie.AddCell(celLegal); }
                else
                { celLegal.AddElement(new Paragraph("", FontFactory.GetFont(FontFactory.HELVETICA, 8))); celLegal.Border = 0; tabPie.AddCell(celLegal); }

                if (cv.mail_x == true)
                { celEmail.AddElement(new Paragraph(cv.mail, FontFactory.GetFont(FontFactory.HELVETICA, 8))); celEmail.PaddingLeft = 30; celEmail.PaddingRight = 30; celEmail.Border = 0; tabPie.AddCell(celEmail); }
                else
                { celEmail.AddElement(new Paragraph("", FontFactory.GetFont(FontFactory.HELVETICA, 8))); celEmail.Border = 0; tabPie.AddCell(celEmail); }

                celLineaPie.AddElement(new Chunk(""));
                celLineaPie.BackgroundColor = new BaseColor(181, 25, 70);
                celLineaPie.Border = 0;
                celLineaPie.FixedHeight = 30f;
                tabPie.AddCell(celLineaPie);

                tabPie.TotalWidth = document.PageSize.Width;
                tabPie.WriteSelectedRows(0, -1, 0, document.PageSize.GetBottom(tabPie.TotalHeight), writer.DirectContent);
            }
        }
        public void quitaBordes(int indice, PdfPTable tabla)
        {
            foreach (PdfPCell celda in tabla.Rows[indice].GetCells())
            {
                celda.Border = 0;
            }
        }

        public void pintaCabecera(PdfPTable tabla)
        {
            foreach (PdfPCell celda in tabla.Rows[0].GetCells())
            {
                celda.BackgroundColor = BaseColor.LIGHT_GRAY;
                celda.HorizontalAlignment = 1;
                celda.Padding = 3;
            }
        }
        //[System.Web.Services.WebMethod]
        public void eliminaArchivos()
        {
            string[] todoArchivos = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/PdfTemp/"));

            foreach (string archivo in todoArchivos)
            {
                try
                {
                    File.Delete(archivo);
                }
                catch
                {

                }
            }

            //cf = null;
            cv = null;
        }
    }
}