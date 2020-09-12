using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    /// <summary>
    /// Параметры подсчета кол-ва строк в файле
    /// </summary>
    [Verb("lines", HelpText = "Get file lines count")]
    public class LinesCountOptions
    {
        /// <summary>
        /// Файл для проверки, файл по умолчанию с входными данными
        /// </summary>
        [Option('i', "input", Required = false, HelpText = "File name to count lines", Default = "test.data")]
        public string FileName { get; set; }

    }

}
