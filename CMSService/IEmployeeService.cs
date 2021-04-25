using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService
{
    public interface IEmployeeService
    {
        Tuple<List<EmployeeInfo>, int> GetListEmployee(string query,
                                                              int pageIndex,
                                                              int pageSize,
                                                              bool? status,
                                                              string sortColumn,
                                                              string sortType);
    }
}
