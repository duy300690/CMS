using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Query
{
    public class CustomerInfo
    {
        public int Id { get; private set; }
        public string CustomerCart { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Nullable<bool> Gender { get; private set; }
        public string IdentityCartNumber { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public Nullable<System.DateTime> Birthday { get; private set; }
        public string Province { get; private set; }
        public string District { get; private set; }
        public string Ward { get; private set; }
        public string Address { get; private set; }
        public string FullAddress { get; private set; }
        public Nullable<System.DateTime> CreateDate { get; private set; }
        public Nullable<int> CreateBy { get; private set; }
        public Nullable<System.DateTime> ModifiedDate { get; private set; }
        public Nullable<int> ModifiedBy { get; private set; }
        public bool Status { get; private set; }

        public CustomerInfo(int id
                            , string customerCart
                            , string firstName
                            , string lastName
                            , bool? gender
                            , string identityCode
                            , string phone
                            , string email
                            , DateTime? birthday
                            , string province
                            , string district
                            , string ward
                            , string address
                            , string fullAddress
                            , DateTime? createDate
                            , int? createBy
                            , DateTime? modifiedDate
                            , int? modifiedBy
                            , bool status)
        {
            Id = id;
            CustomerCart = customerCart;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            IdentityCartNumber = identityCode;
            Phone = phone;
            Email = email;
            Birthday = birthday;
            Province = province;
            District = district;
            Ward = ward;
            Address = address;
            FullAddress = fullAddress;
            CreateBy = createBy;
            CreateDate = createDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
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
