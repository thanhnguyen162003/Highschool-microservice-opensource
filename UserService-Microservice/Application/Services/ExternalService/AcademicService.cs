//using Domain.Services.UserService;
//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;
//using Microsoft.IdentityModel.Tokens;

//namespace Application.Services.ExternalService
//{
//    public class AcademicService : AcademicServiceRpc.AcademicServiceRpcBase
//    {
//        private readonly ILogger<AcademicService> _logger;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;

//        public AcademicService(ILogger<AcademicService> logger, IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _logger = logger;
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }

//        public override async Task<ObjectListResponse> GetUsers(GetAcademicUserRequest request, ServerCallContext context)
//        {
//            var academicData = await _unitOfWork.UserRepository.GetAllByEmails(request.Emails);

//            var users = _mapper.Map<IEnumerable<AcademicUserResponse>>(academicData);

//            _logger.LogInformation($"Fetched academic details for academicId");

//            return new ObjectListResponse()
//            {
//                Objects = { users }
//            };
//        }

//    }
//}
