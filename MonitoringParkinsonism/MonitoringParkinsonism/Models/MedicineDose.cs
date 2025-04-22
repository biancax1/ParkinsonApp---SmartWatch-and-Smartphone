using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Models
{
    public class MedicineDose
    {
        public int MedicineId { get; set; }
        public int MedicineScheduleId {  get; set; }
        public int MedicineDoseId { get; set; }
        public bool IsTaken { get; set; }
        public string Name { get; set; }
        public DayOfWeek dayOfWeek { get; set; }
        public DateTime time { get; set; }
        public float quantity { get; set; }
        public MedicineUnit medicineUnit { get; set; }

        public MedicineDose(int MedicineId, int MedicineScheduleId, int MedicineDoseId, DayOfWeek dayOfWeek, DateTime time, float quantity, MedicineUnit medicineUnit, string name) { 
        
            this.MedicineId = MedicineId;
            this.MedicineScheduleId = MedicineScheduleId;
            this.MedicineDoseId = MedicineDoseId;
            this.IsTaken = false;
            this.dayOfWeek = dayOfWeek;
            this.time = time;
            this.quantity = quantity;
            this.medicineUnit = medicineUnit;
            this.Name = name;
        }
        public override string ToString()
        {
            return "MedicineDose: " + this.Name + " " + this.quantity + " " + this.medicineUnit + " " + this.time.ToString() + " " + this.dayOfWeek;
        }

    }

}
