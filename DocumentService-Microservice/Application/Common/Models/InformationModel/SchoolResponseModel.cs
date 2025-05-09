namespace Application.Common.Models.InformationModel;

public class SchoolResponseModel
{
    public Guid? Id { get; set; }
    public string? SchoolName { get; set; }
    public int? ProvinceId { get; set; }
    public string? ProvinceName { get; set; }
    public int? NumberDocuments { get; set; }
}