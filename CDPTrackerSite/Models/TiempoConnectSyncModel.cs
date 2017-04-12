using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDPTrackerSite.Models.Custom_Attributes;
using CsvHelper;
using CsvHelper.Configuration;
using DataSource;

namespace CDPTrackerSite.Models
{
    public class TiempoConnectSyncModel
    {
        [ValidateCSVFileAttribute(ErrorMessage = "File Uploaded is not valid")]
        public HttpPostedFileBase File { get; set; }
        public List<Objective> InsertedObjectives { get; set; }
        public List<string[]> RejectedObjectives  { get; set; }

    }
    public class TiempoConnectSyncRow
    {
        public int activeDirectoryID { get; set; }
        public int year { get; set; }
        public int quarter { get; set; }
        public string employeeName { get; set; }
        public string typeObjective { get; set; }
        public string objective { get; set; }
        public string evaluationName { get; set; }
    }
    public sealed class CSVClassMap : CsvClassMap<TiempoConnectSyncRow>
    {
        public CSVClassMap()
        {
            Map(m => m.activeDirectoryID).Name("userIdUnique");
            Map(m => m.year).Name("Year");
            Map(m => m.quarter).Name("Quarter");
            Map(m => m.employeeName).Name("Employee");
            Map(m => m.typeObjective).Name("TypeOfObjective");
            Map(m => m.objective).Name("Objective");
            Map(m => m.evaluationName).Name("EvaluationName");
        }

    }
}