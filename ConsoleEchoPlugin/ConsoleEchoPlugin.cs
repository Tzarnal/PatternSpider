using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using PatternSpider.Plugins;

namespace ConsoleEchoPlugin
{
    [Export(typeof(IPlugin))]
    class ConsoleEchoPlugin : IPlugin
    {
        public List<string> ParseMessage(string message)
        {
            Console.WriteLine(message);

            return null; 
        }

        public List<string>  ParseQuery(string message)
        {
            Console.WriteLine(message);

            return null; 
        }
    }
}
