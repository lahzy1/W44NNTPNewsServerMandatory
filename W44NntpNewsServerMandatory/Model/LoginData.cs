using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W44NntpNewsServerMandatory.Model
{
    internal class LoginData
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
    }
}
