using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository
{
    public static class SortList
    {
        public static string[] sortType = { "ASC", "DESC" };
        public static string[] UserList = { "Id", "Username", "Email", "CreateDate", "Role" };
        public static string[] employeeList = { "Id", "FirstName", "Email", "CreateDate" };
        public static string[] customerList = { "Id", "FirstName", "Email", "CreateDate", "CustomerCart" };
        public static string[] feedbackList = { "Id", "Title", "CreateDate", "Status", "Customer.FirstName" };
    }
}
