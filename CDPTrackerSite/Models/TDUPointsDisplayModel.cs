using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataSource;

namespace CDPTrackerSite.Models
{
	public class TDUPointsDisplayModel
	{
        public int resourceId { get; set; }
        public int quarter { get; set; }
        public int? verifiedPoints { get; set; }
        public int? potentialPoints { get; set; }
        public int missingPoints { get; set; }
        public TDUReward TDURewards { get; set; }
	}
}