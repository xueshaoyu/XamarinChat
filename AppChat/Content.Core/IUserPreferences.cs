using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Core
{
    public interface IUserPreferences
    {
        void SetDefaultValue();

        void SetObj<T>(string key, T obj);
        T GetObj<T>(string key);
        void SetString(string key, string value);
        string GetString(string key);
        void DeleteString(string key);


        void SetInt(string key, int value);
        int GetInt(string key);
    }

    public enum EnumUserPreferences
    {
        UserInfo,
        ServerAddress,
        MqttServerIp,
        MqttServerPort,
    }
}
