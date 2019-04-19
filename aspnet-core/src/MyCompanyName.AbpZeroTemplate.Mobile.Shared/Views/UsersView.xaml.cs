using MyCompanyName.AbpZeroTemplate.Models.Users;
using MyCompanyName.AbpZeroTemplate.ViewModels;
using Xamarin.Forms;

namespace MyCompanyName.AbpZeroTemplate.Views
{
    public partial class UsersView : ContentPage, IXamarinView
    {
        public UsersView()
        {
            InitializeComponent();
        }

        public async void ListView_OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await ((UsersViewModel) BindingContext).LoadMoreUserIfNeedsAsync(e.Item as UserListModel);
        }
    }
}