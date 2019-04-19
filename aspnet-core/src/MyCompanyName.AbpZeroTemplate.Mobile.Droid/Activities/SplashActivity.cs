using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Flurl.Http;
using MyCompanyName.AbpZeroTemplate.ApiClient;
using MyCompanyName.AbpZeroTemplate.Core;
using MyCompanyName.AbpZeroTemplate.Localization.Resources;
using MyCompanyName.AbpZeroTemplate.ViewModels.Base;
using Plugin.Connectivity;
using Process = Android.OS.Process;

namespace MyCompanyName.AbpZeroTemplate.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash",
        MainLauncher = true,
        NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        // Launches the startup task
        protected override async void OnResume()
        {
            base.OnResume();
            ApplicationBootstrapper.InitializeIfNeeds<AbpZeroTemplateXamarinAndroidModule>();
            await CheckInternetAndStartApplication();
        }

        public override void OnBackPressed()
        {
            // Prevent the back button from canceling the startup process
        }

        private async Task CheckInternetAndStartApplication()
        {
            if (CrossConnectivity.Current.IsConnected || ApiUrlConfig.IsLocal)
            {
                await StartApplication();
            }
            else
            {
                var isTryAgain = await UserDialogs.Instance.ConfirmAsync(LocalTranslation.NoInternet, LocalTranslation.MessageTitle);
                if (!isTryAgain)
                {
                    App.ExitApplication();
                }

                await CheckInternetAndStartApplication();
            }
        }

        /// <summary>
        ///  Performing some startup work that takes a bit of time
        /// </summary>
        private async Task StartApplication()
        {
            App.LoadPersistedSession();

            await UserConfigurationManager.GetIfNeedsAsync();

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UserDialogs.Init(this);
            ConfigureFlurlHttp();
            SetExitAction();
        }

        private static void SetExitAction()
        {
            App.ExitApplication = () =>
            {
                Process.KillProcess(Process.MyPid());
            };
        }

        private static void ConfigureFlurlHttp()
        {
            var modernHttpClientFactory = new ModernHttpClientFactory
            {
                OnSessionTimeOut = App.OnSessionTimeout,
                OnAccessTokenRefresh = App.OnAccessTokenRefresh
            };

            FlurlHttp.Configure(c =>
            {
                c.HttpClientFactory = modernHttpClientFactory;
            });
        }
    }
}