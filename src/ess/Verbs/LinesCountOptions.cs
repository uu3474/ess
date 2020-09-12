using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    [Verb("lines", HelpText = "Get file lines count")]
    public class LinesCountOptions
    {
        [Option('i', "input", Required = false, HelpText = "File name to count lines", Default = "test.data")]
        public string FileName { get; set; }

    }

}
