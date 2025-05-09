using Domain.Enumerations;
using Domain.MongoEntities;

namespace Application.Common.Models.OccupationModel
{
    public class OccupationRequestModel
    {
        public List<string> MajorCodes { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public ChanceToFindJob ChanceToFindJob { get; set; }
        public int MinSalary { get; set; }
        public int AverageSalary { get; set; }
        public int MaxSalary { get; set; }
        public List<OccupationSection> Knowledge { get; set; }
        public List<OccupationSection> Skills { get; set; }
        public List<OccupationSection> Abilities { get; set; }
        public List<OccupationSection> Personality { get; set; }
        public List<OccupationSection> Technology { get; set; }
    }

}
