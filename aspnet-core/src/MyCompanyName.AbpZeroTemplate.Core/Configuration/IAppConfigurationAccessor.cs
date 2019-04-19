using Microsoft.Extensions.Configuration;

namespace MyCompanyName.AbpZeroTemplate.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
