namespace Application.Common.Models.SearchModel
{
    public class CreateIndexModel
    {
        public string NameIndex { get; set; } = null!;
        public IEnumerable<object> Data { get; set; } = null!;
        public IEnumerable<string> SearchAttributes { get; set; } = null!;
    }
}
