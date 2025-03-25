using Microsoft.Extensions.Options;
using Net.payOS.Types;
using TransactionalService.Application.Common.Interfaces.PayOsInterface;
using TransactionalService.Core.Configurations;
using TransactionalService.Core.Models.RequestModels;

namespace TransactionalService.Application.Services.PayOsService;

public class PayOsService(IOptions<PayOsSettings> config) : IPayOsService
{
    private readonly Net.payOS.PayOS _payOs = new(
        config.Value.ClientId, config.Value.ApiKey, config.Value.ChecksumKey);

    private readonly string _checksumKey = config.Value.ChecksumKey;
    private const string ReturnUrl = "https://fish-ecomerce-fe.vercel.app/payos";

    public async Task<string> CreatePayment(PaymentRequestModel model)
    {
        var expired = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();

        var paymentData = new PaymentData(
            model.OrderCode,
            (int)model.TotalPrice,
            model.Description,
            [],
            ReturnUrl,
            ReturnUrl,
            "",
            model.FullName,
            "",
            "",
            "",
            expired
        );

        var result = await _payOs.createPaymentLink(paymentData);
        return result.checkoutUrl;
    }

    public async Task<PaymentLinkInformation> CancelPayment(long orderCode)
    {
        return await _payOs.cancelPaymentLink(orderCode);
    }

    public bool SignatureValidate(WebhookType webhookType)
    {
        try
        {
            _payOs.verifyPaymentWebhookData(webhookType);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}