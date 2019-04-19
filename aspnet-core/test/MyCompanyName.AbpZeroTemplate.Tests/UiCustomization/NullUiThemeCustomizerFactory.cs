using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.UiCustomization;

namespace MyCompanyName.AbpZeroTemplate.Tests.UiCustomization
{
    public class NullUiThemeCustomizerFactory : IUiThemeCustomizerFactory
    {
        public async Task<IUiCustomizer> GetCurrentUiCustomizer()
        {
            return new NullThemeUiCustomizer();
        }

        public IUiCustomizer GetUiCustomizer(string theme)
        {
            return new NullThemeUiCustomizer();
        }
    }
}
