using CMSRepository;
using CMSService;
using CMSWeb.Models;
using CMSWeb.Util;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    IdentityCartNumber = item.IdentityCartNumber,
                    Email = item.Email,
                    Phone = item.Phone,
                    Birthday = item.Birthday,
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
            ViewBag.LocationJson = Util.Helpers.GetFileJsonLocation();

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
                return PartialView("_CreatePartial", model);
            }

            return RedirectToAction("Index");
        }
    }
}