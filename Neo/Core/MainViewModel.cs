using Common;
using Common.Log;
using Common.NotifyBase;
using SR104;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NeoVisitor.Core
{
    class MainViewModel : PropertyNotifyObject
    {
        private const string OKImage = "yes.png";
        private const string NOImage = "no.png";
        private const string WelCome = "请刷二维码通行";

        private BardCodeHooK BarCode = new BardCodeHooK();
        private DispatcherTimer _updateTimer = null;
        private string[] _rebootWeekofday = null;

        public string StateImage
        {
            get { return this.GetValue(s => s.StateImage); }
            set { this.SetValue(s => s.StateImage, value); }
        }

        public string VerfiyMessage
        {
            get { return this.GetValue(s => s.VerfiyMessage); }
            set { this.SetValue(s => s.VerfiyMessage, value); }
        }

        public void Init()
        {
            BarCode.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCode.Start();

            SRController.OpenPort(ConfigProfile.Instance.InPortName);

            StateImage = "yes.png";
            VerfiyMessage = WelCome;

            _updateTimer = new DispatcherTimer();
            _rebootWeekofday = ConfigProfile.Instance.RebootWeekofDay.Split(',');
            _updateTimer.Interval = TimeSpan.FromSeconds(1);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            RebootMachine();
        }

        private void RebootMachine()
        {
            int day = (int)DateTime.Now.DayOfWeek;
            if (_rebootWeekofday.Contains(day.ToString()))
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

        private void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            LogHelper.Info(barCode.BarCode);
        }

        public void Dispose()
        {
            BarCode.Stop();
            SRController.ClosePort();
        }
    }
}
