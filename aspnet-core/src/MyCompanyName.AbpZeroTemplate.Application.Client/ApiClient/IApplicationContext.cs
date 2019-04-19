using Abp.Localization;
using Abp.Web.Models.AbpUserConfiguration;
using JetBrains.Annotations;
using MyCompanyName.AbpZeroTemplate.Sessions.Dto;

namespace MyCompanyName.AbpZeroTemplate.ApiClient
{
    public interface IApplicationContext
    {
        [CanBeNull]
        TenantInformation CurrentTenant { get; }

        AbpUserConfigurationDto Configuration { get; set; }

        GetCurrentLoginInformationsOutput LoginInfo { get; }

        void ClearLoginInfo();

        void SetLoginInfo(GetCurrentLoginInformationsOutput loginInfo);

        void SetAsHost();

        void SetAsTenant(string tenancyName, int tenantId);

        LanguageInfo CurrentLanguage { get; set; }

        void Load(TenantInformation currentTenant, GetCurrentLoginInformationsOutput loginInfo);
    }
}