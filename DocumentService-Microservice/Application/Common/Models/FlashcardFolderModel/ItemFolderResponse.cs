using Application.Common.Models.DocumentModel;
using Domain.CustomEntities;

namespace Application.Common.Models.FlashcardFolderModel
{
    public class ItemFolderResponse
    {
        public FolderUserResponse? FolderUser { get; set; }
        public PagedList<Domain.CustomModel.FlashcardModel>? Flashcards { get; set; }
        public PagedList<DocumentResponseModel>? Documents { get; set; }
    }
}
