using Application.Common.Models.MajorModel;
using Domain.Enumerations;
using Domain.MongoEntities;

namespace Application.Common.Models.UniversityMajor
{
    public class UniversityMajorResponseModel
    {
        public string Id { get; set; }
        public string UniCode { get; set; }
        public string MajorCode { get; set; }
        public List<AdmissionMethod> AdmissionMethods { get; set; } = new List<AdmissionMethod>();
        public int Quota { get; set; }
        public DegreeLevel DegreeLevel { get; set; }
        public MajorResponseModel? Major { get; set; }
        public double TuitionPerYear { get; set; }
        public int YearOfReference { get; set; }
    }
}
