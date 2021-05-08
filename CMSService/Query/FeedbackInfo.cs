using CMSRepository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService.Query
{
    public class FeedbackInfo
    {
        public int Id { get; private set; }
        public Nullable<int> CustomerId { get; private set; }
        public string CustomerMemberCard { get; private set; }
        public string CustomerName { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public Nullable<System.DateTime> CreateDate { get; private set; }
        public Nullable<int> CreateBy { get; private set; }
        public Nullable<System.DateTime> ModifiedDate { get; private set; }
        public Nullable<int> ModifiedBy { get; private set; }
        public byte Status { get; private set; }
        public List<ViewAttachmentInfo> Attachments { get; private set; }

        public FeedbackInfo(int id
                            , int? customerId
                            , string title
                            , string content
                            , DateTime? createDate
                            , int? createBy
                            , DateTime? modifiedDate
                            , int? modifiedBy
                            , byte status)
        {
            Id = id;
            CustomerId = customerId;
            Title = title;
            Content = content;
            CreateDate = createDate;
            CreateBy = createBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            Status = status;
        }
        public void SetAttachment(List<ViewAttachmentInfo> attachments)
        {
            Attachments = attachments;
        }

        public void SetCustomerName(string name)
        {
            CustomerName = name;
        }
        public void SetCustomerMemberCard(string code)
        {
            CustomerMemberCard = code;
        }
    }
}
