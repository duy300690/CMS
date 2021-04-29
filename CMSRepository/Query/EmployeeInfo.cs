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
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Avatar { get; private set; }
        public string IdentityCartNumber { get; private set; }
        public Nullable<bool> Gender { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public Nullable<System.DateTime> Birthday { get; private set; }
        public string Province { get; private set; }
        public string District { get; private set; }
        public string Ward { get; private set; }
        public string Address { get; private set; }
        public Nullable<System.DateTime> CreateDate { get; private set; }
        public Nullable<int> CreateBy { get; private set; }
        public Nullable<System.DateTime> ModifiedDate { get; private set; }
        public Nullable<int> ModifiedBy { get; private set; }
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

        public void ChangeInfo(int id
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
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
            Status = status;
        }
    }
}
