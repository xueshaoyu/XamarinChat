using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Content.Core; 
using Xamarin.Forms;

[assembly: Dependency(typeof(Toast_Android))]
namespace Content.Core
{
    public interface IToast
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
    public class Toast_Android : IToast
    {
        private static Toast_Android instance;
        public static Toast_Android Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Toast_Android();
                }

                return instance;
            }
        }
        public void LongAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
        public void ShortAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}