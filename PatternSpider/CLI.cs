using System;
using System.Collections.Generic;
using System.Threading;


namespace PatternSpider
{
    class CLI
    {
        private bool _isRunning = true;
        private Dictionary<string, Action<string>> _commands;
        private PatternSpider _patternSpider;

        public CLI(PatternSpider patternSpider)
        {
            _patternSpider = patternSpider;
            
            _commands = new Dictionary<string, Action<string>>();            
            _commands.Add("quit",Quit);
            _commands.Add("reload",Reload);
            _commands.Add("reloadplugins", ReloadPlugins);
        }

        public void Run()
        {
            while (_isRunning)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                var explodedInput = input.Split(' ');
                var firstWord = explodedInput[0].ToLower();

                if (_commands.ContainsKey(firstWord))
                {
                    _commands[firstWord](input);
                }

            }
        }

        private void Quit(string input)
        {
            _patternSpider.Quit();
            Thread.Sleep(1000);
            _isRunning = false;            
        }

        private void Reload(string input)
        {
            _patternSpider.Quit();
            _patternSpider = null;
            _patternSpider = new PatternSpider();
            _patternSpider.Run();            
        }

        private void ReloadPlugins(string input)
        {
            _patternSpider.ReloadPlugins();
        }
    }
}
