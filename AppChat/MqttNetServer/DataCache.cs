using Chat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttNetServer
{
    /// <summary>
    /// Data Lib
    /// </summary>
    public class DataCache
    {
        /// <summary>
        /// User List Storage
        /// </summary>
        public static List<UserInfo> Users { get; set; } = new List<UserInfo>();

        /// <summary>
        /// Message List Storage
        /// </summary>
        public static List<MsgInfo> Msgs { get; set; } = new List<MsgInfo>();
    }
}
