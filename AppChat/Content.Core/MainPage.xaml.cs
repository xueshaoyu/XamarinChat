using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Content.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private static MainPage instance;
        public static MainPage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainPage();
                }

                return instance;
            }
        }
        private MainPage()
        {
            InitializeComponent();
            Init();
        }

        public ObservableCollection<UserInfo> Users
        {
            get; set;
        } = new ObservableCollection<UserInfo>();
        private void Init()
        {
            Users.Add(new UserInfo { Name = "1" });
            Users.Add(new UserInfo { Name = "11" });
            Users.Add(new UserInfo { Name = "111" });
            Users.Add(new UserInfo { Name = "1111" });
            UserList.ItemsSource = Users;
        }

        private void Exit_App_Clicked(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(App.MainActivity);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle("Waring");
            alertDialog.SetMessage("Confirmation of exit the App?");
            alertDialog.SetButton("Yes", (p1, p2) =>
            {
                App.Exit();

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



        private void UserList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void UserList_Refreshing(object sender, EventArgs e)
        {
            UserList.IsRefreshing = false;
        }
    }
}
