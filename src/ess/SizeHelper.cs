using System;
using System.Collections.Generic;
using System.Text;

namespace ess
{
    public static class SizeHelper
    {
        /// <summary>
        /// Получить размер в байтах из строки размера;
        /// Формат: цифра + литерал;
        /// Литералы:
        ///     b - байты;
        ///     k - килобайты;
        ///     m - мегабайты;
        ///     g - гигабайты;
        /// </summary>
        public static long GetBytesFromSizeString(string sizeStr)
        {
            if (string.IsNullOrWhiteSpace(sizeStr))
                throw new ArgumentException("String is empty", nameof(sizeStr));

            var sizeNumber = sizeStr.AsSpan(0, sizeStr.Length - 1);
            if (!long.TryParse(sizeNumber, out long size))
                throw new ArgumentException($"Invalid size number");

            var sizeLetter = sizeStr[^1];
            long multipiler = sizeLetter switch
            {
                'b' => 1L,
                'k' => 1024L,
                'm' => 1024L * 1024L,
                'g' => 1024L * 1024L * 1024L,
                _ => throw new ArgumentException($"Invalid size leter, allowed: b - bytes, k - KB, m - MB, g - GB")
            };

            long bytes = size * multipiler;
            return bytes;
        }

    }

}
