using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    /// <summary>
    /// Параметры генерации
    /// </summary>
    [Verb("gen", HelpText = "Generate new test file")]
    public class GenerateOptions
    {
        /// <summary>
        /// Имя выходного файла
        /// </summary>
        [Option('o', "output", Required = false, HelpText = "Output file name", Default = "test.data")]
        public string FileName { get; set; }

        /// <summary>
        /// Размер выходного файла, состоит из размера и литерала множителя(b, k, m, g), пример 10m - 10 МБ
        /// </summary>
        [Option('s', "size", Required = false, HelpText = "Size of output test file name", Default = "10m")]
        public string SizeStr { get; set; }

    }

}
