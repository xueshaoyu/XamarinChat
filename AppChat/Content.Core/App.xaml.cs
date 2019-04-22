using System;
using System.Threading;
using Android.App;
using Chat.Model;
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
            AlertDialog.Builder builder = new AlertDialog.Builder(App.MainActivity);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle("Waring");
            alertDialog.SetMessage("Confirmation of exit the App?");
            alertDialog.SetButton("Yes",async (p1, p2) =>
            {
                if (CurrentUser != null && CurrentUser.Id > 0)
                {
                    HttpClientHelper client = new HttpClientHelper();
                    client.Offline(CurrentUser);
                    await MQTTHelper.Instance.Offline(CurrentUser);
                    Thread.Sleep(50);
                }
                DependencyService.Get<ICloseAppService>().CloseApp();

            });
            alertDialog.SetButton2("Cancel", (p1, p2) =>
            {
                // Application.Current.Quit();

            });
            //RunOnUiThread(()=
            //{

            //});
            alertDialog.Show();
            
        }
    }
}
