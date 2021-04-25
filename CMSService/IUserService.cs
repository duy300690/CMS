using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService
{
    public interface IUserService
    {
        UserInfo GetUserLogin(string username, string password);
        Tuple<List<UserInfo>, int> GetListUser(string query, int pageIndex, int pageSize, bool? active);
        UserInfo GetUserById(int id, bool? status);
        void Active(int userId);
        void DeActive(int userId);
        void SetRole(int id, string role);
    }
}
