using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Chat.Model
{
    /// <summary>
    /// 数据模型基类
    /// </summary>
    public class ModelBase: INotifyPropertyChanged
    {
        public ModelBase()
        {
            DateTime = DateTime.Now;
        }
        public int Id { get; set; }
        public DateTime DateTime
        {
            get
            {
                return DateTimeStamp.ToDateTime();
            }
            set
            {
                DateTimeStamp = value.ToTimeStamp();
            }
        }
        public int DateTimeStamp { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
