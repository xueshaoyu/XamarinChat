
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttNetServer
{
    static class Program
    {
        /// <summary>
        /// Main Entry。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists(SQLiteHelper.dbFilePath))
            {                
                var helper = new InitDbHelper();
                helper.InitDb(SQLiteHelper.dbFilePath);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMqttServer());
        }
    }
}
