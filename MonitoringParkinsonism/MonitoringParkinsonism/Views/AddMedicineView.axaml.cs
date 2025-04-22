using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MonitoringParkinsonism.Models;
using MonitoringParkinsonism.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MonitoringParkinsonism.Views;

public partial class AddMedicineView : UserControl
{

    public AddMedicineView()
    {
        InitializeComponent();
    }

    

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}
