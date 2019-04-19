using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Core
{
    public class UserInfo
    {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString("D");
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
