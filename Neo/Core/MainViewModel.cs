using Common;
using Common.Log;
using Common.NotifyBase;
using SR104;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NeoVisitor.Core
{
    class MainViewModel : PropertyNotifyObject
    {
        private const string OKImage = "yes.png";
        private const string NOImage = "no.png";
        private const string WelCome = "请刷二维码通行";

        private WGSerialReader _wgReader = null;
        private BardCodeHook BarCode = new BardCodeHook();
        private DispatcherTimer _updateTimer = null;
        private string[] _rebootWeekofday = null;
        private const int Delay = 1000;
        private FuncTimeout _timeout = null;

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

        public string QRCode
        {
            get { return this.GetValue(s => s.QRCode); }
            set { this.SetValue(s => s.QRCode, value); }
        }

        public void Init()
        {
            if (ConfigProfile.Instance.VirtualPort.ToLower() == "none")
            {
                BarCode.BarCodeEvent += new BardCodeHook.BardCodeDeletegate(BarCode_BarCodeEvent);
                //BarCode.Start();
            }
            else
            {
                _wgReader = new WGSerialReader();
                _wgReader.SetQRCodeCallback(ReadBarCode);
                _wgReader.Open(ConfigProfile.Instance.VirtualPort);
            }

            SRController.OpenPort(ConfigProfile.Instance.SwitchPort);
            _timeout = new FuncTimeout();

            VerfiyMessage = WelCome;

            _rebootWeekofday = ConfigProfile.Instance.RebootWeekofDay.Split(',');
            _updateTimer = new DispatcherTimer();
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

        private void BarCode_BarCodeEvent(BardCodeHook.BarCodes barCode)
        {
            LogHelper.Info(barCode.BarCode);
        }

        public void ReadBarCode(string qrcode)
        {
            var success = true;
            //success = VerifyCloud.Verify(qrcode);
            if (success)
            {
                StateImage = "yes.png";
                VerfiyMessage = "请通行";
                OpenGate();
            }
            else
            {
                StateImage = "no.png";
                VerfiyMessage = "授权失败";
            }

            _timeout.StartOnce(2000, () =>
            {
                StateImage = "";
                VerfiyMessage = WelCome;
            });
        }

        private void OpenGate()
        {
            SRController.Close(1);
            Thread.Sleep(Delay);
            SRController.Free(1);
        }

        public void Dispose()
        {
            BarCode.Stop();
            SRController.ClosePort();
            if (_wgReader != null)
            {
                //关闭虚拟串口
                _wgReader.Dispose();
            }
        }
    }
}
