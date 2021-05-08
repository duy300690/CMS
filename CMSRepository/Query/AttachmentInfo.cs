using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Query
{
    public class AttachmentInfo
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public DateTime Created { get; set; }
        public Guid Iden { get; set; }
        public byte[] FileContent { get; set; }
    }

    public class ViewAttachmentInfo
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public Guid Iden { get; set; }
        public int FeedbackId { get; set; }
        public DateTime Created { get; set; }

        public string DownloadLink { get; set; }
    }


    public class SaveAttachmentInfo
    {
        public string Name { get; set; }
        public Guid Iden { get; set; }
    }

    public class DownloadAttachmentInfo
    {
        public Guid Iden { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }

        public byte[] FileContent { get; set; }
    }
}
