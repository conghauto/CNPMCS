using MyCompanyName.AbpZeroTemplate.Models.Tenants;
using MyCompanyName.AbpZeroTemplate.ViewModels;
using Xamarin.Forms;

namespace MyCompanyName.AbpZeroTemplate.Views
{
    public partial class TenantsView : ContentPage, IXamarinView
    {
        public TenantsView()
        {
            InitializeComponent();
        }

        private async void ListView_OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await ((TenantsViewModel)BindingContext).LoadMoreTenantsIfNeedsAsync(e.Item as TenantListModel);
        }
    }
}