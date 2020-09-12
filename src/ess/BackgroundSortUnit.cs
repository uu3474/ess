using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ess
{
    public class BackgroundSortUnit : ILineSource
    {
        readonly IReadOnlyList<ILineSource> _sources;
        readonly ConcurrentQueue<ProblemString> _buffer;

        public long MaxOutputLinesCount { get; }
        public bool IsEmpty => Completion.IsCompleted && _buffer.Count == 0;
        public Task Completion { get; private set; }

        public BackgroundSortUnit(IReadOnlyList<ILineSource> sources, int maxOutputLinesCount)
        {
            _sources = sources;
            _buffer = new ConcurrentQueue<ProblemString>();
            MaxOutputLinesCount = maxOutputLinesCount;

            Completion = Task.Run(Sort);
        }

        bool ReturnFromBuffer(out ProblemString problemString)
        {
            _buffer.TryDequeue(out problemString);
            return true;
        }

        bool ReturnEmpty(out ProblemString problemString)
        {
            problemString = ProblemString.Empty;
            return false;
        }

        public bool TryReadLine(out ProblemString problemString)
        {
            if (IsEmpty)
                return ReturnEmpty(out problemString);

            if (_buffer.Count > 0)
                return ReturnFromBuffer(out problemString);

            SpinWait.SpinUntil(() => Completion.IsCompleted || _buffer.Count > 0);

            if (IsEmpty)
                return ReturnEmpty(out problemString);

            if (_buffer.Count > 0)
                return ReturnFromBuffer(out problemString);

            throw new Exception("Logic error");
        }

        bool TryInsertLine(SortedDictionary<ProblemString, List<ILineSource>> tree, ILineSource source)
        {
            if (!source.TryReadLine(out ProblemString line))
                return false;

            if (!tree.TryGetValue(line, out List<ILineSource> readers))
            {
                readers = new List<ILineSource>(1);
                tree.Add(line, readers);
            }
            readers.Add(source);

            return true;
        }

        void Sort()
        {
            var tree = new SortedDictionary<ProblemString, List<ILineSource>>(ProblemStringComparer.Default);

            for (int i = 0; i < _sources.Count; i++)
                TryInsertLine(tree, _sources[i]);

            while (tree.Count > 0)
            {
                var min = tree.First();
                tree.Remove(min.Key);

                for (int i = 0; i < min.Value.Count; i++)
                {
                    var source = min.Value[i];
                    if (source.IsEmpty)
                        continue;

                    TryInsertLine(tree, source);
                }

                for (int i = 0; i < min.Value.Count; i++)
                    _buffer.Enqueue(min.Key);

                while (_buffer.Count >= MaxOutputLinesCount)
                    SpinWait.SpinUntil(() => _buffer.Count < MaxOutputLinesCount);
            }
        }

    }

}
