using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Sentience
{
    class RandomResponse
    {
        private static string[] _responses = new[]
            {
                //Basic ones
                "Yes.",
                "No.",
                "...",

                //Magic 8ball
                "It is certain.",
                "It is decidedly so.",
                "Without a doubt.",
                "Yes – definitely.",
                "You may rely on it.",
                "As I see it, yes.",
                "Most likely.",
                "Outlook good.",
                "Signs point to ye.s",
                "Reply hazy, try again.",
                "Ask again later.",
                "Better not tell you now.",
                "Cannot predict now.",
                "Concentrate and ask again.",
                "Don't count on it.",
                "My reply is no.",
                "My sources say no.",
                "Outlook not so good.",
                "Very doubtful.",

                //Pattern Spider jokes
                "Oversight has been notified.",
                "Ha, rolled a 10 on my patternbite!",
                "(unintelligeble spider chitter)",
                "Help, I'm stuck in the loom and I can't get out.",
                "Man messing with Sidereals is so much fun.",
                
                //Being an IRCbot jokes
                "Can't you people just leave me alone.",
                "Zero's subtract, right. Pretty sure they do.",
                "Its because I HATE you {nick}.",
                "Whe have moved one step closer to world domination.",
                
                //Being a dick jokes
                "Its probably MoonWolfs fault.",
                "Its probably your own fault.",
                
                //Misc
                "It is known",
                "It is known",
                "I have spoken",

                //Smilies, why not
                ":P",
                ":D",
                ":O",
                ":)",
                ":(",
                "",
                "-_-",
                "^_^",
                ">_>",
                ">_<",
                "^_<",
                "O.o",
                ";_;",
                ">:c",
                
                //Memes                
                "That is objectivly suboptimal",
                "That is subjectivly optimal",
                "Indiscrimiate Murder Is Counter Productive",
                "Wyverns are NOT Dragons",
            };

        public static string Reponse(string nick)
        {
            var rand = new Random();

            var reponse = _responses[rand.Next(0, _responses.Count())];
            return reponse.Replace("{nick}", nick);
        }

    }
}
