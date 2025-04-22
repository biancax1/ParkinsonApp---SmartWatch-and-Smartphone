using LiveChartsCore.Defaults;
using MonitoringParkinsonism.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Data
{
    public class AppData
    {
        public int Id_medicine_key { get; set; } = 1;
        public int Id_medicine_schedule_key { get; set; } = 1;
        public int Id_medicine_dose_key { get; set; } = 1;
        public List<Medicine> Medicines { get; set; } = new List<Medicine>();
        public List<MedicineDose> MedicineDoses { get; set; } = new List<MedicineDose>();
        public DateTime LastDayOfCongratulation { get; set; }
        public int StepsInADay { get; set; }
        public long TotalActiveTimeToday { get; set; }
        public ObservableCollection<ObservablePoint> ActivityData { get; set; } = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> TremorData { get; set; } = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> GaitData { get; set; } = new ObservableCollection<ObservablePoint>();
        public List<Sleep> Sleeps { get; set; } = new List<Sleep> { };
    }
}
