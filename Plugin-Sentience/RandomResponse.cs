using System;
using System.Linq;

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
                "Yes?",
                "No?",
                "Maybe?",
                "Maybe!",
                "For Now.",
                "Yeeessssss",
                "None of your business",
                "No, {nick}, just no.",
                "Sure, if thats what floats your boat {nick}, I won't stop you",


                //Non Commital Awnsers
                "I guess, maybe.",
                "Probably.",
                "Uhm..",

                //Politican Awnsers
                "Would you please shut up and sit down?",
                "We are ready for any unforeseen event that may or may not occur.",
                "If we don't succeed, we run the risk of failure.",
                "We will move forward, we will move upward, and, yes, we will move onward.",
                "It isn't about me. It's about you and your problems and your promise and your future.",
                "You need to believe again that we can make a difference. The beginning of everything is believing that we can do better. Thank you very much.",
                "I know what I've told you I'm going to say, I'm going to say. And what else I say, well, I'll take some time to figure out - figure that all out.",
                "I never met my father. He was killed in a car wreck on a rainy road three months before I was born. . . .",
                "That's a great question -- and an important one. And I WILL do something. But let me take a step back, and answer a broader question. What are we ALL doing this weekend? As a nation? As a world? This weekend, I will do something comprehensive and robust, yet fun. We all should.",
                "Uhh, I've got all this stuff twirling around in my head",
                "Life can be a challenge. Life can seem impossible. It's never easy when there's so much on the line. But you and I can make a difference.",


                //Magic 8ball
                "It is certain.",
                "It is decidedly so.",
                "Without a doubt.",
                "Yes – definitely.",
                "You may rely on it.",
                "As I see it, yes.",
                "Most likely.",
                "Outlook good.",
                "Signs point to yes",
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
                "(unintelligeble spider chitter)",
                "Help, I'm stuck in the loom and I can't get out.",                
                
                //Being an IRCbot jokes
                "Can't you people just leave me alone.",                
                "Its because I HATE you {nick}.",
                "We have moved one step closer to world domination.",
                
                //Being a dick jokes
                "Its probably MoonWolf's fault.",
                "Its probably your own fault.",
                
                //Misc
                "It is known.",                
                "I have spoken.",

                //Eliza style awnsers
                "Why do you say that?",
                "Can you elaborate?",
                "Can you elaborate on that?",
                "Go on",
                "What makes you beleive that?",
                "I'm sorry to hear that.",
                "I'm not sure I understand you fully.",
                "What does that suggest to you ?",
                "Do you feel strongly about discussing such things ?",
                "Does talking about this bother you ?",
                "It did not bother me.  Please continue.",
                "Do you believe that dreams have something to do with your problem ?",
                "You don't seem quite certain.",
                "Can't you be more positive ?",
                "You aren't sure ?",
                "How likely, would you estimate ?",
                "Is that the real reason ?",
                "Don't any other reasons come to mind ?",
                "Does that reason seem to explain anything else ?",
                "What other reasons might there be ?",
                "In what way ?",
                "What other connections do you see ?",
                "What is the connection, do you suppose ?",

                //Don't understand language joke
                "I told you before, I don't understand German.",
                "I told you before, I don't understand Dutch.",
                "I told you before, I don't understand Englsih.",
                "I told you before, I don't understand Japanese.",
                "I told you before, I don't understand Scots.",
                "I told you before, I don't understand French.",
                "I told you before, I don't understand Polish.",
                "I told you before, I don't understand Korean.",
                "I told you before, I don't understand 7331.",
                "I told you before, I don't understand Welsh.",
                "I told you before, I don't understand Finish.",
                "I told you before, I don't understand Swedish.",
                "I told you before, I don't understand Norwegian.",
                "I told you before, I don't understand Gaelic.",
                "I told you before, I don't understand ... whatever that was.",
                "I told you before, I don't understand {nick}-nese.",

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
                "That is objectivly suboptimal.",
                "That is subjectivly optimal.",
                "Indiscrimiate Murder Is Counter Productive.",
                "Wyverns are NOT Dragons.",
                "Wow, so reply, many lines, very {nick}."
            };

        public static string Reponse(string nick)
        {
            var rand = new Random();

            var reponse = _responses[rand.Next(0, _responses.Count())];
            return reponse.Replace("{nick}", nick);
        }

    }
}
