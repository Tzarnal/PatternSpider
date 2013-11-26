using System.Linq;
using System.Threading;

namespace PatternSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var cliMode = args.Any(arg => arg == "-interactive" || arg == "-i");
                                   
            var patternSpider = new PatternSpider();
            patternSpider.Run();
            
            if (cliMode)
            {
                var cli = new CLI(patternSpider);
                cli.Run();
            }
            else
            {
                while (true)
                {
                    Thread.Sleep(500);
                }
            }

            patternSpider.Quit();        
        }
    }
}
