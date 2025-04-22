using Android.App;
using Android.Content.PM;
using Android;

using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Android.OS;
using Android.Content;
using AndroidX.Core.App;
using System;
using MonitoringParkinsonism.Models;
using Org.Apache.Http.Entity;
using MonitoringParkinsonism.Data;
using MonitoringParkinsonism.Code;
using Splat;
using Android.Util;
using Android.Gms.Wearable;

namespace MonitoringParkinsonism.Android;

[Activity(
    Label = "MonitoringParkinsonism.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode,
    LaunchMode = LaunchMode.SingleTop
    )
    ]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        if (Build.VERSION.SdkInt >= (BuildVersionCodes)13) { 
            RequestPermissions(new string[] { Manifest.Permission.PostNotifications }, 0);
            CreateNotificationChannel();
        }

        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Locator.CurrentMutable.RegisterLazySingleton<IAccelerometerDataService>(
() => new AccelerometerDataService());
        Log.Info("MainActivity", "AccelerometerDataService registered.");

        ScheduleNotification(DateTime.Now.AddSeconds(3), new MedicineDose(-1,-1,-1, DayOfWeek.Monday, new DateTime(), -1, MedicineUnit.Miligram, "test"));
        RegisteredServices.ScheduleNotification = ScheduleNotification;
        RegisteredServices.CancelMedicineDoseNotification = CancelMedicineDoseNotification;


        //ODTIAL
        /*
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer Sleeptimer = new System.Timers.Timer();
        timer.Interval = 1000;
        Sleeptimer.Interval = 10000;

        Sleeptimer.Elapsed += (sender, e) =>
        {
            Random rand = new Random();
            Log.Info("SleepTimer", "SleepTimer elapsed");
            long sleepStartTime = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
            long sleepEndTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long totalSleepTimeInADay = rand.Next(14000, 15000);
            RegisteredServices.OnSleepReceived(sleepStartTime, sleepEndTime, totalSleepTimeInADay);
        };
        Sleeptimer.Start();

        timer.Elapsed += (sender, e) =>
        {
            Random rand = new Random();
            Log.Info("Timer", "Timer elapsed");
                    int totalStepsInDay = rand.Next(6000, 7000);
                    RegisteredServices.OnStepsReceived(totalStepsInDay);
                    bool isActive = rand.Next(2) == 0;
                    long timeStamp2 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    long totalActiveTimeToday = rand.Next(10000, 20000);
                    RegisteredServices.OnIsActiveReceived(isActive, timeStamp2, totalActiveTimeToday);
                
                    long SpeedGate = rand.Next(0, 20000);
                    long timeStamp3 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    RegisteredServices.OnGaitReceived(SpeedGate, timeStamp3);

                    long tremorIsOccuring = rand.Next(2);
                    long timeStamp4 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    RegisteredServices.OnTremorReceived(tremorIsOccuring, timeStamp4); 
        };
        timer.Start();
        */
        //POTIAL
    }

    protected override void OnResume()
    {
        base.OnResume();
        if (RegisteredServices.HandleResume != null)
        {
            RegisteredServices.HandleResume(0);
        }
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        if (intent.HasExtra("bundle"))
        {
            Bundle bundle = intent.GetBundleExtra("bundle");
            string medicine_name = bundle.GetString("Medicine");
            RegisteredServices.ChangeToNotificationView(0);

        }
    }


    private void CreateNotificationChannel()
    {
        var channel = new NotificationChannel(
             "medicamentNotification",
             "Lieky",
             NotificationImportance.High)
        {
            Description = "Notifikácia na branie liekov"
        };  

        var notificationManager = (NotificationManager)GetSystemService("notification");
        notificationManager.CreateNotificationChannel(channel);
    }

    private int ScheduleNotification(DateTime dateTime, MedicineDose dose)
    {
        var intent = new Intent(this, typeof(AlarmReceiver));
        Bundle bundle = new Bundle();
        bundle.PutString("Medicine", dose.Name);
        intent.PutExtra("bundle", bundle);
        intent.AddFlags(ActivityFlags.SingleTop);
        var pendingIntent = PendingIntent.GetBroadcast(this, dose.MedicineDoseId, intent, PendingIntentFlags.Mutable);
        AlarmManager alarmManager = (AlarmManager)GetSystemService("alarm");
        long miliseconds = (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

        alarmManager.Set(AlarmType.RtcWakeup, miliseconds ,pendingIntent);

        return 0;
    }

    private int CancelMedicineDoseNotification(MedicineDose dose)
    {
        var intent = new Intent(this, typeof(AlarmReceiver));
        intent.AddFlags(ActivityFlags.SingleTop);
        var pendingIntent = PendingIntent.GetBroadcast(this, dose.MedicineDoseId, intent, PendingIntentFlags.Mutable);
        AlarmManager alarmManager = (AlarmManager)GetSystemService("alarm");

        alarmManager.Cancel(pendingIntent);
        return 0;
    }

}
