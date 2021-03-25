using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class RecurringEvent
    {
        public string title { get; set; }
        public int[] daysOfWeek { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string description { get; set; }
    }
}
