using Application.Features.Authen.v1.AuthenWithGoogle;
using Application.Features.Authen.v1.ForgotPassword;
using Application.Features.Authen.v1.Login;
using Application.Features.Authen.v1.Register;
using Application.Features.Authen.v1.ResendOTP;
using Application.Features.Authen.v1.ResetPassword;
using Application.Features.Authen.v1.VerifyAccount;
using Carter;
using Domain.Common.Messages;
using System.Net;

namespace Application.Endpoints.v1;

public class AuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/authentication");

        group.MapPost("google", AuthenWithGoogle).WithName(nameof(AuthenWithGoogle));
        group.MapPost("google2", AuthenWithGoogle2).WithName(nameof(AuthenWithGoogle2));
        group.MapPost("login", Login).WithName(nameof(Login));
        group.MapPost("register", Register).WithName(nameof(Register));
        group.MapPost("verify", VerifyAccount).WithName(nameof(VerifyAccount));
        group.MapPost("resend", ResendOTP).WithName(nameof(ResendOTP));
        group.MapGet("verify-token", VerifyValidToken).WithName(nameof(VerifyValidToken));
        group.MapPost("reset-password", ResetPassword).WithName(nameof(ResetPassword));
        group.MapPost("forgot-password", ForgotPassword).WithName(nameof(ForgotPassword));
    }

    private static async Task<IResult> Login(LoginCommand loginCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(loginCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> Register(RegisterCommand registerCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(registerCommand, cancellationToken);

        if (result.Status == HttpStatusCode.Created)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> AuthenWithGoogle(AuthenWithGoogleCommand authenWithGoogleCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(authenWithGoogleCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> AuthenWithGoogle2(AuthenWithGoogleCommandV3 authenWithGoogleCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(authenWithGoogleCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> VerifyAccount(VerifyAccountCommand verifyAccountCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(verifyAccountCommand, cancellationToken);

        if (result.Status == HttpStatusCode.Created)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> ResendOTP(ResendOTPCommand resendOTPCommand, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(resendOTPCommand, cancellationToken);

        return Results.Ok(MessageCommon.SendMailSuccess);
    }

    private static IResult VerifyValidToken(IHttpContextAccessor httpContextAccessor)
    {
        var result = httpContextAccessor.HttpContext?.User.Identity!.IsAuthenticated;

        if ((bool)result!)
        {
            return Results.Ok();
        }

        return Results.Unauthorized();

    }

    private static async Task<IResult> ResetPassword(ResetPasswordCommand resetPasswordCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(resetPasswordCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);

    }

    private static async Task<IResult> ForgotPassword(ForgotPasswordCommand forgotPasswordCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(forgotPasswordCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

}