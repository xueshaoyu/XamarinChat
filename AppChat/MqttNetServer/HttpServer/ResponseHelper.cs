using Chat.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MqttNetServer
{
    public class ResponseHelper
    {
        private HttpListenerResponse response;
        public ResponseHelper(HttpListenerResponse response)
        {
            this.response = response;

        }
     
        public async void WriteToClient(ExchangeData data)
        {
            var returnByteArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            response.OutputStream.Write(returnByteArr, 0, returnByteArr.Length);
            response.StatusCode = 200;
            await response.OutputStream.FlushAsync();
            response.Close();
        }
    
    }
}
