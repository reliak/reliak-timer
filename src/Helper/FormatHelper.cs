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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliakTimer.Helper
{
    class FormatHelper
    {
        public static string FormatTimeSpan(TimeSpan ts)
        {
            if ((int)ts.TotalSeconds < (int)Math.Ceiling(ts.TotalSeconds))
                ts = TimeSpan.FromSeconds((int)ts.TotalSeconds + 1);

            if (ts.TotalHours >= 1)
                return string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", ts.Hours, ts.Minutes, ts.Seconds);

            return string.Format("{0,2:D2}:{1,2:D2}", ts.Minutes, ts.Seconds);
        }

        public static string GetStateString(TimerState state)
        {
            var formatString = " ({0}{1})";
            var workOrBreak = state.ToString().Contains("Work") ? "work" : "break";
            var paused = state.ToString().Contains("Pause") ? " paused" : "";

            return string.Format(formatString, workOrBreak, paused);
        }
    }
}
