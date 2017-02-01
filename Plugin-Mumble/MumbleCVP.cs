using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plugin_Mumble
{

    public class Channel
    {
        public string description { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int parent { get; set; }
        public int position { get; set; }
        public List<object> links { get; set; }
        public bool temporary { get; set; }
        public string x_connecturl { get; set; }
        public List<Channel> channels { get; set; }
        public List<User> users { get; set; }
    }

    public class User
    {
        public string Name;
        public bool Deaf;
    }

    public class Root
    {
        public string description { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int parent { get; set; }
        public int position { get; set; }
        public List<object> links { get; set; }
        public bool temporary { get; set; }
        public string x_connecturl { get; set; }
        public List<Channel> channels { get; set; }
        public List<User> users { get; set; }
    }

    public class MumbleCVP
    {
        public int id { get; set; }
        public string name { get; set; }
        public string x_connecturl { get; set; }
        public int x_uptime { get; set; }
        public Root root { get; set; }

        public static MumbleCVP Load(string data)
        {            
            return JsonConvert.DeserializeObject<MumbleCVP>(data);
        }
    }
}
