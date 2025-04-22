using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Code
{
    public class AccelerometerDataEventArgs : EventArgs
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public long timeStamp { get; }

        public AccelerometerDataEventArgs(float x, float y, float z, long timeStamp)
        {
            X = x;
            Y = y;
            Z = z;
            this.timeStamp = timeStamp;
        }
    }
}
