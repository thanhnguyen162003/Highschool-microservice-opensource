// using Application.Common.Models.Common;
// using Domain.Common.Messages;
// using Domain.Common.Ultils;
// using Domain.Enumerations;
// using Domain.MongoEntities; 
// using Infrastructure.Data;
// using Infrastructure.Repositories.Interfaces;
// using Microsoft.IdentityModel.Tokens;
// using MongoDB.Driver;
// using System.Net;

// namespace Application.Features.User.GetUserHolland
// {
//     public class GetUserHollandTypeCommand : IRequest<ResponseModel>
//     {

//     }

//     public class GetUserHollandTypeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, CareerMongoDatabaseContext context) : IRequestHandler<GetUserHollandTypeCommand, ResponseModel>
//     {
//         private readonly IUnitOfWork _unitOfWork = unitOfWork;
//         private readonly IMapper _mapper = mapper;
//         private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
//         private readonly CareerMongoDatabaseContext _context = context;

//         public async Task<ResponseModel> Handle(GetUserHollandTypeCommand request, CancellationToken cancellationToken)
//         {
//             var userId = _httpContextAccessor.HttpContext?.User.GetUserIdFromToken();
//             var student = await _unitOfWork.StudentRepository.GetStudentByUserId(userId!.Value);

//             if (student == null)
//             {
//                 return new ResponseModel
//                 {
//                     Status = HttpStatusCode.NotFound,
//                     Message = MessageUser.UserNotFound
//                 };
//             }

//             List<HollandTypeContent> result = new List<HollandTypeContent>();

//             var hollandTypeContentList = await _context.HollandTypeContents.Find(_ => true).ToListAsync();

//             if (!student.HollandType.IsNullOrEmpty())
//             {
//                 char[] hollandTraits = student.HollandType!.ToCharArray();
//                 foreach (var hollandTrait in hollandTraits)
//                 {
//                     switch (hollandTrait)
//                     {
//                         case 'R':
//                             result.Add(hollandTypeContentList.Where(content => content.HollandTrait == HollandTrait.Realistic).FirstOrDefault()!);
//                             break;
//                         case 'I':
//                             result.Add(hollandTypeContentList.Where(content => content.HollandTrait == HollandTrait.Investigative).FirstOrDefault()!);
//                             break;
//                         case 'A':
//                             result.Add(hollandTypeContentList.Where(content => content.HollandTrait == HollandTrait.Artistic).FirstOrDefault()!);
//                             break;
//                         case 'S':
//                             result.Add(hollandTypeContentList.Where(content => content.HollandTrait == HollandTrait.Social).FirstOrDefault()!);
//                             break;
//                         case 'E':
//                             result.Add(hollandTypeContentList.Where(content => content.HollandTrait == HollandTrait.Enterprising).FirstOrDefault()!);
//                             break;
//                         case 'C':
//                             result.Add(hollandTypeContentList.Where(content => content.HollandTrait == HollandTrait.Conventional).FirstOrDefault()!);
//                             break;
//                     }
//                 }
//             }

//             return new ResponseModel
//             {
//                 Status = HttpStatusCode.OK,
//                 Message = MessageCommon.GetSuccess,
//                 Data = result
//             };
//         }
//     }
// }
