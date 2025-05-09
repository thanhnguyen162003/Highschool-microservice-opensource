using Application.Common.Models.Common;
using Domain.Enumerations;

namespace Application.Services
{
    public interface IOpenAIService
    {
        Task<ResponseModel> WriteContent(TypeHelperContent type, string text);
        Task<String> GenerateBio(DateTime Birthdate);
        Task<List<string>?> WriteBrief(MBTIType mbtiType, string hollandTrait);
        Task<(List<string>? features, List<string>? descriptions)> WriteBriefMBTI(MBTIType mbtiType);
        Task<(List<string>? features, List<string>? descriptions)> WriteBriefHolland(string hollandType);
    }
}
