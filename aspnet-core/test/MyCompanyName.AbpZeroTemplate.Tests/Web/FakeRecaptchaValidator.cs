using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.Security.Recaptcha;

namespace MyCompanyName.AbpZeroTemplate.Tests.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
