using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ess.Verbs
{
    /// <summary>
    /// Параметры сортировки
    /// </summary>
    [Verb("sort", HelpText = "Sort file")]
    public class SortOptions
    {
        /// <summary>
        /// Входной файл
        /// </summary>
        [Option('i', "input", Required = false, HelpText = "Input file name", Default = "test.data")]
        public string InputFileName { get; set; }

        /// <summary>
        /// Выходной файл
        /// </summary>
        [Option('o', "output", Required = false, HelpText = "Output file name", Default = "sorted.data")]
        public string OutputFileName { get; set; }

        /// <summary>
        /// Примерный(очень) объем данных, которым может оперировать сортировщик
        /// </summary>
        [Option('d', "datasize", Required = false, HelpText = "Max data size in memory, approximately", Default = "2g")]
        public string MaxDataSizeStr { get; set; }

        /// <summary>
        /// Размер чанка
        /// </summary>
        [Option('c', "chunk", Required = false, HelpText = "Size of chunk", Default = "100m")]
        public string ChunkSizeStr { get; set; }

        /// <summary>
        /// Минимальный размер буфера при чтении данных из чанка при слиянии
        /// </summary>
        [Option('p', "chunkpart", Required = false, HelpText = "Size of chunk buffer due merge", Default = "2m")]
        public string MinChunkPartLoadSizeStr { get; set; }

        /// <summary>
        /// Размер выходного буфера слияния
        /// </summary>
        [Option('m', "mergeoutput", Required = false, HelpText = "Size of merge output buffer", Default = "100m")]
        public string MergeOutputBufferMaxSizeStr { get; set; }

        /// <summary>
        /// Размер выходного буфера для инстанса многопоточного сортировщика
        /// </summary>
        [Option('l', "mergeoutput", Required = false, HelpText = "Maximum count of lines buffered by sort unit", Default = 100_000)]
        public int SortUnitMaxLinesCount { get; set; }

        /// <summary>
        /// Кол-во потоков для сортировки, используется для создания чанков и слияния, по умолчани число процессоров минус 2
        /// </summary>
        [Option('s', "sortthreads", Required = false, HelpText = "Threads count to sort chunks", Default = 0)]
        public int SortThreadsCount { get; set; }

    }

}
