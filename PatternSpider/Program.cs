using System;

namespace PatternSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            var patternSpider = new PatternSpider();
            patternSpider.Run();

            Console.WriteLine("Finished, press any key to continue");
            Console.ReadKey();

        }
    }
}
