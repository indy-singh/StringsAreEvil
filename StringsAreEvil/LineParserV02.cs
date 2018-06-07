using System.Text;

namespace StringsAreEvil
{
    /// <summary>
    /// Stats:-
    ///     Took: 6,969 ms
    ///     Allocated: 4,288,215 kb
    ///     Peak Working Set: 16,640 kb
    ///
    /// Change:-
    ///     Use the orginal parts array
    /// </summary>
    public sealed class LineParserV02 : ILineParser
    {
        public void ParseLine(string line)
        {
            var parts = line.Split(',');
            if (parts[0] == "MNO")
            {
                var valueHolder = new ValueHolder(parts);
            }
        }

        public void ParseLine(char[] line)
        {
        }

        public void Dump()
        {
        }

        public void ParseLine(StringBuilder line)
        {
            
        }
    }
}