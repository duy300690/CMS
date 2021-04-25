using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Query
{
    public class EmployeeInfo
    {
        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Avatar { get; }
        public string IdentityCartNumber { get; }
        public string Email { get; }
        public string Phone { get; }
        public DateTime? Birthday { get; }
        public string Address { get; }
        public DateTime? CreateDate { get; }
        public int? CreateBy { get; }
        public DateTime? ModifiedDate { get; }
        public int? ModifiedBy { get; }
        public bool Status { get; set; }

        public EmployeeInfo(int id
                            , string firstName
                            , string lastName
                            , string avatar
                            , string identityCartNumber
                            , string email
                            , string phone
                            , DateTime? birthdate
                            , string address
                            , DateTime? createDate
                            , int? createBy
                            , DateTime? modifiedDate
                            , int? modifiedBy
                            , bool status)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Avatar = avatar;
            IdentityCartNumber = identityCartNumber;
            Email = email;
            Phone = phone;
            Birthday = birthdate;
            Address = address;
            CreateDate = createDate;
            CreateBy = createBy;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
            Status = status;
        }
    }
}
