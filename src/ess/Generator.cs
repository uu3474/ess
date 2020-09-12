using ess.Verbs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ess
{
    public class Generator
    {
        static char[] _vowels = new char[] 
        { 
            'A', 'E', 'I', 'O', 'U', 'Y' 
        };
        static char[] _consonants = new char[] 
        { 
            'B', 'C', 'D', 'F', 'G', 'H', 'J', 
            'K', 'L', 'M', 'N', 'P', 'Q', 'R', 
            'S', 'T', 'V', 'W', 'X', 'Y', 'Z' 
        };
        static int _maxNumber = 100_000;
        static int _maxDictionarySize = 100_000;

        readonly GenerateOptions _options;
        readonly long _size;
        readonly Random _random = new Random();

        public Generator(GenerateOptions options)
        {
            _options = options;

            if (string.IsNullOrWhiteSpace(_options.FileName))
                throw new ArgumentException("Empty file name");

            _size = SizeHelper.GetBytesFromSizeString(_options.SizeStr);
        }

        public Task Generate()
        {
            Console.WriteLine($"Generate test file '{_options.FileName}' of ~{_size} bytes");
            var stopwatch = Stopwatch.StartNew();
            long linesCount = GenerateCore();
            stopwatch.Stop();
            Console.WriteLine($"Generate DONE in {stopwatch.Elapsed}, {linesCount} lines");

            return Task.CompletedTask;
        }

        int GenerateText(TextWriter writer)
        {
            int size = 0;

            var wordsCount = _random.Next(1, 20);
            for (int i = 0; i < wordsCount; i++)
            {
                var syllablesCount = _random.Next(3, 10);
                for (int j = 0; j < syllablesCount; j++)
                {
                    writer.Write(_consonants[_random.Next(_consonants.Length)]);
                    writer.Write(_vowels[_random.Next(_vowels.Length)]);
                    size += 2 * Consts.CharSize;
                }

                if (i != wordsCount - 1)
                {
                    writer.Write(' ');
                    size += Consts.CharSize;
                }
            }

            return size;
        }

        long GenerateCore()
        {
            long linesCount = 0L;
            var dictionary = new List<string>(_maxDictionarySize);
            using (var writer = new StreamWriter(_options.FileName, false))
            {
                long generated = 0L;
                int prevPercent = 0;
                while (generated < _size)
                {
                    var percent = (int)(((float)generated / _size) * 100);
                    if (percent != prevPercent && percent != 100)
                    {
                        Console.WriteLine($"{percent}%");
                        prevPercent = percent;
                    }

                    var numberStr = _random.Next(_maxNumber).ToString();

                    writer.Write(numberStr);
                    generated += numberStr.Length * Consts.CharSize;

                    writer.Write('.');
                    writer.Write(' ');
                    generated += Consts.CharSize * 2;

                    int action = _random.Next(3);
                    switch (action)
                    {
                        case 0:
                            generated += GenerateText(writer);
                            break;
                        case 1:
                            if (dictionary.Count < _maxDictionarySize)
                            {
                                using var textBuffer = new StringWriter();
                                generated += GenerateText(textBuffer);
                                var text = textBuffer.ToString();
                                dictionary.Add(text);
                                writer.Write(text);
                            }
                            else
                            {
                                generated += GenerateText(writer);
                            }
                            break;
                        case 2:
                            if (dictionary.Count > 0)
                            {
                                var text = dictionary[_random.Next(dictionary.Count)];
                                writer.Write(text);
                                generated += text.Length * Consts.CharSize;
                            }
                            else
                            {
                                generated += GenerateText(writer);
                            }
                            break;
                        default:
                            throw new Exception("Internal error");
                    }

                    writer.WriteLine();
                    generated += Consts.LineBreakSize;
                    linesCount++;
                }
            }

            return linesCount;
        }

    }

}
