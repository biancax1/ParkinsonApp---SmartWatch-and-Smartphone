using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace MonitoringParkinsonism.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}