using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
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
    public class FlujosController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();
        #region ocultar
        // GET: Flujos
        public ActionResult Index()
        {
            int pagina = 103; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0))  &&  a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0))  &&  a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }

            List<DET_AGENTE> dda = db.DET_AGENTE.ToList();
            foreach (DET_AGENTE da in dda)
            {
                string id = da.GAUTORIZACION.USUARIOs.Where(a => a.PUESTO_ID.Equals(da.PUESTOA_ID)).FirstOrDefault().ID;
                da.USUARIOA = id;
                da.USUARIOC = da.GAUTORIZACION.USUARIOs.Where(a => a.PUESTO_ID.Equals(da.PUESTOC_ID)).FirstOrDefault().ID;
                db.Entry(da).State = EntityState.Modified;
                db.SaveChanges();
            }

            //List<USUARIO> uu = db.USUARIOs.ToList();
            //foreach(USUARIO u in uu)
            //{
            //    foreach(GAUTORIZACION g in u.GAUTORIZACIONs)
            //    {
            //        int i = 0;
            //    }
            //}

            var fLUJOes = db.FLUJOes.Include(f => f.DOCUMENTO).Include(f => f.USUARIO).Include(f => f.USUARIO1).Include(f => f.WORKFP);
            return View(fLUJOes.ToList());
        }

        // GET: Flujos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FLUJO fLUJO = db.FLUJOes.Find(id);
            if (fLUJO == null)
            {
                return HttpNotFound();
            }
            return View(fLUJO);
        }

        // GET: Flujos/Create
        public ActionResult Create(decimal id)
        {
            int pagina = 103; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0))  &&  a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0))  &&  a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;

                DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
                //db.DOCUMENTOes.Add(dOCUMENTO);
                //db.SaveChanges();

                int rol = user.MIEMBROS.FirstOrDefault().ROL_ID;
                //WORKFV wf = db.WORKFHs.Where(a => a.BUKRS.Equals(dOCUMENTO.SOCIEDAD_ID) && a.ROL_ID == rol).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();

                WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                FLUJO f = new FLUJO();
                f.WORKF_ID = wf.ID;
                f.WF_VERSION = wf.VERSION;
                f.WF_POS = wp.POS;
                f.NUM_DOC = dOCUMENTO.NUM_DOC;
                f.POS = 1;
                f.LOOP = 1;
                f.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                f.ESTATUS = "I";
                f.FECHAC = DateTime.Now;
                f.FECHAM = DateTime.Now;
                ProcesaFlujo pf = new ProcesaFlujo();
                string c = pf.procesa(f, "");

                //WORKFP next = wf.WORKFPs.Where(a => a.POS.Equals(wp.NEXT_STEP)).FirstOrDefault();
                //FLUJO fn = new FLUJO();
                //fn.WORKF_ID = wf.ID;
                //fn.WF_VERSION = wf.VERSION;
                //fn.WF_POS = next.POS;
                //fn.NUM_DOC = dOCUMENTO.NUM_DOC;
                //fn.POS = 2;
                //fn.LOOP = 1;
                //fn.ESTATUS = "P";
                //fn.FECHAC = DateTime.Now;
                //fn.FECHAM = DateTime.Now;
                //fn.USUARIOA_ID = db.USUARIOs.Where(a => a.ID.Equals(dOCUMENTO.USUARIOC_ID)).FirstOrDefault().MANAGER;

                //db.FLUJOes.Add(f);
                //db.FLUJOes.Add(fn);

                db.SaveChanges();
                if (wp.EMAIL.Equals("X"))
                    return RedirectToAction("Index", "Correos", new { id = id });

                return RedirectToAction("Details", "Solicitudes", new { id = id });
            }
        }

        // POST: Flujos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WORKF_ID,WF_VERSION,WF_POS,NUM_DOC,POS,LOOP,USUARIOA_ID,USUARIOD_ID,ESTATUS,FECHAC,HORAC,FECHAM,HORAM")] FLUJO fLUJO)
        {
            if (ModelState.IsValid)
            {
                db.FLUJOes.Add(fLUJO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NUM_DOC = new SelectList(db.DOCUMENTOes, "NUM_DOC", "TSOL_ID", fLUJO.NUM_DOC);
            ViewBag.USUARIOA_ID = new SelectList(db.USUARIOs, "ID", "PASS", fLUJO.USUARIOA_ID);
            ViewBag.USUARIOD_ID = new SelectList(db.USUARIOs, "ID", "PASS", fLUJO.USUARIOD_ID);
            ViewBag.WORKF_ID = new SelectList(db.WORKFPs, "ID", "EMAIL", fLUJO.WORKF_ID);
            return View(fLUJO);
        }

        // GET: Flujos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FLUJO fLUJO = db.FLUJOes.Find(id);
            if (fLUJO == null)
            {
                return HttpNotFound();
            }
            ViewBag.NUM_DOC = new SelectList(db.DOCUMENTOes, "NUM_DOC", "TSOL_ID", fLUJO.NUM_DOC);
            ViewBag.USUARIOA_ID = new SelectList(db.USUARIOs, "ID", "PASS", fLUJO.USUARIOA_ID);
            ViewBag.USUARIOD_ID = new SelectList(db.USUARIOs, "ID", "PASS", fLUJO.USUARIOD_ID);
            ViewBag.WORKF_ID = new SelectList(db.WORKFPs, "ID", "EMAIL", fLUJO.WORKF_ID);
            return View(fLUJO);
        }

        // POST: Flujos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WORKF_ID,WF_VERSION,WF_POS,NUM_DOC,POS,LOOP,USUARIOA_ID,USUARIOD_ID,ESTATUS,FECHAC,HORAC,FECHAM,HORAM")] FLUJO fLUJO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fLUJO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NUM_DOC = new SelectList(db.DOCUMENTOes, "NUM_DOC", "TSOL_ID", fLUJO.NUM_DOC);
            ViewBag.USUARIOA_ID = new SelectList(db.USUARIOs, "ID", "PASS", fLUJO.USUARIOA_ID);
            ViewBag.USUARIOD_ID = new SelectList(db.USUARIOs, "ID", "PASS", fLUJO.USUARIOD_ID);
            ViewBag.WORKF_ID = new SelectList(db.WORKFPs, "ID", "EMAIL", fLUJO.WORKF_ID);
            return View(fLUJO);
        }

        // GET: Flujos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FLUJO fLUJO = db.FLUJOes.Find(id);
            if (fLUJO == null)
            {
                return HttpNotFound();
            }
            return View(fLUJO);
        }

        // POST: Flujos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            FLUJO fLUJO = db.FLUJOes.Find(id);
            db.FLUJOes.Remove(fLUJO);
            db.SaveChanges();
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
#endregion ocultar
        [HttpGet]
        public ActionResult Procesa(decimal id, string accion)
        {
            int pagina = 103; //ID EN BASE DE DATOS
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    ////return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            
            FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();
            f.ESTATUS = accion;
            return View(f);
        }
        [HttpPost]
        public ActionResult Procesa(FLUJO f)
        {
            FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

            DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);
            List<TS_FORM> tts = db.TS_FORM.Where(a => a.BUKRS_ID.Equals(d.SOCIEDAD_ID)  &&  a.LAND_ID.Equals(d.PAIS_ID)  &&  a.TS_CAMPO.ACTIVO).ToList();
            
            if (actual.WORKFP.ACCION.TIPO == "R")
            {
                List<DOCUMENTOT> ddt = new List<DOCUMENTOT>();
                foreach (TS_FORM ts in tts)
                {
                    DOCUMENTOT dts = new DOCUMENTOT();
                    dts.NUM_DOC = f.NUM_DOC;
                    dts.TSFORM_ID = ts.POS;
                    try
                    {
                        string temp = Request.Form["chk-" + ts.POS].ToString();
                        if (temp == "on")
                            dts.CHECKS = true;
                    }
                    catch
                    {
                        dts.CHECKS = false;
                    }
                    int tt = db.DOCUMENTOTS.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)  &&  a.TSFORM_ID == ts.POS).Count();
                    if (tt == 0)
                        ddt.Add(dts);
                    else
                        db.Entry(dts).State = EntityState.Modified;
                }
                if (ddt.Count > 0)
                    db.DOCUMENTOTS.AddRange(ddt);
                db.SaveChanges();
                
            }

            FLUJO flujo = actual;
            flujo.ESTATUS = f.ESTATUS;
            flujo.FECHAM = DateTime.Now;
            flujo.COMENTARIO = f.COMENTARIO;
            flujo.USUARIOA_ID = User.Identity.Name;
            ProcesaFlujo pf = new ProcesaFlujo();
            if (ModelState.IsValid)
            {
                string res = pf.procesa(flujo, "");
                
                using (TruequeEntities db1 = new TruequeEntities())
                {
                    FLUJO ff = db1.FLUJOes.Where(x => x.NUM_DOC == flujo.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    Estatus es = new Estatus();//RSG 18.09.2018
                    DOCUMENTO ddoc = db1.DOCUMENTOes.Find(f.NUM_DOC);
                    ff.STATUS = es.getEstatus(ddoc);
                    db1.Entry(ff).State = EntityState.Modified;
                    db1.SaveChanges();
                }


                if (res.Equals("0"))//Aprobado
                {
                    ////return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                    return RedirectToAction("Index", "Solicitudes");
                }
                else if (res.Equals("1") || res.Equals("2") || res.Equals("3"))//CORREO
                {
                    Email em = new Email();
                    string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                    string image = Server.MapPath("~/images/logo_kellogg.png");
                    DOCUMENTO doc = db.DOCUMENTOes.Where(x => x.NUM_DOC == flujo.NUM_DOC).First();
                    string imageFlag = Server.MapPath("~/images/flags/mini/" + doc.PAIS_ID + ".png");
                    string spras_id = FnCommon.ObtenerSprasId(db,User.Identity.Name);
                    if (res.Equals("1") || res.Equals("2"))//CORREO
                    {
                        em.enviaMailC(f.NUM_DOC, true, spras_id, UrlDirectory, "Index", image, imageFlag);
                    }
                    else
                    {
                        em.enviaMailC(f.NUM_DOC, true, spras_id, UrlDirectory, "Details", image, imageFlag);
                        return RedirectToAction("Index", "Home");
                    }
                    ////return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                    return RedirectToAction("Index", "Solicitudes");
                }
                else
                {
                    TempData["error"] = res;
                    ////return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                    return RedirectToAction("Index", "Solicitudes");
                }
            }

            int pagina = 103; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".png";
            }
            catch
            {
                ////return RedirectToAction("Pais", "Home");
            }
            Session["spras"] = user.SPRAS_ID;
            
            return View(f);
        }

        public ActionResult Carga()
        {
            int pagina = 601; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

            FnCommon.ObtenerConfPage(db, pagina, User.Identity.Name, this.ControllerContext.Controller);
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".png";
            }
            catch
            {
                ////return RedirectToAction("Pais", "Home");
            }
                Session["spras"] = user.SPRAS_ID;
            
            return View();
        }
        [HttpPost]
        public ActionResult Carga(IEnumerable<HttpPostedFileBase> files)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoadExcel()
        {
            List<DET_AGENTEC> ld = new List<DET_AGENTEC>();
            
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];

                ////string extension = System.IO.Path.GetExtension(file.FileName);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                // 2. Use the AsDataSet extension method
                DataSet result = reader.AsDataSet();

                // The result of each spreadsheet is in result.Tables
                // 3.DataSet - Create column names from first row
                DataTable dt = result.Tables[0];
                ld = objAList(dt);

                reader.Close();

            }

            List<Flujos> ff = new List<Flujos>();
            List<USUARIO> usuarios = new List<USUARIO>();
            List<CLIENTE> clientes = new List<CLIENTE>();
            List<PAI> paises = new List<PAI>();

            foreach (DET_AGENTEC da in ld)
            {
                Flujos f = new Flujos();
                ////---------------------------------------USUARIO
                f.USUARIOC_ID = da.USUARIOC_ID;
                f.USUARIOC_IDX = true;
                USUARIO u = usuarios.FirstOrDefault(x => x.ID.Equals(f.USUARIOC_ID));
                if (u == null)
                {
                    u = db.USUARIOs.Where(x => x.ID.Equals(f.USUARIOC_ID)  &&  x.ACTIVO == true).FirstOrDefault();
                    if (u == null)
                        f.USUARIOC_IDX = false;
                    else
                        usuarios.Add(u);
                }
                if (!f.USUARIOC_IDX)
                    f.USUARIOC_ID = "<span class='red white-text'>" + f.USUARIOC_ID + "</span>";
                ////---------------------------------------PAIS
                f.PAIS_ID = da.PAIS_ID;
                f.PAIS_IDX = true;

                PAI p = paises.FirstOrDefault(x => x.LAND.Equals(f.PAIS_ID));
                if (p == null)
                {
                    p = db.PAIS.Where(x => x.LAND.Equals(f.PAIS_ID) && x.ACTIVO  && x.SOCIEDAD_ID != null).FirstOrDefault();
                    if (p == null)
                        f.PAIS_IDX = false;
                    else
                        paises.Add(p);
                }
                if (!f.PAIS_IDX)
                    f.PAIS_ID = "<span class='red white-text'>" + f.PAIS_ID + "</span>";

                ////---------------------------------------CLIENTE
                f.KUNNR = da.KUNNR;
                f.KUNNRX = true;

                CLIENTE c = clientes.FirstOrDefault(x => x.KUNNR.Equals(f.KUNNR));
                if (c == null)
                {
                    c = db.CLIENTEs.Where(cc => cc.KUNNR.Equals(f.KUNNR) && cc.ACTIVO).FirstOrDefault();
                    if (c == null)
                        f.KUNNRX = false;
                    else
                        clientes.Add(c);
                }
                if (!f.KUNNRX)
                    f.KUNNR = "<span class='red white-text'>" + f.KUNNR + "</span>";


                f.VERSION = da.VERSION.ToString();
                f.POS = da.POS.ToString();
                ////---------------------------------------USUARIOA
                f.USUARIOA_ID = da.USUARIOA_ID;
                f.USUARIOA_IDX = true;
                USUARIO ua = usuarios.FirstOrDefault(x => x.ID.Equals(f.USUARIOA_ID));
                if (ua == null)
                {
                    ua = db.USUARIOs.Where(x => x.ID.Equals(f.USUARIOA_ID) && x.ACTIVO == true).FirstOrDefault();
                    if (ua == null)
                        f.USUARIOA_IDX = false;
                    else
                        usuarios.Add(ua);
                }
                if (!f.USUARIOA_IDX)
                    f.USUARIOA_ID = "<span class='red white-text'>" + f.USUARIOA_ID + "</span>";

                if (da.MONTO == null)
                    f.MONTO = null;
                else
                    f.MONTO = da.MONTO.ToString();
                f.PRESUPUESTO = da.PRESUPUESTO.ToString();
                ff.Add(f);
            }
            JsonResult jl = Json(ff, JsonRequestBehavior.AllowGet);
            return jl;
        }

        private string completa(string s, int longitud)
        {
            StringBuilder cadena = new StringBuilder();
            try
            {
                long a = Int64.Parse(s);
                int l = a.ToString().Length;
                
                for (int i = l; i < longitud; i++)
                {
                    cadena.Insert(0,"0");
                }
                cadena.Insert(cadena.Length, a);
            }
            catch
            {
                cadena = new StringBuilder(s);
            }
            return cadena.ToString();
        }

        private List<DET_AGENTEC> objAList(DataTable dt)
        {

            List<DET_AGENTEC> ld = new List<DET_AGENTEC>();
            List<CLIENTE> clientes = new List<CLIENTE>();
            //Rows
            var rowsc = dt.Rows.Count;
            //columns
            ////var columnsc = dt.Columns.Count;

            //Columnd and row to start
            var rows = 1; // 2
                          //var cols = 0; // A
            var pos = 1;

            for (int i = rows; i < rowsc; i++)
            {
                ////for (var j = 0; j < columnsc; j++)
                ////{
                ////    var data = dt.Rows[i][j];
                ////}
                if (i >= 4)
                {
                    var v = dt.Rows[i][1];
                    if (Convert.ToString(v) == "")
                    {
                        break;
                    }
                }
                DET_AGENTEC doc = new DET_AGENTEC();

                string a = Convert.ToString(pos);

                doc.POS = Convert.ToInt32(a);
                try
                {
                    doc.USUARIOC_ID = dt.Rows[i][0].ToString(); //Usuario creador

                }
                catch
                {
                    doc.USUARIOC_ID = null;
                }
                try
                {
                    doc.PAIS_ID = dt.Rows[i][1].ToString(); //País
                }
                catch
                {
                    doc.PAIS_ID = null;
                }
                try
                {
                    doc.KUNNR = dt.Rows[i][2].ToString();
                    doc.KUNNR = completa(doc.KUNNR, 10);

                    CLIENTE u = clientes.FirstOrDefault(x => x.KUNNR.Equals(doc.KUNNR));
                    if (u == null)
                    {
                        u = db.CLIENTEs.Where(cc => cc.KUNNR.Equals(doc.KUNNR) && cc.ACTIVO).FirstOrDefault();
                        if (u == null)
                            doc.VKORG = null;
                        else
                            clientes.Add(u);
                    }

                    CLIENTE c = clientes.FirstOrDefault(cc => cc.KUNNR.Equals(doc.KUNNR) && cc.ACTIVO);
                    if (c != null)
                    {
                        doc.VKORG = c.VKORG;
                        doc.VTWEG = c.VTWEG;
                        doc.SPART = c.SPART;
                    }
                    else
                    {
                        doc.VKORG = null;
                    }
                }
                catch
                {
                    doc.KUNNR = null;
                }
                try
                {
                    doc.POS = int.Parse(dt.Rows[i][3].ToString());
                }
                catch
                {
                    doc.POS = 0;
                }
                try
                {
                    doc.USUARIOA_ID = dt.Rows[i][4].ToString();
                }
                catch
                {
                    doc.USUARIOA_ID = null;
                }

                try
                {
                    doc.MONTO = decimal.Parse(dt.Rows[i][5].ToString());
                }
                catch
                {
                    doc.MONTO = null;
                }
                try
                {
                    string p = dt.Rows[i][6].ToString();
                    if (p == "X" || p == "x")
                        doc.PRESUPUESTO = true;
                }
                catch
                {
                    doc.PRESUPUESTO = false;
                }

                ////DET_AGENTEC poss = ld.Where(x => x.USUARIOC_ID.Equals(doc.USUARIOC_ID) && x.PAIS_ID.Equals(doc.PAIS_ID)
                ////& x.KUNNR.Equals(doc.KUNNR)).FirstOrDefault();
                ////if (poss == null)
                ////    pos = 1;
                ////else
                ////    pos = ld.Where(x => x.USUARIOC_ID.Equals(doc.USUARIOC_ID) && x.PAIS_ID.Equals(doc.PAIS_ID) && x.KUNNR.Equals(doc.KUNNR)).Count() + 1;

                ld.Add(doc);
                pos++;
            }
            return ld;
        }


        [HttpGet]
        public ActionResult Nuevo(decimal id)
        {
            try
            {
                ProcesaFlujo pf = new ProcesaFlujo();
                DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
                ////WORKFV wf = db.WORKFHs.Where(a => a.BUKRS.Equals(dOCUMENTO.SOCIEDAD_ID) && a.ROL_ID == rol).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                WORKFV wf = db.WORKFHs.Where(a => a.TSOL_ID.Equals(dOCUMENTO.TSOL_ID)).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();
                if (wf != null)
                {
                    WORKFP wp = wf.WORKFPs.OrderBy(a => a.POS).FirstOrDefault();
                    FLUJO f = new FLUJO();
                    f.WORKF_ID = wf.ID;
                    f.WF_VERSION = wf.VERSION;
                    f.WF_POS = wp.POS;
                    f.NUM_DOC = dOCUMENTO.NUM_DOC;
                    f.POS = 1;
                    f.LOOP = 1;
                    f.USUARIOA_ID = dOCUMENTO.USUARIOC_ID;
                    f.USUARIOD_ID = dOCUMENTO.USUARIOD_ID;
                    f.ESTATUS = "I";
                    f.FECHAC = DateTime.Now;
                    f.FECHAM = DateTime.Now;
                    f.COMENTARIO = "";//ADD RSG 20.08.2018
                    string c = pf.procesa(f, "");
                    FLUJO conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();
                    while (c == "1")
                    {
                        Email em = new Email();
                        string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                        string image = Server.MapPath("~/images/logo_kellogg.png");
                        DOCUMENTO docc = db.DOCUMENTOes.Where(x => x.NUM_DOC == dOCUMENTO.NUM_DOC).First();
                        string imageFlag = Server.MapPath("~/images/flags/mini/" + docc.PAIS_ID + ".png");
                        em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image, imageFlag);

                        if (conta.WORKFP.ACCION.TIPO == "B")
                        {
                            ////WORKFP wpos = db.WORKFPs.Where(x => x.ID == conta.WORKF_ID && x.VERSION == conta.WF_VERSION && x.POS == conta.WF_POS).FirstOrDefault();
                            conta.ESTATUS = "A";
                            conta.FECHAM = DateTime.Now;
                            c = pf.procesa(conta, "");
                            conta = db.FLUJOes.Where(x => x.NUM_DOC == f.NUM_DOC).Include(x => x.WORKFP).OrderByDescending(x => x.POS).FirstOrDefault();

                        }
                        else
                        {
                            c = "";
                        }
                    }
                    Estatus es = new Estatus();//RSG 18.09.2018
                    DOCUMENTO doc = db.DOCUMENTOes.Find(f.NUM_DOC);
                    conta.STATUS = es.getEstatus(doc);
                    db.Entry(conta).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch 
            {
                return RedirectToAction("Index", "Solicitudes");
            }

            return RedirectToAction("Index", "Solicitudes");
        }
    }
}
