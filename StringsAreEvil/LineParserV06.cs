using System.Text;

namespace StringsAreEvil
{
    /// <summary>
    /// Stats:-
    ///     Took: 8,219 ms
    ///     Allocated: 2,454,235 kb
    ///     Peak Working Set: 16,328 kb
    ///
    /// Change:-
    ///     Whilst the length of the line isn't fixed, the number of section are
    ///     and they are always delimited by a comma. We can optimize for this
    /// </summary>
    public sealed class LineParserV06 : ILineParser
    {
        private readonly StringBuilder _stringBuilder;

        public LineParserV06()
        {
            _stringBuilder = new StringBuilder();
        }

        public void ParseLine(string line)
        {
            if (line.StartsWith("MNO"))
            {
                var findCommasInLine = FindCommasInLine(line);
                var elementId = ParseSectionAsInt(findCommasInLine[0] + 1, findCommasInLine[1], line); // equal to parts[1] - element id
                var vehicleId = ParseSectionAsInt(findCommasInLine[1] + 1, findCommasInLine[2], line); // equal to parts[2] - vehicle id
                var term = ParseSectionAsInt(findCommasInLine[2] + 1, findCommasInLine[3], line); // equal to parts[3] - term
                var mileage = ParseSectionAsInt(findCommasInLine[3] + 1, findCommasInLine[4], line); // equal to parts[4] - mileage
                var value = ParseSectionAsDecimal(findCommasInLine[4] + 1, findCommasInLine[5], line); // equal to parts[5] - value
                var valueHolder = new ValueHolder(elementId, vehicleId, term, mileage, value);
            }
        }

        public void ParseLine(char[] line)
        {
        }

        public void Dump()
        {
        }

        private decimal ParseSectionAsDecimal(int start, int end, string line)
        {
            _stringBuilder.Clear();

            for (var index = start; index < end; index++)
            {
                _stringBuilder.Append(line[index]);
            }

            return decimal.Parse(_stringBuilder.ToString());
        }

        private int ParseSectionAsInt(int start, int end, string line)
        {
            _stringBuilder.Clear();

            for (var index = start; index < end; index++)
            {
                _stringBuilder.Append(line[index]);
            }

            return int.Parse(_stringBuilder.ToString());
        }

        private byte[] FindCommasInLine(string line)
        {
            byte[] nums = new byte[7];
            byte counter = 0;

            for (byte index = 0; index < line.Length; index++)
            {
                if (line[index] == ',')
                {
                    nums[counter++] = index;
                }
            }

            return nums;
        }
    }
}