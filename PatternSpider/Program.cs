using System;
using PatternSpider.Irc;

namespace PatternSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot1 = new IrcBot();
            var bot2 = new IrcBot();

            bot1.Run();          
            bot1.Connect("irc.mmoirc.com",bot1.RegistrationInfo);

            bot2.Run();
            bot2.Connect("irc.sorcery.net", bot2.RegistrationInfo);

            Console.ReadLine();
            bot1.Stop();
            bot2.Stop();
        }

    }
}
