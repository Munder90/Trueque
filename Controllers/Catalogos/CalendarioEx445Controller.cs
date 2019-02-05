using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Models.Dao;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class CalendarioEx445Controller : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        const string CMB_SOCIEDADES = "SOC";
        const string CMB_TIPOSSOLICITUD = "TSOL";
        const string CMB_PERIODOS = "PER";
        const string CMB_USUARIOS = "USU";

        //------------------DAO------------------------------
        readonly UsuariosDao usuariosDao = new UsuariosDao();
        readonly SociedadesDao sociedadesDao = new SociedadesDao();
        readonly PeriodosDao periodosDao = new PeriodosDao();
        readonly TiposSolicitudesDao tiposSolicitudesDao = new TiposSolicitudesDao();

        // GET: CalendarioEx445/5
        public ActionResult Index(short ejercicio, int periodo, string sociedad_id, string tsol_id)
        {
            int pagina_id = 533;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            Calendario445ViewModel modelView = new Calendario445ViewModel();
            modelView.calendarioEx445.EJERCICIO = ejercicio;
            modelView.calendarioEx445.PERIODO = periodo;
            modelView.calendarioEx445.SOCIEDAD_ID = sociedad_id;
            modelView.calendarioEx445.TSOL_ID = tsol_id;

            modelView.calendariosEx445List = ObtenerExcepciones(ejercicio,periodo,sociedad_id,tsol_id);

            modelView.calendario445 = ObtenerCalendario445(ejercicio, periodo, sociedad_id, tsol_id);

            CargarSelectList(ref modelView, new string[] {
                CMB_SOCIEDADES + "," + sociedad_id,
                CMB_PERIODOS + "," + periodo,
                CMB_TIPOSSOLICITUD + "," + tsol_id,
                CMB_USUARIOS });

            return View(modelView);
        }
        

        // POST: CalendarioEx445/Index
        [HttpPost]
        public ActionResult Index(Calendario445ViewModel modelView)
        {
            int pagina_id = 533;//ID EN BASE DE DATOS
            CALENDARIO_EX calendarioEx = modelView.calendarioEx445;
            try
            {
                DateTime fechaActual = DateTime.Now;

                calendarioEx.USUARIOC_ID = User.Identity.Name;
                calendarioEx.FEHAC = fechaActual;
                calendarioEx.ACTIVO = true;

                if (!ValidarExcepcionExistente(calendarioEx) || !ValidarFechas(calendarioEx))
                {
                    throw (new Exception());
                }
                db.CALENDARIO_EX.Add(calendarioEx);
                db.SaveChanges();

                return RedirectToAction("Index",new {
                    ejercicio = calendarioEx.EJERCICIO,
                    periodo = calendarioEx.PERIODO,
                    sociedad_id = calendarioEx.SOCIEDAD_ID,
                    tsol_id = calendarioEx.TSOL_ID });
            }
            catch (Exception)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

                modelView.calendariosEx445List = ObtenerExcepciones(calendarioEx.EJERCICIO, calendarioEx.PERIODO, calendarioEx.SOCIEDAD_ID, calendarioEx.TSOL_ID); 
                modelView.calendario445 = ObtenerCalendario445(calendarioEx.EJERCICIO, calendarioEx.PERIODO, calendarioEx.SOCIEDAD_ID, calendarioEx.TSOL_ID);

                CargarSelectList(ref modelView, new string[] {
                    CMB_SOCIEDADES + "," + calendarioEx.SOCIEDAD_ID,
                    CMB_PERIODOS + "," + calendarioEx.PERIODO,
                    CMB_TIPOSSOLICITUD + "," + calendarioEx.TSOL_ID,
                    CMB_USUARIOS });

                return View(modelView);
            }
        }
        // GET: CalendarioEx445/Edit/5
        public ActionResult Edit(short ejercicio, int periodo, string sociedad_id, string tsol_id, string usuario_id,string paginaIndex)
        {
            int pagina_id = 534;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            Calendario445ViewModel modelView = new Calendario445ViewModel
            {
                calendarioEx445 = db.CALENDARIO_EX
                .Where(x => x.EJERCICIO == ejercicio && x.PERIODO == periodo && x.SOCIEDAD_ID == sociedad_id && x.TSOL_ID == tsol_id && x.USUARIO_ID == usuario_id)
                .FirstOrDefault(),
                calendario445 = ObtenerCalendario445(ejercicio, periodo, sociedad_id, tsol_id)
            };

            CargarSelectList(ref modelView, new string[] {
                CMB_SOCIEDADES + "," + sociedad_id,
                CMB_PERIODOS + "," + periodo,
                CMB_TIPOSSOLICITUD + "," + tsol_id,
                CMB_USUARIOS +","+ usuario_id });
            ViewBag.paginaIndex = paginaIndex;

            return View(modelView);
        }

        // POST: CalendarioEx445/Edit
        [HttpPost]
        public ActionResult Edit(Calendario445ViewModel modelView, string paginaIndex)
        {
            int pagina_id = 534;//ID EN BASE DE DATOS
            CALENDARIO_EX calendarioEx = modelView.calendarioEx445;
            try
            {
                if ( !ValidarFechas(calendarioEx))
                {
                    throw (new Exception());
                }
                db.Entry(calendarioEx).State = EntityState.Modified;
                db.SaveChanges();
                if (paginaIndex== "Calendario445") {
                    return RedirectToAction("Index",paginaIndex);
                   }
                else
                {
                    return RedirectToAction("Index", new {
                        ejercicio = calendarioEx.EJERCICIO,
                        periodo = calendarioEx.PERIODO,
                        sociedad_id = calendarioEx.SOCIEDAD_ID,
                        tsol_id = calendarioEx.TSOL_ID });

                }
            }
            catch (Exception)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                modelView.calendario445 = ObtenerCalendario445(calendarioEx.EJERCICIO, calendarioEx.PERIODO, calendarioEx.SOCIEDAD_ID, calendarioEx.TSOL_ID);

                CargarSelectList(ref modelView, new string[] {
                    CMB_SOCIEDADES + "," + calendarioEx.SOCIEDAD_ID,
                    CMB_PERIODOS + "," + calendarioEx.PERIODO,
                    CMB_TIPOSSOLICITUD + "," + calendarioEx.TSOL_ID,
                    CMB_USUARIOS +","+calendarioEx.USUARIO_ID });
                ViewBag.paginaIndex = paginaIndex;

                return View(modelView);
            }
        }
        bool ValidarExcepcionExistente(CALENDARIO_EX calendarioEx)
        {
            int pagina_id = 530;
            if (db.CALENDARIO_EX.Any(x => x.SOCIEDAD_ID == calendarioEx.SOCIEDAD_ID 
            && x.PERIODO == calendarioEx.PERIODO && x.EJERCICIO == calendarioEx.EJERCICIO
            && x.TSOL_ID == calendarioEx.TSOL_ID && x.USUARIO_ID == calendarioEx.USUARIO_ID))
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_mnjExisteExcepcion",User.Identity.Name);
                return false;
            }
            return true;
        }
        bool ValidarFechas(CALENDARIO_EX calendarioEx)
        {
            int pagina_id = 530;

            if (calendarioEx.EX_FROMF > calendarioEx.EX_TOF)
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_mnjErrorRangoFechas",User.Identity.Name);
                return false;
            }
            return true;
        }
        
        void CargarSelectList(ref Calendario445ViewModel modelView, string[] combos)
        {
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            for (int i = 0; i < combos.Length; i++)
            {
                string[] combosSplit = combos[i].Split(',');
                string combo = combosSplit[0];
                string id = combosSplit.Length > 1 ? combosSplit[1] : null;

                switch (combo)
                {
                    case CMB_SOCIEDADES:
                        modelView.sociedades = sociedadesDao.ComboSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES, id);
                        break;
                    case CMB_PERIODOS:
                        int? idAux = (id == null ? null : (int?)int.Parse(id));
                        modelView.periodos = periodosDao.ComboPeriodos( spras_id, idAux);
                        break;
                    case CMB_USUARIOS:
                        modelView.usuarios = usuariosDao.ComboUsuarios(TATConstantes.ACCION_LISTA_USUARIO, id);
                        break;
                    case CMB_TIPOSSOLICITUD:
                        modelView.cmbTiposSolicitud = tiposSolicitudesDao.ComboTiposSolicitudes( spras_id, id);
                        break;
                    default:
                        break;
                }
            }
        }
        List<CALENDARIO_EX> ObtenerExcepciones(short ejercicio, int periodo, string sociedad_id, string tsol_id)
        {
            return db.CALENDARIO_EX
                .Where(x => x.EJERCICIO == ejercicio && x.PERIODO == periodo && x.SOCIEDAD_ID == sociedad_id && x.TSOL_ID == tsol_id)
                .ToList();
        }
        CALENDARIO_AC ObtenerCalendario445(short ejercicio, int periodo, string sociedad_id, string tsol_id)
        {
            return db.CALENDARIO_AC
                 .Where(x => x.EJERCICIO == ejercicio && x.PERIODO == periodo && x.SOCIEDAD_ID == sociedad_id && x.TSOL_ID == tsol_id)
                 .FirstOrDefault();
        }
    }
}
