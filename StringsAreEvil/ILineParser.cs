using System.Text;

namespace StringsAreEvil
{
    public interface ILineParser
    {
        void ParseLine(string line);
        void ParseLine(char[] line);
        void Dump();
        void ParseLine(StringBuilder line);
    }
}