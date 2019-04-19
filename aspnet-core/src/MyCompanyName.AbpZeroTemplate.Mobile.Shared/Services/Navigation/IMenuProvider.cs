using System.Collections.Generic;
using MvvmHelpers;
using MyCompanyName.AbpZeroTemplate.Models.NavigationMenu;

namespace MyCompanyName.AbpZeroTemplate.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}