using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Enumerations;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Application.Features.User.UpdateNewUser
{
    public class UpdateNewUserCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<UpdateNewUserCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<ResponseModel> Handle(UpdateNewUserCommand request, CancellationToken cancellationToken)
        {
            var roleId = _authenticationService.GetRoleId();
            var user = await _unitOfWork.UserRepository.GetDetailUser(request.UserId);

            if (user == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageUser.UserNotFound,
                    Data = request.UserId
                };
            }

            if(roleId.Equals(((int)RoleEnum.Student).ToString()))
            {
                switch (user.ProgressStage!.ConvertToValue<ProgressStage>())
                {
                    case ProgressStage.NewUser:
                        if(user.RoleId != 0)
                        {
                            user.ProgressStage = ProgressStage.SubjectInformation.ToString();
                        } else
                        {
                            return new ResponseModel
                            {
                                Status = HttpStatusCode.BadRequest,
                                Message = MessageUser.YouNeedToChoseRoleBefore
                            };
                        }
                        break;
                    case ProgressStage.SubjectInformation:
                        if ((user.UserSubjects.Count > 0) && (!string.IsNullOrEmpty(user.Student!.TypeExam)))
                        {
                            user.ProgressStage = ProgressStage.PersonalityAssessment.ToString();
                        } else
                        {
                            return new ResponseModel
                            {
                                Status = HttpStatusCode.BadRequest,
                                Message = MessageUser.YouNeedToChoseSubjectAndTypeExamBefore
                            };
                        }
                        break;
                    case ProgressStage.PersonalityAssessment:
                        if(!string.IsNullOrEmpty(user.Student!.HollandType) && (user.Student.MbtiType != null))
                        {
                            user.ProgressStage = ProgressStage.Completion.ToString();
                        } else
                        {
                            return new ResponseModel
                            {
                                Status = HttpStatusCode.BadRequest,
                                Message = MessageUser.YouNeedToTestHollandAndMBTIBefore
                            };
                        }
                        break;
                    case ProgressStage.Completion:
                        return new ResponseModel
                        {
                            Status = HttpStatusCode.BadRequest,
                            Message = MessageUser.YouHaveBeenCompleteSetup
                        };
                    default:
                        return new ResponseModel
                        {
                            Status = HttpStatusCode.BadRequest,
                            Message = MessageCommon.UpdateFailed
                        };
                }

            } else if(roleId.Equals(((int)RoleEnum.Teacher).ToString()))
            {
                switch (user.ProgressStage!.ConvertToValue<ProgressStage>())
                {
                    case ProgressStage.NewUser:
                        if (user.RoleId != 0)
                        {
                            user.ProgressStage = ProgressStage.SubjectInformation.ToString();
                        } else
                        {
                            return new ResponseModel
                            {
                                Status = HttpStatusCode.BadRequest,
                                Message = MessageUser.YouNeedToChoseRoleBefore
                            };
                        }
                        break;
                    case ProgressStage.SubjectInformation:
                        if ((user.UserSubjects.Count > 0) && (!string.IsNullOrEmpty(user.Teacher!.GraduatedUniversity) && (!string.IsNullOrEmpty(user.Teacher!.WorkPlace))))
                        {
                            user.ProgressStage = ProgressStage.VerifyTeacher.ToString();
                        } else
                        {
                            return new ResponseModel
                            {
                                Status = HttpStatusCode.BadRequest,
                                Message = MessageUser.YouNeedToChoseSubjectAndUniversityAndWorkPlaceBefore
                            };
                        }
                        break;
                    case ProgressStage.VerifyTeacher:
                        if (user.Teacher!.Verified)
                        {
                            user.ProgressStage = ProgressStage.Completion.ToString();
                        } else
                        {
                            return new ResponseModel
                            {
                                Status = HttpStatusCode.BadRequest,
                                Message = MessageUser.YouNeedWaitToVerifyIsTeacherBefore
                            };
                        }
                        break;
                    case ProgressStage.Completion:
                        return new ResponseModel
                        {
                            Status = HttpStatusCode.BadRequest,
                            Message = MessageUser.YouHaveBeenCompleteSetup
                        };
                    default:
                        return new ResponseModel
                        {
                            Status = HttpStatusCode.BadRequest,
                            Message = MessageCommon.UpdateFailed
                        };
                }
            } else
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageUser.RoleNotSupport
                };
            }

            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.SaveChangesAsync())
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.UpdateFailed,
                Data = user
            };

        }
    }
}
