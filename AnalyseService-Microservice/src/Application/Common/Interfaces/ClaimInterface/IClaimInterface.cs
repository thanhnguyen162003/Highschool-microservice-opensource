namespace Application.Common.Interfaces.ClaimInterface;

public interface IClaimInterface
{
    public Guid GetCurrentUserId {  get; }
    public string GetCurrentEmail {  get; }
    public string GetRole { get; }
}
