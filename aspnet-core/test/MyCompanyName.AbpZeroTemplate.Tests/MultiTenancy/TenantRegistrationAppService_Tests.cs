using System;
using System.Threading.Tasks;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.MultiTenancy;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using Shouldly;
using Xunit;

namespace MyCompanyName.AbpZeroTemplate.Tests.MultiTenancy
{
    public class TenantRegistrationAppService_Tests : AppTestBase
    {
        private readonly ITenantRegistrationAppService _tenantRegistrationAppService;
        
        public TenantRegistrationAppService_Tests()
        {
            _tenantRegistrationAppService = Resolve<ITenantRegistrationAppService>();
        }

        [MultiTenantFact]
        public async Task SubscriptionEndDateUtc_ShouldBe_Null_After_Free_Registration()
        {
            //Arrange
            var edition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(edition);
                await context.SaveChangesAsync();
            });

            //Act
            var registerResult = await _tenantRegistrationAppService.RegisterTenant(new RegisterTenantInput
            {
                EditionId = edition.Id,
                AdminEmailAddress = "admin@volosoft.com",
                AdminPassword = "123qwe",
                Name = "Volosoft",
                SubscriptionStartType = SubscriptionStartType.Free,
                TenancyName = "Volosoft"
            });

            //Assert
            await UsingDbContextAsync(async context =>
            {
                var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == registerResult.TenantId);
                tenant.ShouldNotBe(null);
                tenant.SubscriptionEndDateUtc.HasValue.ShouldBe(false);
            });
        }

        [MultiTenantFact]
        public async Task Cannot_Register_To_Free_Edition_As_Trial()
        {
            //Arrange
            var edition = new SubscribableEdition
            {
                DisplayName = "Free Edition"
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(edition);
                await context.SaveChangesAsync();
            });

            var exception = await Assert.ThrowsAsync<Exception>(async () => await _tenantRegistrationAppService.RegisterTenant(new RegisterTenantInput
            {
                EditionId = edition.Id,
                AdminEmailAddress = "admin@volosoft.com",
                AdminPassword = "123qwe",
                Name = "Volosoft",
                SubscriptionStartType = SubscriptionStartType.Trial,
                TenancyName = "Volosoft"
            }));

            exception.Message.ShouldBe("Trial is not available for this edition !");
        }

        [MultiTenantFact]
        public async Task Should_Subscribe_To_Edition_As_Trial_If_Trial_Is_Available()
        {
            //Arrange
            var utcNow = Clock.Now.ToUniversalTime();
            var trialDayCount = 10;
            var edition = new SubscribableEdition
            {
                DisplayName = "Standard Edition",
                TrialDayCount = trialDayCount,
                MonthlyPrice = 9,
                AnnualPrice = 99
            };

            await UsingDbContextAsync(async context =>
            {
                context.SubscribableEditions.Add(edition);
                await context.SaveChangesAsync();
            });

            var result = await _tenantRegistrationAppService.RegisterTenant(new RegisterTenantInput
            {
                EditionId = edition.Id,
                AdminEmailAddress = "admin@volosoft.com",
                AdminPassword = "123qwe",
                Name = "Volosoft",
                SubscriptionStartType = SubscriptionStartType.Trial,
                TenancyName = "Volosoft"
            });

            //Assert
            await UsingDbContextAsync(async context =>
            {
                var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == result.TenantId);
                tenant.ShouldNotBe(null);
                tenant.SubscriptionEndDateUtc.HasValue.ShouldBe(true);
                tenant.SubscriptionEndDateUtc.Value.Date.ShouldBe(utcNow.Date.AddDays(trialDayCount));
            });
        }
    }
}
