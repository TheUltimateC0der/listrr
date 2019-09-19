using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Configuration
{
    public class HangFireConfiguration
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string DashboardPath { get; set; }

        public int Workers { get; set; }
    }
}