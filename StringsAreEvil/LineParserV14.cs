using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StringsAreEvil
{
    /// <summary>
    /// Stats:-
    ///     Took: 6,063 ms
    ///     Allocated: 8 kb
    ///     Peak Working Set: 10,700 kb
    ///
    /// Change:-
    ///     We don't revisit the same index twice (might change speed if the line is long
    ///     Drop in allocation is from the small fixed sized buffer in the FileStream above
    /// </summary>
    public sealed class LineParserV14 : ILineParser
    {
        private List<ValueHolderAsStruct> list = new List<ValueHolderAsStruct>();

        public void ParseLine(string line)
        {
        }

        public void ParseLine(char[] line)
        {

        }

        public void Dump()
        {

        }

        public void ParseLine(StringBuilder line)
        {
            if (line[0] == 'M' && line[1] == 'N' && line[2] == 'O')
            {
                var startIndex = 3;
                int elementId = ParseSectionAsInt(line, ref startIndex); // equal to parts[1] - element id
                int vehicleId = ParseSectionAsInt(line, ref startIndex); // equal to parts[2] - vehicle id
                int term = ParseSectionAsInt(line, ref startIndex); // equal to parts[3] - term
                int mileage = ParseSectionAsInt(line, ref startIndex); // equal to parts[4] - mileage
                decimal value = ParseSectionAsDecimal(line, ref startIndex); // equal to parts[5] - value
                var valueHolder = new ValueHolderAsStruct(elementId, vehicleId, term, mileage, value);
                //list.Add(valueHolder);
            }
        }

        private static decimal ParseSectionAsDecimal(StringBuilder line, ref int startIndex)
        {
            decimal val = 0;
            bool seenDot = false;
            int fractionCounter = 10;
            int counter = 0;
            bool flip = false;

            for (var index = startIndex; index < line.Length; index++)
            {
                // move along the line until we have skipped the required amount of commas
                var c = line[index];
                if (c == ',')
                {
                    counter++;

                    if (counter == 2)
                    {
                        startIndex = index;
                        break;
                    }
                    continue;
                }

                // we have skipped enough commas, the next section before the upcoming comma is what we are interested in
                // the number is a negative means we have to flip it at the end.
                if (c == '-')
                {
                    flip = true;
                    continue;
                }

                if (c == '.')
                {
                    seenDot = true;
                    continue;
                }

                // before the . eg; 12.34 this looks for the 12
                if (seenDot == false)
                {
                    val *= 10;
                    val += c - '0';
                }
                else
                {
                    val += decimal.Divide(c - '0', fractionCounter);
                    fractionCounter *= 10;
                }
            }

            return flip ? -val : val;
        }

        private static int ParseSectionAsInt(StringBuilder line, ref int startIndex)
        {
            int val = 0;
            int counter = 0;
            bool flip = false;

            for (var index = startIndex; index < line.Length; index++)
            {
                var c = line[index];
                if (c == ',')
                {
                    counter++;

                    if (counter == 2)
                    {
                        startIndex = index;
                        break;
                    }
                    continue;
                }

                // the number is a negative means we have to flip it at the end.
                if (c == '-')
                {
                    flip = true;
                    continue;
                }

                val *= 10;
                val += c - '0';
            }

            return flip ? -val : val;
        }
    }
}