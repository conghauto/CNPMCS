using Xamarin.Forms.Internals;

namespace MyCompanyName.AbpZeroTemplate.Behaviors
{
    [Preserve(AllMembers = true)]
    public interface IAction
    {
        bool Execute(object sender, object parameter);
    }
}