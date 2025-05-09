using Application.Common.Models.QuestionModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.QuestionFeature.Queries
{
    public class QuestionAdvanceQuery : IRequest<PagedList<QuestionResponseModel>>
    {
        public required QuestionAdvanceQueryFilter QueryFilter { get; set; }
    }
    public class QuestionAdvanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<QuestionAdvanceQuery, PagedList<QuestionResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedList<QuestionResponseModel>> Handle(QuestionAdvanceQuery request, CancellationToken cancellationToken)
        {
            var (listQuestions, totalCount) = await _unitOfWork.QuestionRepository.GetQuestionAdvanceFilter(request.QueryFilter, cancellationToken);

            if (!listQuestions.Any())
            {
                return new PagedList<QuestionResponseModel>(new List<QuestionResponseModel>(), 0, 0, 0);
            }

            var mappedQuestions = _mapper.Map<List<QuestionResponseModel>>(listQuestions);

            return new PagedList<QuestionResponseModel>(mappedQuestions, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }
    }


}
