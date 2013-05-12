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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReliakTimer
{
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel vm;
        private Settings settingsCopy;

        public SettingsWindow()
        {
            InitializeComponent();

            vm = new SettingsViewModel();
            vm.Settings = Settings.Default;
            vm.FontNames = Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(f => f).ToList();
            this.DataContext = vm;

            this.settingsCopy = SimpleMapper.Map<Settings, Settings>(Settings.Default);
        }

        private void TestSound_Click(object sender, RoutedEventArgs e)
        {
            SoundHelper.PlayAlarm();
        }

        private void TestFlash_Click(object sender, RoutedEventArgs e)
        {
            this.FlashWindow(3, true);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SimpleMapper.Map<Settings,Settings>(settingsCopy, Settings.Default);
            this.Close();
        }

        private void FontReset_Click(object sender, RoutedEventArgs e)
        {
            vm.Settings.TextFontName = "Arial";
            vm.Settings.TextFontSize = 36.ToString();
        }

        private void ColorsReset_Click(object sender, RoutedEventArgs e)
        {
            vm.Settings.WorkTextColor = Colors.Black;
            vm.Settings.BreakTextColor = Colors.Black;
            vm.Settings.WorkBackgroundColor = Colors.Orange;
            vm.Settings.BreakBackgroundColor = Colors.LightGreen;
        }

        private class SettingsViewModel
        {
            public Settings Settings { get; set; }
            public List<string> FontNames { get; set; }
        }
    }
}
