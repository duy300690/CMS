using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSWeb.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
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
    }
}