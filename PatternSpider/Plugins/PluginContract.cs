using System.Collections.Generic;

namespace PatternSpider.Plugins
{
    public interface IPlugin
    {
        List<string> ParseMessage(string message);
        List<string> ParseQuery(string essage);
    }
}
