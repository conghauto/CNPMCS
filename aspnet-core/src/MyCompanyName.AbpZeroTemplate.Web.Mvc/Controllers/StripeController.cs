using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe.Dto;
using MyCompanyName.AbpZeroTemplate.Web.Models.Stripe;

namespace MyCompanyName.AbpZeroTemplate.Web.Controllers
{
    public class StripeController : StripeControllerBase
    {
        private readonly StripePaymentGatewayConfiguration _stripeConfiguration;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IStripePaymentAppService _stripePaymentAppService;

        public StripeController(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration stripeConfiguration,
            IStripePaymentAppService stripePaymentAppService,
            ISubscriptionPaymentRepository subscriptionPaymentRepository)
            : base(stripeGatewayManager, stripeConfiguration)
        {
            _stripeConfiguration = stripeConfiguration;
            _stripePaymentAppService = stripePaymentAppService;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
        }

        public async Task<ActionResult> Purchase(long paymentId, bool isUpgrade = false)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            if (payment.Status != SubscriptionPaymentStatus.NotPaid)
            {
                throw new ApplicationException("This payment is processed before");
            }

            var model = new StripePurchaseViewModel
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Description = payment.Description,
                IsRecurring = payment.IsRecurring,
                Configuration = _stripeConfiguration,
                UpdateSubscription = isUpgrade
            };

            if (!payment.IsRecurring)
            {
                return View(model);
            }

            if (isUpgrade)
            {
                return View("UpdateSubscription", model);
            }

            return View("Subscribe", model);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmPayment(long paymentId, string stripeToken)
        {
            try
            {
                await _stripePaymentAppService.ConfirmPayment(new StripeConfirmPaymentInput
                {
                    PaymentId = paymentId,
                    StripeToken = stripeToken
                });

                var returnUrl = await GetSuccessUrlAsync(paymentId);
                return Redirect(returnUrl);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);

                var returnUrl = await GetErrorUrlAsync(paymentId);
                return Redirect(returnUrl);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateSubscription(long paymentId, string stripeToken)
        {
            try
            {
                await _stripePaymentAppService.CreateSubscription(new StripeCreateSubscriptionInput
                {
                    PaymentId = paymentId,
                    StripeToken = stripeToken
                });

                var returnUrl = await GetSuccessUrlAsync(paymentId);
                return Redirect(returnUrl);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);

                var returnUrl = await GetErrorUrlAsync(paymentId);
                return Redirect(returnUrl);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(long paymentId)
        {
            try
            {
                await _stripePaymentAppService.UpdateSubscription(new StripeUpdateSubscriptionInput
                {
                    PaymentId = paymentId
                });

                var returnUrl = await GetSuccessUrlAsync(paymentId);
                return Redirect(returnUrl);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);

                var returnUrl = await GetErrorUrlAsync(paymentId);
                return Redirect(returnUrl);
            }
        }

        private async Task<string> GetSuccessUrlAsync(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            return payment.SuccessUrl + (payment.SuccessUrl.Contains("?") ? "&" : "?") + "paymentId=" + paymentId;
        }

        private async Task<string> GetErrorUrlAsync(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            return payment.ErrorUrl + (payment.ErrorUrl.Contains("?") ? "&" : "?") + "paymentId=" + paymentId;
        }
    }
}