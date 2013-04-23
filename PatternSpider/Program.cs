using System;
using IrcDotNet;
using PatternSpider.Irc;

namespace PatternSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var userinfo = new IrcUserRegistrationInfo
            {
                NickName = "DevSpider",
                UserName = "DevSpider",
                RealName = "DevSpider"
            };
                        
            var bot1 = new IrcBot();
            var bot2 = new IrcBot();

            bot1.Run();
            bot1.Connect("irc.mmoirc.com", userinfo);
            bot1.OnUserMessage += UserMessage;
            bot1.Join("#bot");
            bot1.QuitMessage = "Spider Out";

            bot2.Run();
            bot2.Connect("irc.sorcery.net", userinfo);
            bot2.OnUserMessage += UserMessage;
            bot2.Join("#bot");

            Console.ReadLine();
            bot1.Stop();
            bot2.Stop();
        }

        private static void UserMessage(object sender, IrcBot ircBot, IrcMessageEventArgs e)
        {            
            Console.WriteLine("[{0}] <{1}>: {2}", ircBot.Server, e.Source, e.Text);
        }

    }
}
