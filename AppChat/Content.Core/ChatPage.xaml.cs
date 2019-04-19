using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Protocol;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Content.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ObservableCollection<MsgInfo> Messages
        {
            get; set;
        } = new ObservableCollection<MsgInfo>();
        private UserInfo RemoteUser;

        CancellationToken cancellationToken = new CancellationToken();
        private TopicFilter CurrentTopicFilter;
        public ChatPage(UserInfo remoteUser)
        {
            InitializeComponent();
            RemoteUser = remoteUser;
            this.Title = $"Chat with [{remoteUser.Name}]";
            MsgList.ItemsSource = Messages;
            CurrentTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Msg.ToString() + "-" + App.CurrentUser.Guid)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build();
          var  currentOptions = new MqttClientSubscribeOptions();
            currentOptions.TopicFilters.Add(CurrentTopicFilter);
            MQTTHelper.MqttClient.SubscribeAsync(currentOptions, cancellationToken);
           
         
        }

        /// <summary>
        /// jump to MainPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_MainPage(object sender, EventArgs e)
        {
            var aa = new MqttClientUnsubscribeOptions();
            aa.TopicFilters.Add(CurrentTopicFilter.Topic);
            MQTTHelper.MqttClient.UnsubscribeAsync(aa, cancellationToken);
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
                msgInfo.ReceiveId = RemoteUser.Guid;
                msgInfo.SendId = App.CurrentUser.Guid;
                var result = await MQTTHelper.Instance.SendMsg(msgInfo);
                if (result)
                {
                    SendMsg.Text = "";
                    Messages.Add(msgInfo);
                    //Toast_Android.Instance.ShortAlert("Send Successed!");
                }
                else
                {

                    Toast_Android.Instance.ShortAlert("Send Failed!");
                }
            }
        }
    }
}