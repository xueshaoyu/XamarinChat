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
            var registerUser = App.UserPreferences.GetObj<UserInfo>(EnumUserPreferences.UserInfo.ToString());
            var currentUser = new UserInfo();
            currentUser.Guid = registerUser.Guid;
            currentUser.Name = UserName.Text;
            currentUser.Password = Password.Text;
            if (registerUser.Name == currentUser.Name && registerUser.Password == currentUser.Password)
            {
                var result = await  MQTTHelper.Instance.Online(currentUser);
               
                if (result)
                {

                    Toast_Android.Instance.ShortAlert("Login Successed!");
                    App.CurrentUser = currentUser;
                    Application.Current.MainPage = new NavigationPage(MainPage.Instance);
                }
            }
            else
            {
                Toast_Android.Instance.ShortAlert("Login Failed!");
            }
        }

        private void Close_Button_Clicked(object sender, EventArgs e)
        {
            App.Exit();
        }

        private void Register_Button_Clicked(object sender, EventArgs e)
        {

            Application.Current.MainPage = Register.Instance;
        }
    }
}