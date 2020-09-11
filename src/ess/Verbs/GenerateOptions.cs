using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    [Verb("gen", HelpText = "Generate new test file")]
    class GenerateOptions
    {
        [Option('o', "output", Required = false, HelpText = "Output test file name, test.txt default", Default = "test.txt")]
        public string FileName { get; set; }

        [Option('s', "size", Required = false, HelpText = "Size of output test file name, 10m default", Default = "10m")]
        public string SizeStr { get; set; }

    }

}
