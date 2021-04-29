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
        EmployeeInfo GetEmployeeById(int id, bool? status);

        int CreateEmployee(EmployeeInfo employee, int userId);
        bool IsExistEmail(string email);
        bool IsExistIdentityCode(string identityCode);
        void Active(int employeeId, int userId);
        void DeActive(int employeeId, int userId);
        void Edit(EmployeeInfo model, int userId);
    }
}
