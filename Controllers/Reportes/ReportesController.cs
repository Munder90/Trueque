using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using TAT001.Entities;
using TAT001.Models;
using TAT001.Services;
using static TAT001.Models.ReportesModel;
using ClosedXML.Excel;

namespace TAT001.Controllers.Reportes
{
    [Authorize]
    public class ReportesController : Controller
    {
        #region SF
        private TruequeEntities db = new TruequeEntities();
        public TAT001.Services.Calendario445 cal = new TAT001.Services.Calendario445();
        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }

        //GET: Reportes/Allowance
        public ActionResult ReporteIntegracionTs()
        {
            int pagina = 1103; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = false;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.baseURL = String.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~/"));
            //ViewBag.cuentagl = db.CUENTAGLs.ToList();
            ViewBag.cuentagl = (from c in db.CUENTAGLs join d in db.DOCUMENTOes on c.ID equals d.CUENTAP select c).Union(from c in db.CUENTAGLs join d in db.DOCUMENTOes on c.ID equals d.CUENTAPL select c).
                Union(from c in db.CUENTAGLs join d in db.DOCUMENTOes on c.ID equals d.CUENTACL select c).DistinctBy(x=>x.ID).ToList();
            ViewBag.periodo = db.PERIODOes.ToList();

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
            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);
            return View(dOCUMENTOes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteIntegracionTs(String Form)
        {
            int pagina = 1103; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            string comcode = Request["selectcocode"] as string;
            string[] comcodessplit = comcode.Split(',');
            string accnt = Request["selectaccount"] as string;
            string[] accntssplit = accnt.Split(',');
            int period = Int32.Parse(Request["selectperiod"]);
            string year = Request["selectyear"];
            decimal decAccnt;
            ViewBag.display = true;
            ViewBag.Calendario = cal;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.cuentagl = db.CUENTAGLs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.cuenta = db.CUENTAs.ToList();
            ViewBag.Consulta = "";
            ViewBag.baseURL = String.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~/"));
            List<object> queryList = new List<object>();
            List<object> provitions = new List<object>();
            decimal docrefs = 0;



            
                foreach (string account in accntssplit)
                {
                foreach (string companyCode in comcodessplit)
                {
                    decimal numDoc;
                    decimal montoDoc;

                    Decimal.TryParse(account, out decAccnt);
                    var queryP = (from cg in db.CUENTAGLs
                                  join doc in db.DOCUMENTOes on cg.ID equals doc.CUENTAP  //NUM_DOC + DOCUMENTO_SAP PAYER_ID + CLIENTE-NAME1 + TALLT-TXT050 + DOCUMENTO-CONCEPTO --DOCUMENTO-USUARIOD
                                  join docsap in db.DOCUMENTOSAPs on doc.NUM_DOC equals docsap.NUM_DOC 
                                  join ta in db.TALLTs on doc.TALL_ID equals ta.TALL_ID
                                  join cli in db.CLIENTEs on new { doc.VKORG, doc.VTWEG, doc.SPART, doc.PAYER_ID } equals new { cli.VKORG, cli.VTWEG, cli.SPART, PAYER_ID = cli.KUNNR }   // NAME1
                                  join fl in db.FLUJOes on doc.NUM_DOC equals fl.NUM_DOC  //FLUJO-COMENTARIO -- FLUJO-USUARIOA
                                  where doc.SOCIEDAD_ID == companyCode.ToString() && doc.PERIODO == period && cg.ID == decAccnt && doc.EJERCICIO == year && fl.POS == 2
                                  select new {
                                      cg.ID, cg.NOMBRE,
                                      doc.NUM_DOC,
                                      doc.DOCUMENTO_SAP,
                                      doc.PAYER_ID,
                                      doc.CONCEPTO,
                                      doc.USUARIOD_ID,
                                      cli.NAME1,
                                      fl.COMENTARIO,
                                      fl.USUARIOA_ID,
                                      ta.TALL_ID,
                                      ta.TXT50,
                                      //FECHAC = dsap.FirstOrDefault().FECHAC,
                                      docsap.FECHAC,
                                      doc.MONTO_DOC_MD,
                                      doc.PERIODO,
                                      doc.EJERCICIO
                                  }).Distinct().ToList();



                    var qqueryp2 = (from doc in db.DOCUMENTOes.ToList()
                                    join refe in queryP on doc.DOCUMENTO_REF equals refe.NUM_DOC
                                    select new { refe.NUM_DOC, refe.MONTO_DOC_MD, doc.DOCUMENTO_REF }).ToList();


                    //var docnumb = (from cu in db.cuentas
                    //              join cg in db.cuentagls on cu.abono equals cg.id
                    //              join doc in db.documentoes on cg.id equals doc.cuentap  //num_doc + documento_sap payer_id + cliente-name1 + tallt-txt050 + documento-concepto --documento-usuariod
                    //              join docsap in db.documentosaps on doc.num_doc equals docsap.num_doc
                    //              join ta in db.tallts on doc.tall_id equals ta.tall_id
                    //              join cli in db.clientes on new { doc.vkorg, doc.vtweg, doc.spart, doc.payer_id } equals new { cli.vkorg, cli.vtweg, cli.spart, payer_id = cli.kunnr }   // name1
                    //              join fl in db.flujoes on doc.num_doc equals fl.num_doc  //flujo-comentario -- flujo-usuarioa
                    //              where cu.sociedad_id == companycode.tostring() && doc.periodo == period && cg.id == decaccnt && doc.ejercicio == year
                    //              select new { doc.num_doc}).groupby(o => o.num_doc).take(150).tolist();

                    //foreach (var docNumb1 in docNumb)
                    //{
                    //    var docRef = (from doc in db.DOCUMENTOes
                    //                  where doc.DOCUMENTO_REF == 33
                    //                  select new { doc.MONTO_DOC_MD }).ToList();
                    //    foreach (var monto in docRef)
                    //    {
                    //        Decimal.TryParse(monto.ToString(), out montoDoc);

                    //        docrefs = docrefs + montoDoc;
                    //        provitions.Add(docRef.ToList());
                    //    }
                    //}


                    provitions.Add(qqueryp2);
                    queryList.Add(queryP);

                }
            }



            ViewBag.Consulta2 = queryList;
            ViewBag.MontoNeg = provitions;
            //var queryP = (from cu in db.CUENTAs
            //              join cg in db.CUENTAGLs on cu.ABONO equals cg.ID
            //              join doc in db.DOCUMENTOes on cg.ID equals doc.CUENTAP  //NUM_DOC + DOCUMENTO_SAP PAYER_ID + CLIENTE-NAME1 + TALLT-TXT050 + DOCUMENTO-CONCEPTO --DOCUMENTO-USUARIOD
            //                 // NAME1
            //              where cu.SOCIEDAD_ID == cocode
            //              select new { cg.ID, cg.NOMBRE, doc.NUM_DOC, doc.DOCUMENTO_SAP, doc.PAYER_ID, doc.CONCEPTO, doc.USUARIOD_ID }).ToList();


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
            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);


            return View(dOCUMENTOes.ToList());
        }



