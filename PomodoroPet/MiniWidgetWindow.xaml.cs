using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PomodoroPet.Core;

namespace PomodoroPet;

public partial class MiniWidgetWindow : Window
{
    private readonly PomodoroEngine _engine;
    private readonly Action _restoreMainWindow;
    private readonly DispatcherTimer _blinkTimer;
    private bool _eyesOpen = true;

    public MiniWidgetWindow(PomodoroEngine engine, Action restoreMainWindow)
    {
        InitializeComponent();
        _engine = engine;
        _restoreMainWindow = restoreMainWindow;

        _engine.Tick += OnTick;
        _engine.StateChanged += OnStateChanged;

        _blinkTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.5) };
        _blinkTimer.Tick += (_, _) => Blink();
        _blinkTimer.Start();

        RefreshTime();
        RefreshFace();
    }

    public void SnapToBottomRight()
    {
        var workArea = SystemParameters.WorkArea;
        const double margin = 16;
        Left = workArea.Right - Width - margin;
        Top = workArea.Bottom - Height - margin;
    }

    private void Widget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            _restoreMainWindow();
            return;
        }
        DragMove();
    }

    private void ExpandButton_Click(object sender, RoutedEventArgs e)
    {
        _restoreMainWindow();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void OnTick(TimeSpan remaining)
    {
        RefreshTime();
    }

    private void OnStateChanged(SessionState state)
    {
        RefreshFace();
    }

    private void RefreshTime()
    {
        var remaining = _engine.Remaining;
        TimeLabel.Text = $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
    }

    private void RefreshFace()
    {
        (string face, string tag, string color) = _engine.State switch
        {
            SessionState.Working => ("(•ω•)", "专注", "#FFA9C4"),
            SessionState.ShortBreak => ("(◕‿◕)", "休息", "#9AF0C6"),
            SessionState.LongBreak => ("(◕‿◕)", "长休息", "#9AF0C6"),
            SessionState.Paused => ("(-_-)", "暂停", "#CFCFCF"),
            _ => ("(-.-)", "待机", "#CFCFCF"),
        };
        FaceLabel.Text = face;
        StateTag.Text = tag;
        FaceLabel.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom(color)!;
    }

    private void Blink()
    {
        if (_engine.State == SessionState.Idle || _engine.State == SessionState.Paused) return;

        _eyesOpen = !_eyesOpen;
        string baseFace = _engine.State == SessionState.Working ? "•ω•" : "◕‿◕";
        FaceLabel.Text = _eyesOpen ? $"({baseFace})" : "(-‿-)";
    }
}
