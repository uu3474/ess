using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ess
{
    public class ChunkReader : ILineSource, IDisposable
    {
        StreamReader _chunk;
        readonly Queue<ProblemString> _buffer;

        public int ID { get; }
        public long BufferSize { get; }
        public bool EOF => _chunk == null;
        public bool IsEmpty => _buffer.Count == 0 && EOF;

        public ChunkReader(int id, long bufferSize)
        {
            ID = id;
            BufferSize = bufferSize;
            _chunk = new StreamReader(Consts.GetChunkFileName(id));
            _buffer = new Queue<ProblemString>();

            Load();
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

        void Load()
        {
            if (EOF)
                return;

            long readed = 0L;
            while (readed < BufferSize)
            {
                var lineRaw = _chunk.ReadLine();
                if (lineRaw == null)
                {
                    _chunk.Dispose();
                    _chunk = null;
                    break;
                }

                var line = new ProblemString(lineRaw);
                readed += Consts.GetLineSize(line.RawString);
                _buffer.Enqueue(line);
            }
        }

        public bool TryReadLine(out ProblemString problemString)
        {
            if (_buffer.Count > 0)
            {
                return ReturnFromBuffer(out problemString);
            }
            else
            {
                if (EOF)
                    return ReturnEmpty(out problemString);

                Load();

                return _buffer.Count > 0
                    ? ReturnFromBuffer(out problemString)
                    : ReturnEmpty(out problemString);
            }
        }

        public void Dispose()
            => _chunk?.Dispose();

        public override string ToString()
            => $"{ID}, eof: {EOF}, buffer: {_buffer.Count}";

    }

}
