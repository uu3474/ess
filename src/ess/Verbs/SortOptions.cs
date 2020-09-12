using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    [Verb("sort", HelpText = "Sort file")]
    public class SortOptions
    {
        [Option('i', "input", Required = false, HelpText = "Input file name", Default = "test.data")]
        public string InputFileName { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file name", Default = "sorted.data")]
        public string OutputFileName { get; set; }

        [Option('d', "datasize", Required = false, HelpText = "Max data size in memory, approximately", Default = "4g")]
        public string MaxDataSizeStr { get; set; }

        [Option('c', "chunk", Required = false, HelpText = "Size of chunk", Default = "100m")]
        public string ChunkSizeStr { get; set; }

        [Option('p', "chunkpart", Required = false, HelpText = "Size of chunk buffer due merge", Default = "2m")]
        public string MinChunkPartLoadSizeStr { get; set; }

        [Option('m', "mergeoutput", Required = false, HelpText = "Size of merge output buffer", Default = "100m")]
        public string MergeOutputBufferMaxSizeStr { get; set; }

        [Option('l', "mergeoutput", Required = false, HelpText = "Maximum count of lines buffered by sort unit", Default = 100_000)]
        public int SortUnitMaxLinesCount { get; set; }

        [Option('s', "sortthreads", Required = false, HelpText = "Threads count to sort chunks", Default = 0)]
        public int SortThreadsCount { get; set; }

    }

}
