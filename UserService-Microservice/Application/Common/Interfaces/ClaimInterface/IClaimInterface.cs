namespace Domain.Common.Interfaces.ClaimInterface;

public interface IClaimInterface
{
	public Guid GetCurrentUserId { get; }
	public string GetCurrentUsername { get; }
	public string GetCurrentEmail { get; }
	public string GetRole { get; }
}