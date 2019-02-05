using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class AyudaController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        enum Clasificacion
        {
            Usuario=1,
            Administrador,
            Politica,
            Definicion,
            Otro
        }
        // GET: Ayuda
        public ActionResult Index()
        {
            int pagina_id = 1500;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            string spras_id = ViewBag.spras_id;
            //if (ViewBag.rol == "Administrador")
                return View(db.DOCTOAYUDAs.ToList());
            //else return View("Error");
        }
        public ActionResult Consulta(int id)
        {
            int pagina_id = 1500;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id+id, User.Identity.Name, this.ControllerContext.Controller,pagina_id);
            string spras_id = ViewBag.spras_id;
            return View(db.DOCTOAYUDAs.Where(t => t.ID_CLASIFICACION == id && t.ACTIVO).ToList());
        }
        // GET: Ayuda/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCTOAYUDA dOCTOAYUDA = db.DOCTOAYUDAs.Find(id);
            if (dOCTOAYUDA == null)
            {
                return HttpNotFound();
            }
            return View(dOCTOAYUDA);
        }

        // GET: Ayuda/Create
        public ActionResult Create()
        {
            int pagina_id = 1506;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 1500);
            string spras_id = ViewBag.spras_id;
            ViewBag.ID_Clasificacion= new SelectList(db.DOCTOCLASIFTs.Where(t=>t.SPRAS_ID==spras_id), "ID_Clasificacion", "Texto");
            //if (ViewBag.rol == "Administrador")
                return View(new DOCTOAYUDA{ ACTIVO=true });
            //else return View("Error");
        }

        // POST: Ayuda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Documento,Nombre,ID_Clasificacion,Activo,Ruta_Documento")] DOCTOAYUDA dOCTOAYUDA, HttpPostedFileWrapper Ruta_Documento)
        {
            int pagina_id = 1506;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 1500);
            string spras_id = ViewBag.spras_id;
            ViewBag.ID_Clasificacion = new SelectList(db.DOCTOCLASIFTs.Where(t => t.SPRAS_ID == spras_id), "ID_Clasificacion", "Texto");
            if (ModelState.IsValid)
            {
                if (!ExtensionesNS.Contains(Ruta_Documento.ContentType))
                {
                    if (Ruta_Documento.ContentLength < 31457280 && Ruta_Documento.ContentLength > 0)//30 MB
                    {
                        dOCTOAYUDA.ID_DOCUMENTO = getID_Docto(dOCTOAYUDA.ID_CLASIFICACION);

                        var fileName = Path.GetFileName(Ruta_Documento.FileName);
                        dOCTOAYUDA.RUTA_DOCUMENTO = Path.Combine(
                            Server.MapPath("~/Archivos/DoctosAyuda"),dOCTOAYUDA.ID_DOCUMENTO+"_"+fileName);
                        Ruta_Documento.SaveAs(dOCTOAYUDA.RUTA_DOCUMENTO);

                        dOCTOAYUDA.RUTA_DOCUMENTO = "Archivos/DoctosAyuda/" + dOCTOAYUDA.ID_DOCUMENTO+"_"+ Ruta_Documento.FileName;
                        db.DOCTOAYUDAs.Add(dOCTOAYUDA);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                        TempData["Filesize"]= "El archivo excede el tamaño máximo permitido";
                }
                else
                    TempData["Extension"]= "No se admiten archivos con el tipo de extensión seleccionado.";
            }
            dOCTOAYUDA.RUTA_DOCUMENTO = null;
            return View(dOCTOAYUDA);
        }

        // GET: Ayuda/Edit/5
        public ActionResult Edit(string id)
        {
            int pagina_id = 1507;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller,1500);
            string spras_id = ViewBag.spras_id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCTOAYUDA dOCTOAYUDA = db.DOCTOAYUDAs.Find(id);
            if (dOCTOAYUDA == null)
            {
                return HttpNotFound();
            }
            //if (ViewBag.rol == "Administrador")
                return View(dOCTOAYUDA);
            //else return View("Error");
        }

        // POST: Ayuda/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Documento,Nombre,ID_Clasificacion,Activo,Ruta_Documento")] DOCTOAYUDA dOCTOAYUDA, HttpPostedFileWrapper Ruta_Documento)
        {
            int pagina_id = 1507;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 1500);
            string spras_id = ViewBag.spras_id;
            if (ModelState.IsValid)
            {
                DOCTOAYUDA actdOCTOAYUDA = db.DOCTOAYUDAs.Find(dOCTOAYUDA.ID_DOCUMENTO);
                
                if (Ruta_Documento != null)//Se ha seleccionado un nuevo archivo
                {
                    if (!ExtensionesNS.Contains(Ruta_Documento.ContentType))
                    {
                        if (Ruta_Documento.ContentLength < 31457280 && Ruta_Documento.ContentLength > 0)//30 MB maximo
                        {
                            //Elimina el anterior
                            var eliminar = Path.Combine(
                                Server.MapPath("~/Archivos/DoctosAyuda"), actdOCTOAYUDA.RUTA_DOCUMENTO.Split('/').Last());
                            FileInfo file = new FileInfo(eliminar);
                            file.Delete();
                            //Guarda el nuevo archivo
                            var fileName = Path.GetFileName(Ruta_Documento.FileName);
                            dOCTOAYUDA.RUTA_DOCUMENTO = Path.Combine(
                                Server.MapPath("~/Archivos/DoctosAyuda"),dOCTOAYUDA.ID_DOCUMENTO+"_"+fileName);
                            Ruta_Documento.SaveAs(dOCTOAYUDA.RUTA_DOCUMENTO);

                            actdOCTOAYUDA.RUTA_DOCUMENTO = "Archivos/DoctosAyuda/" +dOCTOAYUDA.ID_DOCUMENTO+"_"+Ruta_Documento.FileName;
                            actdOCTOAYUDA.NOMBRE = dOCTOAYUDA.NOMBRE;
                            actdOCTOAYUDA.ACTIVO = dOCTOAYUDA.ACTIVO;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                            TempData["Filesize"] = "El archivo excede el tamaño máximo permitido";
                    }
                    else
                        TempData["Extension"] = "No se admiten archivos con el tipo de extensión seleccionado.";
                }
                else//No se actualiza el archivo, solo los demas campos
                {                  
                    actdOCTOAYUDA.NOMBRE = dOCTOAYUDA.NOMBRE;
                    actdOCTOAYUDA.ACTIVO = dOCTOAYUDA.ACTIVO;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }               
            }
            return View(dOCTOAYUDA);
        }

        // GET: Ayuda/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DOCTOAYUDA dOCTOAYUDA = db.DOCTOAYUDAs.Find(id);
        //    if (dOCTOAYUDA == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dOCTOAYUDA);
        //}

        // POST: Ayuda/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            DOCTOAYUDA dOCTOAYUDA = db.DOCTOAYUDAs.Find(id);
            db.DOCTOAYUDAs.Remove(dOCTOAYUDA);
            db.SaveChanges();
            //Ahora remueve el archivo
            var eliminar = Path.Combine(
                            Server.MapPath("~/Archivos/DoctosAyuda"), dOCTOAYUDA.RUTA_DOCUMENTO.Split('/').Last());
            FileInfo file = new FileInfo(eliminar);
            file.Delete();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string getID_Docto(int clasi)
        {
            var inicial = "";
            DOCTOAYUDA ult_docto = new DOCTOAYUDA();
            try
            {        
                if (clasi == 1 || clasi == 2)
                    inicial = "M";
                else if (clasi == 3)
                    inicial = "P";
                else if (clasi == 4)
                    inicial = "D";
                else
                    inicial = "O";
                if (clasi == 1 || clasi == 2)
                {
                    var doctos = db.DOCTOAYUDAs.Where(t => t.ID_CLASIFICACION == 1 || t.ID_CLASIFICACION == 2).ToList();
                    ult_docto = doctos.OrderByDescending(x => Convert.ToInt32(Regex.Replace(x.ID_DOCUMENTO, @"[^\d]", ""))).First();
                }
                else
                {
                    var doctos = db.DOCTOAYUDAs.Where(t => t.ID_CLASIFICACION == clasi).ToList();
                    ult_docto = doctos.OrderByDescending(x => Convert.ToInt32(Regex.Replace(x.ID_DOCUMENTO, @"[^\d]", ""))).First();
                }
                return inicial+ (Convert.ToInt16(ult_docto.ID_DOCUMENTO.Substring(1, ult_docto.ID_DOCUMENTO.Length-1)) + 1);
            }
            catch (Exception e)
            {
                return inicial+1;
            }
        }
        public List<string> ExtensionesNS = new List<string> { "application/bat", "application/x-bat","application/x-msdos-program","application/x-msdownload" };
    }
}
