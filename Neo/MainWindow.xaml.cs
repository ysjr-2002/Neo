using Common;
using Common.Dialog;
using Common.Log;
using NeoVisitor.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NeoVisitor
{
    /// <summary>
    /// 主控系统
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm = null;
        private FuncTimeout timeout = null;
        private DispatcherTimer _updateTimer = null;
        private string[] _rebootWeekofday = null;

        public MainWindow()
        {
            InitializeComponent();
            vm = new Core.MainViewModel();
            this.DataContext = vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timeout = new FuncTimeout();
            AutoRun();
            vm.Init();

            _rebootWeekofday = ConfigProfile.Instance.RebootWeekofDay.Split(',');
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(1);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            txtBarcode.Focus();
            RebootMachine();
        }

        private void RebootMachine()
        {
            int day = (int)DateTime.Now.DayOfWeek;
            if (_rebootWeekofday.Length < day)
                return;

            if (_rebootWeekofday[day] == "1")
            {
                var nowTime = DateTime.Now.ToString("HH:mm");
                if (nowTime == ConfigProfile.Instance.RebootTime)
                {
                    _updateTimer.Stop();
                    LogHelper.Info("执行重启计划 {0}", nowTime);
                    RebootMachineAPI.ExitWindow();
                }
            }
        }

        private void AutoRun()
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

        private void txtBarcode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtBarcode.Text.Length > 0)
                {
                    var qrcode = txtBarcode.Text;
                    Task.Factory.StartNew(() =>
                    {
                        vm.ReadBarCode(qrcode);
                        vm.QRCode = "";
                    });
                }
            }
        }
    }
}
