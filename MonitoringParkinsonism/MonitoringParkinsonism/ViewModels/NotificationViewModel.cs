using System;
using System.ComponentModel;
using System.Windows.Input;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using ReactiveUI;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;
using MonitoringParkinsonism.Models;
using System.Collections.ObjectModel;
using MonitoringParkinsonism;
using MonitoringParkinsonism.Data;
using System.Collections.Generic;

namespace MonitoringParkinsonism.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;
        public ObservableCollection<MedicineDose> Doses { get => _doses; set => this.RaiseAndSetIfChanged(ref _doses, value); }
        private ObservableCollection<MedicineDose> _doses = new ObservableCollection<MedicineDose>();
        public ObservableCollection<MedicineDose> AllDoses { get => _allDoses; set => this.RaiseAndSetIfChanged(ref _allDoses, value); }
        private ObservableCollection<MedicineDose> _allDoses = new ObservableCollection<MedicineDose>();


        public NotificationViewModel(MainViewModel mainViewModel)
        {
            this._mainViewModel = mainViewModel;
            TakeMedicineDoseCommand = ReactiveCommand.Create<MedicineDose>(TakeMedicineDose); 
        }

        public ICommand TakeMedicineDoseCommand { get; }

        public void RefreshDoses()
        {
            Doses = new ObservableCollection<MedicineDose>(App.AppData.MedicineDoses);
            FilterAndApplyMedicineDoses();
        }

        public void RefreshAllDoses()
        {
            AllDoses = new ObservableCollection<MedicineDose>(App.AppData.MedicineDoses);
            ShowMedicineDosesThatAreNotTaken();
        }


        public void TakeMedicineDose(MedicineDose dose)
        {
            if (dose != null){
                dose.IsTaken = true;
            }

            FilterAndApplyMedicineDoses();
        }

        public void FilterAndApplyMedicineDoses()
        {
            List<MedicineDose> updatedDoses = new List<MedicineDose>();

            DateTime dateTimeNow = DateTime.Now;
            DateTime datetimeOneHourAgo = DateTime.Now.AddMinutes(-60);

            foreach (MedicineDose medicineDose in Doses)
            {
                if (!medicineDose.IsTaken && medicineDose.time > datetimeOneHourAgo && medicineDose.time<dateTimeNow)
                {
                    updatedDoses.Add(medicineDose);
                }
            }

            Doses = new ObservableCollection<MedicineDose>(updatedDoses);
        }


        public void ShowMedicineDosesThatAreNotTaken()
        {
            List<MedicineDose> updatedDoses = new List<MedicineDose>();
            DateTime dateTimeNow = DateTime.Now;

            foreach (MedicineDose medicineDose in AllDoses)
            {
                if (!medicineDose.IsTaken && medicineDose.time < dateTimeNow)
                {
                    updatedDoses.Add(medicineDose);
                }
            }

            AllDoses = new ObservableCollection<MedicineDose>(updatedDoses);
        }

    }
}