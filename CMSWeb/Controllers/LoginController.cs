using CMSService;
using CMSWeb.Language;
using CMSWeb.Models;
using CMSWeb.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CMSWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Login        
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            if (SessionContext.IsAuthentication().Item1)
                return Redirect("~/Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModels model, string returnUrl)
        {
            try
            {
                string Password = Helpers.MD5Hash(model.Password);
                var user = _userService.GetUserLogin(model.Username, Password);
                if (user == null)
                {
                    ViewBag.ErrorMessage = Resource.LoginFaiure;
                    return View("Index");
                }

                var userModel = new UserModel
                {
                    Id = user.Id,
                    EmployeeId = user.EmployeeId,
                    EmployeeName = user.EmployeeName,
                    Username = user.Username,
                    CreateDate = user.CreateDate,
                    CreateBy = user.CreateBy,
                    ModifiedDate = user.ModifiedDate,
                    ModifiedBy = user.ModifiedBy,
                    Role = user.Role,
                    Status = user.Status
                };
                SessionContext.SetAuthenticationToken(userModel.Username, true, userModel);

                return Redirect("~/Home");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Login faiure";
                return View("Index");
            }
        }


        // GET: Logout
        public ActionResult Logout()
        {
            SessionContext.RemoveAuthenticationToken();
            return RedirectToAction("Index", "Login");
        }
    }
}