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
using System.Reflection;
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
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            var asm = Assembly.GetExecutingAssembly();
            var vm = new AboutViewModel();
            vm.Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            vm.Title = asm.GetCustomAttribute<AssemblyProductAttribute>().Product;
            vm.Version = asm.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            vm.Url = "https://sourceforge.net/projects/reliak-timer";
            vm.UrlSource = "https://github.com/reliak/reliak-timer";

            this.DataContext = vm;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Hyperlink_RequestNavigate_1(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                this.Close();
        }

        private class AboutViewModel
        {
            public string Author { get; set; }
            public string Url { get; set; }
            public string UrlSource { get; set; }
            public string Copyright { get; set; }
            public string Title { get; set; }
            public string Version { get; set; }
        }
    }
}
