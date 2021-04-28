using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Query
{
    public class EmployeeInfo
    {
        public int Id { get; private set; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Avatar { get; }
        public string IdentityCartNumber { get; }
        public Nullable<bool> Gender { get; }
        public string Email { get; }
        public string Phone { get; }
        public Nullable<System.DateTime> Birthday { get; }
        public string Province { get; }
        public string District { get; }
        public string Ward { get; }
        public string Address { get; }
        public Nullable<System.DateTime> CreateDate { get; }
        public Nullable<int> CreateBy { get; }
        public Nullable<System.DateTime> ModifiedDate { get; }
        public Nullable<int> ModifiedBy { get; }
        public bool Status { get; private set; }

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

        public void SetId(int id)
        {
            Id = id;
        }
        public void Activate()
        {
            Status = true;
        }
        public void Deactivate()
        {
            Status = false;
        }
    }
}
