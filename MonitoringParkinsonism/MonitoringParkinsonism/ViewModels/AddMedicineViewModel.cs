using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia.Controls;
using DynamicData;
using MonitoringParkinsonism.Data;
using MonitoringParkinsonism.Models;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using ReactiveUI;
using System.Linq;

namespace MonitoringParkinsonism.ViewModels
{
    public class AddMedicineViewModel : ViewModelBase
    {
        public string? name { get; set; } = "";
        public DateTimeOffset? initialDate { get; set; } = DateTime.Now;
        public DayOfWeek? dayOfWeek = DayOfWeek.Monday;
        public DateTimeOffset? dayOfWeekDateTime { get; set; } = DateTime.Now;
        private TimeSpan? _time = DateTime.Now.TimeOfDay;
        public TimeSpan? Time
        {
            get => _time;
            set
            {
                _time = new TimeSpan(value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                this.RaiseAndSetIfChanged(ref _time, value);
            }
        }

       /* private double _zoomFactor = 1.0;
        public double ZoomFactor
        {
            get => _zoomFactor;
            set => this.RaiseAndSetIfChanged(ref _zoomFactor, value);
        }*/

        public  float? quantity { get; set; } = 1;
        public int? unit { get; set; }  = 0;

        public  ObservableCollection<MedicineSchedule> Schedules { get => _schedules  ; set => this.RaiseAndSetIfChanged(ref _schedules, value); }
        private ObservableCollection<MedicineSchedule> _schedules = new ObservableCollection<MedicineSchedule>();

        public ICommand AddMedicineAndGoBackCommand { get; }

        private MainViewModel mainViewModel;

        public AddMedicineViewModel(MainViewModel mainViewModel) { 
            this.mainViewModel = mainViewModel;
            AddMedicineAndGoBackCommand = ReactiveCommand.Create(AddMedicineAndGoBack);
            AddScheduleToMedicineCommand = ReactiveCommand.Create(AddScheduleToMedicine);
        }

        private async void ShowMessageBox(string title, string message)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(
            new MessageBoxCustomParams
            {
                ButtonDefinitions = new List<ButtonDefinition>
                {
                            new ButtonDefinition { Name = "OK", },
                },
                ContentTitle = title,
                ContentMessage = message,
                Icon = MsBox.Avalonia.Enums.Icon.Question,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                MaxWidth = 500,
                MaxHeight = 800,
                SizeToContent = SizeToContent.WidthAndHeight,
                ShowInCenter = true,
                CloseOnClickAway = true,
                Topmost = true,
                SystemDecorations = SystemDecorations.Full
            });

            string result = await box.ShowAsync();
        }

        public void AddMedicineAndGoBack()
        {
            ValidateMedicineName();

            dayOfWeek = dayOfWeekDateTime?.DayOfWeek;

            Medicine medicine = new Medicine(App.AppData.Id_medicine_key++, name, initialDate.Value.DateTime, new List<MedicineSchedule>(Schedules));

            App.AppData.Medicines.Add(medicine);
            try
            {
                DataSerializer.SaveData(App.AppData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            Debug.WriteLine(medicine.ToString());
            ClearForm();
            mainViewModel.ChangeToCalendarView();
            mainViewModel.PlanMedicineDoseAndNotifications(initialDate.Value.DateTime, DateTime.Now.AddDays(7), medicine);
        }

        public void ValidateMedicineName()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowMessageBox("neplatný názov lieku", "message Názov lieku je prázdny");
                return;
            }
            else if (name.Length < 3)
            {
                ShowMessageBox("neplatný názov lieku", "Názov lieku musí mať aspoň 3 znaky");
                return;
            }
            else if (name.Length > 50)
            {
                ShowMessageBox("neplatný názov lieku", "Názov lieku nesmie mať viac ako 50 znakov");
                return;
            }
            else if (!name.All(char.IsLetterOrDigit))
            {
                ShowMessageBox("neplatný názov lieku", "Názov lieku môže obsahovať iba písmená abecedy a čísla");
                return;
            }
            else if (char.IsDigit(name[0]))
            {
                ShowMessageBox("neplatný názov lieku", "Názov lieku nesmie začínať číslicou");
                return;
            }
        }

        public void ClearForm()
        {
            Schedules.Clear();
            dayOfWeek = DayOfWeek.Monday;
            name = "";
            initialDate = DateTime.Now;
            dayOfWeekDateTime = initialDate;
            quantity = 1;
            unit = 0;
        }

        public void AddScheduleToMedicine() {

            dayOfWeek = dayOfWeekDateTime?.DayOfWeek;
            MedicineSchedule schedule = new MedicineSchedule(App.AppData.Id_medicine_schedule_key++, dayOfWeek.Value, _time.Value, quantity.Value, (MedicineUnit)unit);
            Schedules.Add(schedule);
        }

        public ICommand AddScheduleToMedicineCommand { get; }

    }


}