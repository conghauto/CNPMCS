using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.ApiClient.Models;

namespace MyCompanyName.AbpZeroTemplate.Services.Account
{
    public interface IAccountService
    {
        AbpAuthenticateModel AbpAuthenticateModel { get; set; }
        
        AbpAuthenticateResultModel AuthenticateResultModel { get; set; }
        
        Task LoginUserAsync();

        Task LogoutAsync();
    }
}
