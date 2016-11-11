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
    class MainViewModel : PropertyNotifyObject, IDisposable
    {
        private const string OKImage = "yes.png";
        private const string NOImage = "no.png";
        private const string WelCome = "请刷二维码通行";

        private WGSerialReader _wgReader = null;
        private BardCodeHook BarCode = new BardCodeHook();
        
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

            CheatCall();
        }

        private async void CheatCall()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var qrcode = "run";
                VerifyCloud.PostVerify(qrcode);
            });
            await task;
        }

        private void BarCode_BarCodeEvent(BardCodeHook.BarCodes barCode)
        {
            LogHelper.Info(barCode.BarCode);
        }

        public void ReadBarCode(string qrcode)
        {
            var success = true;
            success = VerifyCloud.PostVerify(qrcode);
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
