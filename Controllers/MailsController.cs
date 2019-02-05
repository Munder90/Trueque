using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using TAT001.Entities;

namespace TAT001.Controllers
{
    public class MailsController : Controller
    {
        private TruequeEntities db = new TruequeEntities();
        // GET: Mails
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Solicitud(decimal id, string spras)
        {

            //int pagina = 203; //ID EN BASE DE DATOS
            ViewBag.Title = "Solicitud";

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                    & a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                    & a.SPART.Equals(dOCUMENTO.SPART)
                                                    & a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
            ViewBag.workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            ViewBag.acciones = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P") & a.USUARIOA_ID.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            return View(dOCUMENTO);
        }

        public ActionResult Solicitudes(decimal id, string spras)
        {

            //int pagina = 203; //ID EN BASE DE DATOS
            ViewBag.Title = "Solicitud";

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                    & a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                    & a.SPART.Equals(dOCUMENTO.SPART)
                                                    & a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
            ViewBag.workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderBy(a => a.POS).ToList();
            ViewBag.acciones = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P") & a.USUARIOA_ID.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.url = "http://localhost:64497";
            ViewBag.url = "http://192.168.1.77";
            ViewBag.url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "");
            return View(dOCUMENTO);
        }

        public ActionResult Enviar(decimal id, bool index, string tipo, string spras)
        {
            //int pagina = 203; //ID EN BASE DE DATOS
            ViewBag.Title = "Solicitud";

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
            if (dOCUMENTO == null)
            {
                return HttpNotFound();
            }
            dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                    & a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                    & a.SPART.Equals(dOCUMENTO.SPART)
                                                    & a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();
            var workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();
            //ViewBag.acciones = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id) & a.ESTATUS.Equals("P") & a.USUARIOA_ID.Equals(User.Identity.Name)).FirstOrDefault();
            
            string mailt = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mail")).FirstOrDefault().VALUE;
            string mtest = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailTest")).FirstOrDefault().VALUE;
            string mailTo = "";
            if (mtest == "X")
                mailTo = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailAdmin")).FirstOrDefault().VALUE;
            else
                mailTo = workflow.USUARIO.EMAIL;

            CONMAIL conmail = db.CONMAILs.Find(mailt);
            if (conmail != null)
            {
                MailMessage mail = new MailMessage(conmail.MAIL, mailTo);
                SmtpClient client = new SmtpClient();
                if (conmail.SSL)
                {
                    client.Port = (int)conmail.PORT;
                    client.EnableSsl = conmail.SSL;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                }
                else
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = conmail.HOST;


                if (workflow == null)
                    mail.Subject = "N" + dOCUMENTO.NUM_DOC + "-" + DateTime.Now.ToShortTimeString();
                else
                    mail.Subject = workflow.ESTATUS + dOCUMENTO.NUM_DOC + "-" + DateTime.Now.ToShortTimeString();
                mail.IsBodyHtml = true;
                string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                //UrlDirectory = UrlDirectory.Substring(0, UrlDirectory.LastIndexOf("/"));
                if (tipo == "R")
                    UrlDirectory = UrlDirectory.Replace("/Mails/Enviar", "/Correos/Details");
                if (tipo == "A")
                    UrlDirectory = UrlDirectory.Replace("/Mails/Enviar", "/Correos/Index");
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(UrlDirectory);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

                mail.Body = result;

                client.Send(mail);

            }
            if (index)
                return RedirectToAction("Index", "Solicitudes", new { id = id, spras = spras });
            else
                return RedirectToAction("Details", "Solicitudes", new { id = id, spras = spras });


            //return View(dOCUMENTO);
        }
    }
}