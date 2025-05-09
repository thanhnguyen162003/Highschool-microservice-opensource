using Domain.Enums;

namespace Application.Common.Models.SearchModel
{
    public class SearchEventDataModifiedModel
    {
        public TypeEvent Type { get; set; }
        public IndexName IndexName { get; set; }
        public IEnumerable<SearchEventDataModel>? Data { get; set; }

    }

    public class SearchEventDataModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public UpdateCourseType? TypeField { get; set; }
    }

}
