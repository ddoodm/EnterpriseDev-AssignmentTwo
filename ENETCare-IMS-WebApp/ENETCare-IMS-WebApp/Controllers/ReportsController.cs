﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text;
using ENETCare.IMS;
using ENETCare.IMS.Users;
using ENETCare.IMS.Interventions;
using ENETCare.IMS.WebApp.Models;
using ENETCare.IMS.Data.DataAccess;

namespace ENETCare_IMS_WebApp.Controllers
{
    public enum ReportTypes { TOTAL_COSTS, AVERAGE_COSTS, DISTRICT_COSTS, MONTHLY_COSTS };

    [Authorize(Roles = "Accountant")]
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            LinkedList<string> reportTypes = new LinkedList<string>();
            reportTypes.AddLast("Total Costs");
            reportTypes.AddLast("Average Costs");
            reportTypes.AddLast("District Costs");
            reportTypes.AddLast("Monthly Costs");

            List<District> districts;
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                DistrictRepo repo = new DistrictRepo(db);
                districts = repo.GetAllDistricts();
            }

            GenerateReportViewModel model = new GenerateReportViewModel();
            model.ReportTypes = reportTypes;
            model.Districts = districts;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Index(GenerateReportViewModel m)
        {
            if (!ModelState.IsValid)
                return Index();

            return RedirectToAction("ViewReport", "Reports", new { ID = m.ID, DistrictID = m.SelectedDistrictID });

        }


        public ActionResult ViewReport(string ID, int DistrictID)
        {
            string report = string.Empty;

            if (ID == GenerateReportViewModel.TOTAL_COSTS)
            {
                report = GenerateTotalCostsReport();
            }
            else if (ID == GenerateReportViewModel.AVERAGE_COSTS)
            {
                report = GenerateAverageCostsReport();
            }
            else if (ID == GenerateReportViewModel.DISTRICT_COSTS)
            {
                report = GenerateDistrictCostsReport();
            }
            else if (ID == GenerateReportViewModel.MONTHLY_COSTS)
            {
                report = GenerateMonthlyCostsReport(DistrictID);
            }
            GenerateReportViewModel model = new GenerateReportViewModel();
            model.Report = report;

            return View(model);
        }

        string GenerateTotalCostsReport()
        {
            StringBuilder report = new StringBuilder();
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionsRepo = new InterventionRepo(db);
                UserRepo userRepo = new UserRepo(db);
                List<Intervention> interventions = interventionsRepo.GetAllInterventions().GetInterventions();
                List<SiteEngineer> engineers = userRepo.GetAllSiteEngineers();

                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br />");
                report.Append("               TOTAL COSTS BY ENGINEER              ");
                report.Append("<br />");
                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br /> <br />");
                foreach (SiteEngineer engineer in engineers)
                {
                    decimal totalLaborHours = interventions.Where(i => i.SiteEngineer.Id == engineer.Id).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Select(i => i.Labour).Sum();
                    decimal totalCosts = interventions.Where(i => i.SiteEngineer.Id == engineer.Id).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Select(i => i.Cost).Sum();

                    report.Append(engineer.Name.ToUpper());
                    report.Append("<br />");
                    report.AppendFormat("Total Labour Hours: {0} hours", totalLaborHours);
                    report.Append("<br />");
                    report.AppendFormat("Total Costs: {0}", totalCosts.ToString("C2"));

                    if (engineer != engineers.Last())
                        report.Append("<br /><br />---oOo---<br /><br />");
                }
            }
            return report.ToString();
        }

        string GenerateAverageCostsReport()
        {
            StringBuilder report = new StringBuilder();
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionsRepo = new InterventionRepo(db);
                UserRepo userRepo = new UserRepo(db);
                List<Intervention> interventions = interventionsRepo.GetAllInterventions().GetInterventions();
                List<SiteEngineer> engineers = userRepo.GetAllSiteEngineers();

                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br />");
                report.Append("              AVERAGE COSTS BY ENGINEER             ");
                report.Append("<br />");
                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br /> <br />");
                foreach (SiteEngineer engineer in engineers)
                {
                    decimal totalLaborHours = interventions.Where(i => i.SiteEngineer.Id == engineer.Id).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Select(i => i.Labour).DefaultIfEmpty(0).Average();
                    decimal totalCosts = interventions.Where(i => i.SiteEngineer.Id == engineer.Id).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Select(i => i.Cost).DefaultIfEmpty(0).Average();

                    report.Append(engineer.Name.ToUpper());
                    report.Append("<br />");
                    report.AppendFormat("Average Labour Hours: {0} hours", totalLaborHours);
                    report.Append("<br />");
                    report.AppendFormat("Average Costs: {0}", totalCosts.ToString("C2"));

                    if (engineer != engineers.Last())
                        report.Append("<br /><br />---oOo---<br /><br />");
                }
            }

            return report.ToString();
        }

        string GenerateDistrictCostsReport()
        {
            StringBuilder report = new StringBuilder();
            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionsRepo = new InterventionRepo(db);
                DistrictRepo districtRepo = new DistrictRepo(db);
                List<District> districts = districtRepo.GetAllDistricts();

                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br />");
                report.Append("               TOTAL COSTS BY DISTRICT              ");
                report.Append("<br />");
                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br /> <br />");

                decimal companyTotalHours = 0, companyTotalCosts = 0;

                foreach (District district in districts)
                {
                    decimal totalLaborHours = interventionsRepo.GetInterventionsByDistrict(district).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Select(i => i.Labour).Sum();
                    decimal totalCosts = interventionsRepo.GetInterventionsByDistrict(district).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Select(i => i.Cost).Sum();

                    companyTotalHours += totalLaborHours;
                    companyTotalCosts += totalCosts;

                    report.Append(district.Name.ToUpper());
                    report.Append("<br />");
                    report.AppendFormat("Total Labour Hours: {0} hours", totalLaborHours);
                    report.Append("<br />");
                    report.AppendFormat("Total Costs: {0}", totalCosts.ToString("C2"));

                    report.Append("<br /><br />---oOo---<br /><br />");


                }

                report.Append(@"            --//COMPANY TOTALS\\--             ");
                report.Append("<br /> <br />");
                report.AppendFormat("Total Labour Hours: {0} hours", companyTotalHours);
                report.Append("<br />");
                report.AppendFormat("Total Costs: {0}", companyTotalCosts.ToString("C2"));
            }
            return report.ToString();
        }

        string GenerateMonthlyCostsReport(int districtID)
        {

            StringBuilder report = new StringBuilder();

            using (EnetCareDbContext db = new EnetCareDbContext())
            {
                InterventionRepo interventionsRepo = new InterventionRepo(db);
                DistrictRepo districtRepo = new DistrictRepo(db);
                District district = districtRepo.GetDistrictById(districtID);

                int[] months = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br />");
                report.Append("               TOTAL COSTS BY DISTRICT              ");
                report.Append("<br />");
                report.Append("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                report.Append("<br /> <br />");

                report.Append(district.Name.ToUpper());
                report.Append("<br />");

                foreach (int month in months)
                {
                    decimal totalLaborHours = interventionsRepo.GetInterventionsByDistrict(district).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Where(i => i.Date.Month == month).Select(i => i.Labour).Sum();
                    decimal totalCosts = interventionsRepo.GetInterventionsByDistrict(district).Where(i => i.ApprovalState == InterventionApprovalState.Completed).Where(i => i.Date.Month == month).Select(i => i.Cost).Sum();

                    report.AppendFormat("Total Labour Hours for {0}: {1} hours", GetMonthName(month), totalLaborHours);
                    report.Append("<br />");
                    report.AppendFormat("Total Costs for {0}: {1}", GetMonthName(month), totalCosts.ToString("C2"));

                    report.Append("<br /><br />---oOo---<br /><br />");
                }
            }

            return report.ToString();

        }

        private string GetMonthName(int monthNumber)
        {
            switch (monthNumber)
            {
                case 1: return "January";
                case 2: return "February";
                case 3: return "March";
                case 4: return "April";
                case 5: return "May";
                case 6: return "June";
                case 7: return "July";
                case 8: return "August";
                case 9: return "September";
                case 10: return "October";
                case 11: return "November";
                case 12: return "December";
                default: return string.Empty;
            }
        }

    }
}