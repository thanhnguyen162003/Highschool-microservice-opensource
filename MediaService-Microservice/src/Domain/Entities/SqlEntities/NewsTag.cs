using System.ComponentModel.DataAnnotations.Schema;
using Discussion_Microservice.Domain.Common;
using Domain.Entities.SqlEntites;


namespace Domain.Entities;
public class NewsTag : BaseAuditableEntitySQL
{

    public string NewTagName { get; set; } = null!;
    public string? Desciption { get; set; }
}
