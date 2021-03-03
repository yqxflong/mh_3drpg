using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BE.Level
{
    /// <summary>
    /// 区域代号
    /// </summary>
    [Flags]
    public enum enZoneType
    {
        Zone_0 = 1,
        Zone_1 = 1 << 1,
        Zone_2 = 1 << 2,
        Zone_3 = 1 << 3,
        Zone_4 = 1 << 4,
        Zone_5 = 1 << 5,
        Zone_6 = 1 << 6,
        Zone_7 = 1 << 7,
        Zone_8 = 1 << 8,
        Zone_9 = 1 << 9,
        Zone_10 = 1 << 10,
        Zone_11 = 1 << 11,
        Zone_12 = 1 << 12,
    }
}
