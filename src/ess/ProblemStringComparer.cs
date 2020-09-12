using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ess
{
    public class ProblemStringComparer : IComparer<ProblemString>
    {
        public static ProblemStringComparer Default { get; } = new ProblemStringComparer();

        public int Compare(ProblemString x, ProblemString y)
        {
            var xStringPart = x.RawString.AsSpan(x.PointIndex + 1);
            var yStringPart = y.RawString.AsSpan(y.PointIndex + 1);
            var stringPartCompareResult = MemoryExtensions.CompareTo(xStringPart, yStringPart, StringComparison.Ordinal);

            return stringPartCompareResult == 0
                ? x.Number - y.Number
                : stringPartCompareResult;
        }
    }

}
