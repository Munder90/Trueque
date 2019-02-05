using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class DocRelacionController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: DocRelacion
        public ActionResult Index()
        {
            int pagina_id = 1210; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            try
            {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            return View(db.FACTURASCONFs.ToList());
        }

        // GET: DocRelacion/Details/5
        public ActionResult Details(string id, string pais, string tsol)
        {
            int pagina_id = 1212; //ID EN BASE DE DATOS
            int pagina2 = 1210;
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller,pagina2);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //ViewBag.pais = "mx.svg";
                    //return RedirectToAction("Pais", "Home");
                }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rel = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == id && x.PAIS_ID == pais && x.TSOL == tsol).FirstOrDefault();
             if (id == null)
            {
                return HttpNotFound();
            }
            return View(rel);
        }
        // GET: DocRelacion/Create
        public ActionResult Create()
        {
            int pagina_id = 1211; //ID EN BASE DE DATOS
            int pagina2 = 1210;
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, pagina2);

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            return View();
        }

        // POST: DocRelacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SOCIEDAD_ID,PAIS_ID,TSOL,FACTURA,FECHA,PROVEEDOR,CONTROL,AUTORIZACION,VENCIMIENTO,FACTURAK,EJERCICIOK,BILL_DOC,BELNR,IMPORTE_FAC,PAYER,DESCRIPCION,SOCIEDAD,ACTIVO")] FACTURASCONF fACTURASCONF)
        {
            try
            {
                db.FACTURASCONFs.Add(fACTURASCONF);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                int pagina_id = 1211; //ID EN BASE DE DATOS
                int pagina2 = 1210; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, pagina2);

            }

            
            return View(fACTURASCONF);
        }

        // GET: DocRelacion/Edit/5
        public ActionResult Edit(string id, string pais, string tsol)
        {
            int pagina_id = 1213; //ID EN BASE DE DATOS
            int pagina2 = 1210;
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, pagina2);

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rel = db.FACTURASCONFs.Where(x => x.SOCIEDAD_ID == id && x.PAIS_ID == pais && x.TSOL == tsol).FirstOrDefault();
    
            if (id == null)
            {
                return HttpNotFound();
            }

            return View(rel);
        }

        // POST: DocRelacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOCIEDAD_ID,PAIS_ID,TSOL,FACTURA,FECHA,PROVEEDOR,CONTROL,AUTORIZACION,VENCIMIENTO,FACTURAK,EJERCICIOK,BILL_DOC,BELNR,IMPORTE_FAC,PAYER,DESCRIPCION,SOCIEDAD,ACTIVO")] FACTURASCONF fACTURASCONF)
        {
            if (ModelState.IsValid)
            {       
                db.Entry(fACTURASCONF).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int pagina_id = 1213; //ID EN BASE DE DATOS
            int pagina2 = 1210;
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, pagina2);

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            return View(fACTURASCONF);
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var fc = db.FACTURASCONFs.ToList();
           generarExcelHome(fc, Server.MapPath("~/PdfTemp/"));
            return File(Server.MapPath("~/PdfTemp/DocRel" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocPr" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<FACTURASCONF> lst, string ruta)
        {
            string spras_id = FnCommon.ObtenerSprasId(db,User.Identity.Name);
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
                {new {COL = "CO. CODE"} };
                worksheet.Cell("B1").Value = new[]
                {new {COL = "PAÍS"}};
                worksheet.Cell("C1").Value = new[]
                {new {COL = "TIPO SOLICITUD"}};
                worksheet.Cell("D1").Value = new[]
                {new {COL = "FACTURA"}};
                worksheet.Cell("E1").Value = new[]
                {new {COL = "FECHA"}};
                worksheet.Cell("F1").Value = new[]
               {new {COL = "PROVEEDOR"}};
                worksheet.Cell("G1").Value = new[]
               {new {COL = "CONTROL"}};
                worksheet.Cell("H1").Value = new[]
               {new {COL = "AUTORIZACIÓN"}};
                worksheet.Cell("I1").Value = new[]
              {new {COL = "VENCIMIENTO"}};
                worksheet.Cell("J1").Value = new[]
              {new {COL = "FACTURA KELLOGG"}};
                worksheet.Cell("K1").Value = new[]
             {new {COL = "AÑO"}};
                worksheet.Cell("L1").Value = new[]
             {new {COL = "DOC. FACTURA"}};
                worksheet.Cell("M1").Value = new[]
             {new {COL = "FOLIO"}};
                worksheet.Cell("N1").Value = new[]
             {new {COL = "IMPORTE"}};
                worksheet.Cell("O1").Value = new[]
             {new {COL = "CLIENTE"}};
                worksheet.Cell("P1").Value = new[]
             {new {COL = "DESCRIPCIÓN"}};
                worksheet.Cell("Q1").Value = new[]
             {new {COL = "CO. CODE"}};
                worksheet.Cell("R1").Value = new[]
             {new {COL = "ACTIVO"}};

                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    string sociedad_id = lst[i - 2].SOCIEDAD_ID;
                    string tsol_id = lst[i - 2].TSOL;
                    SOCIEDAD sociedad = db.SOCIEDADs.First(x=>x.BUKRS== sociedad_id);
                    TSOL tSol = db.TSOLs.Include(x=>x.TSOLTs).First(x => x.ID == tsol_id);

                    worksheet.Cell("A" + i).Value = new[]
                    {new { COL       = (sociedad.BUKRS+" - "+sociedad.BUTXT)}};
                    worksheet.Cell("B" + i).Value = new[]
                    {new {COL       = lst[i-2].PAIS_ID}};
                    worksheet.Cell("C" + i).Value = new[]
                    {new {COL       = (tSol.ID+" - "+(tSol.TSOLTs.Any(x=>x.SPRAS_ID==spras_id)?tSol.TSOLTs.First(x=>x.SPRAS_ID==spras_id).TXT50:tSol.DESCRIPCION))}};
                    worksheet.Cell("D" + i).Value = new[]
                    { new {COL       = ObtenerTextoSINO(lst[i-2].FACTURA)}};
                    worksheet.Cell("E" + i).Value = new[]
                     { new {COL      = ObtenerTextoSINO(lst[i-2].FECHA)}};
                    worksheet.Cell("F" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].PROVEEDOR)}};
                    worksheet.Cell("G" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].CONTROL)}};
                    worksheet.Cell("H" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].AUTORIZACION)}};
                    worksheet.Cell("I" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].VENCIMIENTO)}};
                    worksheet.Cell("J" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].FACTURAK)}};
                    worksheet.Cell("K" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].EJERCICIOK)}};
                    worksheet.Cell("L" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].BILL_DOC)}};
                    worksheet.Cell("M" + i).Value = new[]
                   { new {COL      = ObtenerTextoSINO(lst[i-2].IMPORTE_FAC)}};
                    worksheet.Cell("N" + i).Value = new[]
                  { new {COL      = ObtenerTextoSINO(lst[i-2].BELNR)}};
                    worksheet.Cell("O" + i).Value = new[]
                  { new {COL      = ObtenerTextoSINO(lst[i-2].DESCRIPCION)}};
                    worksheet.Cell("P" + i).Value = new[]
                  { new {COL      = ObtenerTextoSINO(lst[i-2].BELNR)}};
                    worksheet.Cell("Q" + i).Value = new[]
                  { new {COL      = ObtenerTextoSINO(lst[i-2].SOCIEDAD)}};
                    worksheet.Cell("R" + i).Value = new[]
                 { new {COL      = ObtenerTextoSINO(lst[i-2].ACTIVO)}};
                }
                var rt = ruta + @"\DocRel" + DateTime.Now.ToShortDateString() + ".xlsx";
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e,"DocRelacion","generarExcelHome");
            }

        }

        string ObtenerTextoSINO(bool val)
        {
            return (val?"SI":"NO");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
