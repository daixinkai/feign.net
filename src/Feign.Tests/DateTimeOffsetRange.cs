using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    /// <summary>
    /// 表示一个时间范围
    /// </summary>
    public class DateTimeOffsetRange
    {
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}
