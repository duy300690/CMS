using CMSRepository;
using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public UserInfo GetUserLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username is empty");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password is empty");

            var userInfo = _userRepository.GetUserLogin(username, password);
            if (userInfo != null)
                return new UserInfo(userInfo.Id,
                                    userInfo.EmployeeId,
                                    userInfo.EmployeeName,
                                    userInfo.Avatar,
                                    userInfo.Username,
                                    userInfo.Password,
                                    userInfo.CreateDate,
                                    userInfo.CreateBy,
                                    userInfo.ModifiedDate,
                                    userInfo.ModifiedBy,
                                    userInfo.Role,
                                    userInfo.Status
                    );

            return null;
        }

        public Tuple<List<UserInfo>, int> GetListUser(string query, int pageIndex, int pageSize, bool? active)
        {
            if (pageIndex < 1) throw new ArgumentNullException("pageIndex");
            if (pageSize < 1) throw new ArgumentNullException("pageSize");

            var data = _userRepository.GetListUser(query, pageIndex, pageSize, active);
            if (data != null)
            {
                List<UserInfo> userInfos = new List<UserInfo>();
                foreach (var userInfo in data.Item1)
                {
                    userInfos.Add(new UserInfo(
                                    userInfo.Id,
                                    userInfo.EmployeeId,
                                    userInfo.EmployeeName,
                                    userInfo.Avatar,
                                    userInfo.Username,
                                    userInfo.Password,
                                    userInfo.CreateDate,
                                    userInfo.CreateBy,
                                    userInfo.ModifiedDate,
                                    userInfo.ModifiedBy,
                                    userInfo.Role,
                                    userInfo.Status
                        ));
                }
                return Tuple.Create(userInfos, data.Item2);
            }
            return Tuple.Create(new List<UserInfo>(), 0); ;
        }

        public UserInfo GetUserById(int id, bool? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero id");

            var userInfo = _userRepository.GetUserById(id, status);
            return new UserInfo(userInfo.Id,
                                userInfo.EmployeeId,
                                userInfo.EmployeeName,
                                userInfo.Avatar,
                                userInfo.Username,
                                userInfo.Password,
                                userInfo.CreateDate,
                                userInfo.CreateBy,
                                userInfo.ModifiedDate,
                                userInfo.ModifiedBy,
                                userInfo.Role,
                                userInfo.Status);
        }

        public void Active(int userId)
        {
            if (userId < 1) throw new ArgumentOutOfRangeException("userId");

            var user = _userRepository.GetUserById(userId, false);
            if (user == null) throw new ArgumentOutOfRangeException("User not found");

            user.Activate();
            _userRepository.Save(user);
        }

        public void DeActive(int userId)
        {
            if (userId < 1) throw new ArgumentOutOfRangeException("userId");

            var user = _userRepository.GetUserById(userId, true);
            if (user == null) throw new ArgumentOutOfRangeException("User not found");

            user.Deactivate();
            _userRepository.Save(user);
        }

        public void SetRole(int id, string role)
        {
            if (id < 1) throw new ArgumentOutOfRangeException("userId");
            if (string.IsNullOrEmpty(role))
                throw new ArgumentOutOfRangeException("Empty role");

            var user = _userRepository.GetUserById(id, null);
            if (user == null) throw new ArgumentOutOfRangeException("User not found");

            user.SetRole(role);
            _userRepository.Save(user);
        }
    }
}
