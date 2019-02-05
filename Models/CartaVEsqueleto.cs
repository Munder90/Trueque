using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using TAT001.Models;
using TAT001.Entities;
using TAT001.Common;

namespace TAT001.Models
{
    public class CartaVEsqueleto
    {
        iTextSharp.text.Font letraTabNegrita = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7);
        iTextSharp.text.Font letraTab = FontFactory.GetFont(FontFactory.HELVETICA, 7);
        iTextSharp.text.Font negritaPeque = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
        iTextSharp.text.Font normalPeque = FontFactory.GetFont(FontFactory.HELVETICA, 11);
        iTextSharp.text.Font normalMasPeque = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        PdfPTable tablaDatos = new PdfPTable(1);
        PdfPTable tablaDatos1 = new PdfPTable(2);
        PdfPTable tablaDatos2 = new PdfPTable(2);
        PdfPTable tablaDatos3 = new PdfPTable(2);
        PdfPTable tabComentarios = new PdfPTable(2);
        public int a, b, r;
        private int pos, pos2 = 0,pos3=0;

        public string crearPDF(CartaV v, string spr, bool aprob)
        {
            string ruta = "";
            using (TruequeEntities db = new TruequeEntities())
            {
                HeaderFooter hfClass = new HeaderFooter(v);
                DateTime fechaCreacion = DateTime.Now;
                string nombreArchivo = string.Format("{0}.pdf", fechaCreacion.ToString(@"yyyyMMdd") + "_" + fechaCreacion.ToString(@"HHmmss"));
                string rutaCompleta = HttpContext.Current.Server.MapPath("~/PdfTemp/" + nombreArchivo);
                FileStream fsDocumento = new FileStream(rutaCompleta, FileMode.Create);
                //PASO UNO DEMINIMOS EL TIPO DOCUMENTO CON LOS RESPECTIVOS MARGENES (A4,IZQ,DER,TOP,BOT)
                Document pdfDoc = new Document(PageSize.A4, 30f, 30f, 40f, 100f);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fsDocumento);
                pdfWriter.PageEvent = new HeaderFooter();
                var text = db.TEXTOCVs.Where(x => x.SPRAS_ID == spr);

                try
                {
                    pdfDoc.Open();
                    Paragraph frase1, frase2;

                    if (v.company_x )
                    {
                        frase1 = new Paragraph(v.company, negritaPeque);
                        a = 18;
                    }
                    else
                    {
                        frase1 = new Paragraph("", negritaPeque);
                        a = 0;
                    }

                    if (!aprob)
                    {
                        float fontSize = 250;
                        float xPosition = 300;
                        float yPosition = 400;
                        float angle = 45;
                        PdfContentByte under = pdfWriter.DirectContentUnder;
                        BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                        under.BeginText();
                        under.SetColorFill(BaseColor.LIGHT_GRAY);
                        under.SetFontAndSize(baseFont, fontSize);
                        under.ShowTextAligned(PdfContentByte.ALIGN_CENTER, text.Where(x => x.CAMPO == "agua").First().TEXTO, xPosition, yPosition, angle);
                        under.EndText();
                    }


                    frase1.Alignment = Element.ALIGN_RIGHT;
                    pdfDoc.Add(frase1);
                    pdfDoc.Add(new Chunk(""));

                    if (v.taxid_x )
                    {
                        frase2 = new Paragraph(v.taxid, negritaPeque);
                        b = 18;
                    }
                    else
                    {
                        frase2 = new Paragraph("", negritaPeque);
                        b = 0;
                    }
                    frase2.Alignment = Element.ALIGN_RIGHT;
                    pdfDoc.Add(frase2);
                    r = a + b;

                    //AQUI VA LA LINEA 2
                    pdfDoc.Add(new Chunk(""));
                    PdfPTable tabColor = new PdfPTable(5);
                    PdfPCell celdaColor = new PdfPCell(new Paragraph(""));
                    PdfPCell celdaColor2 = new PdfPCell(new Paragraph(""));
                    PdfPCell celdaColor3 = new PdfPCell(new Paragraph(""));
                    PdfPCell celdaColor4 = new PdfPCell(new Paragraph(""));
                    PdfPCell celdaColor5 = new PdfPCell(new Paragraph(""));
                    celdaColor.BackgroundColor = new BaseColor(181, 25, 70);
                    celdaColor2.BackgroundColor = new BaseColor(150, 23, 46);
                    celdaColor3.BackgroundColor = new BaseColor(238, 175, 48);
                    celdaColor4.BackgroundColor = new BaseColor(224, 0, 52);
                    celdaColor5.BackgroundColor = new BaseColor(252, 217, 0);
                    celdaColor.FixedHeight = 10f;
                    tabColor.AddCell(celdaColor);
                    tabColor.AddCell(celdaColor2);
                    tabColor.AddCell(celdaColor3);
                    tabColor.AddCell(celdaColor4);
                    tabColor.AddCell(celdaColor5);
                    tabColor.SetWidthPercentage(new float[] { 400, 50, 80, 25, 110 }, PageSize.A4);
                    for (int i = 0; i < tabColor.Rows.Count; i++)
                    {
                        if (i <= 4)
                        {
                            hfClass.quitaBordes(i, tabColor);
                        }
                    }
                    pdfDoc.Add(tabColor);

                    //AQUI EMPIEZA APARTADO DE DATOS
                    pdfDoc.Add(new Chunk(""));
                    tablaDatos1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaDatos1.SetWidthPercentage(new float[] { 298, 298 }, PageSize.A4);

                    if (v.concepto_x )
                    { PdfPCell celda1 = new PdfPCell(new Paragraph(v.concepto, negritaPeque)); celda1.Border = 0; tablaDatos1.AddCell(celda1); }
                    else
                    { PdfPCell celda1 = new PdfPCell(new Paragraph("", negritaPeque)); celda1.Border = 0; tablaDatos1.AddCell(celda1); }

                    if (v.folio_x )
                    { PdfPCell celda2 = new PdfPCell(new Paragraph(text.Where(x => x.CAMPO == "folio").Select(x => x.TEXTO).First() + " " + v.folio, negritaPeque)); celda2.HorizontalAlignment = Element.ALIGN_RIGHT; celda2.Border = 0; tablaDatos1.AddCell(celda2); }
                    else
                    { PdfPCell celda2 = new PdfPCell(new Paragraph("", normalPeque)); celda2.HorizontalAlignment = Element.ALIGN_RIGHT; celda2.Border = 0; tablaDatos1.AddCell(celda2); }

                    PdfPCell celdaB1 = new PdfPCell(new Paragraph("\n", negritaPeque)); celdaB1.Border = 0; tablaDatos1.AddCell(celdaB1);
                    PdfPCell celdaB2 = new PdfPCell(new Paragraph("\n", negritaPeque)); celdaB2.Border = 0; tablaDatos1.AddCell(celdaB2);

                    if (v.payerNom_x )
                    { PdfPCell celda3 = new PdfPCell(new Paragraph(v.payerNom, negritaPeque)); celda3.Border = 0; tablaDatos1.AddCell(celda3); }
                    else
                    { PdfPCell celda3 = new PdfPCell(new Paragraph("", normalPeque)); celda3.Border = 0; tablaDatos1.AddCell(celda3); }

                    if (v.lugarFech_x )
                    { PdfPCell celda4 = new PdfPCell(new Paragraph(v.lugarFech, negritaPeque)); celda4.HorizontalAlignment = Element.ALIGN_RIGHT; celda4.Border = 0; tablaDatos1.AddCell(celda4); }
                    else
                    { PdfPCell celda4 = new PdfPCell(new Paragraph("", normalPeque)); celda4.HorizontalAlignment = Element.ALIGN_RIGHT; celda4.Border = 0; tablaDatos1.AddCell(celda4); }

                    if (v.cliente_x )
                    { PdfPCell celda5 = new PdfPCell(new Paragraph(v.cliente, negritaPeque)); celda5.Border = 0; tablaDatos1.AddCell(celda5); }
                    else
                    { PdfPCell celda5 = new PdfPCell(new Paragraph("", negritaPeque)); celda5.Border = 0; tablaDatos1.AddCell(celda5); }

                    if (v.lugar_x )
                    { PdfPCell celda6 = new PdfPCell(new Paragraph(v.lugar, negritaPeque)); celda6.HorizontalAlignment = Element.ALIGN_RIGHT; celda6.Border = 0; tablaDatos1.AddCell(celda6); }
                    else
                    { PdfPCell celda6 = new PdfPCell(new Paragraph("", negritaPeque)); celda6.HorizontalAlignment = Element.ALIGN_RIGHT; celda6.Border = 0; tablaDatos1.AddCell(celda6); }

                    if (v.puesto_x )
                    { PdfPCell celda7 = new PdfPCell(new Paragraph(v.puesto, normalPeque)); celda7.Border = 0; tablaDatos1.AddCell(celda7); }
                    else
                    { PdfPCell celda7 = new PdfPCell(new Paragraph("", normalPeque)); celda7.Border = 0; tablaDatos1.AddCell(celda7); }

                    if (v.payerId_x )
                    { PdfPCell celda8 = new PdfPCell(new Paragraph(text.Where(x => x.CAMPO == "control").Select(x => x.TEXTO).First(), negritaPeque)); celda8.BackgroundColor = new BaseColor(204, 204, 204); tablaDatos1.AddCell(celda8); }
                    else
                    { PdfPCell celda8 = new PdfPCell(new Paragraph("", negritaPeque)); celda8.Border = 0; tablaDatos1.AddCell(celda8); }

                    if (v.direccion_x )
                    { PdfPCell celda9 = new PdfPCell(new Paragraph(v.direccion, normalPeque)); celda9.Border = 0; tablaDatos1.AddCell(celda9); }
                    else
                    { PdfPCell celda9 = new PdfPCell(new Paragraph("", normalPeque)); celda9.Border = 0; tablaDatos1.AddCell(celda9); }

                    if (v.payerId_x )
                    { PdfPCell celda10 = new PdfPCell(new Paragraph(text.Where(x => x.CAMPO == "payer").Select(x => x.TEXTO).First() + " " + v.payerId, normalPeque)); tablaDatos1.AddCell(celda10); }
                    else
                    { PdfPCell celda10 = new PdfPCell(new Paragraph("", normalPeque)); celda10.Border = 0; tablaDatos1.AddCell(celda10); }

                    float var = tablaDatos1.TotalHeight;
                    pdfDoc.Add(tablaDatos1);

                    //APARTIR DE AQUI VA EL ESTIMADO
                    pdfDoc.Add(new Chunk("\n"));
                    Phrase fraseEstimado = new Phrase();

                    if (v.estimado_x )
                    {
                        fraseEstimado.Add(new Paragraph(text.Where(x => x.CAMPO == "estimado").Select(x => x.TEXTO).First() + " " + v.estimado, negritaPeque));
                    }
                    else
                    {
                        fraseEstimado.Add("");
                    }
                    pdfDoc.Add(fraseEstimado);

                    //APARTIR DE AQUI VA LA MECANICA
                    pdfDoc.Add(new Chunk("\n"));
                    pdfDoc.Add(new Chunk("\n"));
                    Phrase miFrase = new Phrase();

                    if (v.mecanica_x)
                    {
                        miFrase.Add(new Paragraph(v.mecanica, normalPeque));
                    }
                    else
                    {
                        miFrase.Add("");
                    }
                    pdfDoc.Add(miFrase);

                    //AQUI COMIENZAN LAS TABLAS
                    //1.- TABLA DE MATERIALES
                    int tablas = v.listaFechas.Count; //SE RECIBE LA N CANTIDAD DE TABLAS A GENERAR
                    int cols = v.numColEncabezado.Count; // SE RECIBE LA CANTIDAD DE COLUMNAS A CONTENER LA TABLA
                    int tamaño = 0;
                    //B20180726 MGC 2018.07.26
                    try
                    {
                        tamaño = 600 / cols;
                    }
                    catch (Exception)
                    {
                        tamaño = 0;
                    }
                    for (int a = 0; a < tablas; a++)
                    {
                        PdfPTable tablasN = new PdfPTable(cols);

                        if (cols == 1)
                        { tablasN.SetWidthPercentage(new float[] { tamaño }, PageSize.A4); }
                        else if (cols == 2)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 3)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 4)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 5)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 6)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 7)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 8)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 9)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }
                        else if (cols == 10)
                        { tablasN.SetWidthPercentage(new float[] { tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño, tamaño }, PageSize.A4); }

                        PdfPCell rangoFecha = new PdfPCell(new Paragraph(text.Where(x => x.CAMPO == "de").Select(x => x.TEXTO).First() + " " + v.listaFechas[a].Remove(10) + " " + text.Where(x => x.CAMPO == "a").Select(x => x.TEXTO).First() + " " + (v.listaFechas[a].Remove(0, v.listaFechas[a].Length / 2)).Remove(10)));
                        rangoFecha.Border = 0;
                        rangoFecha.Colspan = cols;
                        tablasN.AddCell(rangoFecha);

                        foreach (var celCabecera in v.numColEncabezado)
                        {
                            PdfPCell celdaCabeza = new PdfPCell();
                            celdaCabeza.AddElement(new Paragraph(celCabecera, letraTabNegrita));
                            celdaCabeza.BackgroundColor = new BaseColor(204, 204, 204);
                            tablasN.AddCell(celdaCabeza);
                        }

                        int columnas = 0;
                        int filas = 0;
                        columnas = cols;
                        filas = v.numfilasTabla[a];

                        for (int i = 0; i < filas; i++)
                        {

                            for (int j = 0; j < columnas; j++)
                            {
                                //B20180710 MGC 2018.07.10 Modificaciones para editar los campos de distribución se agrego los objetos
                                //tablasN.AddCell(new Paragraph(v.listaCuerpo[pos], letraTab));
                                tablasN.AddCell(new Paragraph(v.listaCuerpo[pos], letraTab));
                                pos++;
                            }
                        }

                        pdfDoc.Add(tablasN);
                        pdfDoc.Add(new Chunk("\n"));
                    }

                    //APARTIR DE AQUI VA EL MONTO
                    miFrase.Clear();
                    if (v.monto_x )
                    {
                        //pdfDoc.Add(new Chunk("\n"));
                        miFrase.Add(new Paragraph(text.Where(x => x.CAMPO == "monto").Select(x => x.TEXTO).First() + " " + v.monto + " " + v.moneda, normalPeque));
                    }
                    else
                    {
                        miFrase.Add("");
                    }
                    pdfDoc.Add(miFrase);

                    //APARTIR DE AQUI VA LA SEGUNDA TABLA
                    //2.- TABLA DE RECURRENCIAS
                    if (v.secondTab_x )
                    {
                        pdfDoc.Add(new Paragraph(text.Where(x => x.CAMPO == "tituloRec").Select(x => x.TEXTO).First(),  negritaPeque));
                        pdfDoc.Add(new Chunk("\n"));
                        int cols2 = v.numColEncabezado2.Count;
                        int tamaño2 = 600 / cols2;
                        PdfPTable tablasN2 = new PdfPTable(cols2);

                        if (cols2==5) {
                            tablasN2.SetWidthPercentage(new float[] { tamaño2, tamaño2, tamaño2, tamaño2, tamaño2 }, PageSize.A4);
                        }
                        else if (cols2 == 4)
                        {
                            tablasN2.SetWidthPercentage(new float[] { tamaño2, tamaño2, tamaño2, tamaño2 }, PageSize.A4);
                        }

                        foreach (var celCabecera2 in v.numColEncabezado2)
                        {
                            PdfPCell celdaCabeza2 = new PdfPCell();
                            celdaCabeza2.AddElement(new Paragraph(celCabecera2, letraTabNegrita));
                            celdaCabeza2.BackgroundColor = new BaseColor(204, 204, 204);
                            tablasN2.AddCell(celdaCabeza2);
                        }

                        for (int i = 0; i < v.numfilasTabla2; i++)
                        {
                            for (int j = 0; j < cols2; j++)
                            {
                                tablasN2.AddCell(new Paragraph(v.listaCuerpoRec[pos2], letraTab));
                                pos2++;
                            }
                        }

                        pdfDoc.Add(tablasN2);
                        pdfDoc.Add(new Chunk("\n"));
                    }

                    //APARTIR DE AQUI VA LA TERCER TABLA
                    //2.- TABLA DE OBJETIVO Q
                    if (v.tercerTab_x)
                    {
                        pdfDoc.Add(new Paragraph(text.Where(x => x.CAMPO == "tituloObjQ").Select(x => x.TEXTO).First(),  negritaPeque));
                        pdfDoc.Add(new Chunk("\n"));
                        int cols3 = v.numColEncabezado3.Count;
                        int tamano3 = 600 / cols3;
                        PdfPTable tablasN3 = new PdfPTable(cols3);

                        if (cols3 == 5)
                        {
                            tablasN3.SetWidthPercentage(new float[] { tamano3, tamano3, tamano3, tamano3, tamano3 }, PageSize.A4);
                        }

                        foreach (var celCabecera3 in v.numColEncabezado3)
                        {
                            PdfPCell celdaCabeza3 = new PdfPCell();
                            celdaCabeza3.AddElement(new Paragraph(celCabecera3, letraTabNegrita));
                            celdaCabeza3.BackgroundColor = new BaseColor(204, 204, 204);
                            tablasN3.AddCell(celdaCabeza3);
                        }

                        for (int i = 0; i < v.numfilasTabla3; i++)
                        {
                            for (int j = 0; j < cols3; j++)
                            {
                                tablasN3.AddCell(new Paragraph(v.listaCuerpoObjQ[pos3], letraTab));
                                pos3++;
                            }
                        }

                        pdfDoc.Add(tablasN3);
                        pdfDoc.Add(new Chunk("\n"));
                    }
                    //APARTIR DE AQUI VAN LAS FIRMAS
                    //LINEAS PARA LA FIRMA EN UNA TABLA
                    //pdfDoc.Add(new Chunk("\n"));
                    PdfPCell celFirma1 = new PdfPCell();
                    PdfPCell celFirma2 = new PdfPCell();

                    PdfPTable tabFirma1 = new PdfPTable(1);
                    PdfPCell celFirmita1 = new PdfPCell();
                    if (v.nombreE_x  || v.puestoE_x  || v.companyC_x )
                    { celFirmita1.AddElement(new Paragraph("\n", normalPeque)); celFirmita1.Border = 2; }
                    else
                    { celFirmita1.AddElement(new Paragraph("", normalPeque)); celFirmita1.Border = 0; }
                    tabFirma1.AddCell(celFirmita1);
                    tabFirma1.SetWidthPercentage(new float[] { 450 }, PageSize.A4);

                    PdfPTable tabFirma2 = new PdfPTable(1);
                    PdfPCell celFirmita2 = new PdfPCell();
                    if (v.nombreC_x  || v.puestoC_x  || v.companyCC_x )
                    { celFirmita2.AddElement(new Paragraph("\n", normalPeque)); celFirmita2.Border = 2; }
                    else
                    { celFirmita2.AddElement(new Paragraph("", normalPeque)); celFirmita2.Border = 0; }
                    tabFirma2.AddCell(celFirmita2);
                    tabFirma2.SetWidthPercentage(new float[] { 450 }, PageSize.A4);

                    celFirma1.AddElement(tabFirma1);
                    celFirma1.Border = 0;

                    celFirma2.AddElement(tabFirma2);
                    celFirma2.Border = 0;

                    tablaDatos2.AddCell(celFirma1);
                    tablaDatos2.AddCell(celFirma2);
                    tablaDatos2.SetWidthPercentage(new float[] { 300, 300 }, PageSize.A4);

                    pdfDoc.Add(tablaDatos2);

                    //DATOS PARA LAS FIRMAS
                    tablaDatos3.HorizontalAlignment = Element.ALIGN_LEFT;
                    tablaDatos3.SetWidthPercentage(new float[] { 298, 298 }, PageSize.A4);

                    if (v.nombreE_x )
                    { PdfPCell celda1Dat3 = new PdfPCell(new Paragraph(v.nombreE, negritaPeque)); celda1Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda1Dat3); }
                    else
                    { PdfPCell celda1Dat3 = new PdfPCell(new Paragraph("", negritaPeque)); celda1Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda1Dat3); }

                    if (v.nombreC_x )
                    { PdfPCell celda2Dat3 = new PdfPCell(new Paragraph(v.nombreC, negritaPeque)); celda2Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda2Dat3); }
                    else
                    { PdfPCell celda2Dat3 = new PdfPCell(new Paragraph("", negritaPeque)); celda2Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda2Dat3); }

                    if (v.puestoE_x )
                    { PdfPCell celda3Dat3 = new PdfPCell(new Paragraph(v.puestoE, normalPeque)); celda3Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda3Dat3); }
                    else
                    { PdfPCell celda3Dat3 = new PdfPCell(new Paragraph("", normalPeque)); celda3Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda3Dat3); }

                    if (v.puestoC_x )
                    { PdfPCell celda4Dat3 = new PdfPCell(new Paragraph(v.puestoC, normalPeque)); celda4Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda4Dat3); }
                    else
                    { PdfPCell celda4Dat3 = new PdfPCell(new Paragraph("", normalPeque)); celda4Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda4Dat3); }

                    if (v.companyC_x )
                    { PdfPCell celda5Dat3 = new PdfPCell(new Paragraph(v.companyC, negritaPeque)); celda5Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda5Dat3); }
                    else
                    { PdfPCell celda5Dat3 = new PdfPCell(new Paragraph("", negritaPeque)); celda5Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda5Dat3); }

                    if (v.companyCC_x )
                    { PdfPCell celda6Dat3 = new PdfPCell(new Paragraph(v.companyCC, negritaPeque)); celda6Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda6Dat3); }
                    else
                    { PdfPCell celda6Dat3 = new PdfPCell(new Paragraph("", negritaPeque)); celda6Dat3.HorizontalAlignment = Element.ALIGN_CENTER; tablaDatos3.AddCell(celda6Dat3); }

                    for (int i = 0; i < tablaDatos3.Rows.Count; i++)
                    {
                        if (i <= 4)
                        {
                            hfClass.quitaBordes(i, tablaDatos3);
                        }
                    }
                    pdfDoc.Add(tablaDatos3);


                    //TABLA PARA LOS COMENTARIOS
                    pdfDoc.Add(new Chunk("\n"));
                    tabComentarios.HorizontalAlignment = Element.ALIGN_LEFT;
                    tabComentarios.SetWidthPercentage(new float[] { 300, 300 }, PageSize.A4);

                    if (v.comentarios_x )
                    { PdfPCell celda1 = new PdfPCell(new Paragraph(v.comentarios, normalPeque)); celda1.Border = 0; tabComentarios.AddCell(celda1); }
                    else
                    { PdfPCell celda1 = new PdfPCell(new Paragraph("", normalPeque)); celda1.Border = 0; tabComentarios.AddCell(celda1); }

                    if (v.compromisoK_x )
                    { PdfPCell celda2 = new PdfPCell(new Paragraph(v.compromisoK, normalPeque)); celda2.Border = 0; tabComentarios.AddCell(celda2); }
                    else
                    { PdfPCell celda2 = new PdfPCell(new Paragraph("", normalPeque)); celda2.Border = 0; tabComentarios.AddCell(celda2); }

                    if (v.compromisoC_x )
                    { PdfPCell celda3 = new PdfPCell(new Paragraph("\n" + v.compromisoC, normalPeque)); celda3.Border = 0; tabComentarios.AddCell(celda3); }
                    else
                    { PdfPCell celda3 = new PdfPCell(new Paragraph("", normalPeque)); celda3.Border = 0; tabComentarios.AddCell(celda3); }

                    PdfPCell celVacia = new PdfPCell(new Paragraph("", normalPeque));
                    celVacia.Border = 0;
                    tabComentarios.AddCell(celVacia);

                    pdfDoc.Add(tabComentarios);
                    pdfDoc.Close();

                    ruta = "/PdfTemp/" + nombreArchivo;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                    Log.ErrorLogApp(ex,"CartaV","Generar PDF");
                }
                return ruta;
            }
        }
    }
}