using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class TMIResponses
    {
       public String Text { set; get; }
       public int Strongly_Agree { set; get; }
       public int Agree { set; get; }
       public int Neutral { set; get; }
       public int Disagree { set; get; }
       public int Strongly_Disagree { set; get; }

    }
}