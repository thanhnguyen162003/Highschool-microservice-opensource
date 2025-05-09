using Domain.Enums;

namespace Application.Common.Models.ZoneModel
{
    public class ZoneResponseModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? LogoUrl { get; set; }
        public string Status { get; set; }
        public string? BannerUrl { get; set; }
        public int DocumentCount { get; set; }
        public int FlashcardCount { get; set; }
        public int FolderCount { get; set; }
        public int AssignmentCount { get; set; }
        public bool IsOwner { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Author? Author { get; set; }
        public int MemberCount { get; set; }

    }
    public class Author
    {
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
    }
}
