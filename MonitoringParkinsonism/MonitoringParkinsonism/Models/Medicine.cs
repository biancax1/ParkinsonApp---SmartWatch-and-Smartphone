using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime InitialDate { get; set; }

        public List<MedicineSchedule> schedules = new List<MedicineSchedule>();

        public Medicine(int _id, string _name, DateTime initialDate, List<MedicineSchedule> schedules) {
            this.Id = _id;
            this.Name = _name; 
            this.InitialDate = initialDate; 
            this.schedules = schedules;
        }

    }
}
