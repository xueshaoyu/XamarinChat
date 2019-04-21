using Chat.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Content.Core
{
    public class HttpClientHelper
    {
        HttpClient client = new HttpClient();

        public async Task<int> Register(UserInfo userInfo)
        {
            var retuanObj = 0;
            var content = new StringContent(JsonConvert.SerializeObject(userInfo),Encoding.UTF8);
            var response = await client.PostAsync(App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString()) + "Register", content);
            var txt = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ExchangeData>(txt);
            if (result.IsSuccess)
            {
                int.TryParse(result.Data.ToString(), out retuanObj);
            }
            return retuanObj;
        }
        public async Task<int> Login(UserInfo userInfo)
        {
            var retuanObj = 0;
            var content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8);
            var response = await client.PostAsync(App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString()) + "Login", content);
            var txt = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ExchangeData>(txt);
            if (result.IsSuccess)
            {
                int.TryParse(result.Data.ToString(), out retuanObj);
            }
            return retuanObj;
        }
        public async void Message(MsgInfo msgInfo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(msgInfo), Encoding.UTF8);
            var response = await client.PostAsync(App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString()) + "Message", content);
        }

        public async void Online(UserInfo userInfo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8);
            var response = await client.PostAsync(App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString()) + "Online", content);
        }
        public async void Offline(UserInfo userInfo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8);
            var response = await client.PostAsync(App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString()) + "Offline", content);
        }
        public async Task<List<UserInfo>> GetFrinds(UserInfo userInfo)
        {
            var retuanObj = new List<UserInfo>();
            var content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8);
            var response = await client.PostAsync(App.UserPreferences.GetString(EnumUserPreferences.ServerAddress.ToString()) + "Frinds", content);
            var txt = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ExchangeData>(txt);
            if (result.IsSuccess)
            {
                retuanObj = JsonConvert.DeserializeObject<List<UserInfo>>(result.Data.ToString());
            }
            return retuanObj;
        }
    }
}
