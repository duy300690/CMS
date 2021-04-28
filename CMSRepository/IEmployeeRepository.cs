using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository
{
    public interface IEmployeeRepository
    {
        Tuple<List<EmployeeInfo>, int> GetListEmployee(string query, int pageIndex, int pageSize, bool? status, string sortColumn, string sortType);
        EmployeeInfo GetEmployeeById(int id, bool? status);
        void Save(EmployeeInfo employee, int userId);
        bool IsExistEmail(string email);
        bool IsExistIdentityCode(string identityCode);
    }
}
