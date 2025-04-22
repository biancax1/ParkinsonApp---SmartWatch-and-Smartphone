using MonitoringParkinsonism.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {

        public void PrintAllDoses()
        {
            for (int i = 0; i < App.AppData.MedicineDoses.Count; i++)
            {
                MedicineDose dose = App.AppData.MedicineDoses[i];
                Console.WriteLine("Medicine dose: " + dose.ToString());
            }
        }
    }



}
