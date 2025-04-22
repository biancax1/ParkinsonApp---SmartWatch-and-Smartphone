using MonitoringParkinsonism.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism
{
    public static class RegisteredServices
    {
        public static Func<DateTime, MedicineDose, int> ScheduleNotification;
        public static Func<MedicineDose, int> CancelMedicineDoseNotification;
        public static Func<int, int> HandleResume;
        public static Func<int, int> ChangeToNotificationView;
        public static Func<bool, long, long, int> OnIsActiveReceived;
        public static Func<long, long, long, int> OnSleepReceived;
        public static Func<long, long, int> OnGaitReceived;
        public static Func<long, long, int> OnTremorReceived;
        public static Func<int, int> OnStepsReceived;


    }
}
