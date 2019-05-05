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
            this.Title = $"正在与[{remoteUser.Name}]聊天…";
            MsgList.ItemsSource = Messages;
            MsgList.Refreshing += MsgList_Refreshing;
            var handler = MQTTHelper.Instance.MqttClient.ApplicationMessageReceivedHandler as MqttApplicationMessageReceivedHandler;
            if (handler != null)
            {
                handler.ReceiveMsg += Handler_ReceiveMsg;
            }

        }
        /// <summary>
        /// 订阅
        /// </summary>

        private async void SubscribeAsync()
        {
            var msgTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Msg.ToString() + "-" + App.CurrentUser.Id)
                       .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build();
            var mqttClientSubscribeOptions = new MqttClientSubscribeOptions();
            mqttClientSubscribeOptions.TopicFilters.Add(msgTopicFilter);
            var cancel = new CancellationToken();
            await MQTTHelper.Instance.MqttClient.SubscribeAsync(mqttClientSubscribeOptions, cancel);
        }
        /// <summary>
        /// 退订
        /// </summary>
        private async void UnsubscribeAsync()
        {
            var mqttClientUnsubscribeOptions = new MqttClientUnsubscribeOptions();
            mqttClientUnsubscribeOptions.TopicFilters.Add(MQTTTopic.Msg.ToString() + "-" + App.CurrentUser.Id);
            var cancel = new CancellationToken();
            await MQTTHelper.Instance.MqttClient.UnsubscribeAsync(mqttClientUnsubscribeOptions, cancel);
        }

        private void MsgList_Refreshing(object sender, EventArgs e)
        {
           
        }

        private void ScollerToButtom()
        {
            if (Messages.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MsgList.ScrollTo(Messages.Last(), ScrollToPosition.End, false);
                });
            }
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
               await client.Message(msgInfo);
                if (result)
                {
                    SendMsg.Text = "";
                    Messages.Add(msgInfo); ScollerToButtom();
                    //Toast_Android.Instance.ShortAlert("Send Successed!");
                }
                else
                {

                    Toast_Android.Instance.ShortAlert("发送失败!");
                }
            }
        }

        private  void SubMsg_MainPage(object sender, EventArgs e)
        {
             SubscribeAsync();
        }

        private  void UnsubMsg_MainPage(object sender, EventArgs e)
        {
            UnsubscribeAsync();
        }
    }
}