﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;
using ServiceStack;
using ServiceStack.Text.Common;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MqttNetServer
{
    public partial class FrmMqttServer : Form
    {

        private IMqttServer _mqttServer = null;

        private Action<string> _updateListBoxAction;
        public FrmMqttServer()
        {
            InitializeComponent();
        }

        private void FrmMqttServer_Load(object sender, EventArgs e)
        {
            var ips = Dns.GetHostAddressesAsync(Dns.GetHostName());

            foreach (var ip in ips.Result)
            {
                switch (ip.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        TxbServer.Text = ip.ToString();
                        break;
                    case AddressFamily.InterNetworkV6:
                        break;
                }
            }
            _updateListBoxAction = new Action<string>((s) =>
            {
                listBox1.Items.Add(s);
                if (listBox1.Items.Count > 1000)
                {
                    listBox1.Items.RemoveAt(0);
                }
                var visibleItems = listBox1.ClientRectangle.Height / listBox1.ItemHeight;

                listBox1.TopIndex = listBox1.Items.Count - visibleItems + 1;
            });


            listBox1.KeyPress += (o, args) =>
            {
                if (args.KeyChar == 'c' || args.KeyChar == 'C')
                {
                    listBox1.Items.Clear();
                }
            };

            BtnStart.Enabled = true;
            BtnStop.Enabled = false;
            TxbServer.Enabled = true;
            TxbPort.Enabled = true;
        }

        ServerHelper Server;//= new ServerHelper();
        private void BtnStart_Click(object sender, EventArgs e)
        {
            MqttServer();
            HttpServer();
            BtnStart.Enabled = false;
            BtnStop.Enabled = true;
            TxbServer.Enabled = false;
            TxbPort.Enabled = false;
        }
        Task httpTask;
        private void HttpServer()
        {
            Server = new ServerHelper();
            httpTask = Task.Run(() =>
            {
                Server.Setup(7778);
            });
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (null != _mqttServer)
            {
                foreach (var clientSessionStatuse in _mqttServer.GetClientSessionsStatusAsync().Result)
                {
                    clientSessionStatuse.DisconnectAsync();
                }
                _mqttServer.StopAsync();
                _mqttServer = null;

            }
            Server.Stop();
            //var task = Task.Run(() =>
            //{
            //    Server.Stop();
            //});
            httpTask.Wait(1);

            BtnStart.Enabled = true;
            BtnStop.Enabled = false;
            TxbServer.Enabled = true;
            TxbPort.Enabled = true;
        }

        private async void MqttServer()
        {
            if (null != _mqttServer)
            {
                return;
            }

            var optionBuilder =
                new MqttServerOptionsBuilder().WithConnectionBacklog(1000).WithPersistentSessions().WithDefaultEndpointPort(Convert.ToInt32(TxbPort.Text));

            if (!TxbServer.Text.IsNullOrEmpty())
            {
                optionBuilder.WithDefaultEndpointBoundIPAddress(IPAddress.Parse(TxbServer.Text));
            }

            //MqttServerOptions options = new MqttServerOptions();
            //options.EnablePersistentSessions = true;
            var options = optionBuilder.Build();
            // var   options.PairWith(option);
            //var sessiongs=   options.EnablePersistentSessions;
            (options as MqttServerOptions).ConnectionValidator += context =>
            {
                if (context.ClientId.Length < 10)
                {
                    context.ReturnCode = MqttConnectReturnCode.ConnectionRefusedIdentifierRejected;
                    return;
                }
                if (!context.Username.Equals("admin"))
                {
                    context.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                    return;
                }
                if (!context.Password.Equals("public"))
                {
                    context.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                    return;
                }
                context.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;

            };

            _mqttServer = new MqttFactory().CreateMqttServer();
            _mqttServer.ClientConnected += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction, $">Client Connected:ClientId:{args.ClientId},ProtocalVersion:");

                var s = _mqttServer.GetClientSessionsStatus();
                label3.BeginInvoke(new Action(() => { label3.Text = $"Total Count：{s.Count}"; }));
            };

            _mqttServer.ClientDisconnected += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction, $"<Client DisConnected:ClientId:{args.ClientId}");
                var s = _mqttServer.GetClientSessionsStatus();
                label3.BeginInvoke(new Action(() => { label3.Text = $"Total Count：{s.Count}"; }));
            };

            _mqttServer.ApplicationMessageReceived += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction,
                    $"ClientId:{args.ClientId} Topic:{args.ApplicationMessage.Topic} Payload:{Encoding.UTF8.GetString(args.ApplicationMessage.Payload)} QualityOfServiceLevel:{args.ApplicationMessage.QualityOfServiceLevel}");

            };

            _mqttServer.ClientSubscribedTopic += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction, $"@ClientSubscribedTopic ClientId:{args.ClientId} Topic:{args.TopicFilter.Topic} QualityOfServiceLevel:{args.TopicFilter.QualityOfServiceLevel}");
            };
            _mqttServer.ClientUnsubscribedTopic += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction, $"%ClientUnsubscribedTopic ClientId:{args.ClientId} Topic:{args.TopicFilter.Length}");
            };

            _mqttServer.Started += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction, "Mqtt Server Start...");
            };

            _mqttServer.Stopped += (sender, args) =>
            {
                listBox1.BeginInvoke(_updateListBoxAction, "Mqtt Server Stop...");

            };

            await _mqttServer.StartAsync(options);
        }

        private async void btnPublish_Click(object sender, EventArgs e)
        {
            //发布系统消息
            if (cbbQos.Text.IsNullOrEmpty())
            {
                MessageBox.Show("请先选择Qos等级");
                return;
            }

            var qos = (MqttQualityOfServiceLevel)Enum.Parse(typeof(MqttQualityOfServiceLevel), cbbQos.Text);
            await _mqttServer.PublishAsync(txtTopicName.Text, txtNoticeContent.Text, qos);
        }
    }
}
