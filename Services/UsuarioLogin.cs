using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TAT001.Entities;

namespace TAT001.Services
{
    public class UsuarioLogin
    {
        readonly TruequeEntities db = new TruequeEntities();

        public bool validaUsuario(string usuario)
        {
            bool us = false;
            string utest = ConfigurationManager.AppSettings["userTest"];
            if (utest == null)
                utest = "";
            if (utest == "X")
                us = true;

            if (!us)
            {
                var existeUsuario = db.USUARIOLOGs.SingleOrDefault(x => x.USUARIO_ID == usuario);

                if (existeUsuario != null && System.Web.HttpContext.Current.Session.SessionID != existeUsuario.SESION)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
    }
}