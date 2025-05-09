using Domain.Entities;

namespace Application.Common.Models.RoadmapDataModel;

public class RoadmapDetailResponseModel
{
    public string? RoadmapName { get; set; }

    public string? ContentJson { get; set; }

    public string? RoadmapDescription { get; set; }
    
    public List<Guid>? RoadmapSubjectIds { get; set; }
    
    public List<Guid>? RoadmapDocumentIds { get; set; }
    
    public List<string>? TypeExam { get; set; }
    
    public List<Node>? Nodes { get; set; }
    
    public List<Edge>? Edges { get; set; }
}

