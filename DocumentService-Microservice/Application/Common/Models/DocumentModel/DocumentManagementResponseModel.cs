using Application.Common.Models.SearchModel;
using Domain.Enums;

namespace Application.Common.Models.DocumentModel
{
    public class DocumentManagementResponseModel : DocumentResponseModel
    {
        public DocumentSchoolResponseModel School { get; set; }
    }
    public class DocumentSchoolResponseModel
    {
        public Guid SchoolId { get; set; }
        public string? SchoolName { get; set; }
    }
}
