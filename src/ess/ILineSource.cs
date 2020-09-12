using System;
using System.Collections.Generic;
using System.Text;

namespace ess
{
    public interface ILineSource
    {
        bool IsEmpty { get; }
        bool TryReadLine(out ProblemString problemString);

    }

}
