using Chat.Model;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MqttNetServer
{
    public class RequestHelper
    {
        private HttpListenerRequest request;
        public RequestHelper(HttpListenerRequest request)
        {
            this.request = request;
        }
        public Stream RequestStream { get; set; }
        public void ExtracHeader()
        {
            RequestStream = request.InputStream;
        }

        public delegate void ExecutingDispatch(ExchangeData data);
        public void DispatchResources(ExecutingDispatch action)
        {
            ExchangeData data = new ExchangeData();
            try
            {
                var rawUrl = request.RawUrl;
                if (request.Url.Segments.Length == 3 && request.Url.Segments[0] == "/" && request.Url.Segments[1].ToLower() == "api/")
                {//Filter url
                    UserInfo userInfo = null;
                    MsgInfo msgInfo = null;
                    if (request.HttpMethod == "POST" && request.InputStream.CanRead)
                    {
                        StreamReader reader = new StreamReader(request.InputStream);
                        string bodyContent = reader.ReadToEnd();
                        userInfo = JsonConvert.DeserializeObject<UserInfo>(bodyContent);
                        msgInfo = JsonConvert.DeserializeObject<MsgInfo>(bodyContent);
                    }
                    if (userInfo == null && userInfo == null)
                    {
                        data.Data = "No Data!";
                    }
                    else
                    {
                        data.IsSuccess = true;
                        switch (request.Url.Segments[2].ToLower())
                        {
                            case "register":
                                var obj = SQLiteHelper.ExecuteScalar(string.Format("Select Id From UserInfo Where Name='{0}'", userInfo.Name));
                                if (obj != null)
                                {
                                    data.IsSuccess = false;
                                    data.Data = "The User Has Exist!";
                                }
                                else
                                {
                                    var idObj = SQLiteHelper.ExecuteScalar(string.Format("Insert Into UserInfo(Name,Password,DateTimeStamp)Values('{0}','{1}',{2});select last_insert_rowid() from UserInfo;", userInfo.Name, userInfo.Password, userInfo.DateTimeStamp));
                                    var id = Convert.ToInt32(idObj);
                                    data.Data = id;
                                }
                                break;
                            case "login":
                                var tb = SQLiteHelper.ExecuteDataTable(string.Format("Select Id,Name,Password From UserInfo Where Name='{0}' And Password='{1}'", userInfo.Name, userInfo.Password));
                                if (tb != null && tb.Rows.Count > 0 && tb.Rows[0]["Id"] != null)
                                {
                                    data.Data = tb.Rows[0]["Id"];
                                }
                                else
                                {
                                    data.IsSuccess = false;
                                    data.Data = "Login Error";
                                }
                                break;
                            case "frinds":
                                var dataTable = SQLiteHelper.ExecuteDataTable(string.Format("Select * From UserInfo  Where Id!={0}", userInfo.Id));
                               var users= ConvertDtToModels(dataTable);
                                data.Data = users;
                                break;
                            case "online":
                                var OnNum = SQLiteHelper.ExecuteNonQuery(string.Format("Update UserInfo Set OnLine=1 Where Id={0}", userInfo.Id));
                                data.Data = OnNum > 0;
                                break;
                            case "offline":
                                var num = SQLiteHelper.ExecuteNonQuery(string.Format("Update UserInfo Set OnLine=0 Where Id={0}", userInfo.Id));
                                data.Data = num > 0;
                                break;
                            case "message":
                                if (msgInfo == null)
                                {
                                    data.IsSuccess = false;
                                    data.Data = "No Data!";
                                }
                                else
                                {
                                    var mnum = SQLiteHelper.ExecuteNonQuery(string.Format("Insert Into MsgInfo(SendId,ReceiveId,Content)Values({0},{1},{2})", msgInfo.SendId, msgInfo.ReceiveId, msgInfo.Content));
                                    data.Data = mnum > 0;
                                }
                                break;
                            default:
                                data.IsSuccess = false;
                                break;
                        }
                    }
                }
                else
                {
                    data.IsSuccess = false;
                }
            }
            catch(Exception ex)
            {
                data.IsSuccess = false;
                data.Data = ex;
            }
            action?.Invoke(data);
        }
        private UserInfo ConvertRowToModel(DataRow dr)
        {
            if (dr != null)
            {
                UserInfo user = new UserInfo();
                user.Id = (int)dr["Id"].ToString().ToInt();
                user.Online = (int)dr["Online"].ToString().ToInt();
                user.Name = (string)dr["Name"];
                user.DateTimeStamp = (int)dr["DateTimeStamp"].ToString().ToInt();

                return user;
            }
            else { return null; }
        }
        private List<UserInfo> ConvertDtToModels(DataTable dt)
        {
            List<UserInfo> users = new List<UserInfo>();
            if(dt!=null&&dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                   var user= ConvertRowToModel(dt.Rows[i]);
                    if(user!=null)
                    {
                        users.Add(user);
                    }
                }
            }
            return users;
        }
    }
}
