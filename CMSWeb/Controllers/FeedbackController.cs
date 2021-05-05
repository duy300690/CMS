using CMSRepository;
using CMSService;
using CMSWeb.Models;
using CMSWeb.Util;
using EmssWeb.Util;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CMSWeb.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ICustomerService _customerService;

        private const int PageSizeDefault = SystemSetting.PageSizeDefault;
        private string tempFolder = SystemSetting.TempFolder;

        public FeedbackController(IFeedbackService feedbackService
                                  , ICustomerService customerService)
        {
            _feedbackService = feedbackService;
            _customerService = customerService;
        }

        // GET: Feedback
        public ActionResult Index(string query,
                                string sortColumnName,
                                int? page,
                                string fromtDate,
                                string toDate,
                                byte? status)
        {
            if (!SessionContext.IsAuthentication().Item1
                || SessionContext.IsAuthentication().Item2 != CMSService.Secure.Roles.ADMIN)
                return RedirectToAction("Index", "Login");

            int pageIndex = page ?? 1;

            pageIndex = pageIndex < 2 ? 1 : pageIndex;

            // Default sort by Id desc                
            string sortColumn = SortList.feedbackList[0]
                   , sortType = SortList.sortType[1];

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.Split('_').Length <= 2)
            {
                sortColumnName = sortColumnName.ToLower();
                string[] sortArr = sortColumnName.Split('_');

                if (sortArr.Length == 2
                    && CMSRepository.Helpers.CheckExistStringInArray(sortArr[0], SortList.feedbackList)
                    && CMSRepository.Helpers.CheckExistStringInArray(sortArr[1], SortList.sortType))
                {
                    sortColumn = SortList.feedbackList.FirstOrDefault(p => p.ToLower().Equals(sortArr[0].ToLower()));
                    sortType = sortArr[1];
                }
                else if (sortArr.Length == 1
                        && CMSRepository.Helpers.CheckExistStringInArray(sortArr[0], SortList.feedbackList))
                    sortColumn = SortList.feedbackList.FirstOrDefault(p => p.ToLower().Equals(sortArr[0].ToLower()));
            }
            DateTime? from = Util.Helpers.ConvertStringToDate(fromtDate);
            DateTime? to = Util.Helpers.ConvertStringToDate(toDate);

            var data = _feedbackService.GetList(query, pageIndex, PageSizeDefault, from, to, status, sortColumn, sortType);
            List<FeedbackModel> listFeedback = new List<FeedbackModel>();

            foreach (var item in data.Item1)
            {
                listFeedback.Add(new FeedbackModel()
                {
                    Id = item.Id,
                    CustomerId = item.CustomerId,
                    Title = item.Title,
                    Content = item.Content,
                    CreateBy = item.CreateBy,
                    CreateDate = item.CreateDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    Status = item.Status
                });
            }

            IPagedList feedbackPagedList = listFeedback.ToPagedList(pageIndex - 1, PageSizeDefault, data.Item2);

            sortType = sortType.Equals(SortList.sortType[0].ToLower())
                                        ? SortList.sortType[1].ToLower()
                                        : SortList.sortType[0].ToLower();

            ViewBag.CurrentSortField = sortColumn;
            ViewBag.CurrentSortParam = sortColumnName;

            ViewBag.SortAsc = sortType;

            // Search paging
            ViewBag.Query = query;
            ViewBag.Active = status.HasValue ? status.Value.ToString() : "";
            ViewBag.FromDate = from.HasValue ? from.Value.ToString() : "";
            ViewBag.ToDate = to.HasValue ? to.Value.ToString() : "";

            if (Request.IsAjaxRequest())
            {
                return PartialView("_FeedbackGrid", feedbackPagedList);
            }
            return View(feedbackPagedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile()
        {
            int statusCode = 0;
            string statusMessage = string.Empty;

            foreach (string file in Request.Files)
            {
                var FileDataContent = Request.Files[file];
                if (FileDataContent != null && FileDataContent.ContentLength > 0)
                {
                    // take the input stream, and save it to a temp folder using the original file.part name posted
                    var stream = FileDataContent.InputStream;
                    var fileName = Path.GetFileName(FileDataContent.FileName);
                    var UploadPath = Server.MapPath(tempFolder);
                    Directory.CreateDirectory(UploadPath);
                    string path = Path.Combine(UploadPath, fileName);
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        // Once the file part is saved, see if we have enough to merge it
                        AttachmentHelper UT = new AttachmentHelper();
                        UT.MergeFile(path);

                        statusCode = 1;
                        statusMessage = FileDataContent.FileName;
                    }
                    catch (IOException ex)
                    {
                        return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
                    }
                }
            }

            return Json(XUtil.JsonDie(statusMessage, statusCode));
        }

        public ActionResult Create()
        {
            // Ajax create
            if (Request.IsAjaxRequest())
            {
                return PartialView("_CreatePartial");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CheckCustomerCode(string code)
        {
            var model = _customerService.GetByCustomerCard(code, null);
            if (model == null)
                return Json(XUtil.JsonDie("", 0));

            model.SetFullName();
            string jsonObj = XUtil.Object2Json(model);

            return Json(XUtil.JsonDie(jsonObj, 1));
        }
    }
}