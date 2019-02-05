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
    public class AlertasController : Controller
    {

        readonly TruequeEntities db = new TruequeEntities();

        const string CMB_SOCIEDADES = "SOC";
        const string CMB_TIPOSSOLICITUD = "TSOL";
        const string CMBTREE_TIPOSSOLICITUD = "TREE_TSOL";
        const string CMB_TABS = "TAB";
        const string CMB_CAMPOS = "CAMP";
        const string CMB_TIPOS = "TIP";
        const string CMB_CONDCAMPOS = "CCAMP";
        const string CMB_CONDVALORES = "CVAL";

        const string WARNING_ID_EX = "concepto1";

        //------------------DAO------------------------------
        readonly SociedadesDao sociedadesDao = new SociedadesDao();
        readonly TiposSolicitudesDao tiposSolicitudesDao = new TiposSolicitudesDao();
        readonly TabsDao tabsDao = new TabsDao();
        readonly CamposDao camposDao = new CamposDao();

        // GET: Alertas
        public ActionResult Index()
        {
            int pagina_id = 540;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            string spras_id = ViewBag.spras_id;
            AlertaViewModel modelView = new AlertaViewModel
            {
                alertas = db.WARNINGPs.ToList(),
                alertaMensajes = db.WARNINGPTs.Where(x => x.SPRAS_ID == spras_id).ToList(),
                alertaCondiciones = db.WARNING_COND.Where(x => x.ACTIVO).ToList(),
                tabCampos = db.TAB_CAMPO
                            .Join(db.TEXTOes, tc => tc.CAMPO_ID, te => te.CAMPO_ID, (ta, te) => te)
                            .Where(x => x.SPRAS_ID == spras_id && x.PAGINA_ID == 202).ToList()
            };
            CargarSelectList(ref modelView, new string[] { CMB_TABS, CMB_CONDVALORES });
            return View(modelView);
        }

        // GET: Alertas/Create
        public ActionResult Create()
        {
            int pagina_id = 541;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            AlertaViewModel modelView = new AlertaViewModel
            {
                //Mensajes
                alertaMensajes = new List<WARNINGPT> { new WARNINGPT { SPRAS_ID = "ES" }, new WARNINGPT { SPRAS_ID = "EN" }},
                //Condiciones
                alertaCondiciones = new List<WARNING_COND> { new WARNING_COND { POS = 1 }, new WARNING_COND { POS = 2, ORAND = ")" } }
            };
            //Combos
            CargarSelectList(ref modelView, new string[] { CMB_TIPOS,CMB_SOCIEDADES, CMBTREE_TIPOSSOLICITUD, CMB_TABS , CMB_CONDCAMPOS },null,true);
            return View(modelView);
        }

        // POST: Alertas/Create
        [HttpPost]
        public ActionResult Create(AlertaViewModel modelView)
        {
            int pagina_id = 541;//ID EN BASE DE DATOS
            try
            {
                WARNINGP warningpAux = modelView.alerta;
                bool reverso = (warningpAux.TSOL_ID == "REV");
                string id_padre = "";
                string tipo_padre = "";
                string spras_id = FnCommon.ObtenerSprasId(db,User.Identity.Name);
                if (!reverso && warningpAux.TSOL_ID.Contains("|")) {
                     id_padre = warningpAux.TSOL_ID.Split('|')[0];
                     tipo_padre = warningpAux.TSOL_ID.Split('|')[1];
                }
                List<SelectTreeItem> tiposSolicitudesT =tiposSolicitudesDao.ObtenerItemsTSOLT(id_padre, tipo_padre, spras_id, reverso);
                List<WARNINGP> warningpListAux = new List<WARNINGP>();
                bool valido = true;
                foreach (string sociedad_id in modelView.sociedadesSelected)
                {
                    foreach (SelectTreeItem tsolt in tiposSolicitudesT)
                    {
                        WARNINGP warningP = new WARNINGP
                        {
                            ID = warningpAux.ID.Replace(" ", "_") + "_" + sociedad_id + "_" + tsolt.value,
                            DESCR = warningpAux.DESCR,
                            SOCIEDAD_ID = sociedad_id,
                            TIPO = warningpAux.TIPO,
                            TSOL_ID= tsolt.value,
                            CAMPOVAL_ID = warningpAux.CAMPO_ID,
                            PAGINA_ID = warningpAux.PAGINA_ID,
                            CAMPO_ID = warningpAux.CAMPO_ID,
                            ACCION = "focusout",
                            TAB_ID = warningpAux.TAB_ID
                        };
                        warningpListAux.Add(warningP);
                        if (!ValidarAlertaExistente(warningP) || !ValidarCondExistente(warningP, modelView.alertaCondiciones))
                        {
                            valido = false;
                        }
                    }
                }
                if (!valido)
                {
                    throw (new Exception());
                }
                List<WARNING_COND> warningconds = modelView.alertaCondiciones;
                List<WARNINGPT> warningts = modelView.alertaMensajes;
                foreach (WARNINGP warningP in warningpListAux)
                {
                        //Guardar Alerta
                        db.WARNINGPs.Add(warningP);

                        //Guardar Mensajes
                        warningts.ForEach(x =>
                        {
                            x.TAB_ID = warningP.TAB_ID;
                            x.WARNING_ID = warningP.ID;
                            db.WARNINGPTs.Add(new WARNINGPT {
                                SPRAS_ID =x.SPRAS_ID,
                                TAB_ID= warningP.TAB_ID,
                                WARNING_ID = warningP.ID,
                                TXT100=x.TXT100
                            });
                        });

                        //Guardar Condiciones
                        warningconds.ForEach(x =>
                        {
                            if (x.CONDICION_ID != null && x.VALOR_COMP != null)
                            {
                                db.WARNING_COND.Add(new WARNING_COND {
                                    TAB_ID = warningP.TAB_ID,
                                    WARNING_ID = warningP.ID,
                                    POS=x.POS,
                                    ANDOR=x.ANDOR,
                                    ORAND=x.ORAND,
                                    CONDICION_ID=x.CONDICION_ID,
                                    ACTIVO = true,
                                    VALOR_COMP=(x.VALOR_COMP == "v"?"": x.VALOR_COMP)
                                });
                            }
                        });
                        db.SaveChanges();
                    
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Log.ErrorLogApp( e,"Alertas","Create");
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                CargarSelectList(ref modelView, new string[] {
                    CMB_SOCIEDADES,
                    CMBTREE_TIPOSSOLICITUD,
                    CMB_TABS,
                    CMB_TIPOS,
                    CMB_CAMPOS,
                    CMB_CONDCAMPOS,
                    CMB_CONDVALORES  },
                    modelView.alerta.TAB_ID,
                    true,
                    modelView.alertaCondiciones[0].CONDICION_ID,
                    modelView.alertaCondiciones[1].CONDICION_ID);
                return View(modelView);
            }
        }

        // GET: Alertas/Edit
        public ActionResult Edit(string warning_id, string tab_id)
        {
            int pagina_id = 542;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            AlertaViewModel modelView = new AlertaViewModel
            {
                //Alerta
                alerta = db.WARNINGPs.Where(x => x.ID == warning_id && x.TAB_ID == tab_id).FirstOrDefault()
            };
            if (modelView.alerta == null) { return RedirectToAction("Index"); }

            //Mensajes
            modelView.alertaMensajes = new List<WARNINGPT>
            {
                ObtenerWarningT("ES", warning_id, tab_id),
                ObtenerWarningT("EN", warning_id, tab_id)
            };

            //Condiciones  
            bool esAlertaGral = (modelView.alerta.SOCIEDAD_ID == null || modelView.alerta.TSOL_ID == null || modelView.alerta.ID == WARNING_ID_EX);
            modelView.alertaCondiciones = db.WARNING_COND.Where(x => x.WARNING_ID == warning_id && x.TAB_ID == tab_id && x.ACTIVO).ToList();
            modelView.alertaCondiciones.Where(x => x.VALOR_COMP == "").ToList().ForEach(x => { x.VALOR_COMP = (x.VALOR_COMP == "" ? "v" : x.VALOR_COMP); });
            if (!esAlertaGral && modelView.alertaCondiciones.Count == 1)
            {
                modelView.alertaCondiciones.Add(new WARNING_COND { POS = 2, ORAND = ")" });
            }

            //Combos
            CargarSelectList(ref modelView, new string[] {
                CMB_TIPOS,
                CMB_SOCIEDADES + "," + modelView.alerta.SOCIEDAD_ID,
                CMB_TIPOSSOLICITUD + "," + modelView.alerta.TSOL_ID,
                CMB_TABS + "," + modelView.alerta.TAB_ID,
                CMB_CAMPOS + "," + modelView.alerta.CAMPO_ID,
                CMB_CONDCAMPOS,
                CMB_CONDVALORES },
                modelView.alerta.TAB_ID,
                (esAlertaGral ? null : (bool?)true),
                (esAlertaGral ? null : modelView.alertaCondiciones[0].CONDICION_ID),
                (esAlertaGral ? null : modelView.alertaCondiciones[1].CONDICION_ID));
            return View(modelView);
        }

        // POST: Alertas/Edit
        [HttpPost]
        public ActionResult Edit(AlertaViewModel modelView)
        {
            int pagina_id = 542;//ID EN BASE DE DATOS
            bool esAlertaGral = false;
            try
            {
                WARNINGP warningP = modelView.alerta;
                List<WARNINGPT> warningts = modelView.alertaMensajes;
                List<WARNING_COND> warningconds = modelView.alertaCondiciones;

                esAlertaGral = (modelView.alerta.SOCIEDAD_ID == null || modelView.alerta.TSOL_ID == null || modelView.alerta.ID == WARNING_ID_EX);
                if (!esAlertaGral && !ValidarCondExistente(warningP, warningconds))
                {
                    throw(new Exception());
                }
                //Guardar Alerta
                db.Entry(warningP).State = EntityState.Modified;

                //Guardar Mensajes
                warningts.ForEach(x =>
                {
                    if (x.TAB_ID == null && x.WARNING_ID == null)
                    {
                        x.TAB_ID = warningP.TAB_ID;
                        x.WARNING_ID = warningP.ID;
                        db.WARNINGPTs.Add(x);
                    }
                    else
                    {
                        db.Entry(x).State = EntityState.Modified;
                    }
                });
                if (!esAlertaGral)
                {
                    // Eliminar Condiciones
                    db.WARNING_COND.RemoveRange(db.WARNING_COND.Where(x => x.WARNING_ID == warningP.ID && x.TAB_ID == warningP.TAB_ID));
                    db.SaveChanges();
                    //Guardar  Condiciones
                    warningconds.ForEach(x =>
                    {
                        if (x.VALOR_COMP == "v") {  x.VALOR_COMP = "";}
                        if ( x.CONDICION_ID != null && x.VALOR_COMP != null)
                        {
                            x.TAB_ID = warningP.TAB_ID;
                            x.WARNING_ID = warningP.ID;
                            x.ACTIVO = true;
                            db.WARNING_COND.Add(x);
                        }

                    });
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                CargarSelectList(ref modelView, new string[] {
                    CMB_TIPOS,
                    CMB_SOCIEDADES + "," + modelView.alerta.SOCIEDAD_ID,
                    CMB_TIPOSSOLICITUD + "," + modelView.alerta.TSOL_ID,
                    CMB_TABS + "," + modelView.alerta.TAB_ID,
                    CMB_CAMPOS + "," + modelView.alerta.CAMPO_ID,
                    CMB_CONDCAMPOS, CMB_CONDVALORES },
                    modelView.alerta.TAB_ID,
                    (esAlertaGral ? null : (bool?)true),
                    (esAlertaGral ? null : modelView.alertaCondiciones[0].CONDICION_ID),
                    (esAlertaGral ? null : modelView.alertaCondiciones[1].CONDICION_ID));
                return View(modelView);
            }
        }

        // GET: Alertas/Delete
        public ActionResult Delete(string warning_id, string tab_id)
        {
            WARNINGP warningP = db.WARNINGPs.Where(x => x.ID == warning_id && x.TAB_ID == tab_id).FirstOrDefault();
            if (warningP == null) { return RedirectToAction("Index"); }
            List<WARNINGPT> warningts = db.WARNINGPTs.Where(x => x.WARNING_ID == warning_id && x.TAB_ID == tab_id).ToList();
            List<WARNING_COND> warningconds = db.WARNING_COND.Where(x => x.WARNING_ID == warning_id && x.TAB_ID == tab_id).ToList();

            db.WARNING_COND.RemoveRange(warningconds);
            db.WARNINGPTs.RemoveRange(warningts);
            db.WARNINGPs.Remove(warningP);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult ObtenerCamposPorTabId(string val)
        {
            AlertaViewModel modelView = new AlertaViewModel();
            CargarSelectList(ref modelView, new string[] { CMB_CAMPOS }, val,true);
            return Json(modelView.campos);
        }
        [HttpPost]
        public ActionResult ObtenerCondValores(int val)
        {
            AlertaViewModel modelView = new AlertaViewModel();
            CargarSelectList(ref modelView, new string[] { CMB_CONDVALORES });
            return Json(ObtenerCondValores(modelView.condValores, val));
        }

        WARNINGPT ObtenerWarningT(string spras_id, string warning_id, string tab_id)
        {
            WARNINGPT mensaje = db.WARNINGPTs.Where(x => x.WARNING_ID == warning_id && x.TAB_ID == tab_id && x.SPRAS_ID == spras_id).FirstOrDefault();
            return mensaje ?? new WARNINGPT { SPRAS_ID = spras_id };
        }
        bool ValidarAlertaExistente(WARNINGP warningP)
        {
            int pagina_id = 540;
            if (db.WARNINGPs.Any(x => x.ID == warningP.ID && x.TAB_ID == warningP.TAB_ID))
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db,pagina_id, "lbl_mnjExisteAlerta",User.Identity.Name);
                return false;
            }
            return true;
        }
        bool ValidarCondExistente(WARNINGP warningP, List<WARNING_COND> warningConds)
        {
            int pagina_id = 540;

            bool existeCondParaTabCampo = false;
            List<WARNING_COND> warningPs = db.WARNINGPs
                .Where(x => x.CAMPO_ID == warningP.CAMPO_ID && x.TAB_ID == warningP.TAB_ID && x.ID != warningP.ID && ((x.SOCIEDAD_ID ==null || x.SOCIEDAD_ID==warningP.SOCIEDAD_ID )&&(x.TSOL_ID==null || x.TSOL_ID==warningP.TSOL_ID)))
                .Join(db.WARNING_COND, w => w.ID, wc => wc.WARNING_ID, (w, wc) => wc)
                .ToList();
            warningPs.ForEach(x =>
            {
                if (warningConds.Any(y => y.VALOR_COMP == x.VALOR_COMP && y.CONDICION_ID == x.CONDICION_ID))
                {
                    existeCondParaTabCampo = true;
                }
            });
            if (warningConds.Count == 2 && warningConds[0].VALOR_COMP == warningConds[1].VALOR_COMP && warningConds[0].CONDICION_ID == warningConds[1].CONDICION_ID)
            {
                existeCondParaTabCampo = true;
            }

            if (existeCondParaTabCampo)
            {
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_mnjExisteCondAlerta",User.Identity.Name);
                return false;
            }

            return true;
        }
       

        void CargarSelectList(ref AlertaViewModel modelView, string[] combos, string tab_id = null,bool? activo = null, int? cond_id = null, int? cond_id1 = null)
        {
            string spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            for (int i = 0; i < combos.Length; i++)
            {
                string[] combosSplit = combos[i].Split(',');
                string combo = combosSplit[0];
                string id = combosSplit.Length > 1 ? combosSplit[1] : null;
                int? idAux = null;
                switch (combo)
                {
                    case CMB_SOCIEDADES:
                        modelView.sociedades = sociedadesDao.ComboSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES, id);
                        break;
                    case CMBTREE_TIPOSSOLICITUD:
                        modelView.treeTiposSolicitud = tiposSolicitudesDao.TreePadresTiposSolicitudes( spras_id);
                        break;
                    case CMB_TIPOSSOLICITUD:
                        modelView.cmbTiposSolicitud = tiposSolicitudesDao.ComboTiposSolicitudes(spras_id, id);
                        break;
                    case CMB_TABS:
                        modelView.tabs = tabsDao.ComboTabs(spras_id, activo, id);
                        break;
                    case CMB_CAMPOS:
                        modelView.campos = camposDao.ComboCamposPorTabId(spras_id,tab_id, activo,id);
                        break;
                    case CMB_TIPOS:
                        int pagina_id = 540;//ID EN BASE DE DATOS
                        string error = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_error",User.Identity.Name);
                        string alerta = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_alerta",User.Identity.Name);
                        modelView.tipos = new List<SelectListItem> {
                            new SelectListItem{ Value = "A", Text = "A - "+alerta},
                            new SelectListItem {Value = "E",Text = "E - "+error}
                        };
                        break;
                    case CMB_CONDCAMPOS:
                        idAux = (id == null ? null : (int?)int.Parse(id));
                        modelView.condCampos = db.CONDICIONs
                            .Where(x => x.ACTIVO == true && (x.COND == "=" || x.COND == "!=" || x.COND == ">" || x.COND == "<"))
                            .Join(db.CONDICIONTs, c => c.ID, ct => ct.CONDICION_ID, (c, ct) => ct)
                            .Where(x => x.SPRAS_ID == spras_id && (x.CONDICION_ID == idAux || idAux == null))
                            .Select(x => new SelectListItem
                            {
                                Value = x.CONDICION_ID.ToString(),
                                Text = x.TXT050
                            }).ToList();
                        modelView.condCampos1 = modelView.condCampos
                            .Select(x => new SelectListItem
                            {
                                Value = x.Value,
                                Text = x.Text
                            }).ToList();
                        break;
                    case CMB_CONDVALORES:
                        idAux = (id == null ? null : (int?)int.Parse(id));
                        List<SelectListItem> condValoresAux = db.CONDICIONs
                            .Where(x => x.ACTIVO == true && (x.COND == "e" || x.COND == "dec" || x.COND == "0" || x.COND == "n" || x.COND == "c" || x.COND == ""))
                            .Join(db.CONDICIONTs, c => c.ID, ct => ct.CONDICION_ID, (c, ct) => ct)
                            .Where(x => x.SPRAS_ID == spras_id && (x.CONDICION_ID == idAux || idAux == null))
                            .Select(x => new SelectListItem
                            {
                                Value = (x.CONDICION.COND == "" ? "v" : x.CONDICION.COND),
                                Text = x.TXT050
                            }).ToList();

                        modelView.condValores = ObtenerCondValores(condValoresAux, cond_id);

                        if (cond_id1 != null)
                        {
                            modelView.condValores1 = ObtenerCondValores(condValoresAux, cond_id1);
                        }

                        break;
                    default:
                        break;
                }
            }
        }
        public List<SelectListItem> ObtenerCondValores(List<SelectListItem> condValores, int? cond_id)
        {
            if (cond_id == null)
            {
                return condValores;
            }
            CONDICION cond = db.CONDICIONs.Where(x => x.ID == cond_id).First();
            List<SelectListItem> condValoresAux;
            if (cond.COND.Equals(">") || cond.COND.Equals("<"))
            {
                condValoresAux = condValores.Where(x => x.Value.Equals("0")).ToList();
                return condValoresAux;
            }
            return condValores;
        }
        
        
    }
}
