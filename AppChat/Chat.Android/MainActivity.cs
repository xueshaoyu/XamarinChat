using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Content.Core;
using Java.Lang;
using Xamarin.Forms.Platform.Android;

namespace Chat.Droid
{
    [Activity(Label = "Chat", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
            

            var app = new App();
            LoadApplication(app);
            // MQTTHelper.Init();
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                Exit();
                return false;
            }

            return base.OnKeyDown(keyCode, e);
        }

        bool isExit = false;


        private Handler handler = new Handler(p =>
        {
            Instance.isExit = false;
        });

        public void Exit()
        {
            if (!isExit)
            {
                isExit = true;
                ToastHelper.Instance.LongAlert("Click back button again to exit process!");
                handler.SendEmptyMessageDelayed(0, 2000);
            }
            else
            {
                Finish();
                System.Environment.Exit(0);
            }
        }
    }
}