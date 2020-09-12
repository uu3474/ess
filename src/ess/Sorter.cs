using ess.Verbs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ess
{
    public class Sorter
    {
        static int _defaultDelay = 100; // 0.1 second
        static int _outputBufferInitSize = 100_000;

        readonly SortOptions _options;
        readonly long _maxDataSize;
        readonly long _chunkSize;
        readonly long _minChunkPartLoadSize;
        readonly long _mergeOutputBufferMaxSize;
        readonly int _sortUnitMaxLinesCount;
        readonly int _sortThreadsCount;

        // Используем Dataflow для удобной организации многопоточных конвейров
        readonly TransformBlock<ChunkWriter, ChunkWriter> _sortChunkBlock;
        readonly ActionBlock<ChunkWriter> _writeChunkBlock;
        readonly ActionBlock<List<string>> _writeResultPartBlock;

        public Sorter(SortOptions options)
        {
            _options = options;

            if (string.IsNullOrWhiteSpace(_options.InputFileName))
                throw new ArgumentException("Empty input file name");

            if (string.IsNullOrWhiteSpace(_options.OutputFileName))
                throw new ArgumentException("Empty output file name");

            _maxDataSize = SizeHelper.GetBytesFromSizeString(_options.MaxDataSizeStr);
            _chunkSize = SizeHelper.GetBytesFromSizeString(_options.ChunkSizeStr);
            _minChunkPartLoadSize = SizeHelper.GetBytesFromSizeString(_options.MinChunkPartLoadSizeStr);
            _mergeOutputBufferMaxSize = SizeHelper.GetBytesFromSizeString(_options.MergeOutputBufferMaxSizeStr);

            _sortUnitMaxLinesCount = _options.SortUnitMaxLinesCount;

            _sortThreadsCount = _options.SortThreadsCount <= 0
                ? Environment.ProcessorCount - 2
                : _options.SortThreadsCount;

            _sortChunkBlock = new TransformBlock<ChunkWriter, ChunkWriter>(SortChunk,
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _sortThreadsCount }); // Сортируем многопоточно
            _writeChunkBlock = new ActionBlock<ChunkWriter>(WriteChunk,
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 }); // Записываем чанки в один поток

            _sortChunkBlock.LinkTo(_writeChunkBlock);

            _writeResultPartBlock = new ActionBlock<List<string>>(WriteResultPartBlock,
                new ExecutionDataflowBlockOptions { EnsureOrdered = true, MaxDegreeOfParallelism = 1 }); // Записываем результат в один поток
        }

        void DeleteChunks()
        {
            if (Directory.Exists(Consts.ChunksDirectoryName))
                Directory.Delete(Consts.ChunksDirectoryName, true);
        }

        void DeleteOldResult()
        {
            if (File.Exists(_options.OutputFileName))
                File.Delete(_options.OutputFileName);
        }

        public async Task Sort()
        {
            DeleteChunks();
            Directory.CreateDirectory(Consts.ChunksDirectoryName);
            
            DeleteOldResult();

            Console.WriteLine($"Sort file '{_options.InputFileName}' to file '{_options.OutputFileName}', sort threads: {_sortThreadsCount}");
            var sortStopwatch = Stopwatch.StartNew();

            Console.WriteLine("Start create chunks");
            var chunksStopwatch = Stopwatch.StartNew();
            int chunksCount = await CreateChunks();
            chunksStopwatch.Stop();
            Console.WriteLine($"Create chunks DONE in {chunksStopwatch.Elapsed}");

            Console.WriteLine("Start merge chunks");
            var mergeStopwatch = Stopwatch.StartNew();
            await MergeChunks(chunksCount);
            mergeStopwatch.Stop();
            Console.WriteLine($"Merge chunks DONE in {mergeStopwatch.Elapsed}");

            sortStopwatch.Stop();
            Console.WriteLine($"Sort DONE in {sortStopwatch.Elapsed}");

            DeleteChunks();
        }

        ChunkWriter SortChunk(ChunkWriter chunk)
        {
            chunk.Sort();
            return chunk;
        }

        void WriteChunk(ChunkWriter chunk)
            => chunk.Write();

        bool IsChunkChainFull()
            => (_sortChunkBlock.InputCount + _sortChunkBlock.OutputCount + _writeChunkBlock.InputCount) * _chunkSize >= _maxDataSize;

        void WriteResultPartBlock(List<string> part)
            => File.AppendAllLines(_options.OutputFileName, part);

        bool IsOutputWriterFull()
            => _writeResultPartBlock.InputCount * _mergeOutputBufferMaxSize >= _maxDataSize;

        async Task<int> CreateChunks()
        {
            int nextChunkID = 0;

            using (var file = File.OpenText(_options.InputFileName))
            {
                long currentSize = 0L;
                var lines = new List<ProblemString>();
                while (true)
                {
                    string line = file.ReadLine();
                    if (line != null)
                    {
                        lines.Add(new ProblemString(line));
                        currentSize += Consts.GetLineSize(line);
                    }

                    if ((line == null || currentSize >= _chunkSize) && lines.Count > 0)
                    {
                        while (IsChunkChainFull())
                            await Task.Delay(_defaultDelay);

                        var chunk = new ChunkWriter(nextChunkID++, lines);
                        _sortChunkBlock.Post(chunk);

                        lines = new List<ProblemString>();
                        currentSize = 0L;
                    }

                    if (line == null)
                        break;
                }
            }

            _sortChunkBlock.Complete();
            await _sortChunkBlock.Completion;
            _writeChunkBlock.Complete();
            await _writeChunkBlock.Completion;

            return nextChunkID;
        }

        async Task WriteResultPart(List<string> resultPart)
        {
            while (IsOutputWriterFull())
                await Task.Delay(_defaultDelay);

            _writeResultPartBlock.Post(resultPart);
        }

        IEnumerable<List<T>> SplitList<T>(List<T> list, int elementsCount)
        {
            for (int i = 0; i < list.Count; i += elementsCount)
                yield return list.GetRange(i, Math.Min(elementsCount, list.Count - i));
        }

        /// <summary>
        /// Запуск многопоточной сортировки
        /// </summary>
        ILineSource GetSortOutputSource(int chunksCount)
        {
            long chunkPartLoadSize = _maxDataSize / chunksCount;
            if (chunkPartLoadSize < _minChunkPartLoadSize)
                chunkPartLoadSize = _minChunkPartLoadSize;

            var readers = new List<ChunkReader>(chunksCount);
            for (int i = 0; i < chunksCount; i++)
                readers.Add(new ChunkReader(i, chunkPartLoadSize)); // Долгая стадия чтения начала всех чанков, может стоило распаралеллить

            
            if (_sortThreadsCount <= 2 || readers.Count < _sortThreadsCount * 2) // Если чанков мало, или потоков мало, то будем сливать все чанки в одном потоке
            {
                var unit = new BackgroundSortUnit(readers, _sortUnitMaxLinesCount);
                return unit;
            }
            else // Если потоков и чанков хватает, запускаем сортировку в несколько потоков, а результаты этих сортировок соединяем ещё в одном потоке 
            {
                var subUnitsCount = _sortThreadsCount - 1;
                var readersToUnit = readers.Count / subUnitsCount + 1;
                var subUnits = SplitList(readers, readersToUnit)
                    .Select(x => new BackgroundSortUnit(x.ToList(), _sortUnitMaxLinesCount))
                    .ToList();

                var unit = new BackgroundSortUnit(subUnits, _sortUnitMaxLinesCount);
                return unit;
            }
        }

        async Task MergeChunks(int chunksCount)
        {
            var unit = GetSortOutputSource(chunksCount);

            long outputSize = 0L;
            var outputBuffer = new List<string>(_outputBufferInitSize);

            while(!unit.IsEmpty)
            {
                if (!unit.TryReadLine(out ProblemString line))
                    break;

                outputSize += Consts.GetLineSize(line.RawString);
                outputBuffer.Add(line.RawString);

                if (outputSize > _mergeOutputBufferMaxSize)
                {
                    await WriteResultPart(outputBuffer);
                    outputSize = 0L;
                    outputBuffer = new List<string>(_outputBufferInitSize);
                }
            }

            if (outputBuffer.Count > 0)
                await WriteResultPart(outputBuffer);

            _writeResultPartBlock.Complete();
            await _writeResultPartBlock.Completion;
        }

    }

}
