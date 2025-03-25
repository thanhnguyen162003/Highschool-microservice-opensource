using Net.payOS.Types;
using TransactionalService.Core.Models.RequestModels;

namespace TransactionalService.Application.Common.Interfaces.PayOsInterface;

public interface IPayOsService
{
    Task<string> CreatePayment(PaymentRequestModel model);
    Task<PaymentLinkInformation> CancelPayment(long orderCode);
    bool SignatureValidate(WebhookType webhookType);
}