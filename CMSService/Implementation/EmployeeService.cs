using CMSRepository;
using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public Tuple<List<EmployeeInfo>, int> GetListEmployee(string query,
                                                              int pageIndex,
                                                              int pageSize,
                                                              bool? status,
                                                              string sortColumn,
                                                              string sortType)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 0) throw new ArgumentOutOfRangeException("PageSize");

            var data = _employeeRepository.GetListEmployee(query, pageIndex, pageSize, status, sortColumn, sortType);
            if (data != null)
            {
                List<EmployeeInfo> listEmployee = new List<EmployeeInfo>();
                foreach (var item in data.Item1)
                {
                    listEmployee.Add(new EmployeeInfo(
                                                    item.Id,
                                                    item.FirstName,
                                                    item.LastName,
                                                    item.Avatar,
                                                    item.IdentityCartNumber,
                                                    item.Email,
                                                    item.Phone,
                                                    item.Birthday,
                                                    item.Address,
                                                    item.CreateDate,
                                                    item.CreateBy,
                                                    item.ModifiedDate,
                                                    item.ModifiedBy,
                                                    item.Status));
                }

                return Tuple.Create(listEmployee, data.Item2);
            }

            return Tuple.Create(new List<EmployeeInfo>(), 0); ;
        }
    }
}
