using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ess
{
    public static class Consts
    {
        /// <summary>
        /// Размер символа;
        /// Все символы, которые я использую, занимают 1 байт;
        /// </summary>
        public static int CharSize { get; } = 1;

        /// <summary>
        /// Размер перевода строки
        /// </summary>
        public static int LineBreakSize { get; } = 2;

        /// <summary>
        /// ПОлучить размер строки в байтах
        /// </summary>
        public static long GetLineSize(string line)
            => line.Length * CharSize + LineBreakSize;

        /// <summary>
        /// Имя временной директории для хранения чанков
        /// </summary>
        public static string ChunksDirectoryName { get; } = "chunks";

        /// <summary>
        /// Расширение файла чанка
        /// </summary>
        public static string ChunkFileExtension { get; } = ".chunk";

        /// <summary>
        /// Получить имя файла чанка
        /// </summary>
        public static string GetChunkFileName(int id)
            => Path.Combine(ChunksDirectoryName, id.ToString() + ChunkFileExtension);

    }

}
