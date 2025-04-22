using Avalonia.Controls;
using Avalonia.Interactivity;
using MonitoringParkinsonism.Views;
using MonitoringParkinsonism;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonitoringParkinsonism.Views;

public partial class MainWindow : Window
{
    private Dictionary<string, UserControl> views = new Dictionary<string, UserControl>();

    public MainWindow()
    {
        InitializeComponent();
    }

}
