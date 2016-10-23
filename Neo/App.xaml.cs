using NeoVisitor;
using NeoVisitor.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
            ConfigProfile.Instance.ReadConfig();

            MainWindow window = new MainWindow();
            window.Show();
            base.OnStartup(e);
        }
    }
}
