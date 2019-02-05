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

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class IimpuestoController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Iimpuesto
        public ActionResult Index()
        {
            int pagina = 731; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
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
                ViewBag.lan = user.SPRAS_ID;
            }
            return View(db.IIMPUESTOes.Where(x => x.ACTIVO == true).ToList());
        }

        // GET: Iimpuesto/Details/5
        public ActionResult Details(string id, string id2, string id3)
        {
            int pagina = 732; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller,731);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
                //Busqueda de un registro que coincida con los parametros
                var con = db.IIMPUESTOes.Where(x => x.LAND == id && x.MWSKZ == id2 && x.KSCHL == id3).First();
                IIMPUESTO txx = new IIMPUESTO();
                txx.LAND = con.LAND;
                txx.MWSKZ = con.MWSKZ;
                txx.KSCHL = con.KSCHL;
                txx.KBETR = con.KBETR;
                return View(txx);
            }
        }

        // GET: Iimpuesto/Create
        public ActionResult Create()
        {
            int pagina = 734; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller,731);
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
                Session["spras"] = user.SPRAS_ID;
            }
            ViewBag.MWSKZ = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ");
            ViewBag.LAND = new SelectList(db.PAIS, "LAND", "SPRAS");
            return View();
        }

        // POST: Iimpuesto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LAND,MWSKZ,KSCHL,KBETR")] IIMPUESTO iIMPUESTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Automaticamente se activara
                    iIMPUESTO.ACTIVO = true;
                    iIMPUESTO.KSCHL = iIMPUESTO.KSCHL.ToUpper();
                    db.IIMPUESTOes.Add(iIMPUESTO);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.MWSKZ = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ", iIMPUESTO.MWSKZ);
                ViewBag.LAND = new SelectList(db.PAIS, "LAND", "SPRAS", iIMPUESTO.LAND);
                //Como se genera la lista se borra y se ocupa regenerar
                int pagina = 734; //ID EN BASE DE DATOS
                using (TruequeEntities db = new TruequeEntities())
                {
                    string u = User.Identity.Name;
                    //string u = "admin";
                    var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                    FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller,731);
                    ViewBag.error = "Campo Nulo";
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
                    Session["spras"] = user.SPRAS_ID;
                }
                return View(iIMPUESTO);
            }
            catch (Exception e)
            {
                //Como se genera la lista se borra y se ocupa regenerar
                int pagina = 734; //ID EN BASE DE DATOS
                using (TruequeEntities db = new TruequeEntities())
                {
                    string u = User.Identity.Name;
                    //string u = "admin";
                    var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                    FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller,731);
                    ViewBag.error = e.Message.ToString();
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
                    Session["spras"] = user.SPRAS_ID;
                }
                ViewBag.MWSKZ = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ");
                ViewBag.LAND = new SelectList(db.PAIS, "LAND", "SPRAS");
                return View();
            }
        }

        // GET: Iimpuesto/Edit/5
        public ActionResult Edit(string id, string id2, string id3)
        {
            int pagina = 733; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller,731);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
                var con = db.IIMPUESTOes.Where(x => x.LAND == id && x.MWSKZ == id2 && x.KSCHL == id3).First();
                IIMPUESTO txx = new IIMPUESTO();
                txx.LAND = con.LAND;
                txx.MWSKZ = con.MWSKZ;
                txx.KSCHL = con.KSCHL;
                txx.KBETR = con.KBETR;
                return View(txx);
            }
        }

        // POST: Iimpuesto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LAND,MWSKZ,KSCHL,KBETR")] IIMPUESTO iIMPUESTO)
        {
            if (ModelState.IsValid)
            {
                iIMPUESTO.ACTIVO = true;
                db.Entry(iIMPUESTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MWSKZ = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ", iIMPUESTO.MWSKZ);
            ViewBag.LAND = new SelectList(db.PAIS, "LAND", "SPRAS", iIMPUESTO.LAND);
            return View(iIMPUESTO);
        }

        // GET: Iimpuesto/Delete/5
        public ActionResult Delete(string id, string id2, string id3)
        {
            int pagina = 201; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                //string u = "admin";
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
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
                Session["spras"] = user.SPRAS_ID;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IIMPUESTO txx = db.IIMPUESTOes.Where(x => x.LAND == id && x.MWSKZ == id2 && x.KSCHL == id3).FirstOrDefault();
            return View(txx);
        }

        // POST: Iimpuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string id2, string id3)
        {
            IIMPUESTO iIMPUESTO = db.IIMPUESTOes.Where(x => x.LAND == id && x.MWSKZ == id2 && x.KSCHL == id3).FirstOrDefault();
            iIMPUESTO.ACTIVO = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult Descargar()
        {
            var pr = db.IIMPUESTOes.Where(x => x.ACTIVO == true).ToList();
            generarExcelHome(pr, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocIimp" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocIimp" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<IIMPUESTO> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
                {
                    new {
                        BANNER = "KSCHL"
                    },
                };
                worksheet.Cell("B1").Value = new[]
                {
                    new {
                        BANNER = "KBETR"
                    },
                };
                worksheet.Cell("C1").Value = new[]
                {
                    new {
                        BANNER = "MWSKZ"
                    },
                };
                worksheet.Cell("D1").Value = new[]
                {
                    new {
                        BANNER = "PAIS"
                    },
                };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].KSCHL
                        },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].KBETR
                        },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].IMPUESTO.MWSKZ
                        },
                    };
                    worksheet.Cell("D" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].PAI.LAND
                        },
                    };
                }
                var rt = ruta + @"\DocIimp" + DateTime.Now.ToShortDateString() + ".xlsx";
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }

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
