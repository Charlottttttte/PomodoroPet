using System;
using System.Windows.Threading;

namespace PomodoroPet.Core
{
    public enum SessionState
    {
        Idle,
        Working,
        ShortBreak,
        LongBreak,
        Paused
    }

    public class PomodoroEngine
    {
        public TimeSpan WorkDuration { get; set; } = TimeSpan.FromMinutes(25);
        public TimeSpan ShortBreakDuration { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan LongBreakDuration { get; set; } = TimeSpan.FromMinutes(15);
        public int SessionsBeforeLongBreak { get; set; } = 4;

        private readonly DispatcherTimer _timer;
        private TimeSpan _remaining;
        private SessionState _stateBeforePause = SessionState.Idle;

        public SessionState State { get; private set; } = SessionState.Idle;
        public TimeSpan Remaining => _remaining;
        public int CompletedWorkSessions { get; private set; } = 0;

        public event Action<TimeSpan>? Tick;
        public event Action<SessionState>? StateChanged;
        public event Action<SessionState>? PhaseCompleted;

        public PomodoroEngine()
        {
            _remaining = WorkDuration;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += OnTick;
        }

        public bool IsRunning => _timer.IsEnabled;

        public void Start()
        {
            if (State == SessionState.Paused)
            {
                State = _stateBeforePause;
            }
            else if (State == SessionState.Idle)
            {
                State = SessionState.Working;
                _remaining = WorkDuration;
            }
            else
            {
                return;
            }

            _timer.Start();
            StateChanged?.Invoke(State);
            Tick?.Invoke(_remaining);
        }

        public void Pause()
        {
            if (!_timer.IsEnabled) return;
            _timer.Stop();
            _stateBeforePause = State;
            State = SessionState.Paused;
            StateChanged?.Invoke(State);
        }

        public void Reset()
        {
            _timer.Stop();
            State = SessionState.Idle;
            CompletedWorkSessions = 0;
            _remaining = WorkDuration;
            StateChanged?.Invoke(State);
            Tick?.Invoke(_remaining);
        }

        private void OnTick(object? sender, EventArgs e)
        {
            _remaining -= TimeSpan.FromSeconds(1);
            if (_remaining <= TimeSpan.Zero)
            {
                AdvancePhase();
            }
            else
            {
                Tick?.Invoke(_remaining);
            }
        }

        private void AdvancePhase()
        {
            var finishedState = State;
            PhaseCompleted?.Invoke(finishedState);

            if (finishedState == SessionState.Working)
            {
                CompletedWorkSessions++;
                bool longBreak = CompletedWorkSessions % SessionsBeforeLongBreak == 0;
                State = longBreak ? SessionState.LongBreak : SessionState.ShortBreak;
                _remaining = longBreak ? LongBreakDuration : ShortBreakDuration;
            }
            else
            {
                State = SessionState.Working;
                _remaining = WorkDuration;
            }

            StateChanged?.Invoke(State);
            Tick?.Invoke(_remaining);
        }
    }
}
