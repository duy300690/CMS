using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSRepository.Query
{
    public class SolutionInfo
    {
        public int Id { get; private set; }
        public Nullable<int> FeedbackId { get; private set; }
        public Nullable<int> UserSolveId { get; private set; }
        public string Solutions { get; private set; }
        public Nullable<System.DateTime> CreateDate { get; private set; }
        public Nullable<int> CreateBy { get; private set; }
        public Nullable<System.DateTime> ModifiedDate { get; private set; }
        public Nullable<int> ModifiedBy { get; private set; }
        public bool Status { get; private set; }

        public SolutionInfo(int id
                            , int feedbackId
                            , int userId
                            , string solution
                            , int createBy
                            , int modifiedBy
                            )
        {
            Id = id;
            FeedbackId = feedbackId;
            UserSolveId = userId;
            Solutions = solution;
            CreateBy = createBy;
            ModifiedBy = modifiedBy;
            CreateDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
