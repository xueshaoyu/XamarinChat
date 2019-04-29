using Chat.Uwp;
using Content.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

[assembly: Xamarin.Forms.Dependency(typeof(Toast_Uwp))]
namespace Chat.Uwp
{
    public class Toast_Uwp : IToast
    {
        static string templateStr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<toast launch=""app-defined-string"">
  <visual>
    <binding template=""ToastGeneric"">
      <text>{0}</text>
    </binding>
  </visual>
</toast>";
        static ToastNotifier toastNotifier;
        static Toast_Uwp()
        {
            toastNotifier = ToastNotificationManager.CreateToastNotifier();

        }
        public void LongAlert(string message)
        {
            var docStr = string.Format(templateStr, message);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(docStr);
            var notification = new ToastNotification(doc);
            toastNotifier.Show(notification);
        }

        public void ShortAlert(string message)
        {
            var docStr = string.Format(templateStr, message);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(docStr);
            var notification = new ToastNotification(doc);
            toastNotifier.Show(notification);
        }
    }
}
