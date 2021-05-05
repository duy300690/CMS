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

        public int CreateEmployee(EmployeeInfo employee, int userId)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            if (string.IsNullOrEmpty(employee.FirstName)) throw new ArgumentNullException("First name null");
            if (string.IsNullOrEmpty(employee.LastName)) throw new ArgumentNullException("Last name null");
            if (string.IsNullOrEmpty(employee.IdentityCardNumber)) throw new ArgumentNullException("IdentityCardNumber null");
            if (string.IsNullOrEmpty(employee.Email)) throw new ArgumentNullException("Email null");
            if (string.IsNullOrEmpty(employee.Province)
                || string.IsNullOrEmpty(employee.District)
                || string.IsNullOrEmpty(employee.Ward)
                || string.IsNullOrEmpty(employee.Address)
                ) throw new ArgumentNullException("Address null");

            CMSRepository.Query.EmployeeInfo employeeInfo = new CMSRepository.Query.EmployeeInfo
                (
                                                    employee.Id,
                                                    employee.FirstName,
                                                    employee.LastName,
                                                    employee.Avatar,
                                                    employee.IdentityCardNumber,
                                                    employee.Gender,
                                                    employee.Email,
                                                    employee.Phone,
                                                    employee.Birthday,
                                                    employee.Province,
                                                    employee.District,
                                                    employee.Ward,
                                                    employee.Address,
                                                    employee.CreateDate,
                                                    employee.CreateBy,
                                                    employee.ModifiedDate,
                                                    employee.ModifiedBy,
                                                    employee.Status
                );
            _employeeRepository.Save(employeeInfo, userId);
            return employeeInfo.Id;
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
                                                    item.IdentityCardNumber,
                                                    item.Gender,
                                                    item.Email,
                                                    item.Phone,
                                                    item.Birthday,
                                                    item.Province,
                                                    item.District,
                                                    item.Ward,
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

        public EmployeeInfo GetEmployeeById(int id, bool? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero Id");

            var employee = _employeeRepository.GetEmployeeById(id, status);
            EmployeeInfo model = new EmployeeInfo(
                                                  employee.Id,
                                                  employee.FirstName,
                                                  employee.LastName,
                                                  employee.Avatar,
                                                  employee.IdentityCardNumber,
                                                  employee.Gender,
                                                  employee.Email,
                                                  employee.Phone,
                                                  employee.Birthday,
                                                  employee.Province,
                                                  employee.District,
                                                  employee.Ward,
                                                  employee.Address,
                                                  employee.CreateDate,
                                                  employee.CreateBy,
                                                  employee.ModifiedDate,
                                                  employee.ModifiedBy,
                                                  employee.Status
                );
            return model;
        }

        public bool IsExistEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email null");
            return _employeeRepository.IsExistEmail(email);
        }

        public bool IsExistIdentityCode(string identityCode)
        {
            if (string.IsNullOrEmpty(identityCode))
                throw new ArgumentNullException("Identity code null");
            return _employeeRepository.IsExistIdentityCode(identityCode);
        }

        public void Active(int employeeId, int userId)
        {
            if (employeeId < 1) throw new ArgumentOutOfRangeException("employeeId");

            var employee = _employeeRepository.GetEmployeeById(employeeId, false);
            if (employee == null) throw new ArgumentOutOfRangeException("Employee not found");

            employee.Activate();
            _employeeRepository.Save(employee, userId);
        }

        public void DeActive(int employeeId, int userId)
        {
            if (employeeId < 1) throw new ArgumentOutOfRangeException("employeeId");

            var employee = _employeeRepository.GetEmployeeById(employeeId, true);
            if (employee == null) throw new ArgumentOutOfRangeException("Employee not found");

            employee.Deactivate();
            _employeeRepository.Save(employee, userId);
        }

        public void Edit(EmployeeInfo model, int userId)
        {
            if (model == null) throw new ArgumentOutOfRangeException("Employee");

            var employee = _employeeRepository.GetEmployeeById(model.Id, null);
            if (employee == null) throw new ArgumentNullException("Employee");
            employee.ChangeInfo(
                                model.Id,
                                model.FirstName,
                                model.LastName,
                                model.Avatar,
                                model.IdentityCardNumber,
                                model.Gender,
                                model.Email,
                                model.Phone,
                                model.Birthday,
                                model.Province,
                                model.District,
                                model.Ward,
                                model.Address,
                                model.ModifiedDate,
                                model.ModifiedBy,
                                model.Status);

            _employeeRepository.Save(employee, userId);
        }
    }
}
