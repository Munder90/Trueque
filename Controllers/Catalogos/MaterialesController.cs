using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
    public class MaterialesController : Controller
    {
        readonly TruequeEntities db = new TruequeEntities();
        readonly UsuarioLogin usuValidateLogin = new UsuarioLogin();

        [LoginActive]
        public ActionResult Index()
        {
            int pagina_id = 661; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            ViewBag.pais = Session["pais"] != null ? Session["pais"].ToString() + ".png" : null;


           MaterialViewModel viewModel = new MaterialViewModel();
            viewModel.pageSizes = FnCommon.ObtenerCmbPageSize();
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
            int pagina_id = 661; //ID EN BASE DE DATOS
            MaterialViewModel viewModel = new MaterialViewModel();
            ObtenerListado(ref viewModel, colOrden, ordenActual, numRegistros, pagina, buscar);
            FnCommon.ObtenerTextos(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);
            return View(viewModel);
        }

        public void ObtenerListado(ref MaterialViewModel viewModel, string colOrden = "", string ordenActual = "", int? numRegistros = 10, int? pagina = 1, string buscar = "")
        {
            int pageIndex = pagina.Value;
            List<MVKEMaterial> materiales = db.MATERIALVKEs.Join(db.MATERIALs,
                                                              p => p.MATERIAL_ID,
                                                              e => e.ID,
                                                              (p, e) => new MVKEMaterial
                                                              {
                                                                  MATERIAL_ID = p.MATERIAL_ID,
                                                                  VKORG = p.VKORG,
                                                                  VTWEG = p.VTWEG,
                                                                  MATKL_ID = e.MATKL_ID,
                                                                  MAKTX = e.MAKTX,
                                                                  MATERIAL_GROUP = e.MATERIALGP.DESCRIPCION,
                                                                  ACTIVO=p.ACTIVO
                                                              }).Where(t=>t.ACTIVO).ToList();
            viewModel.ordenActual = (string.IsNullOrEmpty(ordenActual) || !colOrden.Equals(ordenActual) ? colOrden : "");
            viewModel.numRegistros = numRegistros.Value;
            viewModel.buscar = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                materiales = materiales.Where(x =>
                String.Concat(x.MATERIAL_ID, x.VKORG, x.VTWEG, x.MATKL_ID, x.MAKTX, x.MATERIAL_GROUP)
                .ToLower().Contains(buscar.ToLower()))
                .ToList();
            }
            switch (colOrden)
            {
                case "MATERIAL_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.materiales = materiales.OrderByDescending(m => m.MATERIAL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.materiales = materiales.OrderBy(m => m.MATERIAL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "VKORG":
                    if (colOrden.Equals(ordenActual))
                        viewModel.materiales = materiales.OrderByDescending(m => m.VKORG).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.materiales = materiales.OrderBy(m => m.VKORG).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "VTWEG":
                    if (colOrden.Equals(ordenActual))
                        viewModel.materiales = materiales.OrderByDescending(m => m.VTWEG).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.materiales = materiales.OrderBy(m => m.VTWEG).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;

                case "MAKTX":
                    if (colOrden.Equals(ordenActual))
                        viewModel.materiales = materiales.OrderByDescending(m => m.MAKTX).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.materiales = materiales.OrderBy(m => m.MAKTX).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "MATKL_ID":
                    if (colOrden.Equals(ordenActual))
                        viewModel.materiales = materiales.OrderByDescending(m => m.MATKL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.materiales = materiales.OrderBy(m => m.MATKL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                case "MATERIAL_GROUP":
                    if (colOrden.Equals(ordenActual))
                        viewModel.materiales = materiales.OrderByDescending(m => m.MATERIAL_GROUP).ToPagedList(pageIndex, viewModel.numRegistros);
                    else
                        viewModel.materiales = materiales.OrderBy(m => m.MATERIAL_GROUP).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
                default:
                    viewModel.materiales = materiales.OrderBy(m => m.MATERIAL_ID).ToPagedList(pageIndex, viewModel.numRegistros);
                    break;
            }
        }

        // GET: Materiales/Details/5

        [LoginActive]
        public ActionResult Details(string id)
        {
            int pagina_id = 662; //ID EN BASE DE DATOS
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATERIAL mATERIAL = db.MATERIALs.Find(id);
            if (mATERIAL == null)
            {
                return HttpNotFound();
            }
            return View(mATERIAL);
        }


        // GET: Materiales/Edit/5
        [LoginActive]
        public ActionResult Edit(string id)
        {
            int pagina_id = 664; //ID EN BASE DE DATOS
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATERIAL mATERIAL = db.MATERIALs.Find(id);
            if (mATERIAL == null)
            {
                return HttpNotFound();
            }
            if (mATERIAL.MATERIALTs.Count > 0)
                foreach (var e in mATERIAL.MATERIALTs)
                {
                    if (e.SPRAS == "EN")
                        ViewBag.EN = e.MAKTX;
                    if (e.SPRAS == "ES")
                        ViewBag.ES = e.MAKTX;
                    if (e.SPRAS == "PT")
                        ViewBag.PT = e.MAKTX;
                }
            else
                ViewBag.EN = mATERIAL.MAKTX;
            return View(mATERIAL);
        }

        // POST: Materiales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoginActive]
        public ActionResult Edit([Bind(Include = "ID,MTART,MATKL_ID,MAKTX,MAKTG,MEINS,PUNIT,ACTIVO,CTGR,BRAND,MATERIALGP_ID,EN,ES,PT")] MATERIAL mATERIAL, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                MATERIAL mATERIAL1 = db.MATERIALs.Find(mATERIAL.ID);
                var materialtextos = db.MATERIALTs.Where(t => t.MATERIAL_ID == mATERIAL.ID).ToList();
                db.MATERIALTs.RemoveRange(materialtextos);
                List<MATERIALT> ListmATERIALTs = new List<MATERIALT>();
                if (collection.AllKeys.Contains("EN"))
                {
                    if (!String.IsNullOrEmpty(collection["EN"]))
                    {
                        MATERIALT m = new MATERIALT { SPRAS = "EN", MATERIAL_ID = mATERIAL.ID, MAKTX = collection["EN"], MAKTG = collection["EN"].ToUpper() };
                        ListmATERIALTs.Add(m);
                    }
                        if (mATERIAL1.MAKTX != collection["EN"])
                        {
                            mATERIAL1.MAKTX = collection["EN"];
                            mATERIAL1.MAKTG = Convert.ToString(collection["EN"]).ToUpper();
                        }
                }
                if (collection.AllKeys.Contains("ES")&& !String.IsNullOrEmpty(collection["ES"]))
                    {
                        MATERIALT m = new MATERIALT { SPRAS = "ES", MATERIAL_ID = mATERIAL.ID, MAKTX = collection["ES"], MAKTG = Convert.ToString(collection["ES"]).ToUpper() };
                        ListmATERIALTs.Add(m);
                    }
                if (collection.AllKeys.Contains("PT") && !String.IsNullOrEmpty(collection["PT"]))
                {
                    MATERIALT m = new MATERIALT { SPRAS = "PT", MATERIAL_ID = mATERIAL.ID, MAKTX = collection["PT"], MAKTG = Convert.ToString(collection["PT"]).ToUpper() };
                    ListmATERIALTs.Add(m);
                }
                db.MATERIALTs.AddRange(ListmATERIALTs);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int pagina_id = 664; //ID EN BASE DE DATOS
            FnCommon.ObtenerConfPage(db, pagina_id, User.Identity.Name, this.ControllerContext.Controller);

            return View(mATERIAL);
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
