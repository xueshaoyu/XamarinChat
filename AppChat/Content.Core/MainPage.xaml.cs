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
using Application = Xamarin.Forms.Application;
using Chat.Model;

namespace Content.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        HttpClientHelper client = new HttpClientHelper();
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
            var handler = MQTTHelper.Instance.MqttClient.ApplicationMessageReceivedHandler as MqttApplicationMessageReceivedHandler;
            if (handler != null)
            {
                handler.ReceiveOnLine += Handler_ReceiveOnLine;

                handler.ReceiveOffline += Handler_ReceiveOffline; ;
            }
            Init();
        }

        private void Handler_ReceiveOffline(int obj)
        {
            var user = Users.FirstOrDefault(p => p.Id == obj);
            user.Online = 0;
        }

        private void Handler_ReceiveOnLine(int obj)
        {
         var user=  Users.FirstOrDefault(p => p.Id == obj);
            user.Online = 1;
        }

        public ObservableCollection<UserInfo> Users
        {
            get; set;
        }
        private async void Init()
        {
            var users = await client.GetFrinds(App.CurrentUser);
            Users= new ObservableCollection<UserInfo>(users);
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

        private void UserList_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            var item = e.Item as UserInfo;
            if (item != null)
                Application.Current.MainPage = new NavigationPage(new ChatPage(item));
        }
    }
}
