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

            var dict = new Dictionary<string, Action>
            {
                ["1"] = () =>
                {
                    Console.WriteLine("#1 ViaStreamReader");
                    ViaStreamReader(new LineParserV01());
                },
                ["2"] = () =>
                {
                    Console.WriteLine("#2 ViaStreamReader");
                    ViaStreamReader(new LineParserV02());
                },
                ["3"] = () =>
                {
                    Console.WriteLine("#3 ViaStreamReader");
                    ViaStreamReader(new LineParserV03());
                },
                ["4"] = () =>
                {
                    Console.WriteLine("#4 ViaStreamReader");
                    ViaStreamReader(new LineParserV04());
                },
                ["5"] = () =>
                {
                    Console.WriteLine("#5 ViaStreamReader");
                    ViaStreamReader(new LineParserV05());
                },
                ["6"] = () =>
                {
                    Console.WriteLine("#6 ViaStreamReader");
                    ViaStreamReader(new LineParserV06());
                },
                ["7"] = () =>
                {
                    Console.WriteLine("#7 ViaStreamReader");
                    ViaStreamReader(new LineParserV07());
                },
                ["8"] = () =>
                {
                    Console.WriteLine("#8 ViaStreamReader");
                    ViaStreamReader(new LineParserV08());
                },
                ["9"] = () =>
                {
                    Console.WriteLine("#9 ViaStreamReader");
                    ViaStreamReader(new LineParserV09());
                },
                ["10"] = () =>
                {
                    Console.WriteLine("#10 ViaStreamReader");
                    ViaStreamReader(new LineParserV10());
                },
                ["11"] = () =>
                {
                    Console.WriteLine("#11 ViaRawStream");
                    ViaRawStream(new LineParserV11());
                },
                ["12"] = () =>
                {
                    Console.WriteLine("#12 ViaRawStream2");
                    ViaRawStream(new LineParserV12());
                },
                ["14"] = () =>
                {
                    Console.WriteLine("#14 ViaRawStream2");
                    ViaRawStreamWithSmallBuffer(new LineParserV14());
                },
            };


#if DEBUG
            dict["14"]();
            Environment.Exit(0);
#endif

            if (args.Length == 1 && dict.ContainsKey(args[0]))
            {
                dict[args[0]]();
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

        private static void ViaRawStreamWithSmallBuffer(ILineParser lineParser)
        {
            var sb = new StringBuilder();

            using (var reader = new FileStream(@"..\..\example-input.csv", FileMode.Open, FileAccess.Read, FileShare.None, 256))
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

                        lineParser.ParseLine(sb);
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
