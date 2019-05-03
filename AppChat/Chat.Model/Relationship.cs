using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Model
{
    /// <summary>
    /// 好友表
    /// </summary>
    public class Relationship : ModelBase
    {
        /// <summary>
        /// 主人
        /// </summary>

        public int MasterId { get; set; }
        /// <summary>
        /// 好友
        /// </summary>
        public int SlaverId { get; set; }

    }
}
