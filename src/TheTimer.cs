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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ReliakTimer
{
    class TheTimer
    {
        public event EventHandler Elapsed;
        public event EventHandler<TimerEventArgs> Changed;
        public event EventHandler<TimerEventArgs> Paused;

        private Thread workerThread;
        private Dispatcher dispatcher;
        private DateTime startTimeUtc;
        private TimeSpan duration;
        private bool isPaused;

        public bool IsPaused
        {
            get { return this.isPaused; }
        }

        public TheTimer()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Start(TimeSpan duration)
        {
            this.isPaused = false;
            this.duration = duration;
            this.startTimeUtc = DateTime.UtcNow;

            this.workerThread = new Thread(DoWork);
            this.workerThread.IsBackground = true;
            this.workerThread.Start();
        }

        public void Pause()
        {
            this.isPaused = true;
        }

        public void UnPause()
        {
            this.Start(this.duration);
        }

        public void Stop()
        {
            if (this.workerThread != null)
            {
                this.workerThread.Abort();
                this.workerThread = null;
            }
        }

        private void DoWork()
        {
            while (!this.isPaused)
            {
                var diff = DateTime.UtcNow - startTimeUtc;
                var hasElapsed = diff.Ticks > duration.Ticks;

                this.dispatcher.Invoke(new Action(() =>
                    {
                        if (hasElapsed)
                        {
                            if (this.Elapsed != null)
                                this.Elapsed(this, EventArgs.Empty);
                        }
                        else if (this.Changed != null)
                            this.Changed(this, new TimerEventArgs(duration - diff));
                    }));

                if (hasElapsed)
                    return;

                Thread.Sleep(100);
            }

            this.duration = this.duration - (DateTime.UtcNow - startTimeUtc);

            if (this.Paused != null)
            {
                this.dispatcher.Invoke(new Action(() => this.Paused(this, new TimerEventArgs(this.duration))));
            }
        }
    }

    public class TimerEventArgs : EventArgs
    {
        public TimeSpan RemainingTime { get; private set; }

        public TimerEventArgs(TimeSpan remainingTime)
        {
            this.RemainingTime = remainingTime;
        }
    }
}
