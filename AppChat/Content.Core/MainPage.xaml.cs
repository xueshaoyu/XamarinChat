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
using Newtonsoft.Json;

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

                handler.ReceiveOffline += Handler_ReceiveOffline;
                handler.ReceiveMsg += Handler_ReceiveMsg; ;
            } 
            Init();
        }

        private void Handler_ReceiveMsg(string obj)
        {
            try
            {
                var msgInfo = JsonConvert.DeserializeObject<MsgInfo>(obj);
                if (msgInfo != null)
                {
                    var user = Users.FirstOrDefault(p => p.Id == msgInfo.SendId);
                    if (user!=null)
                    {
                        user.HasNewMessage = true;
                    }
                }
            }
            catch { }
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
                App.Exit();
            
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
            {
                item.HasNewMessage = false;
                Application.Current.MainPage = new NavigationPage(new ChatPage(item));
            }
        }
    }
}
