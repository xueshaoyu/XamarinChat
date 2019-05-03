using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Core
{
    /// <summary>
    /// 配置信息设置接口
    /// </summary>
    public interface IUserPreferences
    {
        void SetObj<T>(string key, T obj);
        T GetObj<T>(string key);
        void SetString(string key, string value);
        string GetString(string key);
        void DeleteString(string key);


        void SetInt(string key, int value);
        int GetInt(string key);
    }
    /// <summary>
    /// 本地信息分类
    /// </summary>
    public enum EnumUserPreferences
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        UserInfo,
        /// <summary>
        /// 服务器地址
        /// </summary>
        ServerAddress,
        /// <summary>
        /// MQTT服务器IP
        /// </summary>
        MqttServerIp,
        /// <summary>
        /// MQTT服务器端口
        /// </summary>
        MqttServerPort,
    }
}
