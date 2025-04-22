using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MonitoringParkinsonism.Data;
using MonitoringParkinsonism.Models;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using ReactiveUI;
using System.Xml.Linq;

namespace MonitoringParkinsonism.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Dictionary<string, ViewModelBase> views = new Dictionary<string, ViewModelBase>();
        private ObservableCollection<Medicine> medicines = new ObservableCollection<Medicine>();
        public ObservableCollection<Medicine> Medicines
        {
            get => medicines;
            set => this.RaiseAndSetIfChanged(ref medicines, value);
        }
        public string Greeting => "Welcome to Avalonia!";
        private ViewModelBase _CurrentPage;
        private NotificationViewModel _NotificationViewModel;

        public ViewModelBase CurrentPage
        {
            get { return _CurrentPage; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
        }

        public MainViewModel()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            TestViewModel testViewModel = new TestViewModel(this);
            views.Add("Main", homeViewModel);
            views.Add("Calendar", new CalendarViewModel(this));
            views.Add("Test", testViewModel);
            _NotificationViewModel = new NotificationViewModel(this);
            views.Add("Notification", _NotificationViewModel);
            views.Add("Medicine", new AddMedicineViewModel(this));
            CurrentPage = homeViewModel;
            RegisteredServices.HandleResume = HandleResume;
            RegisteredServices.ChangeToNotificationView = ChangeToNotificationViewfromNotification;
            RegisteredServices.OnIsActiveReceived = testViewModel.OnIsActiveReceived;
            RegisteredServices.OnSleepReceived = testViewModel.OnSleepReceived;
            RegisteredServices.OnGaitReceived = testViewModel.OnGaitReceived;
            RegisteredServices.OnTremorReceived = testViewModel.OnTremorReceived;
            RegisteredServices.OnStepsReceived = testViewModel.OnTotalStepsReceived;


            // JE DOLEZITE ABY TO EXISTOVALO 
            // dovod: RegisteredServices nie je hotovy v konstruktore
            Task.Factory.StartNew(() => Thread.Sleep(3000))
                   .ContinueWith((t) =>
                   {
                       PlanAllMedicineDosesAndNotifications();
                   }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void ChangeToHomeView()
        {
            CurrentPage = views["Main"];
        }

        public void ChangeToCalendarView()
        {
            CurrentPage = views["Calendar"];
        }

        public void ChangeToTestView()
        {
            //RegisteredServices.ScheduleNotification(DateTime.Now.AddSeconds(5), new Medicine(1, "name_1", DateTime.Now, null));
            //RegisteredServices.ScheduleNotification(DateTime.Now.AddSeconds(9), new Medicine(2, "name_2", DateTime.Now, null));
            CurrentPage = views["Test"];

        }

        public void ChangeToNotificationView()
        {
            CurrentPage = views["Notification"];
            _NotificationViewModel.RefreshDoses();
        }

        public int ChangeToNotificationViewfromNotification(int medicineID)
        {
            CurrentPage = views["Notification"];
            _NotificationViewModel.RefreshDoses();
            return 0;
        }

        public void ChangeToAddMedicineView()
        {
            CurrentPage = views["Medicine"];
        }

        public void PlanAllMedicineDosesAndNotifications()
        {
            DateTime now = DateTime.Now;
            DateTime to = DateTime.Now.AddDays(7);
            DateTime from = new DateTime();
            Debug.WriteLine("PlanNotifications");

            foreach (MedicineDose medicineDose in App.AppData.MedicineDoses)
            {
                if (medicineDose.time > from)
                {
                    from = medicineDose.time;
                }
            }

            foreach (Medicine medicine in App.AppData.Medicines)
            {
                Debug.WriteLine("Check plan for " + medicine.Name + "   ID: " + medicine.Id);
                PlanMedicineDoseAndNotifications(from, to, medicine);
            }
        }

        public void PlanMedicineDoseAndNotifications(DateTime from, DateTime to, Medicine medicine)
        {

            Debug.WriteLine("Planning " + medicine.Name);
            PlanMedicineDosesAndNotificationsFromToDates(from, to, medicine);

        }

        public void PlanMedicineDosesAndNotificationsFromToDates(DateTime from, DateTime to, Medicine medicine)
        {
            foreach (MedicineSchedule schedule in medicine.schedules)
            {
                DateTime dateTime = new DateTime(from.Year, from.Month, from.Day, schedule.time.Hours, schedule.time.Minutes, 0);

                int nextday = (int)schedule.dayOfWeek - (int)DateTime.Now.DayOfWeek;

                if (nextday < 0)
                {
                    nextday = 7 - (int)DateTime.Now.DayOfWeek;
                    nextday += (int)schedule.dayOfWeek;
                }

                Debug.WriteLine("NextDay " + nextday);

                DateTime newDateTime = dateTime.AddDays(nextday);

                if (newDateTime > to || newDateTime < medicine.InitialDate || newDateTime <= from)
                {
                    continue;
                }

                Debug.WriteLine("dateTime " + newDateTime);

                MedicineDose medicineDose = new(medicine.Id, schedule.Id, App.AppData.Id_medicine_dose_key++, schedule.dayOfWeek, newDateTime, schedule.quantity, schedule.medicineUnit, medicine.Name);
                RegisteredServices.ScheduleNotification(newDateTime, medicineDose);

                App.AppData.MedicineDoses.Add(medicineDose);
            }
            try
            {
                DataSerializer.SaveData(App.AppData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void ScheduleDosesOnRepeat()
        {

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

        public void ThankYouMessage()
        {
            if (App.AppData.MedicineDoses.Count == 0) {
                return;
            }
            if (App.AppData.LastDayOfCongratulation <= DateTime.Now.AddDays(-7))
            { 
                bool atLeastOneMedicineHasBeenTaken = false;

                foreach (MedicineDose medicineDose in App.AppData.MedicineDoses)
                {
                    if (!medicineDose.IsTaken && medicineDose.time >= DateTime.Now.AddDays(-7))
                    {
                        return;
                    }
                    if (medicineDose.IsTaken && medicineDose.time >= DateTime.Now.AddDays(-7)) 
                    { 
                        atLeastOneMedicineHasBeenTaken = true;
                    }
                }

                if (!atLeastOneMedicineHasBeenTaken) {
                    App.AppData.LastDayOfCongratulation = DateTime.Now;
                    try
                    {
                        DataSerializer.SaveData(App.AppData);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    ShowMessageBox("Poďakovanie", "Ďakujeme, že ste za posledný týždeň nezabudni zobrať nijaký liek");
                }
            }
        }

        public int HandleResume(int hocico)
        {
            Task.Factory.StartNew(() => Thread.Sleep(500))
                   .ContinueWith((t) =>
                   {
                       ThankYouMessage();
                   }, TaskScheduler.FromCurrentSynchronizationContext());

            return 0;
        }

        }

}