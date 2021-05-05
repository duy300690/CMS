using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository
{
    public interface IFeedbackRepository
    {
        Tuple<List<FeedbackInfo>, int> GetList(string query,
                                               int pageIndex,
                                               int pageSize,
                                               DateTime? from,
                                               DateTime? to,
                                               byte? status,
                                               string sortColumn,
                                               string sortType);

        FeedbackInfo GetById(int id, byte? status);

        void Save(FeedbackInfo feedback, int userId);
    }
}
