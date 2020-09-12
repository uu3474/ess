using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    [Verb("gen", HelpText = "Generate new test file")]
    public class GenerateOptions
    {
        [Option('o', "output", Required = false, HelpText = "Output file name", Default = "test.data")]
        public string FileName { get; set; }

        [Option('s', "size", Required = false, HelpText = "Size of output test file name", Default = "10m")]
        public string SizeStr { get; set; }

    }

}
