using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Query
{
    public class UserInfo
    {
        public int Id { get; }
        public int? EmployeeId { get; }
        public string EmployeeName { get; }
        public string Username { get; }
        public string Password { get; }
        public DateTime? CreateDate { get; }
        public int? CreateBy { get; }
        public DateTime? ModifiedDate { get; }
        public int? ModifiedBy { get; }
        public string Role { get; }
        public bool Status { get; }


        public UserInfo(
            int id
            , int? employeeId
            , string employeeName
            , string userName
            , string password
            , DateTime? createDate
            , int? createBy
            , DateTime? modifiedDate
            , int? modifiedBy
            , string role
            , bool status
            )
        {
            Id = id;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Username = userName;
            Password = password;
            CreateDate = createDate;
            CreateBy = createBy;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
            Role = role;
            Status = status;
        }
    }
}
