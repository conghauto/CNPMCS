using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using MyCompanyName.AbpZeroTemplate.Configuration.Dto;
using MyCompanyName.AbpZeroTemplate.UiCustomization.Dto;

namespace MyCompanyName.AbpZeroTemplate.UiCustomization
{
    public interface IUiCustomizer : ISingletonDependency
    {
        Task<UiCustomizationSettingsDto> GetUiSettings();

        Task UpdateUserUiManagementSettingsAsync(UserIdentifier user, ThemeSettingsDto settings);

        Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings);

        Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings);

        Task<ThemeSettingsDto> GetHostUiManagementSettings();

        Task<ThemeSettingsDto> GetTenantUiCustomizationSettings(int tenantId);
    }
}
