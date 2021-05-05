using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CMSEntities _context;

        public CustomerRepository(CMSEntities context)
        {
            _context = context;
        }
        public Tuple<List<CustomerInfo>, int> GetListCustomer(string query
                                                            , string provinceCode
                                                            , string district
                                                            , int pageIndex
                                                            , int pageSize
                                                            , bool? status
                                                            , string sortColumn
                                                            , string sortType)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 0) throw new ArgumentOutOfRangeException("PageSize");

            if (!string.IsNullOrEmpty(sortType) && !Helpers.CheckExistStringInArray(sortType, SortList.sortType))
                throw new ArgumentOutOfRangeException("Invalid sort type");

            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (!Helpers.CheckExistStringInArray(sortColumn, SortList.customerList))
                    throw new ArgumentOutOfRangeException("Invalid sort column name");

                sortColumn = SortList.customerList.FirstOrDefault(p => p.ToLower().Equals(sortColumn.ToLower()));
            }
            else
                sortColumn = "Id";

            var customer = _context.Customers.Select(x => x);
            if (status.HasValue)
                customer = customer.Where(x => x.Status == status.Value);

            if (!string.IsNullOrEmpty(query))
            {
                customer = (from p in customer
                            let fullname = p.FirstName + " " + p.LastName
                            where (fullname.Contains(query)
                            || p.Email.Contains(query)
                            || p.IdentityCardNumber.Contains(query)
                            || p.CustomerCard.Contains(query))
                            select p).AsQueryable();
            }
            if (!string.IsNullOrEmpty(provinceCode))
                customer = customer.Where(x => x.Province.Equals(provinceCode));

            if (!string.IsNullOrEmpty(district))
                customer = customer.Where(x => x.District.Equals(district));

            // Sort data            
            string orderByStr = $"{sortColumn} {sortType}";
            customer = customer.OrderBy(orderByStr);

            int pageCount = 0;
            int totalRows = customer.Count();

            if (pageSize > totalRows && totalRows > 0) { pageSize = totalRows; }
            pageCount = (int)Math.Ceiling((double)(totalRows / pageSize));
            if (pageIndex > pageCount) { pageIndex = pageCount + 1; }

            customer = customer.Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize);

            List<CustomerInfo> customers = new List<CustomerInfo>();
            foreach (var item in customer)
            {
                customers.Add(new CustomerInfo(
                                                 item.Id,
                                                 item.CustomerCard,
                                                 item.FirstName,
                                                 item.LastName,
                                                 item.Gender,
                                                 item.IdentityCardNumber,
                                                 item.Phone,
                                                 item.Email,
                                                 item.Birthday,
                                                 item.Province,
                                                 item.District,
                                                 item.Ward,
                                                 item.Address,
                                                 item.FullAddress,
                                                 item.CreateDate,
                                                 item.CreateBy,
                                                 item.ModifiedDate,
                                                 item.ModifiedBy,
                                                 item.Status
                    ));
            }

            return Tuple.Create(customers, totalRows);
        }

        public CustomerInfo GetById(int id, bool? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero Id");

            var customer = _context.Customers.Where(u => u.Id == id);
            if (status.HasValue)
                customer = customer.Where(u => u.Status == status.Value);

            if (customer == null) throw new ArgumentNullException("Customer not found");
            var model = customer.FirstOrDefault();
            CustomerInfo customerInfo = new CustomerInfo(
                                                  model.Id,
                                                  model.CustomerCard,
                                                  model.FirstName,
                                                  model.LastName,
                                                  model.Gender,
                                                  model.IdentityCardNumber,
                                                  model.Phone,
                                                  model.Email,
                                                  model.Birthday,
                                                  model.Province,
                                                  model.District,
                                                  model.Ward,
                                                  model.Address,
                                                  model.FullAddress,
                                                  model.CreateDate,
                                                  model.CreateBy,
                                                  model.ModifiedDate,
                                                  model.ModifiedBy,
                                                  model.Status
                );
            return customerInfo;
        }

        public void Save(CustomerInfo customer, int userId)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (string.IsNullOrEmpty(customer.FirstName)) throw new ArgumentNullException("First name null");
            if (string.IsNullOrEmpty(customer.LastName)) throw new ArgumentNullException("Last name null");
            if (string.IsNullOrEmpty(customer.IdentityCardNumber)) throw new ArgumentNullException("IdentityCartNumber null");
            if (string.IsNullOrEmpty(customer.Email)) throw new ArgumentNullException("Email null");
            if (string.IsNullOrEmpty(customer.Province)
                || string.IsNullOrEmpty(customer.District)
                || string.IsNullOrEmpty(customer.Ward)
                || string.IsNullOrEmpty(customer.Address)
                ) throw new ArgumentNullException("Address null");


            Customer saveCustomer = MappingFromModelToEntity(customer);

            if (saveCustomer.Id == 0)
            {
                saveCustomer.CreateDate = DateTime.Now;
                saveCustomer.CreateBy = userId;

                _context.Customers.Add(saveCustomer);
                _context.SaveChanges();
                customer.SetId(saveCustomer.Id);

                if (saveCustomer.Id == 0) throw new InvalidOperationException("Can't create employee");
            }
            else
            {
                Customer customerEntity = _context.Customers.FirstOrDefault(p => p.Id == customer.Id);

                saveCustomer.ModifiedDate = DateTime.Now;
                saveCustomer.ModifiedBy = userId;
                _context.Entry(customerEntity).CurrentValues.SetValues(saveCustomer);

                _context.SaveChanges();
            }
        }
        public bool IsExistEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email null");

            return _context.Customers.Any(x => x.Email.Equals(email.Trim()));
        }

        public bool IsExistIdentityCode(string identityCode)
        {
            if (string.IsNullOrEmpty(identityCode))
                throw new ArgumentNullException("Identity code null");

            return _context.Customers.Any(x => x.IdentityCardNumber.Equals(identityCode.Trim()));
        }

        private Customer MappingFromModelToEntity(CustomerInfo model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            Customer customer = new Customer()
            {
                Id = model.Id,
                CustomerCard = model.CustomerCard,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                IdentityCardNumber = model.IdentityCardNumber,
                Phone = model.Phone,
                Email = model.Email,
                Birthday = model.Birthday,
                Province = model.Province,
                District = model.District,
                Ward = model.Ward,
                Address = model.Address,
                FullAddress = model.FullAddress,
                CreateDate = model.CreateDate,
                CreateBy = model.CreateBy,
                ModifiedDate = model.ModifiedDate,
                ModifiedBy = model.ModifiedBy,
                Status = model.Status
            };
            return customer;
        }
    }
}
