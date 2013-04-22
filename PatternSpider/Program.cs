using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternSpider.Irc;

namespace PatternSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new IrcBot();

            bot.Run();
        }
    }
}
