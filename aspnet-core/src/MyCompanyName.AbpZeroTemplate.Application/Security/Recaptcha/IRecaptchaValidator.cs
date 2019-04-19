using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}