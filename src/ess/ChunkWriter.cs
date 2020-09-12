using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ess
{
    public class ChunkWriter
    {
        List<ProblemString> _data;

        public int ID { get; }
        public IReadOnlyList<ProblemString> Data => _data;

        public ChunkWriter(int id, List<ProblemString> data)
        {
            ID = id;
            _data = data;
        }

        public void Sort()
        {
            Console.WriteLine($"Start sort chunk {ID}, {_data.Count} lines");
            var stopwatch = Stopwatch.StartNew();
            _data.Sort(ProblemStringComparer.Default);
            stopwatch.Stop();
            Console.WriteLine($"Sort chunk {ID} DONE in {stopwatch.Elapsed}, {_data.Count} lines");
        }

        public void Write()
        {
            Console.WriteLine($"Start write chunk {ID}, {_data.Count} lines");
            var stopwatch = Stopwatch.StartNew();
            File.WriteAllLines(Consts.GetChunkFileName(ID), _data.Select(x => x.RawString));
            stopwatch.Stop();
            Console.WriteLine($"Write chunk {ID} DONE in {stopwatch.Elapsed}, {_data.Count} lines");
        }

    }

}
