using MonitoringParkinsonism.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Android
{
    public class AccelerometerDataService : IAccelerometerDataService
    {
        public event EventHandler<AccelerometerDataEventArgs> AccelerometerDataReceived
        {
            add => DataLayerListenerService.AccelerometerDataReceived += value;
            remove => DataLayerListenerService.AccelerometerDataReceived -= value;
        }
    }
}
