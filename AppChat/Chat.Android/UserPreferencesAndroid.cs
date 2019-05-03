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
        /// <summary>
        /// 设置字符串到本地存储
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>

        public void SetString(string key, string value)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutString(key, value);
            prefsEditor.Commit();
        }
        /// <summary>
        /// 获取本地存储的字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            return prefs.GetString(key, "");

        }
        /// <summary>
        /// 删除本地存储值
        /// </summary>
        /// <param name="key"></param>
        public void DeleteString(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            prefs.Edit().Remove(key).Commit();
        }
        /// <summary>
        /// 设置值到本地存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
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
        /// <summary>
        /// 获取本地存储值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 设置本地存储的int值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>

        public void SetInt(string key, int value)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutInt(key, value);
            prefsEditor.Commit();
        }
        /// <summary>
        /// 获取本地存储的int值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            return prefs.GetInt(key, 0);
        }
    }
}