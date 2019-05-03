using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

    namespace MqttNetServer
    {
        /// <summary>
        /// 数据库初始化帮助类
        /// </summary>
        public class InitDbHelper
        {
            private string dbPath;
            /// <summary>
            /// 初始化数据库结构
            /// </summary>
            /// <param name="dbPath"></param>
            public void InitDb(string dbPath)
            {
                this.dbPath = dbPath;
                if (!File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Thread.Sleep(2);
                }
                SQLiteConnection.CreateFile(dbPath);
                CreateUserInfoTable();
                CreateMessageTable();
                CreateRelationshipTable();
            }

            public void CreateUserInfoTable()
            {
                var sql = @"CREATE TABLE UserInfo
                (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name NVARCHAR(50) NOT NULL,
                Password NVARCHAR(50) NOT NULL,
                Online INTEGER NOT NULL DEFAULT 0,
                DateTimeStamp INTEGER NOT NULL
                )";
                SQLiteHelper.ExecuteScalar(sql);
            }
            public void CreateMessageTable()
            {
                var sql = @"CREATE TABLE Message
                (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SendId INTEGER NOT NULL,
                ReceiveId INTEGER NOT NULL,
                Content NVARCHAR(500) NOT NULL,
                DateTimeStamp INTEGER NOT NULL
                )";
                SQLiteHelper.ExecuteScalar(sql);
            }
            public void CreateRelationshipTable()
            {
                var sql = @"CREATE TABLE Relationship
                (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MasterId INTEGER NOT NULL,
                SlaverId INTEGER NOT NULL,
                DateTimeStamp INTEGER NOT NULL
                )";
                SQLiteHelper.ExecuteScalar(sql);
            }
        }
    }