using System.Collections.Generic;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    public interface IPaymentGatewayStore
    {
        List<PaymentGatewayModel> GetActiveGateways();
    }
}
