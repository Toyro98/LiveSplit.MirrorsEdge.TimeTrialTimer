using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class TimeTrialTimerComponent : IComponent
    {
        protected InfoTextComponent InternalComponent { get; set; }
        public TimeTrialTimerSettings Settings { get; set; }
        protected LiveSplitState CurrentState { get; set; }

        private Process _process { get; set; }
        private int _splitIndex;
        private float _time;
        private float _oldValue;
        private float _currentValue;
        private bool _updateTextFromEvent;

        public string ComponentName => "Time Trial Timer";

        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumHeight => InternalComponent.MinimumHeight;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumWidth => InternalComponent.MinimumHeight;

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

        public TimeTrialTimerComponent(LiveSplitState state)
        {
            Settings = new TimeTrialTimerSettings();
            InternalComponent = new InfoTextComponent("Time Trial Time", "0.00");

            state.OnStart += State_OnStart;
            state.OnReset += State_OnReset;

            CurrentState = state;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            InternalComponent.NameLabel.HasShadow = InternalComponent.ValueLabel.HasShadow = state.LayoutSettings.DropShadows;
            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;
            InternalComponent.NameLabel.HasShadow = InternalComponent.ValueLabel.HasShadow = state.LayoutSettings.DropShadows;
            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (state.CurrentPhase != TimerPhase.Running)
            {
                if (_updateTextFromEvent)
                {
                    _updateTextFromEvent = false;
                    InternalComponent.Update(invalidator, state, width, height, mode);
                }

                return;
            }

            if (state.CurrentSplitIndex == _splitIndex)
            {
                return;
            }

            if (_process != null && !_process.HasExited)
            {
                _currentValue = Settings.TimePointer.Deref<float>(_process);

                if (_currentValue != 0f && _currentValue != _oldValue)
                {
                    _oldValue = _currentValue;
                    _time += _currentValue;

                    UpdateTimerText(_time);
                }
            }
            else
            {
                _process = Process.GetProcessesByName("MirrorsEdge").FirstOrDefault();
                Settings.GetGameVersion(_process);
            }

            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            _updateTextFromEvent = true;
            UpdateTimerText(0f);
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
            _updateTextFromEvent = true;
            UpdateTimerText(0f);
        }

        private void UpdateTimerText(float time)
        {
            _time = time;
            _splitIndex = CurrentState.CurrentSplitIndex;

            InternalComponent.InformationValue = Settings.Formatter.Format(TimeSpan.FromSeconds(_time));
        }

        public void Dispose()
        {
            CurrentState.OnStart -= State_OnStart;
            CurrentState.OnReset -= State_OnReset;
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
