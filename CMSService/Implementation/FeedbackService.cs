using CMSRepository;
using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Implementation
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        public Tuple<List<FeedbackInfo>, int> GetList(string query
                                                        , int pageIndex
                                                        , int pageSize
                                                        , DateTime? from
                                                        , DateTime? to
                                                        , byte? status
                                                        , string sortColumn
                                                        , string sortType)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 0) throw new ArgumentOutOfRangeException("PageSize");

            var data = _feedbackRepository.GetList(query, pageIndex, pageSize, from, to, status, sortColumn, sortType);

            List<FeedbackInfo> listFeedback = new List<FeedbackInfo>();
            if (data != null)
            {
                foreach (var item in data.Item1)
                {
                    FeedbackInfo feedback = new FeedbackInfo(
                       item.Id,
                       item.CustomerId,
                       item.Title,
                       item.Content,
                       item.CreateDate,
                       item.CreateBy,
                       item.ModifiedDate,
                       item.ModifiedBy,
                       item.Status
                       );

                    feedback.SetCustomerMemberCard(item.CustomerMemberCard);
                    feedback.SetCustomerName(item.CustomerName);

                    if (item.Attachments != null && item.Attachments.Any())
                        feedback.SetAttachment(item.Attachments);

                    listFeedback.Add(feedback);
                }
                return Tuple.Create(listFeedback, data.Item2);
            }
            return Tuple.Create(listFeedback, 0);
        }

        public FeedbackInfo GetById(int id, byte? status)
        {
            if (id < 1) throw new ArgumentNullException("Id");

            var data = _feedbackRepository.GetById(id, status);
            if (data == null) return null;
            FeedbackInfo feedback = new FeedbackInfo(
                       data.Id,
                       data.CustomerId,
                       data.Title,
                       data.Content,
                       data.CreateDate,
                       data.CreateBy,
                       data.ModifiedDate,
                       data.ModifiedBy,
                       data.Status
                       );

            feedback.SetCustomerMemberCard(data.CustomerMemberCard);
            feedback.SetCustomerName(data.CustomerName);
            feedback.SetSolution(data.Solution);

            if (data.Attachments != null && data.Attachments.Any())
                feedback.SetAttachment(data.Attachments);

            return feedback;
        }

        public int Save(FeedbackInfo feedback, int userId)
        {
            if (feedback is null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            if (!feedback.CustomerId.HasValue || feedback.CustomerId.Value < 1)
                throw new ArgumentNullException("Customer");
            if (string.IsNullOrEmpty(feedback.Title)) throw new ArgumentNullException("Title");
            if (string.IsNullOrEmpty(feedback.Content)) throw new ArgumentNullException("Content");

            CMSRepository.Query.FeedbackInfo feedbackInfo = new CMSRepository.Query.FeedbackInfo(
                                                                feedback.Id
                                                                , feedback.CustomerId
                                                                , feedback.Title
                                                                , feedback.Content
                                                                , feedback.CreateDate
                                                                , feedback.CreateBy
                                                                , feedback.ModifiedDate
                                                                , feedback.ModifiedBy
                                                                , feedback.Status);

            _feedbackRepository.Save(feedbackInfo, userId);
            return feedbackInfo.Id;
        }

        public void SaveListAttachment(List<CMSRepository.Query.AttachmentInfo> attachments)
        {
            _feedbackRepository.SaveListAttachment(attachments);
        }

        public List<CMSRepository.Query.ViewAttachmentInfo> GetAttachmentFiles(int feedbackId)
        {
            return _feedbackRepository.GetAttachmentFiles(feedbackId);
        }

        public CMSRepository.Query.DownloadAttachmentInfo GetAttachmentByIden(Guid iden, int feedbackId)
        {
            return _feedbackRepository.GetAttachmentByIden(iden, feedbackId);
        }

        public void SaveSolution(SolutionInfo solution, int userId)
        {
            if (string.IsNullOrEmpty(solution.Solutions)) throw new ArgumentNullException("Solution");

            CMSRepository.Query.SolutionInfo solutionInfo = new CMSRepository.Query.SolutionInfo(
                                                                solution.Id
                                                                , solution.FeedbackId ?? 0
                                                                , userId
                                                                , solution.Solutions
                                                                , userId
                                                                , userId);
            _feedbackRepository.SaveSolution(solutionInfo, userId);
        }

    }
}
