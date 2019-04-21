using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Content.Core;
using Newtonsoft.Json;

namespace Chat.Droid
{
    public class UserPreferencesAndroid : IUserPreferences
    {
        public UserPreferencesAndroid()
        {
        }


        public void SetString(string key, string value)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutString(key, value);
            prefsEditor.Commit();
        }

        public string GetString(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            return prefs.GetString(key, "");

        }

        public void DeleteString(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            prefs.Edit().Remove(key).Commit();
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
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutInt(key, value);
            prefsEditor.Commit();
        }

        public int GetInt(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            return prefs.GetInt(key, 0);
        }
    }
}