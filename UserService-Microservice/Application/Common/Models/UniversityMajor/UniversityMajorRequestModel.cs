using Domain.Enumerations;

namespace Application.Common.Models.UniversityMajor
{
    public class UniversityMajorRequestModel
    {
        public string UniCode { get; set; }
        public string MajorCode { get; set; }
        public List<AdmissionMethod> AdmissionMethods { get; set; } = new List<AdmissionMethod>();
        public int Quota { get; set; }
        public DegreeLevel DegreeLevel { get; set; }
        public double TuitionPerYear { get; set; }
        public int YearOfReference { get; set; }
    }

}
