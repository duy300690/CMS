using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSWeb.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string Avatar { get; set; }
        public string EmployeeName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string Role { get; set; }
        public bool Status { get; set; }
    }
}