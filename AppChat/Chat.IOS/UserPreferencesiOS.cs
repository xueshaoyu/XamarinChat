using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Content.Core;
using Foundation;
using Newtonsoft.Json;
using UIKit;

namespace Chat.IOS.iOS
{
    public class UserPreferencesIOS : IUserPreferences
    {
        public UserPreferencesIOS()
        {
            SetDefaultValue();
        }
        public void SetDefaultValue()
        {
            var exist = GetString(EnumUserPreferences.ServerAddress.ToString());
            if (exist == null || exist == "")
            {
                SetString(EnumUserPreferences.ServerAddress.ToString(),
                     "http://192.168.0.150:7778/api/");
                SetString(EnumUserPreferences.MqttServerIp.ToString(), "192.168.0.150");
                SetInt(EnumUserPreferences.MqttServerPort.ToString(), 7777);
            }
        }

        public void SetString(string key, string value)
        {
            NSUserDefaults.StandardUserDefaults.SetString(value, key);
        }

        public string GetString(string key)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(key);

        }

        public void DeleteString(string key)
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(key);
        }

        public void SetObj<T>(string key, T obj)
        {
            if (obj is string)
            {
                SetString(key, obj.ToString());
            }
            else
            {
                var str = JsonConvert.SerializeObject(obj);
                SetString(key, str);
            }
        }

        public T GetObj<T>(string key)
        {
            try
            {
                var str = GetString(key);
                var obj = JsonConvert.DeserializeObject<T>(str);
                return obj;
            }
            catch
            {
                return default(T);
            }

        }

        public void SetInt(string key, int value)
        {
            NSUserDefaults.StandardUserDefaults.SetInt(value, key);
        }

        public int GetInt(string key)
        {
            return (int)NSUserDefaults.StandardUserDefaults.IntForKey(key);
        }

    }
}