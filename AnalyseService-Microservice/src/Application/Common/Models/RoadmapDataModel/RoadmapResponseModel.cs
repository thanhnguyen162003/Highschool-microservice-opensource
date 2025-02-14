namespace Application.Common.Models.RoadmapDataModel;

public class RoadmapResponseModel
{
    public string? Id { get; set; }
    public string? RoadmapName { get; set; }

    public string? RoadmapDescription { get; set; }
    
    public List<Guid>? RoadmapSubjectIds { get; set; }
    
    public List<Guid>? RoadmapDocumentIds { get; set; }
    
    public List<string>? TypeExam { get; set; }
    
}
