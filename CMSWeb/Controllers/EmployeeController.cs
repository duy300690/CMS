using CMSRepository;
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
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private const int PageSizeDefault = SystemSetting.PageSizeDefault;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Employee
        public ActionResult Index(string query,
                                string sortColumnName,
                                int? page,
                                byte? status
                                )
        {
            if (!SessionContext.IsAuthentication().Item1
                || SessionContext.IsAuthentication().Item2 != CMSService.Secure.Roles.ADMIN)
                return RedirectToAction("Index", "Login");

            int pageIndex = page ?? 1;
            bool? isActive = null;

            if (status != null)
                isActive = Convert.ToBoolean(status);

            pageIndex = pageIndex < 2 ? 1 : pageIndex;

            // Default sort by Id desc                
            string sortColumn = SortList.employeeList[0]
                   , sortType = SortList.sortType[1];

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.Split('_').Length <= 2)
            {
                sortColumnName = sortColumnName.ToLower();
                string[] sortArr = sortColumnName.Split('_');

                if (sortArr.Length == 2
                    && CMSRepository.Helpers.CheckExistStringInArray(sortArr[0], SortList.employeeList)
                    && CMSRepository.Helpers.CheckExistStringInArray(sortArr[1], SortList.sortType))
                {
                    sortColumn = SortList.employeeList.FirstOrDefault(p => p.ToLower().Equals(sortArr[0].ToLower()));
                    sortType = sortArr[1];
                }
                else if (sortArr.Length == 1
                        && CMSRepository.Helpers.CheckExistStringInArray(sortArr[0], SortList.employeeList))
                    sortColumn = SortList.employeeList.FirstOrDefault(p => p.ToLower().Equals(sortArr[0].ToLower()));
            }

            var data = _employeeService.GetListEmployee(query, pageIndex, PageSizeDefault, isActive, sortColumn, sortType);

            List<EmployeeModel> listEmployee = new List<EmployeeModel>();

            foreach (var item in data.Item1)
            {
                listEmployee.Add(new EmployeeModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = $"{item.FirstName} {item.LastName}",
                    Avatar = item.Avatar,
                    IdentityCardNumber = item.IdentityCardNumber,
                    Gender = item.Gender,
                    Email = item.Email,
                    Phone = item.Phone,
                    Birthday = item.Birthday,
                    Province = item.Province,
                    District = item.District,
                    Ward = item.Ward,
                    Address = item.Address,
                    CreateBy = item.CreateBy,
                    CreateDate = item.CreateDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    Status = item.Status
                });
            }
            IPagedList userPagedList = listEmployee.ToPagedList(pageIndex - 1, PageSizeDefault, data.Item2);

            sortType = sortType.Equals(SortList.sortType[0].ToLower())
                                        ? SortList.sortType[1].ToLower()
                                        : SortList.sortType[0].ToLower();

            ViewBag.CurrentSortField = sortColumn;
            ViewBag.CurrentSortParam = sortColumnName;

            ViewBag.SortAsc = sortType;

            // Search paging
            ViewBag.Query = query;
            ViewBag.Active = status != null ? status.Value.ToString() : "";

            if (Request.IsAjaxRequest())
            {
                return PartialView("_EmployeeGrid", userPagedList);
            }
            return View(userPagedList);
        }

        public ActionResult Create()
        {
            // Ajax create
            if (Request.IsAjaxRequest())
            {
                EmployeeModel model = new EmployeeModel();

                string strLocationJson = Util.Helpers.GetFileJsonLocation();
                var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);

                SelectList selectProvince = new SelectList(dataLocation, "Code", "Name");

                ViewBag.ListProvince = selectProvince;
                return PartialView("_CreatePartial", model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetDistrict(string provinceCode)
        {
            string strLocationJson = Util.Helpers.GetFileJsonLocation();
            var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);

            var district = dataLocation.Where(p => p.Code.Equals(provinceCode))
                                        .Select(x => x.Districts);

            string jsonDistrict = XUtil.Object2Json(district);

            return Json(XUtil.JsonDie(jsonDistrict, 1));
        }

        [HttpPost]
        public JsonResult GetWard(string provinceCode, string districtCode)
        {
            string strLocationJson = Util.Helpers.GetFileJsonLocation();
            var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);

            var district = dataLocation.FirstOrDefault(p => p.Code.Equals(provinceCode)).Districts;

            var ward = district.FirstOrDefault(p => p.Id.Equals(districtCode)).Wards;
            string jsonWard = XUtil.Object2Json(ward);
            return Json(XUtil.JsonDie(jsonWard, 1));
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeModel model)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (model.Birthday != null && Util.Helpers.GetAge(model.Birthday.Value) < SystemSetting.DefaultAge)
                    statusMessage = EmployeeResource.ErrorEmployeeAge;

                if (_employeeService.IsExistEmail(model.Email))
                    statusMessage = EmployeeResource.ErrorDuplicateEmail;

                if (_employeeService.IsExistIdentityCode(model.IdentityCardNumber))
                    statusMessage = EmployeeResource.ErrorDuplicateIdentity;

                if (!string.IsNullOrEmpty(statusMessage))
                    return Json(XUtil.JsonDie(statusMessage, statusCode));

                CMSService.Query.EmployeeInfo employee = new CMSService.Query.EmployeeInfo(
                                                    model.Id,
                                                    model.FirstName,
                                                    model.LastName,
                                                    model.Avatar,
                                                    model.IdentityCardNumber,
                                                    model.Gender,
                                                    model.Email,
                                                    model.Phone,
                                                    model.Birthday,
                                                    model.Province,
                                                    model.District,
                                                    model.Ward,
                                                    model.Address,
                                                    model.CreateDate,
                                                    model.CreateBy,
                                                    model.ModifiedDate,
                                                    model.ModifiedBy,
                                                    true
                    );
                int employeeId = _employeeService.CreateEmployee(employee, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#employee" + employeeId;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }

        public ActionResult Enable(int id)
        {
            if (SessionContext.IsAuthentication().Item1)
            {
                if (Request.IsAjaxRequest())
                {
                    ViewBag.EmployeeId = id;
                    return PartialView("_EnablePartial");
                }
                return RedirectToAction("Index", "Employee");
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

                if (id < 1) return Json(XUtil.JsonDie(EmployeeResource.EmployeeNotFound, statusCode));

                var userInfo = _employeeService.GetEmployeeById(id, false);
                if (userInfo == null) return Json(XUtil.JsonDie(EmployeeResource.EmployeeNotFound, statusCode));

                _employeeService.Active(id, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#employee" + id;

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
                    ViewBag.EmployeeId = id;
                    return PartialView("_DisablePartial");
                }
                return RedirectToAction("Index", "Employee");
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

                if (id < 1) return Json(XUtil.JsonDie(EmployeeResource.EmployeeNotFound, statusCode));

                var userInfo = _employeeService.GetEmployeeById(id, true);
                if (userInfo == null) return Json(XUtil.JsonDie(EmployeeResource.EmployeeNotFound, statusCode));

                _employeeService.DeActive(id, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#employee" + id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }

        public ActionResult Edit(int id)
        {
            if (SessionContext.IsAuthentication().Item1)
            {
                if (Request.IsAjaxRequest())
                {
                    var employee = _employeeService.GetEmployeeById(id, null);
                    if (employee == null)
                        return HttpNotFound();

                    EmployeeModel model = new EmployeeModel()
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Avatar = string.IsNullOrEmpty(employee.Avatar) ? "/img/undraw_profile.svg" : employee.Avatar,
                        IdentityCardNumber = employee.IdentityCardNumber,
                        Gender = employee.Gender,
                        Email = employee.Email,
                        Phone = employee.Phone,
                        Birthday = employee.Birthday,
                        Province = employee.Province,
                        District = employee.District,
                        Ward = employee.Ward,
                        Address = employee.Address,
                        CreateBy = employee.CreateBy,
                        CreateDate = employee.CreateDate,
                        ModifiedBy = employee.ModifiedBy,
                        ModifiedDate = employee.ModifiedDate,
                        Status = employee.Status
                    };

                    string strLocationJson = Util.Helpers.GetFileJsonLocation();
                    var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);
                    SelectList selectProvince = new SelectList(dataLocation, "Code", "Name", model.Province);
                    ViewBag.ListProvince = selectProvince;

                    List<District> listDistrict = new List<District>();
                    var dataProvince = dataLocation.FirstOrDefault(x => x.Code.Equals(model.Province));
                    if (dataProvince != null)
                    {
                        listDistrict = dataProvince.Districts;
                        SelectList selectDistrict = new SelectList(listDistrict, "Id", "Name", model.District);
                        ViewBag.ListDistrict = selectDistrict;
                    }

                    List<Ward> listWard = new List<Ward>();
                    var dataWard = listDistrict.FirstOrDefault(x => x.Id.Equals(model.District));
                    if (dataWard != null)
                    {
                        listWard = dataWard.Wards;
                        SelectList selecWard = new SelectList(listWard, "Id", "Name", model.Ward);
                        ViewBag.ListWard = selecWard;
                    }

                    return PartialView("_EditPartial", model);
                }
                return RedirectToAction("Index", "Employee");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeModel model)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (model.Birthday != null && Util.Helpers.GetAge(model.Birthday.Value) < SystemSetting.DefaultAge)
                    statusMessage = EmployeeResource.ErrorEmployeeAge;

                //if (_employeeService.IsExistEmail(model.Email))
                //    statusMessage = EmployeeResource.ErrorDuplicateEmail;

                //if (_employeeService.IsExistIdentityCode(model.IdentityCardNumber))
                //    statusMessage = EmployeeResource.ErrorDuplicateIdentity;

                var employee = _employeeService.GetEmployeeById(model.Id, null);
                if (employee == null)
                    statusMessage = EmployeeResource.EmployeeNotFound;

                if (!string.IsNullOrEmpty(statusMessage))
                    return Json(XUtil.JsonDie(statusMessage, statusCode));


                CMSService.Query.EmployeeInfo saveData = new CMSService.Query.EmployeeInfo(
                                                    model.Id,
                                                    model.FirstName,
                                                    model.LastName,
                                                    model.Avatar,
                                                    model.IdentityCardNumber,
                                                    model.Gender,
                                                    model.Email,
                                                    model.Phone,
                                                    model.Birthday,
                                                    model.Province,
                                                    model.District,
                                                    model.Ward,
                                                    model.Address,
                                                    model.CreateDate,
                                                    model.CreateBy,
                                                    model.ModifiedDate,
                                                    model.ModifiedBy,
                                                    employee.Status
                    );
                _employeeService.Edit(saveData, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#employee" + model.Id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }
    }
}