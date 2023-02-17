using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComplainBox.Data;
using ComplainBox.Models;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplainBox.Controllers
{
    public class ReportsController : Controller
    {
        private static readonly string ReportsFolder = FindReportsFolder();
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static string FindReportsFolder()
        {
            string fReportsFolder = "";
            string thisFolder = Config.ApplicationFolder;

            for (int i = 0; i < 6; i++)
            {
                string dir = Path.Combine(thisFolder, "Reports");
                if (Directory.Exists(dir))
                {
                    string rep_dir = Path.GetFullPath(dir);
                    if (System.IO.File.Exists(Path.Combine(rep_dir, "reports.xml")))
                    {
                        fReportsFolder = rep_dir;
                        break;
                    }
                }
                thisFolder = Path.Combine(thisFolder, @"..");
            }
            return fReportsFolder;
        }
        public async Task<IActionResult> ComplainsPreview()
        {
            var reportableDataSet = new List<Complain>
            {

            };
            await _context.Complains
               .ForEachAsync(p =>
                {
                    var reportData = new Complain
                    {
                        ComplainTo = p.ComplainTo,
                        Id = p.Id,
                        ComplainTime = p.ComplainTime,
                        ComplainType = p.ComplainType,
                        ComplainedBy = p.ComplainedBy,
                        ContactNo = p.ContactNo,
                        Description = p.Description,
                        Email = p.Email,
                        IsResolved = p.IsResolved,
                        Reference = p.Reference,
                        Title = p.Title

                    };
                    reportableDataSet.Add(reportData);
                });
            

            ReportViewModel model;
            model = GenerateReportViewModel(reportableDataSet.OrderBy(p => p.ComplainTime).ToList(), "Complain");


            return View("ReportViewer", model);
        }

        private ReportViewModel GenerateReportViewModel<T>(IEnumerable<T> reportableDataSet)
        {
            var model = new ReportViewModel
            {
                WebReport = new WebReport(),
                ReportName = typeof(T).Name
            };

            var reportToLoad = model.ReportName;
            model.WebReport.Report.Load(Path.Combine(ReportsFolder, $"{reportToLoad}.frx"));
            model.WebReport.Report.RegisterData(reportableDataSet, $"{model.ReportName}s");
            model.WebReport.Report.GetDataSource($"{model.ReportName}s").Enabled = true;
            model.WebReport.Report.Prepare();
            return model;
        }
        private ReportViewModel GenerateReportViewModel<T>(IEnumerable<T> reportableDataSet, string templateName)
        {
            var model = new ReportViewModel
            {
                WebReport = new WebReport(),
                ReportName = typeof(T).Name
            };

            var reportToLoad = templateName;
            model.WebReport.Report.Load(Path.Combine(ReportsFolder, $"{reportToLoad}.frx"));
            model.WebReport.Report.RegisterData(reportableDataSet, $"{model.ReportName}s");
            model.WebReport.Report.GetDataSource($"{model.ReportName}s").Enabled = true;
            model.WebReport.Report.Prepare();
            return model;
        }

    }
}
