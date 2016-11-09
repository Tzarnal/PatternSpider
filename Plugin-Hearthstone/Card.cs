using System.Collections.Generic;

namespace Plugin_Hearthstone
{
    public class Card
    {
        public string _id { get; set; }
        public string card_id { get; set; }
        public int cost { get; set; }
        public string set { get; set; }
        public string format { get; set; }
        public string faction { get; set; }
        public string type { get; set; }
        public string rarity { get; set; }
        public bool collectible { get; set; }
        public string text { get; set; }
        public string name { get; set; }
        public string card_slug { get; set; }
        public string artist { get; set; }
        public string flavor_text { get; set; }
        public string card_class { get; set; }
        public int legacy_card_id { get; set; }
        public List<string> mechanics { get; set; }
        public int? attack { get; set; }
        public int? health { get; set; }
        public int? durability { get; set; }
        public string how_to_earn { get; set; }
        public string how_to_earn_golden { get; set; }
    }
}