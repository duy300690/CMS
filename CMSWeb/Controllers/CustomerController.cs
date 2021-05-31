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
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private const int PageSizeDefault = SystemSetting.PageSizeDefault;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customer
        public ActionResult Index(string query,
                                string sortColumnName,
                                string province,
                                string district,
                                int? page,
                                byte? status)
        {
            if (!SessionContext.IsAuthentication().Item1)
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
                    && CMSRepository.Helpers.CheckExistStringInArray(sortArr[0], SortList.customerList)
                    && CMSRepository.Helpers.CheckExistStringInArray(sortArr[1], SortList.sortType))
                {
                    sortColumn = SortList.customerList.FirstOrDefault(p => p.ToLower().Equals(sortArr[0].ToLower()));
                    sortType = sortArr[1];
                }
                else if (sortArr.Length == 1
                        && CMSRepository.Helpers.CheckExistStringInArray(sortArr[0], SortList.customerList))
                    sortColumn = SortList.customerList.FirstOrDefault(p => p.ToLower().Equals(sortArr[0].ToLower()));
            }

            string strLocationJson = Util.Helpers.GetFileJsonLocation();
            var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);

            SelectList selectProvince = new SelectList(dataLocation, "Code", "Name");
            ViewBag.ListProvince = selectProvince;


            var data = _customerService.GetListCustomer(query, province, district, pageIndex, PageSizeDefault, isActive, sortColumn, sortType);
            List<CustomerModel> listCustomer = new List<CustomerModel>();
            foreach (var item in data.Item1)
            {
                listCustomer.Add(new CustomerModel()
                {
                    Id = item.Id,
                    CustomerCard = item.CustomerCard,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = $"{item.FirstName} {item.LastName}",
                    Gender = item.Gender,
                    IdentityCardNumber = item.IdentityCardNumber,
                    Phone = item.Phone,
                    Email = item.Email,
                    Birthday = item.Birthday,
                    FullAddress = item.FullAddress,
                    Province = item.Province,
                    District = item.District,
                    Ward = item.Ward,
                    Address = item.Address,
                    CreateDate = item.CreateDate,
                    CreateBy = item.CreateBy,
                    ModifiedDate = item.ModifiedDate,
                    ModifiedBy = item.ModifiedBy,
                    Status = item.Status
                });
            }

            IPagedList customerPagedList = listCustomer.ToPagedList(pageIndex - 1, PageSizeDefault, data.Item2);

            sortType = sortType.Equals(SortList.sortType[0].ToLower())
                                        ? SortList.sortType[1].ToLower()
                                        : SortList.sortType[0].ToLower();

            ViewBag.CurrentSortField = sortColumn;
            ViewBag.CurrentSortParam = sortColumnName;
            ViewBag.ProvinceField = province;
            ViewBag.SortAsc = sortType;

            // Search paging
            ViewBag.Query = query;
            ViewBag.Active = status != null ? status.Value.ToString() : "";

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CustomerGrid", customerPagedList);
            }
            return View(customerPagedList);
        }

        public ActionResult Create()
        {
            // Ajax create
            if (Request.IsAjaxRequest())
            {
                string strLocationJson = Util.Helpers.GetFileJsonLocation();
                var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);
                SelectList selectProvince = new SelectList(dataLocation, "Code", "Name");
                ViewBag.ListProvince = selectProvince;

                CustomerModel model = new CustomerModel();
                return PartialView("_CreatePartial", model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(CustomerModel model)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (model.Birthday != null && Util.Helpers.GetAge(model.Birthday.Value) < SystemSetting.DefaultAge)
                    statusMessage = EmployeeResource.ErrorEmployeeAge;

                if (_customerService.IsExistEmail(model.Email))
                    statusMessage = EmployeeResource.ErrorDuplicateEmail;

                if (_customerService.IsExistIdentityCode(model.IdentityCardNumber))
                    statusMessage = EmployeeResource.ErrorDuplicateIdentity;

                if (!string.IsNullOrEmpty(statusMessage))
                    return Json(XUtil.JsonDie(statusMessage, statusCode));

                // Get Full address
                string strLocationJson = Util.Helpers.GetFileJsonLocation();
                var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);

                var province = dataLocation.FirstOrDefault(x => x.Code.Equals(model.Province));
                string provinceName = province.Name;

                var district = province.Districts.FirstOrDefault(x => x.Id.Equals(model.District));
                string districtName = district.Name;

                var ward = district.Wards.FirstOrDefault(x => x.Id.Equals(model.Ward));
                string wardName = ward.Name;

                string fullAddress = $"{model.Address}; {wardName}; {districtName}; {provinceName}";

                CMSService.Query.CustomerInfo employee = new CMSService.Query.CustomerInfo(
                                                  model.Id,
                                                  "CS" + Util.Helpers.ConvertToTimestamp(DateTime.Now),
                                                  model.FirstName,
                                                  model.LastName,
                                                  model.Gender,
                                                  model.IdentityCardNumber,
                                                  model.Phone,
                                                  model.Email,
                                                  model.Birthday,
                                                  model.Province,
                                                  model.District,
                                                  model.Ward,
                                                  model.Address,
                                                  fullAddress,
                                                  model.CreateDate,
                                                  model.CreateBy,
                                                  model.ModifiedDate,
                                                  model.ModifiedBy,
                                                  true
                    );
                int customerId = _customerService.Create(employee, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#custmer" + customerId;

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
                return RedirectToAction("Index", "Customer");
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

                if (id < 1) return Json(XUtil.JsonDie(CustomerResource.CustomertNotFound, statusCode));

                var userInfo = _customerService.GetById(id, true);
                if (userInfo == null) return Json(XUtil.JsonDie(EmployeeResource.EmployeeNotFound, statusCode));

                _customerService.DeActive(id, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#customer" + id;

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
                return RedirectToAction("Index", "Customer");
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

                if (id < 1) return Json(XUtil.JsonDie(CustomerResource.CustomertNotFound, statusCode));

                var userInfo = _customerService.GetById(id, false);
                if (userInfo == null) return Json(XUtil.JsonDie(EmployeeResource.EmployeeNotFound, statusCode));

                _customerService.Active(id, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#customer" + id;

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
                    var customer = _customerService.GetById(id, null);
                    if (customer == null)
                        return HttpNotFound();

                    CustomerModel model = new CustomerModel()
                    {
                        Id = customer.Id,
                        CustomerCard = customer.CustomerCard,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        FullName = $"{customer.FirstName} {customer.LastName}",
                        Gender = customer.Gender,
                        IdentityCardNumber = customer.IdentityCardNumber,
                        Phone = customer.Phone,
                        Email = customer.Email,
                        Birthday = customer.Birthday,
                        FullAddress = customer.FullAddress,
                        Province = customer.Province,
                        District = customer.District,
                        Ward = customer.Ward,
                        Address = customer.Address,
                        CreateDate = customer.CreateDate,
                        CreateBy = customer.CreateBy,
                        ModifiedDate = customer.ModifiedDate,
                        ModifiedBy = customer.ModifiedBy,
                        Status = customer.Status
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
                return RedirectToAction("Index", "Customer");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerModel model)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (model.Birthday != null && Util.Helpers.GetAge(model.Birthday.Value) < SystemSetting.DefaultAge)
                    statusMessage = EmployeeResource.ErrorEmployeeAge;

                //if (_customerService.IsExistEmail(model.Email))
                //    statusMessage = EmployeeResource.ErrorDuplicateEmail;

                //if (_customerService.IsExistIdentityCode(model.IdentityCardNumber))
                //    statusMessage = EmployeeResource.ErrorDuplicateIdentity;

                var customer = _customerService.GetById(model.Id, null);
                if (customer == null)
                    statusMessage = EmployeeResource.EmployeeNotFound;

                if (!string.IsNullOrEmpty(statusMessage))
                    return Json(XUtil.JsonDie(statusMessage, statusCode));

                // Get Full address
                string strLocationJson = Util.Helpers.GetFileJsonLocation();
                var dataLocation = Util.Helpers.ConvertJsonToObject<CityModel>(strLocationJson);

                var province = dataLocation.FirstOrDefault(x => x.Code.Equals(model.Province));
                string provinceName = province.Name;

                var district = province.Districts.FirstOrDefault(x => x.Id.Equals(model.District));
                string districtName = district.Name;

                var ward = district.Wards.FirstOrDefault(x => x.Id.Equals(model.Ward));
                string wardName = ward.Name;

                string fullAddress = $"{model.Address}; {wardName}; {districtName}; {provinceName}";

                CMSService.Query.CustomerInfo saveData = new CMSService.Query.CustomerInfo(
                                                  model.Id,
                                                  model.CustomerCard,
                                                  model.FirstName,
                                                  model.LastName,
                                                  model.Gender,
                                                  model.IdentityCardNumber,
                                                  model.Phone,
                                                  model.Email,
                                                  model.Birthday,
                                                  model.Province,
                                                  model.District,
                                                  model.Ward,
                                                  model.Address,
                                                  fullAddress,
                                                  model.CreateDate,
                                                  model.CreateBy,
                                                  model.ModifiedDate,
                                                  model.ModifiedBy,
                                                  customer.Status
                    );
                _customerService.Edit(saveData, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#customer" + model.Id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }
    }
}