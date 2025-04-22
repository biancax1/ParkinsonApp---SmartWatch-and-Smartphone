using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonitoringParkinsonism.Models
{
    public class Sleep 
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long TotalSleepTimeInADay { get; set; }

        public Sleep(long StartTime, long EndTime, long TotalSleepTimeInADay)
        {
            this.StartTime= DateTimeOffset.FromUnixTimeMilliseconds(StartTime).UtcDateTime;
            this.EndTime = DateTimeOffset.FromUnixTimeMilliseconds(EndTime).UtcDateTime;
            this.TotalSleepTimeInADay = TotalSleepTimeInADay;
        }

        [JsonConstructor]
        public Sleep(DateTime StartTime, DateTime EndTime, long TotalSleepTimeInADay)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.TotalSleepTimeInADay = TotalSleepTimeInADay;
        }


    }
}
