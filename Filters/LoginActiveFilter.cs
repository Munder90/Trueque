using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using TAT001.Entities;

namespace TAT001.Filters
{
    public class LoginActiveAttribute : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (validUserSession(context.HttpContext.User.Identity.Name))
                {
                    // do nothing
                }
                else
                {
                    FormsAuthentication.SignOut();
                    context.Result = new HttpUnauthorizedResult(); // mark unauthorized
                }
            }
            else
            {
                context.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                context.Result = new RedirectToRouteResult("Default",
                new System.Web.Routing.RouteValueDictionary{
                {"controller", "Account"},
                {"action", "Login"},
                {"returnUrl", context.HttpContext.Request.RawUrl}
                });
            }
        }
        public bool validUserSession(string username)
        {
            bool us = false;
            string utest = ConfigurationManager.AppSettings["userTest"];
            if (utest == null)
                utest = "";
            if (utest == "X")
                us = true;

            if (!us)
            {
                using (TruequeEntities db = new TruequeEntities())
                {
                    var existeSession = db.USUARIOLOGs.FirstOrDefault(x => x.USUARIO_ID == username);
                    if (existeSession != null)
                    {
                        if (existeSession != null && HttpContext.Current.Session.SessionID != existeSession.SESION)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else return false;
                }
            }
            return true;
        }
    }
}
