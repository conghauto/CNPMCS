using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Identity
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}