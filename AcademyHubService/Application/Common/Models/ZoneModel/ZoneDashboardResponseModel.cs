using Domain.Enums;

namespace Application.Common.Models.ZoneModel
{
    public class ZoneDashboardResponseModel
    {
        public ZoneUser ZoneUser { get; set; }
        public ZoneAssignment ZoneAssignment { get; set; }
        public List<ZoneDashboard> ZoneDashboards { get; set; } = new List<ZoneDashboard>();
    }
    public class ZoneDashboard
    {
        public string Range { get; set; }
        public int Count { get; set; }
    }
    public class ZoneUser
    {
        public int TotalUser { get; set; }
        public int TotalStudent { get; set; }
        public int TotalMentor { get; set; }
    }
    public class ZoneAssignment
    {
        public int TotalAssignment { get; set; }
        public int TotalSubmission { get; set; }
    }

}
