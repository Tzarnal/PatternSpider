using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using Newtonsoft.Json;

namespace Plugin_Hearthstone
{
    [Export(typeof(IPlugin))]
    public class Hearthstone : IPlugin
    {
        public string Name { get { return "Hearthstone"; } }
        public string Description { get { return "Gives a link to a hearhstone card when invoked."; } }

        public List<string> Commands { get { return new List<string> { "hs" }; } }


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
            HtmlDocument document;
            
            try
            {
                document = UrlRequest(string.Format("http://www.hearthhead.com/cards=?filter=na={0}'", searchString));
            }
            catch
            {
                return "Error Occured trying to search for card.";
            }

            var jsonString = ExtractJsonStringFromPage(document);
            
            if (jsonString == null)
                return "No Results found for: " + searchString;

            var cards = ParseJson(jsonString);

            if (cards.Count == 1)
            {
                var card = cards[0];
                return CardToString(card);
            }


            return String.Format("[http://www.hearthhead.com/cards=?filter=na={1}] Found {0} cards.", cards.Count, searchString);
        }

        private List<Card> ParseJson(string data)
        {
            return JsonConvert.DeserializeObject<List<Card>>(data);
        }

        private string CardToString(Card card)
        {
            var cardText = "";
            var cardSet = "?";
            var cardClass = "?";
            var block = WildorStandard(card.set);

            if (Tables.Sets.ContainsKey(card.set))
            {
                 cardSet = Tables.Sets[card.set];
            }

            if (Tables.Classes.ContainsKey(card.cardClass))
            {
                cardClass = Tables.Classes[card.cardClass];
            }


            switch (card.type)
            {
                case 4:
                    cardText = String.Format("[http://www.hearthhead.com/card={5}] [{7}] [{8}] {6} Minion: {0} - {1}/{2} for {3} - {4}", 
                                            card.name, card.attack, card.health, card.cost, card.description, card.id, cardClass, cardSet, block);
                    break;
                case 5:
                    cardText = String.Format("[http://www.hearthhead.com/card={3}] [{5}] [{6}] {4} Spell: {0} - {1} mana - {2}",
                                            card.name, card.cost, card.description, card.id, cardClass, cardSet, block);
                    break;
                case 7:
                    cardText = string.Format("[http://www.hearthhead.com/card={4}] [{6}] [{7}] {5} Weapon: {0} - {1}/{2} for {3}",
                                             card.name, card.attack, card.durability, card.cost, card.id, cardClass, cardSet, block);
                    
                    if(!string.IsNullOrWhiteSpace(card.description))
                    {
                        cardText += " - " + card.description;
                    }
                    break;
                default:
                    cardText = String.Format("[http://www.hearthhead.com/card={3}] [{4}] [{5}] {0} - {1} mana - {2}",
                                             card.name, card.cost, card.description, card.id, cardSet, block);
                    break;
            }

            return cardText;
        }

        private string WildorStandard(int setId)
        {
            if (!Tables.Block.ContainsKey(setId))
            {
                return "-";
            }

            var cardBlock = Tables.Block[setId];

            if (Tables.StandardLegal.Contains(cardBlock))
            {
                return "Standard";
            }

            return "Wild";
        }

        private string ExtractJsonStringFromPage(HtmlDocument document)
        {
            string flatDocument = document.DocumentNode.InnerHtml;
            
            var matchString = @"hearthstoneCards.=.(\[([^]]+)\]);";
            var regex = new Regex(matchString, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (regex.IsMatch(flatDocument))
            {
                var match = regex.Match(flatDocument);
                return match.Groups[1].Value;
            }

            return null;
        }

        private HtmlDocument UrlRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Headers.Add("Accept-Language", "en;q=0.8");
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

            var responseStream = req.GetResponse().GetResponseStream();
            var document = new HtmlDocument();

            if (responseStream == null)
            {
                throw new NoNullAllowedException();
            }

            using (var reader = new StreamReader(responseStream))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        writer.Write(reader.ReadToEnd());
                        memoryStream.Position = 0;
                        document.Load(memoryStream, new UTF8Encoding());
                    }
                }
            }

            return document;
        }
    }
}
