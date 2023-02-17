using System;
using FastReport.Web;

namespace ComplainBox.Models
{
    public class ReportViewModel
    {
        public WebReport WebReport { get; set; }
        public string ReportName { get; set; }
        public Guid RouteId { get; set; }
    }
}