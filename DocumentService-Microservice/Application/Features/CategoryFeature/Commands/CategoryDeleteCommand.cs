// using System.Net;
// using Application.Common.Models;
// using Application.Common.UUID;
// using Application.Constants;
// using Domain.Entities;
// using Infrastructure.Repositories.Interfaces;
//
// namespace Application.Features.CategoryFeature.Commands;
//
// public record CategoryDeleteCommand : IRequest<ResponseModel>
// {
//     public Guid Id { get; init; }
// }
//
// public class CategoryDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//     : IRequestHandler<CategoryDeleteCommand, ResponseModel>
// {
//     public async Task<ResponseModel> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
//     {
//         var result = await unitOfWork.CategoryRepository.DeleteCategory(request.Id);
//         if (result is true)
//         {
//             return new ResponseModel(HttpStatusCode.Created,ResponseConstaints.CategoryDeleted);
//         }
//         return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CategoryDeleteFailed);
//     }
// }