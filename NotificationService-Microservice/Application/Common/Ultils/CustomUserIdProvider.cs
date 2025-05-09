using Application.Common.Interfaces.ClaimInterface;
using Microsoft.AspNetCore.SignalR;

namespace Application.Common.Ultils;

public class CustomUserIdProvider(IClaimInterface claimInterface) : IUserIdProvider
{
    private readonly IClaimInterface _claimInterface = claimInterface;

    public string? GetUserId(HubConnectionContext connection)
    {
        var userId = _claimInterface.GetCurrentUserId;

        // Return the UserId as a string, or null if it doesn't exist
        return userId.ToString();
    }
}
