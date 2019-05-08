using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Chat.Model;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Content.Core
{
    /// <summary>
    /// MQTT帮助类
    /// </summary>
    public class MQTTHelper
    {
        private static string clientId;
        private static MQTTHelper instance;

        static MQTTHelper()
        {
            var id = App.UserPreferences.GetString(EnumUserPreferences.ClientId.ToString());
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString("D");
                App.UserPreferences.SetString(EnumUserPreferences.ClientId.ToString(),id);
            }
            clientId = id;
        }
        public static MQTTHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MQTTHelper();
                    var task = instance.Connect();
                    Thread.Sleep(500);
                }

                return instance;
            }
        }
        public static string ServerIp { get; set; } = App.UserPreferences.GetString(EnumUserPreferences.MqttServerIp.ToString());

        public static int Port { get; set; } = App.UserPreferences.GetInt(EnumUserPreferences.MqttServerPort.ToString());
        private IMqttClient _mqttClient;

        public IMqttClient MqttClient
        {
            get { return _mqttClient; }
        }

        public async Task Disconnect()
        {
            await _mqttClient.DisconnectAsync();
        }
        public async Task<bool> Connect()
        {

            try
            {

                // var options = new MqttClientOptions() { ClientId = userInfo.Guid + "-" + userInfo.Name };
                var options = new MqttClientOptions() { ClientId = clientId };
                options.ChannelOptions = new MqttClientTcpOptions()
                {
                    Server = ServerIp,
                    Port = Port
                };
                options.Credentials = new MqttClientCredentials()
                {
                    Username = "admin",
                    Password = "public"
                };
                options.CleanSession = false;
                options.KeepAlivePeriod = TimeSpan.FromSeconds(100.5);
                options.KeepAliveSendInterval = TimeSpan.FromSeconds(20000);
                if (_mqttClient == null)
                { _mqttClient = new MqttFactory().CreateMqttClient(); }

                var handler = new MqttApplicationMessageReceivedHandler();

                handler.SystemNotice += Handler_SystemNotice;
                _mqttClient.ApplicationMessageReceivedHandler = handler;

                var r = await _mqttClient.ConnectAsync(options);
                if (r.ResultCode == MQTTnet.Client.Connecting.MqttClientConnectResultCode.Success)
                {
                    //var msgTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Msg.ToString() + "-" + App.CurrentUser.Id)
                    //   .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce).Build();

                    var onlineTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Online.ToString())
                          .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build();
                    var offlineTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Offline.ToString())
                          .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build();
                    var systemNoticeTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.SystemNotice.ToString())
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build();

                    var currentOptions = new MqttClientSubscribeOptions();
                    //currentOptions.TopicFilters.Add(msgTopicFilter);
                    //currentOptions.TopicFilters.Remove(msgTopicFilter);
                    currentOptions.TopicFilters.Add(onlineTopicFilter);
                    currentOptions.TopicFilters.Add(offlineTopicFilter);
                    currentOptions.TopicFilters.Add(systemNoticeTopicFilter);
                    var cancel = new CancellationToken();
                    await _mqttClient.SubscribeAsync(currentOptions);
                    return true;
                }
                else
                {
                    Toast_Android.Instance.ShortAlert("服务器错误!");
                    return false;
                }
                //r.Wait(cancel);
            }
            catch (Exception ex)
            {

                Toast_Android.Instance.LongAlert("异常!");
                return false;
            }
        }
        private async void Handler_SystemNotice(string obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(App.MainActivity);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("系统提示");
                alertDialog.SetMessage(obj);
                alertDialog.SetButton("知道了", (p1, p2) =>
                {
                });
                alertDialog.Show();
                // DisplayAlert("系统提示", obj, "知道了");
            });
        }


        /// <summary>
        /// 发送消息到MQTT
        /// </summary>
        /// <param name="msgInfo"></param>
        /// <returns></returns>
        public async Task<bool> SendMsg(MsgInfo msgInfo)
        {
            try
            {
                var msg = new MqttApplicationMessage()
                {
                    Topic = MQTTTopic.Msg.ToString() + "-" + msgInfo.ReceiveId,
                    Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msgInfo)),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                    Retain = false
                };
                var cancel = new CancellationToken();
                var result = await _mqttClient.PublishAsync(msg, cancel);
                return result.ReasonCode == MqttClientPublishReasonCode.Success;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public async Task<bool> Register(UserInfo userinfo)
        {
            try
            {
                var msg = new MqttApplicationMessage()
                {
                    Topic = MQTTTopic.Register.ToString(),
                    Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userinfo)),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                    Retain = false
                };
                var cancel = new CancellationToken();
                var result = await _mqttClient.PublishAsync(msg, cancel);
                return result.ReasonCode == MqttClientPublishReasonCode.Success;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public async Task<bool> Online(UserInfo userinfo)
        {
            try
            {
                var msg = new MqttApplicationMessage()
                {
                    Topic = MQTTTopic.Online.ToString(),
                    Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userinfo)),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                    Retain = false
                };
                var cancel = new CancellationToken();
                var result = await _mqttClient.PublishAsync(msg, cancel);
                return result.ReasonCode == 0;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 离线
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public async Task<bool> Offline(UserInfo userinfo)
        {
            try
            {
                var msg = new MqttApplicationMessage()
                {
                    Topic = MQTTTopic.Offline.ToString(),
                    Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userinfo)),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                    Retain = false
                };
                var cancel = new CancellationToken();
                var result = await _mqttClient.PublishAsync(msg, cancel);
                return result.ReasonCode == MqttClientPublishReasonCode.Success;
            }
            catch
            {
                return false;
            }
        }
    }
    /// <summary>
    /// MQTT消息主题枚举
    /// </summary>
    public enum MQTTTopic
    {
        /// <summary>
        /// 注册
        /// </summary>
        Register,
        /// <summary>
        /// 在线
        /// </summary>
        Online,
        /// <summary>
        /// 离线
        /// </summary>
        Offline,
        /// <summary>
        /// 只订阅我的消息主题，主题名称为 【Msg-'userid'】
        /// </summary>
        Msg,
        /// <summary>
        /// 系统消息
        /// </summary>
        SystemNotice
    }
}
