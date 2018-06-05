namespace StringsAreEvil
{
    public interface ILineParser
    {
        void ParseLine(string line);
        void ParseLine(char[] line);
        void Dump();
    }
}