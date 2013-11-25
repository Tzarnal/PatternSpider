using System;

namespace PatternSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var patternSpider = new PatternSpider();
            patternSpider.Run();

            Console.WriteLine("Press any key to close");
            Console.ReadKey();

            patternSpider.Quit();

            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }
    }
}
