using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ENETCare.IMS;
using ENETCare.IMS.Interventions;

namespace ENETCare.IMS.WebApp.Models
{
    public class GenerateReportViewModel
    {
        public LinkedList<string> ReportTypes { get; set; }
        public string ID { get; set; }
        public string Report { get; set; }
        public int SelectedDistrictID { get; set; }
        public List<District> Districts { get; set; }

        public static string TOTAL_COSTS = "Total Costs";
        public static string AVERAGE_COSTS = "Average Costs";
        public static string DISTRICT_COSTS = "District Costs";
        public static string MONTHLY_COSTS = "Monthly Costs";
    }

}