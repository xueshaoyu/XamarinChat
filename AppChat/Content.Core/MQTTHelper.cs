using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
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
                    instance.Init();
                }

                return instance;
            }
        }
        public static string ServerIp { get; set; } = "192.168.1.3";

        public static int Port { get; set; } = 7777;
        private static IMqttClient _mqttClient;

        public static IMqttClient MqttClient
        {
            get { return _mqttClient; }
        }


        private void Init()
        {

            try
            {

               // var options = new MqttClientOptions() { ClientId = userInfo.Guid + "-" + userInfo.Name };
                var options = new MqttClientOptions() { ClientId =Guid.NewGuid().ToString("D") };
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
                //_mqttClient.SubscribeAsync(
                //   new TopicFilterBuilder().WithTopic("")
                //       .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build());

                var cancel = new CancellationToken();
                var r = _mqttClient.ConnectAsync(options, cancel);
                r.Wait(cancel);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<bool> SendMsg(MsgInfo msgInfo)
        {
            var msg = new MqttApplicationMessage()
            {
                Topic = MQTTTopic.Msg.ToString() +"-"+ msgInfo.ReceiveId,
                Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msgInfo)),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
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
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
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
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
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
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
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
