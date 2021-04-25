using CMSService;
using CMSWeb.Language;
using CMSWeb.Models;
using CMSWeb.Util;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CMSWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private const int PageSizeDefault = SystemSetting.PageSizeDefault;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: User
        public ActionResult Index(string query, int? page, byte? status)
        {
            if (!SessionContext.IsAuthentication().Item1
                || SessionContext.IsAuthentication().Item2 != CMSService.Secure.Roles.ADMIN)
                return RedirectToAction("Index", "Login");

            bool? isActive = null;
            int pageIndex = page ?? 1;

            pageIndex = pageIndex < 2 ? 1 : pageIndex;

            if (status != null)
                isActive = Convert.ToBoolean(status);

            var data = _userService.GetListUser(query, pageIndex, PageSizeDefault, isActive);

            List<UserModel> listUser = new List<UserModel>();
            foreach (var user in data.Item1)
            {
                listUser.Add(new UserModel
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
                });
            }
            IPagedList userPagedList = listUser.ToPagedList(pageIndex - 1, PageSizeDefault, data.Item2);

            ViewBag.Query = query;
            ViewBag.Active = status != null ? status.Value.ToString() : "";

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserGrid", userPagedList);
            }
            return View(userPagedList);
        }

        public ActionResult Enable(int id)
        {
            if (SessionContext.IsAuthentication().Item1)
            {
                if (Request.IsAjaxRequest())
                {
                    try
                    {
                        var user = _userService.GetUserById(id, null);

                        UserModel userModel = new UserModel
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
                        ViewBag.UserId = userModel.Id;
                        return PartialView("_EnablePartial", userModel);
                    }
                    catch (Exception ex)
                    {
                        return HttpNotFound(ex.Message);
                    }
                }
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EnablePost(int id)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (id < 1) return Json(XUtil.JsonDie(UserResource.UserNotFound, statusCode));

                var userInfo = _userService.GetUserById(id, false);
                if (userInfo == null) return Json(XUtil.JsonDie(UserResource.UserNotFound, statusCode));

                _userService.Active(id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#user" + id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }

        public ActionResult Disable(int id)
        {
            if (SessionContext.IsAuthentication().Item1)
            {
                if (Request.IsAjaxRequest())
                {
                    try
                    {
                        var user = _userService.GetUserById(id, null);

                        UserModel userModel = new UserModel
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
                        ViewBag.UserId = userModel.Id;
                        return PartialView("_DisablePartial", userModel);
                    }
                    catch (Exception ex)
                    {
                        return HttpNotFound(ex.Message);
                    }
                }
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DisablePost(int id)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (id < 1) return Json(XUtil.JsonDie(UserResource.UserNotFound, statusCode));

                var userInfo = _userService.GetUserById(id, true);
                if (userInfo == null) return Json(XUtil.JsonDie(UserResource.UserNotFound, statusCode));

                _userService.DeActive(id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#user" + id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }

        public ActionResult SetRole(int id)
        {
            if (SessionContext.IsAuthentication().Item1)
            {
                if (Request.IsAjaxRequest())
                {
                    try
                    {
                        var user = _userService.GetUserById(id, null);

                        UserModel userModel = new UserModel
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

                        List<RoleModel> roles = new List<RoleModel>()
                        {
                            new RoleModel()
                            {
                                Id = CMSService.Secure.Roles.ADMIN.ToString(),
                                Name = CMSService.Secure.Roles.ADMIN.ToString()
                            },
                            new RoleModel()
                            {
                                Id = CMSService.Secure.Roles.USER.ToString(),
                                Name = CMSService.Secure.Roles.USER.ToString()
                            }
                        };
                        ViewBag.UserId = userModel.Id;
                        ViewBag.Roles = new SelectList(roles, "Id", "Name", userModel.Role);
                        ViewBag.CurrentRole = userModel.Role.Trim();
                        ViewBag.UserName = userModel.Username;

                        return PartialView("_SetRolePartial", userModel);
                    }
                    catch (Exception ex)
                    {
                        return HttpNotFound(ex.Message);
                    }
                }
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetRole(int id, string role)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (id < 1) return Json(XUtil.JsonDie(UserResource.UserNotFound, statusCode));

                if (string.IsNullOrEmpty(role)) return Json(XUtil.JsonDie(UserResource.InvalidRole, statusCode));

                var userInfo = _userService.GetUserById(id, true);
                if (userInfo == null) return Json(XUtil.JsonDie(UserResource.UserNotFound, statusCode));

                var listRole = Helpers.GetListEnumData<CMSService.Secure.Roles>();

                if (!listRole.Any(x => x.Value.Equals(role)))
                    return Json(XUtil.JsonDie(UserResource.InvalidRole, statusCode));
                _userService.SetRole(id, role);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#user" + id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }
    }
}