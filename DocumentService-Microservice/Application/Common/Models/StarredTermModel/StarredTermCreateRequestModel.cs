namespace Application.Common.Models.StarredTermModel;

public class StarredTermCreateRequestModel
{
    public Guid FlashcardContentId { get; set; }
    public Guid ContainerId { get; set; }
}