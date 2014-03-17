using System.Linq;
using System.Management.Instrumentation;

namespace Plugin_Replace
{
    class LineHistory
    {
        private string[] _history;

        public LineHistory(int size)
        {
            _history = new string[size];
            for (var i = 0; i < _history.Length ; i++)
            {
                _history[i] = string.Empty;
            }
        }

        public bool HasMatch(string text)
        {
            return _history.Any(line => line.Contains(text));
        }

        public void AddLine(string line)
        {
            //move all entries in _history down one
            for (var i = 0; i < _history.Length -1; i++)
            {
                _history[i] = _history[i+1];
            }

            //add new line
            _history[_history.Length-1] = line;
        }

        public string Replace(string original, string replacement)
        {
            foreach (var line in _history.Reverse())
            {
                if (line.Contains(original))
                {
                    return line.Replace(original, replacement);
                }
            }

            throw new InstanceNotFoundException("Could not find "+original+" in History");
        }
    }
}
