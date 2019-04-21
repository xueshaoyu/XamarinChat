using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Content.Core;

namespace Chat.Droid
{
    [Activity(Label = "Chat", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Instance = this;
            App.MainActivity = this;
            App.Init(new UserPreferencesAndroid());
            var exist = App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString());
            if (exist == null || exist == "")
            {
                App.UserPreferences.SetString(EnumUserPreferences.ServerAddress.ToString(), "http://192.168.0.150:7778/api/");
                App.UserPreferences.SetString(EnumUserPreferences.MqttServerIp.ToString(), "192.168.0.150");
                App.UserPreferences.SetInt(EnumUserPreferences.MqttServerPort.ToString(), 7777);
            }
            var app = new App();
            LoadApplication(app);
            // MQTTHelper.Init();
        }

    }
}