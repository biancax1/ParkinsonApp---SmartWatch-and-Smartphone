using MonitoringParkinsonism.Code;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Android
{
    public partial class App : MonitoringParkinsonism.App
    {
        public override void RegisterServices()
        {
            Locator.CurrentMutable.RegisterLazySingleton<IAccelerometerDataService>(
                () => new AccelerometerDataService());
        }
    }
}
