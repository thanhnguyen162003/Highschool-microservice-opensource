namespace TransactionalService.Core.Models.RequestModels;

public class PaymentRequestModel
{
    public long OrderCode { get; set; }
    public decimal TotalPrice { get; set; }
    public string Description { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}