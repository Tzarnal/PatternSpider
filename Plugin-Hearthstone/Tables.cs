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
    }
}
