using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Nebula
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStart(object sender, StartupEventArgs e)
        {
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
        }

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            
        }
    }
}