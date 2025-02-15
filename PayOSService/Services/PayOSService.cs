using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PayOSService.Config;
using PayOSService.DTO;

namespace PayOSService.Services
{
    public class PayOSService(IOptions<PayOSConfig> payosConfigOptions) : IPayOSService
    {
        private readonly PayOSConfig _payOSConfig = payosConfigOptions.Value;

        public async Task<string> CreatePaymentAsync(CreatePaymentDTO createPaymentDTO)
        {
            var clientId = _payOSConfig.ClientId;
            var apiKey = _payOSConfig.ApiKey;
            var checksumKey = _payOSConfig.ChecksumKey;
            var returnUrl = _payOSConfig.ReturnUrl;

            var payOS = new PayOS(clientId, apiKey, checksumKey);

            var paymentLinkRequest = new PaymentData(
                orderCode: createPaymentDTO.OrderCode,
                amount: createPaymentDTO.RequiredAmount,
                description: createPaymentDTO.Content,
                items: [],
                returnUrl: returnUrl,
                cancelUrl: returnUrl
            );
            var response = await payOS.createPaymentLink(paymentLinkRequest);

            return response.checkoutUrl;
        }
    }
}
