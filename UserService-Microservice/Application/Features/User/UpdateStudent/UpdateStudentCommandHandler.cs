using Application.Common.Models.Common;
using Application.Services.ServiceTask.Common;
using Dapr.Client;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Entities;
using Domain.MongoEntities;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.User.UpdateStudent
{
	public class UpdateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICommonTask commonTask,
        IHttpContextAccessor httpContextAccessor, DaprClient client, CareerMongoDatabaseContext databaseContext) : IRequestHandler<UpdateStudentCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly DaprClient _client = client;
		private readonly ICommonTask _commonTask = commonTask;
		private readonly CareerMongoDatabaseContext _databaseContext = databaseContext;

        public async Task<ResponseModel> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
		{
			var student = await _unitOfWork.StudentRepository.GetStudentByUserId(request.BaseUserId);

			if (student == null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.NotFound,
					Message = MessageUser.UserNotFound
				};
			}

			// Call subject service to validation subject name
			if (request.SubjectIds.Any())
			{

				SubjectNameRequest subjectNameRequest = new SubjectNameRequest
				{
					SubjectName = { request.SubjectIds }
				};

				var subjectNameResponse = await _client.InvokeMethodAsync<List<string>>(
							HttpMethod.Get,
							"document-sidecar",
							$"api/v1/dapr/check-subject-name?subjectCurriculumId={string.Join("&subjectCurriculumId=", subjectNameRequest)}"
						);

				if (subjectNameResponse != null && subjectNameResponse.Any())
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.BadRequest,
						Message = MessageUser.InvalidSubject,
						Data = subjectNameResponse
					};
				}
			}

			var newStudent = _mapper.Map(request, student);

			await UpdatePersonalityFromCache(student, student.BaseUser);

			_unitOfWork.StudentRepository.Update(newStudent);

			var userId = _httpContextAccessor.HttpContext?.User.GetUserIdFromToken();
			await _unitOfWork.UserSubjectRepository.Add((Guid)userId!, request.SubjectIds);

			if (await _unitOfWork.SaveChangesAsync())
			{
				// Publish message to Kafka
				if (!request.TypeExams.IsNullOrEmpty() || !request.SubjectIds.IsNullOrEmpty())
				{
					_commonTask.PublishUserUpdatedMessage((Guid)userId);
				}

				return new ResponseModel
				{
					Status = HttpStatusCode.OK,
					Message = MessageCommon.UpdateSuccesfully
				};
			}

			return new ResponseModel
			{
				Status = HttpStatusCode.InternalServerError,
				Message = MessageCommon.ServerError
			};
		}

        private async Task UpdatePersonalityFromCache(Student student, BaseUser baseUser)
        {
            var cachePersonality = await _databaseContext.CacheMBTIHollands
                .Find(cache => cache.Email == baseUser.Email)
                .FirstOrDefaultAsync();

            if (cachePersonality != null)
            {
                student.MbtiType = cachePersonality.MBTIType;
                student.HollandType = cachePersonality.HollandType;

                var filter = Builders<CacheMBTIHolland>.Filter.Eq(cache => cache.Email, cachePersonality.Email);

                if (cachePersonality.MBTIType != null)
                    await _databaseContext.MBTIHistory.InsertOneAsync(new MBTIHistory()
                    {
                        UserId = baseUser.Id,
                        MBTIType = cachePersonality.MBTIType.Value,
                        CreatedAt = cachePersonality.MBTIUpdatedAt,
                    });

                if (!string.IsNullOrWhiteSpace(cachePersonality.HollandType))
                    await _databaseContext.HollandHistory.InsertOneAsync(new HollandHistory()
                    {
                        UserId = baseUser.Id,
                        HollandType = cachePersonality.HollandType,
                        CreatedAt = cachePersonality.HollandUpdatedAt
                    });

                await _databaseContext.CacheMBTIHollands.DeleteOneAsync(filter);
            }
        }

    }
}
