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
    public partial class Register : ContentPage
    {
        private static Register instance;
        public static Register Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Register();
                }

                return instance;
            }
        }
        private Register()
        {
            InitializeComponent();
        }


        private async void Register_Button_Clicked(object sender, EventArgs e)
        {
            var currentUser = new UserInfo();
            currentUser.Name = UserName.Text;
            currentUser.Password = Password.Text;

            HttpClientHelper client = new HttpClientHelper();
            var id = await client.Register(currentUser);
            if (id > 0)
            {
                Toast_Android.Instance.ShortAlert("注册成功!");
                Application.Current.MainPage = new NavigationPage(Login.Instance);
            }
            else
            {
                Toast_Android.Instance.ShortAlert("注册失败!");
            }
        }

        private void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(Login.Instance);
        }
    }
}