using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    [Verb("checksort", HelpText = "Check file is sort")]
    public class CheckSortOptions
    {
        [Option('i', "input", Required = false, HelpText = "File name to check is sort", Default = "sorted.data")]
        public string FileName { get; set; }

    }

}
