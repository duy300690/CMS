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
using CMSWeb.Language;

namespace CMSWeb.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ICustomerService _customerService;

        private const int PageSizeDefault = SystemSetting.PageSizeDefault;
        private const string tempFolder = SystemSetting.TempFolder;

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
            if (!SessionContext.IsAuthentication().Item1)
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
                FeedbackModel feedback = new FeedbackModel()
                {
                    Id = item.Id,
                    CustomerId = item.CustomerId,
                    Title = item.Title,
                    Content = item.Content,
                    CreateBy = item.CreateBy,
                    CreateDate = item.CreateDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    Status = item.Status,
                };
                feedback.CustomerMemberCard = item.CustomerMemberCard;
                feedback.CustomerName = item.CustomerName;

                feedback.Attachments = new List<CMSRepository.Query.ViewAttachmentInfo>();
                if (item.Attachments != null && item.Attachments.Any())
                    feedback.Attachments = item.Attachments;

                listFeedback.Add(feedback);
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

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(FeedbackModel model)
        {
            string statusMessage = string.Empty;
            string url = string.Empty;
            int statusCode = 0;

            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.CustomerId.HasValue || model.CustomerId < 1)
                        statusMessage = CustomerResource.CustomertNotFound;

                    if (_customerService.GetById(model.CustomerId.Value, null) == null)
                        statusMessage = CustomerResource.CustomertNotFound;

                    if (!string.IsNullOrEmpty(statusMessage))
                        return Json(XUtil.JsonDie(statusMessage, statusCode));

                    CMSService.Query.FeedbackInfo feedback = new CMSService.Query.FeedbackInfo(
                                                            model.Id
                                                            , model.CustomerId
                                                            , model.Title
                                                            , model.Content
                                                            , model.CreateDate
                                                            , model.CreateBy
                                                            , model.ModifiedDate
                                                            , model.ModifiedBy
                                                            , model.Status
                                                            );

                    int feedbackId = _feedbackService.Save(feedback, SessionContext.GetUserLogin().Id);

                    if (feedbackId == 0)
                        return Json(XUtil.JsonDie(FeedbackResource.FeedbackError, statusCode));

                    //Save attachments
                    List<CMSRepository.Query.AttachmentInfo> attachments = new List<CMSRepository.Query.AttachmentInfo>();
                    if (model.AttachFiles != null && model.AttachFiles.Any())
                    {
                        try
                        {
                            string tmpFolder = tempFolder;
                            var tmpPath = Server.MapPath(tmpFolder);
                            for (int i = 0; i < model.AttachFiles.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(model.AttachFiles[i]))
                                {
                                    string fileName = $"{ model.AttachFiles[i]}";
                                    string path = Path.Combine(tmpPath, fileName);

                                    var iden = Guid.NewGuid();
                                    var attachment = new CMSRepository.Query.AttachmentInfo
                                    {
                                        FeedbackId = feedbackId,
                                        Iden = iden,
                                        Name = Util.Helpers.FormatAttachment(model.AttachFiles[i]),
                                        Created = DateTime.Now,
                                        FileContent = Util.Helpers.ReadFile(path),
                                        MimeType = Path.GetExtension(path),
                                        Id = 0
                                    };

                                    attachments.Add(attachment);

                                    Util.Helpers.DeleteTemporaryAttachmentsInBackgroundThread(tmpPath, fileName);
                                }
                            }
                            _feedbackService.SaveListAttachment(attachments);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }

                    statusCode = 1;
                    statusMessage = Request.Url.AbsolutePath + "#feedback" + feedbackId;

                    return Json(XUtil.JsonDie(statusMessage, statusCode));
                }
                else
                {
                    return Json(XUtil.JsonDie(FeedbackResource.FeedbackError, statusCode));
                }
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }

        public ActionResult Detail(int id)
        {
            // Ajax detail
            if (Request.IsAjaxRequest())
            {
                var data = _feedbackService.GetById(id, null);
                FeedbackModel feedback = new FeedbackModel()
                {
                    Id = data.Id,
                    CustomerId = data.CustomerId,
                    Title = data.Title,
                    Content = data.Content,
                    CreateBy = data.CreateBy,
                    CreateDate = data.CreateDate,
                    ModifiedBy = data.ModifiedBy,
                    ModifiedDate = data.ModifiedDate,
                    Status = data.Status,
                };
                feedback.CustomerMemberCard = data.CustomerMemberCard;
                feedback.CustomerName = data.CustomerName;
                feedback.Solution = data.Solution;

                feedback.Attachments = new List<CMSRepository.Query.ViewAttachmentInfo>();
                if (data.Attachments != null && data.Attachments.Any())
                    feedback.Attachments = data.Attachments;

                return PartialView("_DetailPartial", feedback);
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

        public ActionResult ViewAttachment(int feedbackId)
        {
            ViewBag.FeedbackId = feedbackId;
            var attachements = _feedbackService.GetAttachmentFiles(feedbackId);

            return PartialView("_AttachmentGridPartial", attachements);
        }

        public FileResult Download(Guid iden, int feedbackId)
        {
            var attachment = _feedbackService.GetAttachmentByIden(iden, feedbackId);
            return File(attachment.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, attachment.Name);
        }

        public ActionResult ChangeToPending(int id)
        {
            // Ajax
            if (Request.IsAjaxRequest())
            {
                ViewBag.FeedbackId = id;
                return PartialView("_PendingPartial");
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeToPendingPost(int id)
        {
            try
            {
                int statusCode = 0;
                string statusMessage = string.Empty;

                if (id < 1) return Json(XUtil.JsonDie(FeedbackResource.FeedbackNotFound, statusCode));

                var feedbackInfo = _feedbackService.GetById(id, null);
                if (feedbackInfo == null) return Json(XUtil.JsonDie(FeedbackResource.FeedbackNotFound, statusCode));
                feedbackInfo.SetStatus(1);

                _feedbackService.Save(feedbackInfo, SessionContext.GetUserLogin().Id);

                statusCode = 1;
                statusMessage = Request.Url.AbsolutePath + "#feedback" + id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }

        public ActionResult ChangeToComplete(int id)
        {
            // Ajax
            if (Request.IsAjaxRequest())
            {
                var data = _feedbackService.GetById(id, null);
                FeedbackModel feedback = new FeedbackModel()
                {
                    Id = data.Id,
                    CustomerId = data.CustomerId,
                    Title = data.Title,
                    Content = data.Content,
                    CreateBy = data.CreateBy,
                    CreateDate = data.CreateDate,
                    ModifiedBy = data.ModifiedBy,
                    ModifiedDate = data.ModifiedDate,
                    Status = data.Status,
                };
                feedback.CustomerName = data.CustomerName;

                return PartialView("_CompletePartial", feedback);
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeToComplete(int id, string solution)
        {
            try
            {
                int statusCode = 0;
                if (id < 1) return Json(XUtil.JsonDie(FeedbackResource.FeedbackNotFound, statusCode));

                var feedbackInfo = _feedbackService.GetById(id, null);
                if (feedbackInfo == null) return Json(XUtil.JsonDie(FeedbackResource.FeedbackNotFound, statusCode));
                feedbackInfo.SetStatus(2);

                _feedbackService.Save(feedbackInfo, SessionContext.GetUserLogin().Id);

                // Save to Solution
                CMSService.Query.SolutionInfo solutionInfo = new CMSService.Query.SolutionInfo(0
                                                                                            , id
                                                                                            , SessionContext.GetUserLogin().Id
                                                                                            , solution
                                                                                            , SessionContext.GetUserLogin().Id
                                                                                            , SessionContext.GetUserLogin().Id);

                _feedbackService.SaveSolution(solutionInfo, SessionContext.GetUserLogin().Id);
                statusCode = 1;
                string statusMessage = Request.Url.AbsolutePath + "#feedback" + id;

                return Json(XUtil.JsonDie(statusMessage, statusCode));
            }
            catch (Exception ex)
            {
                return Json(XUtil.JsonDie(ex.Message, (int)HttpStatusCode.InternalServerError));
            }
        }
    }
}