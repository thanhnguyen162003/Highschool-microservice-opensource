using Application.Common.Models.SearchModel;
using Domain.Enums;

namespace Application.Common.Models.DocumentModel
{
    public class DocumentCountResponseModel
    {
        public int TotalCount { get; set; }
        public int DocumentCount { get; set; }
        public int LessonCount { get; set; }
        public int FolderCount { get; set; }
    }
}
