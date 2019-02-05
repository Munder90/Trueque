using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAT001.Common;
using TAT001.Entities;
using TAT001.Filters;
using TAT001.Models;

namespace TAT001.Controllers.Catalogos
{
    [Authorize]
    [LoginActive]
    public class ContactoController : Controller
    {
        // GET: Contacto
        public ActionResult Index(string vko, string vtw, string kun, string spa)
        {
            int pagina = 641; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                FnCommon.ObtenerConfPage(db,pagina,u, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }

                if (vko != null && vtw != null && kun != null && spa != null)
                {
                    var con = db.CONTACTOCs
                            .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ACTIVO == true).ToList();
                    CLIENTE cli = db.CLIENTEs.Find(vko, vtw, spa, kun);
                    Contactoc conCli = new Contactoc
                    {
                        vkorg = vko,
                        vtweg = vtw,
                        kunnr = kun,
                        spart = spa,
                        tabContacto = con
                    };
                    ViewBag.Cliente = cli.NAME1;
                    ViewBag.NoCliente = cli.KUNNR;
                    return View(conCli);
                }
                else
                {
                    var con = db.CONTACTOCs.Where(x => x.ACTIVO == true).ToList();

                    Contactoc conCli = new Contactoc
                    {
                        vkorg = vko,
                        vtweg = vtw,
                        kunnr = kun,
                        spart = spa,
                        tabContacto = con
                    };

                    return View(conCli);
                }
            }
        }

        [HttpPost]
        public ActionResult Index(string vko, string vtw, string kun, string spa, string aa)
        {
            int pagina = 641; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }

                var con = db.CONTACTOCs
                            .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ACTIVO == true).ToList();

                Contactoc conCli = new Contactoc
                {
                    kunnr = kun,
                    vkorg = vko,
                    vtweg = vtw,
                    spart = spa,
                    tabContacto = con
                };

                return View(conCli);
            }
        }
        // GET: Contacto/Details/5
        public ActionResult Details(string vko, string vtw, string kun, string spa, int id)
        {
            int pagina = 642; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }

                var cli = db.CONTACTOCs
                          .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ID == id)
                          .First();

                Contactoc cl = new Contactoc
                {
                    vkorg = cli.VKORG,
                    vtweg = cli.VTWEG,
                    kunnr = cli.KUNNR,
                    spart = cli.SPART,
                    id = cli.ID.ToString(),
                    nombre = cli.NOMBRE,
                    telefono = cli.PHONE,
                    correo = cli.EMAIL,
                    defecto = Convert.ToBoolean(cli.DEFECTO),
                    carta = Convert.ToBoolean(cli.CARTA)
                };
                CLIENTE cliente = db.CLIENTEs.Find(vko, vtw, spa, kun);
                ViewBag.Cliente = cliente.NAME1;
                ViewBag.NoCliente = cliente.KUNNR;
                return View(cl);
            }
        }

      
        // POST: Contacto/Create
        [HttpGet]
        public ActionResult Create(string vko, string vtw, string kun, string spa)
        {
            int pagina = 643; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }

                Contactoc conC = new Contactoc
                {
                    vkorg = vko,
                    vtweg = vtw,
                    kunnr = kun,
                    spart = spa
                };
                CLIENTE cliente = db.CLIENTEs.Find(vko, vtw, spa, kun);
                ViewBag.Cliente = cliente.NAME1;
                ViewBag.NoCliente = cliente.KUNNR;
                return View(conC);
            }
        }

        [HttpPost]
        public ActionResult Create2(string vko, string vtw, string kun, string spa, Contactoc conC)
        {
            using (TruequeEntities db = new TruequeEntities())
            {

                if (conC.defecto)
                {
                    var conAct = db.CONTACTOCs
                                   .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.DEFECTO == true).ToList();

                    foreach (var item in conAct)
                    {
                        item.DEFECTO = false;
                    }

                    db.SaveChanges();

                    var con = db.CONTACTOCs
                            .Add(new CONTACTOC { NOMBRE = conC.nombre, PHONE = conC.telefono, EMAIL = conC.correo, VKORG = vko, VTWEG = vtw, KUNNR = kun, SPART = spa, ACTIVO = true, DEFECTO = conC.defecto, CARTA = conC.carta });
                    db.SaveChanges();
                }
                else
                {
                    var con = db.CONTACTOCs
                            .Add(new CONTACTOC { NOMBRE = conC.nombre, PHONE = conC.telefono, EMAIL = conC.correo, VKORG = vko, VTWEG = vtw, KUNNR = kun, SPART = spa, ACTIVO = true, DEFECTO = conC.defecto, CARTA = conC.carta });
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { vko = vko, vtw = vtw, kun = kun, spa = spa });
            }
        }

        // GET: Contacto/Edit/5
        public ActionResult Edit(string vko, string vtw, string kun, string spa, int id)
        {
            int pagina = 644; //ID EN BASE DE DATOS
            using (TruequeEntities db = new TruequeEntities())
            {
                string u = User.Identity.Name;
                FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //return RedirectToAction("Pais", "Home");
                }

                var con = db.CONTACTOCs
                          .Where(x => x.VKORG == vko && x.VTWEG == vtw && x.KUNNR == kun && x.SPART == spa && x.ID == id)
                          .First();

                Contactoc co = new Contactoc
                {
                    vkorg = con.VKORG,
                    vtweg = con.VTWEG,
                    kunnr = con.KUNNR,
                    spart = con.SPART,
                    id = con.ID.ToString(),
                    nombre = con.NOMBRE,
                    telefono = con.PHONE,
                    correo = con.EMAIL,
                    defecto = Convert.ToBoolean(con.DEFECTO),
                    carta = Convert.ToBoolean(con.CARTA)
                };
                CLIENTE cli = db.CLIENTEs.Find(vko, vtw, spa, kun);
                ViewBag.Cliente = cli.NAME1;
                ViewBag.NoCliente = cli.KUNNR;
                return View(co);
            }
        }

        // POST: Contacto/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Contactoc co)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                try
                {
                    var idCli = db.CONTACTOCs.Where(x => x.ID == id).First();

                    if (co.defecto )
                    {
                        var conAct = db.CONTACTOCs
                                       .Where(x => x.VKORG == idCli.VKORG && x.VTWEG == idCli.VTWEG && x.KUNNR == idCli.KUNNR && x.SPART == idCli.SPART && x.DEFECTO == true).ToList();

                        foreach (var item in conAct)
                        {
                            item.DEFECTO = false;
                        }

                        db.SaveChanges();

                        var con = db.CONTACTOCs.Where(x => x.ID == id).First();
                        con.NOMBRE = co.nombre;
                        con.PHONE = co.telefono;
                        con.EMAIL = co.correo;
                        con.DEFECTO = co.defecto;
                        con.CARTA = co.carta; 
                        db.SaveChanges();
                    }
                    else
                    {
                        var con = db.CONTACTOCs.Where(x => x.ID == id).First();
                        con.NOMBRE = co.nombre;
                        con.PHONE = co.telefono;
                        con.EMAIL = co.correo;
                        con.DEFECTO = co.defecto;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index", new { vko = idCli.VKORG, vtw = idCli.VTWEG, kun = idCli.KUNNR, spa = idCli.SPART});
                }
                catch
                {
                    int pagina = 644; //ID EN BASE DE DATOS
                    string u = User.Identity.Name;
                    FnCommon.ObtenerConfPage(db, pagina, u, this.ControllerContext.Controller);
                    return View(co);
                }
            }
        }

        // GET: Contacto/Delete/5
        public ActionResult Delete(int id)
        {
            using (TruequeEntities db = new TruequeEntities())
            {
                var con = db.CONTACTOCs.Where(x => x.ID == id).First();
                con.ACTIVO = false;
                db.SaveChanges();
                return RedirectToAction("Index", new { vko = con.VKORG, vtw = con.VTWEG, kun = con.KUNNR, spa = con.SPART });
            }
        }
    }
}
