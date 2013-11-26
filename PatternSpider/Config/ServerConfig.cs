using System.Collections.Generic;

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
