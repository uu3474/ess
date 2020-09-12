using ess.Verbs;
using System;
using System.Threading.Tasks;
using CommandLine;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace ess
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if DEBUG
            // В дебаге игнорируем параметры и берем их их stdin, это удобно для отладки
            Console.WriteLine("Pls enter arguments:");
            args = Console.ReadLine().Split();
#endif
            await CommandLine.Parser.Default.ParseArguments<GenerateOptions, SortOptions, LinesCountOptions, CheckSortOptions, CheckDataOptions>(args)
                .MapResult(
                    async (GenerateOptions options) => await Generate(options),
                    async (SortOptions options) => await Sort(options),
                    async (LinesCountOptions options) => await LinesCount(options),
                    async (CheckSortOptions options) => await CheckSort(options),
                    async (CheckDataOptions options) => await CheckData(options),
                    errors => Task.FromResult(1));
        }

        static async Task Generate(GenerateOptions options)
            => await new Generator(options).Generate();

        static async Task Sort(SortOptions options)
            => await new Sorter(options).Sort();

        static Task LinesCount(LinesCountOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.FileName))
                throw new ArgumentException("Empty file name");

            Console.WriteLine($"Get lines count for file '{options.FileName}'");

            long linesCount = 0L;
            using var file = File.OpenText(options.FileName);
            while (file.ReadLine() != null)
                linesCount++;

            Console.WriteLine($"Get lines count DONE, {linesCount} lines");

            return Task.CompletedTask;
        }

        static Task CheckSort(CheckSortOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.FileName))
                throw new ArgumentException("Empty file name");

            using var file = File.OpenText(options.FileName);
            if (file.EndOfStream)
                throw new ArgumentException("Empty file");

            Console.WriteLine($"Check file is sort for file '{options.FileName}'");

            long linesCount = 0L;
            
            var prevLine = new ProblemString(file.ReadLine());
            linesCount++;

            while (!file.EndOfStream)
            {
                var line = new ProblemString(file.ReadLine());
                linesCount++;

                if (ProblemStringComparer.Default.Compare(prevLine, line) > 0)
                {
                    Console.WriteLine("Wrong order found:");
                    Console.WriteLine($"[{linesCount - 1}] {prevLine.RawString}");
                    Console.WriteLine($"[{linesCount}] {line.RawString}");
                    break;
                }

                prevLine = line;
            }

            Console.WriteLine($"Check file is sort DONE, {linesCount} lines");

            return Task.CompletedTask;
        }

        static Task CheckData(CheckDataOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.FirstFileName))
                throw new ArgumentException("Empty first file name");

            if (string.IsNullOrWhiteSpace(options.SecondFileName))
                throw new ArgumentException("Empty second file name");

            Console.WriteLine($"Check files data start");

            Console.WriteLine($"Load first file '{options.FirstFileName}'");
            var firstFileLines = File.ReadLines(options.FirstFileName)
                .Select(x => new ProblemString(x))
                .ToList();

            Console.WriteLine($"Sort first file '{options.FirstFileName}'");
            firstFileLines.Sort(ProblemStringComparer.Default);

            Console.WriteLine($"Load second file '{options.SecondFileName}'");
            var secondFileLines = File.ReadLines(options.SecondFileName)
                .Select(x => new ProblemString(x))
                .ToList();

            Console.WriteLine($"Sort second file '{options.SecondFileName}'");
            secondFileLines.Sort(ProblemStringComparer.Default);

            Console.WriteLine($"Compare");
            if (firstFileLines.SequenceEqual(secondFileLines))
                Console.WriteLine($"Check files data DONE, data is same");
            else
                Console.WriteLine($"Check files data DONE, DATA IS NOT SAME");

            return Task.CompletedTask;
        }

    }

}
