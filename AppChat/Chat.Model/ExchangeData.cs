using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Model
{
    /// <summary>
    /// 数据交换类
    /// </summary>
    public class ExchangeData
    {
        /// <summary>
        /// 标识请求是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
