using CDPTrackerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class QuestionModel
    {
        public int? questionId { get; set; }
        public int surveyId { get; set; }
        public string text { get; set; }
        public int sequence { get; set; }
        public int questionType { get; set; }
        public int? questionChild { get; set; }
        public int? required { get; set; }
        public int? displayWhenValue { get; set; }

        public List<ResponseModel> responses { get; set; }
    }
}