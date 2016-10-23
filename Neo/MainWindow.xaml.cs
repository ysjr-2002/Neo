using Common;
using Common.Dialog;
using HIDSdk;
using NeoVisitor.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace NeoVisitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel vm = null;
        public MainWindow()
        {
            InitializeComponent();
            vm = new Core.MainViewModel();
            this.DataContext = vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int f = (int)'h';
            var j = (int)'t';

            var a = (char)104;
            var c = (char)106;
            AutoRun();
            vm.Init();
        }

        private static void AutoRun()
        {
            var appName = System.Windows.Forms.Application.ProductName;
            var executePath = System.Windows.Forms.Application.ExecutablePath;
            Funs.runWhenStart(false, appName, executePath);
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialog = CustomDialog.Confirm("确认退出系统吗？");
            if (dialog == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            vm.Dispose();
        }
    }
}
