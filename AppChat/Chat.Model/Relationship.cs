using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Model
{
   public class Relationship : ModelBase
    {

      
        public int MasterId { get; set; }
        public int SlaverId { get; set; }
       
    }
}
