using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService
{
    public interface ICustomerService
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
        int Create(CustomerInfo customer, int userId);
        bool IsExistEmail(string email);
        bool IsExistIdentityCode(string identityCode);
        void Active(int customerId, int userId);
        void DeActive(int customerId, int userId);
        void Edit(CustomerInfo model, int userId);
    }
}
