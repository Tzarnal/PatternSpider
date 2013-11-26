using System;
using System.Diagnostics;

namespace PatternSpider.Irc
{
    public class InvalidCommandParametersException : Exception
    {
        public InvalidCommandParametersException(int minParameters, int? maxParameters = null)
            : base()
        {
            Debug.Assert(minParameters >= 0,
                "minParameters must be at least zero.");
            Debug.Assert(maxParameters == null || maxParameters >= minParameters,
                "maxParameters must be at least minParameters.");

            this.MinParameters = minParameters;
            this.MaxParameters = maxParameters ?? minParameters;
        }

        public int MinParameters
        {
            get;
            private set;
        }

        public int MaxParameters
        {
            get;
            private set;
        }

        public override string Message
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public string GetMessage(string command)
        {
            if (this.MinParameters == 0 && this.MaxParameters == 0)
                return string.Format("The command `{0}' does not take any parameters.", command);
            else if (this.MinParameters == this.MaxParameters)
                return string.Format("The command `{0}' does not take any parameters.", command,
                    this.MinParameters);
            else
                return string.Format("The command `{0}' takes between {1} and {2} parameters.", command,
                    this.MinParameters, this.MaxParameters);
        }
    }
}
