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
                    item.Email,
                    item.Phone,
                    item.Birthday,
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
    }
}
