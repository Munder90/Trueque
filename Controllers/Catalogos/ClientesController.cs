using ClosedXML.Excel;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Services;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]  
    public class ClientesController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();
        readonly UsuarioLogin usuValidateLogin = new UsuarioLogin();

        // GET: Clientes 
        [LoginActive]
        public ActionResult Index()
        {
            int pagina_id = 631; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            ViewBag.pais = Session["pais"]!=null? Session["pais"].ToString() + ".png" :null;


            ClienteViewModel viewModel = new ClienteViewModel
            {
                pageSizes = FnCommon.ObtenerCmbPageSize()
            };
            ObtenerListado(ref viewModel);

            return View(viewModel);
        }

        public ActionResult List(string colOrden, string ordenActual, int? numRegistros = 10, int? pagina = 1, string buscar = "")
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            int pagina_id = 631; //ID EN BASE DE DATOS
            ClienteViewModel viewModel = new ClienteViewModel();
            ObtenerListado(ref viewModel,colOrden,ordenActual,numRegistros,pagina,buscar);
            FnCommon.ObtenerTextos(db,pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            return View(viewModel);
        }

        void ObtenerListado(ref ClienteViewModel viewModel, string colOrden="", string ordenActual="", int? numRegistros = 10, int? pagina = 1, string buscar = "")
        {
            int pageIndex = pagina.Value;
            List<CLIENTE> clientes = db.CLIENTEs.Include(c => c.PAI).Include(c => c.TCLIENTE).ToList();
            viewModel.ordenActual = (string.IsNullOrEmpty(ordenActual) || !colOrden.Equals(ordenActual) ? colOrden:"");
            viewModel.numRegistros = numRegistros.Value;
            viewModel.buscar = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                clientes = clientes.Where(x =>
                String.Concat(x.KUNNR, x.NAME1, (x.SUBREGION ?? ""), x.LAND, x.PAI.LANDX, x.PARVW, x.PAYER, (x.CANAL ?? ""))
                .ToLower().Contains(buscar.ToLower()))
                .ToList();
            }
            switch (colOrden)
            {
                case "KUNNR":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.KUNNR).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.KUNNR).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "NAME1":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.NAME1).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.NAME1).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "SUBREGION":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.SUBREGION).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.SUBREGION).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "LAND":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.LAND).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.LAND).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "PARVW":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.PARVW).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.PARVW).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "PAYER":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.PAYER).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.PAYER).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "CANAL":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.CANAL).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.CANAL).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "ACTIVO":
                    if (colOrden.Equals(ordenActual))
                        viewModel.clientes = clientes.OrderByDescending(m => m.ACTIVO).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.clientes = clientes.OrderBy(m => m.ACTIVO).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                default:
                    viewModel.clientes = clientes.OrderBy(m => m.KUNNR).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
            }
        }

        public ActionResult VerFlujo(string vko, string vtw, string spa, string kun, int version)
        {
            if (!usuValidateLogin.validaUsuario(User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                });
            }
            int pagina_id = 604; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            var flujo = db.CLIENTEFs.Find(vko, vtw, spa, kun, version);
            if (flujo.USUARIO1_ID != null)
            {
                var usu1 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO1_ID).SingleOrDefault();
                ViewBag.Usuario1 = usu1 != null ? flujo.USUARIO1_ID + " - " + usu1.NOMBRE + " " + usu1.APELLIDO_P : "";
            }
            if (flujo.USUARIO2_ID != null)
            {
                var usu2 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO2_ID).SingleOrDefault();
                ViewBag.Usuario2 = usu2 != null ? flujo.USUARIO2_ID + " - " + usu2.NOMBRE + " " + usu2.APELLIDO_P : "";
            }
            if (flujo.USUARIO3_ID != null)
            {
                var usu3 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO3_ID).SingleOrDefault();
                ViewBag.Usuario3 = usu3 != null ? flujo.USUARIO3_ID + " - " + usu3.NOMBRE + " " + usu3.APELLIDO_P : "";
            }
            if (flujo.USUARIO4_ID != null)
            {
                var usu4 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO4_ID).SingleOrDefault();
                ViewBag.Usuario4 = usu4 != null ? flujo.USUARIO4_ID + " - " + usu4.NOMBRE + " " + usu4.APELLIDO_P : "";
            }
            if (flujo.USUARIO5_ID != null)
            {
                var usu5 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO5_ID).SingleOrDefault();
                ViewBag.Usuario5 = usu5 != null ? flujo.USUARIO5_ID + " - " + usu5.NOMBRE + " " + usu5.APELLIDO_P : "";
            }
            if (flujo.USUARIO6_ID != null)
            {
                var usu6 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO6_ID).SingleOrDefault();
                ViewBag.Usuario6 = usu6 != null ? flujo.USUARIO6_ID+" - "+ usu6.NOMBRE + " " + usu6.APELLIDO_P : "";
            }
            if (flujo.USUARIO7_ID != null)
            {
                var usu7 = db.USUARIOs.Where(t => t.ID == flujo.USUARIO7_ID).SingleOrDefault();
                ViewBag.Usuario7 = usu7 != null ? flujo.USUARIO7_ID + " - " + usu7.NOMBRE + " " + usu7.APELLIDO_P : "";
            }
            return View(flujo);
        }

        // GET: Clientes/Details/5
        [LoginActive]
        public ActionResult Details(string vko, string vtw, string spa, string kun)
        {
            int pagina_id = 632; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".png";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            if (vko == null || vtw == null || spa == null || kun == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CLIENTE cLIENTE = db.CLIENTEs.Find(vko, vtw, spa, kun);
            if (cLIENTE == null)
            {
                return HttpNotFound();
            }
            if (!String.IsNullOrEmpty(cLIENTE.CANAL))
            {
                CANAL canaldsc = db.CANALs.Where(t => t.CANAL1 == cLIENTE.CANAL).SingleOrDefault();
                if (canaldsc != null)
                    ViewBag.CanalDsc = canaldsc.CANAL1 + "-" + canaldsc.CDESCRIPCION;
            }
            if (!String.IsNullOrEmpty(cLIENTE.PROVEEDOR_ID))
            {
                PROVEEDOR proveedordsc = db.PROVEEDORs.Where(p => p.ID == cLIENTE.PROVEEDOR_ID).SingleOrDefault();
                if (proveedordsc != null)
                    ViewBag.ProveedorDsc = proveedordsc.ID + "-" + proveedordsc.NOMBRE;
            }
            else
            {
                ViewBag.ProveedorDsc = "";
            }
            return View(cLIENTE);
        }

        // GET: Clientes/Edit/5
        [LoginActive]
        public ActionResult Edit(string vko, string vtw, string spa, string kun)
        {
            int pagina_id = 635; //ID EN BASE DE DATOS PARA EL TITULO
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            
            try
            {
                string p = Session["pais"].ToString();
                ViewBag.pais = p + ".png";
            }
            catch
            {
                //return RedirectToAction("Pais", "Home");
            }

            if (vko == null || vtw == null || spa == null || kun == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CLIENTE cLIENTE = db.CLIENTEs.Find(vko, vtw, spa, kun);
            if (cLIENTE == null)
            {
                return HttpNotFound();
            }
              return View(cLIENTE);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoginActive]
        public ActionResult Edit([Bind(Include = "VKORG,VTWEG,SPART,KUNNR,NAME1,STCD1,STCD2,LAND,REGION,SUBREGION,REGIO,ORT01,STRAS_GP,PSTLZ,CONTAC,CONT_EMAIL,PARVW,PAYER,GRUPO,SPRAS,ACTIVO,BDESCRIPCION,BANNER, PROVEEDOR_ID,CANAL,BZIRK,KONDA,VKGRP,VKBUR,BANNERG")] CLIENTE cLIENTE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cLIENTE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LAND = new SelectList(db.PAIS, "LAND", "SPRAS", cLIENTE.LAND);
            ViewBag.PARVW = new SelectList(db.TCLIENTEs, "ID", "ID", cLIENTE.PARVW);
            return View(cLIENTE);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [LoginActive]
        public ActionResult Carga()
        {
            int pagina = 631; //ID EN BASE DE DATOS PARA EL TITULO
            string u = User.Identity.Name;
            FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);

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
            return View();
        }

        [HttpPost]
        [LoginActive]
        public ActionResult Carga(IEnumerable<HttpPostedFileBase> files)
        {
            string uz = User.Identity.Name;
            var userz = db.USUARIOs.Where(a => a.ID.Equals(uz)).FirstOrDefault();
            if (!usuValidateLogin.validaUsuario(userz.ID))
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoadExcel()
        {
            string uz = User.Identity.Name;
            var userz = db.USUARIOs.Where(a => a.ID.Equals(uz)).FirstOrDefault();
            if (!usuValidateLogin.validaUsuario(userz.ID))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                });
            }
            List<Clientes> clientes = new List<Clientes>();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                DataSet result = reader.AsDataSet();
                DataTable dt = result.Tables[0];

                if (dt.Columns.Count != 19 || dt.Rows.Count>2000)
                {
                    return Json("NO VALIDO", JsonRequestBehavior.AllowGet);
                }
                DataTable dtClientes = new DataTable("Clientes");

                dtClientes.Columns.Add("BUKRS", typeof(String)); //--0
                dtClientes.Columns.Add("LAND", typeof(String));  //--1
                dtClientes.Columns.Add("KUNNR", typeof(String)); //--2
                dtClientes.Columns.Add("VKORG", typeof(String)); 
                dtClientes.Columns.Add("VTWEG", typeof(String)); 
                dtClientes.Columns.Add("SPART", typeof(String)); 
                dtClientes.Columns.Add("CLIENTE_N", typeof(String)); //--3
                dtClientes.Columns.Add("ID_US0", typeof(String)); //--4
                dtClientes.Columns.Add("ID_US1", typeof(String)); //--5
                dtClientes.Columns.Add("ID_US2", typeof(String)); //--6
                dtClientes.Columns.Add("ID_US3", typeof(String)); //--7
                dtClientes.Columns.Add("ID_US4", typeof(String)); //--8
                dtClientes.Columns.Add("ID_US5", typeof(String)); //--9
                dtClientes.Columns.Add("ID_US6", typeof(String)); //--10
                dtClientes.Columns.Add("ID_US7", typeof(String)); //--11
                dtClientes.Columns.Add("ID_PROVEEDOR", typeof(String)); //--12
                dtClientes.Columns.Add("BANNER", typeof(String)); //--13
                dtClientes.Columns.Add("BANNERG", typeof(String)); //--14
                dtClientes.Columns.Add("CANAL", typeof(String)); //--15
                dtClientes.Columns.Add("EXPORTACION", typeof(String)); //--16
                dtClientes.Columns.Add("CONTACTO", typeof(String)); //--17
                dtClientes.Columns.Add("CONTACTOE", typeof(String)); //--18
                dtClientes.Columns.Add("MESS", typeof(String)); 

                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (i > 0)
                    {
                       string kunnr= !string.IsNullOrEmpty(row.ItemArray[2].ToString()) ? Completa(row.ItemArray[2].ToString().Trim(), 10) : null;
                        string provedor_id= !string.IsNullOrEmpty(row.ItemArray[12].ToString()) ? Completa( row.ItemArray[12].ToString().Trim() , 10) : null;
                        string banner = !string.IsNullOrEmpty(row.ItemArray[13].ToString()) ? Completa( row.ItemArray[13].ToString().Trim() , 10) : null;
                        string bannerg = !string.IsNullOrEmpty(row.ItemArray[14].ToString()) ? Completa( row.ItemArray[14].ToString().Trim(), 10) : null;

                        dtClientes.Rows.Add(new object[] {
                        row.ItemArray[0], row.ItemArray[1], kunnr,"","","", row.ItemArray[3],
                        row.ItemArray[4], row.ItemArray[5],row.ItemArray[6], row.ItemArray[7], row.ItemArray[8],row.ItemArray[9], row.ItemArray[10], row.ItemArray[11],
                        provedor_id, banner, bannerg,row.ItemArray[15], row.ItemArray[16], row.ItemArray[17], row.ItemArray[18],""});
                    }
                    i++;
                }

                SqlParameter param = new SqlParameter("@CLIENTES", dtClientes)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.ClientesTableType"
                };
                db.Database.CommandTimeout = 6000;
                clientes = db.Database.SqlQuery<Clientes>("CSP_MASIVA_CLIENTES @ACCION,@CLIENTES",
                new SqlParameter("@ACCION", TATConstantes.ACCION_MASIVA_CLIENTES_PROCESAR),param).ToList();

                reader.Close();
            }
            
            return View("CargaList", clientes);
        }

        [HttpPost]
        public JsonResult Agregar()
        {
            string uz = User.Identity.Name;
            var userz = db.USUARIOs.Where(a => a.ID.Equals(uz)).FirstOrDefault();
            if (!usuValidateLogin.validaUsuario(userz.ID))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                });
            }

            var coc = Request["coc"].Split(',');
            var pai = Request["pai"].Split(',');
            var cli = Request["cli"].Split(',');
            var noc = Request["noc"].Split(',');
            var ni0 = Request["ni0"].Split(',');
            var ni1 = Request["ni1"].Split(',');
            var ni2 = Request["ni2"].Split(',');
            var ni3 = Request["ni3"].Split(',');
            var ni4 = Request["ni4"].Split(',');
            var ni5 = Request["ni5"].Split(',');
            var ni6 = Request["ni6"].Split(',');
            var ni7 = Request["ni7"].Split(',');
            var ven = Request["ven"].Split(',');
            var ban = Request["ban"].Split(',');
            var baa = Request["baa"].Split(',');
            var can = Request["can"].Split(',');
            var exp = Request["exp"].Split(',');
            var con = Request["con"].Split(',');
            var eco = Request["eco"].Split(',');
            var mes = Request["mes"].Split(',');

            var rows = coc.Length;
            string[,] tablas = new string[rows, 20];

            for (int i = 0; i < rows; i++)
            {
                tablas[i, 0] = coc[i];
                tablas[i, 1] = pai[i];
                tablas[i, 2] = cli[i];
                if (noc[i] == "")
                { tablas[i, 3] = null; }
                else
                { tablas[i, 3] = noc[i]; }
                if (ni0[i] == "")
                { tablas[i, 4] = null; }
                else
                { tablas[i, 4] = ni0[i]; }
                tablas[i, 5] = ni1[i];
                if (ni2[i] == "")
                { tablas[i, 6] = null; }
                else
                { tablas[i, 6] = ni2[i]; }
                if (ni3[i] == "")
                { tablas[i, 7] = null; }
                else
                { tablas[i, 7] = ni3[i]; }
                if (ni3[i] == "")
                { tablas[i, 8] = null; }
                else
                { tablas[i, 8] = ni4[i]; }
                if (ni5[i] == "")
                { tablas[i, 9] = null; }
                else
                { tablas[i, 9] = ni5[i]; }
                tablas[i, 10] = ni6[i];
                if (ni7[i] == "")
                { tablas[i, 11] = null; }
                else
                { tablas[i, 11] = ni7[i]; }
                if (ven[i] == "")
                { tablas[i, 12] = null; }
                else
                { tablas[i, 12] = ven[i]; }
                if (ban[i] == "")
                { tablas[i, 13] = null; }
                else
                { tablas[i, 13] = ban[i]; }
                if (baa[i] == "")
                { tablas[i, 14] = null; }
                else
                { tablas[i, 14] = baa[i]; }
                if (can[i] == "")
                { tablas[i, 15] = null; }
                else
                { tablas[i, 15] = can[i]; }
                if (exp[i] == "")
                { tablas[i, 16] = null; }
                else
                { tablas[i, 16] = exp[i]; }
                if (con[i] == "")
                { tablas[i, 17] = null; }
                else
                { tablas[i, 17] = con[i]; }
                if (eco[i] == "")
                { tablas[i, 18] = null; }
                else
                { tablas[i, 18] = eco[i]; }
                tablas[i, 19] = mes[i];
            }

            List<DET_AGENTE1>  ld = ObjAList2(tablas, rows);

            DataTable dtClientes = new DataTable("Clientes");

            dtClientes.Columns.Add("BUKRS", typeof(String)); //--0
            dtClientes.Columns.Add("LAND", typeof(String));  //--1
            dtClientes.Columns.Add("KUNNR", typeof(String)); //--2
            dtClientes.Columns.Add("VKORG", typeof(String));
            dtClientes.Columns.Add("VTWEG", typeof(String));
            dtClientes.Columns.Add("SPART", typeof(String));
            dtClientes.Columns.Add("CLIENTE_N", typeof(String)); //--3
            dtClientes.Columns.Add("ID_US0", typeof(String)); //--4
            dtClientes.Columns.Add("ID_US1", typeof(String)); //--5
            dtClientes.Columns.Add("ID_US2", typeof(String)); //--6
            dtClientes.Columns.Add("ID_US3", typeof(String)); //--7
            dtClientes.Columns.Add("ID_US4", typeof(String)); //--8
            dtClientes.Columns.Add("ID_US5", typeof(String)); //--9
            dtClientes.Columns.Add("ID_US6", typeof(String)); //--10
            dtClientes.Columns.Add("ID_US7", typeof(String)); //--11
            dtClientes.Columns.Add("ID_PROVEEDOR", typeof(String)); //--12
            dtClientes.Columns.Add("BANNER", typeof(String)); //--13
            dtClientes.Columns.Add("BANNERG", typeof(String)); //--14
            dtClientes.Columns.Add("CANAL", typeof(String)); //--15
            dtClientes.Columns.Add("EXPORTACION", typeof(String)); //--16
            dtClientes.Columns.Add("CONTACTO", typeof(String)); //--17
            dtClientes.Columns.Add("CONTACTOE", typeof(String)); //--18
            dtClientes.Columns.Add("MESS", typeof(String));

            foreach (DET_AGENTE1 row in ld) {
                
                dtClientes.Rows.Add(new object[] {
                        row.BUKRS, row.LAND, row.KUNNR,row.VKORG,row.VTWEG,row.SPART, row.CLIENTE_N,
                        row.ID_US0, row.ID_US1,row.ID_US2,row.ID_US3, row.ID_US4,row.ID_US5, row.ID_US6,row.ID_US7,
                        row.ID_PROVEEDOR, row.BANNER, row.BANNERG,row.CANAL, row.EXPORTACION, row.CONTACTO, row.CONTACTOE,""});
            }

            SqlParameter param = new SqlParameter("@CLIENTES", dtClientes)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.ClientesTableType"
            };
            db.Database.ExecuteSqlCommand("CSP_MASIVA_CLIENTES @ACCION,@CLIENTES",
            new SqlParameter("@ACCION", TATConstantes.ACCION_MASIVA_CLIENTES_GUARDAR), param);
            
            JsonResult jl = Json(ld.Count, JsonRequestBehavior.AllowGet);
            return jl;
        }

        [HttpPost]
        public ActionResult Comprobar()
        {
            string uz = User.Identity.Name;
            var userz = db.USUARIOs.Where(a => a.ID.Equals(uz)).FirstOrDefault();
            if (!usuValidateLogin.validaUsuario(userz.ID))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                });
            }
           
            
            DataTable dtClientes = new DataTable("Clientes");

            dtClientes.Columns.Add("BUKRS", typeof(String)); //--0
            dtClientes.Columns.Add("LAND", typeof(String));  //--1
            dtClientes.Columns.Add("KUNNR", typeof(String)); //--2
            dtClientes.Columns.Add("VKORG", typeof(String));
            dtClientes.Columns.Add("VTWEG", typeof(String));
            dtClientes.Columns.Add("SPART", typeof(String));
            dtClientes.Columns.Add("CLIENTE_N", typeof(String)); //--3
            dtClientes.Columns.Add("ID_US0", typeof(String)); //--4
            dtClientes.Columns.Add("ID_US1", typeof(String)); //--5
            dtClientes.Columns.Add("ID_US2", typeof(String)); //--6
            dtClientes.Columns.Add("ID_US3", typeof(String)); //--7
            dtClientes.Columns.Add("ID_US4", typeof(String)); //--8
            dtClientes.Columns.Add("ID_US5", typeof(String)); //--9
            dtClientes.Columns.Add("ID_US6", typeof(String)); //--10
            dtClientes.Columns.Add("ID_US7", typeof(String)); //--11
            dtClientes.Columns.Add("ID_PROVEEDOR", typeof(String)); //--12
            dtClientes.Columns.Add("BANNER", typeof(String)); //--13
            dtClientes.Columns.Add("BANNERG", typeof(String)); //--14
            dtClientes.Columns.Add("CANAL", typeof(String)); //--15
            dtClientes.Columns.Add("EXPORTACION", typeof(String)); //--16
            dtClientes.Columns.Add("CONTACTO", typeof(String)); //--17
            dtClientes.Columns.Add("CONTACTOE", typeof(String)); //--18
            dtClientes.Columns.Add("MESS", typeof(String));
            
            string coc = Request["coc"];
            string pai = Request["pai"];
            string cli = Request["cli"];
            string noc = Request["noc"];
            string ni0 = Request["ni0"];
            string ni1 = Request["ni1"];
            string ni2 = Request["ni2"];
            string ni3 = Request["ni3"];
            string ni4 = Request["ni4"];
            string ni5 = Request["ni5"];
            string ni6 = Request["ni6"];
            string ni7 = Request["ni7"];
            string ven = Request["ven"];
            string ban = Request["ban"];
            string baa = Request["baa"];
            string can = Request["can"];
            string exp = Request["exp"];
            string con = Request["con"];
            string eco = Request["eco"];

            string kunnr = !string.IsNullOrEmpty(cli) ? Completa(cli, 10) : null;
                    string provedor_id = !string.IsNullOrEmpty(ven) ? Completa(ven, 10) : null;
                    string banner = !string.IsNullOrEmpty(ban) ? Completa(ban, 10) : null;
                    string bannerg = !string.IsNullOrEmpty(baa) ? Completa(baa, 10) : null;

                    dtClientes.Rows.Add(new object[] {
                        coc, pai, kunnr,"","","", noc,
                        ni0, ni1,ni2,ni3, ni4,ni5, ni6,ni7,
                        provedor_id, banner, bannerg,can, exp, con, eco,""});
              
            SqlParameter param = new SqlParameter("@CLIENTES", dtClientes)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.ClientesTableType"
            };
            List<Clientes> clientes = db.Database.SqlQuery<Clientes>("CSP_MASIVA_CLIENTES @ACCION,@CLIENTES",
            new SqlParameter("@ACCION", TATConstantes.ACCION_MASIVA_CLIENTES_PROCESAR),param).ToList();

            return View("CargaList", clientes);


        }

        [HttpPost]
        public ActionResult Actualizar()
        {
            string uz = User.Identity.Name;
            var userz = db.USUARIOs.Where(a => a.ID.Equals(uz)).FirstOrDefault();
            if (!usuValidateLogin.validaUsuario(userz.ID))
            {
                FormsAuthentication.SignOut();
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home"),
                    isRedirect = true
                });
            }
            List<Clientes> cc = new List<Clientes>();
            
            var cli = Request["cli"];

            if (!string.IsNullOrEmpty(cli))
            {
                Clientes cl = new Clientes();
                cli = Completa(cli, 10);
                CLIENTE k = db.CLIENTEs.Where(x => x.KUNNR.Equals(cli) && x.ACTIVO).FirstOrDefault();
                cl.KUNNRX = true;
                cl.BUKRS = "";
                cl.LAND = "";
                cl.KUNNR = "";
                cl.CLIENTE_N = "";
                cl.ID_US0 = "";
                cl.ID_US1 = "";
                cl.ID_US2 = "";
                cl.ID_US3 = "";
                cl.ID_US4 = "";
                cl.ID_US5 = "";
                cl.ID_US6 = "";
                cl.ID_US7 = "";
                cl.ID_PROVEEDOR = "";
                cl.BANNER = "";
                cl.BANNERG = "";
                cl.CANAL = "";
                cl.EXPORTACION = "";
                cl.CONTACTO = "";
                cl.CONTACTOE = "";
                cl.MESS = "";

                if (k == null)
                    cl.KUNNRX = false;
                else
                {
                    CLIENTEF clienteF = db.CLIENTEFs.FirstOrDefault(x => x.KUNNR.Equals(cli) && x.ACTIVO);
                    if (k.LAND != null)
                        cl.LAND = k.LAND;
                    string bukrs = (from x in db.PAIS where x.LAND.Equals(k.LAND) && x.ACTIVO select x.SOCIEDAD_ID).FirstOrDefault();
                    if (bukrs != null)
                        cl.BUKRS = bukrs;
                    cl.KUNNR = cli;
                    if (k.NAME1 != null)
                        cl.CLIENTE_N = k.NAME1;
                    if (clienteF!=null) {
                        if (clienteF.USUARIO0_ID != null)
                            cl.ID_US0 = clienteF.USUARIO0_ID;
                        if (clienteF.USUARIO1_ID != null)
                            cl.ID_US1 = clienteF.USUARIO1_ID;
                        if (clienteF.USUARIO2_ID != null)
                            cl.ID_US2 = clienteF.USUARIO2_ID;
                        if (clienteF.USUARIO3_ID != null)
                            cl.ID_US3 = clienteF.USUARIO3_ID;
                        if (clienteF.USUARIO4_ID != null)
                            cl.ID_US4 = clienteF.USUARIO4_ID;
                        if (clienteF.USUARIO5_ID != null)
                            cl.ID_US5 = clienteF.USUARIO5_ID;
                        if (clienteF.USUARIO6_ID != null)
                            cl.ID_US6 = clienteF.USUARIO6_ID;
                        if (clienteF.USUARIO7_ID != null)
                            cl.ID_US7 = clienteF.USUARIO7_ID;
                    }
                    if (k.PROVEEDOR_ID != null)
                        cl.ID_PROVEEDOR = k.PROVEEDOR_ID;
                    if (k.BANNER != null)
                        cl.BANNER = k.BANNER;
                    if (k.BANNERG != null)
                        cl.BANNERG = k.BANNERG;
                    if (k.CANAL != null)
                        cl.CANAL = k.CANAL;
                    if (k.EXPORTACION != null)
                        cl.EXPORTACION = k.EXPORTACION;
                    if (k.CONTAC != null)
                        cl.CONTACTO = k.CONTAC;
                    if (k.CONT_EMAIL != null)
                        cl.CONTACTOE = k.CONT_EMAIL;
                }
                if (!cl.KUNNRX)
                {
                    cl.KUNNR = cli + "?";
                    cl.MESS = "El cliente no existe.";
                }
                cc.Add(cl);
            }

            return View("CargaList", cc);
        }

        [HttpPost]
        public FileResult Descargar()
        {
            string uz = User.Identity.Name;
            var userz = db.USUARIOs.Where(a => a.ID.Equals(uz)).FirstOrDefault();
            if (!usuValidateLogin.validaUsuario(userz.ID))
            {
                FormsAuthentication.SignOut();
                return null;
            }
            var cLiente = db.CLIENTEs.ToList();
            generarExcelHome(cLiente, Server.MapPath("~/pdfTemp/"));
            return File(Server.MapPath("~/pdfTemp/Clientes_" + DateTime.Now.ToShortDateString() + ".xlsx"), "application /vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clientes_" + DateTime.Now.ToShortDateString() + ".xlsx");
        }

        public void generarExcelHome(List<CLIENTE> lst, string ruta)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            try
            {
                worksheet.Cell("A1").Value = new[] { new { BANNER = "Id Cliente" }, };
                worksheet.Cell("B1").Value = new[] { new { BANNER = "Nombre" }, };
                worksheet.Cell("C1").Value = new[] { new { BANNER = "Región" }, };
                worksheet.Cell("D1").Value = new[] { new { BANNER = "País" }, };
                worksheet.Cell("E1").Value = new[] { new { BANNER = "Tipo de Cliente" }, };
                worksheet.Cell("F1").Value = new[] { new { BANNER = "Payer" }, };
                worksheet.Cell("G1").Value = new[] { new { BANNER = "Canal" }, };
                worksheet.Cell("H1").Value = new[] { new { BANNER = "Estatus" }, };
                worksheet.Cell("I1").Value = new[] { new { BANNER = "Contacto" }, };
                worksheet.Cell("J1").Value = new[] { new { BANNER = "Email" }, };

                for (int i = 2; i <= (lst.Count + 1); i++)
                {
                    var NOMBRE = "";
                    var EMAIL = "";
                    var pais = lst[i - 2].LAND;
                    var kunnr = lst[i - 2].KUNNR;
                    var pais2 = db.PAIS.Where(X => X.LAND.Equals(pais)).Select(x => x.LANDX).FirstOrDefault();
                    var contacto = db.CONTACTOCs.Where(x => x.KUNNR == kunnr && x.ACTIVO == true).ToArray();
                    if (contacto != null)
                    {
                        for (int j = 0; j < contacto.Length; j++)
                        {
                            if (contacto[j].DEFECTO == true)
                            {
                                NOMBRE = NOMBRE + contacto[j].NOMBRE + "*/";
                                EMAIL = EMAIL + contacto[j].EMAIL + "*/";
                            }
                            else
                            {
                                NOMBRE = NOMBRE + contacto[j].NOMBRE + "/";
                                EMAIL = EMAIL + contacto[j].EMAIL + "/";
                            }
                        }
                    }
                    worksheet.Cell("A" + i).Value = new[] { new { BANNER = lst[i - 2].KUNNR.TrimStart('0') }, };
                    worksheet.Cell("B" + i).Value = new[] { new { BANNER = lst[i - 2].NAME1 }, };
                    worksheet.Cell("C" + i).Value = new[] { new { BANNER = lst[i - 2].SUBREGION }, };
                    worksheet.Cell("D" + i).Value = new[] { new { BANNER = pais2 }, };
                    worksheet.Cell("E" + i).Value = new[] { new { BANNER = lst[i - 2].PARVW }, };
                    worksheet.Cell("F" + i).Value = new[] { new { BANNER = lst[i - 2].PAYER.TrimStart('0') }, };
                    worksheet.Cell("G" + i).Value = new[] { new { BANNER = lst[i - 2].CANAL }, };
                    worksheet.Cell("H" + i).Value = new[] { new { BANNER = lst[i - 2].ACTIVO? "Activo":"Inactivo" }, };
                    worksheet.Cell("I" + i).Value = new[] { new { BANNER = NOMBRE.TrimEnd('/') }, };
                    worksheet.Cell("J" + i).Value = new[] { new { BANNER = EMAIL.TrimEnd('/') }, };
                }
                var rt = ruta + @"\Clientes_" + DateTime.Now.ToShortDateString() + ".xlsx";
                workbook.SaveAs(rt);
            }
            catch (Exception e)
            {
                var ex = e.ToString();
            }
        }

        private string Completa(string s, int longitud)
        {
            string cadena = "";
            try
            {
                long a = Int64.Parse(s);
                int l = a.ToString().Length;
                for (int i = l; i < longitud; i++)
                {
                    cadena += "0";
                }
                cadena += a.ToString();
            }
            catch
            {
                cadena = s;
            }
            return cadena;
        }
        
        private List<DET_AGENTE1> ObjAList2(string[,] dt, int rowsc)
        {

            List<DET_AGENTE1> ld = new List<DET_AGENTE1>();
            List<CLIENTE> clientes = new List<CLIENTE>();

            var rows = 0;
            var pos = 1;

            for (int i = rows; i < rowsc; i++)
            {
                DET_AGENTE1 doc = new DET_AGENTE1();

                string a = Convert.ToString(pos);

                doc.POS = Convert.ToInt32(a);

                try
                {
                    doc.BUKRS = dt[i, 0].ToUpper();
                }
                catch (Exception)
                {
                    doc.BUKRS = null;
                } //CoCode
                try
                {
                    doc.LAND = dt[i, 1].ToUpper();
                }
                catch (Exception)
                {
                    doc.LAND = null;
                } //Pais
                try
                {
                    doc.KUNNR = dt[i, 2];
                    doc.KUNNR = Completa(doc.KUNNR, 10);

                    CLIENTE u = clientes.FirstOrDefault(x => x.KUNNR.Equals(doc.KUNNR));
                    if (u == null)
                    {
                        u = db.CLIENTEs.FirstOrDefault(cc => cc.KUNNR.Equals(doc.KUNNR) && cc.ACTIVO);
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
                catch (Exception)
                {
                    doc.KUNNR = null;
                } //Cliente
                try
                {
                    doc.CLIENTE_N = dt[i, 3];
                }
                catch (Exception)
                {
                    doc.CLIENTE_N = null;
                } //Nombre Cliente
                try
                {
                    doc.ID_US0 = dt[i, 4];
                }
                catch (Exception)
                {
                    doc.ID_US0 = null;
                } //Nivel 0
                try
                {
                    doc.ID_US1 = dt[i, 5];
                }
                catch (Exception)
                {
                    doc.ID_US1 = null;
                } //Nivel 1
                try
                {
                    doc.ID_US2 = dt[i, 6];
                }
                catch (Exception)
                {
                    doc.ID_US2 = null;
                } //Nivel 2
                try
                {
                    doc.ID_US3 = dt[i, 7];
                }
                catch (Exception)
                {
                    doc.ID_US3 = null;
                } //Nivel 3
                try
                {
                    doc.ID_US4 = dt[i, 8];
                }
                catch (Exception )
                {
                    doc.ID_US4 = null;
                } //Nivel 4
                try
                {
                    doc.ID_US5 = dt[i, 9];
                }
                catch (Exception )
                {
                    doc.ID_US5 = null;
                } //Nivel 5
                try
                {
                    doc.ID_US6 = dt[i, 10];
                }
                catch (Exception )
                {
                    doc.ID_US6 = null;
                } //Nivel 6
                try
                {
                    doc.ID_US7 = dt[i, 11];
                }
                catch (Exception )
                {
                    doc.ID_US7 = null;
                } //Nivel 7
                try
                {
                    doc.ID_PROVEEDOR = dt[i, 12];
                    doc.ID_PROVEEDOR = Completa(doc.ID_PROVEEDOR, 10);
                }
                catch (Exception )
                {
                    doc.ID_PROVEEDOR = null;
                } //Vendor
                try
                {
                    doc.BANNER = dt[i, 13];
                    doc.BANNER = Completa(doc.BANNER, 10);
                }
                catch (Exception )
                {
                    doc.BANNER = null;
                } //Banner
                try
                {
                    doc.BANNERG = dt[i, 14];
                    doc.BANNERG = Completa(doc.BANNERG, 10);
                }
                catch (Exception )
                {
                    doc.BANNERG = null;
                } //Banner Agrupador
                try
                {
                    doc.CANAL = dt[i, 15];
                }
                catch (Exception )
                {
                    doc.CANAL = null;
                } //Canal
                try
                {
                    doc.EXPORTACION = dt[i, 16];
                }
                catch (Exception )
                {
                    doc.EXPORTACION = null;
                } //Exportacion
                try
                {
                    doc.CONTACTO = dt[i, 17];
                }
                catch (Exception )
                {
                    doc.CONTACTO = null;
                } //Contacto
                try
                {
                    doc.CONTACTOE = dt[i, 18];
                }
                catch (Exception )
                {
                    doc.CONTACTOE = null;
                } //Email de contacto
                try
                {
                    doc.MESS = dt[i, 19];
                }
                catch (Exception )
                {
                    doc.MESS = null;
                } //Mensaje

                ld.Add(doc);
                pos++;
            }
            return ld;
        }

        public partial class DET_AGENTE1 : IEquatable<DET_AGENTE1>
        {
            public string BUKRS { get; set; }
            public string LAND { get; set; }
            public string KUNNR { get; set; }
            public string VKORG { get; set; }
            public string VTWEG { get; set; }
            public string SPART { get; set; }
            public string CLIENTE_N { get; set; }
            public string ID_US0 { get; set; }
            public string ID_US1 { get; set; }
            public string ID_US2 { get; set; }
            public string ID_US3 { get; set; }
            public string ID_US4 { get; set; }
            public string ID_US5 { get; set; }
            public string ID_US6 { get; set; }
            public string ID_US7 { get; set; }
            public string ID_PROVEEDOR { get; set; }
            public string BANNER { get; set; }
            public string BANNERG { get; set; }
            public string CANAL { get; set; }
            public string EXPORTACION { get; set; }
            public string CONTACTO { get; set; }
            public string CONTACTOE { get; set; }
            public string MESS { get; set; }
            public int POS { get; set; }

            public virtual CLIENTE CLIENTE { get; set; }
            public virtual USUARIO USUARIO { get; set; }

            public bool Equals(DET_AGENTE1 other)
            {
                throw new NotImplementedException();
            }
        }
        
        public JsonResult Usuario8(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";

            TruequeEntities db = new TruequeEntities();

            var c = (from x in db.USUARIOs
                     where x.ID.Contains(Prefix) && x.ACTIVO == true && x.PUESTO_ID == 8
                     select new { x.ID, x.NOMBRE, x.APELLIDO_P }).ToList();

            if (c.Count == 0)
            {
                var c2 = (from x in db.USUARIOs
                          where x.NOMBRE.Contains(Prefix) && x.ACTIVO == true && x.PUESTO_ID == 8
                          select new { x.ID, x.NOMBRE, x.APELLIDO_P }).ToList();
                c.AddRange(c2);
                if (c2.Count == 0)
                {
                    var c3 = (from x in db.USUARIOs
                              where x.APELLIDO_P.Contains(Prefix) && x.ACTIVO == true && x.PUESTO_ID == 8
                              select new { x.ID, x.NOMBRE, x.APELLIDO_P }).ToList();
                    c.AddRange(c3);
                }
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }

        public JsonResult Usuario9(string Prefix)
        {
            if (Prefix == null)
                Prefix = "";

            TruequeEntities db = new TruequeEntities();

            var c = (from x in db.USUARIOs
                     where x.ID.Contains(Prefix) && x.ACTIVO == true && x.PUESTO_ID == 9
                     select new { x.ID, x.NOMBRE, x.APELLIDO_P }).ToList();

            if (c.Count == 0)
            {
                var c2 = (from x in db.USUARIOs
                          where x.NOMBRE.Contains(Prefix) && x.ACTIVO == true && x.PUESTO_ID == 9
                          select new { x.ID, x.NOMBRE, x.APELLIDO_P }).ToList();
                c.AddRange(c2);
                if (c2.Count == 0)
                {
                    var c3 = (from x in db.USUARIOs
                              where x.APELLIDO_P.Contains(Prefix) && x.ACTIVO == true && x.PUESTO_ID == 9
                              select new { x.ID, x.NOMBRE, x.APELLIDO_P }).ToList();
                    c.AddRange(c3);
                }
            }

            JsonResult cc = Json(c, JsonRequestBehavior.AllowGet);
            return cc;
        }
        


    }
}
