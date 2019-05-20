using Content.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Chat.Uwp
{
    public class UserPreferencesUwp : IUserPreferences
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public void DeleteString(string key)
        {
            localSettings.Values.Remove(key);

        }
        public UserPreferencesUwp()
        {

            SetDefaultValue();
        }
        public void SetDefaultValue()
        {
            var exist = GetString(EnumUserPreferences.ServerAddress.ToString());
            if (string.IsNullOrWhiteSpace(exist))
            {
                SetString(EnumUserPreferences.ServerAddress.ToString(),
                     "http://192.168.0.150:7778/api/");
                SetString(EnumUserPreferences.MqttServerIp.ToString(), "192.168.0.150");
                SetInt(EnumUserPreferences.MqttServerPort.ToString(), 7777);
            }
        }
        public int GetInt(string key)
        {
            if (!localSettings.Values.ContainsKey(key))
            { return 0; }
            return (int)localSettings.Values[key];
        }

        public T GetObj<T>(string key)
        {
            if (!localSettings.Values.ContainsKey(key))
            { return default(T); }
            return (T)localSettings.Values[key];
        }

        public string GetString(string key)
        {
            if (!localSettings.Values.ContainsKey(key))
            { return string.Empty; }
            return localSettings.Values[key].ToString();
        }

        public void SetInt(string key, int value)
        {
            localSettings.Values[key] = value;
        }

        public void SetObj<T>(string key, T obj)
        {
            if (obj.GetType() == typeof(string))
            {
                SetString(key, obj.ToString());
            }
            else
            {
                localSettings.Values[key] = JsonConvert.SerializeObject(obj);
            }
        }

        public void SetString(string key, string value)
        {
            localSettings.Values[key] = value;
        }
    }
}
