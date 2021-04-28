using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly CMSEntities _context;

        public EmployeeRepository(CMSEntities context)
        {
            _context = context;
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

            if (!string.IsNullOrEmpty(sortType) && !Helpers.CheckExistStringInArray(sortType, SortList.sortType))
                throw new ArgumentOutOfRangeException("Invalid sort type");

            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (!Helpers.CheckExistStringInArray(sortColumn, SortList.employeeList))
                    throw new ArgumentOutOfRangeException("Invalid sort column name");

                sortColumn = SortList.employeeList.FirstOrDefault(p => p.ToLower().Equals(sortColumn.ToLower()));
            }
            else
                sortColumn = "Id";

            var employees = _context.Employees.Select(e => e);
            if (status.HasValue)
                employees = employees.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(query))
            {
                employees = (from p in employees
                             let fullname = p.FirstName + " " + p.LastName
                             where (fullname.Contains(query)
                             || p.Email.Contains(query)
                             || p.IdentityCartNumber.Contains(query))
                             select p).AsQueryable();
            }

            // Sort data            
            string orderByStr = $"{sortColumn} {sortType}";
            employees = employees.OrderBy(orderByStr);

            int pageCount = 0;
            int totalRows = employees.Count();

            if (pageSize > totalRows && totalRows > 0) { pageSize = totalRows; }
            pageCount = (int)Math.Ceiling((double)(totalRows / pageSize));
            if (pageIndex > pageCount) { pageIndex = pageCount + 1; }

            employees = employees.Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize);

            List<EmployeeInfo> listEmployeeInfo = new List<EmployeeInfo>();
            foreach (var item in employees)
            {
                listEmployeeInfo.Add(new EmployeeInfo(
                    item.Id,
                    item.FirstName,
                    item.LastName,
                    item.Avatar,
                    item.IdentityCartNumber,
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
                    item.Status
                    ));
            }
            return Tuple.Create(listEmployeeInfo, totalRows);
        }

        public EmployeeInfo GetEmployeeById(int id, bool? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero Id");

            var employee = _context.Employees.Where(u => u.Id == id);
            if (status.HasValue)
                employee = employee.Where(u => u.Status == status.Value);

            if (employee == null) throw new ArgumentNullException("Employee not found");

            var employeeInfo = employee.FirstOrDefault();

            return new EmployeeInfo(employeeInfo.Id,
                                    employeeInfo.FirstName,
                                    employeeInfo.LastName,
                                    employeeInfo.Avatar,
                                    employeeInfo.IdentityCartNumber,
                                    employeeInfo.Gender,
                                    employeeInfo.Email,
                                    employeeInfo.Phone,
                                    employeeInfo.Birthday,
                                    employeeInfo.Province,
                                    employeeInfo.District,
                                    employeeInfo.Ward,
                                    employeeInfo.Address,
                                    employeeInfo.CreateDate,
                                    employeeInfo.CreateBy,
                                    employeeInfo.ModifiedDate,
                                    employeeInfo.ModifiedBy,
                                    employeeInfo.Status);
        }

        public void Save(EmployeeInfo employee, int userId)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            if (string.IsNullOrEmpty(employee.FirstName)) throw new ArgumentNullException("First name null");
            if (string.IsNullOrEmpty(employee.LastName)) throw new ArgumentNullException("Last name null");
            if (string.IsNullOrEmpty(employee.IdentityCartNumber)) throw new ArgumentNullException("IdentityCartNumber null");
            if (string.IsNullOrEmpty(employee.Email)) throw new ArgumentNullException("Email null");
            if (string.IsNullOrEmpty(employee.Province)
                || string.IsNullOrEmpty(employee.District)
                || string.IsNullOrEmpty(employee.Ward)
                || string.IsNullOrEmpty(employee.Address)
                ) throw new ArgumentNullException("Address null");


            Employee saveEmployee = MappingFromModelToEntity(employee);

            if (saveEmployee.Id == 0)
            {
                saveEmployee.CreateDate = DateTime.Now;
                saveEmployee.CreateBy = userId;

                _context.Employees.Add(saveEmployee);
                _context.SaveChanges();
                employee.SetId(saveEmployee.Id);

                if (saveEmployee.Id == 0) throw new InvalidOperationException("Can't create employee");
            }
            else
            {
                Employee employeeEntity = _context.Employees.FirstOrDefault(p => p.Id == employee.Id);

                saveEmployee.ModifiedDate = DateTime.Now;
                saveEmployee.ModifiedBy = userId;
                _context.Entry(employeeEntity).CurrentValues.SetValues(saveEmployee);

                _context.SaveChanges();
            }
        }

        public bool IsExistEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email null");

            return _context.Employees.Any(x => x.Email.Equals(email.Trim()));
        }

        public bool IsExistIdentityCode(string identityCode)
        {
            if (string.IsNullOrEmpty(identityCode))
                throw new ArgumentNullException("Identity code null");

            return _context.Employees.Any(x => x.IdentityCartNumber.Equals(identityCode.Trim()));
        }

        private Employee MappingFromModelToEntity(EmployeeInfo employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            Employee model = new Employee()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Avatar = employee.Avatar,
                IdentityCartNumber = employee.IdentityCartNumber,
                Gender = employee.Gender,
                Email = employee.Email,
                Phone = employee.Phone,
                Birthday = employee.Birthday,
                Province = employee.Province,
                District = employee.District,
                Ward = employee.Ward,
                Address = employee.Address,
                CreateBy = employee.CreateBy,
                CreateDate = employee.CreateDate,
                ModifiedBy = employee.ModifiedBy,
                ModifiedDate = employee.ModifiedDate,
                Status = employee.Status
            };

            return model;
        }
    }
}
