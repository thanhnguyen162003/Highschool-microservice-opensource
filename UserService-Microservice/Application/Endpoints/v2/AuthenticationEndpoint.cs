using Application.Features.Authen.v1.Logout;
using Application.Features.Authen.v2.Login;
using Application.Features.Authen.v2.LoginWithGoogle;
using Application.Features.Authen.v2.RefreshToken;
using Application.Features.Authen.v2.VerifyAccount;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Application.Endpoints.v2;


public class AuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v2/authentication");

        group.MapPost("login", MagicLogin).WithName(nameof(MagicLogin));
        group.MapPost("verify-account", VerifyAccountLogin).WithName(nameof(VerifyAccountLogin));
        group.MapPost("refresh-token", RefreshToken).WithName(nameof(RefreshToken));
        group.MapPost("google", LoginWithGoogle).WithName(nameof(LoginWithGoogle));
        group.MapPost("logout", Logout).WithName(nameof(Logout));
    }

    private static async Task<IResult> MagicLogin(MagicLoginCommand loginCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(loginCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> VerifyAccountLogin([FromBody] VerifyAccountLoginCommand verifyAccount, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(verifyAccount, cancellationToken);

        if(result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(refreshTokenCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> LoginWithGoogle([FromBody] LoginWithGoogleCommand loginWithGoogleCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(loginWithGoogleCommand, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    private static async Task<IResult> Logout([FromBody] LogoutCommand logoutCommand, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(logoutCommand, cancellationToken);
        if (result.Status == HttpStatusCode.OK)
        {
            return Results.Ok(result);
        }
        return Results.BadRequest(result);
    }

}