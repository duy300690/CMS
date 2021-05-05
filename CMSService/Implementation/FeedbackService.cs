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
                    listFeedback.Add(new FeedbackInfo(
                        item.Id,
                        item.CustomerId,
                        item.Title,
                        item.Content,
                        item.CreateDate,
                        item.CreateBy,
                        item.ModifiedDate,
                        item.ModifiedBy,
                        item.Status
                        ));
                }
                return Tuple.Create(listFeedback, data.Item2);
            }
            return Tuple.Create(listFeedback, 0);
        }
    }
}
