using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;
using TAT001.Models.Dao;

namespace TAT001.Controllers
{
    [Authorize]
    [LoginActive]
    public class LayoutCargaMasivaController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();
        const string CMB_SOCIEDADES = "SOC";
        const string CMB_PAIS = "PAIS";

        //------------DAO´s--------------
        readonly SociedadesDao sociedadesDao = new SociedadesDao();

        // GET: LayoutCargaMasiva
        public ActionResult Index()
        {
            int pagina_id = 550;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            LayoutCargaMasivaViewModels viewModel = new LayoutCargaMasivaViewModels
            {
                layouts = db.LAYOUT_CARGA.ToList()
            };

            return View(viewModel);
        }


        // GET: LayoutCargaMasiva/Create
        public ActionResult Create()
        {
            int pagina_id = 551;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            LayoutCargaMasivaViewModels modelView = new LayoutCargaMasivaViewModels();
            CargarSelectList(ref modelView, new string[] { CMB_PAIS, CMB_SOCIEDADES });

            return View(modelView);

        }

        // POST: LayoutCargaMasiva/Create
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase PathArchivo, LayoutCargaMasivaViewModels modelView)
        {
            int pagina_id = 551;//ID EN BASE DE DATOS
            string msj = "lbl_mnjErrorGuardar";
            try
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                var layoutExistente =  db.LAYOUT_CARGA.FirstOrDefault(x => x.LAND == modelView.layoutMasiva.LAND && x.SOCIEDAD_ID == modelView.layoutMasiva.SOCIEDAD_ID);
                if (layoutExistente!=null)
                {
                    msj = "lbl_LayoutExistente";
                    throw new Exception();
                }


                LAYOUT_CARGA layout = modelView.layoutMasiva;
                layout.FECHAC = DateTime.Now;
                layout.TIPO = "Solicitud";
                if (PathArchivo != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Archivos/LayoutCargaMasiva"));
                    var ruta = path + "/" + modelView.layoutMasiva.LAND +"_"+ modelView.layoutMasiva.SOCIEDAD_ID + "_" + PathArchivo.FileName;
                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }
                    PathArchivo.SaveAs(ruta);
                    layout.RUTA = ruta;


                    db.LAYOUT_CARGA.Add(modelView.layoutMasiva);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                CargarSelectList(ref modelView, new string[] { CMB_PAIS, CMB_SOCIEDADES });
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina_id, msj, User.Identity.Name);
                return View(modelView);
            }
        }
        // GET: Alertas/Delete
        public ActionResult Delete(int layout_id)
        {
            LAYOUT_CARGA layout = db.LAYOUT_CARGA.Where(x => x.ID == layout_id).FirstOrDefault();
            if (layout == null) { return RedirectToAction("Index"); }

            string RutaAnterior = layout.RUTA;
            db.LAYOUT_CARGA.Remove(layout);
            db.SaveChanges();
            System.IO.File.Delete(RutaAnterior);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            int pagina_id = 552;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            LayoutCargaMasivaViewModels modelView = new LayoutCargaMasivaViewModels();
            modelView.layoutMasiva = db.LAYOUT_CARGA.Where(x => x.ID == Id).FirstOrDefault();
            CargarSelectList(ref modelView, new string[]{
                CMB_SOCIEDADES+","+modelView.layoutMasiva.SOCIEDAD_ID,
                CMB_PAIS+","+modelView.layoutMasiva.LAND
            });
            string[] ruta = modelView.layoutMasiva.RUTA.Split(new string[] { "LayoutCargaMasiva/" }, StringSplitOptions.None);
            if (ruta.Length > 0)
                ViewBag.NombreArchivo = ruta[1];
            
            return View(modelView);
        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase PathArchivo, LayoutCargaMasivaViewModels modelView)
        {
            int pagina_id = 552;//ID EN BASE DE DATOS
            try
            {
                LAYOUT_CARGA layout = modelView.layoutMasiva;
                string rutaAnterior = modelView.layoutMasiva.RUTA;
                if (PathArchivo != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Archivos/LayoutCargaMasiva"));
                    var ruta = path + "/" + modelView.layoutMasiva.LAND + "_" + modelView.layoutMasiva.SOCIEDAD_ID+ "_" + PathArchivo.FileName;
                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }
                    if (System.IO.File.Exists(rutaAnterior))
                    {
                        System.IO.File.Delete(rutaAnterior);
                    }
                    PathArchivo.SaveAs(ruta);
                    layout.RUTA = ruta;
                    db.Entry(layout).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
                CargarSelectList(ref modelView, new string[]{
                CMB_SOCIEDADES+","+modelView.layoutMasiva.SOCIEDAD_ID,
                CMB_PAIS+","+modelView.layoutMasiva.LAND
            });
                ViewBag.mnjError = FnCommon.ObtenerTextoMnj(db, pagina_id, "lbl_mnjErrorGuardar", User.Identity.Name);

                return View(modelView);
            }
        }
        public ActionResult Details(int Id)
        {
            int pagina_id = 553;//ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            LayoutCargaMasivaViewModels modelView = new LayoutCargaMasivaViewModels();
            modelView.layoutMasiva = db.LAYOUT_CARGA.Where(x => x.ID == Id).FirstOrDefault();

            CargarSelectList(ref modelView, new string[]{
                CMB_SOCIEDADES+","+modelView.layoutMasiva.SOCIEDAD_ID,
                CMB_PAIS+","+modelView.layoutMasiva.LAND
            });

            string[] ruta = modelView.layoutMasiva.RUTA.Split(new string[] { "LayoutCargaMasiva/" }, StringSplitOptions.None);
            if (ruta.Length > 0)
                ViewBag.NombreArchivo = ruta[1];

            return View(modelView);
        }
        void CargarSelectList(ref LayoutCargaMasivaViewModels modelView, string[] combos)
        {
            for (int i = 0; i < combos.Length; i++)
            {
                string[] combosSplit = combos[i].Split(',');
                string combo = combosSplit[0];
                string id = combosSplit.Length > 1 ? combosSplit[1] : null;

                switch (combo)
                {
                    case CMB_SOCIEDADES:
                        modelView.sociedades = sociedadesDao.ComboSociedades(TATConstantes.ACCION_LISTA_SOCIEDADES);
                        break;
                    case CMB_PAIS:
                        modelView.paises = db.PAIS
                          .Where(x => (x.LAND == id || id == null) && x.ACTIVO)
                          .Select(x => new SelectListItem
                          {
                              Value = x.LAND,
                              Text = x.LAND + "-" + x.LANDX
                          }).ToList();
                        break;
                    default:
                        break;
                }
            }
        }

        [HttpPost]
        public FileResult Descargar(int idLayout)
        {
            try
            {
                Models.PresupuestoModels carga = new Models.PresupuestoModels();
                string archivo = db.LAYOUT_CARGA.FirstOrDefault(x => x.ID == idLayout).RUTA;
                string nombre = "", contentyp = "";
                carga.contDescarga(archivo, ref contentyp, ref nombre);
                return File(archivo, contentyp, nombre);
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e, "Layout", "Descargar");
                return null;
            }

        }
    }
}