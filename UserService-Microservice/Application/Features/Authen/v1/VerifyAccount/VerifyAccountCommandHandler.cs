using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Services.ServiceTask.CacheCommon;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.VerifyAccount
{
    public class VerifyAccountCommandHandler(ICacheRepository cacheRepository, IUnitOfWork unitOfWork, ICacheDataTask cacheDataTask, CareerMongoDatabaseContext context) : IRequestHandler<VerifyAccountCommand, ResponseModel>
    {
        private readonly ICacheRepository _cacheRepository = cacheRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheDataTask _cacheDataTask = cacheDataTask;
        private readonly CareerMongoDatabaseContext _context = context;

        public async Task<ResponseModel> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
        {
            var fullKeyOTP = $"{request.Email}:OTP";
            var otp = await _cacheRepository.GetAsync<dynamic>(StorageRedis.VerifyAccount, fullKeyOTP);
            if (otp == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageUser.UserNotFound
                };
            }
            else if (otp.Expire < DateTime.Now)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageUser.OTPExpired
                };
            }
            else if (otp.Otp != request.OTP)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageUser.OTPNotValid
                };
            }

            var fullKey = $"{request.Email}:information";
            var user = await _cacheRepository.GetAsync<BaseUser>(StorageRedis.VerifyAccount, fullKey);

            if (user == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = MessageCommon.CreateFailed,
                    Data = user?.Id
                };
            }

            // Add user to database
            await _unitOfWork.UserRepository.AddAsync(user);

            // Create profile 
            if (user.RoleId == (int)RoleEnum.Teacher)
            {
                await CreateIfNotExistTeacher(user.Id);
            }
            else if (user.RoleId == (int)RoleEnum.Student)
            {
                await CreateIfNotExistStudent(user.Id);
            }

            // Remove cache account to verify
            _cacheDataTask.RemoveAccountToVerify(user);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully,
                    Data = user.Id
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.ServerError
            };
        }

        private async Task CreateIfNotExistTeacher(Guid userId)
        {
            var teacher = await _unitOfWork.TeacherRepository.GetTeacherByUserId(userId);

            if (teacher == null)
            {
                teacher = new Teacher
                {
                    BaseUserId = userId,
                    Id = new UuidV7().Value,
                    Verified = false
                };

                await _unitOfWork.TeacherRepository.AddAsync(teacher);

                return;
            }
        }

        private async Task CreateIfNotExistStudent(Guid userId)
        {
            var student = await _unitOfWork.StudentRepository.GetStudentByUserId(userId);

            if (student == null)
            {
                student = new Student
                {
                    BaseUserId = userId,
                    Id = new UuidV7().Value,
                    Grade = 10
                };

                await _unitOfWork.StudentRepository.AddAsync(student);

                return;
            }

            _unitOfWork.StudentRepository.Update(student);
        }
    }
}
