using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using Newtonsoft.Json;

namespace Plugin_Hearthstone
{
    [Export(typeof(IPlugin))]
    public class Hearthstone : IPlugin
    {
        public string Name => "Hearthstone";
        public string Description => "Gives a link to a hearhstone card when invoked.";

        public List<string> Commands => new List<string> { "hs" };


        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var text = m.Text.Trim();
            var messageParts = text.Split(' ');
            var searchString = string.Join(" ",messageParts.Skip(1));

            var searchResult = SearchHearthHead(searchString);

            return new List<string> { searchResult };
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }

        private string SearchHearthHead(string searchString)
        {            
            List<Card> cards;
            string searchUrl = $"http://hearthstone.services.zam.com/v1/card?sort=cost,name&search={searchString}&cost=0,1,2,3,4,5,6,7&type=MINION,SPELL,WEAPON&collectible=true";
            string jsonData;


            try
            {                
                jsonData = new WebClient().DownloadString(searchUrl);
            }
            catch
            {
                return "Error Occured trying to search for card.";
            }
            
            //Console.WriteLine(jsonData);
                        
            if (string.IsNullOrWhiteSpace(jsonData))
                return "No Results found for: " + searchString;


            //try
            {
                cards = ParseJson(jsonData);
            }
            //catch
            //{
                //return string.Format("[http://www.hearthhead.com/cards=?filter=na={0}] Did not understand results.", searchString);
            //}

            if (cards.Count == 1)
            {
                var card = cards[0];
                return CardToString(card);
            }

            return $"[http://www.hearthhead.com/cards] Found {cards.Count} results.";
        }

        private static List<Card> ParseJson(string data)
        {
            return JsonConvert.DeserializeObject<List<Card>>(data);
        }

        private string CardToString(Card card)
        {
            string cardText;
            var zamName = card.name.ToLower().Replace(" ", "-");            
            var cardClass = ReCapitilize(card.card_class);
            card.text = CorrectCardText(card.text);

            var cardSet = card.set;
            if (Tables.BlockNameCorrection.ContainsKey(cardSet))
            {
                cardSet = Tables.BlockNameCorrection[cardSet];
            }

            var block = "Unknown";
            if (Tables.Block.ContainsKey(cardSet))
            {
                block = Tables.Block[cardSet];
            }

            var format = "Wild";
            if (Tables.StandardLegal.Contains(block))
            {
                format = "Standard";
            }

            switch (card.type)
            {
                case "MINION":
                    cardText = string.Format("[http://www.hearthhead.com/cards/{5}] [{7}] [{8}] {6} Minion: {0} - {1}/{2} for {3} - {4}", 
                                            card.name, card.attack, card.health, card.cost, card.text, zamName, cardClass, cardSet, format);
                    break;
                case "SPELL":
                    cardText = string.Format("[http://www.hearthhead.com/cards/{3}] [{5}] [{6}] {4} Spell: {0} - {1} mana - {2}",
                                            card.name, card.cost, card.text, zamName, cardClass, cardSet, format);
                    break;
                case "WEAPON":
                    cardText = string.Format("[http://www.hearthhead.com/cards/{4}] [{6}] [{7}] {5} Weapon: {0} - {1}/{2} for {3}",
                                             card.name, card.attack, card.durability, card.cost, zamName, cardClass, cardSet, format);
                    
                    if(!string.IsNullOrWhiteSpace(card.text))
                    {
                        cardText += " - " + card.text;
                    }
                    break;
                default:
                    cardText = string.Format("[http://www.hearthhead.com/cards/{3}] [{4}] [{5}] {0} - {1} mana - {2}",
                                             card.name, card.cost, card.text, zamName, card.set, block);
                    break;
            }

            return cardText;
        }

        private string ReCapitilize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            
            return text[0].ToString().ToUpper() + text.Substring(1).ToLower();
        }

        private string CorrectCardText(string text)
        {
            //Insert points for details
            var newText = Regex.Replace(text, "{.}", string.Empty);

            //Markup
            newText = Regex.Replace(newText, "<.>", string.Empty);
            newText = Regex.Replace(newText, "</.>", string.Empty);

            //@ Symbols
            newText = newText.Replace("@", " ");

            return newText;
        }
    }
}
