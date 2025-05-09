using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Application.Constants;
using Application.ProduceMessage;
using Dapr.Client;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.MongoEntities;
using Domain.Services.Authentication;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;
using SharedProject.Models;
using System.Net;

namespace Application.Features.User.UpdateBaseUser
{
	public class UpdateBaseUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producerService,
            IAuthenticationService authenticationService, CareerMongoDatabaseContext databaseContext,
            DaprClient daprClient) : IRequestHandler<UpdateBaseUserCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;
		private readonly IAuthenticationService _authenticationService = authenticationService;
		private readonly IProducerService _producerService = producerService;
		private readonly CareerMongoDatabaseContext _databaseContext = databaseContext;
		private readonly DaprClient _daprClient = daprClient;

        public async Task<ResponseModel> Handle(UpdateBaseUserCommand request, CancellationToken cancellationToken)
		{
			// Get user id from token
			var userId = _authenticationService.GetUserId();

			// Get user by user id
			var user = await _unitOfWork.UserRepository.GetUserByUserId(userId);

			// Begin transaction to update user
			await _unitOfWork.BeginTransactionAsync();

			// Check if user has role
			if (user?.RoleId != (int)RoleEnum.Unknown && request.RoleName != null)
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.BadRequest,
					Message = MessageUser.UserNotPermissionUpdateRole
				};
			}
			else if (user?.RoleId == (int)RoleEnum.Unknown)
			{
				if (request.RoleName == null)
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.BadRequest,
						Message = MessageUser.YouNeedToChoseRoleBefore
					};
				}
				await UpdateUserWithoutRole(user, request);
				user.RoleId = (int)EnumExtensions.ConvertToRoleValue(request.RoleName)!;
			}

			// Map to baseuser and check if user is null
			_mapper.Map(request, user);

			// Update BaseUser
			_unitOfWork.UserRepository.Update(user);

			if (user.RoleId == (int)RoleEnum.Teacher && request.Teacher != null)
			{
				if (!await CheckExistTeacher(userId, request.Teacher!))
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.BadRequest,
						Message = MessageUser.UserNotFound
					};
				}

			}
			else if (user.RoleId == (int)RoleEnum.Student && request.Student != null)
			{
				if (!await CheckExistStudent(user, request.Student!))
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.BadRequest,
						Message = MessageUser.UserNotFound
					};
				}

				// Publish message to Kafka
				if (!string.IsNullOrEmpty(request.Address) || request.Student.SubjectIds.Any() || request.Student.TypeExams.Any())
				{
					if (request.Student.SubjectIds.Any())
					{
						SubjectCurriculumNameRequest subjectNameRequest = new SubjectCurriculumNameRequest
						{
							SubjectCurriculumName = { request.Student.SubjectIds }
						};

						var subjectNameResponse = await _daprClient.InvokeMethodAsync<List<string>>(
							HttpMethod.Get,
							"document-sidecar",
							$"api/v1/dapr/check-subject-curriculum-name?subjectCurriculumId={string.Join("&subjectCurriculumId=", request.Student.SubjectIds)}"
						);

						Console.WriteLine("Subject Name Response: " + string.Join(", ", subjectNameResponse ?? new List<string>()));

						if (subjectNameResponse != null && subjectNameResponse.Any())
						{
							return new ResponseModel
							{
								Status = HttpStatusCode.BadRequest,
								Message = MessageUser.InvalidSubject,
								Data = subjectNameResponse
							};
						}

						await _unitOfWork.UserSubjectRepository.Add(userId, request.Student.SubjectIds);
					}

					if (!await PublishUserUpdatedMessageAsync(user.Id, request.Student.SubjectIds))
					{
						await _unitOfWork.RollbackTransactionAsync();

						return new ResponseModel
						{
							Status = HttpStatusCode.BadRequest,
							Message = MessageCommon.ProducerMessageFailed
						};
					}

				}
			}

			if (!await _unitOfWork.CommitTransactionAsync())
			{
				return new ResponseModel
				{
					Status = HttpStatusCode.InternalServerError,
					Message = MessageCommon.ServerError
				};
			}

			await UpdateProgressStage(userId, user.ProgressStage!);
			NotificationUserModel dataModel = new NotificationUserModel()
			{
				UserId = userId.ToString(),
				Content = MessageUser.ThankYou,
				Title = MessageUser.YouHaveBeenCompleteSetup
            };

            _ = Task.Run(async () =>
			{
				var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, userId.ToString(), dataModel);
			}, cancellationToken);
			return new ResponseModel
			{
				Status = HttpStatusCode.OK,
				Message = MessageCommon.UpdateSuccesfully,
				Data = request.RoleName != null ? await GetUserLoginInfo(userId) : null
			};
		}

		private async Task<LoginResponseModel?> GetUserLoginInfo(Guid userId)
		{
			var sessionId = _authenticationService.GetSessionId();
			var user = await _unitOfWork.UserRepository.GetUserByUserId(userId);
			var session = await _unitOfWork.SessionRepository.GetSessionUser(sessionId, userId);

			var userLogin = _mapper.Map<LoginResponseModel>(user);
			userLogin.SessionId = session!.Id;
			userLogin.RefreshToken = session.RefreshToken;
			var (accessToken, timeExpire) = _authenticationService.GenerateAccessToken(user, sessionId);
			userLogin.AccessToken = accessToken;
			userLogin.ExpiresAt = timeExpire;

			return userLogin;

		}

		private async Task<bool> UpdateProgressStage(Guid userId, string progressStage)
		{
			if (progressStage.Equals(ProgressStage.Completion.ToString()))
			{
				return true;
			}

			return await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.ProgressStageUpdated, userId.ToString(), userId);
		}

		private async Task<bool> PublishUserUpdatedMessageAsync(Guid userId, IEnumerable<string> subjectIds)
		{
			// Get student by user id
			var student = await _unitOfWork.StudentRepository.GetStudentByUserId(userId);

			if (student == null)
			{
				return false;
			}

			// Map student to message
			var message = _mapper.Map<UserUpdatedMessage>(student);

			message.Subjects = subjectIds.ToList();

			// Publish message to Kafka
			return await _producerService.ProduceObjectWithKeyAsync(TopicConstant.RecommendOnBoarding, userId.ToString(), message);
		}

		private async Task<bool> CheckExistTeacher(Guid userId, TeacherRequestModel teacherRequestModel)
		{
			var teacher = await _unitOfWork.TeacherRepository.GetTeacherByUserId(userId);

			if (teacher == null)
			{
				var newTeacher = new Teacher()
				{
					Id = new UuidV7().Value,
					BaseUserId = userId,
					Verified = false,
					Rating = 0
				};

				teacher = _mapper.Map(teacherRequestModel, newTeacher);

				await _unitOfWork.TeacherRepository.AddAsync(teacher);
			}
			else
			{
				teacher = _mapper.Map(teacherRequestModel, teacher);

				_unitOfWork.TeacherRepository.Update(teacher);
			}

			return true;
		}

		private async Task<bool> CheckExistStudent(BaseUser user, StudentRequestModel studentRequestModel)
		{
			var student = await _unitOfWork.StudentRepository.GetStudentByUserId(user.Id);

			var cachePersonality = await _databaseContext.CacheMBTIHollands
						.Find(cache => cache.Email == user.Email)
						.FirstOrDefaultAsync();

			if (student == null)
			{
				var newStudent = new Student()
				{
					Id = new UuidV7().Value,
					BaseUserId = user.Id,
					Grade = 10
				};

				student = _mapper.Map(studentRequestModel, newStudent);

				await UpdatePersonalityFromCache(student, user);

				await _unitOfWork.StudentRepository.AddAsync(student);
			}
			else
			{
				_mapper.Map(studentRequestModel, student);

				await UpdatePersonalityFromCache(student, user);

				_unitOfWork.StudentRepository.Update(student);
			}

			return true;
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

		private async Task UpdateUserWithoutRole(BaseUser baseUser, UpdateBaseUserCommand user)
		{
			if (user.RoleName!.ToLower().Equals(RoleEnum.Teacher.ToString().ToLower()))
			{
				var teacher = new Teacher()
				{
					BaseUserId = baseUser.Id,
					Id = new UuidV7().Value,
					Verified = false
				};

				teacher = _mapper.Map(user.Teacher, teacher);

				await _unitOfWork.TeacherRepository.AddAsync(teacher);
			}
			else if (user.RoleName.ToLower().Equals(RoleEnum.Student.ToString().ToLower()))
			{
				var student = new Student()
				{
					BaseUserId = baseUser.Id,
					Id = new UuidV7().Value,
					Grade = 10
				};

				student = _mapper.Map(user.Student, student);

				await UpdatePersonalityFromCache(student, baseUser);

				await _unitOfWork.StudentRepository.AddAsync(student);
			}
		}
	}
}