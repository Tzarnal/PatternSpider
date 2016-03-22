using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Hearthstone
{
    class Tables
    {
        public static Dictionary<int,string> Classes = new Dictionary<int, string>
        {
            {0, "Neutral"},
            {1, "Warrior"},
            {2, "Paladin"},
            {3, "Hunter"},
            {4, "Rogue"},
            {5, "Priest"},
            {6, "Druid"},
            {7, "Shaman"},
            {8, "Mage"},
            {9, "Warlock"},                
        };

        public static Dictionary<int, string> Sets = new Dictionary<int, string>
        {
            {1, "Unknown"},    
            {2, "Basic" },
            {3, "Classic" },
            {4, "Reward" },
            {5, "Tutorial" },
            {11, "Promo" },
            {12, "Naxx" },
            {13, "GvG" },
            {14, "BRM" },
            {15, "TGT" },
            {16, "Credits" },
            {17, "Alt Heroes" },
            {18, "Brawl" },
            {20, "LoE" },
            {21, "WOG" }
        };

        public static Dictionary<int, string> Block = new Dictionary<int, string>
        {
            {2, "Basic" },
            {3, "Classic" },
            {4, "Wild" },            
            {11, "Wild" },
            {12, "Year1" },
            {13, "Year1"},
            {14, "Year2" },
            {15, "Year2" },
            {20, "Year2" },
            {21, "Kraken" }
        };

        public static List<string> StandardLegal = new List<string>
        {
            "Basic",
            "Classic",            
            "Year2",
            "Kraken"
        };
    }
}
