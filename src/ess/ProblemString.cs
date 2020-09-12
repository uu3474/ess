using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ess
{
    /// <summary>
    /// Строка из задания
    /// </summary>
    public struct ProblemString : IEquatable<ProblemString>
    {
        public static ProblemString Empty { get; } = new ProblemString(string.Empty, -1, -1);

        /// <summary>
        /// Сама строка
        /// </summary>
        public string RawString { get; }

        /// <summary>
        /// Индекс точки
        /// </summary>
        public int PointIndex { get; }

        /// <summary>
        /// Цифра
        /// </summary>
        public int Number { get; }

        ProblemString(string rawString, int pointIndex, int number)
        {
            RawString = rawString;
            PointIndex = pointIndex;
            Number = number;
        }

        public ProblemString(string rawString)
        {
            RawString = rawString;

            PointIndex = rawString.IndexOf('.');
            if (PointIndex <= 0)
                throw new ArgumentException("Must contain number and point after", nameof(rawString));
            if (PointIndex == rawString.Length - 1)
                throw new ArgumentException("Must contain text after point", nameof(rawString));

            var numberSpan = rawString.AsSpan(0, PointIndex);
            if (!int.TryParse(numberSpan, out int parsedNumber))
                throw new ArgumentException("Must contain valid number", nameof(rawString));

            Number = parsedNumber;
        }

        public override string ToString()
            => RawString;

        public override int GetHashCode()
            => RawString.GetHashCode();

        public bool Equals(ProblemString other)
            => RawString == other.RawString;

    }

}
