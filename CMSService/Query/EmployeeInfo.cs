using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Query
{
    public class EmployeeInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string IdentityCartNumber { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool Status { get; set; }

        public EmployeeInfo(int id
                            , string firstName
                            , string lastName
                            , string avatar
                            , string identityCartNumber
                            , bool? gender
                            , string email
                            , string phone
                            , DateTime? birthdate
                            , string province
                            , string district
                            , string ward
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
            Gender = gender;
            Email = email;
            Phone = phone;
            Birthday = birthdate;
            Province = province;
            District = district;
            Ward = ward;
            Address = address;
            CreateDate = createDate;
            CreateBy = createBy;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
            Status = status;
        }
    }
}
