using System;
using System.Collections.Generic;
using System.Text;

namespace ess
{
    /// <summary>
    /// Источник строк, может быть как чанк, так и сортировщик
    /// </summary>
    public interface ILineSource
    {
        /// <summary>
        /// Закончились ли строки
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// Попытаться прочитать строку
        /// </summary>
        bool TryReadLine(out ProblemString problemString);

    }

}
