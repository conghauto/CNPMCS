using System;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using MyCompanyName.AbpZeroTemplate.Authorization.Users;
using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe.Dto;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    public class StripePaymentAppService : AbpZeroTemplateAppServiceBase, IStripePaymentAppService
    {
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly StripeGatewayManager _stripeGatewayManager;
        private readonly StripePaymentGatewayConfiguration _strieStripePaymentGatewayConfiguration;
        private readonly IRepository<SubscribableEdition> _editionRepository;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<User, long> _userRepository;

        public StripePaymentAppService(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration strieStripePaymentGatewayConfiguration,
            IRepository<SubscribableEdition> editionRepository,
            TenantManager tenantManager,
            IRepository<User, long> userRepository,
            ISubscriptionPaymentRepository subscriptionPaymentRepository)
        {
            _stripeGatewayManager = stripeGatewayManager;
            _strieStripePaymentGatewayConfiguration = strieStripePaymentGatewayConfiguration;
            _editionRepository = editionRepository;
            _tenantManager = tenantManager;
            _userRepository = userRepository;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
        }

        public async Task ConfirmPayment(StripeConfirmPaymentInput input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(input.PaymentId);
            if (payment.Status != SubscriptionPaymentStatus.NotPaid)
            {
                throw new AbpException($"Invalid payment status {payment.Status}, cannot create a charge on stripe !");
            }

            var result = await _stripeGatewayManager.CreateCharge(input.StripeToken, payment.Amount, payment.Description);

            payment.Gateway = SubscriptionPaymentGatewayType.Stripe;
            payment.ExternalPaymentId = result.Id;
            payment.SetAsPaid();
        }

        public async Task CreateSubscription(StripeCreateSubscriptionInput input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(input.PaymentId);
            var edition = await _editionRepository.GetAsync(payment.EditionId);

            var product = await _stripeGatewayManager.GetProductAsync(StripeGatewayManager.ProductName);
            if (product.Id.IsNullOrEmpty())
            {
                product = await _stripeGatewayManager.CreateProductAsync(StripeGatewayManager.ProductName);
            }

            var planId = _stripeGatewayManager.GetPlanId(edition.Name, payment.GetPaymentPeriodType());
            var plan = await _stripeGatewayManager.GetPlanAsync(planId);

            if (plan.Id.IsNullOrEmpty())
            {
                var planInterval = _stripeGatewayManager.GetPlanInterval(payment.PaymentPeriodType);
                plan = await _stripeGatewayManager.CreatePlanAsync(planId, payment.Amount, planInterval, product.Id);
            }

            Tenant tenant;

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                tenant = await _tenantManager.GetByIdAsync(payment.TenantId);
            }

            var adminUser = await _userRepository.FirstOrDefaultAsync(u => u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                throw new ApplicationException("There is no admin user for current Tenant");
            }

            var customer = await _stripeGatewayManager.CreateCustomerAsync(tenant.TenancyName, adminUser.EmailAddress, input.StripeToken);

            var subscriptionResult = await _stripeGatewayManager.CreateSubscribtion(customer.Id, plan.Id);

            payment.Gateway = SubscriptionPaymentGatewayType.Stripe;
            payment.ExternalPaymentId = subscriptionResult.Id;
            payment.SetAsPaid();
        }

        public async Task UpdateSubscription(StripeUpdateSubscriptionInput input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(input.PaymentId);
            var edition = await _editionRepository.GetAsync(payment.EditionId);

            var lastPayment = await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(
                tenantId: AbpSession.GetTenantId(),
                SubscriptionPaymentGatewayType.Stripe,
                isRecurring: true);

            if (lastPayment == null)
            {
                throw new ApplicationException("You don't have a valid subscription !");
            }

            var newPlanId = _stripeGatewayManager.GetPlanId(edition.Name, lastPayment.GetPaymentPeriodType());

            var paymentPeriodType = payment.GetPaymentPeriodType();
            await _stripeGatewayManager.UpdateSubscribtion(lastPayment.ExternalPaymentId, newPlanId, edition.GetPaymentAmount(paymentPeriodType), _stripeGatewayManager.GetPlanInterval(paymentPeriodType));
        }

        public StripeConfigurationDto GetConfiguration()
        {
            return new StripeConfigurationDto
            {
                PublishableKey = _strieStripePaymentGatewayConfiguration.PublishableKey
            };
        }
    }
}