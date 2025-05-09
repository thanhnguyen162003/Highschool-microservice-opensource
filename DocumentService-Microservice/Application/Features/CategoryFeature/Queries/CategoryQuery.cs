// using Application.Common.Models.CategoryModel;
// using Infrastructure.Repositories.Interfaces;
//
// namespace Application.Features.CategoryFeature.Queries;
//
// public record CategoryQuery : IRequest<List<CategoryReponseModel>>
// {
// }
//
// public class CategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//     : IRequestHandler<CategoryQuery, List<CategoryReponseModel>>
// {
//     public async Task<List<CategoryReponseModel>> Handle(CategoryQuery request, CancellationToken cancellationToken)
//     {
//         return mapper.Map<List<CategoryReponseModel>>(await unitOfWork.CategoryRepository.GetCategories());
//     }
// }