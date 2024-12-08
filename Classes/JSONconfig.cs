using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests.Classes
{
    public class JSONconfig
    {
        public string? IPAddress { get; set; }
        public int Port { get; set; }   

        public string? Username { get; set; }
        public string? Password { get; set; }

    }
}
