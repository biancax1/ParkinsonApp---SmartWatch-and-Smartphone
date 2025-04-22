using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Avalonia.Controls.Notifications;
using MonitoringParkinsonism.Data;
using MonitoringParkinsonism.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Android
{
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Bundle bundle = intent.GetBundleExtra("bundle");
            string medicine_name = bundle.GetString("Medicine");
            Console.WriteLine(medicine_name);
            var notificationManager = NotificationManagerCompat.From(context);
            Intent mainActivityIntent = new Intent(context, typeof(MainActivity));
            mainActivityIntent.AddFlags(ActivityFlags.SingleTop);
            mainActivityIntent.PutExtra("bundle", bundle);
            PendingIntent moveToApplication = PendingIntent.GetActivity(context, 500, mainActivityIntent, PendingIntentFlags.Mutable);
            var notificationBuilder = new NotificationCompat.Builder(context, "medicamentNotification")
                .SetContentTitle("Lieky")
                .SetContentText("Je potrebne zobrat liek s názvom " + medicine_name)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetAutoCancel(true)
                .SetContentIntent(moveToApplication);

            notificationManager.Notify(0, notificationBuilder.Build());
        }

    }
}
