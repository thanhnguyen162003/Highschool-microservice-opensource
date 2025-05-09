using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.SubjectCurriculumModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StatisticFeature.Queries
{
    public class MaterialCountQuery : IRequest<DocumentCountResponseModel>
    {
    }
    public class MaterialCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<MaterialCountQuery, DocumentCountResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<DocumentCountResponseModel> Handle(MaterialCountQuery request, CancellationToken cancellationToken)
        {
            var documentCount = await _unitOfWork.DocumentRepository.GetDocumentsCount(cancellationToken);
            var folderCount = await _unitOfWork.FolderUserRepository.GetFoldersCount(cancellationToken);
            var lessonCount = await _unitOfWork.LessonRepository.GetLessonsCount(cancellationToken);
            var result = new DocumentCountResponseModel
            {
                TotalCount = documentCount + folderCount + lessonCount,
                DocumentCount = documentCount,
                LessonCount = lessonCount,
                FolderCount = folderCount
            };
            return result;
        }
    }
}
