/*! Reliak Timer
Copyright (C) 2013  (see AUTHORS file)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
!*/
using ReliakTimer.Helper;
using ReliakTimer.Properties;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ReliakTimer
{
    public partial class MainWindow : Window
    {
        private bool isContextMenuOpen;
        private TimerState timerState;
        private TimerState TimerState
        {
            get { return this.timerState; }
            set { this.timerState = value; this.UpdateColors(); }
        }

        private TheTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            Settings.Default.PropertyChanged += Default_PropertyChanged;
            this.TimerState = TimerState.NotStarted;
            this.DataContext = Settings.Default;

            this.timer = new TheTimer();
            this.timer.Changed += timer_Changed;
            this.timer.Paused += timer_Paused;
            this.timer.Elapsed += timer_Elapsed;

            this.UpdatePanelsVisibility();
            this.UpdateTitleAndLabel();

            this.InitFromSettings();
        }

        private void InitFromSettings()
        {
            if (Settings.Default.WindowSize.Width > 0)
            {
                this.Width = Settings.Default.WindowSize.Width;
                this.Height = Settings.Default.WindowSize.Height;
            }

            if (Settings.Default.StartUpLocation.X > 0 && (Settings.Default.StartUpLocation.X + this.Width) <= SystemParameters.VirtualScreenWidth)
            {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                this.Left = Settings.Default.StartUpLocation.X;
                this.Top = Settings.Default.StartUpLocation.Y;
            }

            if (Settings.Default.AutoStartTimer)
                StartWorkTimer(false);

            if (Settings.Default.SnapWindow)
                this.EnableSnapBehavior(true);
        }

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "SnapWindow"))
                this.EnableSnapBehavior(Settings.Default.SnapWindow);
            else if (string.Equals(e.PropertyName, "UseCompactMode"))
                this.UpdateTitleAndLabel();
            else if (e.PropertyName.Contains("Color"))
                this.UpdateColors();
            else if (string.Equals(e.PropertyName, "ShowStarterInfo"))
                this.UpdatePanelsVisibility();
        }

        private void UpdateTitleAndLabel()
        {
            var tsText = FormatHelper.FormatTimeSpan(TimeSpan.FromMinutes(Settings.Default.WorkTimeInMinutes));

            if (this.timerState == ReliakTimer.TimerState.NotStarted)
                this.lblMain.Content = tsText;
            else if (this.timerState == ReliakTimer.TimerState.WorkTimeCompleted)
            {
                this.Title = "Time is up!";

                if (Settings.Default.BreakTimeInMinutes > 0)
                    this.lblMain.Content = FormatHelper.FormatTimeSpan(TimeSpan.FromMinutes(Settings.Default.BreakTimeInMinutes));
                else
                    this.lblMain.Content = tsText;
            }
            else if (this.timerState == ReliakTimer.TimerState.BreakTimeCompleted)
            {
                this.Title = "Break is over!";
                this.lblMain.Content = tsText;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.StopFlashingWindow();
        }

        private void TimerArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !isContextMenuOpen)
                MoveToNextState();
        }

        private void MoveToNextState()
        {
            if (this.TimerState == TimerState.NotStarted)
            {
                StartWorkTimer(false);
            }
            // worktime stuff
            else if (this.TimerState == TimerState.WorkTimeRunning)
            {
                this.Pause(TimerState.WorkTimePaused);
            }
            else if (this.TimerState == TimerState.WorkTimePaused)
            {
                this.UnPause(TimerState.WorkTimeRunning);
            }
            else if (this.TimerState == TimerState.WorkTimeCompleted)
            {
                if (Settings.Default.BreakTimeInMinutes > 0)
                    StartBreakTimer(false);
                else
                    StartWorkTimer(false);
            }
            // breaktime stuff
            else if (this.TimerState == TimerState.BreakTimeRunning)
            {
                this.Pause(TimerState.BreakTimePaused);
            }
            else if (this.TimerState == TimerState.BreakTimePaused)
            {
                this.UnPause(TimerState.BreakTimeRunning);
            }
            else if (this.TimerState == TimerState.BreakTimeCompleted)
            {
                StartWorkTimer(false);
            }
        }

        private void UpdateColors()
        {
            if (this.timerState == ReliakTimer.TimerState.NotStarted || this.timerState == ReliakTimer.TimerState.WorkTimeRunning
                || this.timerState == ReliakTimer.TimerState.WorkTimePaused || this.timerState == ReliakTimer.TimerState.BreakTimeCompleted)
            {
                this.timerArea.Background = new SolidColorBrush(Settings.Default.WorkBackgroundColor);
                this.lblMain.Foreground = new SolidColorBrush(Settings.Default.WorkTextColor);
            }
            else if (this.timerState.ToString().Contains("Break") || (this.timerState == ReliakTimer.TimerState.WorkTimeCompleted && Settings.Default.BreakTimeInMinutes > 0))
            {
                this.timerArea.Background = new SolidColorBrush(Settings.Default.BreakBackgroundColor);
                this.lblMain.Foreground = new SolidColorBrush(Settings.Default.BreakTextColor);
            }
        }
        
        void timer_Paused(object sender, TimerEventArgs e)
        {
            this.OnTimerChanged(e);
        }

        void timer_Changed(object sender, TimerEventArgs e)
        {
            this.OnTimerChanged(e);
        }

        private void OnTimerChanged(TimerEventArgs e)
        {
            var tsFormat = FormatHelper.FormatTimeSpan(e.RemainingTime);
            this.Title = tsFormat + FormatHelper.GetStateString(this.timerState);
            this.lblMain.Content = tsFormat;
        }

        void timer_Elapsed(object sender, EventArgs e)
        {
            if (this.TimerState == TimerState.WorkTimeRunning)
            {
                ShowNotifications();

                if (Settings.Default.BreakTimeInMinutes > 0 && Settings.Default.LoopTimer)
                {
                    StartBreakTimer(true);
                    return;
                }

                this.TimerState = TimerState.WorkTimeCompleted;
                this.UpdateTitleAndLabel();
            }
            else if (this.TimerState == TimerState.BreakTimeRunning)
            {
                ShowNotifications();

                if (Settings.Default.LoopTimer)
                {
                    StartWorkTimer(true);
                    return;
                }
                
                this.TimerState = TimerState.BreakTimeCompleted;
                this.UpdateTitleAndLabel();
            }
        }

        private void ShowNotifications()
        {
            if (Settings.Default.FlashWindowWhenComplete)
                this.FlashWindow(this.IsActive ? 2u : 3u, true);

            if (Settings.Default.PlaySoundWhenComplete)
                SoundHelper.PlayAlarm();

            if (Settings.Default.PopupWindowWhenComplete)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.Activate();
            }
        }

        private void StartWorkTimer(bool isLoopCall)
        {
            StartTimer(Settings.Default.WorkTimeInMinutes, TimerState.WorkTimeRunning, isLoopCall);
        }

        private void StartBreakTimer(bool isLoopCall)
        {
            StartTimer(Settings.Default.BreakTimeInMinutes, TimerState.BreakTimeRunning, isLoopCall);
        }

        private void StartTimer(int minutes, TimerState state, bool isLoopCall)
        {
            this.timer.Start(TimeSpan.FromMinutes(minutes));
            this.TimerState = state;

            if (!isLoopCall && Settings.Default.MinimizeOnTimerStart)
                this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Pause(TimerState state)
        {
            this.timer.Pause();
            this.TimerState = state;
        }

        private void UnPause(TimerState state)
        {
            this.timer.UnPause();
            this.TimerState = state;
        }

        private void SetLabelAndTitle(string s)
        {
            this.Title = s;
            this.lblMain.Content = s;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            Settings.Default.StartUpLocation = new System.Drawing.Point((int)this.Left, (int)this.Top);
            Settings.Default.WindowSize = new Size(this.Width, this.Height);
            Settings.Default.Save();
        }

        private void SettingsLink_Click(object sender, RoutedEventArgs e)
        {
            this.ShowDialog(new SettingsWindow());
        }

        private void AboutLink_Click(object sender, RoutedEventArgs e)
        {
            this.ShowDialog(new AboutWindow());
        }

        private void ShowDialog(Window w)
        {
            w.Owner = this;
            w.ShowDialog();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if( e.Key == Key.Space )
                MoveToNextState();
        }

        private void MenuItemStartWork_Click(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.StartWorkTimer(false);
        }

        private void MenuItemStartBreak_Click(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.StartBreakTimer(false);
        }

        private void MenuItemUseCompactMode_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility();
        }

        private void UpdatePanelsVisibility()
        {
            this.pseudoToolbar.Visibility = Settings.Default.UseCompactMode ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.pnlStarterInfo.Visibility = Settings.Default.ShowStarterInfo ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var vis = Settings.Default.UseCompactMode ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.ctxSettings.Visibility = vis;
            this.ctxAbout.Visibility = vis;
            this.ctxSep.Visibility = vis;

            this.isContextMenuOpen = true;
        }

        private void ctxSettings_Click(object sender, RoutedEventArgs e)
        {
            this.ShowDialog(new SettingsWindow());
        }

        private void ctxAbout_Click(object sender, RoutedEventArgs e)
        {
            this.ShowDialog(new AboutWindow());
        }

        private void timerArea_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int proposedValue = int.Parse(Settings.Default.TextFontSize) + (e.Delta > 0 ? 2 : -2);

            Settings.Default.TextFontSize = proposedValue > 98 ? 98.ToString() : (proposedValue < 12 ? 12 : proposedValue).ToString();
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            this.isContextMenuOpen = false;
        }

        private void DontShowStarterInfoAgain_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowStarterInfo = false;
            this.pnlStarterInfo.Visibility = System.Windows.Visibility.Collapsed;
        }        
    }
}
