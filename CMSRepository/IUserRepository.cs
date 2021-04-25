using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository
{
    public interface IUserRepository
    {
        void Save(UserInfo userInfo);

        UserInfo GetUserLogin(string username, string password);

        Tuple<List<UserInfo>, int> GetListUser(string query
                                               , int pageIndex
                                               , int pageSize
                                               , bool? active
                                              );
        UserInfo GetUserById(int id, bool? status);
    }
}
