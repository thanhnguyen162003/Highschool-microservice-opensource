using Domain.Enumerations;

namespace Application.Common.Models.OccupationModel
{
    public class OccupationResponseModel
    {
        public string Id { get; set; }
        public string OccupationGroup { get; set; }
        public string OccupationName { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string OccupationDetail { get; set; }
        public string WorkEnvironment { get; set; }
        public string HowToAchieve { get; set; }
        public string SalaryDescription { get; set; }
        public string OccupationProspect { get; set; }
        public string Location { get; set; }
    }
}
