namespace StringsAreEvil
{
    /// <summary>
    /// Stats:-
    ///     Took: 5,609 ms
    ///     Allocated: 768,739 kb
    ///     Peak Working Set: 16,344 kb
    ///
    /// Change:-
    ///     Flipping the value holder to a struct instead of a class
    /// </summary>
    public sealed class LineParserV10 : ILineParser
    {
        public void ParseLine(string line)
        {
            if (line.StartsWith("MNO"))
            {
                int elementId = ParseSectionAsInt(line, 1); // equal to parts[1] - element id
                int vehicleId = ParseSectionAsInt(line, 2); // equal to parts[2] - vehicle id
                int term = ParseSectionAsInt(line, 3); // equal to parts[3] - term
                int mileage = ParseSectionAsInt(line, 4); // equal to parts[4] - mileage
                decimal value = ParseSectionAsDecimal(line, 5); // equal to parts[5] - value
                var valueHolder = new ValueHolderAsStruct(elementId, vehicleId, term, mileage, value);
            }
        }

        public void ParseLine(char[] line)
        {
        }

        public void Dump()
        {
        }

        private static decimal ParseSectionAsDecimal(string line, int numberOfCommasToSkip)
        {
            decimal val = 0;
            bool seenDot = false;
            ulong fractionCounter = 10;
            int counter = 0;
            bool flip = false;

            for (var index = 0; index < line.Length; index++)
            {
                // move along the line until we have skipped the required amount of commas
                if (line[index] == ',')
                {
                    counter++;

                    if (counter > numberOfCommasToSkip)
                    {
                        break;
                    }
                    continue;
                }

                // we have skipped enough commas, the next section before the upcoming comma is what we are interested in
                if (counter == numberOfCommasToSkip)
                {
                    // the number is a negative means we have to flip it at the end.
                    if (line[index] == '-')
                    {
                        flip = true;
                        continue;
                    }

                    if (line[index] == '.')
                    {
                        seenDot = true;
                        continue;
                    }

                    // before the . eg; 12.34 this looks for the 12
                    if (char.IsNumber(line[index]) && seenDot == false)
                    {
                        val *= 10;
                        val += line[index] - '0';
                        continue;
                    }

                    // after the . eg; 12.34 this looks for the 34
                    if (char.IsNumber(line[index]) && seenDot == true)
                    {
                        val += decimal.Divide(line[index] - '0', fractionCounter);
                        fractionCounter *= 10;
                        continue;
                    }
                }
            }

            return flip ? -val : val;
        }

        private static int ParseSectionAsInt(string line, int numberOfCommasToSkip)
        {
            int val = 0;
            int counter = 0;
            bool flip = false;

            for (var index = 0; index < line.Length; index++)
            {
                // move along the line until we have skipped the required amount of commas
                if (line[index] == ',')
                {
                    counter++;

                    if (counter > numberOfCommasToSkip)
                    {
                        break;
                    }

                    continue;
                }

                // we have skipped enough commas, the next section before the upcoming comma is what we are interested in
                if (counter == numberOfCommasToSkip)
                {
                    // the number is a negative means we have to flip it at the end.
                    if (line[index] == '-')
                    {
                        flip = true;
                        continue;
                    }

                    val *= 10;
                    val += line[index] - '0';
                }
            }

            return flip ? -val : val;
        }
    }
}