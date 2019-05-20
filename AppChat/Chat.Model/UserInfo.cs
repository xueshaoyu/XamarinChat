using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Model
{
    public class UserInfo:ModelBase
    {
        public string Name { get; set; }
        public string Password { get; set; }
        private int online = 0;
        public int Online { get{
                return online;
            } set
            {
                if (value != online)
                {
                    online = value;
                    OnPropertyChanged("Online");
                }
            }
        }
        public bool HasNewMessage { get; set; }
    }
}
