using Domain.Models.UserModels;

namespace Domain.Models.KetModels
{
    public class KetResponseModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? TotalQuestion { get; set; }

        public int? TotalPlay { get; set; }

        public string? Thumbnail { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public AuthorResponseModel? Author { get; set; }
    }
}
