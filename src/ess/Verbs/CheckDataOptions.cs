using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    [Verb("checkdata", HelpText = "Check files has same lines")]
    public class CheckDataOptions
    {
        [Option('a', "filea", Required = false, HelpText = "First file name", Default = "test.data")]
        public string FirstFileName { get; set; }

        [Option('b', "fileb", Required = false, HelpText = "Second file name", Default = "sorted.data")]
        public string SecondFileName { get; set; }

    }

}
