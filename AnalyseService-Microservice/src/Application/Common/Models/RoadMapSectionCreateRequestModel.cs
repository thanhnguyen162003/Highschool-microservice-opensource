using Domain.Entities;

namespace Application.Common.Models;

public class RoadMapSectionCreateRequestModel
{
    public string RoadmapId { get; set; }
    public string ContentJson { get; set; }
    public List<Node> Nodes { get; set; }
    public List<Edge> Edges { get; set; }
}

