using MonitoringParkinsonism.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace MonitoringParkinsonism.Data
{
    public static class DataSerializer
    {
        public static void SaveData(AppData appData)
        {
            Debug.WriteLine("*** : " + AppDomain.CurrentDomain.BaseDirectory);
            string output = JsonConvert.SerializeObject(appData);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory +"/output2.txt", output);
        }

        public static AppData LoadData()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory+"/output2.txt"))
            {
                string input = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory+"/output2.txt");
                return JsonConvert.DeserializeObject<AppData>(input);
            }
            return new AppData();
        }

        public static string MedicineToString(Medicine medicine)
        {
            string output = JsonConvert.SerializeObject(medicine);
            return output;
        }

        public static Medicine StringToMedicine(string medicine)
        {
            return JsonConvert.DeserializeObject<Medicine>(medicine);
        }
    }
}
