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

namespace TAT001.Controllers
{
    [Authorize]
    public class ModificacionesGlobalesController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();

        //------------------DAO´s-------------
        readonly SolicitudesDao solicitudesDao = new SolicitudesDao();
        readonly SociedadesDao sociedadesDao = new SociedadesDao();

        readonly UsuarioLogin usuValidateLogin = new UsuarioLogin();

        // GET: ModificacionesGlobales
        [LoginActive]
        public ActionResult Index()
        {
            int pagina_id = 240;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            ModificacionesGlobalesViewModel modelView = new ModificacionesGlobalesViewModel
            {
                sociedades = sociedadesDao
                .ComboSociedades(TATConstantes.ACCION_LISTA_SOCPORUSUARIO, null, User.Identity.Name)
            };
            return View(modelView);
        }

        public ActionResult ListModAutorizador(string sociedad_id,decimal? num_doci, decimal? num_docf, DateTime? fechai, DateTime? fechaf, string kunnr, string usuarioa_id, string usuario_id)
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
            int pagina_id = 240;//ID EN BASE DE DATOS
            FnCommon.ObtenerTextos(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            ViewBag.spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            ModificacionesGlobalesViewModel modelView = new ModificacionesGlobalesViewModel
            {
                solicitudPorAprobar = solicitudesDao.ListaSolicitudesPorAprobar(sociedad_id,num_doci, num_docf, fechai, fechaf, kunnr, usuarioa_id, usuario_id)
            };
            return View(modelView);
        }

        [HttpPost]
        [LoginActive]
        public ActionResult ListModAutorizador(List<SolicitudPorAprobar> solicitudPorAprobar)
        {
            try
            {
                string usuarioa_id = solicitudPorAprobar.First().USUARIOA_ID_NUEVO;
                foreach (SolicitudPorAprobar sol in solicitudPorAprobar)
                {
                    //actualizar registro ant.
                    FLUJO f = db.FLUJOes.Where(x => x.NUM_DOC == sol.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();
                    if (f.ESTATUS== "P")
                    {

                        db.FLUJOes.Add(new FLUJO
                        {
                            WORKF_ID=f.WORKF_ID,
                            WF_VERSION=f.WF_VERSION,
                            WF_POS=f.WF_POS,
                            NUM_DOC=f.NUM_DOC,
                            POS=f.POS+1,
                            DETPOS=f.DETPOS,
                            DETVER=f.DETVER,
                            LOOP=f.LOOP,
                            USUARIOD_ID=f.USUARIOD_ID,
                            USUARIOA_ID = usuarioa_id,
                            ESTATUS = f.ESTATUS,
                            FECHAC = DateTime.Now,
                            FECHAM = DateTime.Now,
                            COMENTARIO = "Se modifica Autorizador",
                            STATUS= f.STATUS
                        });

                        f.ESTATUS = "M";
                        f.STATUS = null;
                        f.FECHAM = DateTime.Now;
                        db.Entry(f).State = EntityState.Modified;

                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e, "ModificacionesGlobales", "ListModAutorizador");
            }

            return RedirectToAction("Index");
        }

        public ActionResult ListModSolicitudes(string sociedad_id, decimal? num_doci, decimal? num_docf, DateTime? fechai, DateTime? fechaf, string kunnr, string usuario_id)
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
            int pagina_id = 240;//ID EN BASE DE DATOS
            FnCommon.ObtenerTextos(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            ViewBag.spras_id = FnCommon.ObtenerSprasId(db, User.Identity.Name);

            ModificacionesGlobalesViewModel modelView = new ModificacionesGlobalesViewModel
            {
                solicitudes = solicitudesDao.ListaSolicitudes(TATConstantes.ACCION_LISTA_SOCIEDADES,null,sociedad_id, num_doci, num_docf, usuario_id, fechai, fechaf, kunnr)
            };
            return View(modelView);
        }

        [HttpPost]
        [LoginActive]
        public ActionResult ListModSolicitudes(List<DOCUMENTO> solicitudes)
        {
            try
            {
                string contacto = solicitudes[0].PAYER_NOMBRE;
                string emailContacto = solicitudes[0].PAYER_EMAIL;
                string estado = solicitudes[0].ESTADO;
                string ciudad = solicitudes[0].CIUDAD;
                string concepto = solicitudes[0].CONCEPTO;
                string mecanica = solicitudes[0].NOTAS;
                
                foreach (DOCUMENTO sol in solicitudes)
                {
                    DOCUMENTO docActual = db.DOCUMENTOes.Find(sol.NUM_DOC);
                    if (!string.IsNullOrEmpty(contacto))
                    {
                        docActual.PAYER_NOMBRE = contacto;
                    }
                    if (!string.IsNullOrEmpty(emailContacto))
                    {
                        docActual.PAYER_EMAIL = emailContacto;
                    }
                    if (!string.IsNullOrEmpty(estado))
                    {
                        docActual.ESTADO = estado;
                    }
                    if (!string.IsNullOrEmpty(ciudad))
                    {
                        docActual.CIUDAD = ciudad;
                    }
                    if (!string.IsNullOrEmpty(concepto))
                    {
                        docActual.CONCEPTO = concepto;
                    }
                    if (!string.IsNullOrEmpty(mecanica))
                    {
                        docActual.NOTAS = mecanica;
                    }

                    db.Entry(docActual).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e, "ModificacionesGlobales", "ListModSolicitudes");
            }

            return RedirectToAction("Index");
        }

    }
}