using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MonitoringParkinsonism.Models;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using System.Runtime.InteropServices;
using MonitoringParkinsonism.Data;
using Avalonia.Media;
using DynamicData.Kernel;

namespace MonitoringParkinsonism.Views;

public partial class CalendarView : UserControl
{
    public CalendarView()
    {
        this.Initialized += CalendarView_Initialized;
        this.Loaded += CalendarView_Loaded;
        InitializeComponent();
    }

    private void CalendarView_Loaded(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("CalendarView_Loaded");

        /*
        List<MedicineSchedule> medicineSchedules = new List<MedicineSchedule>();
        medicineSchedules.Add(new MedicineSchedule(DayOfWeek.Friday, DateTime.Now.TimeOfDay, 10, MedicineUnit.NumberOfSuppositiories));

        Medicine medicine = new Medicine("Gin", DateTime.Now, medicineSchedules);

        App.AppData.Medicines.Add(medicine);*/

        Debug.WriteLine("CalendarView_Initialized");
        ShowMedicines();
    }

    private void CalendarView_Initialized(object? sender, System.EventArgs e)
    {
        Debug.WriteLine("CalendarView_Initialized");

    }

    private void ShowMedicines()
    {
        foreach (Medicine medicine in App.AppData.Medicines)
        {
            try
            {
                Debug.WriteLine("ShowMedicines : " + medicine.Name);

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
                stackPanel.Margin = new Thickness(0, 10, 0, 0);
                stackPanel.Background = new SolidColorBrush(Color.Parse("#EBEDFF"));

                
                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = new SolidColorBrush(Color.Parse("#7F74B3")); 
                border.Background = new SolidColorBrush(Color.Parse("#EBEDFF")); 
                border.CornerRadius = new CornerRadius(10); 
                border.Padding = new Thickness(10); 
                border.Margin = new Thickness(0, 30, 0, 0);

                // odkial ked tak zmazem
                TextBlock textBlock = new TextBlock();
                textBlock.FontWeight = Avalonia.Media.FontWeight.Bold;
                textBlock.FontSize = 20; 
                textBlock.Text = medicine.Name;

                stackPanel.Children.Add(textBlock);
                //potial zmazem

                /* a toto pridam:
                 
           
            StackPanel nameStackPanel = new StackPanel();
            nameStackPanel.Orientation = Avalonia.Layout.Orientation.Horizontal;
            nameStackPanel.Spacing = 10; 

            
            TextBlock pillIcon = new TextBlock();
            pillIcon.Text = "💊"; 
            pillIcon.FontSize = 16; 

            
            TextBlock textBlock = new TextBlock();
            textBlock.FontWeight = Avalonia.Media.FontWeight.Bold;
            textBlock.Text = medicine.Name;

           
            nameStackPanel.Children.Add(pillIcon);
            nameStackPanel.Children.Add(textBlock);

           
            stackPanel.Children.Add(nameStackPanel);
                
             */


                foreach (MedicineSchedule medicineSchedule in medicine.schedules) {
                    StackPanel horizontalStackPanel = new StackPanel();
                    horizontalStackPanel.Orientation = Avalonia.Layout.Orientation.Horizontal;

                    TextBlock textBlock_d = new TextBlock();
                    textBlock_d.Text = DateTimeFormatInfo.CurrentInfo.GetDayName(medicineSchedule.dayOfWeek);
                    textBlock_d.FontSize = 18; 

                    TextBlock textBlock_t = new TextBlock();
                    textBlock_t.Text = string.Format("{0:00}:{1:00}", medicineSchedule.time.Hours, medicineSchedule.time.Minutes);
                    textBlock_t.FontSize = 18;

                    TextBlock textBlock_c = new TextBlock();
                    textBlock_c.Text = string.Format("{0} {1}", medicineSchedule.quantity, MedicineUnitFunctions.GetTranslatedString(medicineSchedule.medicineUnit));
                    textBlock_c.FontSize = 18;

                    horizontalStackPanel.Children.Add(textBlock_d);
                    horizontalStackPanel.Children.Add(textBlock_t);
                    horizontalStackPanel.Children.Add(textBlock_c);
                    stackPanel.Children.Add(horizontalStackPanel);
                }

                Button button = new Button();
                button.Content = "Zmazať";
                button.Tag = medicine;
                button.Classes.Add("LiekButton");
                button.Click += ButtonDeleteDrug_Click;

                stackPanel.Children.Add(button);
                border.Child = stackPanel;

                StackPanel lieky = this.FindControl<StackPanel>("stackpanel_lieky");
                lieky.Children.Add(border);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }

        }
            
    }

    private void ButtonDeleteDrug_Click(object? sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        DeleteMedicine(button.Tag as Medicine);
    }

    private async void DeleteMedicine(Medicine medicine)
    {

        var box = MessageBoxManager.GetMessageBoxCustom(
            new MessageBoxCustomParams
            {
                ButtonDefinitions = new List<ButtonDefinition>
                {
                    new ButtonDefinition { Name = "Áno", },
                    new ButtonDefinition { Name = "Nie", },
                },
                ContentTitle = "Zmazanie lieku",
                ContentMessage = "Ste si istí, že chcete liek zmazať zo zoznamu braných liekov?",
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

        foreach (MedicineDose medicineDose in App.AppData.MedicineDoses)
        {
            Debug.WriteLine(medicineDose.Name);
            Debug.WriteLine(medicineDose.time == null ? "" : medicineDose.time.ToString());
        }

        if (result == "Áno")
        {
            App.AppData.Medicines.Remove(medicine);

            List<MedicineDose> newMedicineDoseList = new List<MedicineDose>();
            DateTime dateTimeNow = DateTime.Now;
            
            for (int i = 0; i<App.AppData.MedicineDoses.Count; i++) {
                if (App.AppData.MedicineDoses[i].MedicineId == medicine.Id && App.AppData.MedicineDoses[i].time > dateTimeNow) {
                RegisteredServices.CancelMedicineDoseNotification(App.AppData.MedicineDoses[i]);
                } else {
                        newMedicineDoseList.Add(App.AppData.MedicineDoses[i]);
                 }
            }

            Debug.WriteLine("new List");
            App.AppData.MedicineDoses = newMedicineDoseList;
            foreach (MedicineDose medicineDose in App.AppData.MedicineDoses)
            {
                Debug.WriteLine(medicineDose.Name);
                Debug.WriteLine(medicineDose.time == null ? "" : medicineDose.time.ToString());
            }
            DataSerializer.SaveData(App.AppData);

        }

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}