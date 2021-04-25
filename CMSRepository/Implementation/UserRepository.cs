using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CMSRepository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly CMSEntities _context;

        public UserRepository(CMSEntities context)
        {
            _context = context;
        }

        public Tuple<List<UserInfo>, int> GetListUser(string query, int pageIndex, int pageSize, bool? active)
        {
            if (pageIndex < 1) throw new ArgumentNullException("pageIndex");
            if (pageSize < 1) throw new ArgumentNullException("pageSize");

            var listUser = _context.Users.Select(u => u);

            if (active.HasValue)
                listUser = listUser.Where(u => u.Status == active.Value);

            if (!string.IsNullOrEmpty(query))
                listUser = listUser.Where(u => u.Username.Contains(query));

            listUser = listUser.OrderByDescending(u => u.Id);

            int pageCount = 0;
            int totalRows = listUser.Count();

            if (pageSize > totalRows && totalRows > 0) { pageSize = totalRows; }
            pageCount = (int)Math.Ceiling((double)(totalRows / pageSize));
            if (pageIndex > pageCount) { pageIndex = pageCount + 1; }

            listUser = listUser.Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize);

            List<UserInfo> listUserInfo = new List<UserInfo>();
            foreach (var item in listUser)
            {

                listUserInfo.Add(new UserInfo(
                    item.Id,
                    item.EmployeeId,
                    item.Employee != null ? $"{item.Employee.FirstName} {item.Employee.LastName}" : string.Empty,
                    item.Username,
                    item.Password,
                    item.CreateDate,
                    item.CreateBy,
                    item.ModifiedDate,
                    item.ModifiedBy,
                    item.Role,
                    item.Status
                    ));
            }
            return Tuple.Create(listUserInfo, totalRows);
        }

        public UserInfo GetUserLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username is empty");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password is empty");

            var userInfo = _context.Users.FirstOrDefault(u => u.Username.Equals(username)
                                                         && u.Password.Equals(password)
                                                         && u.Status);

            if (userInfo == null) return null;
            return new UserInfo(userInfo.Id,
                                 userInfo.EmployeeId,
                                 userInfo.Employee != null ? $"{userInfo.Employee.FirstName} {userInfo.Employee.LastName}" : string.Empty,
                                 userInfo.Username,
                                 userInfo.Password,
                                 userInfo.CreateDate,
                                 userInfo.CreateBy,
                                 userInfo.ModifiedDate,
                                 userInfo.ModifiedBy,
                                 userInfo.Role,
                                 userInfo.Status
                                 );

        }

        public UserInfo GetUserById(int id, bool? status)
        {
            if (id < 1) throw new ArgumentNullException("Zero Id");

            var user = _context.Users.Where(u => u.Id == id);
            if (status.HasValue)
                user = user.Where(u => u.Status == status.Value);

            if (user == null) throw new ArgumentNullException("User not found");

            var userInfo = user.FirstOrDefault();

            return new UserInfo(userInfo.Id,
                                userInfo.EmployeeId,
                                userInfo.Employee != null ? $"{userInfo.Employee.FirstName} {userInfo.Employee.LastName}" : string.Empty,
                                userInfo.Username,
                                userInfo.Password,
                                userInfo.CreateDate,
                                userInfo.CreateBy,
                                userInfo.ModifiedDate,
                                userInfo.ModifiedBy,
                                userInfo.Role,
                                userInfo.Status);
        }

        public void Save(UserInfo userInfo)
        {
            if (userInfo is null)
            {
                throw new ArgumentNullException(nameof(userInfo));
            }

            User saveUser = MappingFromModelToEntity(userInfo);

            if (saveUser.Id == 0)
            {
                saveUser.CreateDate = DateTime.Now;

                _context.Users.Add(saveUser);
                _context.SaveChanges();
                userInfo.SetId(saveUser.Id);

                if (saveUser.Id == 0) throw new InvalidOperationException("Can't create user");
            }
            else
            {
                User user = _context.Users.FirstOrDefault(p => p.Id == userInfo.Id);

                saveUser.ModifiedDate = DateTime.Now;                
                _context.Entry(user).CurrentValues.SetValues(saveUser);

                _context.SaveChanges();
            }
        }

        private User MappingFromModelToEntity(UserInfo userInfo)
        {
            if (userInfo is null)
            {
                throw new ArgumentNullException(nameof(userInfo));
            }

            User modelUser = new User()
            {
                Id = userInfo.Id,
                EmployeeId = userInfo.EmployeeId,
                Username = userInfo.Username,
                Password = userInfo.Password,
                CreateDate = userInfo.CreateDate,
                CreateBy = userInfo.CreateBy,
                ModifiedDate = userInfo.ModifiedDate,
                ModifiedBy = userInfo.ModifiedBy,
                Role = userInfo.Role,
                Status = userInfo.Status
            };

            return modelUser;
        }
    }
}
