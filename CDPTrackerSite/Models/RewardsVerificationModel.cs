using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDPTrackerSite.DataAccessor;
using DataSource;

namespace CDPTrackerSite.Models
{
    public class RewardsVerificationModel
    {
        public List<TDUReward> AllRewards { get; set; }
        public List<Resource> ValidResources { get; set; }
        public List<TDUReward> ValidRewards { get; set; }
    }
}