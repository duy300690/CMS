using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Implementation
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly CMSEntities _context;
        public FeedbackRepository(CMSEntities context)
        {
            _context = context;
        }
        public Tuple<List<FeedbackInfo>, int> GetList(string query,
                                                      int pageIndex,
                                                      int pageSize,
                                                      DateTime? from,
                                                      DateTime? to,
                                                      byte? status,
                                                      string sortColumn,
                                                      string sortType)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 0) throw new ArgumentOutOfRangeException("PageSize");

            if (!string.IsNullOrEmpty(sortType) && !Helpers.CheckExistStringInArray(sortType, SortList.sortType))
                throw new ArgumentOutOfRangeException("Invalid sort type");

            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (!Helpers.CheckExistStringInArray(sortColumn, SortList.feedbackList))
                    throw new ArgumentOutOfRangeException("Invalid sort column name");

                sortColumn = SortList.feedbackList.FirstOrDefault(p => p.ToLower().Equals(sortColumn.ToLower()));
            }
            else
                sortColumn = "Id";

            var feedbacks = _context.Feedbacks.Select(e => e);
            if (status.HasValue)
                feedbacks = feedbacks.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(query))
            {
                feedbacks = feedbacks.Where(p => p.Title.Contains(query)
                || p.Content.Contains(query)
                || p.Customer.IdentityCardNumber.Contains(query)
                || p.Customer.Email.Contains(query)
                || p.Customer.CustomerCard.Contains(query)
                || p.Customer.FirstName.Contains(query)
                || p.Customer.LastName.Contains(query));
            }
            if (from.HasValue)
                feedbacks = feedbacks.Where(p => p.CreateDate >= from.Value);

            if (to.HasValue)
            {
                to = to.Value.AddDays(1);
                feedbacks = feedbacks.Where(p => p.CreateDate <= to.Value);
            }

            // Sort data            
            string orderByStr = $"{sortColumn} {sortType}";
            feedbacks = feedbacks.OrderBy(orderByStr);

            int pageCount = 0;
            int totalRows = feedbacks.Count();

            if (pageSize > totalRows && totalRows > 0) { pageSize = totalRows; }
            pageCount = (int)Math.Ceiling((double)(totalRows / pageSize));
            if (pageIndex > pageCount) { pageIndex = pageCount + 1; }

            feedbacks = feedbacks.Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize);

            List<FeedbackInfo> listFeedback = new List<FeedbackInfo>();
            foreach (var item in feedbacks)
            {
                var feedback = new FeedbackInfo(
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

                feedback.SetCustomerMemberCard(item.Customer.CustomerCard);
                feedback.SetCustomerName($"{item.Customer.FirstName} {item.Customer.LastName}");
                if (item.FeedbackAttachments != null && item.FeedbackAttachments.Count > 0)
                {
                    List<ViewAttachmentInfo> attachments = new List<ViewAttachmentInfo>();
                    foreach (var attachFile in item.FeedbackAttachments)
                    {
                        attachments.Add(new ViewAttachmentInfo()
                        {
                            Created = attachFile.CreateDate ?? DateTime.Now,
                            DownloadLink = attachFile.Name,
                            Iden = attachFile.Iden ?? new Guid(),
                            MimeType = attachFile.MimeType,
                            Name = attachFile.Name,
                            FeedbackId = attachFile.FeedbackId ?? 0

                        });
                    }
                    feedback.SetAttachment(attachments);
                }

                listFeedback.Add(feedback);

            }
            return Tuple.Create(listFeedback, totalRows);
        }

        public FeedbackInfo GetById(int id, byte? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero Id");

            var feedback = _context.Feedbacks.Where(x => x.Id == id);
            if (status.HasValue)
                feedback = feedback.Where(x => x.Status == status.Value);

            if (feedback == null) throw new ArgumentNullException("feedback");

            var feedbackInfo = feedback.FirstOrDefault();
            return new FeedbackInfo(feedbackInfo.Id
                                    , feedbackInfo.CustomerId
                                    , feedbackInfo.Title
                                    , feedbackInfo.Content
                                    , feedbackInfo.CreateDate
                                    , feedbackInfo.CreateBy
                                    , feedbackInfo.ModifiedDate
                                    , feedbackInfo.ModifiedBy
                                    , feedbackInfo.Status);
        }

        public void Save(FeedbackInfo feedback, int userId)
        {
            if (feedback is null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            if (string.IsNullOrEmpty(feedback.Title)) throw new ArgumentNullException("Title null");
            if (string.IsNullOrEmpty(feedback.Content)) throw new ArgumentNullException("Content null");

            Feedback saveFeedback = MappingFromModelToEntity(feedback);

            if (saveFeedback.Id == 0)
            {
                saveFeedback.CreateDate = DateTime.Now;
                saveFeedback.CreateBy = userId;

                _context.Feedbacks.Add(saveFeedback);
                _context.SaveChanges();
                feedback.SetId(saveFeedback.Id);

                if (saveFeedback.Id == 0) throw new InvalidOperationException("Can't create Feedback");
            }
            else
            {
                Feedback feedbackEntity = _context.Feedbacks.FirstOrDefault(p => p.Id == feedback.Id);

                saveFeedback.ModifiedDate = DateTime.Now;
                saveFeedback.ModifiedBy = userId;
                _context.Entry(feedbackEntity).CurrentValues.SetValues(saveFeedback);

                _context.SaveChanges();
            }
        }

        private Feedback MappingFromModelToEntity(FeedbackInfo feedback)
        {
            if (feedback is null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            Feedback model = new Feedback()
            {
                Id = feedback.Id,
                CustomerId = feedback.CustomerId,
                Title = feedback.Title,
                Content = feedback.Content,
                CreateDate = feedback.CreateDate,
                CreateBy = feedback.CreateBy,
                ModifiedBy = feedback.ModifiedBy,
                ModifiedDate = feedback.ModifiedDate,
                Status = feedback.Status
            };

            return model;
        }

        public void SaveListAttachment(List<AttachmentInfo> attachments)
        {
            if (attachments.Any())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var newAttachments = attachments.Select(p => new FeedbackAttachment
                        {
                            Id = p.Id,
                            Iden = p.Iden,
                            FeedbackId = p.FeedbackId,
                            Name = p.Name,
                            MimeType = p.MimeType,
                            FileContent = p.FileContent,
                            CreateDate = DateTime.Now,
                        }).ToList();

                        _context.FeedbackAttachments.AddRange(newAttachments);

                        _context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }

                }
            }

        }

        public List<ViewAttachmentInfo> GetAttachmentFiles(int feedbackId)
        {
            return (from p in _context.FeedbackAttachments
                    where p.FeedbackId == feedbackId
                    select new ViewAttachmentInfo
                    {
                        Iden = p.Iden ?? new Guid(),
                        Name = p.Name,
                        MimeType = p.MimeType,
                        Created = p.CreateDate ?? new DateTime()
                    }).ToList();
        }

        public DownloadAttachmentInfo GetAttachmentByIden(Guid iden, int feedbackId)
        {
            return (from p in _context.FeedbackAttachments
                    where p.Iden == iden && p.FeedbackId == feedbackId
                    select new DownloadAttachmentInfo
                    {
                        Iden = p.Iden ?? new Guid(),
                        Name = p.Name,
                        MimeType = p.MimeType,
                        FileContent = p.FileContent
                    }).FirstOrDefault();

        }
    }
}
