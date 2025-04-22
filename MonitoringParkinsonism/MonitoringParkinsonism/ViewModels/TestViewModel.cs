using System;
using System.ComponentModel;
using System.Windows.Input;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using ReactiveUI;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;
using Avalonia.Threading;
using MonitoringParkinsonism.Code;
using Splat;
using System.Diagnostics;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using MonitoringParkinsonism.Data;
using MonitoringParkinsonism.Models;
using Avalonia.Input.TextInput;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

namespace MonitoringParkinsonism.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        private readonly IAccelerometerDataService _accelerometerDataService;
        private ObservableCollection<ObservablePoint> _activityPoints;
        public ObservableCollection<ObservablePoint> ActivityPoints
        {
            get { return _activityPoints; }
            set => this.RaiseAndSetIfChanged(ref _activityPoints, value);
        }
        private ObservableCollection<ObservablePoint> _tremorPoints;
        public ObservableCollection<ObservablePoint> TremorPoints
        {
            get { return _tremorPoints; }
            set => this.RaiseAndSetIfChanged(ref _tremorPoints, value);
        }
        private ObservableCollection<ObservablePoint> _gaitPoints;
        public ObservableCollection<ObservablePoint> GaitPoints
        {
            get { return _gaitPoints; }
            set => this.RaiseAndSetIfChanged(ref _gaitPoints, value);
        }
        private ObservableCollection<Sleep> _sleeps;
        public ObservableCollection<Sleep> Sleeps
        {
            get { return _sleeps; }
            set => this.RaiseAndSetIfChanged(ref _sleeps, value);
        }

        private Axis[] _activitySeriesXAxis;

        public Axis[] ActivitySeriesXAxis
        {
            get { return _activitySeriesXAxis; }
            set => this.RaiseAndSetIfChanged(ref _activitySeriesXAxis, value);
        }

        private Axis[] _tremorSeriesXAxis;

        public Axis[] TremorSeriesXAxis
        {
            get { return _tremorSeriesXAxis; }
            set => this.RaiseAndSetIfChanged(ref _tremorSeriesXAxis, value);
        }

        private Axis[] _gaitSeriesXAxis;

        public Axis[] GaitSeriesXAxis
        {
            get { return _gaitSeriesXAxis; }
            set => this.RaiseAndSetIfChanged(ref _gaitSeriesXAxis, value);
        }

        private int _stepsInADay;
        public int StepsInADay
        {
            get { return _stepsInADay; }
            set => this.RaiseAndSetIfChanged(ref _stepsInADay, value);
        }

        private long _totalActiveTimeToday;
        public long TotalActiveTimeToday
        {
            get { return _totalActiveTimeToday; }
            set
            {
                this.RaiseAndSetIfChanged(ref _totalActiveTimeToday, value);
                this.RaisePropertyChanged(nameof(TotalActiveTimeFormatted));
            }
        }

        public string TotalActiveTimeFormatted
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(TotalActiveTimeToday);
                return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }

        private long _totalSleepTimeToday;
        public long TotalSleepTimeToday
        {
            get { return _totalSleepTimeToday; }
            set
            {
                this.RaiseAndSetIfChanged(ref _totalSleepTimeToday, value);
                this.RaisePropertyChanged(nameof(TotalSleepTimeFormatted));
            }
        }

        public string TotalSleepTimeFormatted
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(TotalSleepTimeToday);
                return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }

        enum GraphRange
        {
            day,
            week,
            month
        }

        private GraphRange _graphRange;

        private Object Sleeplock = new Object();

        /* private float _x;

         public float X
         {
             get => _x;
             set => this.RaiseAndSetIfChanged(ref _x, value);
         }

         private float _y;
         public float Y
         {
             get => _y;
             set => this.RaiseAndSetIfChanged(ref _y, value);
         }

         private float _z;
         public float Z
         {
             get => _z;
             set => this.RaiseAndSetIfChanged(ref _z, value);
         }*/

        public TestViewModel(MainViewModel mainViewModel)
        {

            ActivityPoints = new ObservableCollection<ObservablePoint>();
            TremorPoints = new ObservableCollection<ObservablePoint>();
            GaitPoints = new ObservableCollection<ObservablePoint>();
            Sleeps = new ObservableCollection<Sleep>();
            Sleeps = new ObservableCollection<Sleep>(App.AppData.Sleeps.OrderBy(sleep => -sleep.EndTime.Ticks));


            Debug.WriteLine("***** TestViewModel *****");
            _accelerometerDataService = Locator.Current.GetService<IAccelerometerDataService>();

            if (_accelerometerDataService != null)
            {
                _accelerometerDataService.AccelerometerDataReceived += OnAccelerometerDataReceived;
                Debug.WriteLine("AccelerometerDataService retrieved successfully.");
            }
            else
            {
                Debug.WriteLine("AccelerometerDataService is null.");
            }

            this._mainViewModel = mainViewModel;

            for (int i = 0; i < App.AppData.ActivityData.Count; i++)
            {
                ObservablePoint loadedActivityPoint = App.AppData.ActivityData[i];

                ActivityPoints.Add(new ObservablePoint(loadedActivityPoint.X, loadedActivityPoint.Y));
            }

            /*ActivityPoints.Add(new ObservablePoint(DateTime.Now.Ticks, 5));
            ActivityPoints.Add(new ObservablePoint(DateTime.Now.AddHours(-1).Ticks, 2));
            ActivityPoints.Add(new ObservablePoint(DateTime.Now.AddHours(-2).Ticks, 7));
            ActivityPoints.Add(new ObservablePoint(DateTime.Now.AddHours(-7).Ticks, 1));
            ActivityPoints.Add(new ObservablePoint(DateTime.Now.AddDays(-3).Ticks, 4));
            ActivityPoints.Add(new ObservablePoint(DateTime.Now.AddDays(-12).Ticks, 5));*/
            for (int i = 0; i < App.AppData.TremorData.Count; i++)
            {
                ObservablePoint loadedTremorPoint = App.AppData.TremorData[i];

                TremorPoints.Add(new ObservablePoint(loadedTremorPoint.X, loadedTremorPoint.Y));
            }
            /*TremorPoints.Add(new ObservablePoint(DateTime.Now.Ticks, 5));
            TremorPoints.Add(new ObservablePoint(DateTime.Now.AddHours(-1).Ticks, 3));
            TremorPoints.Add(new ObservablePoint(DateTime.Now.AddHours(-2).Ticks, 1));
            TremorPoints.Add(new ObservablePoint(DateTime.Now.AddHours(-7).Ticks, 8));
            TremorPoints.Add(new ObservablePoint(DateTime.Now.AddDays(-3).Ticks, 2));
            TremorPoints.Add(new ObservablePoint(DateTime.Now.AddDays(-12).Ticks, 3));*/

            StepsInADay = App.AppData.StepsInADay;
            TotalActiveTimeToday = App.AppData.TotalActiveTimeToday;


            ChangeRangeToDay();
            ActivitySeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = ActivityPoints,
                    Stroke = new SolidColorPaint(new SKColor(127, 116, 179)) { StrokeThickness = 4 },
                    Fill = new SolidColorPaint(new SKColor(127, 116, 179).WithAlpha(90)),
                    GeometryFill = new SolidColorPaint(new SKColor(127, 116, 179)),
                    GeometryStroke = new SolidColorPaint(new SKColor(127, 116, 179)),
                    GeometrySize = 10,
                }
            };

            TremorSeries = new ISeries[]
            {
                new StepLineSeries<ObservablePoint>
                {
                    Values = TremorPoints,
                    Stroke = new SolidColorPaint(new SKColor(127, 116, 179)) { StrokeThickness = 4 },
                    Fill = new SolidColorPaint(new SKColor(127, 116, 179).WithAlpha(90)),
                    GeometryFill = new SolidColorPaint(new SKColor(127, 116, 179)),
                    GeometryStroke = new SolidColorPaint(new SKColor(127, 116, 179)),
                    GeometrySize = 10,
                }
            };

            GaitSeries = new ISeries[]
            {
                new LineSeries<ObservablePoint>
                {
                    Values = GaitPoints,
                    Stroke = new SolidColorPaint(new SKColor(127, 116, 179)) { StrokeThickness = 4 },
                    Fill = new SolidColorPaint(new SKColor(127, 116, 179).WithAlpha(90)),
                    GeometryFill = new SolidColorPaint(new SKColor(127, 116, 179)),
                    GeometryStroke = new SolidColorPaint(new SKColor(127, 116, 179)),
                    GeometrySize = 10,
                }
            };

            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.AutoReset = true;
            timer.Elapsed += RefreshGraphs;
            timer.Enabled = true;
        }

        private void RefreshGraphs(object? sender, ElapsedEventArgs e)
        {
            switch (_graphRange)
            {
                case GraphRange.day:
                    ChangeRangeToDay();
                    break;
                case GraphRange.week:
                    ChangeRangeToWeek();
                    break;
                case GraphRange.month:
                    ChangeRangeToMonth();
                    break;
                default:
                    break;
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (ActivityPoints)
                {
                    lock (TremorPoints)
                    {
                        lock (GaitPoints)
                        {
                            lock (Sleeplock)
                            {
                                try
                                {
                                    DataSerializer.SaveData(App.AppData);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex);
                                }
                            }
                        }
                    }
                }
            });
        }

        private DateTime UnixToCsharpTimeConverter(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;

        }

        private void OnAccelerometerDataReceived(object sender, AccelerometerDataEventArgs e)
        {
            //e.timeStamp
            long currentTime = UnixToCsharpTimeConverter(e.timeStamp).Ticks;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (TremorPoints)
                {
                    App.AppData.TremorData.Add(new ObservablePoint(currentTime, e.X));
                    TremorPoints.Add(new ObservablePoint(currentTime, e.X));
                }
            });
            /*Dispatcher.UIThread.Post(() =>
            {
                X = e.X;
                Y = e.Y;
                Z = e.Z;

                Debug.WriteLine("X: " + X + "   Y: " + Y + "   Z: " + Z);
            });*/
        }

        public int OnIsActiveReceived(bool isActive, long timeStamp, long totalActiveTimeToday)

        // totalActiveTimeToday vypis jedneho cisla
        {
            long currentTime = UnixToCsharpTimeConverter(timeStamp).Ticks;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (ActivityPoints)
                {
                    App.AppData.ActivityData.Add(new ObservablePoint(currentTime, isActive ? 1 : 0));
                    ActivityPoints.Add(new ObservablePoint(currentTime, isActive ? 1 : 0));
                    App.AppData.TotalActiveTimeToday = totalActiveTimeToday;
                    TotalActiveTimeToday = totalActiveTimeToday;
                }
            });
            return 0;
        }

        public int OnSleepReceived(long SleepStartTime, long SleepEndTime, long TotalSleepTimeInADay)
        {
            Sleep newSleep = new Sleep(SleepStartTime, SleepEndTime, TotalSleepTimeInADay);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (Sleeplock)
                {
                    App.AppData.Sleeps.Add(newSleep);
                    Sleeps.Insert(0, newSleep);

                    TotalSleepTimeToday = TotalSleepTimeInADay;
                }
            });
            return 0;
        }

        public int OnGaitReceived(long SpeedGate, long timeStamp)
        // ciselny graf
        {
            long currentTime = UnixToCsharpTimeConverter(timeStamp).Ticks;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (GaitPoints)
                {
                    App.AppData.GaitData.Add(new ObservablePoint(currentTime, SpeedGate));
                    GaitPoints.Add(new ObservablePoint(currentTime, SpeedGate));
                }
            });
            return 0;
        }

        public int OnTremorReceived(long tremorIsOccuring, long timeStamp)
        // ciselny graf
        {
            long currentTime = UnixToCsharpTimeConverter(timeStamp).Ticks;
            Dispatcher.UIThread.InvokeAsync(() =>
            {

                lock (TremorPoints)
                {
                    App.AppData.TremorData.Add(new ObservablePoint(currentTime, tremorIsOccuring));
                    TremorPoints.Add(new ObservablePoint(currentTime, tremorIsOccuring));
                }
            });
            return 0;
        }

        public int OnTotalStepsReceived(int totalSteps)
        {
            App.AppData.StepsInADay = totalSteps;
            StepsInADay = totalSteps;

            return 0;
        }

        private int _counter;
        private MainViewModel _mainViewModel;
        public int Counter
        {
            get => _counter;
            set => this.RaiseAndSetIfChanged(ref _counter, value);
        }

        // boolean graf
        public ISeries[] TremorSeries { get; set; }

        // číselný graf
        public ISeries[] GaitSeries { get; set; }

        // číselný graf
        public ISeries[] ActivitySeries { get; set; }


        public void ChangeRangeToDay()
        {
            _graphRange = GraphRange.day;
            ActivitySeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddHours(-1).AddSeconds(-60).Ticks,
                        MaxLimit = DateTime.Now.AddHours(-1).AddSeconds(60).Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("HH:mm");
                        },
                        MinStep = TimeSpan.FromHours(4).Ticks,
                        ForceStepToMin = true
                    }
                };

            GaitSeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddDays(-1).Ticks,
                        MaxLimit = DateTime.Now.AddMinutes(10).Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("HH:mm");
                        },
                        MinStep = TimeSpan.FromHours(4).Ticks,
                        ForceStepToMin = true
                    }
                };

            TremorSeriesXAxis = new Axis[]
            {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddHours(-1).AddSeconds(-60).Ticks,
                        MaxLimit = DateTime.Now.AddHours(-1).AddSeconds(60).Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("HH:mm");
                        },
                        MinStep = TimeSpan.FromHours(4).Ticks,
                        ForceStepToMin = true
                    }
            };

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (Sleeplock)
                {
                    Sleeps.Clear();
                    Sleeps = new ObservableCollection<Sleep>();
                    List<Sleep> DaySleep = App.AppData.Sleeps
                        .Where(sleep => DateTime.Now.AddDays(-1) < sleep.StartTime)
                        .OrderBy(sleep => -sleep.EndTime.Ticks)
                        .ToList();

                    for (int i = 0; i < DaySleep.Count; i++)
                    {
                        Sleep sleep = DaySleep[i];
                        Sleeps.Add(sleep);
                    }
                }
            });
        }

        public void ChangeRangeToWeek()
        {
            _graphRange = GraphRange.week;
            ActivitySeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddDays(-7).Ticks,
                        MaxLimit = DateTime.Now.Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("dd.MM");
                        },
                        MinStep = TimeSpan.FromHours(24).Ticks,
                        ForceStepToMin = true
                    }
                };

            GaitSeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddDays(-7).Ticks,
                        MaxLimit = DateTime.Now.Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("dd.MM");
                        },
                        MinStep = TimeSpan.FromHours(24).Ticks,
                        ForceStepToMin = true
                    }
                };

            TremorSeriesXAxis = new Axis[]
    {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddDays(-7).Ticks,
                        MaxLimit = DateTime.Now.Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("dd.MM");
                        },
                        MinStep = TimeSpan.FromHours(24).Ticks,
                        ForceStepToMin = true
                    }
    };
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (Sleeplock)
                {
                    Sleeps.Clear();
                    Sleeps = new ObservableCollection<Sleep>();

                    List<Sleep> DaySleep = App.AppData.Sleeps
                        .Where(sleep => DateTime.Now.AddDays(-7) < sleep.StartTime)
                        .OrderBy(sleep => -sleep.EndTime.Ticks)
                        .ToList();

                    for (int i = 0; i < DaySleep.Count; i++)
                    {
                        Sleep sleep = DaySleep[i];
                        Sleeps.Add(sleep);
                    }
                }
            });
        }





        public void ChangeRangeToMonth()
        {
            _graphRange = GraphRange.month;
            ActivitySeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddMonths(-1).Ticks,
                        MaxLimit = DateTime.Now.Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("dd.MM");
                        },
                        MinStep = TimeSpan.FromDays(7).Ticks,
                        ForceStepToMin = true
                    }
                };

            GaitSeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddMonths(-1).Ticks,
                        MaxLimit = DateTime.Now.Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("dd.MM");
                        },
                        MinStep = TimeSpan.FromDays(7).Ticks,
                        ForceStepToMin = true
                    }
                };

            TremorSeriesXAxis = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = DateTime.Now.AddMonths(-1).Ticks,
                        MaxLimit = DateTime.Now.Ticks,
                        Labeler = value => {
                            if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                            {
                                return string.Empty;
                            }
                            return new DateTime((long)value).ToString("dd.MM");
                        },
                        MinStep = TimeSpan.FromDays(7).Ticks,
                        ForceStepToMin = true
                    }
                };

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (Sleeplock)
                {
                    Sleeps.Clear();
                    Sleeps = new ObservableCollection<Sleep>();

                    List<Sleep> DaySleep = App.AppData.Sleeps
                        .Where(sleep => DateTime.Now.AddMonths(-1) < sleep.StartTime)
                        .OrderBy(sleep => -sleep.EndTime.Ticks)
                        .ToList();

                    for (int i = 0; i < DaySleep.Count; i++)
                    {
                        Sleep sleep = DaySleep[i];
                        Sleeps.Add(sleep);
                    }

                }
            });


        }


    }
}