using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.Views;
using Xamarin.Forms;

namespace MyCompanyName.AbpZeroTemplate.Services.Modal
{
    public interface IModalService
    {
        Task ShowModalAsync(Page page);

        Task ShowModalAsync<TView>(object navigationParameter) where TView : IXamarinView;

        Task<Page> CloseModalAsync();
    }
}
