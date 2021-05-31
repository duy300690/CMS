using CMSService.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSService
{
    public interface IFeedbackService
    {
        Tuple<List<FeedbackInfo>, int> GetList(string query,
                                              int pageIndex,
                                              int pageSize,
                                              DateTime? from,
                                              DateTime? to,
                                              byte? status,
                                              string sortColumn,
                                              string sortType);
        FeedbackInfo GetById(int id, byte? status);
        int Save(FeedbackInfo feedback, int userId);
        void SaveListAttachment(List<CMSRepository.Query.AttachmentInfo> attachments);
        List<CMSRepository.Query.ViewAttachmentInfo> GetAttachmentFiles(int feedbackId);
        CMSRepository.Query.DownloadAttachmentInfo GetAttachmentByIden(Guid iden, int feedbackId);
        void SaveSolution(SolutionInfo solution, int userId);
    }
}
