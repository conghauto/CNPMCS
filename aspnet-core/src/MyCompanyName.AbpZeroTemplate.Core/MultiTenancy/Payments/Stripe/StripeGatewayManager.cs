using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Threading;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Editions;
using Stripe;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe
{
    public class StripeGatewayManager : AbpZeroTemplateServiceBase, ISupportsRecurringPayments, ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;

        public static string ProductName = "AbpZeroTemplate";

        public StripeGatewayManager(
            TenantManager tenantManager,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            EditionManager editionManager)
        {
            _tenantManager = tenantManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _editionManager = editionManager;
        }

        public async Task<StripeIdResponse> CreateCharge(string source, decimal amount, string description)
        {
            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(new ChargeCreateOptions
            {
                SourceId = source,
                Amount = ConvertToStripePrice(amount),
                Description = description,
                Currency = AbpZeroTemplateConsts.Currency,
                Capture = true
            });

            if (!charge.Paid)
            {
                throw new UserFriendlyException(L("PaymentCouldNotCompleted"));
            }

            return new StripeIdResponse
            {
                Id = charge.Id
            };
        }

        public async Task<StripeIdResponse> CreateSubscribtion(string customerId, string planId)
        {
            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
            {
                CustomerId = customerId,
                Items = new List<SubscriptionItemOption>
                {
                    new SubscriptionItemOption
                    {
                        PlanId = planId
                    }
                }
            });

            return new StripeIdResponse
            {
                Id = subscription.Id
            };
        }

        public async Task UpdateSubscribtion(string subscriptionId, string newPlanId, decimal newAmount, string interval)
        {
            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.GetAsync(subscriptionId);

            var newPlan = await GetPlanAsync(newPlanId);
            if (newPlan.Id.IsNullOrEmpty())
            {
                await CreatePlanAsync(newPlanId, newAmount, interval, ProductName);
            }

            var oldPlanId = subscription.Items.Data[0].Plan.Id;

            await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Items = new List<SubscriptionItemUpdateOption> {
                    new SubscriptionItemUpdateOption {
                        Id = subscription.Items.Data[0].Id,
                        PlanId = newPlanId
                    }
                }
            });

            var invoiceService = new InvoiceService();
            var invoice = await invoiceService.CreateAsync(new InvoiceCreateOptions
            {
                SubscriptionId = subscription.Id,
                CustomerId = subscription.CustomerId
            });

            if (!invoice.Paid)
            {
                invoice = await invoiceService.PayAsync(invoice.Id, null);
                if (!invoice.Paid)
                {
                    await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
                    {
                        CancelAtPeriodEnd = false,
                        Items = new List<SubscriptionItemUpdateOption> {
                            new SubscriptionItemUpdateOption {
                                Id = subscription.Items.Data[0].Id,
                                PlanId = oldPlanId
                            }
                        }
                    });

                    throw new UserFriendlyException(L("PaymentCouldNotCompleted"));
                }
            }

            var lastRecurringPayment = await _subscriptionPaymentRepository.GetByGatewayAndPaymentIdAsync(SubscriptionPaymentGatewayType.Stripe, subscriptionId);
            var payment = await _subscriptionPaymentRepository.GetLastPaymentOrDefaultAsync(
                tenantId: lastRecurringPayment.TenantId,
                SubscriptionPaymentGatewayType.Stripe,
                isRecurring: true);

            payment.Amount = ConvertFromStripePrice(invoice.Total);
            payment.IsRecurring = false;
            payment.ExternalPaymentId = invoice.ChargeId;
            payment.SetAsPaid();
        }

        public async Task CancelSubscribtion(string subscriptionId)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.CancelAsync(subscriptionId, null);

            var payment = await _subscriptionPaymentRepository.GetByGatewayAndPaymentIdAsync(
                SubscriptionPaymentGatewayType.Stripe,
                subscriptionId
            );

            payment.Status = SubscriptionPaymentStatus.Cancelled;
        }

        public async Task<StripeIdResponse> CreatePlanAsync(string planId, decimal amount, string interval, string productId)
        {
            var planService = new PlanService();
            var plan = await planService.CreateAsync(new PlanCreateOptions
            {
                Id = planId,
                Amount = ConvertToStripePrice(amount),
                Interval = interval,
                ProductId = productId,
                Currency = AbpZeroTemplateConsts.Currency
            });

            return new StripeIdResponse
            {
                Id = plan.Id
            };
        }

        public async Task<StripeIdResponse> GetPlanAsync(string planId)
        {
            try
            {
                var planService = new PlanService();
                var plan = await planService.GetAsync(planId);

                return new StripeIdResponse
                {
                    Id = plan.Id
                };
            }
            catch (StripeException exception)
            {
                Logger.Error(exception.Message, exception);
                return new StripeIdResponse();
            }
        }

        public async Task<StripeIdResponse> GetProductAsync(string productId)
        {
            try
            {
                var productService = new ProductService();
                var product = await productService.GetAsync(productId);

                return new StripeIdResponse
                {
                    Id = product.Id
                };
            }
            catch (StripeException exception)
            {
                Logger.Error(exception.Message, exception);
                return new StripeIdResponse();
            }
        }

        public async Task<StripeIdResponse> CreateProductAsync(string name)
        {
            var productService = new ProductService();
            var product = await productService.CreateAsync(new ProductCreateOptions
            {
                Name = name,
                Type = "service"
            });

            return new StripeIdResponse
            {
                Id = product.Id
            };
        }

        public async Task<StripeIdResponse> CreateCustomerAsync(string description, string emailAddress, string source)
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Description = description,
                SourceToken = source,
                Email = emailAddress
            });

            return new StripeIdResponse
            {
                Id = customer.Id
            };
        }

        public string GetPlanInterval(PaymentPeriodType? paymentPeriod)
        {
            if (!paymentPeriod.HasValue)
            {
                throw new ArgumentNullException(nameof(paymentPeriod));
            }

            return paymentPeriod == PaymentPeriodType.Annual ? "year" : "month";
        }

        public void HandleEvent(RecurringPaymentsDisabledEventData eventData)
        {
            var subscriptionPayment = GetLastCompletedSubscriptionPayment(eventData.TenantId);

            int daysUntilDue;

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var edition = (SubscribableEdition)AsyncHelper.RunSync(() => _editionManager.GetByIdAsync(eventData.EditionId));
                daysUntilDue = edition.WaitingDayAfterExpire ?? 3;
            }

            var subscriptionService = new SubscriptionService();
            subscriptionService.Update(subscriptionPayment.ExternalPaymentId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Billing = Billing.SendInvoice,
                DaysUntilDue = daysUntilDue
            });
        }

        public void HandleEvent(RecurringPaymentsEnabledEventData eventData)
        {
            var subscriptionPayment = GetLastCompletedSubscriptionPayment(eventData.TenantId);

            if (subscriptionPayment == null || subscriptionPayment.ExternalPaymentId.IsNullOrEmpty())
            {
                return;
            }

            var subscriptionService = new SubscriptionService();
            subscriptionService.Update(subscriptionPayment.ExternalPaymentId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Billing = Billing.ChargeAutomatically
            });
        }

        public void HandleEvent(TenantEditionChangedEventData eventData)
        {
            if (!eventData.OldEditionId.HasValue)
            {
                return;
            }

            var subscriptionPayment = GetLastCompletedSubscriptionPayment(eventData.TenantId);

            if (subscriptionPayment == null || subscriptionPayment.ExternalPaymentId.IsNullOrEmpty())
            {
                // no subscription on stripe !
                return;
            }

            if (!eventData.NewEditionId.HasValue)
            {
                AsyncHelper.RunSync(() => CancelSubscribtion(subscriptionPayment.ExternalPaymentId));
                return;
            }

            string newPlanId;
            decimal? newPlanAmount;

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var edition = (SubscribableEdition)AsyncHelper.RunSync(() => _editionManager.GetByIdAsync(eventData.NewEditionId.Value));
                newPlanId = GetPlanId(edition.Name, subscriptionPayment.GetPaymentPeriodType());
                newPlanAmount = edition.GetPaymentAmountOrNull(subscriptionPayment.PaymentPeriodType);
            }

            if (!newPlanAmount.HasValue || newPlanAmount.Value == 0)
            {
                AsyncHelper.RunSync(() => CancelSubscribtion(subscriptionPayment.ExternalPaymentId));
                return;
            }

            AsyncHelper.RunSync(async () =>
                {
                    await _subscriptionPaymentRepository.InsertAndGetIdAsync(
                        new SubscriptionPayment
                        {
                            TenantId = eventData.TenantId,
                            Amount = 0,
                            PaymentPeriodType = subscriptionPayment.GetPaymentPeriodType(),
                            Description = $"Edition change by admin from {eventData.OldEditionId} to {eventData.NewEditionId}",
                            EditionId = eventData.NewEditionId.Value,
                            Gateway = SubscriptionPaymentGatewayType.Stripe,
                            DayCount = subscriptionPayment.DayCount,
                            IsRecurring = true,
                            Status = SubscriptionPaymentStatus.NotPaid
                        });

                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            );

            AsyncHelper.RunSync(() => UpdateSubscribtion(
                subscriptionPayment.ExternalPaymentId,
                newPlanId,
                newPlanAmount.Value,
                GetPlanInterval(subscriptionPayment.PaymentPeriodType))
            );
        }

        public async Task HandleInvoicePaymentSucceededAsync(Invoice invoice)
        {
            var customerService = new CustomerService();
            var customer = await customerService.GetAsync(invoice.CustomerId);

            int tenantId;
            int editionId;

            PaymentPeriodType? paymentPeriodType = null;

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await _tenantManager.FindByTenancyNameAsync(customer.Description);
                tenantId = tenant.Id;
                editionId = tenant.EditionId.Value;

                using (CurrentUnitOfWork.SetTenantId(tenantId))
                {
                    var lastPayment = GetLastCompletedSubscriptionPayment(tenantId);
                    paymentPeriodType = lastPayment.GetPaymentPeriodType();
                }

                await _tenantManager.UpdateTenantAsync(
                    tenant.Id,
                    isActive: true,
                    isInTrialPeriod: false,
                    paymentPeriodType,
                    tenant.EditionId.Value,
                    EditionPaymentType.Extend);
            }

            await _subscriptionPaymentRepository.InsertAsync(new SubscriptionPayment
            {
                TenantId = tenantId,
                Amount = ConvertFromStripePrice(invoice.AmountPaid),
                DayCount = (int)paymentPeriodType,
                PaymentPeriodType = paymentPeriodType,
                EditionId = editionId,
                ExternalPaymentId = invoice.ChargeId,
                Gateway = SubscriptionPaymentGatewayType.Stripe,
                IsRecurring = true,
                Status = SubscriptionPaymentStatus.Paid
            });
        }

        public string GetPlanId(string editionName, PaymentPeriodType paymentPeriodType)
        {
            return editionName + "_" + paymentPeriodType;
        }

        private long ConvertToStripePrice(decimal amount)
        {
            return Convert.ToInt64(amount * 100);
        }

        private decimal ConvertFromStripePrice(long amount)
        {
            return Convert.ToDecimal(amount) / 100;
        }

        private SubscriptionPayment GetLastCompletedSubscriptionPayment(int tenantId)
        {
            return AsyncHelper.RunSync(() =>
                {
                    return _subscriptionPaymentRepository.GetAll()
                          .LastOrDefaultAsync(p =>
                              p.TenantId == tenantId &&
                              p.Status == SubscriptionPaymentStatus.Completed &&
                              p.Gateway == SubscriptionPaymentGatewayType.Stripe &&
                              p.ExternalPaymentId.StartsWith("sub"));
                }
            );
        }
    }
}