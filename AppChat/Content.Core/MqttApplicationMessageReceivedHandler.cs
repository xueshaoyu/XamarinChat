using Chat.Model;
using MQTTnet;
using MQTTnet.Client.Receiving;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Content.Core
{
    public class MqttApplicationMessageReceivedHandler : IMqttApplicationMessageReceivedHandler
    {
        public event Action<string> ReceiveMsg;
        public event Action<int> ReceiveOnLine;
        public event Action<int> ReceiveOffline;
        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var topic = eventArgs.ApplicationMessage.Topic;
            if (topic == MQTTTopic.Msg.ToString() + "-" + App.CurrentUser.Id)
            {
                var content = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
                ReceiveMsg?.Invoke(content);
            }
            else if (topic == MQTTTopic.Online.ToString())
            {
                var content = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
                var userinfo = JsonConvert.DeserializeObject<UserInfo>(content);
                if (userinfo != null && userinfo.Id > 0)
                {
                    ReceiveOnLine?.Invoke(userinfo.Id);
                }
            }
            else if (topic == MQTTTopic.Offline.ToString())
            {
                var content = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
                var userinfo = JsonConvert.DeserializeObject<UserInfo>(content);
                if (userinfo != null && userinfo.Id > 0)
                {
                    ReceiveOffline?.Invoke(userinfo.Id);
                }
            }
        }
    }
}
