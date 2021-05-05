using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSWeb.Models
{
    public class FeedbackModel
    {
        public int Id { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public byte Status { get; set; }
    }
}