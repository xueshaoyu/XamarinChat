using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MqttNetServer
{
    public class ServerHelper
    {
        HttpListener httpListener = new HttpListener();
        public void Setup(int port = 7778)
        {
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Prefixes.Add(string.Format("http://*:{0}/", port));
            httpListener.Start();

            Receive();
        }
        public void Stop()
        {
            httpListener.Stop();
        }

        private void Receive()
        {
            httpListener.BeginGetContext(new AsyncCallback(EndReceive), null);
        }

        void EndReceive(IAsyncResult ar)
        {
            try
            {
                var context = httpListener.EndGetContext(ar);
                Dispather(context);
                Receive();
            }
            catch
            {

            }
        }

        RequestHelper RequestHelper;
        ResponseHelper ResponseHelper;
        void Dispather(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            RequestHelper = new RequestHelper(request);
            ResponseHelper = new ResponseHelper(response);
            RequestHelper.DispatchResources(str => {
                ResponseHelper.WriteToClient(str);
            });
        }
    }
}
