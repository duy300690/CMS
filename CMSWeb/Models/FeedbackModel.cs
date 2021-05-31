using CMSRepository.Query;
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
        public string CustomerMemberCard { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Solution { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public byte Status { get; set; }
        public string[] AttachFiles { get; set; }
        public List<ViewAttachmentInfo> Attachments { get; set; }
    }
}