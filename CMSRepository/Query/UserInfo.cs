using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Query
{
    public class UserInfo
    {
        public int Id { get; private set; }
        public int? EmployeeId { get; }
        public string EmployeeName { get; }
        public string Username { get; }
        public string Password { get; }
        public DateTime? CreateDate { get; }
        public int? CreateBy { get; }
        public DateTime? ModifiedDate { get; }
        public int? ModifiedBy { get; }
        public string Role { get; private set; }
        public bool Status { get; private set; }

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

        public void SetId(int newId)
        {
            if (Id != 0) throw new Exception("Can not change on existing companyId");
            Id = newId;
        }

        public void Activate()
        {
            Status = true;
        }

        public void Deactivate()
        {
            Status = false;
        }

        public void SetRole(string role)
        {
            Role = role;
        }
    }
}
