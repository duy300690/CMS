using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository
{
    public interface ICustomerRepository
    {
        Tuple<List<CustomerInfo>, int> GetListCustomer(string query
                                                    , string provinceCode
                                                    , string districtId
                                                    , int pageIndex
                                                    , int pageSize
                                                    , bool? status
                                                    , string sortColumn
                                                    , string sortType);
        CustomerInfo GetById(int id, bool? status);
        CustomerInfo GetByCustomerCard(string customerCard, bool? status);
        void Save(CustomerInfo customer, int userId);
        bool IsExistEmail(string email);
        bool IsExistIdentityCode(string identityCode);
    }
}
