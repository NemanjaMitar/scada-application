using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator.Model
{
    // Unikatni properties za sve input tagove
    public interface IInputTags
    {
        TimeSpan ScanTime { get; }
        bool IsOnScan { get; }
    }
}
