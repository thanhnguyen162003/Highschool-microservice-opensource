// using System.Net;
// using Application.Common.Models;
// using Application.Common.Models.CategoryModel;
// using Application.Common.Ultils;
// using Application.Common.UUID;
// using Application.Constants;
// using Domain.Entities;
// using Infrastructure.Repositories.Interfaces;
//
// namespace Application.Features.CategoryFeature.Commands;
//
// public class CategoryUpdateCommand : IRequest<ResponseModel>
// {
//     public CategoryCreateRequestModel Category { get; init; }
//     public Guid Id { get; init; }
// }
//
// public class CategoryUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//     : IRequestHandler<CategoryUpdateCommand, ResponseModel>
// {
//     public async Task<ResponseModel> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
//     {
//         Category category = mapper.Map<Category>(request.Category);
//         category.Id = request.Id;
//         category.CategorySlug = SlugHelper.GenerateSlug(request.Category.CategoryName);
//         var result = await unitOfWork.CategoryRepository.UpdateCategory(category);
//         if (result is true)
//         {
//             return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CategoryUpdated);
//         }
//         return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CategoryUpdateFailed);
//     }
// }