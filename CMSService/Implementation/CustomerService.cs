using CMSRepository;
using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public Tuple<List<CustomerInfo>, int> GetListCustomer(string query
                                                    , string provinceCode
                                                    , string districtId
                                                    , int pageIndex
                                                    , int pageSize
                                                    , bool? status
                                                    , string sortColumn
                                                    , string sortType)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex");
            if (pageSize < 0) throw new ArgumentOutOfRangeException("PageSize");

            List<CustomerInfo> customers = new List<CustomerInfo>();
            var data = _customerRepository.GetListCustomer(query, provinceCode, districtId, pageIndex, pageSize, status, sortColumn, sortType);
            if (data != null)
            {
                foreach (var item in data.Item1)
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
                        item.Status));
                }
                return Tuple.Create(customers, data.Item2);
            }
            return Tuple.Create(customers, 0);
        }

        public int Create(CustomerInfo customer, int userId)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (string.IsNullOrEmpty(customer.FirstName)) throw new ArgumentNullException("First name null");
            if (string.IsNullOrEmpty(customer.LastName)) throw new ArgumentNullException("Last name null");
            if (string.IsNullOrEmpty(customer.IdentityCardNumber)) throw new ArgumentNullException("IdentityCardNumber null");
            if (string.IsNullOrEmpty(customer.Email)) throw new ArgumentNullException("Email null");
            if (string.IsNullOrEmpty(customer.Province)
                || string.IsNullOrEmpty(customer.District)
                || string.IsNullOrEmpty(customer.Ward)
                || string.IsNullOrEmpty(customer.Address)
                ) throw new ArgumentNullException("Address null");

            CMSRepository.Query.CustomerInfo customerInfo = new CMSRepository.Query.CustomerInfo(
                customer.Id
                , customer.CustomerCard
                , customer.FirstName
                , customer.LastName
                , customer.Gender
                , customer.IdentityCardNumber
                , customer.Phone
                , customer.Email
                , customer.Birthday
                , customer.Province
                , customer.District
                , customer.Ward
                , customer.Address
                , customer.FullAddress
                , customer.CreateDate
                , customer.CreateBy
                , customer.ModifiedDate
                , customer.ModifiedBy
                , customer.Status
                );
            _customerRepository.Save(customerInfo, userId);
            return customer.Id;
        }

        public CustomerInfo GetById(int id, bool? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero Id");

            var customer = _customerRepository.GetById(id, status);
            CustomerInfo model = new CustomerInfo(
                                                  customer.Id,
                                                  customer.CustomerCard,
                                                  customer.FirstName,
                                                  customer.LastName,
                                                  customer.Gender,
                                                  customer.IdentityCardNumber,
                                                  customer.Phone,
                                                  customer.Email,
                                                  customer.Birthday,
                                                  customer.Province,
                                                  customer.District,
                                                  customer.Ward,
                                                  customer.Address,
                                                  customer.FullAddress,
                                                  customer.CreateDate,
                                                  customer.CreateBy,
                                                  customer.ModifiedDate,
                                                  customer.ModifiedBy,
                                                  customer.Status);
            return model;
        }

        public bool IsExistEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email null");
            return _customerRepository.IsExistEmail(email);
        }

        public bool IsExistIdentityCode(string identityCode)
        {
            if (string.IsNullOrEmpty(identityCode))
                throw new ArgumentNullException("Identity code null");
            return _customerRepository.IsExistIdentityCode(identityCode);
        }

        public void Active(int customerId, int userId)
        {
            if (customerId < 1) throw new ArgumentOutOfRangeException("customerId");

            var customer = _customerRepository.GetById(customerId, false);
            if (customer == null) throw new ArgumentOutOfRangeException("Customer not found");

            customer.Activate();
            _customerRepository.Save(customer, userId);
        }

        public void DeActive(int customerId, int userId)
        {
            if (customerId < 1) throw new ArgumentOutOfRangeException("customerId");

            var customer = _customerRepository.GetById(customerId, true);
            if (customer == null) throw new ArgumentOutOfRangeException("Customer not found");

            customer.Deactivate();
            _customerRepository.Save(customer, userId);
        }

        public void Edit(CustomerInfo model, int userId)
        {
            if (model == null) throw new ArgumentOutOfRangeException("Employee");

            var customer = _customerRepository.GetById(model.Id, null);
            if (customer == null) throw new ArgumentNullException("Employee");
            customer.ChangeInfo(
                                model.Id,
                                model.FirstName,
                                model.LastName,                             
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

            _customerRepository.Save(customer, userId);
        }
    }
}
