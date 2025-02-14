using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.FlashcardModel;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFolderUserFeature.Queries
{
    public class FlashcardFolderQuery : IRequest<ResponseModel>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Guid Id { get; set; }
    }

    public class FlashcardFolderQueryHandler : IRequestHandler<FlashcardFolderQuery, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimInterface _claimInterface;
        private readonly IMapper _mapper;

        public FlashcardFolderQueryHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _claimInterface = claimInterface;
            _mapper = mapper;
        }

        public async Task<ResponseModel> Handle(FlashcardFolderQuery request, CancellationToken cancellationToken)
        {
            FolderUser? folder = null;
            Guid? userId = null;
            if (_claimInterface.IsAuthenticated)
            {
                userId = _claimInterface.GetCurrentUserId;

                folder = await _unitOfWork.FolderUserRepository.GetById(request.Id, (Guid)userId);
            } else
            {
                folder = await _unitOfWork.FolderUserRepository.GetById(request.Id);
            }

            if(folder == null)
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Folder not found",
                    Data = request.Id
                };
            }

            // Get flashcard folder by folder id
            var resultFlashcard = await _unitOfWork.FlashcardFolderRepository.GetFlashcardFolderByFolderId(request.Id, userId);
            var flashcardsMap = _mapper.Map<IEnumerable<FlashcardModel>>(resultFlashcard.Select(x => x.Flashcard).ToList());
            var flashcards = _mapper.Map<IEnumerable<FlashcardResponseModel>>(flashcardsMap);
            // Get document folder by folder id
            var resultDocument = await _unitOfWork.DocumentFolderRepository.GetDocumentFolderByFolderId(request.Id);
            var documents = _mapper.Map<IEnumerable<DocumentResponseModel>>(resultDocument.Select(x => x.Document).ToList());

            if(request.PageNumber == -1)
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Get folder success",
                    Data = new ItemFolderResponse()
                    {
                        FolderUser = _mapper.Map<FolderUserResponse>(resultFlashcard.FirstOrDefault()?.Folder),
                        Flashcards = PagedList<FlashcardResponseModel>.Create(flashcards, 1, flashcards.Count()),
                        Documents = PagedList<DocumentResponseModel>.Create(documents, 1, documents.Count())
                    }
                };
            }

            var countFlashcard = flashcards.Count();
            var countDocument = documents.Count();

            return new ResponseModel()
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get folder success",
                Data = new ItemFolderResponse()
                {
                    FolderUser = _mapper.Map<FolderUserResponse>(folder),
                    Flashcards = new PagedList<FlashcardResponseModel>(flashcards.ToList(), countFlashcard, 1, countFlashcard),
                    Documents = new PagedList<DocumentResponseModel>(documents.ToList(), countDocument, 1, countDocument)
                }
            };
        }
    }
}
