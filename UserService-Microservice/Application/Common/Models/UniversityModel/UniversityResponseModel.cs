using Application.Common.Models.MajorModel;
using Application.Common.Models.UniversityMajor;
using Domain.Enumerations;

namespace Application.Common.Models.UniversityModel
{
    public class UniversityResponseModel
    {
        public Guid Id { get; set; }
        public string UniCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public string LogoUrl { get; set; }
        public bool IsSaved { get; set; }
        public string? City { get; set; }
        public int CityId { get; set; }
        public string news_details { get; set; }
        public string admission_details { get; set; }
        public string program_details { get; set; }
        public string field_details { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<string>? tags { get; set; } = new List<string>();
        public List<UniversityMajorResponseModel> UniversityMajors { get; set; } = new List<UniversityMajorResponseModel>();
    }
}
