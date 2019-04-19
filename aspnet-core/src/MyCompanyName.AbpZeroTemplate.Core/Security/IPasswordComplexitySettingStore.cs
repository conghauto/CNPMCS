using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