        // GET: Reportes/Create
        public ActionResult ReporteProvisiones(string filtroCode)
        {
            int pagina = 1102; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.display = false;
            ViewBag.baseURL = String.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~/"));

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


            ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1");
            ViewBag.CUENTAP = new SelectList(db.CUENTAGLs, "ID", "NOMBRE");
            ViewBag.CUENTAPL = new SelectList(db.CUENTAGLs, "ID", "NOMBRE");
            ViewBag.GALL_ID = new SelectList(db.GALLs, "ID", "DESCRIPCION");
            ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS");
            ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT");
            ViewBag.TALL_ID = new SelectList(db.TALLs, "ID", "DESCRIPCION");
            ViewBag.TSOL_ID = new SelectList(db.TSOLs, "ID", "DESCRIPCION");
            ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS");
            ViewBag.NUM_DOC = new SelectList(db.DOCUMENTOSAPs, "NUM_DOC", "BUKRS");




            //var consultaAnio = (from a in db.DOCUMENTOes
            //                    group a by a.EJERCICIO into EJERCICIO
            //                    select new { EJERCICIO });
            //ViewBag.miconsultaAnio = consultaAnio; ;
            var consultaAnio = (from a in db.DOCUMENTOes select new { a.EJERCICIO }).Distinct().ToList();
            ViewBag.miconsultaAnio = new SelectList(consultaAnio, "EJERCICIO", "EJERCICIO");


            ViewBag.documento = db.DOCUMENTOes.ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();

            //ViewBag.consulta = miConsulta;

            //var miConsulta2 = miConsulta.GroupBy(sociedad => SOCIEDAD_ID);
            //ViewBag.consulta2 = miConsulta2;


            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);
            return View();
        }


        // POST: Reportes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteProvisiones(string filtroCode, int filtroPeriodo, string miconsultaAnio)
        {

            //if (ModelState.IsValid)
            //{
            //    db.DOCUMENTOes.Add(dOCUMENTO);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            int pagina = 1102; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.display = true;
            

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

            //ViewBag.VKORG = new SelectList(db.CLIENTEs, "VKORG", "NAME1", dOCUMENTO.VKORG);
            //ViewBag.CUENTAP = new SelectList(db.CUENTAGLs, "ID", "NOMBRE", dOCUMENTO.CUENTAP);
            //ViewBag.CUENTAPL = new SelectList(db.CUENTAGLs, "ID", "NOMBRE", dOCUMENTO.CUENTAPL);
            //ViewBag.GALL_ID = new SelectList(db.GALLs, "ID", "DESCRIPCION", dOCUMENTO.GALL_ID);
            //ViewBag.PAIS_ID = new SelectList(db.PAIS, "LAND", "SPRAS", dOCUMENTO.PAIS_ID);
            //ViewBag.SOCIEDAD_ID = new SelectList(db.SOCIEDADs, "BUKRS", "BUTXT", dOCUMENTO.SOCIEDAD_ID);
            //ViewBag.TALL_ID = new SelectList(db.TALLs, "ID", "DESCRIPCION", dOCUMENTO.TALL_ID);
            //ViewBag.TSOL_ID = new SelectList(db.TSOLs, "ID", "DESCRIPCION", dOCUMENTO.TSOL_ID);
            //ViewBag.USUARIOC_ID = new SelectList(db.USUARIOs, "ID", "PASS", dOCUMENTO.USUARIOC_ID);
            //ViewBag.NUM_DOC = new SelectList(db.DOCUMENTOSAPs, "NUM_DOC", "BUKRS", dOCUMENTO.NUM_DOC);

            ViewBag.documento = db.DOCUMENTOes.ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.baseURL = String.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~/"));

            string year = Request["selectyear"];
            var code = Request["filtroCode"];
            var periodo = Request["filtroPeriodo"];
            var anio = Request["filtroAnio"];
            //var cocode = code.Request["filtroCode"];
            string[] cocodesplit = code.Split(',');
            
            List<object> lista = new List<object>();
            List<object> rema = new List<object>();
            List<object> perio = new List<object>();
            List<object> comen = new List<object>();
            List<object> cuenta = new List<object>();
            List<object> periodo445 = new List<object>();

            ViewBag.miles = ",";
            ViewBag.decimales = ".";

            
            foreach (string item in cocodesplit)
            {
                var miConsulta = (from x in db.DOCUMENTOes

                                  join CLIENTE in db.CLIENTEs on new { x.VKORG, x.VTWEG, x.SPART, x.PAYER_ID } equals new { CLIENTE.VKORG, CLIENTE.VTWEG, CLIENTE.SPART, PAYER_ID = CLIENTE.KUNNR }
                                  join FLUJO in db.FLUJOes on x.NUM_DOC equals FLUJO.NUM_DOC
                                  join CUENTAGL in db.CUENTAGLs on x.CUENTAP equals CUENTAGL.ID                          
                                  join DOCUMENTOSAP in db.DOCUMENTOSAPs on x.NUM_DOC equals DOCUMENTOSAP.NUM_DOC

                                  where x.SOCIEDAD_ID == item.ToString() && x.PERIODO == filtroPeriodo && x.EJERCICIO == year && FLUJO.POS == 2

                                  select new
                                  {
                                      x.SOCIEDAD_ID,
                                      x.PAIS_ID,
                                      x.NUM_DOC,
                                      x.PAYER_ID,
                                      CLIENTE.NAME1,
                                      CLIENTE.CANAL,
                                      x.CONCEPTO,
                                      x.FECHAC,
                                      x.MONTO_DOC_MD,
                                      x.MONTO_DOC_ML2,
                                      DOCUMENTOSAP.IMPORTE,
                                      //DOCUMENTOSAP.FECHAC,
                                      x.CUENTAP,
                                      CUENTAGL.NOMBRE,                        
                                      x.CUENTAPL,
                                      x.DOCUMENTO_SAP,
                                      x.USUARIOC_ID,
                                      x.USUARIOD_ID,
                                      FLUJO.USUARIOA_ID,
                                      FLUJO.COMENTARIO,
                                      x.EJERCICIO,
                                      x.PERIODO
                                  }).Distinct().ToList();

                var nombregl = (from a in db.DOCUMENTOes
                                join CUENTAGL in db.CUENTAGLs on a.CUENTAPL equals CUENTAGL.ID
                                select new { CUENTAGL.ID, CUENTAGL.NOMBRE }).Distinct().ToList();
                                //.GroupBy(n => new { n.ID, n.NOMBRE })
                                //           .Select(g => g.FirstOrDefault())
                                //           .ToList();

                var per445 = (from p in db.DOCUMENTOes
                              select new { p.EJERCICIO, p.PERIODO }).FirstOrDefault();

                var montos = (from doc in db.DOCUMENTOes.ToList()
                              join refe in miConsulta on doc.DOCUMENTO_REF equals refe.NUM_DOC
                              select new { refe.NUM_DOC, refe.MONTO_DOC_MD, doc.DOCUMENTO_REF }).ToList();

                var period = (from doc in db.DOCUMENTOes.ToList()
                              join per in miConsulta on doc.PERIODO equals per.PERIODO
                              select new { per.NUM_DOC, per.MONTO_DOC_MD, doc.DOCUMENTO_REF }).ToList();

                List<Comentarios> comentarios = (from ff in db.FLUJOes
                                                 join j in
                                                 (from d in db.DOCUMENTOes
                                                  join f in db.FLUJOes on d.NUM_DOC equals f.NUM_DOC
                                                  where f.COMENTARIO != null
                                                  group f by d.NUM_DOC into g
                                                  select new { NUM_DOC = g.Key, POS = g.Max(p => p.POS) })
                                                  on new { ff.NUM_DOC, ff.POS } equals new { j.NUM_DOC, j.POS }
                                                 select new Comentarios { NUM_DOC = ff.NUM_DOC, COMENTARIOS = ff.COMENTARIO }).Distinct().ToList();

                
                lista.Add(miConsulta);
                rema.Add(montos);
                perio.Add(period);
                comen.Add(comentarios);
                cuenta.AddRange(nombregl);
                periodo445.Add(per445);
            }
            ViewBag.miConsulSplit = lista;                 
            ViewBag.remanente = rema;
            ViewBag.peri = perio;
            ViewBag.ultimo = comen.Distinct().ToList(); 
            ViewBag.cuenta = cuenta.Distinct().ToList();
            ViewBag.perio445 = periodo445;

            //CONSULTA DEL FILTRO AÑO
            var consultaAnio = (from a in db.DOCUMENTOes select new { a.EJERCICIO }).Distinct().ToList();
            ViewBag.miconsultaAnio = new SelectList(consultaAnio, "EJERCICIO", "EJERCICIO");

            ViewBag.documento = db.DOCUMENTOes.ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            //ViewBag.consulta = miConsulta;


            //var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP).Where(a => a.SOCIEDAD_ID == SOCIEDAD_ID);
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion SF


        // REPORTE 1 - CONCENTRADO
        public ActionResult ReporteConcentrado()
        {
            int pagina = 1101;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = false;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.paises = db.PAIS.ToList();
            ViewBag.anios = string.Empty;//TODO: Reserch which it's the anio value
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //SWALLOW
            }
            Session["spras"] = user.SPRAS_ID;
            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);
            return View(dOCUMENTOes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteConcentrado(string Form)
        {
            int pagina = 1101;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = true;
            ViewBag.Calendario = cal;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.cuentagl = db.CUENTAGLs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.cuenta = db.CUENTAs.ToList();
            ViewBag.paises = db.PAIS.ToList();
            ViewBag.anios = string.Empty;//TODO: Reserch which it's the anio value
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //SWALLOW
            }
            Session["spras"] = user.SPRAS_ID;

            string[] campossplit;
            //Campos
            string campos = Request["selectcampos"] as string;
            campossplit = null;
            if (!string.IsNullOrEmpty(campos))
            {
                campossplit = campos.Split(',');
            }
            ViewBag.campos = campossplit;
            ViewBag.camposstring = campos;

            ViewBag.selectedcocode = Request["selectcocode"];
            ViewBag.selectedyear = Request["selectyear"];
            ViewBag.selectedperiod = Request["selectperiod"];
            ViewBag.selectedpais = Request["selectpais"];
            ViewBag.selectedcampos = Request["selectcampos"];
            ViewBag.lista_reporte = GenerarConcentrado(Request["selectcocode"], Request["selectyear"], Request["selectperiod"], Request["selectpais"], Request["selectcampos"]);
            return View();
        }

        public dynamic GenerarConcentrado(string selectcocode, string selectyear, string selectperiod, string selectpais, string selectcampos)
        {
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            string[] comcodessplit = { };
            string[] yearsplit = { };
            string[] periodsplit = { };
            string[] paissplit = { };
            string[] campossplit;
            string comcode = selectcocode;
            //Co. Code
            if (!string.IsNullOrEmpty(comcode))
            {
                comcodessplit = comcode.Split(',');
            }
            //Quarter
            string year = selectyear;
            if (!string.IsNullOrEmpty(year))
            {
                yearsplit = year.Split(',');
            }
            //Period
            string period = selectperiod;
            if (!string.IsNullOrEmpty(period))
            {
                periodsplit = period.Split(',');
            }
            //Pais
            string pais = selectpais;
            if (!string.IsNullOrEmpty(pais))
            {
                paissplit = pais.Split(',');
            }
            //Campos
            string campos = selectcampos;
            campossplit = null;
            if (!string.IsNullOrEmpty(campos))
            {
                campossplit = campos.Split(',');
            }

            List<ReportesModel.Concentrado> reporte = new List<Concentrado>();
            List<DOCUMENTO> documentos = db.DOCUMENTOes
                .Where(d => comcodessplit.Contains(d.SOCIEDAD_ID) && periodsplit.Contains(d.PERIODO.ToString()) && yearsplit.Contains(d.EJERCICIO))
                .Include(d => d.CLIENTE)
                .Include(d => d.CARTAs)
                .Include(d => d.CUENTAGL)
                .Include(d => d.CUENTAGL1)
                .Include(d => d.GALL)
                .Include(d => d.PAI)
                .Include(d => d.SOCIEDAD)
                .Include(d => d.TALL)
                .Include(d => d.TSOL)
                .Include(d => d.USUARIO)
                .Include(d => d.DOCUMENTOAs)
                .Include(d => d.DOCUMENTOFs)
                .Include(d => d.DOCUMENTOLs)
                .Include(d => d.DOCUMENTONs)
                .Include(d => d.DOCUMENTOR)
                .Include(d => d.DOCUMENTOPs)
                .Include(d => d.DOCUMENTORECs)
                .Include(d => d.DOCUMENTOTS)
                .Include(d => d.FLUJOes).ToList();

            if (!string.IsNullOrEmpty(pais))
            {
                documentos = documentos.Where(d => paissplit.Contains(d.CLIENTE.LAND)).ToList();
            }

            foreach (DOCUMENTO dOCUMENTO in documentos)
            {
                Concentrado r1 = new Concentrado();
                dOCUMENTO.DOCUMENTORAN = db.DOCUMENTORANs.Where(d => d.NUM_DOC.Equals(dOCUMENTO.NUM_DOC)).ToList();

                decimal montoProv = 0.0M;
                decimal montoApli = 0.0M;
                r1.remanente = 0.0M;
                r1.porcentaje_remanente = 0;
                if (dOCUMENTO.DOCUMENTO_REF != null)
                {
                    montoProv = db.DOCUMENTOes.First(x => x.NUM_DOC == dOCUMENTO.DOCUMENTO_REF).MONTO_DOC_MD.Value;
                }
                else if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == dOCUMENTO.NUM_DOC && x.ESTATUS_C == null))
                {
                    montoProv = dOCUMENTO.MONTO_DOC_MD.Value;
                }

                if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == dOCUMENTO.NUM_DOC && x.ESTATUS_C == null))
                {
                    montoApli = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == dOCUMENTO.NUM_DOC && x.ESTATUS_C == null).Sum(x => x.MONTO_DOC_MD.Value);
                }
                else if (dOCUMENTO.DOCUMENTO_REF != null)
                {
                    if (db.DOCUMENTOes.Any(x => x.DOCUMENTO_REF == dOCUMENTO.DOCUMENTO_REF && x.ESTATUS_C == null))
                    {
                        montoApli = db.DOCUMENTOes.Where(x => x.DOCUMENTO_REF == dOCUMENTO.DOCUMENTO_REF && x.ESTATUS_C == null).Sum(x => x.MONTO_DOC_MD.Value);
                    }
                }
                if (montoProv > 0 && montoApli > 0)
                {
                    r1.remanente = montoProv - montoApli;
                    r1.porcentaje_remanente = Convert.ToInt32((r1.remanente * 100) / montoProv);
                }
                r1.documento = dOCUMENTO;
                if (dOCUMENTO.PAYER_ID == null) continue;
                //dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                //                                        & a.VTWEG.Equals(dOCUMENTO.VTWEG)
                //                                        & a.SPART.Equals(dOCUMENTO.SPART)
                //                                        & a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
                r1.CANAL = db.CANALs.Where(a => a.CANAL1.Equals(dOCUMENTO.CLIENTE.CANAL)).FirstOrDefault();
                var cuentas = (from C in db.CUENTAs
                               join cgl in db.CUENTAGLs on C.CARGO equals cgl.ID
                               where C.SOCIEDAD_ID == dOCUMENTO.SOCIEDAD_ID
                               & C.PAIS_ID == dOCUMENTO.PAIS_ID
                               & C.TALL_ID == dOCUMENTO.TALL_ID
                               & C.EJERCICIO.ToString() == dOCUMENTO.EJERCICIO
                               select new { C.ABONO, C.CARGO, C.CLEARING, C.LIMITE, cgl.NOMBRE }).FirstOrDefault();
                if (cuentas != null)
                {
                    r1.CUENTA_ABONO = Convert.ToDecimal(cuentas.GetType().GetProperty("ABONO").GetValue(cuentas, null)); // Convertir a decimal por si es null
                    r1.CUENTA_CARGO = Convert.ToDecimal(cuentas.GetType().GetProperty("CARGO").GetValue(cuentas, null));
                    r1.CUENTA_CLEARING = Convert.ToDecimal(cuentas.GetType().GetProperty("CLEARING").GetValue(cuentas, null));
                    r1.CUENTA_LIMITE = Convert.ToDecimal(cuentas.GetType().GetProperty("LIMITE").GetValue(cuentas, null));
                    r1.CUENTA_CARGO_NOMBRE = cuentas.GetType().GetProperty("NOMBRE").GetValue(cuentas, null).ToString();
                }
                r1.PRESUPUESTO = getPresupuesto(dOCUMENTO.CLIENTE.KUNNR);
                var proveedor = dOCUMENTO.DOCUMENTOFs.Select(df => df.PROVEEDOR).FirstOrDefault();
                r1.PROVEEDOR_NOMBRE = db.PROVEEDORs.Where(x => x.ID.Equals(proveedor)).Select(p => p.NOMBRE).FirstOrDefault();

                //r1.SEMANA = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear((DateTime)r1.documento.FECHAC, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                r1.SEMANA = (((DateTime)r1.documento.FECHAC).Day + ((int)((DateTime)r1.documento.FECHAC).DayOfWeek)) / 7 + 1;

                r1.STATUS = dOCUMENTO.ESTATUS_WF;
                r1.STATUSS1 = (dOCUMENTO.ESTATUS ?? " ") + (dOCUMENTO.ESTATUS_C ?? " ") + (dOCUMENTO.ESTATUS_SAP ?? " ") + (dOCUMENTO.ESTATUS_WF ?? " ");
                r1.STATUSS3 = (dOCUMENTO.TSOL.PADRE ? "P" : " ");
                var queryFlujo = (from f in db.FLUJOes
                                  join wfp in db.WORKFPs on new { f.WORKF_ID, f.WF_VERSION, f.WF_POS } equals new { WORKF_ID = wfp.ID, WF_VERSION = wfp.VERSION, WF_POS = wfp.POS }
                                  join ac in db.ACCIONs on wfp.ACCION_ID equals ac.ID
                                  orderby f.POS descending
                                  where f.NUM_DOC == dOCUMENTO.NUM_DOC
                                  select ac.TIPO
                                  ).FirstOrDefault();
                r1.STATUSS2 = ((queryFlujo == null) ? " " : queryFlujo.ToString());
                r1.STATUSS4 = ((from dr in db.DOCUMENTORECs
                                where dr.NUM_DOC == dOCUMENTO.NUM_DOC
                                select dr.NUM_DOC
                                  ).ToList().Count > 0 ? "R" : " ");

                string estatuss = r1.STATUSS1 + r1.STATUSS2 + r1.STATUSS3 + r1.STATUSS4;
                if (r1.STATUS == "R")
                {
                    r1.STATUSS = estatuss.Substring(0, 6);
                    try
                    {
                        r1.STATUSS += db.USUARIOs.Where(y => y.ID == db.FLUJOes.Where(x => x.NUM_DOC == dOCUMENTO.NUM_DOC & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault().USUARIOA_ID).FirstOrDefault().PUESTO_ID.ToString();
                    } catch (Exception ex) { }
                    r1.STATUSS += estatuss.Substring(6, 1);
                }
                else
                {
                    r1.STATUSS = estatuss.Substring(0, 6) + " " + estatuss.Substring(6, 1); ;
                }
                Estatus e = new Estatus();
                r1.ESTATUS_STRING = e.getText(r1.STATUSS, dOCUMENTO.NUM_DOC, user.SPRAS_ID);

                r1.DOCSREFREVERSOS = (from d in db.DOCUMENTOes
                                      join dr in db.DOCUMENTORs on d.NUM_DOC equals dr.NUM_DOC
                                      join tr in db.TREVERSATs on dr.TREVERSA_ID equals tr.TREVERSA_ID
                                      where tr.SPRAS_ID == user.SPRAS_ID
                                      where d.DOCUMENTO_REF == r1.documento.NUM_DOC
                                      select new { d, dr, tr }).FirstOrDefault();

                r1.DOCREVERSOS2 = (from d in db.DOCUMENTOes
                                   join ts in db.TSOLs on d.TSOL_ID equals ts.ID
                                   where ts.REVERSO
                                   where (bool)ts.ACTIVO
                                   where d.DOCUMENTO_REF == r1.documento.NUM_DOC
                                   select d).FirstOrDefault();

                r1.DOCBACKORDER = (from dl in db.DOCUMENTOLs
                                   where dl.NUM_DOC == r1.documento.NUM_DOC
                                   select dl).FirstOrDefault();

                reporte.Add(r1);
            }
            return reporte;
        }

        [HttpPost]
        public FileResult ExportReporteConcentrado()
        {
            int pagina = 1101;
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            List<ExcelExportColumn> columnas = new List<ExcelExportColumn>();
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_1"))).FirstOrDefault().TEXTOS, "CoCode_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_2"))).FirstOrDefault().TEXTOS, "Pais_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_3"))).FirstOrDefault().TEXTOS, "Subregion_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_4"))).FirstOrDefault().TEXTOS, "Estado_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_5"))).FirstOrDefault().TEXTOS, "Ciudad_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_6"))).FirstOrDefault().TEXTOS, "NumeroSolicitud_STRING", Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "Solicitudes/Details/", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_7"))).FirstOrDefault().TEXTOS, "FechaSolicitud_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_8"))).FirstOrDefault().TEXTOS, "HoraSolicitud_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_9"))).FirstOrDefault().TEXTOS, "SemanaPeriodo_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_10"))).FirstOrDefault().TEXTOS, "PeriodoContable_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_11"))).FirstOrDefault().TEXTOS, "Anio_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_12"))).FirstOrDefault().TEXTOS, "TipoSolicitud_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_13"))).FirstOrDefault().TEXTOS, "TipoProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_14"))).FirstOrDefault().TEXTOS, "TipoNC_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_15"))).FirstOrDefault().TEXTOS, "TipoOP_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_16"))).FirstOrDefault().TEXTOS, "Status_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_17"))).FirstOrDefault().TEXTOS, "Concepto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_18"))).FirstOrDefault().TEXTOS, "Mecanica_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_19"))).FirstOrDefault().TEXTOS, "De_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_20"))).FirstOrDefault().TEXTOS, "A_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_21"))).FirstOrDefault().TEXTOS, "DePeriodo_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_22"))).FirstOrDefault().TEXTOS, "APeriodo_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_23"))).FirstOrDefault().TEXTOS, "Clasificacion_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_24"))).FirstOrDefault().TEXTOS, "CuentaContableGasto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_25"))).FirstOrDefault().TEXTOS, "NombreCuenta_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_26"))).FirstOrDefault().TEXTOS, "Cliente_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_27"))).FirstOrDefault().TEXTOS, "Nombre_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_28"))).FirstOrDefault().TEXTOS, "TipoCliente_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_29"))).FirstOrDefault().TEXTOS, "OrganizacionVentas_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_30"))).FirstOrDefault().TEXTOS, "TaxID_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_31"))).FirstOrDefault().TEXTOS, "Canal_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_32"))).FirstOrDefault().TEXTOS, "Descripcion_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_33"))).FirstOrDefault().TEXTOS, "NombreContacto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_34"))).FirstOrDefault().TEXTOS, "EmailContacto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_35"))).FirstOrDefault().TEXTOS, "MontoOriginalProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_36"))).FirstOrDefault().TEXTOS, "MontoNCOP_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_37"))).FirstOrDefault().TEXTOS, "MontoAplicado_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_38"))).FirstOrDefault().TEXTOS, "SaldoRemanenteProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_39"))).FirstOrDefault().TEXTOS, "MontoReversoProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_40"))).FirstOrDefault().TEXTOS, "PorcentajeReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_41"))).FirstOrDefault().TEXTOS, "MontoExcesoProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_42"))).FirstOrDefault().TEXTOS, "PorcentajeExcesoProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_43"))).FirstOrDefault().TEXTOS, "Impactos_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_44"))).FirstOrDefault().TEXTOS, "NumRegistroProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_45"))).FirstOrDefault().TEXTOS, "NumRegistroNCOP_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_46"))).FirstOrDefault().TEXTOS, "NumRegistroReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_47"))).FirstOrDefault().TEXTOS, "FechaReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_48"))).FirstOrDefault().TEXTOS, "PeriodoContableReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_49"))).FirstOrDefault().TEXTOS, "RazonReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_50"))).FirstOrDefault().TEXTOS, "ComentarioReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_51"))).FirstOrDefault().TEXTOS, "Usuario_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_52"))).FirstOrDefault().TEXTOS, "Backup_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_53"))).FirstOrDefault().TEXTOS, "CreadoPor_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_54"))).FirstOrDefault().TEXTOS, "CreadoPorID_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_55"))).FirstOrDefault().TEXTOS, "SolicitadoPor_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_56"))).FirstOrDefault().TEXTOS, "SolicitadoPorID_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_57"))).FirstOrDefault().TEXTOS, "ModificadoPor_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_58"))).FirstOrDefault().TEXTOS, "ModificadoPorID_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_59"))).FirstOrDefault().TEXTOS, "AprobadorN1_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_60"))).FirstOrDefault().TEXTOS, "AprobadorN2_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_61"))).FirstOrDefault().TEXTOS, "AprobadorN3_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_62"))).FirstOrDefault().TEXTOS, "AprobadorN4_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_63"))).FirstOrDefault().TEXTOS, "AprobadorN5_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_64"))).FirstOrDefault().TEXTOS, "Proveedor_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_65"))).FirstOrDefault().TEXTOS, "NombreProveedor_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_66"))).FirstOrDefault().TEXTOS, "NumeroFacturaProveedor_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_67"))).FirstOrDefault().TEXTOS, "NumeroFacturaK_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_68"))).FirstOrDefault().TEXTOS, "NumeroNC_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_69"))).FirstOrDefault().TEXTOS, "NumeroOP_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_70"))).FirstOrDefault().TEXTOS, "ExpRecognitionPeriodo_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_71"))).FirstOrDefault().TEXTOS, "ExpRecognitionEjercicio_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_72"))).FirstOrDefault().TEXTOS, "SoporteIncorrecto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_73"))).FirstOrDefault().TEXTOS, "SoporteValidado_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_74"))).FirstOrDefault().TEXTOS, "Carta_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_75"))).FirstOrDefault().TEXTOS, "Contrato_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_76"))).FirstOrDefault().TEXTOS, "JBP_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_77"))).FirstOrDefault().TEXTOS, "Factura_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_78"))).FirstOrDefault().TEXTOS, "Otros_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_79"))).FirstOrDefault().TEXTOS, "NegociacionMonto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_80"))).FirstOrDefault().TEXTOS, "NegociacionPorcentaje_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_81"))).FirstOrDefault().TEXTOS, "DistribucionMaterial_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_82"))).FirstOrDefault().TEXTOS, "DistribucionCategoria_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_83"))).FirstOrDefault().TEXTOS, "MontoBase_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_84"))).FirstOrDefault().TEXTOS, "MontoPorcentaje_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_85"))).FirstOrDefault().TEXTOS, "Recurrente_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_86"))).FirstOrDefault().TEXTOS, "RecurrentePorcentaje_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_87"))).FirstOrDefault().TEXTOS, "RecurrenteMonto_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_88"))).FirstOrDefault().TEXTOS, "RecurrenteCancelada_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_89"))).FirstOrDefault().TEXTOS, "ObjetivoInicio_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_90"))).FirstOrDefault().TEXTOS, "ObjetivoLimite_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_91"))).FirstOrDefault().TEXTOS, "Estatus_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_92"))).FirstOrDefault().TEXTOS, "VentaPeriodo_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_93"))).FirstOrDefault().TEXTOS, "MontoProvision_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_94"))).FirstOrDefault().TEXTOS, "BackOrder_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_95"))).FirstOrDefault().TEXTOS, "VentaRealBackOrder_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_96"))).FirstOrDefault().TEXTOS, "NCOPReverso_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_campo_97"))).FirstOrDefault().TEXTOS, "MontoProvision_STRING", true));

            var datos = GenerarConcentrado(Request["selectedcocode"], Request["selectedyear"], Request["selectedperiod"], Request["selectedpais"], Request["selectedcampos"]);
            string[] subfiltros = Request["selectedsubfilters"].Split(new string[] { "|T,T|" }, StringSplitOptions.None);
            string[] subfiltros_ordenado = subfiltros;
            for (int index = 0; index < subfiltros.Length; index++)
            {
                string temp_subfiltro = subfiltros[index];
                string[] subfiltroArray = temp_subfiltro.Split(new string[] { "|ST|" }, StringSplitOptions.None);
                if (!String.IsNullOrEmpty(subfiltroArray[0]))
                    if (subfiltroArray.Length < 2)
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = string.Empty;
                    else
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = subfiltroArray[1];
            }
            string nombreArchivo = ExcelExport.generarExcel(columnas, datos, "Concentrado", Server.MapPath(ExcelExport.getRuta()), subfiltros_ordenado);
            return File(Server.MapPath(ExcelExport.getRuta() + nombreArchivo), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }

        [HttpPost]
        public FileResult ExportReporteConcentrado2()
        {
            //var dOCUMENTOes = db.DOCUMENTOes.Where(a => a.USUARIOC_ID.Equals(User.Identity.Name)).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.CLIENTE).Include(d => d.PAI).Include(d => d.SOCIEDAD).ToList();
            //var dOCUMENTOVs = db.DOCUMENTOVs.Where(a => a.USUARIOA_ID.Equals(User.Identity.Name)).ToList();
            //var tsol = db.TSOLs.ToList();
            //var tall = db.TALLs.ToList();
            //foreach (DOCUMENTOV v in dOCUMENTOVs)
            //{
            //    DOCUMENTO d = new DOCUMENTO();
            //    var ppd = d.GetType().GetProperties();
            //    var ppv = v.GetType().GetProperties();
            //    foreach (var pv in ppv)
            //    {
            //        foreach (var pd in ppd)
            //        {
            //            if (pd.Name == pv.Name)
            //            {
            //                pd.SetValue(d, pv.GetValue(v));
            //                break;
            //            }
            //        }
            //    }
            //    d.TSOL = tsol.Where(a => a.ID.Equals(d.TSOL_ID)).FirstOrDefault();
            //    d.TALL = tall.Where(a => a.ID.Equals(d.TALL_ID)).FirstOrDefault();
            //    dOCUMENTOes.Add(d);
            //}
            //dOCUMENTOes = dOCUMENTOes.Distinct(new DocumentoComparer()).ToList();
            //dOCUMENTOes = dOCUMENTOes.OrderByDescending(a => a.FECHAC).OrderByDescending(a => a.NUM_DOC).ToList();

            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            string us = "";
            us = User.Identity.Name;
            var sociedades = user.SOCIEDADs;

            List<CSP_DOCUMENTOSXUSER2_Result> dOCUMENTOes = db.CSP_DOCUMENTOSXUSER2(us, user.SPRAS_ID).ToList();

            List<Documento> listaDocs = new List<Documento>();
            foreach (var cocode in sociedades)
            {
                List<CSP_DOCUMENTOSXCOCODE_Result> dOCUMENTOesc = db.CSP_DOCUMENTOSXCOCODE(cocode.BUKRS, user.SPRAS_ID).ToList();
                foreach (CSP_DOCUMENTOSXCOCODE_Result item in dOCUMENTOesc)
                {
                    Documento ld = new Documento();
                    ld.BUTTON = item.BUTTON;

                    ld.NUM_DOC = item.NUM_DOC;
                    ld.NUM_DOC_TEXT = item.NUM_DOC_TEXT;
                    ld.SOCIEDAD_ID = item.SOCIEDAD_ID;
                    ld.PAIS_ID = item.PAIS_ID;
                    ld.FECHADD = item.FECHAD.Value.Day + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Year;
                    ld.FECHAD = item.FECHAD.Value.Year + "/" + item.FECHAD.Value.Month + "/" + item.FECHAD.Value.Day;
                    ld.HORAC = item.HORAC.Value.ToString().Split('.')[0];
                    ld.PERIODO = item.PERIODO + "";

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
                    ld.ESTATUS = e.getText(item.ESTATUSS, ld.NUM_DOC, user.SPRAS_ID);
                    ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS, ld.NUM_DOC);
                    //ld.ESTATUS = e.getText(item.ESTATUSS);
                    //ld.ESTATUS_CLASS = e.getClass(item.ESTATUSS);

                    ld.PAYER_ID = item.PAYER_ID;

                    ld.CLIENTE = item.NAME1;
                    ld.CANAL = item.CANAL;
                    ld.TSOL = item.TXT020;
                    ld.TALL = item.TXT50;
                    //foreach (CUENTA cuenta in db.CUENTAs.Where(x => x.SOCIEDAD_ID.Equals(item.SOCIEDAD_ID)).ToList())
                    //{
                    //    if (item.TALL != null)
                    //    {
                    //        if (cuenta.TALL_ID == item.TALL.ID)
                    //        {
                    //            ld.CUENTAS = cuenta.CARGO.ToString();
                    //            break;
                    //        }
                    //    }
                    //}
                    try
                    {
                        ld.CUENTAS = item.CARGO + "";
                        ld.CUENTAP = item.CUENTAP;
                        ld.CUENTAPL = item.CUENTAPL;
                        ld.CUENTACL = item.CUENTACL;
                    }
                    catch { }
                    ld.CONCEPTO = item.CONCEPTO;
                    ld.MONTO_DOC_ML = item.MONTO_DOC_MD != null ? Convert.ToDecimal(item.MONTO_DOC_MD).ToString("C") : "";

                    ld.FACTURA = item.FACTURA;
                    ld.FACTURAK = item.FACTURAK;

                    ld.USUARIOC_ID = item.USUARIOD_ID;
                    ld.USUARIOM_ID = item.USUARIOD_ID;

                    if (item.DOCUMENTO_SAP != null)
                    {
                        if (item.PADRE)
                        {
                            ld.NUM_PRO = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_NC = "";
                            ld.NUM_REV = "";
                        }
                        else if (item.REVERSO)
                        {
                            ld.NUM_REV = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_NC = "";
                            ld.NUM_PRO = "";
                        }
                        else
                        {
                            ld.NUM_NC = item.DOCUMENTO_SAP;
                            ld.NUM_AP = "";
                            ld.NUM_PRO = "";
                            ld.NUM_REV = "";
                        }
                        //<!--NUM_SAP-->
                        ld.BLART = item.BLART;
                        ld.NUM_PAYER = item.KUNNR;
                        ld.NUM_CLIENTE = item.DESCR;
                        ld.NUM_IMPORTE = item.IMPORTE != null ? Convert.ToDecimal(item.IMPORTE).ToString("C") : "";
                        ld.NUM_CUENTA = item.CUENTA_C;
                    }
                    else
                    {
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                        //<td></td>
                    }

                    listaDocs.Add(ld);
                }
            }
            string nombre_archivo = generarExcelConcentrado(listaDocs.OrderByDescending(t => t.FECHAD).ThenByDescending(t => t.HORAC).ThenByDescending(t => t.NUM_DOC).ToList(), Server.MapPath("~/PdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/" + nombre_archivo), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombre_archivo);
        }

        public string generarExcelConcentrado(List<Documento> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                //Creamos el encabezado
                worksheet.Cell("A1").Value = new[]
{
                  new {
                      BANNER = "Recurrencia"
                      },
                    };
                worksheet.Cell("B1").Value = new[]
             {
                  new {
                      BANNER = "Número Solicitud"
                      },
                    };
                worksheet.Cell("C1").Value = new[]
            {
                  new {
                      BANNER = "Company Code"
                      },
                    };
                worksheet.Cell("D1").Value = new[]
            {
                  new {
                      BANNER = "País"
                      },
                    };
                worksheet.Cell("E1").Value = new[]
            {
                  new {
                      BANNER = "Fecha Solicitud"
                      },
                    };
                worksheet.Cell("F1").Value = new[]
            {
                  new {
                      BANNER = "Hora de Solicitud"
                      },
                    };
                worksheet.Cell("G1").Value = new[]
            {
                  new {
                      BANNER = "Período Contable"
                      },
                    };
                worksheet.Cell("H1").Value = new[]
              {
                  new {
                      BANNER = "Estatus"
                      },
                    };
                worksheet.Cell("I1").Value = new[]
            {
                  new {
                      BANNER = "Cliente ID"
                      },
                    };
                worksheet.Cell("J1").Value = new[]
            {
                  new {
                      BANNER = "Cliente"
                      },
                    };
                worksheet.Cell("K1").Value = new[]
                {
                    new {
                        BANNER = "Canal"
                    },
                };
                worksheet.Cell("L1").Value = new[]
            {
                  new {
                      BANNER = "Tipo Solicitud"
                      },
                    };
                worksheet.Cell("M1").Value = new[]
            {
                  new {
                      BANNER = "Clasificación Allowances"
                      },
                    };
                worksheet.Cell("N1").Value = new[]
      {
                  new {
                      BANNER = "Cuenta Contable Gasto"
                      },
                    };
                worksheet.Cell("O1").Value = new[]
{
                  new {
                      BANNER = "Cuenta Contable Pasivo"
                      },
                    };
                worksheet.Cell("P1").Value = new[]
                {
                  new {
                      BANNER = "Cuenta Contable Clearing"
                      },
                    };
                worksheet.Cell("Q1").Value = new[]
{
                  new {
                      BANNER = "Descripción Solicitud"
                      },
                    };
                worksheet.Cell("R1").Value = new[]
      {
                  new {
                      BANNER = "$ Importe Solicitud"
                      },
                    };
                worksheet.Cell("S1").Value = new[]
      {
                  new {
                      BANNER = "Número Factura Proveedor"
                      },
                    };
                worksheet.Cell("T1").Value = new[]
      {
                  new {
                      BANNER = "Número Factura Kellogg"
                      },
                    };
                worksheet.Cell("U1").Value = new[]
                {
                  new {
                      BANNER = "Creado por"
                      },
                    };
                worksheet.Cell("V1").Value = new[]
     {
                  new {
                      BANNER = "Modificado por"
                      },
                    };
                worksheet.Cell("W1").Value = new[]
      {
                  new {
                      BANNER = "Núm. Registro Provisión"
                      },
                    };
                worksheet.Cell("X1").Value = new[]
     {
                  new {
                      BANNER = "Núm. Registro NC/OP"
                      },
                    };
                worksheet.Cell("Y1").Value = new[]
  {
                  new {
                      BANNER = "Núm. Registro AP"
                      },
                    };
                worksheet.Cell("Z1").Value = new[]
                {
                  new {
                      BANNER = "Núm. Registro Reverso"
                      },
                    };
                worksheet.Cell("AA1").Value = new[]
                {
                  new {
                      BANNER = "Tipo Registro SAP"
                      },
                    };
                worksheet.Cell("AB1").Value = new[]
                {
                  new {
                      BANNER = "Cliente ID"
                      },
                    };
                worksheet.Cell("AC1").Value = new[]
               {
                  new {
                      BANNER = "Cliente"
                      },
                    };
                worksheet.Cell("AD1").Value = new[]
              {
                  new {
                      BANNER = "$ Importe Moneda Local"
                      },
                    };
                worksheet.Cell("AF1").Value = new[]
              {
                  new {
                      BANNER = "Cuenta Contable Gasto"
                      },
                    };
                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    worksheet.Cell("A" + i).Value = new[]
{
                  new {
                      BANNER       = !String.IsNullOrEmpty(lst[i-2].TIPO_RECURRENTE)?"X":""
                      },
                    };
                    worksheet.Cell("B" + i).Value = new[]
                 {
                  new {
                      BANNER       = lst[i-2].NUM_DOC
                      },
                    };
                    try
                    {
                        worksheet.Cell(i - 2, "B").Hyperlink = new XLHyperlink(new Uri(Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "Solicitudes/Details/" + lst[i - 2].NUM_DOC));
                    }
                    catch (Exception e) { }
                    worksheet.Cell("C" + i).Value = new[]
                {
                  new {
                      BANNER       = lst[i-2].SOCIEDAD_ID
                      },
                    };
                    worksheet.Cell("D" + i).Value = new[]
                 {
                    new {
                        BANNER       = lst[i-2].PAIS_ID
                        },
                      };
                    worksheet.Cell("E" + i).Value = new[]
                  {
                   new {
                       BANNER       = lst[i-2].FECHAD
                       },
                     };
                    worksheet.Cell("F" + i).Value = new[]
               {
                  new {
                      BANNER       = lst[i-2].HORAC.ToString().Split('.')[0]
                      },
                    };
                    var fx = lst[i - 2].PERIODO;
                    worksheet.Cell("G" + i).Value = new[]
                 {
                  new {
                      BANNER       = fx
                      },
                    };
                    //Verdes
                    worksheet.Cell("H" + i).Value = new[]
                {
                        new
                        {
                            BANNER = lst[i - 2].ESTATUS
                    },
                };
                    worksheet.Cell("I" + i).Value = new[]
                {
                    new {
                        BANNER       = lst[i-2].PAYER_ID
                        },
                      };
                    //Para sacar el Nombre "I" y "J"
                    worksheet.Cell("J" + i).Value = new[]
                    {
                                //List Clientes y for para sacar su name 
                                new {
                                    BANNER = lst[i-2].CLIENTE
                                },
                            };

                    worksheet.Cell("K" + i).Value = new[]
                    {
                                //   List Clientes y for para sacar su name
                                new
                                {
                                    BANNER = lst[i-2].CANAL
                                },
                            };
                    worksheet.Cell("L" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].TSOL
                        },
                    };
                    worksheet.Cell("M" + i).Value = new[]
                   {
                        new {
                            BANNER       = lst[i-2].TALL
                        },
                    };

                    worksheet.Cell("N" + i).Value = new[]
                    {
                                new {
                                    BANNER       = lst[i-2].CUENTAP
                                },
                            };
                    worksheet.Cell("O" + i).Value = new[]
                            {
                                new {
                                    BANNER       = lst[i-2].CUENTAPL
                                },
                            };
                    worksheet.Cell("P" + i).Value = new[]
                            {
                                new {
                                    BANNER       = lst[i-2].CUENTACL
                                },
                            };

                    worksheet.Cell("Q" + i).Value = new[]
                   {
                        new {
                            BANNER       = lst[i-2].CONCEPTO
                        },
                    };
                    worksheet.Cell("R" + i).Value = new[]
                   {
                        new {
                            BANNER       = lst[i-2].MONTO_DOC_ML
                        },
                    };
                    worksheet.Cell("S" + i).Value = new[]
                    {
                                new {
                                    BANNER       =lst[i-2].FACTURA
                                },
                            };

                    worksheet.Cell("T" + i).Value = new[]
                    {
                                new {
                                    BANNER       = lst[i-2].FACTURAK
                                },
                            };

                    worksheet.Cell("U" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].USUARIOC_ID
                        },
                    };
                    worksheet.Cell("V" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].USUARIOC_ID
                        },
                    };
                    worksheet.Cell("W" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_PRO
                        },
                    };
                    worksheet.Cell("X" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_NC
                        },
                    };
                    worksheet.Cell("Y" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_AP
                        },
                    };
                    worksheet.Cell("Z" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_REV
                        },
                    };
                    worksheet.Cell("AA" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].BLART
                        },
                    };
                    worksheet.Cell("AB" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_PAYER
                        },
                    };
                    worksheet.Cell("AC" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_CLIENTE
                        },
                    };
                    worksheet.Cell("AD" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_IMPORTE
                        },
                    };
                    worksheet.Cell("AF" + i).Value = new[]
                    {
                        new {
                            BANNER       = lst[i-2].NUM_CUENTA
                        },
                    };
                }
                string nombre_archivo = "Concentrado" + DateTime.Now.ToString("HHmmyyyyMMdd") + ".xlsx";
                var rt = ruta + nombre_archivo;
                workbook.SaveAs(rt);
                return nombre_archivo;
            }
            catch (Exception e)
            {
                var ex = e.ToString();
                return String.Empty;
            }

        }

        public PRESUPUESTO_MOD getPresupuesto(string kunnr)
        {
            PRESUPUESTO_MOD pm = new PRESUPUESTO_MOD();
            //try
            //{
            //    if (kunnr == null)
            //        kunnr = "";

            //    //Obtener presupuesto
            //    string mes = DateTime.Now.Month.ToString();
            //    var presupuesto = db.CSP_PRESU_CLIENT(cLIENTE: kunnr, pERIODO: mes).Select(p => new { DESC = p.DESCRIPCION.ToString(), VAL = p.VALOR.ToString() }).ToList();
            //    string clien = db.CLIENTEs.Where(x => x.KUNNR == kunnr).Select(x => x.BANNERG).First();
            //    if (presupuesto != null)
            //    {
            //        if (String.IsNullOrEmpty(clien))
            //        {
            //            pm.P_CANAL = presupuesto[0].VAL;
            //            pm.P_BANNER = presupuesto[1].VAL;
            //            pm.PC_C = (float.Parse(presupuesto[4].VAL) + float.Parse(presupuesto[5].VAL) + float.Parse(presupuesto[6].VAL)).ToString();
            //            pm.PC_A = presupuesto[8].VAL;
            //            pm.PC_P = presupuesto[9].VAL;
            //            pm.PC_T = presupuesto[10].VAL;
            //            pm.CONSU = (float.Parse(presupuesto[1].VAL) - float.Parse(presupuesto[10].VAL)).ToString();
            //        }
            //        else
            //        {
            //            pm.P_CANAL = presupuesto[0].VAL;
            //            pm.P_BANNER = presupuesto[0].VAL;
            //            pm.PC_C = (float.Parse(presupuesto[4].VAL) + float.Parse(presupuesto[5].VAL) + float.Parse(presupuesto[6].VAL)).ToString();
            //            pm.PC_A = presupuesto[8].VAL;
            //            pm.PC_P = presupuesto[9].VAL;
            //            pm.PC_T = presupuesto[10].VAL;
            //            pm.CONSU = (float.Parse(presupuesto[0].VAL) - float.Parse(presupuesto[10].VAL)).ToString();
            //        }
            //    }
            //}
            //catch
            //{

            //}

            return pm;
        }

        // FIN REPORTE 1 - CONCENTRADO

        // REPORTE 4.1 - ALLOWANCESPL
        public ActionResult ReporteAllowancesPL()
        {
            int pagina = 1104;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = false;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.canales = db.CANALs.ToList();
            ViewBag.payers = db.CLIENTEs.Select(x => x.KUNNR).ToList();
            ViewBag.categories = db.MATERIALGPTs.Where(x => !string.IsNullOrEmpty(x.TXT50)).Select(x => x.TXT50).Distinct().ToList();
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //SWALLOW
            }
            Session["spras"] = user.SPRAS_ID;
            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);
            return View(dOCUMENTOes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteAllowancesPL(string Form)
        {
            int pagina = 1104;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();

            ViewBag.display = true;
            ViewBag.Calendario = cal;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.cuentagl = db.CUENTAGLs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.canales = db.CANALs.ToList();
            ViewBag.payers = db.CLIENTEs.Select(x => x.KUNNR).Distinct().ToList();
            ViewBag.categories = db.MATERIALGPTs.Where(x => !string.IsNullOrEmpty(x.TXT50)).Select(x => x.TXT50).Distinct().ToList();
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //SWALLOW
            }

            ViewBag.selectedcocode = Request["selectcocode"];
            ViewBag.selectedq = Request["selectq"];
            ViewBag.selectedperiod = Request["selectperiod"];
            ViewBag.selectedpayer = Request["selectpayer"];
            ViewBag.selectedcategory = Request["selectcategory"];
            ViewBag.selectedyear = Request["selectyear"];
            ViewBag.selectedcanal = Request["selectcanal"];
            ViewBag.reporte = GenerarAllowancesPL(Request["selectcocode"], Request["selectq"], Request["selectperiod"], Request["selectpayer"], Request["selectcategory"], Request["selectyear"], Request["selectcanal"]);

            Session["spras"] = user.SPRAS_ID;
            return View();
        }

        public dynamic GenerarAllowancesPL(string selectcocode, string selectq, string selectperiod, string selectpayer, string selectcategory, string selectyear, string selectcanal)
        {
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            //Co. Codes
            string[] comcodessplit = { };
            string comcode = selectcocode;
            if (!string.IsNullOrEmpty(comcode))
            {
                comcodessplit = comcode.Split(',');
            }

            //Quarters
            string[] quartersplit = { };
            List<string> quarterperiod = new List<string>();
            string quarter = selectq;
            if (!string.IsNullOrEmpty(quarter))
            {
                quartersplit = quarter.Split(',');
            }
            foreach (string q in quartersplit)
            {
                switch (q)
                {
                    case "1":
                        quarterperiod.Add("1");
                        quarterperiod.Add("2");
                        quarterperiod.Add("3");
                        break;
                    case "2":
                        quarterperiod.Add("4");
                        quarterperiod.Add("5");
                        quarterperiod.Add("6");
                        break;
                    case "3":
                        quarterperiod.Add("7");
                        quarterperiod.Add("8");
                        quarterperiod.Add("9");
                        break;
                    case "4":
                        quarterperiod.Add("10");
                        quarterperiod.Add("11");
                        quarterperiod.Add("12");
                        break;
                }
            }
            //Periods
            string[] periodsplit = { };
            string period = selectperiod;
            if (!string.IsNullOrEmpty(period))
            {
                periodsplit = period.Split(',');
            }
            //Payers
            string[] payersplit = { };
            string payer = selectpayer;
            if (!string.IsNullOrEmpty(payer))
            {
                payersplit = payer.Split(',');
            }
            //Categories
            string[] categorysplit = { };
            string category = selectcategory;
            if (!string.IsNullOrEmpty(category))
            {
                categorysplit = category.Split(',');
            }
            string year = selectyear;
            //Canal
            string[] canalsplit = { };
            string canal = selectcanal;
            if (!string.IsNullOrEmpty(canal))
            {
                canalsplit = canal.Split(',');
            }

            var queryDocs = (from d in db.DOCUMENTOes
                             join c in db.CLIENTEs on new { d.VKORG, d.VTWEG, d.SPART, d.PAYER_ID } equals new { c.VKORG, c.VTWEG, c.SPART, PAYER_ID = c.KUNNR }
                             join ca in db.CANALs on c.CANAL equals ca.CANAL1
                             join ts in db.TSOLs on d.TSOL_ID equals ts.ID
                             join f in db.FLUJOes on d.NUM_DOC equals f.NUM_DOC
                             join wfp in db.WORKFPs on new { f.WORKF_ID, f.WF_POS, f.WF_VERSION } equals new { WORKF_ID = wfp.ID, WF_POS = wfp.POS, WF_VERSION = wfp.VERSION }
                             join ac in db.ACCIONs on wfp.ACCION_ID equals ac.ID
                             join dp in db.DOCUMENTOPs on d.NUM_DOC equals dp.NUM_DOC
                             join m in db.MATERIALs on dp.MATNR equals m.ID
                             join mgpt in db.MATERIALGPTs on m.MATERIALGP_ID equals mgpt.MATERIALGP_ID
                             where mgpt.SPRAS_ID == "EN" //user.SPRAS_ID
                                 && d.EJERCICIO == year
                                 && (!string.IsNullOrEmpty(canal) ? canalsplit.Contains(c.CANAL) : true)
                                 && (!string.IsNullOrEmpty(comcode) ? comcodessplit.Contains(d.SOCIEDAD_ID) : true)
                                 && (!string.IsNullOrEmpty(period) ? periodsplit.Contains(d.PERIODO.ToString()) : true)
                                 && (!string.IsNullOrEmpty(quarter) ? quarterperiod.Contains(d.PERIODO.ToString()) : true)
                                 && (!string.IsNullOrEmpty(payer) ? payersplit.Contains(d.PAYER_ID) : true)
                                 && (!string.IsNullOrEmpty(category) ? categorysplit.Contains(mgpt.TXT50) : true)
                             select new { d.PERIODO, d.EJERCICIO, d.SOCIEDAD_ID, d.PAYER_ID, c.CANAL, ca.CDESCRIPCION, mgpt.TXT50, d.TALL_ID, f.ESTATUS, d.ESTATUS_C, d.ESTATUS_SAP, d.ESTATUS_WF, ac.TIPO, ts.PADRE, d.MONTO_DOC_ML })
                             .ToList()
                             .Select(s => new { s.PERIODO, s.EJERCICIO, s.SOCIEDAD_ID, s.PAYER_ID, s.CANAL, s.CDESCRIPCION, s.TXT50, s.TALL_ID, STATUS_ALLOWANCE = statusAllowance(s.ESTATUS, s.ESTATUS_C, s.ESTATUS_SAP, s.ESTATUS_WF, s.TIPO, s.PADRE), s.MONTO_DOC_ML })
                             .GroupBy(x => new { x.PERIODO, x.EJERCICIO, x.SOCIEDAD_ID, x.PAYER_ID, x.CANAL, x.CDESCRIPCION, x.TXT50 })
                             .ToList();

            List<AllowancesPL> alls = new List<AllowancesPL>();
            foreach (var doc in queryDocs)
            {
                AllowancesPL all = new AllowancesPL();
                all.PERIODO = doc.First().PERIODO.ToString();
                all.YEAR = doc.First().EJERCICIO;
                all.BU = doc.First().SOCIEDAD_ID;
                all.CANAL = doc.First().CANAL + " " + doc.First().CDESCRIPCION;
                all.PAYER = doc.First().PAYER_ID;
                all.CATEGORIA = doc.First().TXT50;
                foreach (var doc2 in doc)
                {
                    switch (doc2.TALL_ID)
                    {
                        case "1000000008": // Cash Discounts
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.CD_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.CD_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000017": // Clearance
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.C_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.C_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000013": // Consumer Data
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.COD_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.COD_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000005": // Direct Plant Ship & Customer Pickup Allowance
                        case "1000000006":
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.DPS_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.DPS_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000010": // Distribution Comission
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.DC_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.DC_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000003": // Everyday Low Price
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.ELP_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.ELP_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000011": // Free Goods
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.FG_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.FG_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "": // Government Discounts
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.GD_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.GD_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000012": // Growth Program
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.GP_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.GP_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000009": // Logistic Discount
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.LD_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.LD_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case " ": // Margin Support
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.MS_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.MS_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000016": // Rollbacks
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.R_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.R_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "  ": // Trade Promotion – Non-Kellogg Coupons
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.TP_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.TP_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000004": // Sponsorship / In-Store adv (before: Booklets)
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.SIS_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.SIS_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000014": // Store Openings
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.SO_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.SO_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000002": // Trade Promotion – In-Store sampling Demostration cost
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.TPIS_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.TPIS_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000001": // Trade Promotion - Other
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.TPO_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.TPO_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000015": // Unsaleables
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.U_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.U_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                        case "1000000007": // Warehouse Allowances (“DxD”)
                            switch (doc2.STATUS_ALLOWANCE)
                            {
                                case "En Proceso TAT":
                                    all.WA_EN_PROCESO += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                                case "Allowance TAT":
                                    all.WA_ALLOWANCE_TAT += (Decimal)doc2.MONTO_DOC_ML;
                                    break;
                            }
                            break;
                    }
                }
                //var queryPresup = (from p in db.PRESUPSAPPs
                //                   join m in db.MATERIALs on p.MATNR equals m.ID
                //                   join mgpt in db.MATERIALGPTs on m.MATERIALGP_ID equals mgpt.MATERIALGP_ID
                //                   //where mgpt.SPRAS_ID == user.SPRAS_ID
                //                   //&& mgpt.TXT50 == all.CATEGORIA
                //                   //&& p.PERIOD.ToString() == all.PERIODO
                //                   //&& p.ANIO.ToString() == all.YEAR
                //                   //&& p.BUKRS == all.BU
                //                   //&& p.BANNER == all.PAYER
                //                   select new { p.TYPE, p.VVX17, p.CSHDC, p.RECUN, p.DSTRB, p.OTHTA, p.ADVER, p.CORPM, p.POP, p.OTHER, p.CONPR, p.OHV, p.FREEG, p.RSRDV, p.SPA, p.PMVAR, p.GRSLS, p.NETLB })
                //                   .GroupBy(x => new { x.TYPE })
                //                   .ToList();
                //                   //.ToList();
                //foreach(var presup in queryPresup)
                //{
                //    switch(presup.First().TYPE)
                //    {
                //        case "F":
                //            all.CD_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Cash Discount
                //            all.C_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Clearance
                //            all.COD_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CONPR); // Consumer Data
                //            all.DPS_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Direct Plant Ship & Customer Pickup Allowance
                //            all.DC_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.DSTRB); // Distribution Comission
                //            all.ELP_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Everyday Low Price
                //            all.FG_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Free Goods
                //            all.GD_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Government Discounts
                //            all.GP_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Growth Program
                //            all.LD_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Logistic Discount
                //            all.MS_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Margin Support
                //            all.R_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Rollbacks
                //            all.TP_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Trade Promotion – Non-Kellogg Coupons
                //            all.SIS_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Sponsorship / In-Store adv (before: Booklets)
                //            all.SO_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Store Openings
                //            all.TPIS_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Trade Promotion – In-Store sampling Demostration cost
                //            all.TPO_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Trade Promotion - Other
                //            all.U_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Unsaleables
                //            all.WA_ALLOWANCE_FACT += (Decimal)presup.Sum(x => x.CSHDC); // Warehouse Allowances (“DxD”)
                //            break;
                //        case "B":
                //            break;
                //    }
                //}
                //all.CD_ALLOWANCE_FACT += 
                /*
                  ,[VVX17]
                  ,[RECUN]
                  ,[OTHTA]
                  ,[ADVER]
                  ,[CORPM]
                  ,[POP]
                  ,[OTHER]
                  ,[CONPR]
                  ,[OHV]
                  ,[FREEG]
                  ,[RSRDV]
                  ,[SPA]
                  ,[PMVAR]
                  ,[GRSLS]
                  ,[NETLB]
                 */
                alls.Add(all);
            }

            return alls.ToList();
        }

        [HttpPost]
        public FileResult ExportReporteAllowancesPL()
        {
            int pagina = 1104;
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            List<ExcelExportColumn> columnas = new List<ExcelExportColumn>();
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_Periodo"))).FirstOrDefault().TEXTOS, "PERIODO", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_anio"))).FirstOrDefault().TEXTOS, "YEAR", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_BU"))).FirstOrDefault().TEXTOS, "BU", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_Payer"))).FirstOrDefault().TEXTOS, "PAYER", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_Cliente"))).FirstOrDefault().TEXTOS, "CLIENTE", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_Canal"))).FirstOrDefault().TEXTOS, "CANAL", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_categoria"))).FirstOrDefault().TEXTOS, "CATEGORIA", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_CD_EN_PROCESO"))).FirstOrDefault().TEXTOS, "CD_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_CD_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "CD_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_CD_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "CD_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_CD_AJUSTES"))).FirstOrDefault().TEXTOS, "CD_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_CD_TOTALES"))).FirstOrDefault().TEXTOS, "CD_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_C_EN_PROCESO"))).FirstOrDefault().TEXTOS, "C_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_C_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "C_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_C_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "C_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_C_AJUSTES"))).FirstOrDefault().TEXTOS, "C_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_C_TOTALES"))).FirstOrDefault().TEXTOS, "C_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_COD_EN_PROCESO"))).FirstOrDefault().TEXTOS, "COD_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_COD_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "COD_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_COD_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "COD_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_COD_AJUSTES"))).FirstOrDefault().TEXTOS, "COD_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_COD_TOTALES"))).FirstOrDefault().TEXTOS, "COD_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DPS_EN_PROCESO"))).FirstOrDefault().TEXTOS, "DPS_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DPS_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "DPS_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DPS_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "DPS_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DPS_AJUSTES"))).FirstOrDefault().TEXTOS, "DPS_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DPS_TOTALES"))).FirstOrDefault().TEXTOS, "DPS_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DC_EN_PROCESO"))).FirstOrDefault().TEXTOS, "DC_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DC_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "DC_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DC_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "DC_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DC_AJUSTES"))).FirstOrDefault().TEXTOS, "DC_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_DC_TOTALES"))).FirstOrDefault().TEXTOS, "DC_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ELP_EN_PROCESO"))).FirstOrDefault().TEXTOS, "ELP_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ELP_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "ELP_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ELP_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "ELP_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ELP_AJUSTES"))).FirstOrDefault().TEXTOS, "ELP_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ELP_TOTALES"))).FirstOrDefault().TEXTOS, "ELP_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_FG_EN_PROCESO"))).FirstOrDefault().TEXTOS, "FG_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_FG_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "FG_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_FG_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "FG_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_FG_AJUSTES"))).FirstOrDefault().TEXTOS, "FG_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_FG_TOTALES"))).FirstOrDefault().TEXTOS, "FG_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GD_EN_PROCESO"))).FirstOrDefault().TEXTOS, "GD_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GD_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "GD_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GD_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "GD_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GD_AJUSTES"))).FirstOrDefault().TEXTOS, "GD_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GD_TOTALES"))).FirstOrDefault().TEXTOS, "GD_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GP_EN_PROCESO"))).FirstOrDefault().TEXTOS, "GP_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GP_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "GP_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GP_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "GP_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GP_AJUSTES"))).FirstOrDefault().TEXTOS, "GP_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_GP_TOTALES"))).FirstOrDefault().TEXTOS, "GP_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LD_EN_PROCESO"))).FirstOrDefault().TEXTOS, "LD_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LD_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "LD_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LD_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "LD_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LD_AJUSTES"))).FirstOrDefault().TEXTOS, "LD_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LD_TOTALES"))).FirstOrDefault().TEXTOS, "LD_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_MS_EN_PROCESO"))).FirstOrDefault().TEXTOS, "MS_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_MS_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "MS_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_MS_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "MS_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_MS_AJUSTES"))).FirstOrDefault().TEXTOS, "MS_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_MS_TOTALES"))).FirstOrDefault().TEXTOS, "MS_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_R_EN_PROCESO"))).FirstOrDefault().TEXTOS, "R_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_R_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "R_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_R_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "R_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_R_AJUSTES"))).FirstOrDefault().TEXTOS, "R_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_R_TOTALES"))).FirstOrDefault().TEXTOS, "R_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TP_EN_PROCESO"))).FirstOrDefault().TEXTOS, "TP_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TP_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "TP_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TP_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "TP_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TP_AJUSTES"))).FirstOrDefault().TEXTOS, "TP_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TP_TOTALES"))).FirstOrDefault().TEXTOS, "TP_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SIS_EN_PROCESO"))).FirstOrDefault().TEXTOS, "SIS_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SIS_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "SIS_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SIS_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "SIS_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SIS_AJUSTES"))).FirstOrDefault().TEXTOS, "SIS_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SIS_TOTALES"))).FirstOrDefault().TEXTOS, "SIS_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SO_EN_PROCESO"))).FirstOrDefault().TEXTOS, "SO_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SO_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "SO_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SO_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "SO_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SO_AJUSTES"))).FirstOrDefault().TEXTOS, "SO_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_SO_TOTALES"))).FirstOrDefault().TEXTOS, "SO_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPIS_EN_PROCESO"))).FirstOrDefault().TEXTOS, "TPIS_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPIS_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "TPIS_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPIS_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "TPIS_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPIS_AJUSTES"))).FirstOrDefault().TEXTOS, "TPIS_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPIS_TOTALES"))).FirstOrDefault().TEXTOS, "TPIS_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPO_EN_PROCESO"))).FirstOrDefault().TEXTOS, "TPO_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPO_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "TPO_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPO_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "TPO_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPO_AJUSTES"))).FirstOrDefault().TEXTOS, "TPO_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_TPO_TOTALES"))).FirstOrDefault().TEXTOS, "TPO_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_U_EN_PROCESO"))).FirstOrDefault().TEXTOS, "U_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_U_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "U_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_U_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "U_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_U_AJUSTES"))).FirstOrDefault().TEXTOS, "U_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_U_TOTALES"))).FirstOrDefault().TEXTOS, "U_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_WA_EN_PROCESO"))).FirstOrDefault().TEXTOS, "WA_EN_PROCESO_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_WA_ALLOWANCE_TAT"))).FirstOrDefault().TEXTOS, "WA_ALLOWANCE_TAT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_WA_ALLOWANCE_FACT"))).FirstOrDefault().TEXTOS, "WA_ALLOWANCE_FACT_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_WA_AJUSTES"))).FirstOrDefault().TEXTOS, "WA_AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_WA_TOTALES"))).FirstOrDefault().TEXTOS, "WA_TOTALES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_PROCESO_TAT_TOTAL"))).FirstOrDefault().TEXTOS, "PROCESO_TAT_TOTAL_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ALLOWANCE_TAT_TOTAL"))).FirstOrDefault().TEXTOS, "ALLOWANCE_TAT_TOTAL_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ALLOWANCE_FACT_TOTAL"))).FirstOrDefault().TEXTOS, "ALLOWANCE_FACT_TOTAL_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_AJUSTES"))).FirstOrDefault().TEXTOS, "AJUSTES_STRING", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ALLOWANCE_TOTALES"))).FirstOrDefault().TEXTOS, "ALLOWANCE_TOTALES_STRING", true));

            var datos = GenerarAllowancesPL(Request["selectedcocode"], Request["selectedq"], Request["selectedperiod"], Request["selectedpayer"], Request["selectedcategory"], Request["selectedyear"], Request["selectedcanal"]);
            string[] subfiltros = Request["selectedsubfilters"].Split(new string[] { "|T,T|" }, StringSplitOptions.None);
            string[] subfiltros_ordenado = subfiltros;
            for (int index = 0; index < subfiltros.Length; index++)
            {
                string temp_subfiltro = subfiltros[index];
                string[] subfiltroArray = temp_subfiltro.Split(new string[] { "|ST|" }, StringSplitOptions.None);
                if (!String.IsNullOrEmpty(subfiltroArray[0]))
                    if (subfiltroArray.Length < 2)
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = string.Empty;
                    else
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = subfiltroArray[1];
            }
            string nombreArchivo = ExcelExport.generarExcel(columnas, datos, "AllowancesPL", Server.MapPath(ExcelExport.getRuta()), subfiltros_ordenado);
            return File(Server.MapPath(ExcelExport.getRuta() + nombreArchivo), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }

        private string statusAllowance(string ESTATUS, string ESTATUS_C, string ESTATUS_SAP, string ESTATUS_WF, string TIPO, bool PADRE)
        {
            string estatus = "";

            // FORMAR LA CADENA DE STATUS PARA IDENTIFICAR EL MENSAJE A DESPLEGAR
            if (ESTATUS != null) { estatus += ESTATUS; } else { estatus += " "; }
            if (ESTATUS_C != null) { estatus += ESTATUS_C; } else { estatus += " "; }
            if (ESTATUS_SAP != null) { estatus += ESTATUS_SAP; } else { estatus += " "; }
            if (ESTATUS_WF != null) { estatus += ESTATUS_WF; } else { estatus += " "; }
            if (TIPO != null) { estatus += TIPO; } else { estatus += " "; }
            if (PADRE) { estatus += "P"; } else { estatus += " "; }

            // ESTABLECER EL MENSAJE DE STATUS QUE SE MOSTRARÁ
            //if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]")) { return ""; } // "Cancelada"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R].")) { return "En Proceso TAT"; } //"Pendiente validación TS"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A].")) { return "En Proceso TAT"; } // "Pendiente aprobador"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A]..")) { return "En Proceso TAT"; } //"Por gen .txt"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A]..")) { return "En Proceso TAT"; } // "Por contabilizar"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A]..")) { return "En Proceso TAT"; } // "Por gen .txt"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A]..")) { return "En Proceso TAT"; } // "Por contabilizar"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[X][A]..")) { return ""; } // "Error en contabilización"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]")) { return "En Proceso TAT"; } // "Abierta"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A]..")) { return "Allowance TAT"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R]..")) { return "En Proceso TAT"; } // "Pendiente corrección usuario"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R][S].")) { return "En Proceso TAT"; } // "Pendiente corrección usuario TS"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S]..")) { return "En Proceso TAT"; } // "Pendiente firma"; }
            //else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T]..")) { return "En Proceso TAT"; } // "Pendiente tax"; }
            //else { return ""; }

            string ret = "";
            if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "^.[C]"))
                ret = "";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][R]."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[P][A]."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[N]..[A].."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[C]..[A].."))
                ret = "Allowance TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "[P]..[A].."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[P][A].."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "..[E][A].."))
                ret = "";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].[P]"))
                ret = "Allowance TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[A].."))
                ret = "Allowance TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R].."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[R].."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[S].."))
                ret = "En Proceso TAT";
            else if (System.Text.RegularExpressions.Regex.IsMatch(estatus, "...[T].."))
                ret = "En Proceso TAT";

            return ret;
        }

        // FIN REPORTE 4.1 - ALLOWANCESPL

        // REPORTE 4.2 - ALLOWANCESB
        public ActionResult ReporteAllowancesB()
        {
            int pagina = 1105;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = false;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.quarteruarter = string.Empty;//TODO: Reserch which it's the q value
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //SWALLOW
            }
            Session["spras"] = user.SPRAS_ID;
            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);
            return View(dOCUMENTOes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteAllowancesB(string Form)
        {
            int pagina = 1105;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = true;
            ViewBag.Calendario = cal;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.cuentagl = db.CUENTAGLs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.cuenta = db.CUENTAs.ToList();
            ViewBag.quarter = string.Empty;//TODO: Reserch which it's the q value
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
                //SWALLOW
            }
            Session["spras"] = user.SPRAS_ID;

            //            select d.CUENTAP, cgl.NOMBRE, d.SOCIEDAD_ID, f.ESTATUS, d.ESTATUS_C, d.ESTATUS_SAP, d.ESTATUS_WF, ac.TIPO, ts.PADRE
            // from DOCUMENTO d
            //join CUENTAGL cgl on cgl.ID = d.CUENTAP
            //join TSOL ts on d.TSOL_ID = ts.ID
            //join FLUJO f on d.NUM_DOC = f.NUM_DOC
            //join WORKFP wfp on f.WORKF_ID = wfp.ID and f.WF_POS = wfp.POS and f.WF_VERSION = wfp.VERSION
            //join ACCION ac on wfp.ACCION_ID = ac.ID
            ViewBag.selectedcocode = Request["selectcocode"];
            ViewBag.selectedquarter = Request["selectquarter"];
            ViewBag.selectedperiod = Request["selectperiod"];
            ViewBag.selectedyear = Request["selectyear"];
            ViewBag.reporte = GenerarAllowancesB(Request["selectcocode"], Request["selectquarter"], Request["selectperiod"], Request["selectyear"]);

            return View();
        }

        public dynamic GenerarAllowancesB(string selectcocode, string selectquarter, string selectperiod, string selectyear)
        {
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            string[] comcodessplit = { };
            string[] quartersplit = { };
            List<string> quarterperiod = new List<string>();
            string[] periodsplit = { };
            string comcode = selectcocode;

            //Co. Code
            if (!string.IsNullOrEmpty(comcode))
            {
                comcodessplit = comcode.Split(',');
            }

            //Quarter
            string quarter = selectquarter;
            if (!string.IsNullOrEmpty(quarter))
            {
                quartersplit = quarter.Split(',');
            }
            foreach (string q in quartersplit)
            {
                switch (q)
                {
                    case "1":
                        quarterperiod.Add("1");
                        quarterperiod.Add("2");
                        quarterperiod.Add("3");
                        break;
                    case "2":
                        quarterperiod.Add("4");
                        quarterperiod.Add("5");
                        quarterperiod.Add("6");
                        break;
                    case "3":
                        quarterperiod.Add("7");
                        quarterperiod.Add("8");
                        quarterperiod.Add("9");
                        break;
                    case "4":
                        quarterperiod.Add("10");
                        quarterperiod.Add("11");
                        quarterperiod.Add("12");
                        break;
                }
            }

            //Period
            string period = selectperiod;
            if (!string.IsNullOrEmpty(period))
            {
                periodsplit = period.Split(',');
            }

            string year = selectyear;

            var queryDocs = (from d in db.DOCUMENTOes
                                 //join ds in db.DOCUMENTOSAPs on d.NUM_DOC equals ds.NUM_DOC
                             join cgl in db.CUENTAGLs on d.CUENTAPL equals cgl.ID
                             join ts in db.TSOLs on d.TSOL_ID equals ts.ID
                             join f in db.FLUJOes on d.NUM_DOC equals f.NUM_DOC
                             join wfp in db.WORKFPs on new { f.WORKF_ID, f.WF_POS, f.WF_VERSION } equals new { WORKF_ID = wfp.ID, WF_POS = wfp.POS, WF_VERSION = wfp.VERSION }
                             join ac in db.ACCIONs on wfp.ACCION_ID equals ac.ID
                             where d.EJERCICIO == year
                                 && (!string.IsNullOrEmpty(comcode) ? comcodessplit.Contains(d.SOCIEDAD_ID) : true)
                                 && (!string.IsNullOrEmpty(period) ? periodsplit.Contains(d.PERIODO.ToString()) : true)
                                 && (!string.IsNullOrEmpty(quarter) ? quarterperiod.Contains(d.PERIODO.ToString()) : true)
                             orderby d.CUENTAPL, cgl.NOMBRE, d.SOCIEDAD_ID
                             select new { CUENTA_A = d.CUENTAPL, cgl.NOMBRE, d.SOCIEDAD_ID, d.MONTO_DOC_ML, d.MONTO_DOC_ML2, f.ESTATUS, d.ESTATUS_C, d.ESTATUS_SAP, d.ESTATUS_WF, ac.TIPO, ts.PADRE }
                             )
                             .ToList()
                             .Select(s => new { s.CUENTA_A, s.NOMBRE, s.SOCIEDAD_ID, s.MONTO_DOC_ML, s.MONTO_DOC_ML2, STATUS_ALLOWANCE = statusAllowance(s.ESTATUS, s.ESTATUS_C, s.ESTATUS_SAP, s.ESTATUS_WF, s.TIPO, s.PADRE) })
                             .GroupBy(x => new { x.CUENTA_A, x.NOMBRE, x.STATUS_ALLOWANCE })
                             .ToList();

            List<AllowancesB> alls = new List<AllowancesB>();
            foreach (var doc in queryDocs)
            {
                AllowancesB all = new AllowancesB();
                all.CUENTA_DE_BALANCE = doc.First().CUENTA_A.ToString();
                all.DESCRIPCION = doc.First().NOMBRE;
                all.FUENTE = doc.First().STATUS_ALLOWANCE;
                foreach (var doc2 in doc)
                {
                    decimal monto = ((doc2.MONTO_DOC_ML == null) ? 0 : (Decimal)doc2.MONTO_DOC_ML);
                    decimal monto2 = ((doc2.MONTO_DOC_ML2 == null) ? 0 : (Decimal)doc2.MONTO_DOC_ML2);
                    switch (doc2.SOCIEDAD_ID)
                    {
                        case "KCMX":
                            all.KCMX += monto;
                            all.KCMX_USD += monto2;
                            break;
                        case "KLCA":
                            all.KLCA += monto;
                            all.KLCA_USD += monto2;
                            break;
                        case "LCCR":
                            all.LCCR += monto;
                            all.LCCR_USD += monto2;
                            break;
                        case "LPKP":
                            all.LPKP += monto;
                            all.LPKP_USD += monto2;
                            break;
                        case "KLSV":
                            all.KLSV += monto;
                            all.KLSV_USD += monto2;
                            break;
                        case "KCAR":
                            all.KCAR += monto;
                            all.KCAR_USD += monto2;
                            break;
                        case "KPRS":
                            all.KPRS += monto;
                            all.KPRS_USD += monto2;
                            break;
                        case "KLCO":
                            all.KLCO += monto;
                            all.KLCO_USD += monto2;
                            break;
                        case "LEKE":
                            all.LEKE += monto;
                            all.LEKE_USD += monto2;
                            break;
                        case "LAGA":
                            all.LAGA += monto;
                            all.LAGA_USD += monto2;
                            break;
                        case "KLCH":
                            all.KLCH += monto;
                            all.KLCH_USD += monto2;
                            break;
                    }
                }
                if (!String.IsNullOrEmpty(all.FUENTE))
                {
                    alls.Add(all);
                }
            }

            foreach (IGrouping<string, AllowancesB> all in alls.GroupBy(x => x.CUENTA_DE_BALANCE))
            {
                if (all.Count() < 2)
                {
                    AllowancesB tmpall = new AllowancesB();
                    tmpall.CUENTA_DE_BALANCE = all.First().CUENTA_DE_BALANCE;
                    tmpall.DESCRIPCION = all.First().DESCRIPCION;
                    tmpall.FUENTE = ((all.First().DESCRIPCION == "En Proceso TAT") ? "Allowance TAT" : "En Proceso TAT");
                    alls.Add(tmpall);
                }
                AllowancesB tmp_all = new AllowancesB();
                tmp_all.CUENTA_DE_BALANCE = all.First().CUENTA_DE_BALANCE;
                tmp_all.DESCRIPCION = all.First().DESCRIPCION;
                tmp_all.FUENTE = "Allowances Facturados";
                alls.Add(tmp_all);
                tmp_all = new AllowancesB();
                tmp_all.CUENTA_DE_BALANCE = all.First().CUENTA_DE_BALANCE;
                tmp_all.DESCRIPCION = all.First().DESCRIPCION;
                tmp_all.FUENTE = "Ajustes y Reclasificaciones Manuales";
                alls.Add(tmp_all);
            }

            foreach (AllowancesB tall in alls)
            {
                switch (tall.FUENTE)
                {
                    case "En Proceso TAT":
                        tall.ORDEN = 1;
                        break;
                    case "Allowance TAT":
                        tall.ORDEN = 2;
                        break;
                    case "Allowances Facturados":
                        tall.ORDEN = 3;
                        break;
                    case "Ajustes y Reclasificaciones Manuales":
                        tall.ORDEN = 4;
                        break;
                }
            }

            return alls.ToList().OrderBy(x => x.CUENTA_DE_BALANCE).ThenBy(x => x.ORDEN).ToList();
        }

        [HttpPost]
        public FileResult ExportReporteAllowancesB()
        {
            int pagina = 1105;
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            List<ExcelExportColumn> columnas = new List<ExcelExportColumn>();
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_cuenta_balance"))).FirstOrDefault().TEXTOS, "CUENTA_DE_BALANCE", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_descrpcion"))).FirstOrDefault().TEXTOS, "DESCRIPCION"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_fuente"))).FirstOrDefault().TEXTOS, "FUENTE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KCMX"))).FirstOrDefault().TEXTOS, "KCMX_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLCA"))).FirstOrDefault().TEXTOS, "KLCA_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LCCR"))).FirstOrDefault().TEXTOS, "LCCR_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LPKP"))).FirstOrDefault().TEXTOS, "LPKP_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLSV"))).FirstOrDefault().TEXTOS, "KLSV_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KCAR"))).FirstOrDefault().TEXTOS, "KCAR_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KPRS"))).FirstOrDefault().TEXTOS, "KPRS_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLCO"))).FirstOrDefault().TEXTOS, "KLCO_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LEKE"))).FirstOrDefault().TEXTOS, "LEKE_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LAGA"))).FirstOrDefault().TEXTOS, "LAGA_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLCH"))).FirstOrDefault().TEXTOS, "KLCH_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KCMXUSD"))).FirstOrDefault().TEXTOS, "KCMX_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLCAUSD"))).FirstOrDefault().TEXTOS, "KLCA_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LCCRUSD"))).FirstOrDefault().TEXTOS, "LCCR_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LPKPUSD"))).FirstOrDefault().TEXTOS, "LPKP_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLSVUSD"))).FirstOrDefault().TEXTOS, "KLSV_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KCARUSD"))).FirstOrDefault().TEXTOS, "KCAR_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KPRSUSD"))).FirstOrDefault().TEXTOS, "KPRS_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLCOUSD"))).FirstOrDefault().TEXTOS, "KLCO_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LEKEUSD"))).FirstOrDefault().TEXTOS, "LEKE_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_LAGAUSD"))).FirstOrDefault().TEXTOS, "LAGA_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_KLCHUSD"))).FirstOrDefault().TEXTOS, "KLCH_USD_STRING"));

            var datos = GenerarAllowancesB(Request["selectedcocode"], Request["selectedquarter"], Request["selectedperiod"], Request["selectedyear"]);
            string[] subfiltros = Request["selectedsubfilters"].Split(new string[] { "|T,T|" }, StringSplitOptions.None);
            string[] subfiltros_ordenado = subfiltros;
            for(int index = 0; index < subfiltros.Length; index++)
            {
                string temp_subfiltro = subfiltros[index];
                string[] subfiltroArray = temp_subfiltro.Split(new string[] { "|ST|" }, StringSplitOptions.None);
                if(!String.IsNullOrEmpty(subfiltroArray[0]))
                    if (subfiltroArray.Length < 2)
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = string.Empty;
                    else
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = subfiltroArray[1];
            }

            string nombreArchivo = ExcelExport.generarExcel(columnas, datos, "AllowancesBalance", Server.MapPath(ExcelExport.getRuta()), subfiltros_ordenado);
            return File(Server.MapPath(ExcelExport.getRuta() + nombreArchivo), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }

        // FIN REPORTE 4.2 - ALLOWANCESB

        // REPORTE 5 - MRLTS
        public ActionResult ReporteMRLTS()
        {
            int pagina = 1106;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = false;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.dperiodo = db.PERIODOes.ToList();
            ViewBag.aperiodo = db.PERIODOes.ToList();

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
            }
            Session["spras"] = user.SPRAS_ID;
            var dOCUMENTOes = db.DOCUMENTOes.Include(d => d.CLIENTE).Include(d => d.CUENTAGL).Include(d => d.CUENTAGL1).Include(d => d.GALL).Include(d => d.PAI).Include(d => d.SOCIEDAD).Include(d => d.TALL).Include(d => d.TSOL).Include(d => d.USUARIO).Include(d => d.DOCUMENTOSAP);
            return View(dOCUMENTOes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteMRLTS(string Form)
        {
            int pagina = 1106;
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = true;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.cuentagl = db.CUENTAGLs.ToList();
            ViewBag.cuenta = db.CUENTAs.ToList();
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
            }
            Session["spras"] = user.SPRAS_ID;
            // ViewBag.Calendario = cal;
            // ViewBag.Consulta = "";
            ViewBag.dperiodo = db.PERIODOes.ToList();
            ViewBag.aperiodo = db.PERIODOes.ToList();
            ViewBag.selectedcocode = Request["selectcocode"];
            ViewBag.selecteddperiod = Request["selectdperiod"];
            ViewBag.selectedaperiod = Request["selectaperiod"];
            ViewBag.selectedyear = Request["selectyear"];
            ViewBag.tabla_reporte = GenerarMRLTS(Request["selectcocode"], Int32.Parse(Request["selectdperiod"]), Int32.Parse(Request["selectaperiod"]), Request["selectyear"]);
            return View();
        }

        public dynamic GenerarMRLTS(string selectcocode, int selectdperiod, int selectaperiod, string selectyear)
        {
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            // EVALUAR FILTROS
            string[] comcodessplit = new string[] { };
            string comcode = selectcocode;
            if (!string.IsNullOrEmpty(comcode))
            {
                comcodessplit = comcode.Split(',');
            }
            int dperiod = selectdperiod;
            int aperiod = selectaperiod;
            string year = selectyear;

            var queryP = (from d in db.DOCUMENTOes
                          join p in db.PAIS on d.PAIS_ID equals p.LAND
                          join c in db.CLIENTEs on new { d.VKORG, d.VTWEG, d.SPART, d.PAYER_ID } equals new { c.VKORG, c.VTWEG, c.SPART, PAYER_ID = c.KUNNR }
                          join ts in db.TSOLs on d.TSOL_ID equals ts.ID
                          join us in db.USUARIOs on d.USUARIOC_ID equals us.ID
                          join tst in db.TSOLTs on new { TSOL_ID = ts.ID, us.SPRAS_ID } equals new { tst.TSOL_ID, tst.SPRAS_ID }
                          join pt in db.PUESTOTs on new { us.SPRAS_ID, PUESTO_ID = (int)(us.PUESTO_ID) } equals new { pt.SPRAS_ID, pt.PUESTO_ID }
                          join g in db.GALLs on d.GALL_ID equals g.ID
                          join gt in db.GALLTs on new { GALL_ID = g.ID, us.SPRAS_ID } equals new { gt.GALL_ID, gt.SPRAS_ID }
                          orderby d.FECHAC descending // , d.NUM_DOC ascending, f.WF_POS ascending
                          where ((d.PERIODO >= dperiod && d.PERIODO <= aperiod) || (d.PERIODO >= aperiod && d.PERIODO <= dperiod))
                              && d.EJERCICIO == year
                              && (!string.IsNullOrEmpty(comcode) ? comcodessplit.Contains(d.SOCIEDAD_ID) : true)
                          select new MRLTS
                          {
                              CO_CODE = d.SOCIEDAD_ID,
                              PAIS = p.LANDX,
                              NUMERO_SOLICITUD = d.NUM_DOC,
                              FECHA_SOLICITUD = ((d.FECHAC == null) ? new DateTime() : (DateTime)d.FECHAC),
                              PERIODO_CONTABLE = (Int32)d.PERIODO,
                              ANIO_CONTABLE = d.EJERCICIO,
                              NUMERO_DOCUMENTO_SAP = d.DOCUMENTO_SAP,
                              //NUMERO_REVERSO_SAP =
                              //FECHA_REVERSO = (DateTime)dr.FECHAC,
                              //PERIODO_CONTABLE_REVERSO =
                              //COMENTARIOS_REVERSO_PROVISION = dr.COMENTARIO,
                              TIPO_SOLICITUD_CODE = tst.TXT010,
                              TIPO_SOLICITUD = tst.TXT020,
                              TIPO_SOLICITUD_ID = d.TSOL_ID,
                              STATUS = d.ESTATUS_WF,
                              STATUSS1 = (d.ESTATUS ?? " ") + (d.ESTATUS_C ?? " ") + (d.ESTATUS_SAP ?? " ") + (d.ESTATUS_WF ?? " "),
                              STATUSS3 = (ts.PADRE ? "P" : " "),
                              CONCEPTO_SOLICITUD = d.CONCEPTO,
                              DE = ((d.FECHAI_VIG == null) ? new DateTime() : (DateTime)d.FECHAI_VIG),
                              A = ((d.FECHAF_VIG == null) ? new DateTime() : (DateTime)d.FECHAF_VIG),
                              CLASIFICACION = gt.TXT50,
                              NUMERO_CLIENTE = d.PAYER_ID,
                              CLIENTE = c.NAME1,
                              MONTO = (decimal)d.MONTO_DOC_MD,
                              MONEDA = d.MONEDA_ID,
                              TIPO_CAMBIO = (decimal)d.TIPO_CAMBIO,
                              MONTO_2 = (decimal)d.MONTO_DOC_ML2
                          }).ToList();

            foreach (MRLTS renglon in queryP)
            {
                renglon.EXPENSE_RECOGNITION = ((from dts in db.DOCUMENTOTS
                                                where dts.NUM_DOC == renglon.NUMERO_SOLICITUD && (dts.TSFORM_ID == 1 || dts.TSFORM_ID == 2 || dts.TSFORM_ID == 5 || dts.TSFORM_ID == 7 || dts.TSFORM_ID == 9 || dts.TSFORM_ID == 10) && ((bool)dts.CHECKS)
                                                select dts.NUM_DOC
                                                ).ToList().Count > 0 ? renglon.MONTO : 0);

                var queryReverso = (from drs in db.DOCUMENTORs
                                    where drs.NUM_DOC == renglon.NUMERO_SOLICITUD
                                    select new { drs.COMENTARIO, drs.NUM_DOC, drs.FECHAC }
                                    ).FirstOrDefault();
                if (queryReverso == null)
                {
                    renglon.ES_REVERSO = false;
                }
                else
                {
                    renglon.ES_REVERSO = true;
                    renglon.FECHA_REVERSO = ((queryReverso.FECHAC == null) ? new DateTime() : (DateTime)queryReverso.FECHAC);
                    renglon.COMENTARIOS_REVERSO_PROVISION = queryReverso.COMENTARIO;
                }

                var queryFlujo = (from f in db.FLUJOes
                                  join wfp in db.WORKFPs on new { f.WORKF_ID, f.WF_VERSION, f.WF_POS } equals new { WORKF_ID = wfp.ID, WF_VERSION = wfp.VERSION, WF_POS = wfp.POS }
                                  join ac in db.ACCIONs on wfp.ACCION_ID equals ac.ID
                                  orderby f.POS descending
                                  where f.NUM_DOC == renglon.NUMERO_SOLICITUD
                                  select ac.TIPO
                                  ).FirstOrDefault();
                renglon.STATUSS2 = ((queryFlujo == null) ? " " : queryFlujo.ToString());
                renglon.STATUSS4 = ((from dr in db.DOCUMENTORECs
                                     where dr.NUM_DOC == renglon.NUMERO_SOLICITUD
                                     select dr.NUM_DOC
                                  ).ToList().Count > 0 ? "R" : " ");

                string estatuss = renglon.STATUSS1 + renglon.STATUSS2 + renglon.STATUSS3 + renglon.STATUSS4;
                if (renglon.STATUS == "R")
                {
                    try
                    {
                        renglon.STATUSS = estatuss.Substring(0, 6) +
                                        db.FLUJOes.Where(x => x.NUM_DOC == renglon.NUMERO_SOLICITUD & x.ESTATUS == "R").OrderByDescending(a => a.POS).FirstOrDefault().USUARIO.PUESTO_ID +
                                        estatuss.Substring(6, 1);
                    } catch (Exception ex)
                    {
                        renglon.STATUSS = estatuss.Substring(0, 6) + " " + estatuss.Substring(6, 1);
                    }
                }
                else
                {
                    renglon.STATUSS = estatuss.Substring(0, 6) + " " + estatuss.Substring(6, 1);
                }
                Estatus e = new Estatus();
                renglon.ESTATUS_STRING = e.getText(renglon.STATUSS, renglon.NUMERO_SOLICITUD, user.SPRAS_ID);
                renglon.d = db.DOCUMENTOes.Find(renglon.NUMERO_SOLICITUD);
            }
            return queryP;
        }

        [HttpPost]
        public FileResult ExportReporteMRLTS()
        {
            int pagina = 1106;
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            List<ExcelExportColumn> columnas = new List<ExcelExportColumn>();
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_code"))).FirstOrDefault().TEXTOS, "CO_CODE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_pais"))).FirstOrDefault().TEXTOS, "PAIS"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_num_solicitud"))).FirstOrDefault().TEXTOS, "NUMERO_SOLICITUD", Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "Solicitudes/Details/", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_fecha_solicitud"))).FirstOrDefault().TEXTOS, "FECHA_SOLICITUD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_periodo_contable"))).FirstOrDefault().TEXTOS, "PERIODO_CONTABLE_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_anio_contable"))).FirstOrDefault().TEXTOS, "ANIO_CONTABLE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_no_doc_SAP"))).FirstOrDefault().TEXTOS, "NUMERO_DOCUMENTO_SAP"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_no_rev_SAP"))).FirstOrDefault().TEXTOS, "NUMERO_REVERSO_SAP"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_fecha_reverso"))).FirstOrDefault().TEXTOS, "FECHA_REVERSO_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_p_contable_reverso"))).FirstOrDefault().TEXTOS, "PERIODO_CONTABLE_REVERSO"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_c_reverso_provision"))).FirstOrDefault().TEXTOS, "COMENTARIOS_REVERSO_PROVISION"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_tipo_solicitud"))).FirstOrDefault().TEXTOS, "TIPO_SOLICITUD"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_status"))).FirstOrDefault().TEXTOS, "ESTATUS_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_concepto_solicitud"))).FirstOrDefault().TEXTOS, "CONCEPTO_SOLICITUD"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_d"))).FirstOrDefault().TEXTOS, "DE_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_a"))).FirstOrDefault().TEXTOS, "A_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_clasificacion"))).FirstOrDefault().TEXTOS, "CLASIFICACION"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_num_cliente"))).FirstOrDefault().TEXTOS, "NUMERO_CLIENTE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_cliente"))).FirstOrDefault().TEXTOS, "CLIENTE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_monto"))).FirstOrDefault().TEXTOS, "MONTO_PROVISION_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_importe"))).FirstOrDefault().TEXTOS, "MONTO_NCOP_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_reverso"))).FirstOrDefault().TEXTOS, "MONTO_REVERSO_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_beneficio"))).FirstOrDefault().TEXTOS, "BENEFICIO_IMPACTO_MRL_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_expense"))).FirstOrDefault().TEXTOS, "EXPENSE_RECOGNITION_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_monto_USD"))).FirstOrDefault().TEXTOS, "MONTO_PROVISION_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_importe_USD"))).FirstOrDefault().TEXTOS, "MONTO_NCOP_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_reverso_USD"))).FirstOrDefault().TEXTOS, "MONTO_REVERSO_USD_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_beneficio_USD"))).FirstOrDefault().TEXTOS, "BENEFICIO_IMPACTO_MRL_USD_STRING"));

            var datos = GenerarMRLTS(Request["selectedcocode"], Int32.Parse(Request["selecteddperiod"]), Int32.Parse(Request["selectedaperiod"]), Request["selectedyear"]);
            string[] subfiltros = Request["selectedsubfilters"].Split(new string[] { "|T,T|" }, StringSplitOptions.None);
            string[] subfiltros_ordenado = subfiltros;
            for (int index = 0; index < subfiltros.Length; index++)
            {
                string temp_subfiltro = subfiltros[index];
                string[] subfiltroArray = temp_subfiltro.Split(new string[] { "|ST|" }, StringSplitOptions.None);
                if (!String.IsNullOrEmpty(subfiltroArray[0]))
                    if (subfiltroArray.Length < 2)
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = string.Empty;
                    else
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = subfiltroArray[1];
            }
            string nombreArchivo = ExcelExport.generarExcel(columnas, datos, "MRLTS", Server.MapPath(ExcelExport.getRuta()), subfiltros_ordenado);
            return File(Server.MapPath(ExcelExport.getRuta() + nombreArchivo), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }

        // FIN REPORTE 5 - MRLTS

        // REPORTE 6 - TRACKING TS
        public ActionResult ReporteTrackingTS()
        {
            int pagina = 1107; // ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = false;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.UsuarioF = db.USUARIOs.ToList();
            ViewBag.Cliente = db.CLIENTEs.Select(x => new { x.KUNNR, x.NAME1 }).ToList();
            ViewBag.Tds = db.TSOLs.ToList();
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
            }
            Session["spras"] = user.SPRAS_ID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReporteTrackingTS(string Form)
        {
            int pagina = 1107; // ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.display = true;
            ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
            ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
            ViewBag.usuario = user;
            ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
            ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
            ViewBag.sociedad = db.SOCIEDADs.ToList();
            ViewBag.periodo = db.PERIODOes.ToList();
            ViewBag.UsuarioF = db.USUARIOs.ToList();
            ViewBag.Cliente = db.CLIENTEs.Select(x => new { x.KUNNR, x.NAME1 }).ToList();
            ViewBag.Tds = db.TSOLs.ToList();
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".svg";
            }
            catch
            {
            }
            Session["spras"] = user.SPRAS_ID;
            //ViewBag.Calendario = cal;
            //ViewBag.cuentagl = db.CUENTAGLs.ToList();
            //ViewBag.cuenta = db.CUENTAs.ToList();
            //ViewBag.Consulta = "";

            ViewBag.selectedcocode = Request["selectcocode"];
            ViewBag.selectedperiod = Request["selectperiod"];
            ViewBag.selectedyear = Request["selectyear"];
            ViewBag.selectedUsuarioF = Request["selectUsuarioF"];
            ViewBag.selectedCliente = Request["selectCliente"];
            ViewBag.selectedTipoSolicitud = Request["selectTipoSolicitud"];

            ViewBag.tabla_reporte = GenerarTrackingTS(Request["selectcocode"], Request["selectperiod"], Request["selectyear"], Request["selectUsuarioF"], Request["selectCliente"], Request["selectTipoSolicitud"]);
            return View();
        }

        public dynamic GenerarTrackingTS(string selectcocode, string selectperiod, string selectyear, string selectUsuarioF, string selectCliente, string selectTipoSolicitud)
        {
            // EVALUAR FILTROS
            string[] comcodessplit = new string[] { };
            string comcode = selectcocode;
            if (!string.IsNullOrEmpty(comcode))
            {
                comcodessplit = comcode.Split(',');
            }
            int period = Int32.Parse(selectperiod);
            string year = selectyear;
            string usuarioF = selectUsuarioF;
            string clienteF = selectCliente;
            string solicitudF = selectTipoSolicitud;

            var queryP = (from d in db.DOCUMENTOes
                          join f in db.FLUJOes on d.NUM_DOC equals f.NUM_DOC
                          join p in db.PAIS on d.PAIS_ID equals p.LAND
                          join c in db.CLIENTEs on new { d.VKORG, d.VTWEG, d.SPART, d.PAYER_ID } equals new { c.VKORG, c.VTWEG, c.SPART, PAYER_ID = c.KUNNR }
                          join ts in db.TSOLs on d.TSOL_ID equals ts.ID
                          join us in db.USUARIOs on f.USUARIOA_ID equals us.ID
                          join tst in db.TSOLTs on new { TSOL_ID = ts.ID, us.SPRAS_ID } equals new { tst.TSOL_ID, tst.SPRAS_ID }
                          join pt in db.PUESTOTs on new { us.SPRAS_ID, PUESTO_ID = (int)(us.PUESTO_ID) } equals new { pt.SPRAS_ID, pt.PUESTO_ID }
                          join wfp in db.WORKFPs on new { f.WORKF_ID, f.WF_VERSION, f.WF_POS } equals new { WORKF_ID = wfp.ID, WF_VERSION = wfp.VERSION, WF_POS = wfp.POS }
                          join ac in db.ACCIONs on wfp.ACCION_ID equals ac.ID
                          orderby d.FECHAC descending, d.NUM_DOC ascending, f.WF_POS ascending, f.POS ascending
                          where d.PERIODO == period
                              && d.EJERCICIO == year
                              && (!string.IsNullOrEmpty(usuarioF) ? d.USUARIOC_ID == usuarioF : true)
                              && (!string.IsNullOrEmpty(solicitudF) ? d.TSOL_ID == solicitudF : true)
                              && (!string.IsNullOrEmpty(clienteF) ? d.PAYER_ID == clienteF : true)
                              && (!string.IsNullOrEmpty(comcode) ? comcodessplit.Contains(d.SOCIEDAD_ID) : true)
                          select new TrackingTS
                          {
                              WF_POS = f.WF_POS,
                              POS = f.POS,
                              NUMERO_SOLICITUD = d.NUM_DOC,
                              FECHA_SOLICITUD = d.FECHAC,
                              CO_CODE = d.SOCIEDAD_ID,
                              PAIS = p.LANDX,
                              NUMERO_CLIENTE = d.PAYER_ID,
                              CLIENTE = c.NAME1,
                              TIPO_SOLICITUD = tst.TXT50,
                              ESTATUS = f.ESTATUS,
                              ESTATUS_C = d.ESTATUS_C,
                              ESTATUS_SAP = d.ESTATUS_SAP,
                              ESTATUS_WF = d.ESTATUS_WF,
                              TIPO = ac.TIPO,
                              PADRE = ts.PADRE,
                              FECHA = (DateTime)f.FECHAC,
                              PERIODO = (Int32)d.PERIODO,
                              ANIO = d.EJERCICIO,
                              USUARIO = d.USUARIOC_ID,
                              USUARIO_ACCION = f.USUARIOA_ID,
                              COMENTARIO = f.COMENTARIO,
                              ROL = pt.TXT50
                              //, STATUS_STRING = statusToString(f.ESTATUS, d.ESTATUS_C, d.ESTATUS_SAP, d.ESTATUS_WF, ac.TIPO, ts.PADRE)
                          }).ToList();


            foreach (TrackingTS renglon in queryP)
            {
                renglon.f = db.FLUJOes.Where(a => (a.NUM_DOC.Equals(renglon.NUMERO_SOLICITUD) && a.POS.Equals(renglon.POS))).OrderByDescending(x => x.FECHAC).FirstOrDefault();
            }

            return queryP.GroupBy(registro => new { registro.NUMERO_SOLICITUD, registro.POS })
                .Select(grupo => new
                {
                    trackings = grupo.OrderBy(x => x.FECHA)
                })
                .Select(grupo_ordenado => new
                {
                    tracking = calcularValoresGrupo(grupo_ordenado.trackings)
                }).ToList();
        }

        [HttpPost]
        public FileResult ExportReporteTrackingTS()
        {
            int pagina = 1107; // ID EN BASE DE DATOS
            var user = db.USUARIOs.Where(a => a.ID.Equals(User.Identity.Name)).FirstOrDefault();

            List<ExcelExportColumn> columnas = new List<ExcelExportColumn>();
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_code"))).FirstOrDefault().TEXTOS, "CO_CODE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_pais"))).FirstOrDefault().TEXTOS, "PAIS"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_num_solicitud"))).FirstOrDefault().TEXTOS, "NUMERO_SOLICITUD", Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "Solicitudes/Details/", true));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_num_cliente"))).FirstOrDefault().TEXTOS, "NUMERO_CLIENTE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_cliente"))).FirstOrDefault().TEXTOS, "CLIENTE"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_tipo_solicitud"))).FirstOrDefault().TEXTOS, "TIPO_SOLICITUD"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_status"))).FirstOrDefault().TEXTOS, "STATUS_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_fecha"))).FirstOrDefault().TEXTOS, "FECHA_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_hora"))).FirstOrDefault().TEXTOS, "HORA_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_tiempo"))).FirstOrDefault().TEXTOS, "TIEMPO_TRANSCURRIDO_STRING"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_semana"))).FirstOrDefault().TEXTOS, "SEMANA"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_periodo"))).FirstOrDefault().TEXTOS, "PERIODO"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_ano"))).FirstOrDefault().TEXTOS, "ANIO"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_usuario"))).FirstOrDefault().TEXTOS, "USUARIO"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_rol"))).FirstOrDefault().TEXTOS, "ROL"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_usuario_accion"))).FirstOrDefault().TEXTOS, "USUARIO_ACCION"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_comentario"))).FirstOrDefault().TEXTOS, "COMENTARIO"));
            columnas.Add(new ExcelExportColumn(db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) && a.SPRAS_ID.Equals(user.SPRAS_ID) && a.CAMPO_ID.Equals("head_correcciones"))).FirstOrDefault().TEXTOS, "NUMERO_CORRECCIONES_STRING"));

            var datos = GenerarTrackingTS(Request["selectedcocode"], Request["selectedperiod"], Request["selectedyear"], Request["selectedUsuarioF"], Request["selectedCliente"], Request["selectedTipoSolicitud"]);
            List<TrackingTS> reporte = new List<TrackingTS>();
            foreach(object dato in datos)
            {
                reporte.Add((TrackingTS)(dato.GetType().GetProperty("tracking").GetValue(dato, null)));
            }
            string[] subfiltros = Request["selectedsubfilters"].Split(new string[] { "|T,T|" }, StringSplitOptions.None);
            string[] subfiltros_ordenado = subfiltros;
            for (int index = 0; index < subfiltros.Length; index++)
            {
                string temp_subfiltro = subfiltros[index];
                string[] subfiltroArray = temp_subfiltro.Split(new string[] { "|ST|" }, StringSplitOptions.None);
                if (!String.IsNullOrEmpty(subfiltroArray[0]))
                    if (subfiltroArray.Length < 2)
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = string.Empty;
                    else
                        subfiltros_ordenado[Convert.ToInt32(subfiltroArray[0])] = subfiltroArray[1];
            }
            string nombreArchivo = ExcelExport.generarExcel(columnas, reporte, "TrackingTS", Server.MapPath(ExcelExport.getRuta()), subfiltros_ordenado);
            return File(Server.MapPath(ExcelExport.getRuta() + nombreArchivo), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }

        private TrackingTS calcularValoresGrupo(IOrderedEnumerable<TrackingTS> grupo)
        {
            TrackingTS ultimo = grupo.Last();
            TrackingTS primero = grupo.First();
            ultimo.NUMERO_CORRECCIONES = grupo.Where(c => c.ESTATUS.Equals("R")).Count();
            ultimo.TIEMPO_TRANSCURRIDO = (ultimo.FECHA - primero.FECHA).TotalHours; // ToDo: Restar sábados, domingos y días feriados
            ultimo.SEMANA = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(primero.FECHA, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return ultimo;
        }
        // FIN REPORTE 6 - TRACKING TS

    }
}