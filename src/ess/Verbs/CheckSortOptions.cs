using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    /// <summary>
    /// Параметры для проверки подрядка строк в отсортированном файле
    /// </summary>
    [Verb("checksort", HelpText = "Check file is sort")]
    public class CheckSortOptions
    {
        /// <summary>
        /// Файл для проверки, файл по умолчанию для результатов сортировки
        /// </summary>
        [Option('i', "input", Required = false, HelpText = "File name to check is sort", Default = "sorted.data")]
        public string FileName { get; set; }

    }

}
