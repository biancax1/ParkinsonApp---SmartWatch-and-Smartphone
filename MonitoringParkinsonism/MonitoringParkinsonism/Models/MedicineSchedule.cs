using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Models
{
    public class MedicineSchedule
    {
        public int Id { get; set; }
        public DayOfWeek dayOfWeek { get; set; }
        public TimeSpan time { get; set; }
        public float quantity { get; set; }
        public MedicineUnit medicineUnit { get; set; }

        public MedicineSchedule(int id, DayOfWeek dayOfWeek, TimeSpan time, float quantity, MedicineUnit medicineUnit)
        {
            this.Id = id;
            this.dayOfWeek = dayOfWeek;
            this.time = time;
            this.quantity = quantity;
            this.medicineUnit = medicineUnit;
        }
    }
}
