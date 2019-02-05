using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using TAT001.Entities;
using TAT001.Models;

namespace TAT001.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        readonly TruequeEntities db = new TruequeEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                ////ApplicationSignInManager A = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                Session["pais"] = null;
                if (User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                //Si no hay país que borrar
            }

            LoginViewModel m = new LoginViewModel();
            ViewBag.ReturnUrl = returnUrl;
            return View(m);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            USUARIO user = new USUARIO();
            user.ID = model.ID;
            user.PASS = model.Password;

            Cryptography c = new Cryptography();
            string pass = c.Encrypt(user.PASS);

            ////using (TruequeEntities db = new TruequeEntities())
            ////{
                user = db.USUARIOs.Where(a => a.ID.Equals(user.ID) && a.PASS.Equals(pass) && a.ACTIVO == true).FirstOrDefault();
            ////}


            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(model.ID, false);

                ////var authTicket = new FormsAuthenticationTicket(1, user.ID, DateTime.Now, DateTime.Now.AddMinutes(20), false, user.MIEMBROS.FirstOrDefault().ROL.NOMBRE);
                var authTicket = new FormsAuthenticationTicket(1, user.ID.ToUpper(), DateTime.Now, DateTime.Now.AddDays(1), false, "Administrador");
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);
                ////return RedirectToAction("Index", "Home");
                if (returnUrl != null)
                {
                    bool us = false;
                    string utest = ConfigurationManager.AppSettings["userTest"];
                    if (utest == null)
                        utest = "";
                    if (utest == "X")
                        us = true;

                    if (!us)
                    {
                        var checkUser = db.USUARIOLOGs.SingleOrDefault(x => x.USUARIO_ID == user.ID.ToUpper());

                        try
                        {
                            if (checkUser == null)
                            {
                                USUARIOLOG usuLog = new USUARIOLOG();
                                usuLog.USUARIO_ID = user.ID.ToUpper();
                                usuLog.POS = 1;
                                usuLog.SESION = System.Web.HttpContext.Current.Session.SessionID;
                                usuLog.NAVEGADOR = Request.Browser.Type;
                                usuLog.UBICACION = System.Environment.UserName + " - " + RegionInfo.CurrentRegion.DisplayName;
                                usuLog.FECHA = DateTime.Now;
                                usuLog.LOGIN = true;
                                db.USUARIOLOGs.Add(usuLog);
                                db.SaveChanges();
                                Session["userlog"] = usuLog;
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("validateLoginView", new { USUARIO_ID = user.ID.ToUpper(), returnUrl = returnUrl });
                                ////checkUser.USUARIO_ID = user.ID;
                                ////checkUser.POS = 1;
                                ////checkUser.SESION = System.Web.HttpContext.Current.Session.SessionID;
                                ////checkUser.NAVEGADOR = Request.Browser.Type;
                                ////checkUser.UBICACION = RegionInfo.CurrentRegion.DisplayName;
                                ////checkUser.FECHA = DateTime.Now;
                                ////checkUser.LOGIN = true;
                                ////db.SaveChanges();
                                ////Session["userlog"] = checkUser;
                                ////return Redirect(returnUrl);
                            }

                        }
                        catch
                        {
                            //Hay que revisar las posibilidades de error
                        }
                    }

                    USUARIOLOG usuLog2 = new USUARIOLOG();
                    Session["userlog"] = new USUARIOLOG();

                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            else
            {
                ModelState.AddModelError("", "Usuario/contraseña incorrecta.");
                return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            ////AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            try
            {
                Session["pais"] = null;
                USUARIOLOG usu = new USUARIOLOG();

                bool us = false;
                string utest = ConfigurationManager.AppSettings["userTest"];
                if (utest == null)
                    utest = "";
                if (utest == "X")
                    us = true;

                if (!us)
                {
                    usu = (USUARIOLOG)Session["userlog"];
                    if (usu != null)
                    {
                        var checkUser = db.USUARIOLOGs.SingleOrDefault(x => x.USUARIO_ID == usu.USUARIO_ID);
                        if (checkUser != null)
                            if (checkUser.SESION == System.Web.HttpContext.Current.Session.SessionID)
                            {
                                db.Entry(checkUser).State = System.Data.Entity.EntityState.Deleted;
                                db.SaveChanges();
                            }
                    }
                }
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult validateLoginView(string USUARIO_ID, string returnUrl)
        {
            ////int pagina = 221; //ID EN BASE DE DATOS
            string u = User.Identity.Name;
            var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
            ViewBag.usuario = user;
            var checkUser = db.USUARIOLOGs.SingleOrDefault(x => x.USUARIO_ID == USUARIO_ID);
            if (returnUrl == "/Account/validateLoginView")
                returnUrl = "/";
            ViewBag.returnUrl = returnUrl;

            return View(checkUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult validateLoginView([Bind(Include = "USUARIO_ID,POS,SESION,NAVEGADOR,UBICACION,FECHA,LOGIN")] USUARIOLOG uSUARIOLOG, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                uSUARIOLOG.POS = 1;
                uSUARIOLOG.SESION = System.Web.HttpContext.Current.Session.SessionID;
                uSUARIOLOG.NAVEGADOR = Request.Browser.Type;
                ////uSUARIOLOG.UBICACION = System.Environment.MachineName + "/" + System.Environment.UserName + " - " + RegionInfo.CurrentRegion.DisplayName;
                uSUARIOLOG.UBICACION = System.Environment.UserName + " - " + RegionInfo.CurrentRegion.DisplayName;
                uSUARIOLOG.FECHA = DateTime.Now;
                uSUARIOLOG.LOGIN = true;
                try
                {
                    db.Entry(uSUARIOLOG).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["userlog"] = uSUARIOLOG;
                }
                catch {
                    //Tal vez hay error en la conexión
                }
                return Redirect(returnUrl);
            }

            return View();
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        ////[HttpPost]
        ////[AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        ////public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        ////{
        ////    if (!ModelState.IsValid)
        ////    {
        ////        return View(model);
        ////    }

        ////    // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
        ////    // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
        ////    // se bloqueará durante un período de tiempo especificado. 
        ////    // Puede configurar el bloqueo de la cuenta en IdentityConfig
        ////    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        ////    switch (result)
        ////    {
        ////        case SignInStatus.Success:
        ////            return RedirectToLocal(model.ReturnUrl);
        ////        case SignInStatus.LockedOut:
        ////            return View("Lockout");
        ////        case SignInStatus.Failure:
        ////        default:
        ////            ModelState.AddModelError("", "Código no válido.");
        ////            return View(model);
        ////    }
        ////}

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    //// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //// await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                //// string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                //// await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                //// return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        ////[AllowAnonymous]
        ////public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        ////{
        ////    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        ////    if (loginInfo == null)
        ////    {
        ////        return RedirectToAction("Login");
        ////    }

        ////    // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
        ////    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        ////    switch (result)
        ////    {
        ////        case SignInStatus.Success:
        ////            return RedirectToLocal(returnUrl);
        ////        case SignInStatus.LockedOut:
        ////            return View("Lockout");
        ////        case SignInStatus.RequiresVerification:
        ////            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        ////        case SignInStatus.Failure:
        ////        default:
        ////            // Si el usuario no tiene ninguna cuenta, solicitar que cree una
        ////            ViewBag.ReturnUrl = returnUrl;
        ////            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        ////            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        ////    }
        ////}

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}