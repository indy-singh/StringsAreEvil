using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringsAreEvil
{
    /// <summary>
    /// Original implementation.
    /// 
    /// Stats:-
    ///     Took: 8,797 ms
    ///     Allocated: 7,412,234 kb
    ///     Peak Working Set: 16,524 kb
    /// </summary>
    public sealed class LineParserV01 : ILineParser
    {
        private List<ValueHolder> list = new List<ValueHolder>();

        public void ParseLine(string line)
        {
            var parts = line.Split(',');
            if (parts[0] == "MNO")
            {
                var valueHolder = new ValueHolder(line);
                //list.Add(valueHolder);
            }
        }

        public void ParseLine(char[] line)
        {
        }

        public void Dump()
        {
            File.WriteAllLines(@"..\..\v01.txt", list.Select(x => x.ToString()));
        }
    }
}