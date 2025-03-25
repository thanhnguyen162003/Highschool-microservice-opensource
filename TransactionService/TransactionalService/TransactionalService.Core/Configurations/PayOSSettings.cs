namespace TransactionalService.Core.Configurations;

public class PayOsSettings
{
    public required string ClientId { get; set; }
    
    public required string ApiKey { get; set; }
    
    public string ChecksumKey { get; set; }
}