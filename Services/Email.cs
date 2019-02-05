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
using TAT001.Common;
using TAT001.Entities;

namespace TAT001.Services
{
    public class Email
    {
        private readonly TruequeEntities db = new TruequeEntities();

        public void enviaMailC(decimal id, bool ban, string spras, string UrlDirectory, string page, string image, string imageFlag, int? pos = null)
        {
            try
            {
                if (id != 0)
                {
                    DOCUMENTO dOCUMENTO = db.DOCUMENTOes.Find(id);
                    if (dOCUMENTO != null)
                    {

                        dOCUMENTO.CLIENTE = db.CLIENTEs.Where(a => a.VKORG.Equals(dOCUMENTO.VKORG)
                                                                && a.VTWEG.Equals(dOCUMENTO.VTWEG)
                                                                && a.SPART.Equals(dOCUMENTO.SPART)
                                                                && a.KUNNR.Equals(dOCUMENTO.PAYER_ID)).First();

                        var workflow = db.FLUJOes.Where(a => a.NUM_DOC.Equals(id)).OrderByDescending(a => a.POS).FirstOrDefault();

                        string mailt = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mail")).FirstOrDefault().VALUE;
                        string mtest = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailTest")).FirstOrDefault().VALUE;
                        string mailTo = "";
                        if (mtest == "X")
                            mailTo = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailAdmin")).FirstOrDefault().VALUE;
                        else
                        {
                            if (pos == null)
                                mailTo = workflow.USUARIO1.EMAIL;
                            else if (pos == 1)
                                mailTo = dOCUMENTO.USUARIO.EMAIL;
                        }
                        CONMAIL conmail = db.CONMAILs.Find(mailt);
                        if (conmail != null)
                        {
                            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(conmail.MAIL, mailTo);
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
                            //mail.Subject += workflow.USUARIOA_ID;
                            if (pos == null)
                                mail.Subject += " " + workflow.USUARIOA_ID;
                            else if (pos == 1)
                                mail.Subject += " " + dOCUMENTO.USUARIOC_ID;

                            UrlDirectory = UrlDirectory.Replace("Solicitudes/Create", "Correos/" + page);
                            UrlDirectory = UrlDirectory.Replace("Solicitudes/Details", "Correos/" + page);
                            UrlDirectory = UrlDirectory.Replace("Solicitudes/Edit", "Correos/" + page);
                            UrlDirectory = UrlDirectory.Replace("Flujos/Procesa", "Correos/" + page);
                            UrlDirectory = UrlDirectory.Replace("Masiva/setDatos", "Correos/" + page);
                            UrlDirectory = UrlDirectory.Replace("Solicitudes/Cancelar", "Correos/" + page);
                            UrlDirectory = UrlDirectory.Replace("Solicitudes/Reversar", "Correos/" + page);
                            //UrlDirectory += "/" + dOCUMENTO.NUM_DOC + "?mail=true"; //B20180803 MGC Correos
                            //UrlDirectory += "/" + dOCUMENTO.NUM_DOC + ""; //B20180803 MGC Correos
                            UrlDirectory += "/" + dOCUMENTO.NUM_DOC + "?spras=" + spras; //B20180803 MGC Correos
                            Log.Info("Intenta generar page " + UrlDirectory);
                            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(UrlDirectory);
                            myRequest.Method = "GET";
                            WebResponse myResponse = myRequest.GetResponse();
                            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                            string result = sr.ReadToEnd();
                            sr.Close();
                            myResponse.Close();

                            //mail.Body = result;//B20180803 MGC Correos

                            mail.AlternateViews.Add(Mail_Body(result, image, imageFlag));//B20180803 MGC Correos
                            mail.IsBodyHtml = true;//B20180803 MGC Correos

                            Log.Info("Intenta enviar email-To:" + mailTo + " " + UrlDirectory);
                            client.Send(mail);

                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("Error al enviar correo:" + e.Message);
                if (e.InnerException != null)
                    Log.Info(e.InnerException.ToString());
                Console.Write("Error al enviar correo:" + e.Message);
            }
        }

        public void enviaMailB(string usu, string usu2, string spras, string UrlDirectory, string page, string image, string imageFlag)
        {
            USUARIO usuInf1 = db.USUARIOs.Where(x => x.ID == usu).FirstOrDefault();
            USUARIO usuInf2 = db.USUARIOs.Where(x => x.ID == usu2).FirstOrDefault();

            try
            {
                string mailt = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mail")).FirstOrDefault().VALUE;
                string mtest = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailTest")).FirstOrDefault().VALUE;
                string mailTo = "";

                if (mtest == "X")
                    mailTo = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailAdmin")).FirstOrDefault().VALUE;
                else
                    mailTo = usuInf2.EMAIL;

                CONMAIL conmail = db.CONMAILs.Find(mailt);

                if (conmail != null)
                {
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(conmail.MAIL, mailTo);
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
                    mail.Subject = "BACKUP" + "-" + usuInf1.NOMBRE + " " + usuInf1.APELLIDO_P + " " + usuInf1.APELLIDO_M + "-" + DateTime.Now.ToShortTimeString();
                    mail.IsBodyHtml = true;
                    UrlDirectory = UrlDirectory.Replace("Usuarios/AddBackup", "Correos/" + page);
                    UrlDirectory += "/?usu=" + usu + "&usu2=" + usu2 + "&spras=" + spras;
                    Log.Info("Intenta generar page " + UrlDirectory);
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(UrlDirectory);
                    myRequest.Method = "GET";
                    WebResponse myResponse = myRequest.GetResponse();
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                    string result = sr.ReadToEnd();
                    sr.Close();
                    myResponse.Close();
                    mail.AlternateViews.Add(Mail_Body(result, image, imageFlag));
                    mail.IsBodyHtml = true;

                    Log.Info("Intenta enviar email-To:" + mailTo + " " + UrlDirectory);
                    client.Send(mail);

                }
            }
            catch (Exception e)
            {
                Log.Info("Error al enviar correo:" + e.Message);
                if (e.InnerException != null)
                    Log.Info(e.InnerException.ToString());
                Console.Write("Error al enviar correo:" + e.Message);
            }
        }

        private AlternateView Mail_Body(string strr, string path, string path2)
        {

            ////string path = "";
            ////path = HttpContext.Current.Server.MapPath("/images/logo_kellogg.png");

            ////string path = "C:/Users/matias/Documents/GitHub/TAT004/TAT001/images/logo_kellogg.png";// HttpContext.Current.Server.MapPath(@"images/6792532.jpg");
            LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            LinkedResource Img2 = new LinkedResource(path2, MediaTypeNames.Image.Jpeg);
            Img.ContentId = "logo_img";
            Img2.ContentId = "flag_img";

            strr = strr.Replace("\"miimg_id\"", "cid:logo_img");
            strr = strr.Replace("\"miflag_id\"", "cid:flag_img");

            AlternateView AV = AlternateView.CreateAlternateViewFromString(strr, null, MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);
            AV.LinkedResources.Add(Img2);
            return AV;
        }

    }
}
