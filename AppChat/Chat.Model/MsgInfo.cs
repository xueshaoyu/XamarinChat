﻿using Chat.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Model
{
    public class MsgInfo: ModelBase
    {
        public int SendId { get; set; }
        public int ReceiveId { get; set; }
        public string Content { get; set; }
        public int IsRead { get; set; }
    }
}
