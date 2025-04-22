using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        public MainViewModel mainViewModel { get;set;}
        private int _counter = 0;
        public int Counter { get { return _counter; } set { _counter = value; } }

        public CalendarViewModel(MainViewModel mainViewModel) { 
            this.mainViewModel = mainViewModel;
        }

        public void Button_Calendar_Click(object sender, RoutedEventArgs args)
        {
            Debug.WriteLine("Stlacene kalendar tlacidlo !!!");
        }

        public void AddMedicine()
        {
            mainViewModel.ChangeToAddMedicineView();
        }


    }



}
