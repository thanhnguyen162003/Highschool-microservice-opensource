using Application.Common.Models.Common;
using Application.Features.Authen.v1.AuthenWithGoogle.LoginWithGoogle;
using Application.Features.Authen.v1.AuthenWithGoogle.RegisterWithGoogle;
using Domain.Services.Authentication;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.Authen.v1.AuthenWithGoogle
{
    public class AuthenWithGoogleCommandHandlerV3(ISender mediator, IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<AuthenWithGoogleCommandV3, ResponseModel>
    {
        private readonly ISender _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<ResponseModel> Handle(AuthenWithGoogleCommandV3 request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.ValidateGoogleToken2(request.AccessToken!, request.Email!);

            if (result.Status != HttpStatusCode.OK)
            {
                return result;
            }

            var user = await _unitOfWork.UserRepository.IsExistEmail(request.Email!);

            if (user)
            {
                return await _mediator.Send(new LoginWithGoogleCommand
                {
                    Email = request.Email!,
                    AccessToken = request.AccessToken!
                }, cancellationToken);
            }

            return await _mediator.Send(new RegisterWithGoogleCommand
            {
                Email = request.Email!,
                AccessToken = request.AccessToken!,
                Avatar = request.Avatar,
                FullName = request.FullName
            }, cancellationToken);
        }
    }
}
