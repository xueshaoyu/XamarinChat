using Chat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Content.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private static Login instance;
        public static Login Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Login();
                }

                return instance;
            }
        }
        private Login()
        {
            InitializeComponent();
        }


        private async void Login_Button_Clicked(object sender, EventArgs e)
        {
            //  var registerUser = App.UserPreferences.GetObj<UserInfo>(EnumUserPreferences.UserInfo.ToString());
            var currentUser = new UserInfo();
            currentUser.Name = UserName.Text;
            currentUser.Password = Password.Text;
            HttpClientHelper client = new HttpClientHelper();
            var id = await client.Login(currentUser);
            if (id > 0)
            {
                currentUser.Id = id;
                App.UserPreferences.SetObj<UserInfo>(EnumUserPreferences.UserInfo.ToString(), currentUser);
                await client.Online(currentUser);
                ToastHelper.Instance.ShortAlert("Login Successed!");
                App.CurrentUser = currentUser;
              
            
                Application.Current.MainPage = new NavigationPage(MainPage.Instance);
                await MQTTHelper.Instance.Online(currentUser);
            }
            else
            {
                ToastHelper.Instance.ShortAlert("Login Failed!");
            }
        }

        private void Close_Button_Clicked(object sender, EventArgs e)
        {
            App.Exit();
        }

        private void Register_Button_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(Register.Instance);
        }

        private void Settings_Clicked(object sender, EventArgs e)
        {
            SettingArea.IsVisible = true;
            LoginArea.IsVisible = false;


            ServerApi.Text= App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString());
            ServerIp.Text= App.UserPreferences.GetString(EnumUserPreferences.MqttServerIp.ToString());
            ServerPort.Text= App.UserPreferences.GetInt(EnumUserPreferences.MqttServerPort.ToString()).ToString();
        }

        private void Settings_Confirm_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.UserPreferences.SetString(EnumUserPreferences.ServerAddress.ToString(), ServerApi.Text);
                App.UserPreferences.SetString(EnumUserPreferences.MqttServerIp.ToString(), ServerIp.Text);
                App.UserPreferences.SetInt(EnumUserPreferences.MqttServerPort.ToString(), Convert.ToInt32(ServerPort.Text));

                ToastHelper.Instance.ShortAlert("Set Successed!");
                SettingArea.IsVisible = false;
                LoginArea.IsVisible = true;
            }
            catch
            {
                ToastHelper.Instance.ShortAlert("Set Failed!");
            }
        }

        private void Settings_Cancel_Clicked(object sender, EventArgs e)
        {
            SettingArea.IsVisible = false;
            LoginArea.IsVisible = true;
        }

        private void Exit_Clicked(object sender, EventArgs e)
        {
            App.Exit();
        }
    }
}