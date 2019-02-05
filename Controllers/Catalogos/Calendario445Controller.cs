using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Models.Dao;
using TAT001.Services;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    public class Calendario445Controller : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();
        readonly UsuarioLogin usuValidateLogin = new UsuarioLogin();

        const string CMB_SOCIEDADES= "SOC";
        const string CMB_TIPOSSOLICITUD = "TSOL";
        const string CMB_PERIODOS = "PER";
        const string CMB_USUARIOS = "USU";
        const string CMB_EJERCICIO = "EJE";

        //------------------DAO------------------------------
        readonly UsuariosDao usuariosDao = new UsuariosDao();
        readonly SociedadesDao sociedadesDao = new SociedadesDao();
        readonly PeriodosDao periodosDao = new PeriodosDao();
        readonly TiposSolicitudesDao tiposSolicitudesDao = new TiposSolicitudesDao();


        // GET: Calendario445
        [LoginActive]
        public ActionResult Index()
        {
            int pagina_id = 530;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db,pagina_id,User.Identity.Name, this.ControllerContext.Controller);

            Calendario445ViewModel modelView = new Calendario445ViewModel
            {
                pageSizes = FnCommon.ObtenerCmbPageSize()
            };
            ObtenerListado(ref modelView);
            ObtenerListadoEx(ref modelView);

            return View(modelView);
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
            int pagina_id = 530; //ID EN BASE DE DATOS
            Calendario445ViewModel modelView = new Calendario445ViewModel();
            ObtenerListado(ref modelView, colOrden, ordenActual, numRegistros, pagina, buscar);
            FnCommon.ObtenerTextos(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            return View(modelView);
        }

        public ActionResult ListEx(string colOrden, string ordenActual, int? numRegistros = 10, int? pagina = 1, string buscar = "")
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
            int pagina_id = 530; //ID EN BASE DE DATOS
            Calendario445ViewModel modelView = new Calendario445ViewModel();
            ObtenerListadoEx(ref modelView, colOrden, ordenActual, numRegistros, pagina, buscar);
            FnCommon.ObtenerTextos(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            return View(modelView);
        }
        // GET: Calendario445/Create
        [LoginActive]
        public ActionResult Create()
        {
            int pagina_id = 531;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            Calendario445ViewModel modelView = new Calendario445ViewModel();
            CargarSelectList(ref modelView,new string[] {  CMB_PERIODOS, CMB_EJERCICIO });
            return View(modelView);
        }

        // POST: Calendario445/Create
        [HttpPost]
        [LoginActive]
        public ActionResult Create(Calendario445ViewModel modelView)
        {
            int pagina_id = 531;//ID EN BASE DE DATOS
            try
            {
                CALENDARIO_AC calendarioAc = modelView.calendario445;
                calendarioAc.ACTIVO = true;
                calendarioAc.USUARIOC_ID = User.Identity.Name;
                calendarioAc.FECHAC = DateTime.Now;

                if (!ValidarPeriodoExistente(calendarioAc) ||!ValidarFechas(calendarioAc))
                {
                    throw (new Exception());
                }

                string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
                sociedadesDao.ListaSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES).ForEach(x=>
                {
                    tiposSolicitudesDao.ListaTiposSolicitudes(spras_id, null).ForEach(z =>
                    {
                        calendarioAc.SOCIEDAD_ID = x.BUKRS;
                        calendarioAc.TSOL_ID = z.TSOL_ID;
                        if (!db.CALENDARIO_AC.Any(y =>
                        y.EJERCICIO == calendarioAc.EJERCICIO
                        && y.PERIODO == calendarioAc.PERIODO
                        && y.SOCIEDAD_ID == calendarioAc.SOCIEDAD_ID
                        && y.TSOL_ID == calendarioAc.TSOL_ID))
                        {
                            db.CALENDARIO_AC.Add(new CALENDARIO_AC
                            {
                                EJERCICIO = calendarioAc.EJERCICIO,
                                PERIODO = calendarioAc.PERIODO,
                                SOCIEDAD_ID = calendarioAc.SOCIEDAD_ID,
                                TSOL_ID = calendarioAc.TSOL_ID,
                                PRE_FROMF = calendarioAc.PRE_FROMF,
                                PRE_FROMH = calendarioAc.PRE_FROMH,
                                PRE_TOF = calendarioAc.PRE_TOF,
                                PRE_TOH = calendarioAc.PRE_TOH,
                                CIE_FROMF = calendarioAc.CIE_FROMF,
                                CIE_FROMH = calendarioAc.CIE_FROMH,
                                CIE_TOF = calendarioAc.CIE_TOF,
                                CIE_TOH = calendarioAc.CIE_TOH,
                                USUARIOC_ID = calendarioAc.USUARIOC_ID,
                                FECHAC = calendarioAc.FECHAC,
                                USUARIOM_ID = calendarioAc.USUARIOM_ID,
                                FECHAM = calendarioAc.FECHAM,
                                ACTIVO = calendarioAc.ACTIVO

                            });
                            db.SaveChanges();
                        }
                    });
                });
              


                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                CargarSelectList(ref modelView, new string[] {  CMB_PERIODOS, CMB_EJERCICIO  });
                return View(modelView);
            }
        }

        [LoginActive]
        public ActionResult Edit(short ejercicio, int periodo, string sociedad_id, string tsol_id )
        {
            int pagina_id = 532;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            Calendario445ViewModel modelView = new Calendario445ViewModel
            {
                calendario445 = db.CALENDARIO_AC.Where(x => x.EJERCICIO == ejercicio && x.PERIODO == periodo && x.SOCIEDAD_ID == sociedad_id && x.TSOL_ID == tsol_id).FirstOrDefault()
            };

            CargarSelectList(ref modelView, new string[] {
                CMB_SOCIEDADES +","+ modelView.calendario445.SOCIEDAD_ID,
                CMB_PERIODOS + "," + modelView.calendario445.PERIODO,
                CMB_EJERCICIO,
                CMB_TIPOSSOLICITUD + "," + modelView.calendario445.TSOL_ID});

            return View(modelView);
        }
        
        [HttpPost]
        [LoginActive]
        public ActionResult Edit(Calendario445ViewModel modelView)
        {
            int pagina_id = 532;//ID EN BASE DE DATOS
            try
            {
                CALENDARIO_AC calendarioAc = modelView.calendario445;
                DateTime fechaActual = DateTime.Now;

                calendarioAc.USUARIOM_ID = User.Identity.Name;
                calendarioAc.FECHAM = fechaActual;

                if (!ValidarFechas(calendarioAc))
                {
                    throw (new Exception());
                }

                db.Entry(calendarioAc).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                CargarSelectList(ref modelView, new string[] {
                    CMB_SOCIEDADES + "," + modelView.calendario445.SOCIEDAD_ID,
                    CMB_PERIODOS + "," + modelView.calendario445.PERIODO,
                    CMB_EJERCICIO,
                    CMB_TIPOSSOLICITUD + "," + modelView.calendario445.TSOL_ID });
                return View(modelView);
            }
        }

        bool ValidarPeriodoExistente(CALENDARIO_AC calendarioAc)
        {
            int pagina_id = 530;
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);
            int noExiste = 0;
            sociedadesDao.ListaSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES).ForEach(x =>
            {
                tiposSolicitudesDao.ListaTiposSolicitudes(spras_id,null).ForEach(z =>
                {
                    calendarioAc.SOCIEDAD_ID = x.BUKRS;
                    calendarioAc.TSOL_ID = z.TSOL_ID;
                    if (!db.CALENDARIO_AC.Any(y => y.EJERCICIO == calendarioAc.EJERCICIO && y.PERIODO == calendarioAc.PERIODO 
                    && y.SOCIEDAD_ID == calendarioAc.SOCIEDAD_ID && y.TSOL_ID == calendarioAc.TSOL_ID))
                    {
                        noExiste++;
                    }
                });
            });
            if (noExiste==0)
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db,pagina_id, "lbl_mnjExistePeriodo",User.Identity.Name);
                return false;
            }
            return true;
        }
        bool ValidarFechas(CALENDARIO_AC calendarioAc)
        {
            int pagina_id = 530;

            DateTime? fechaIPre = new DateTime(calendarioAc.PRE_FROMF.Year, calendarioAc.PRE_FROMF.Month, calendarioAc.PRE_FROMF.Day, calendarioAc.PRE_FROMH.Hours, calendarioAc.PRE_FROMH.Minutes, calendarioAc.PRE_FROMH.Seconds);
            DateTime? fechaFPre = new DateTime(calendarioAc.PRE_TOF.Year, calendarioAc.PRE_TOF.Month, calendarioAc.PRE_TOF.Day, calendarioAc.PRE_TOH.Hours, calendarioAc.PRE_TOH.Minutes, calendarioAc.PRE_TOH.Seconds);

            DateTime? fechaICi = new DateTime(calendarioAc.CIE_FROMF.Year, calendarioAc.CIE_FROMF.Month, calendarioAc.CIE_FROMF.Day, calendarioAc.CIE_FROMH.Hours, calendarioAc.CIE_FROMH.Minutes, calendarioAc.CIE_FROMH.Seconds);
            DateTime? fechaFCi = new DateTime(calendarioAc.CIE_TOF.Year, calendarioAc.CIE_TOF.Month, calendarioAc.CIE_TOF.Day, calendarioAc.CIE_TOH.Hours, calendarioAc.CIE_TOH.Minutes, calendarioAc.CIE_TOH.Seconds);

            if (fechaIPre > fechaFPre
                || fechaICi > fechaFCi
                || fechaFPre > fechaICi)
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db,pagina_id, "lbl_mnjErrorRangoFechas",User.Identity.Name);
                return false;
            }
            
            CALENDARIO_AC calendarioAux = db.CALENDARIO_AC.Where(x => x.SOCIEDAD_ID == calendarioAc.SOCIEDAD_ID
                && x.EJERCICIO == calendarioAc.EJERCICIO
                && x.TSOL_ID == calendarioAc.TSOL_ID
                && x.PERIODO != calendarioAc.PERIODO
                && ((fechaIPre >= DbFunctions.CreateDateTime(x.PRE_FROMF.Year, x.PRE_FROMF.Month, x.PRE_FROMF.Day, x.PRE_FROMH.Hours, x.PRE_FROMH.Minutes, x.PRE_FROMH.Seconds) && fechaIPre <= DbFunctions.CreateDateTime(x.CIE_TOF.Year, x.CIE_TOF.Month, x.CIE_TOF.Day, x.CIE_TOH.Hours, x.CIE_TOH.Minutes, x.CIE_TOH.Seconds))
                || (fechaFCi >= DbFunctions.CreateDateTime(x.PRE_FROMF.Year, x.PRE_FROMF.Month, x.PRE_FROMF.Day, x.PRE_FROMH.Hours, x.PRE_FROMH.Minutes, x.PRE_FROMH.Seconds) && fechaFCi <= DbFunctions.CreateDateTime(x.CIE_TOF.Year, x.CIE_TOF.Month, x.CIE_TOF.Day, x.CIE_TOH.Hours, x.CIE_TOH.Minutes, x.CIE_TOH.Seconds)))).FirstOrDefault();
            if (calendarioAux != null)
            {
                ViewBag.mnjError = String.Format(FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_mnjTraslapeEnPeriodo", User.Identity.Name), calendarioAux.PERIODO);
                return false;
            }
            calendarioAux = db.CALENDARIO_AC.Where(x => x.SOCIEDAD_ID == calendarioAc.SOCIEDAD_ID
                && x.EJERCICIO == calendarioAc.EJERCICIO
                && x.TSOL_ID == calendarioAc.TSOL_ID
                && x.PERIODO < calendarioAc.PERIODO
                && x.PRE_FROMF > calendarioAc.PRE_FROMF).FirstOrDefault();
            if (calendarioAux != null)
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_mnjMenorAPeriodoAnt", User.Identity.Name);
                return false;
            }
            return true;
        }     
        void CargarSelectList(ref Calendario445ViewModel modelView,string[] combos)
        {
            string spras_id = FnCommon.ObtenerSprasId(db,User.Identity.Name);

            for (int i = 0; i < combos.Length; i++) {
                string[] combosSplit = combos[i].Split(',');
                string combo = combosSplit[0];
                string id = combosSplit.Length > 1 ? combosSplit[1] : null;

                switch (combo)
                {
                    case CMB_SOCIEDADES:
                        modelView.sociedades = sociedadesDao.ComboSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES,id);
                        break;
                    case CMB_PERIODOS:
                        int? idAux = (id==null?null: (int?)int.Parse(id));
                        modelView.periodos = periodosDao.ComboPeriodos(spras_id, idAux);
                        break;
                    case CMB_USUARIOS:
                        modelView.usuarios = usuariosDao.ComboUsuarios(TATConstantes.ACCION_LISTA_USUARIO, id);
                        break;
                    case CMB_TIPOSSOLICITUD:
                        modelView.cmbTiposSolicitud = tiposSolicitudesDao.ComboTiposSolicitudes( spras_id, id);
                        break;
                    case CMB_EJERCICIO:
                        modelView.ejercicio = FnCommon.ObtenerCmbEjercicio();
                        break;
                    default:
                        break;
                }
            }
        }
        void ObtenerListado(ref Calendario445ViewModel viewModel, string colOrden = "", string ordenActual = "", int? numRegistros = 10, int? pagina = 1, string buscar = "")
        {
            int pageIndex = pagina.Value;
            List<CALENDARIO_AC> calendarios445 = db.CALENDARIO_AC.Where(x => x.ACTIVO).ToList();

            viewModel.ordenActual = (string.IsNullOrEmpty(ordenActual) || !colOrden.Equals(ordenActual) ? colOrden : "");
            viewModel.numRegistros = numRegistros.Value;
            viewModel.buscar = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                calendarios445 = calendarios445.Where(x => 
                String.Concat(x.SOCIEDAD_ID, x.PERIODO.ToString(), x.TSOL_ID,
                x.PRE_FROMF.ToString("dd/MM/yyyy"), x.PRE_FROMH, x.PRE_TOF.ToString("dd/MM/yyyy"), x.PRE_TOH,
                x.CIE_FROMF.ToString("dd/MM/yyyy"), x.CIE_FROMH, x.CIE_TOF.ToString("dd/MM/yyyy"), x.CIE_TOH)
                .ToLower().Contains(buscar.ToLower()))
                .ToList();
            }
            switch (colOrden)
            {
                case "SOCIEDAD_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.SOCIEDAD_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.SOCIEDAD_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "PERIODO":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.PERIODO).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.PERIODO).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "TSOL_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.TSOL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.TSOL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "PRE_FROMF":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.PRE_FROMF).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.PRE_FROMF).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "PRE_FROMH":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.PRE_FROMH).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.PRE_FROMH).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "PRE_TOF":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.PRE_TOF).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.PRE_TOF).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "PRE_TOH":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.PRE_TOH).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.PRE_TOH).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "CIE_FROMF":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.CIE_FROMF).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.CIE_FROMF).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "CIE_FROMH":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.CIE_FROMH).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.CIE_FROMH).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "CIE_TOF":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.CIE_TOF).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.CIE_TOF).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "CIE_TOH":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendarios445 = calendarios445.OrderByDescending(m => m.CIE_TOH).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.calendarios445 = calendarios445.OrderBy(m => m.CIE_TOH).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                default:
                    viewModel.calendarios445 = calendarios445.OrderBy(m => m.SOCIEDAD_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
            }
        }
        void ObtenerListadoEx(ref Calendario445ViewModel viewModel, string colOrden = "", string ordenActual = "", int? numRegistros = 10, int? pagina = 1, string buscar = "")
        {
            int pageIndex = pagina.Value;
            List<CALENDARIO_EX> calendariosEx445 = db.CALENDARIO_EX.ToList();

            viewModel.ordenActual = (string.IsNullOrEmpty(ordenActual) || !colOrden.Equals(ordenActual) ? colOrden : "");
            viewModel.numRegistrosEx = numRegistros.Value;
            viewModel.buscar = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                calendariosEx445 = calendariosEx445.Where(x =>
                String.Concat(x.SOCIEDAD_ID, x.PERIODO.ToString(), x.TSOL_ID,x.USUARIO_ID,
                x.EX_FROMF.ToString("dd/MM/yyyy"), x.EX_FROMH, x.EX_TOF.ToString("dd/MM/yyyy"), x.EX_TOH)
                .ToLower().Contains(buscar.ToLower()))
                .ToList();
            }
            switch (colOrden)
            {
                case "SOCIEDAD_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.SOCIEDAD_ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.SOCIEDAD_ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;
                case "PERIODO":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.PERIODO).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.PERIODO).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;

                case "TSOL_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.TSOL_ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.TSOL_ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;

                case "USUARIO_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.USUARIO.ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.USUARIO.ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;

                case "EX_FROMF":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.EX_FROMF).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.EX_FROMF).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;

                case "EX_FROMH":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.EX_FROMH).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.EX_FROMH).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;

                case "EX_TOF":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.EX_TOF).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.EX_TOF).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;

                case "EX_TOH":
                    if (colOrden.Equals(ordenActual))
                        viewModel.calendariosEx445 = calendariosEx445.OrderByDescending(m => m.EX_TOH).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    else
                        viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.EX_TOH).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;
               
                default:
                    viewModel.calendariosEx445 = calendariosEx445.OrderBy(m => m.SOCIEDAD_ID).ToPagedList(pageIndex, viewModel.numRegistrosEx);
                    break;
            }
        }
    }
}
