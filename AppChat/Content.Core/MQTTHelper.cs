using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chat.Model;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Protocol;
using Newtonsoft.Json;

namespace Content.Core
{
    public class MQTTHelper
    {
        private static MQTTHelper instance;
        public static MQTTHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MQTTHelper();
                  var task=  instance.Init();
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


        private async Task<bool> Init()
        {

            try
            {

                // var options = new MqttClientOptions() { ClientId = userInfo.Guid + "-" + userInfo.Name };
                var options = new MqttClientOptions() { ClientId = Guid.NewGuid().ToString("D") };
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
                options.CleanSession = true;
                options.KeepAlivePeriod = TimeSpan.FromSeconds(100.5);
                options.KeepAliveSendInterval = TimeSpan.FromSeconds(20000);


                if (_mqttClient == null)
                { _mqttClient = new MqttFactory().CreateMqttClient(); }






                var handler = new MqttApplicationMessageReceivedHandler();
                _mqttClient.ApplicationMessageReceivedHandler = handler;

                var r =await  _mqttClient.ConnectAsync(options); 
                if (r.ResultCode == MQTTnet.Client.Connecting.MqttClientConnectResultCode.Success)
                {
                    var msgTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Msg.ToString() + "-" + App.CurrentUser.Id)
                       .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce).Build();

                    var onlineTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Online.ToString())
                          .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce).Build();
                    var offlineTopicFilter = new TopicFilterBuilder().WithTopic(MQTTTopic.Offline.ToString())
                          .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce).Build();

                    var currentOptions = new MqttClientSubscribeOptions();
                    currentOptions.TopicFilters.Add(msgTopicFilter);
                    currentOptions.TopicFilters.Add(onlineTopicFilter);
                    currentOptions.TopicFilters.Add(offlineTopicFilter);
                    var cancel = new CancellationToken();
                    await _mqttClient.SubscribeAsync(currentOptions);
                    return true;
                }
                else
                {
                    Toast_Android.Instance.ShortAlert("Server Error!");
                    return false;
                }
                //r.Wait(cancel);
            }
            catch (Exception ex)
            {

                Toast_Android.Instance.ShortAlert("Exception!");
                return false;
            }
        }

        public async Task<bool> SendMsg(MsgInfo msgInfo)
        {
            var msg = new MqttApplicationMessage()
            {
                Topic = MQTTTopic.Msg.ToString() + "-" + msgInfo.ReceiveId,
                Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msgInfo)),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
                Retain = false
            };
            var cancel = new CancellationToken();
            var result = await _mqttClient.PublishAsync(msg, cancel);
            return result.ReasonCode == MqttClientPublishReasonCode.Success;
        }

        public async Task<bool> Register(UserInfo userinfo)
        {
            var msg = new MqttApplicationMessage()
            {
                Topic = MQTTTopic.Register.ToString(),
                Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userinfo)),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
                Retain = false
            };
            var cancel = new CancellationToken();
            var result = await _mqttClient.PublishAsync(msg, cancel);
            return result.ReasonCode == MqttClientPublishReasonCode.Success;
        }
        public async Task<bool> Online(UserInfo userinfo)
        {
            var msg = new MqttApplicationMessage()
            {
                Topic = MQTTTopic.Online.ToString(),
                Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userinfo)),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
                Retain = false
            };
            var cancel = new CancellationToken();
            var result = await _mqttClient.PublishAsync(msg, cancel);
            return result.ReasonCode == 0;
        }
        public async Task<bool> Offline(UserInfo userinfo)
        {
            var msg = new MqttApplicationMessage()
            {
                Topic = MQTTTopic.Offline.ToString(),
                Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userinfo)),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce,
                Retain = false
            };
            var cancel = new CancellationToken();
            var result = await _mqttClient.PublishAsync(msg, cancel);
            return result.ReasonCode == MqttClientPublishReasonCode.Success;
        }
    }
    /// <summary>
    /// Topic Category
    /// </summary>
    public enum MQTTTopic
    {
        Register,
        Online,
        Offline,
        /// <summary>
        /// Only subscribe my msg topic，topic name is 【Msg-'userid'】
        /// </summary>
        Msg
    }
}
