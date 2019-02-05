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

namespace TAT001.Controllers.Configuracion
{
    [Authorize]
    [LoginActive]
    public class TaxeohController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        // GET: Taxeoh
        public ActionResult Index(string vko, string vtw, string kun, string spa)
        {
            int pagina_id = 851; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
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
            
            var tAXEOHs = db.TAXEOHs.Include(t => t.CLIENTE).Include(t => t.IMPUESTO).Include(t => t.PAI).Include(t => t.SOCIEDAD).Include(t => t.TX_CONCEPTO).Include(t => t.TX_TNOTA);
            ViewBag.kun = kun;
            ViewBag.vko = vko;
            ViewBag.vtw = vtw;
            ViewBag.spa = spa;
            CLIENTE cli = db.CLIENTEs.Find(vko, vtw, spa, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(tAXEOHs.Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ACTIVO == true).ToList());
        }

        // GET: Taxeoh/Details/5
        public ActionResult Details(string kun, string vk, string con, string sc)
        {
            int pagina_id = 852; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller,851);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".svg";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }
            
            var coni = Convert.ToInt32(con);
            TAXEOH tAXEOH = db.TAXEOHs.Where(x => x.KUNNR == kun && x.VKORG == vk && x.CONCEPTO_ID == coni && x.SOCIEDAD_ID == sc).FirstOrDefault();
            if (tAXEOH == null)
            {
                return HttpNotFound();
            }
            //ViewBag.kun = kun;
            //ViewBag.vko = vko;
            //ViewBag.vtw = vtw;
            //ViewBag.spa = spa;
            CLIENTE cli = db.CLIENTEs.Find(vk, tAXEOH.VTWEG, tAXEOH.SPART, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(tAXEOH);
        }

        // GET: Taxeoh/Create
        public ActionResult Create(string kun, string vko, string vtw, string spa)
        {
            int pagina_id = 854; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 851);

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
            
            ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1");
            ViewBag.IMPUESTO_ID = new SelectList(db.IMPUESTOes.Where(x => x.ACTIVO), "MWSKZ", "MWSKZ");
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS");
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT");
            ViewBag.CONCEPTO_ID = new SelectList(db.TX_CONCEPTO, "ID", "DESCRIPCION");
            ViewBag.TNOTA_ID = new SelectList(db.TX_TNOTA, "ID", "DESCRIPCION");
            ViewBag.kun = kun;
            ViewBag.vko = vko;
            ViewBag.vtw = vtw;
            ViewBag.spa = spa;
            CLIENTE cli = db.CLIENTEs.Find(vko, vtw, spa, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View();
        }

        // POST: Taxeoh/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SOCIEDAD_ID,PAIS_ID,VKORG,VTWEG,SPART,KUNNR,CONCEPTO_ID,TNOTA_ID,FECHAC,USUARIOC_ID,FECHAM,USUARIOM_ID,IMPUESTO_ID,PORC,PAY_T,ACTIVO")] TAXEOH tx, string kun, string vko, string vtw, string spa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var por = db.IIMPUESTOes.Where(ip => ip.MWSKZ == tx.IMPUESTO_ID).FirstOrDefault();
                    var pa = db.SOCIEDADs.Where(x => x.BUKRS == tx.SOCIEDAD_ID).FirstOrDefault();
                    tx.KUNNR = kun;
                    tx.VKORG = vko;
                    tx.VTWEG = vtw;
                    tx.SPART = spa;
                    if (tx.PAY_T != null && tx.PAY_T != "")
                    {
                        tx.PAY_T = tx.PAY_T.ToUpper();
                    }
                    else
                    {
                        tx.PAY_T = "";
                    }
                    tx.PAIS_ID = pa.LAND;
                    tx.ACTIVO = true;
                    tx.FECHAC = DateTime.Now;
                    tx.USUARIOC_ID = User.Identity.Name;
                    tx.PORC = por.KBETR;
                    db.TAXEOHs.Add(tx);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { vko = tx.VKORG, vtw = tx.VTWEG, kun = tx.KUNNR, spa = tx.SPART });
                }
            }
            catch (Exception e)
            {
            }
            int pagina_id = 854; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 851);

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
            ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1", tx.VKORG);
            ViewBag.IMPUESTO_ID = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ", tx.IMPUESTO_ID);
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", tx.PAIS_ID);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", tx.SOCIEDAD_ID);
            ViewBag.CONCEPTO_ID = new SelectList(db.TX_CONCEPTO, "ID", "DESCRIPCION", tx.CONCEPTO_ID);
            ViewBag.TNOTA_ID = new SelectList(db.TX_TNOTA, "ID", "DESCRIPCION", tx.TNOTA_ID);
            ViewBag.kun = kun;
            ViewBag.vko = vko;
            ViewBag.vtw = vtw;
            ViewBag.spa = spa;
            return View(tx);
        }

        // GET: Taxeoh/Edit/5
        public ActionResult Edit(string kun, string vk, string con)
        {
            int pagina_id = 853; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 851);

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
            
            int ci = Convert.ToInt32(con);
            TAXEOH tAXEOH = db.TAXEOHs.Where(x => x.KUNNR == kun && x.VKORG == vk && x.CONCEPTO_ID == ci).FirstOrDefault();
            if (tAXEOH == null)
            {
                return HttpNotFound();
            }
            ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1", tAXEOH.VKORG);
            ViewBag.MWSKZ = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ", tAXEOH.IMPUESTO_ID);
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", tAXEOH.PAIS_ID);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", tAXEOH.SOCIEDAD_ID);
            ViewBag.CONCEPTO_ID = new SelectList(db.TX_CONCEPTO, "ID", "DESCRIPCION", tAXEOH.CONCEPTO_ID);
            ViewBag.TNOTA_ID = new SelectList(db.TX_TNOTA, "ID", "DESCRIPCION", tAXEOH.TNOTA_ID);
            CLIENTE cli = db.CLIENTEs.Find(vk, tAXEOH.VTWEG, tAXEOH.SPART, kun);
            ViewBag.Cliente = cli.NAME1;
            ViewBag.NoCliente = cli.KUNNR;
            return View(tAXEOH);
        }

        // POST: Taxeoh/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SOCIEDAD_ID,PAIS_ID,VKORG,VTWEG,SPART,KUNNR,CONCEPTO_ID,TNOTA_ID,FECHAC,USUARIOC_ID,FECHAM,USUARIOM_ID,IMPUESTO_ID,PORC,PAY_T,ACTIVO")] TAXEOH tx, string MWSKZ)
        {
            if (ModelState.IsValid)
            {
                var por = db.IIMPUESTOes.Where(ip => ip.MWSKZ == MWSKZ).FirstOrDefault();
                var tax = db.TAXEOHs.Where(x => x.KUNNR == tx.KUNNR && x.VKORG == tx.VKORG && x.CONCEPTO_ID == tx.CONCEPTO_ID).FirstOrDefault();
                tax.IMPUESTO_ID = MWSKZ;
                tax.PORC = por.KBETR;
                tax.PAY_T = tx.PAY_T;
                db.Entry(tax).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int pagina_id = 853; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 851);
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
            
            ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1", tx.VKORG);
            ViewBag.IMPUESTO_ID = new SelectList(db.IMPUESTOes, "MWSKZ", "MWSKZ", tx.IMPUESTO_ID);
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", tx.PAIS_ID);
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", tx.SOCIEDAD_ID);
            ViewBag.CONCEPTO_ID = new SelectList(db.TX_CONCEPTO, "ID", "DESCRIPCION", tx.CONCEPTO_ID);
            ViewBag.TNOTA_ID = new SelectList(db.TX_TNOTA, "ID", "DESCRIPCION", tx.TNOTA_ID);
            return View(tx);
        }

        // GET: Taxeoh/Delete/5
        public ActionResult Delete(string kun, string vk, string con)
        {
            int pagina_id = 855; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 851);

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
            
            int ci = Convert.ToInt32(con);
            TAXEOH tAXEOH = db.TAXEOHs.Where(x => x.KUNNR == kun && x.VKORG == vk && x.CONCEPTO_ID == ci).FirstOrDefault();
            if (tAXEOH == null)
            {
                return HttpNotFound();
            }
            return View(tAXEOH);
        }

        // POST: Taxeoh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(TAXEOH tx)
        {
            try
            {
                TAXEOH tAXEOH = db.TAXEOHs.Where(x => x.KUNNR == tx.KUNNR && x.VKORG == tx.VKORG && x.CONCEPTO_ID == tx.CONCEPTO_ID && x.VTWEG == tx.VTWEG).FirstOrDefault();
                tAXEOH.ACTIVO = false;
                db.Entry(tAXEOH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { vko = tAXEOH.VKORG, vtw = tAXEOH.VTWEG, kun = tAXEOH.KUNNR, spa = tAXEOH.SPART });
            }
            catch (Exception e)
            {
                int pagina_id = 855; //ID EN BASE DE DATOS
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller, 851);

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
                
                return View(tx);
            }
        }

        [HttpPost]
        public FileResult Descargar(string vk, string kun, string vt, string sp)
        {
            var tAXEOHs = db.TAXEOHs.Include(t => t.CLIENTE).Include(t => t.IMPUESTO).Include(t => t.PAI).Include(t => t.SOCIEDAD).Include(t => t.TX_CONCEPTO).Include(t => t.TX_TNOTA);
            var pr = tAXEOHs.Where(x => x.VKORG == vk && x.VTWEG == vt && x.KUNNR == kun && x.SPART == sp).ToList();
            generarExcelHome(pr, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/DocTxh" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DocTxh" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<TAXEOH> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
             {
                  new {
                      BANNER = "ID SOCIEDAD"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
            {
                  new {
                      BANNER = "VKORG"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
            {
                  new {
                      BANNER = "VTWEG"
                      },
                    };
                worksheet.Cell("D1").Value = new[]
           {
                  new {
                      BANNER = "SPART"
                      },
                    };
                worksheet.Cell("E1").Value = new[]
            {
                  new {
                      BANNER = "KUNNR"
                      },
                    };
                worksheet.Cell("F1").Value = new[]
            {
                  new {
                      BANNER = "ID CONCEPTO"
                      },
                    };
                worksheet.Cell("G1").Value = new[]
            {
                  new {
                      BANNER = "ID IMPUESTO"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
               {
                  new {
                      BANNER       = lst[i-2].SOCIEDAD_ID
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].VKORG
                      },
                    };
                    worksheet.Cell("C" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].VTWEG
                        },
                      };
                    worksheet.Cell("D" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].SPART
                        },
                      };
                    worksheet.Cell("E" + i).Value = new[]
             {
                  new {
                      BANNER       = lst[i-2].KUNNR
                      },
                    };
                    worksheet.Cell("F" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].CONCEPTO_ID
                        },
                      };
                    worksheet.Cell("G" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].IMPUESTO_ID
                        },
                      };
                }
                var rt = ruta + @"\DocTxh" + DateTime.Now.ToShortDateString() + ".xlsx";
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

        public JsonResult Concepto(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";
            string p = Session["spras"].ToString();
            TruequeEntities db = new TruequeEntities();

            var c = (from x in db.TX_CONCEPTOT
                     join a in db.TX_CONCEPTO on x.CONCEPTO_ID equals a.ID
                     where x.TXT50.Contains(Prefix) & x.SPRAS_ID.Equals(p) & a.ACTIVO == true
                     group x by new { x.CONCEPTO_ID, x.TXT50 } into g
                     select new { ID = g.Key.CONCEPTO_ID, TEXTO = g.Key.TXT50 }).ToList();

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);

            return cc;
        }

        public JsonResult Tnota(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";
            string p = Session["spras"].ToString();
            TruequeEntities db = new TruequeEntities();

            var c = (from x in db.TX_NOTAT
                     join a in db.TX_TNOTA on x.TNOTA_ID equals a.ID
                     where x.TXT50.Contains(Prefix) & x.SPRAS_ID.Equals(p) & a.ACTIVO == true
                     group x by new { x.TNOTA_ID, x.TXT50 } into g
                     select new { ID = g.Key.TNOTA_ID, TEXTO = g.Key.TXT50 }).ToList();

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);

            return cc;
        }

        public JsonResult Impuesto(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";

            TruequeEntities db = new TruequeEntities();

            var c = (from x in db.IMPUESTOes
                     where x.MWSKZ.Contains(Prefix) && x.ACTIVO == true
                     select new { x.MWSKZ }).ToList();

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);

            return cc;
        }
    }
}
