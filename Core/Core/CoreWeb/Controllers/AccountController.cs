using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CoreWeb.Authorization;
using CoreWeb.Models.Account;
using CoreWeb.Controllers.Base;
using CoreAPI.Types;
using CoreAPI.Helpers;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления проверки доступа пользователя в систему
    /// </summary>
    public class AccountController : BaseDBController
    {
        /// <inheritdoc />
        public AccountController(ICoreDBContext db) : base(db) { }
        /// <summary>
        /// Страница проверки доступа пользователя к системе
        /// </summary>
        /// <param name="returnUrl">обратная ссылка</param>
        /// <returns>интерфейс аутентификации пользователя</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Проверка имени пользователя и пароля в Active Directory
        /// </summary>
        /// <param name="model">данные аутентификации пользователя</param>
        /// <param name="returnUrl">обратная ссылка</param>
        /// <returns>результат аутентифкации</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.UserName))
                    {
                        ModelState.AddModelError("", "Введите имя пользователя.");
                    }
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        ModelState.AddModelError("", "Введите пароль.");
                    }
                    return View(model);
                }
                if (model.UserName.Contains("\\"))
                {
                    ModelState.AddModelError("", "Имя пользователя указывается без имени домена.");
                    return View(model);
                }
                var _user = CoreDB.SysUsers.FirstOrDefault(p => p.Alias == model.UserName.ToUpper() && !p.IsAD);
                //Если пользователь найден
                if (_user != null)
                {
                        if (Crypto.VerifyMd5Hash(model.Password, _user.Password))
                        {
                            IPrincipal _Iprincipal = new GenericPrincipal(new GenericIdentity(model.UserName), null);
                            CustomPrincipal _customPrincipal = new CustomPrincipal(CoreDB, _Iprincipal, null);
                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                            this.Session["user"] = HttpContext.User = _customPrincipal;
                            if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                                           && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return this.Redirect(returnUrl);
                            }
                            return this.RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");
                            return View(model);
                        }
                }
                //Если пользователь не найден
                foreach (MembershipProvider Provider in Membership.Providers)
                {
                    if (Provider.ValidateUser(model.UserName, model.Password))
                    {
                        GetCustomPrincipalForms(model, Provider);
                        if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return this.Redirect(returnUrl);
                        }
                        return this.RedirectToAction("Index", "Home");
                    }
                }
                //Если ничего не найдено, то ошибка
                ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        /// <summary>
        /// Выход из системы пользователя
        /// </summary>
        /// <returns>перенаправление на страницу аутентифкации пользователя</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Вспомогательные методы
        private void GetCustomPrincipalForms(LoginModel model, MembershipProvider provider)
        {
            try
            {
                UserPrincipal principal = null;
                string pathLdap = provider.GetType().GetField("adConnectionString", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider).ToString();
                Match r = Regex.Match(pathLdap, @"DC=([a-zA-Z\\]+)");
                string domain = r.Groups[0].Value.Replace("DC=", "");
                using (var context = new PrincipalContext(ContextType.Domain, domain, model.UserName, model.Password))
                {
                    try
                    {
                        principal = UserPrincipal.FindByIdentity(context, model.UserName);
                    }
                    catch { }
                }

                IPrincipal _Iprincipal = new GenericPrincipal(new GenericIdentity(domain + "\\" + model.UserName), null);
                CustomPrincipal _customPrincipal = new CustomPrincipal(CoreDB, _Iprincipal, principal);
                FormsAuthentication.SetAuthCookie(domain + "\\" + model.UserName, model.RememberMe);
                this.Session["user"] = HttpContext.User = _customPrincipal;
            }
            catch
            {
                throw new Exception("Ошибка в получении данных из Active Directory.");
            }
        }
        #endregion
    }
}
