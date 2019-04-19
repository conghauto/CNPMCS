using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyCompanyName.AbpZeroTemplate.Configuration;
using MyCompanyName.AbpZeroTemplate.Web;

namespace MyCompanyName.AbpZeroTemplate.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AbpZeroTemplateDbContextFactory : IDesignTimeDbContextFactory<AbpZeroTemplateDbContext>
    {
        public AbpZeroTemplateDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AbpZeroTemplateDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);

            AbpZeroTemplateDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AbpZeroTemplateConsts.ConnectionStringName));

            return new AbpZeroTemplateDbContext(builder.Options);
        }
    }
}