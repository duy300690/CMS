using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService
{
    public interface IFeedbackService
    {
        Tuple<List<FeedbackInfo>, int> GetList(string query,
                                              int pageIndex,
                                              int pageSize,
                                              DateTime? from,
                                              DateTime? to,
                                              byte? status,
                                              string sortColumn,
                                              string sortType);
    }
}
