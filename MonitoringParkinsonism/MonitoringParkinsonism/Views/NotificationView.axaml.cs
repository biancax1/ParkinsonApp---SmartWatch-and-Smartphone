using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace MonitoringParkinsonism.Views;

public partial class NotificationView : UserControl
{
    public NotificationView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}