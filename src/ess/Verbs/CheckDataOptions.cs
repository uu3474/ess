using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    /// <summary>
    /// Параметры для сравнения строк в файлах
    /// </summary>
    [Verb("checkdata", HelpText = "Check files has same lines")]
    public class CheckDataOptions
    {
        /// <summary>
        /// Первый файл, файл по умолчанию с входными данными
        /// </summary>
        [Option('a', "filea", Required = false, HelpText = "First file name", Default = "test.data")]
        public string FirstFileName { get; set; }

        /// <summary>
        /// Второй файл, файл по умолчанию для результатов сортировки
        /// </summary>
        [Option('b', "fileb", Required = false, HelpText = "Second file name", Default = "sorted.data")]
        public string SecondFileName { get; set; }

    }

}
