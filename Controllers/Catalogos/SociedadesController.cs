using Newtonsoft.Json;
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
    public class SociedadesController : Controller
    {
        private TruequeEntities db = new TruequeEntities();

        // GET: Sociedades
        public ActionResult Index()
        {
            ObtenerConfPage(920);//ID EN BASE DE DATOS
            var sOCIEDADs = db.SOCIEDADs;
            return View(sOCIEDADs.ToList());
        }

        // GET: Sociedades/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(923);
            SOCIEDAD sOCIEDAD = db.SOCIEDADs.Find(id);
            if (sOCIEDAD == null)
            {
                return HttpNotFound();
            }
            return View(sOCIEDAD);
        }

        // GET: Sociedades/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(922);
            SOCIEDAD sOCIEDAD = db.SOCIEDADs.Find(id);
            if (sOCIEDAD == null)
            {
                return HttpNotFound();
            }
            //if (sOCIEDAD.REGION != null)
              //  sOCIEDAD.REGION= sOCIEDAD.REGION.TrimEnd();
            //ViewBag.REGION = new SelectList(db.REGIONs.Where(x=>x.SOCIEDAD==id).ToList(), "REGION1", "REGION1", sOCIEDAD.REGION!=null?sOCIEDAD.REGION.TrimEnd():"");

            return View(sOCIEDAD);
        }

        // POST: Sociedades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BUKRS,BUTXT,ORT01,LAND,SUBREGIO,WAERS,SPRAS,NAME1,KTOPL,ACTIVO,REGION")] SOCIEDAD sOCIEDAD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sOCIEDAD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ObtenerConfPage(922);
            //if (sOCIEDAD.REGION != null)
            //    sOCIEDAD.REGION = sOCIEDAD.REGION.TrimEnd();
            //ViewBag.REGION = new SelectList(db.REGIONs.Where(x => x.SOCIEDAD == sOCIEDAD.BUKRS).ToList(), "REGION1", "REGION1", sOCIEDAD.REGION != null ? sOCIEDAD.REGION.TrimEnd() : "");
            return View(sOCIEDAD);
        }

        public ActionResult Flujos(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(923,924);
            List<DET_APROBH> flujos = db.DET_APROBH.Where(t=>t.SOCIEDAD_ID==id).OrderByDescending(t=>t.VERSION).DistinctBy(t=>t.PUESTOC_ID).OrderBy(t=>t.PUESTOC_ID).ToList();
            ViewBag.coCode = id;
            return View(flujos);
        }
        public ActionResult CreateF(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(923,925);
            var flujos = db.DET_APROBH.Where(t => t.SOCIEDAD_ID == id).Select(t=>t.PUESTOC_ID).Distinct().ToList();
            var lan =ViewBag.usuario.SPRAS_ID;
            var puestos = db.PUESTOes.Where(t => t.ACTIVO == true && t.ID!=1 && t.ID!=9 && !flujos.Contains(t.ID)).ToList();
            var sl_puestos = puestos.Select(x => new { x.ID,Puesto= x.PUESTOTs.Count>0? x.PUESTOTs.Where(t=>t.SPRAS_ID==lan).FirstOrDefault().TXT50 : ""}).ToList();
            ViewBag.PUESTOC_ID = new SelectList(sl_puestos, "ID", "Puesto");
            ViewBag.NivelesA = new SelectList(FnCommon.ObtenerCmbNivelesA(), "Value", "Text");
            ViewBag.NivelesP = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            ViewBag.NivelesM = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            DET_APROBH dET_APROBP = new DET_APROBH { SOCIEDAD_ID = id, ACTIVO=true, VERSION=1 };
            var sociedad = db.SOCIEDADs.Find(id);
            ViewBag.Miles = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().MILES;
            ViewBag.PD = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().DECIMAL;
            return View(dET_APROBP);
        }
        [HttpPost]
        public ActionResult CreateF([Bind(Include = "SOCIEDAD_ID,PUESTOC_ID,VERSION,ACTIVO")] DET_APROBH dET_APROBH, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                List<DET_APROBP> dET_APROBPs = new List<DET_APROBP>();
                var posiciones = collection.GetValues("posicion");
                var npresupuestos = collection.GetValues("NivelesP");
                var montos = collection.GetValues("monto");
                var nmontos = collection.GetValues("NivelesM");
                var naprobadores = collection.GetValues("NivelesA");
                if (posiciones.Length > 1)
                {
                    
                    for (int i=1;i<posiciones.Length;i++)
                    {
                        DET_APROBP dET_APROB = new DET_APROBP();
                        dET_APROB.POS = Convert.ToInt32(posiciones[i]);
                        if ((i + 1) < posiciones.Length)
                        {
                            if (collection.AllKeys.Contains("p_" + i))
                            {
                                dET_APROB.PRESUPUESTO = true;
                                dET_APROB.N_PRESUP = Convert.ToInt16(npresupuestos[i]);
                            }
                            if (montos[i] != "")
                            {
                                dET_APROB.MONTO = Convert.ToDecimal(montos[i]);
                                dET_APROB.N_MONTO = Convert.ToInt16(nmontos[i]);
                            }
                        }
                        dET_APROB.PUESTOA_ID =Convert.ToInt16(naprobadores[i]);
                        dET_APROB.ACTIVO = true;
                        dET_APROB.PUESTOC_ID = dET_APROBH.PUESTOC_ID;
                        dET_APROB.SOCIEDAD_ID = dET_APROBH.SOCIEDAD_ID;
                        dET_APROB.VERSION = dET_APROBH.VERSION;
                        dET_APROBPs.Add(dET_APROB);
                    }
                    dET_APROBH.DET_APROBP = dET_APROBPs;
                    db.Entry(dET_APROBH).State = EntityState.Added;
                    db.SaveChanges();
                    return RedirectToAction("Flujos", new { id = dET_APROBH.SOCIEDAD_ID });
                }
                else
                    ModelState.AddModelError("Mensaje","Incluya al menos una fila a la matriz de aprobación");
            }
            else
            {

            }
            ObtenerConfPage(923,925);
            var flujos = db.DET_APROBH.Where(t => t.SOCIEDAD_ID == dET_APROBH.SOCIEDAD_ID && t.ACTIVO).Select(t => t.PUESTOC_ID).Distinct().ToList();
            var lan = ViewBag.usuario.SPRAS_ID;
            var puestos = db.PUESTOes.Where(t => t.ACTIVO == true && t.ID != 1 && t.ID != 9 && !flujos.Contains(t.ID)).ToList();
            var sl_puestos = puestos.Select(x => new { x.ID, Puesto = x.PUESTOTs.Count > 0 ? x.PUESTOTs.Where(t => t.SPRAS_ID == lan).FirstOrDefault().TXT50 : "" }).ToList();
            ViewBag.PUESTOC_ID = new SelectList(sl_puestos, "ID", "Puesto");
            ViewBag.NivelesA = new SelectList(FnCommon.ObtenerCmbNivelesA(), "Value", "Text");
            ViewBag.NivelesP = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            ViewBag.NivelesM = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            var sociedad = db.SOCIEDADs.Find(dET_APROBH.SOCIEDAD_ID);
            ViewBag.Miles = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().MILES;
            ViewBag.PD = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().DECIMAL;
            return View(dET_APROBH);
        }
        public ActionResult EditF(string id, int pid, int v)
        {
            if (id == null || pid==0||v==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(923,927);
            DET_APROBH flujo = db.DET_APROBH.Where(t => t.SOCIEDAD_ID == id && t.PUESTOC_ID==pid && t.VERSION==v).SingleOrDefault();

            return View(flujo);
        }
        [HttpPost]
        public ActionResult EditF([Bind(Include = "SOCIEDAD_ID, VERSION, PUESTOC_ID, ACTIVO")]DET_APROBH modelo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modelo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Flujos", new {id=modelo.SOCIEDAD_ID });
            }
            ObtenerConfPage(923,927);
            return View(modelo);
        }
        public ActionResult MAFlujos(string id, int pid, int v)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(923,926);
            List<DET_APROBP> flujos = db.DET_APROBP.Where(t => t.SOCIEDAD_ID == id && t.PUESTOC_ID==pid && t.VERSION==v).ToList();
            ViewBag.Cocode = id;
            var lan = ViewBag.usuario.SPRAS_ID;
            var puesto = db.PUESTOes.Where(t => t.ID == pid).SingleOrDefault();
            var sl_puestos = puesto.PUESTOTs.Count > 0 ? puesto.PUESTOTs.Where(t => t.SPRAS_ID == lan).FirstOrDefault().TXT50 : "";
            return View(flujos);
        }
        public ActionResult CreateFA(string id, int pid, int v)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObtenerConfPage(923, 925);
            List<DET_APROBP> flujosva = db.DET_APROBP.Where(t => t.SOCIEDAD_ID == id && t.PUESTOC_ID == pid && t.VERSION == v).ToList();
            var lan = ViewBag.usuario.SPRAS_ID;
            var puestos = db.PUESTOes.Where(t => t.ID == pid).ToList();
            var sl_puestos = puestos.Select(x => new { x.ID, Puesto = x.PUESTOTs.Count > 0 ? x.PUESTOTs.Where(t => t.SPRAS_ID == lan).FirstOrDefault().TXT50 : "" }).ToList();
            ViewBag.PUESTOC_ID = new SelectList(sl_puestos, "ID", "Puesto");
            ViewBag.NivelesA = new SelectList(FnCommon.ObtenerCmbNivelesA(), "Value", "Text");
            ViewBag.NivelesP = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            ViewBag.NivelesM = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            DET_APROBH dET_APROBP = new DET_APROBH { SOCIEDAD_ID = id, ACTIVO = true, VERSION = v + 1, PUESTOC_ID=pid };
            var sociedad = db.SOCIEDADs.Find(id);
            ViewBag.Miles = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().MILES;
            ViewBag.PD = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().DECIMAL;
            ViewBag.FlujosActuales = flujosva;
            return View(dET_APROBP);
        }
        [HttpPost]
        public ActionResult CreateFA([Bind(Include = "SOCIEDAD_ID,PUESTOC_ID,VERSION,ACTIVO")] DET_APROBH dET_APROBH, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                List<DET_APROBP> dET_APROBPs = new List<DET_APROBP>();
                var posiciones = collection.GetValues("posicion");
                var npresupuestos = collection.GetValues("NivelesPR");
                var montos = collection.GetValues("monto");
                var nmontos = collection.GetValues("NivelesMO");
                var naprobadores = collection.GetValues("NivelesAP");
                if (posiciones.Length > 1)
                {
                    
                    for (int i = 1; i < posiciones.Length; i++)
                    {
                        DET_APROBP dET_APROB = new DET_APROBP();
                        dET_APROB.POS = Convert.ToInt32(posiciones[i]);
                        if ((i + 1) < posiciones.Length)
                        {
                            if (collection.AllKeys.Contains("p_" + i))
                            {
                                dET_APROB.PRESUPUESTO = true;
                                dET_APROB.N_PRESUP = Convert.ToInt16(npresupuestos[i]);
                            }
                            if (montos[i] != "")
                            {
                                dET_APROB.MONTO = Convert.ToDecimal(montos[i]);
                                dET_APROB.N_MONTO = Convert.ToInt16(nmontos[i]);
                            }
                        }
                        dET_APROB.PUESTOA_ID = Convert.ToInt16(naprobadores[i]);
                        dET_APROB.ACTIVO = true;
                        dET_APROB.PUESTOC_ID = dET_APROBH.PUESTOC_ID;
                        dET_APROB.SOCIEDAD_ID = dET_APROBH.SOCIEDAD_ID;
                        dET_APROB.VERSION = dET_APROBH.VERSION;
                        dET_APROBPs.Add(dET_APROB);
                    }
                    dET_APROBH.DET_APROBP = dET_APROBPs;
                    dET_APROBH.ACTIVO = true;
                    db.Entry(dET_APROBH).State = EntityState.Added;
                    db.SaveChanges();
                    var va = db.DET_APROBH.Where(t => t.SOCIEDAD_ID == dET_APROBH.SOCIEDAD_ID && t.PUESTOC_ID == dET_APROBH.PUESTOC_ID && t.VERSION == (dET_APROBH.VERSION - 1)).SingleOrDefault();
                    va.ACTIVO = false;
                    foreach(var e in va.DET_APROBP)
                    {
                        e.ACTIVO=false;
                    }
                    db.SaveChanges();
                    return RedirectToAction("MAFlujos", new { id = dET_APROBH.SOCIEDAD_ID, pid=dET_APROBH.PUESTOC_ID, v=dET_APROBH.VERSION});
                }
                else
                    ModelState.AddModelError("Mensaje", "Incluya al menos una fila a la matriz de aprobación");
            }
            else
            {

            }
            ObtenerConfPage(923, 925);
            var flujos = db.DET_APROBH.Where(t => t.SOCIEDAD_ID == dET_APROBH.SOCIEDAD_ID && t.ACTIVO).Select(t => t.PUESTOC_ID).Distinct().ToList();
            var lan = ViewBag.usuario.SPRAS_ID;
            var puestos = db.PUESTOes.Where(t => t.ACTIVO == true && t.ID != 1 && t.ID != 9 && !flujos.Contains(t.ID)).ToList();
            var sl_puestos = puestos.Select(x => new { x.ID, Puesto = x.PUESTOTs.Count > 0 ? x.PUESTOTs.Where(t => t.SPRAS_ID == lan).FirstOrDefault().TXT50 : "" }).ToList();
            ViewBag.PUESTOC_ID = new SelectList(sl_puestos, "ID", "Puesto");
            ViewBag.NivelesA = new SelectList(FnCommon.ObtenerCmbNivelesA(), "Value", "Text");
            ViewBag.NivelesP = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            ViewBag.NivelesM = new SelectList(FnCommon.ObtenerCmbNivel(), "Value", "Text");
            var sociedad = db.SOCIEDADs.Find(dET_APROBH.SOCIEDAD_ID);
            ViewBag.Miles = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().MILES;
            ViewBag.PD = db.PAIS.Where(t => t.LAND == sociedad.LAND).SingleOrDefault().DECIMAL;
            return View(dET_APROBH);
        }
        void ObtenerConfPage(int pagina, int? pagtitulo=null)//ID EN BASE DE DATOS
        {
            var user = ObtenerUsuario();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.returnUrl = Request.Url.PathAndQuery; ;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            if(pagtitulo==null)
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            else
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagtitulo.Value)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.mensajes = JsonConvert.SerializeObject(db.MENSAJES.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS.Equals(user.SPRAS_ID)).ToList(), Formatting.Indented);
        }
        USUARIO ObtenerUsuario()
        {
            string u = User.Identity.Name;
            return db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
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
