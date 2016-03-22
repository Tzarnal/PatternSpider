using System;
using Newtonsoft.Json;

namespace Plugin_Hearthstone
{
    public class Card
    {
        public int id { get; set; }
        public string image { get; set; }
        public int? set { get; set; }
        public int? quality { get; set; }
        public string icon { get; set; }
        public int? type { get; set; }
        public int? cost { get; set; }

        [JsonProperty(PropertyName = "classs")]
        public int? cardClass { get; set; }
        
        public int? attack { get; set; }
        public int? health { get; set; }
        public int? durability { get; set; }
        public int? faction { get; set; }
        public int? elite { get; set; }
        public int? collectible { get; set; }
        public int? race { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? popularity { get; set; }
    }
}