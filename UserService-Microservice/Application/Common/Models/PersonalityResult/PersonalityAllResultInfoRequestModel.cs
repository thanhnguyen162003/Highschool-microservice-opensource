using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.OccupationModel;
using Application.Common.Models.UniversityModel;
using Domain.Enumerations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.PersonalityResult
{
    public class PersonalityAllResultInfoRequestModel
    {
        public int Limit { get; set; } = 10;
        public bool AdvanceFilter { get; set; } = false;
    }
}
