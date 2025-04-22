using Android.App;
using Android.Gms.Wearable;
using Android.Util;
using MonitoringParkinsonism.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Android
{
    [Service(Exported = true, Name = "com.monitoring.parkinsonism.datalayerlistener")]
    [IntentFilter(new[] { "com.google.android.gms.wearable.BIND_LISTENER" })]
    public class DataLayerListenerService : WearableListenerService
    {
        const string Tag = "DataLayerService";

        public static event EventHandler<AccelerometerDataEventArgs> AccelerometerDataReceived;

        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            try { 
            foreach (var dataEvent in dataEvents)
            {
                if (dataEvent.Type == DataEvent.TypeChanged)
                {
                    var dataItem = dataEvent.DataItem;
                    DataMapItem dataMapItem = DataMapItem.FromDataItem(dataItem);
                    var dataMap = dataMapItem.DataMap;

                    if (dataMap == null)
                    {
                        Console.WriteLine("Error: dataMap is null");
                        return;
                    }
                    long timeStamp = 0;


                    switch (dataItem.Uri.Path) {
                        case "/accelerometer_data":
                            float x = dataMap.GetFloat("x");
                            float y = dataMap.GetFloat("y");
                            float z = dataMap.GetFloat("z");
                            timeStamp = dataMap.GetLong("timeStamp");

                            //Log.Debug(Tag, $"Received accelerometer data: x={x}, y={y}, z={z}");

                            AccelerometerDataReceived?.Invoke(this, new AccelerometerDataEventArgs(x, y, z, timeStamp));
                            break;
                        case "/steps_data":
                            int totalStepsInDay = dataMap.GetInt("totalStepsInDay");
                            RegisteredServices.OnStepsReceived(totalStepsInDay);
                            break;

                        case "/activity_data":
                            bool isActive = dataMap.GetBoolean("isActive");
                            timeStamp = dataMap.GetLong("timeStamp");
                            long totalActiveTimeToday = dataMap.GetLong("totalActiveTimeToday");
                            RegisteredServices.OnIsActiveReceived(isActive, timeStamp, totalActiveTimeToday);
                            break;

                        case "/sleep_data":
                            long sleepStartTime = dataMap.GetLong("sleepStartTime");
                            long sleepEndTime = dataMap.GetLong("sleepEndTime");
                            long totalSleepTimeInADay = dataMap.GetLong("totalSleepTimeInADay");
                            RegisteredServices.OnSleepReceived(sleepStartTime, sleepEndTime, totalSleepTimeInADay);
                            break;

                        case "/gait_data":
                            long SpeedGate = dataMap.GetLong("SpeedGate");
                            timeStamp = dataMap.GetLong("timeStamp");
                            RegisteredServices.OnGaitReceived(SpeedGate, timeStamp);
                            break;

                        case "/tremor_data":
                            long tremorIsOccuring = dataMap.GetLong("tremorIsOccuring");
                            timeStamp = dataMap.GetLong("timeStamp");
                            RegisteredServices.OnTremorReceived(tremorIsOccuring, timeStamp);
                            break;
                    }



                    }
                }
            }
            catch(Exception e) {
                Log.Debug(Tag, $"Stal sa nejaky problem" + e.Message);
            }

        }
    }
}
