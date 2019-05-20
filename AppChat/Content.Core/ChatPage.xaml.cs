using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chat.Model;
using MQTTnet;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Content.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        HttpClientHelper client = new HttpClientHelper();
        public ObservableCollection<MsgInfo> Messages
        {
            get; set;
        } = new ObservableCollection<MsgInfo>();

        private UserInfo RemoteUser;

        public ChatPage(UserInfo remoteUser)
        {
            InitializeComponent();
            RemoteUser = remoteUser;
            this.Title = $"Chat with [{remoteUser.Name}]";


            MsgList.ItemsSource = Messages;
            MsgList.Refreshing += MsgList_Refreshing;
            var handler = MQTTHelper.Instance.MqttClient.ApplicationMessageReceivedHandler as MqttApplicationMessageReceivedHandler;
            if (handler != null)
            {
                handler.ReceiveMsg += Handler_ReceiveMsg;
            }
            LoadMsg();
        }

        public async void LoadMsg()
        {
            var list = await client.GetMessage(RemoteUser.Id);
            if (list != null && list.Count > 0)
            {
                Messages = new ObservableCollection<MsgInfo>(list);
            }
        }

        private void MsgList_Refreshing(object sender, EventArgs e)
        {

        }

        private void ScollerToButtom()
        {
            MsgList.BeginRefresh();
            if (Messages.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {

                    MsgList.ScrollTo(Messages.Last(), ScrollToPosition.End, false);
                });
            }
            MsgList.EndRefresh();
        }

        private void Handler_ReceiveMsg(string obj)
        {
            try
            {
                var msgInfo = JsonConvert.DeserializeObject<MsgInfo>(obj);
                if (msgInfo != null)
                {
                    if (msgInfo.SendId == RemoteUser.Id)
                    {
                        Messages.Add(msgInfo); ScollerToButtom();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// jump to MainPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_MainPage(object sender, EventArgs e)
        {
            var handler = MQTTHelper.Instance.MqttClient.ApplicationMessageReceivedHandler as MqttApplicationMessageReceivedHandler;
            if (handler != null)
            {
                handler.ReceiveMsg -= Handler_ReceiveMsg;
            }
            Application.Current.MainPage = new NavigationPage(MainPage.Instance);
        }
        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendMsg_Button_Clicked(object sender, EventArgs e)
        {
            var msg = SendMsg.Text;
            if (!string.IsNullOrEmpty(msg))
            {
                MsgInfo msgInfo = new MsgInfo();
                msgInfo.Content = msg;
                msgInfo.ReceiveId = RemoteUser.Id;
                msgInfo.SendId = App.CurrentUser.Id;
                var result = await MQTTHelper.Instance.SendMsg(msgInfo);
                client.Message(msgInfo);
                if (result)
                {
                    SendMsg.Text = "";
                    Messages.Add(msgInfo); ScollerToButtom();
                    //Toast_Android.Instance.ShortAlert("Send Successed!");
                }
                else
                {

                    ToastHelper.Instance.ShortAlert("Send Failed!");
                }
            }
        }
    }
}