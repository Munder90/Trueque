using ClosedXML.Excel;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Services;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class LAContabilizacionController : Controller
    {
        private TruequeEntities db = new TruequeEntities();
        // GET: LAContabilizacion
        public ActionResult Index()
        {
            FnCommon.ObtenerConfPage(db, 560, User.Identity.Name, this.ControllerContext.Controller);
            string spras_id = ViewBag.spras_id;
            return View();
        }

        // GET: LAContabilizacion/Create
        [HttpPost]
        public ActionResult Descargar()
        {
            List<FileStreamResult> excels = new List<FileStreamResult>();
            List<SolxContabilizar> doctosxconta = new List<SolxContabilizar>();
            var user = db.USUARIOs.Where(t => t.ID == User.Identity.Name).SingleOrDefault();
            foreach (var cocode in db.SOCIEDADs.Where(t=>t.ACTIVO))
            {
                List<CSP_DOCUMENTOSXCOCODE_Result> dOCUMENTOesc = db.CSP_DOCUMENTOSXCOCODE(cocode.BUKRS, user.SPRAS_ID).ToList();
                List<ESTATU> eec = db.ESTATUS.Where(x => x.ACTIVO == true).ToList();
                foreach (CSP_DOCUMENTOSXCOCODE_Result item in dOCUMENTOesc)
                {
                    SolxContabilizar ld = new SolxContabilizar();

                    ld.NUM_DOC = item.NUM_DOC;
                    ld.SOCIEDAD_ID = item.SOCIEDAD_ID;

                    if (item.ESTATUS == "R")
                    {
                        FLUJO flujo = db.FLUJOes.Include("USUARIO").Where(x => x.NUM_DOC == item.NUM_DOC & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault();
                        item.ESTATUSS = item.ESTATUSS.Substring(0, 6) +
                                        (flujo.USUARIO != null ? flujo.USUARIO.PUESTO_ID.ToString() : "") +
                                        item.ESTATUSS.Substring(6, 2);
                    }
                    else
                    {
                        item.ESTATUSS = item.ESTATUSS.Substring(0, 6) + " " + item.ESTATUSS.Substring(6, 2); ;
                    }
                    Estatus e = new Estatus();
                    ld.STATUS_ID = e.getID(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID, eec);
                    if(ld.STATUS_ID==90)
                        doctosxconta.Add(ld);
                }
            }
            //List<SolxContabilizar> doctosxconta = db.DOCUMENTOes.Where(t => t.ESTATUS_SAP == "P").Select(x => new SolxContabilizar {NUM_DOC=x.NUM_DOC,SOCIEDAD_ID= x.SOCIEDAD_ID }).ToList();
            var cocodes = doctosxconta.DistinctBy(x => x.SOCIEDAD_ID).ToList();
            foreach (var co in cocodes)
            {
                var lista = doctosxconta.Where(t => t.SOCIEDAD_ID == co.SOCIEDAD_ID).ToList();
                var excel=  generarExcel(lista,co.SOCIEDAD_ID);
                excels.Add(excel);
            }
            var outputStream = new MemoryStream();
            using (var zip = new ZipFile())
            {
                foreach (var f in excels)
                {
                    zip.AddEntry(f.FileDownloadName, f.FileStream);
                }
                zip.Save(outputStream);
            }

            outputStream.Position = 0;
            return File(outputStream, "application/zip", "SolxContabilizar"+DateTime.Now.ToString("ddMMyyyy")+".zip");
        }

        public FileStreamResult generarExcel(List<SolxContabilizar> lst,string cocode)
        {
            ArchivoContable acontable = new ArchivoContable();
            List<ArchivoC> l_ac = new List<ArchivoC>();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            foreach (var l in lst)
            {
                acontable.generarArchivo(l.NUM_DOC, 0, 0, "L", ref l_ac);
            }
            var fila = 1;
            foreach(var la in l_ac)
            {
                worksheet.Cell(fila, 1).Value = la.tab.TIPO_DOC;
                worksheet.Cell(fila, 2).Value = la.tab.SOCIEDAD.Trim();
                worksheet.Cell(fila, 3).Value = la.tab.FECHA_DOCU.Trim();
                worksheet.Cell(fila, 4).Value = la.tab.FECHA_DOCU.Trim();
                worksheet.Cell(fila, 5).Value = la.doc.MONEDA_ID.Trim();
                worksheet.Cell(fila, 6).Value = la.tab.HEADER_TEXT.Trim();
                worksheet.Cell(fila, 7).Value = la.tab.REFERENCIA.Trim();
                worksheet.Cell(fila, 8).Value = la.tab.CALC_TAXT.ToString().Replace("True", "X").Replace("False", "");
                worksheet.Cell(fila, 9).Value = la.tab.NOTA.Trim();
                worksheet.Cell(fila, 10).Value = la.tab.CORRESPONDENCIA.Trim();
                var fdetalle = fila+2;
                foreach (var d in la.det)
                {
                    worksheet.Cell(fdetalle, 1).Value = d.POS_TYPE;
                    worksheet.Cell(fdetalle, 2).Value = d.COMP_CODE;
                    worksheet.Cell(fdetalle, 3).Value = d.BUS_AREA;
                    worksheet.Cell(fdetalle, 4).Value = d.POST_KEY;
                    worksheet.Cell(fdetalle, 5).Value = d.ACCOUNT;
                    worksheet.Cell(fdetalle, 6).Value = d.COST_CENTER;
                    worksheet.Cell(fdetalle, 7).Value = d.BALANCE;
                    worksheet.Cell(fdetalle, 8).Value = d.TEXT;
                    worksheet.Cell(fdetalle, 9).Value ="'"+ d.SALES_ORG;
                    worksheet.Cell(fdetalle, 10).Value = d.DIST_CHANEL;
                    worksheet.Cell(fdetalle, 11).Value = d.DIVISION;
                    worksheet.Cell(fdetalle, 12).Value = d.INV_REF;
                    worksheet.Cell(fdetalle, 13).Value = d.PAY_TERM;
                    worksheet.Cell(fdetalle, 14).Value = d.JURIS_CODE;
                    worksheet.Cell(fdetalle, 15).Value = "'"+d.CUSTOMER;
                    worksheet.Cell(fdetalle, 16).Value = "'"+d.PRODUCT;
                    worksheet.Cell(fdetalle, 17).Value = d.TAX_CODE;
                    worksheet.Cell(fdetalle, 18).Value = d.PLANT;
                    worksheet.Cell(fdetalle, 19).Value = d.REF_KEY1;
                    worksheet.Cell(fdetalle, 20).Value = d.REF_KEY2;
                    worksheet.Cell(fdetalle, 21).Value = d.REF_KEY3;
                    worksheet.Cell(fdetalle, 22).Value = "'"+d.ASSIGNMENT;
                    worksheet.Cell(fdetalle, 23).Value = d.QTY;
                    worksheet.Cell(fdetalle, 24).Value = d.BASE_UNIT;
                    worksheet.Cell(fdetalle, 25).Value = d.AMOUNT_LC;
                    worksheet.Cell(fdetalle, 26).Value = d.RETENCION_ID;
                    fdetalle += 1;
                }
                fila =fdetalle+ 2;
            }
            worksheet.Columns().AdjustToContents();
            var fileDownloadName = "SolxContabilizar_"+cocode+"_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fileStream = new MemoryStream();
            workbook.SaveAs(fileStream);
            fileStream.Position = 0;

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;

        }


    }
}
