using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StringsAreEvil
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.MonitoringIsEnabled = true;

            var dict = new Dictionary<string, ILineParser>
            {
                ["1"] = new LineParserV01(),
                ["2"] = new LineParserV02(),
                ["3"] = new LineParserV03(),
                ["4"] = new LineParserV04(),
                ["5"] = new LineParserV05(),
                ["6"] = new LineParserV06(),
                ["7"] = new LineParserV07(),
                ["8"] = new LineParserV08(),
                ["9"] = new LineParserV09(),
                ["10"] = new LineParserV10(),
                ["11"] = new LineParserV11(),
            };

#if DEBUG
            ViaRawStream(new LineParserV11());
            Environment.Exit(0);
#endif

            if (args.Length == 1 && dict.ContainsKey(args[0]))
            {
                var parameter = args[0];
                var lineParser = dict[parameter];

                if (parameter.Equals("11"))
                {
                    Console.WriteLine("Using " + lineParser.GetType().Name + " with a Raw Stream");
                    ViaRawStream(lineParser);
                }
                else
                {
                    Console.WriteLine("Using " + lineParser.GetType().Name + " with a StreamReader");
                    ViaStreamReader(lineParser);
                }

                //lineParser.Dump();
            }
            else
            {
                Console.WriteLine("Incorrect parameters");
                Environment.Exit(1);
            }

            Console.WriteLine($"Took: {AppDomain.CurrentDomain.MonitoringTotalProcessorTime.TotalMilliseconds:#,###} ms");
            Console.WriteLine($"Allocated: {AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize / 1024:#,#} kb");
            Console.WriteLine($"Peak Working Set: {Process.GetCurrentProcess().PeakWorkingSet64 / 1024:#,#} kb");

            for (var index = 0; index <= GC.MaxGeneration; index++)
            {
                Console.WriteLine($"Gen {index} collections: {GC.CollectionCount(index)}");
            }

            Console.WriteLine(Environment.NewLine);
        }

        private static void ViaStreamReader(ILineParser lineParser)
        {
            using (StreamReader reader = File.OpenText(@"..\..\example-input.csv"))
            {
                try
                {
                    while (reader.EndOfStream == false)
                    {
                        lineParser.ParseLine(reader.ReadLine());
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("File could not be parsed", exception);
                }
            }
        }

        private static void ViaRawStream(ILineParser lineParser)
        {
            var sb = new StringBuilder();

            var charPool = ArrayPool<char>.Shared;

            using (var reader = File.OpenRead(@"..\..\example-input.csv"))
            {
                try
                {
                    bool endOfFile = false;
                    while (reader.CanRead)
                    {
                        sb.Clear();

                        while (endOfFile == false)
                        {
                            var readByte = reader.ReadByte();

                            if (readByte == -1)
                            {
                                endOfFile = true;
                                break;
                            }

                            var character = (char)readByte;

                            if (character == '\r')
                            {
                                continue;
                            }

                            if (character == '\n')
                            {
                                break;
                            }

                            sb.Append(character);
                        }

                        if (endOfFile)
                        {
                            break;
                        }

                        char[] rentedCharBuffer = charPool.Rent(sb.Length);

                        try
                        {
                            for (int index = 0; index < sb.Length; index++)
                            {
                                rentedCharBuffer[index] = sb[index];
                            }

                            lineParser.ParseLine(rentedCharBuffer);
                        }
                        finally
                        {
                            charPool.Return(rentedCharBuffer, true);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("File could not be parsed", exception);
                }
            }
        }
    }
}
