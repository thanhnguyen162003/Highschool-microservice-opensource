namespace Application.Common.Models.BaseModelRoadmap;

public class NodeResponseModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public string? TypeDocument { get; set; }
    public List<RelatedDocumentResponse>? RelationDocument { get; set; }
}