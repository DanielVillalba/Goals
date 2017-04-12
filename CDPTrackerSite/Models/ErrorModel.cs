using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class ErrorModel
    {
        public Guid Id { get; set; }
        public int errcode { get; set; }
        public string message { get; set; }
    }
}