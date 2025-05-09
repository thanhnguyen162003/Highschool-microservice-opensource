// using Application.Common.Models.Common;
// using Application.Common.Models.UserModel;
// using Application.Services;
// using Domain.Common.Messages;
// using Domain.Common.Ultils;
// using Infrastructure.Data;
// using Infrastructure.Repositories.Interfaces;
// using MongoDB.Driver;
// using System.Net;

// namespace Application.Features.User.GetPersonalityTestStatus
// {
//     public class GetBriefQuery : IRequest<ResponseModel>
//     {
//     }

//     public class GetBriefQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IOpenAIService openAIService) : IRequestHandler<GetBriefQuery, ResponseModel>
//     {
//         private readonly IUnitOfWork _unitOfWork = unitOfWork;
//         private readonly IMapper _mapper = mapper;
//         private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
//         private readonly IOpenAIService _openAIService = openAIService;

//         public async Task<ResponseModel> Handle(GetBriefQuery request, CancellationToken cancellationToken)
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

//             if (string.IsNullOrWhiteSpace(student.HollandType) || student.MbtiType == null)
//             {
//                 return new ResponseModel
//                 {
//                     Status = HttpStatusCode.BadRequest,
//                     Message = "Bạn chưa làm bài test MBTI hoặc Holland"
//                 };
//             }

//             List<string>? mbtiBriefSummary = new List<string>();
//             List<string>? mbtiBriefDescription = new List<string>();
//             List<string>? hollandBriefSummary = new List<string>();
//             List<string>? hollandBriefDescription = new List<string>();
//             List<string>? overallBrief = new List<string>();

//             var hollandBrief = await _unitOfWork.BriefPersonalityRepository.GetBriefPersonalityByHollandTypeAndMBTIType(student.HollandType, null);
//             if (hollandBrief != null)
//             {
//                 hollandBriefSummary = hollandBrief.Summary;
//                 hollandBriefDescription = hollandBrief.Description;
//             }
//             else
//             {
//                 (hollandBriefSummary, hollandBriefDescription) = await _openAIService.WriteBriefHolland(student.HollandType);
//                 if (hollandBriefSummary == null || hollandBriefDescription == null 
//                     || hollandBriefSummary.Any(string.IsNullOrWhiteSpace)
//                     || hollandBriefDescription.Any(string.IsNullOrWhiteSpace)) 
//                 {
//                     return new ResponseModel()
//                     {
//                         Status = HttpStatusCode.ServiceUnavailable,
//                         Message = "Tạm thời không có dữ liệu, vui lòng quay lại sau"
//                     };
//                 }
//                 await _unitOfWork.BriefPersonalityRepository.CreateBriefPersonality(new Domain.MongoEntities.BriefPersonality()
//                 {
//                     Summary = hollandBriefSummary!,
//                     Description = hollandBriefDescription!,
//                     HollandType = student.HollandType,
//                     MBTIType = null
//                 });
//             }

//             var mbtiBrief = await _context.BriefPersonalities.Find(x => x.MBTIType == student.MbtiType && string.IsNullOrWhiteSpace(x.HollandType)).FirstOrDefaultAsync(cancellationToken);
//             if (mbtiBrief != null)
//             {
//                 mbtiBriefSummary = mbtiBrief.Summary;
//                 mbtiBriefDescription = mbtiBrief.Description;
//             }
//             else
//             {
//                 (mbtiBriefSummary, mbtiBriefDescription) = await _openAIService.WriteBriefMBTI(student!.MbtiType!.Value);
//                 if (mbtiBriefSummary == null || mbtiBriefDescription == null 
//                     || mbtiBriefSummary.Any(string.IsNullOrWhiteSpace)
//                     || mbtiBriefDescription.Any(string.IsNullOrWhiteSpace))
//                 {
//                     return new ResponseModel()
//                     {
//                         Status = HttpStatusCode.ServiceUnavailable,
//                         Message = "Tạm thời không có dữ liệu, vui lòng quay lại sau"
//                     };
//                 }
//                 await _context.BriefPersonalities.InsertOneAsync(new Domain.MongoEntities.BriefPersonality()
//                 {
//                     Summary = mbtiBriefSummary,
//                     Description = mbtiBriefDescription,
//                     MBTIType = student.MbtiType!.Value,
//                     HollandType = null,
//                 });
//             }

//             var summaryBrief = await _context.BriefPersonalities.Find(x => x.MBTIType == student.MbtiType 
//                                                                         && x.HollandType == student.HollandType).FirstOrDefaultAsync(cancellationToken);
//             if (summaryBrief != null)
//             {
//                 overallBrief = summaryBrief.Summary;
//             }
//             else
//             {
//                 overallBrief = await _openAIService.WriteBrief(student.MbtiType.Value, student.HollandType);
//                 if (overallBrief == null)
//                 {
//                     return new ResponseModel()
//                     {
//                         Status = HttpStatusCode.ServiceUnavailable,
//                         Message = "Tạm thời không có dữ liệu, vui lòng quay lại sau"
//                     };
//                 }
//                 await _context.BriefPersonalities.InsertOneAsync(new Domain.MongoEntities.BriefPersonality()
//                 {
//                     Summary = overallBrief,
//                     MBTIType = student.MbtiType!.Value,
//                     HollandType = student.HollandType,
//                     Description = null
//                 });
//             }

//             return new ResponseModel
//             {
//                 Status = HttpStatusCode.OK,
//                 Message = MessageCommon.GetSuccess,
//                 Data = new BriefInfoResponseModel()
//                 {
//                     MBTIResponse = new MBTIBriefResponseModel()
//                     {
//                         MBTIType = student!.MbtiType!.Value,
//                         MBTISummary = mbtiBriefSummary,
//                         MBTIDescription = mbtiBriefDescription,
//                     }, 
//                     HollandResponse = new HollandBriefResponseModel()
//                     {
//                         HollandType = student!.HollandType!,
//                         HollandSummary = hollandBriefSummary,
//                         HollandDescription = hollandBriefDescription,
//                     },
//                     OverallResponse = new OverallBriefResponseModel()
//                     {
//                         OverallBrief = overallBrief,
//                     }
//                 }
//             };


//         }
//     }
// }
