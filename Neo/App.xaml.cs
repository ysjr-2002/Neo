using Common.Log;
using NeoVisitor;
using NeoVisitor.config;
using NeoVisitor.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Windows;

namespace Neo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var appName = "neo";
            var createNew = false;
            var mutex = new Mutex(true, appName, out createNew);
            if (createNew)
            {
                Channels.Load();
                ConfigProfile.Instance.ReadConfig();
                MainWindow window = new MainWindow();
                window.Show();
                base.OnStartup(e);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
