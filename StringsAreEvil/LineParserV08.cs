using System.Buffers;

namespace StringsAreEvil
{
    /// <summary>
    /// Stats:-
    ///     Took: 6,094 ms
    ///     Allocated: 1,160,794 kb
    ///     Peak Working Set: 16,796 kb
    ///
    /// Change:-
    ///     Begin to do manual int & decimal parsing.
    /// </summary>
    public sealed class LineParserV08 : ILineParser
    {
        private readonly ArrayPool<byte> _arrayPool;

        public LineParserV08()
        {
            _arrayPool = ArrayPool<byte>.Shared;
        }

        public void ParseLine(string line)
        {
            if (line.StartsWith("MNO"))
            {
                var tempBuffer = _arrayPool.Rent(7);

                try
                {
                    var findCommasInLine = FindCommasInLine(line, tempBuffer);
                    var elementId = ParseSectionAsInt(line, findCommasInLine[0] + 1, findCommasInLine[1]); // equal to parts[1] - element id
                    var vehicleId = ParseSectionAsInt(line, findCommasInLine[1] + 1, findCommasInLine[2]); // equal to parts[2] - vehicle id
                    var term = ParseSectionAsInt(line, findCommasInLine[2] + 1, findCommasInLine[3]); // equal to parts[3] - term
                    var mileage = ParseSectionAsInt(line, findCommasInLine[3] + 1, findCommasInLine[4]); // equal to parts[4] - mileage
                    var value = ParseSectionAsDecimal(line, findCommasInLine[4] + 1, findCommasInLine[5]); // equal to parts[5] - value
                    var valueHolder = new ValueHolder(elementId, vehicleId, term, mileage, value);
                }
                finally 
                {
                    _arrayPool.Return(tempBuffer, true);
                }
            }
        }

        public void ParseLine(char[] line)
        {
        }

        public void Dump()
        {
        }

        private decimal ParseSectionAsDecimal(string line, int start, int end)
        {
            decimal value = 0;
            bool seenDot = false;
            int fractionCounter = 10;

            for (var index = start; index < end; index++)
            {
                if (char.IsNumber(line[index]) && seenDot == false)
                {
                    value *= 10;
                    value += line[index] - '0';
                }
                else if (char.IsNumber(line[index]) && seenDot)
                {
                    value += decimal.Divide(line[index] - '0', fractionCounter);
                    fractionCounter *= 10;
                }
                else
                {
                    seenDot = true;
                }
            }

            return value;
        }

        private static int ParseSectionAsInt(string line, int start, int end)
        {
            int val = 0;

            for (var index = start; index < end; index++)
            {
                val *= 10;
                val += line[index] - '0';
            }

            return val;
        }

        private byte[] FindCommasInLine(string line, byte[] nums)
        {
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