using System;
using System.Media;
using System.Windows;
using PomodoroPet.Core;

namespace PomodoroPet;

public partial class MainWindow : Window
{
    private readonly PomodoroEngine _engine = new();
    private MiniWidgetWindow? _miniWindow;

    public MainWindow()
    {
        InitializeComponent();

        _engine.Tick += OnTick;
        _engine.StateChanged += OnStateChanged;
        _engine.PhaseCompleted += OnPhaseCompleted;

        RefreshTimeLabel();
    }

    private void StartPauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_engine.IsRunning)
        {
            _engine.Pause();
        }
        else
        {
            _engine.Start();
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.Reset();
    }

    private void DurationBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (_engine.State != SessionState.Idle) return;

        if (int.TryParse(WorkMinutesBox.Text, out int work) && work > 0)
        {
            _engine.WorkDuration = TimeSpan.FromMinutes(work);
        }
        if (int.TryParse(BreakMinutesBox.Text, out int rest) && rest > 0)
        {
            _engine.ShortBreakDuration = TimeSpan.FromMinutes(rest);
        }
        _engine.Reset();
    }

    private void ShrinkButton_Click(object sender, RoutedEventArgs e)
    {
        _miniWindow ??= new MiniWidgetWindow(_engine, RestoreMainWindow);
        _miniWindow.Show();
        _miniWindow.SnapToBottomRight();
        Hide();
    }

    private void RestoreMainWindow()
    {
        _miniWindow?.Hide();
        Show();
        Activate();
    }

    private void OnTick(TimeSpan remaining)
    {
        RefreshTimeLabel();
    }

    private void OnStateChanged(SessionState state)
    {
        StateLabel.Text = state switch
        {
            SessionState.Idle => "待机中",
            SessionState.Working => "专注中",
            SessionState.ShortBreak => "短休息",
            SessionState.LongBreak => "长休息",
            SessionState.Paused => "已暂停",
            _ => ""
        };
        StartPauseButton.Content = _engine.IsRunning ? "暂停" : "开始";
        SessionLabel.Text = $"已完成 {_engine.CompletedWorkSessions} 个番茄";
        RefreshTimeLabel();
    }

    private void OnPhaseCompleted(SessionState finishedState)
    {
        SystemSounds.Asterisk.Play();
    }

    private void RefreshTimeLabel()
    {
        var remaining = _engine.Remaining;
        TimeLabel.Text = $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }
}
