using System;
using Android.App;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Content.Core
{
    public partial class App : Application
    {
        public static IUserPreferences UserPreferences { get; private set; }

        public static void Init(IUserPreferences userPreferencesImpl)
        {
            App.UserPreferences = userPreferencesImpl;
        }
        public static UserInfo CurrentUser { get; set; }
        public static Activity MainActivity { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(Login.Instance);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static void Exit()
        {
            if (CurrentUser != null && CurrentUser.Guid == "")
            {
                var task = MQTTHelper.Instance.Offline(CurrentUser);
                if (task.Result)
                {


                }

            }
            Application.Current.Quit();
        }
    }
}
