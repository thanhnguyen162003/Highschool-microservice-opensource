using Domain.Enumerations;
using System.Reflection.Metadata;

namespace Application.Common.Models.UserModel
{
    public class BriefInfoHistoryResponseModel
    {
        public List<MBTIBriefHistoryResponseModel> MBTIResponses { get; set; }
        public List<HollandBriefHistoryResponseModel> HollandResponses { get; set; }
    }
    public class BriefInfoResponseModel
    {
        public MBTIBriefResponseModel MBTIResponse { get; set; }
        public HollandBriefResponseModel HollandResponse { get; set; }
        public OverallBriefResponseModel OverallResponse { get; set; }        
    }
    public class MBTIBriefResponseModel
    {
        public MBTIType MBTIType { get; set; }
        public List<string> MBTISummary { get; set; }
        public List<string> MBTIDescription { get; set; }
    }
    public class MBTIBriefHistoryResponseModel : MBTIBriefResponseModel
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class HollandBriefResponseModel
    {
        public string HollandType { get; set; }
        public List<string> HollandSummary { get; set; }
        public List<string> HollandDescription { get; set; }
    }
    public class HollandBriefHistoryResponseModel : HollandBriefResponseModel 
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class OverallBriefResponseModel
    {
        public List<string> OverallBrief { get; set; }
    }
}
