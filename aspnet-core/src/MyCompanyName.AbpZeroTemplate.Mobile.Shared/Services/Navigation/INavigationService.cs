using System;
using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.Views;
using Xamarin.Forms;

namespace MyCompanyName.AbpZeroTemplate.Services.Navigation
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task SetMainPage<TView>(object navigationParameter = null, bool clearNavigationHistory = false)
            where TView : IXamarinView;

        Task SetDetailPageAsync(Type viewType, object navigationParameter = null, bool pushToStack = false);

        Task<Page> GoBackAsync();
    }
}