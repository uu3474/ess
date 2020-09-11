using ess.Verbs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ess
{
    class Generator
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

        readonly Random _random;

        public async Task Generate(GenerateOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.SizeStr))
                throw new ArgumentException("Empty size str");

            if (string.IsNullOrWhiteSpace(options.FileName))
                throw new ArgumentException("Empty file name");

            var sizeLetter = options.SizeStr[^1];
            long multipiler = sizeLetter switch
            {
                'b' => 1L,
                'k' => 1024L,
                'm' => 1024L * 1024L,
                'g' => 1024L * 1024L * 1024L,
                _ => throw new ArgumentException($"Invalid size leter '{sizeLetter}'")
            };

            var sizeSpan = options.SizeStr.AsSpan(0, options.SizeStr.Length - 1);
            if (!long.TryParse(sizeSpan, out long size))
                throw new ArgumentException($"Invalid size leter '{sizeSpan.ToString()}'");

            long bytes = size * multipiler;

            Console.WriteLine($"Generate test file '{options.FileName}' of {bytes} bytes");

            await GenerateCore(options.FileName, bytes);
        }

        async Task GenerateCore(string fileName, long bytes)
        {

        }

    }

}
