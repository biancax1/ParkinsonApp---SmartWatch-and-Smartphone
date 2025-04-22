using Avalonia.Controls;

using Avalonia.Controls;
using Avalonia.Interactivity;
using MonitoringParkinsonism.Views;
using MonitoringParkinsonism;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Markup.Xaml;
using MonitoringParkinsonism.ViewModels;

namespace MonitoringParkinsonism.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}