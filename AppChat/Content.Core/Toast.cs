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
  
namespace Content.Core
{
    public interface IToast
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
    public static class ToastHelper
    {
        private static IToast instance;
        public static IToast Instance
        {
            get
            {
                if (instance == null)
                {
                    
                       instance = DependencyService.Get<IToast>();
                }

                return instance;
            }
        }
        public static void ShowToastMessage(this Xamarin.Forms.View view, string strMessage, bool bLong = false) {
            if (bLong)
            {
                instance.LongAlert(strMessage);
            }
            else
            {
                instance.ShortAlert(strMessage);
            }
        }
        public static void ShowToastMessage(this Xamarin.Forms.ContentPage page, string strMessage, bool bLong = false) {
            if (bLong)
            {
                instance.LongAlert(strMessage);
            }
            else
            {
                instance.ShortAlert(strMessage);
            }
        }
    }
}