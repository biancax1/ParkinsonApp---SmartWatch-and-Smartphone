using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MonitoringParkinsonism.Code;
using MonitoringParkinsonism.Data;
using MonitoringParkinsonism.ViewModels;
using MonitoringParkinsonism.Views;
using SkiaSharp;
using Splat;

namespace MonitoringParkinsonism;

public partial class App : Application
{
    public static AppData AppData { get; set; } = new AppData();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        AppData = DataSerializer.LoadData();
        //SLUZI NA VYMAZANIE, zakomentuj riadok 21
        //AppData = new AppData();
    }

    public virtual void RegisterServices()
    {
    }

    public override void OnFrameworkInitializationCompleted()
    {
        RegisterServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
