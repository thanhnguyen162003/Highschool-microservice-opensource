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
// public record CategoryCreateCommand : IRequest<ResponseModel>
// {
//     public CategoryCreateRequestModel Category { get; init; }
// }
//
// public class CategoryCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//     : IRequestHandler<CategoryCreateCommand, ResponseModel>
// {
//     public async Task<ResponseModel> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
//     {
//         Category category = mapper.Map<Category>(request.Category);
//         category.Id = new UuidV7().Value;
//         category.CategorySlug = SlugHelper.GenerateSlug(category.CategoryName);
//         var result = await unitOfWork.CategoryRepository.AddCategory(category);
//         if (result is true)
//         {
//             return new ResponseModel(HttpStatusCode.Created,ResponseConstaints.CategoryCreated);
//         }
//         return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CategoryCreateFailed);
//     }
// }