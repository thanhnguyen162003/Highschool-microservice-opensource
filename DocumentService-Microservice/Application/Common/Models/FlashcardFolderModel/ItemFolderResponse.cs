using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Domain.CustomEntities;

namespace Application.Common.Models.FlashcardFolderModel
{
    public class ItemFolderResponse
    {
        public FolderUserResponse? FolderUser { get; set; }
        public PagedList<FlashcardResponseModel>? Flashcards { get; set; }
        public PagedList<DocumentResponseModel>? Documents { get; set; }
    }
}
