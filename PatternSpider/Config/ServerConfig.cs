using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternSpider.Config
{
    class ServerConfig
    {
        public string Address;
        
        public string RealName;
        public string NickName;
        public string NickservPassword;

        public List<string> Channels;

        public ServerConfig()
        {
            Channels = new List<string>();
        }
    }
}
