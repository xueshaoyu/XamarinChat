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

[assembly: Dependency(typeof(ImpDroidCloseAppService))]
namespace Content.Core
{
    public class ImpDroidCloseAppService : ICloseAppService
    {
        public void CloseApp()
        {
            Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}