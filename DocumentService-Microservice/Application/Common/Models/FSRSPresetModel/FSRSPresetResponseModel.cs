using Application.Common.Models.NewsModel;

namespace Application.Common.Models.FSRSPresetModel
{
    public class FSRSPresetResponseModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Author Author { get; set; }
        public double[] FsrsParameters { get; set; } = new double[19];
        public double Retrievability { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublicPreset { get; set; }
    }
}
